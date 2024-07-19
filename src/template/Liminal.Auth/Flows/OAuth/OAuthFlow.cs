// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Abstractions;
using Liminal.Auth.Flows.OAuth.Providers;
using Liminal.Auth.Models;
using Liminal.Auth.Results;

namespace Liminal.Auth.Flows.OAuth;

public class OAuthFlow<TUser>(
    IOAuthProvidersProvider providersCollection,
    IStateGenerator stateGenerator,
    IUserFactory<TUser> userFactory,
    IUserStore<TUser> userStore,
    IPasswordStore passwordStore,
    IAccountStore accountStore): IAuthFlow
    where TUser : AbstractUser
{
    public string Name { get; } = "oauth";

    // Got idea from supabase auth. GetExternalProviderRedirectUrl also gets an optional user as a linking target.
    public Task<string> GetRedirectUrl(string providerName, string redirectAfter = "/", Guid? linkingTargetId = null)
    {
        var provider = providersCollection.GetProvider(providerName);
        var state = stateGenerator.GenerateState(providerName, redirectAfter, linkingTargetId);
        return provider.GetRedirectUrl(state);
    }

    public async Task<CallbackResult<TUser>> Callback(string providerName, string code, string? state)
    {
        var provider = providersCollection.GetProvider(providerName);

        var signInResult = await provider.SignInOAuthAsync(code, state);

        if (!signInResult.IsSuccess)
        {
            return CallbackResult<TUser>.Failure(signInResult.FailureMessage ?? "Cannot sign in using provider.");
        }

        Guid? targetUserId = null;

        if (!string.IsNullOrWhiteSpace(state))
        {
            var parsedState = await stateGenerator.ParseState(state);
            signInResult.RedirectAfter = parsedState.RedirectAfter;
            targetUserId = parsedState.TargetUserId;
        }

        var existingAccount = await accountStore.GetByProviderAsync(signInResult.User.Email, providerName);
        TUser? existingUser = null;

        // If account does not exist
        // Then we get the user.
        if (existingAccount is null)
        {
            // If target user then we need to get him.
            if (targetUserId is not null)
            {
                existingUser = await GetTargetUserUsingTargetIdAsync(targetUserId);

                if (existingUser is null)
                {
                    return CallbackResult<TUser>.Failure("No user to link.");
                }

                if (!existingUser.IsConfirmed)
                {
                    return CallbackResult<TUser>.Failure("Cannot connect to existing user because user is not confirmed");
                }

                existingAccount =
                    Account.Create(
                        $"{this.Name}_{providerName}",
                        existingUser.Email,
                        signInResult.User.IsVerified,
                        existingUser.Id);

                var result = await accountStore.AddAsync(existingAccount, true);

                if (!result)
                {
                    return CallbackResult<TUser>.Failure("Cannot save new account.");
                }

                await this.StoreTokens(signInResult, existingAccount);

                return CallbackResult<TUser>
                    .Success(existingUser.Email, existingUser.ToPrincipal(), signInResult.RedirectAfter);
            }

            // I assume that if hacker has access to OAuth account then it is not my responsibility
            // to check validity of the external session.
            // Ignore that. May send confirmation email later?

            // If no target user.
            existingUser = await userStore.GetByEmailAsync(signInResult.User.Email);

            // If user does not exist or is not confirmed
            // then we create new user
            // We do not allow unconfirmed account linking.
            if (existingUser is null || !existingUser.IsConfirmed)
            {
                // Create new user for not confirmed account.
                if (signInResult.User.IsVerified)
                {
                    existingUser = userFactory.CreateConfirmed(signInResult.User.Email, signInResult.User.UserName);
                }
                else
                {
                    existingUser = userFactory.CreateUnConfirmed(signInResult.User.Email, signInResult.User.UserName);
                }

                await userStore.AddAsync(existingUser, true);
            }

            existingAccount = Account.Create($"{this.Name}_{providerName}", existingUser.Email, signInResult.User.IsVerified, existingUser.Id);
            await accountStore.AddAsync(existingAccount, true);

            await this.StoreTokens(signInResult, existingAccount);

            return CallbackResult<TUser>.Success(
                existingUser.Email,
                existingUser.ToPrincipal(),
                signInResult.RedirectAfter);
        }

        // We do not care if the existingAccount already has userId.
        // TODO: Check if account is linked to User 1 to 1.
        // If 1 to 1 then we can do this if not then we cannot.
        // But in such a case we need to delete old one.
        // For now let's not care.
        existingUser = await this.GetTargetUserUsingTargetIdAsync(targetUserId);

        // In case that account exists but user does not.
        // This is redundant but i am going to keep it.
        existingUser ??= await userStore.GetByIdAsync(existingAccount.UserId);

        if (existingUser is null)
        {
            throw new Exception("Existing user is null. Account is orphan!!!");
        }

        // If we did not try intentionally connect accounts to
        // not confirmed user
        // We can have 2 ways:
        // 1. Connect automatically since this is a 99% real user.
        // 2. Fuck this 1% hacker.
        // I am going to connect and confirm then.
        if (!existingUser.IsConfirmed && targetUserId is not null)
        {
            // It means that the account tries to link to not confirmed user
            // Do not allow that.
            return CallbackResult<TUser>.Failure("Cannot link to a not confirmed user.");
        }

        // NEVER ROLL YOUR OWN AUTH.
        // TODO: Check that the user is generated specifically for this account.
        // Connect user and account and confirm if it is not.
        if (existingUser.Id != existingAccount.UserId)
        {
            existingAccount.UserId = existingUser.Id;
            await accountStore.UpdateAsync(existingAccount, true);
        }

        if (!existingUser.IsConfirmed)
        {
            existingUser.Confirm();
            await userStore.UpdateAsync(existingUser, true);
        }

        await this.StoreTokens(signInResult, existingAccount);

        var existingPrincipal = existingUser.ToPrincipal();

        return CallbackResult<TUser>.Success(existingUser.Email, existingPrincipal, signInResult.RedirectAfter);
    }

    public async Task<OAuthTokensResult> GetTokensForProvider(Guid accountId, bool autoRefresh = false)
    {
        var refreshToken = await passwordStore.GetByAccountIdAsync(accountId, "refresh_token");
        var accessToken = await passwordStore.GetByAccountIdAsync(accountId, "access_token");

        if (accessToken is null)
        {
            return OAuthTokensResult.Failure("Cannot find access token");
        }

        var providerName = accessToken.Provider;

        if (accessToken.ValidUntil > DateTimeOffset.UtcNow)
        {
            if (refreshToken is null)
            {
                return OAuthTokensResult.Failure("Session expired and could find refresh token");
            }

            var provider = providersCollection.GetProvider(accessToken.Provider);

            var newTokenSet = await provider.RefreshTokenAsync(refreshToken.TokenValue);

            await this.StoreTokens(newTokenSet, providerName, accountId);
            return OAuthTokensResult.Success(newTokenSet.AccessToken, newTokenSet.RefreshToken);
        }

        return OAuthTokensResult.Success(accessToken.TokenValue, refreshToken?.TokenValue);
    }

    private async Task StoreTokens(TokenSet tokens, string provider, Guid accountId)
    {
        // Store tokens and get user roles and claims
        if (tokens.RefreshToken is not null)
        {
            await this.SetTokenAsync(
                provider,
                "refresh_token",
                tokens.RefreshToken,
                accountId,
                tokens.AccessTokenValidUntil);
        }

        await this.SetTokenAsync(
            provider,
            "access_token",
            tokens.AccessToken,
            accountId,
            tokens.AccessTokenValidUntil);

        await passwordStore.SaveChangesAsync();
    }

    private async Task StoreTokens(OAuthSignInResult signInResult, Account account)
    {
        var tokens = signInResult.Tokens;

        // Store tokens and get user roles and claims
        if (tokens.RefreshToken is not null)
        {
            await this.SetTokenAsync(
                signInResult.Provider,
                "refresh_token",
                tokens.RefreshToken,
                account.Id,
                tokens.AccessTokenValidUntil);
        }

        await this.SetTokenAsync(
            signInResult.Provider,
            "access_token",
            tokens.AccessToken,
            account.Id,
            tokens.AccessTokenValidUntil);

        await passwordStore.SaveChangesAsync();
    }

    private async Task SetTokenAsync(string provider, string tokenName, string tokenValue, Guid accountId, DateTimeOffset? expiryDate = null)
    {
        var existingToken = await passwordStore.GetByAccountIdAsync(accountId, tokenName);

        if (existingToken is null)
        {
            existingToken = AccountToken.Create(
                accountId,
                provider,
                tokenName,
                tokenValue,
                expiryDate);

            await passwordStore.AddAsync(existingToken, true);
            return;
        }

        existingToken.TokenValue = tokenValue;

        await passwordStore.UpdateAsync(existingToken, true);
    }

    private async Task<TUser?> GetTargetUserUsingTargetIdAsync(Guid? targetUserId)
    {
        TUser? existingUser = null;

        if (targetUserId is null)
        {
            return existingUser;
        }

        existingUser = await userStore.GetByIdAsync(targetUserId.Value);

        if (existingUser is null)
        {
            throw new Exception("User id is present in the state but not in DB.");
        }
        else if (!existingUser.IsConfirmed)
        {
            throw new Exception("User is not confirmed but present as a target in the state.");
        }

        return existingUser;
    }
}
