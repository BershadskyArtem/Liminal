using System.Security.Claims;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;

namespace Liminal.Auth.Flows.OAuth;

public class OAuthFlow<TUser>(
    OAuthProvidersProvider providersCollection,
    StateGenerator stateGenerator,
    IUserStore<TUser> userStore, 
    IPasswordStore passwordStore,
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

        if (!signInResult.IsSuccess)
        {
           return CallbackResult<TUser>.Failure();
        }
        
        var existingUser = await userStore.GetUserByEmailAsync(signInResult.Email);

        var existingAccount = await accountStore.GetByProviderAsync(signInResult.Email, providerName);

        // Generate claims principal 
        var principal = CreateClaimsPrincipal(signInResult);
        
        if (existingUser is not null)
        {
            existingAccount ??= new Account()
            {
                Provider = providerName,
                UserId = existingUser.Id,
                Id = Guid.NewGuid()
            };
            
            await StoreTokens(signInResult, existingUser, existingAccount);

            return CallbackResult<TUser>.Success(existingUser.Email, principal);
        }
        
        // Register user 
        var user = userFactory();

        user.Id = Guid.NewGuid();
        user.Email = signInResult.Email;
        
        var account = new Account()
        {
            Provider = providerName,
            UserId = user.Id,
            Id = Guid.NewGuid()
        };

        await userStore.AddUserAsync(user);

        await accountStore.AddAsync(account);

        await StoreTokens(signInResult, user, account);

        return CallbackResult<TUser>.Success(user.Email, principal);
    }

    private static ClaimsPrincipal CreateClaimsPrincipal(OAuthSignInResult signInResult)
    {
        var identity = new ClaimsIdentity();
        identity.AddClaims(signInResult.Claims);
            
        // Update claims 
            
        var principal = new ClaimsPrincipal(identity);
        return principal;
    }

    private async Task StoreTokens(OAuthSignInResult signInResult, TUser existingUser, Account account)
    {
        // Store tokens and get user roles and claims
        if (signInResult.RefreshToken is not null)
        {
            await passwordStore.SetOrAddTokenAsync(existingUser.Id, account.Id, "refresh_token", signInResult.RefreshToken);
        }

        await passwordStore.SetOrAddTokenAsync(existingUser.Id, account.Id, "access_token", signInResult.AccessToken);
    }

    public async Task<OAuthTokensResult> GetTokensForProvider(string providerName, string email, bool autoRefresh = false)
    {
        var accessToken = await passwordStore.GetTokenByEmailAsync(email, providerName, "access_token");
        var refreshToken = await passwordStore.GetTokenByEmailAsync(email, providerName, "refresh_token");

        return new OAuthTokensResult()
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }
}