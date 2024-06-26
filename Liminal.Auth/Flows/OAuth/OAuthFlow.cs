using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;
using Liminal.Auth.Results;

namespace Liminal.Auth.Flows.OAuth;

public class OAuthFlow<TUser>(
    OAuthProvidersProvider providersCollection,
    StateGenerator stateGenerator,
    IUserStore<TUser> userStore, 
    IPasswordStore passwordStore,
    IClaimsStore claimsStore,
    IAccountStore accountStore) : IAuthFlow 
    where TUser : AbstractUser
{
    public string Name { get; } = "oauth";
    
    public Task<string> GetRedirectUrl(string providerName, string redirectAfter = "/")
    {
        var provider = providersCollection.GetProvider(providerName);
        var state = stateGenerator.GenerateState(providerName, redirectAfter);
        return provider.GetRedirectUrl(state);
    }

    public async Task<CallbackResult<TUser>> Callback(string providerName, string code, string? state, Func<TUser> userFactory)
    {
        var provider = providersCollection.GetProvider(providerName);

        var signInResult = await provider.SignInOAuthAsync(code, state);

        if (!string.IsNullOrWhiteSpace(state))
        {
            var parsedState = await stateGenerator.ParseState(state);
            signInResult.RedirectAfter = parsedState.RedirectAfter;
        }

        if (!signInResult.IsSuccess)
        {
           return CallbackResult<TUser>.Failure(signInResult.FailureMessage ?? "Cannot sign in using provider.");
        }
        
        var existingUser = await userStore.GetByEmailAsync(signInResult.Email);

        var existingAccount = await accountStore.GetByProviderAsync(signInResult.Email, providerName);

        if (existingUser is null)
        {
            existingUser = userFactory();
            existingUser.Id = Guid.NewGuid();
            existingUser.Email = signInResult.Email;
            existingUser.IsConfirmed = true;
            await userStore.AddAsync(existingUser, true);
        }
        else if(!existingUser.IsConfirmed)
        {
            existingUser.IsConfirmed = true;
            await userStore.UpdateAsync(existingUser, true);
        }

        if (existingAccount is null)
        {
            existingAccount = new Account()
            {
                Provider = providerName,
                UserId = existingUser.Id,
                Id = Guid.NewGuid(),
                Email = signInResult.Email,
                IsConfirmed = true,
            };
            
            await accountStore.AddAsync(existingAccount, true);
            await claimsStore.AddRangeAsync(signInResult.Claims, existingUser.Id, existingAccount.Id, providerName, true);
        }
        
        await StoreTokens(signInResult, existingAccount);

        var existingPrincipal = existingUser.ToPrincipal();

        return CallbackResult<TUser>.Success(existingUser.Email, existingPrincipal, signInResult?.RedirectAfter);
    }
    
    public async Task<OAuthTokensResult> GetTokensForProvider(Guid accountId, bool autoRefresh = false)
    {
        var refreshToken = await passwordStore.GetByAccountIdAsync(accountId, "refresh_token");
        var accessToken = await passwordStore.GetByAccountIdAsync(accountId, "access_token");

        return new OAuthTokensResult()
        {
            AccessToken = accessToken?.TokenValue,
            RefreshToken = refreshToken?.TokenValue
        };
    }

    private async Task StoreTokens(OAuthSignInResult signInResult, Account account)
    {
        // Store tokens and get user roles and claims
        if (signInResult.RefreshToken is not null)
        {
            await SetTokenAsync(signInResult.Provider, "refresh_token", signInResult.RefreshToken, account.Id);
        }
        
        await SetTokenAsync(signInResult.Provider, "access_token", signInResult.AccessToken, account.Id);

        await passwordStore.SaveChangesAsync();
    }

    private async Task SetTokenAsync(string provider, string tokenName, string tokenValue, Guid accountId)
    {
        var existingToken = await passwordStore.GetByAccountIdAsync(accountId, "refresh_token");

        if (existingToken is null)
        {
            existingToken = AccountToken.Create(accountId, provider, tokenName,
                tokenValue);

            await passwordStore.AddAsync(existingToken);
            return;
        }

        existingToken.TokenValue = tokenValue;

        await passwordStore.UpdateAsync(existingToken);
    }
}