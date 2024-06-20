namespace Liminal.Auth.Flows.OAuth;

public interface IOAuthProvider
{
    public string Name { get; set; }
    
    Task<string> GetRedirectUrl(string? state);
    Task<OAuthSignInResult> SignInOAuthAsync(string code, string? state);
}