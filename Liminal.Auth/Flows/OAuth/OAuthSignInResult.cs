using System.Security.Claims;

namespace Liminal.Auth.Flows.OAuth;

public class OAuthSignInResult
{
    public required string Email { get; set; }
    public required string AccessToken { get; set; }
    public bool IsSuccess { get; private set; }
    public string? RefreshToken { get; set; }
    public string? FailureMessage { get; private set; }
    public string Provider { get; set; }
    public string? RedirectAfter { get; set; }
    public List<Claim> Claims { get; set; } = [];

    public static OAuthSignInResult Success(
        string accessToken, 
        string email, 
        string provider,
        List<Claim> claims, 
        string? refreshToken) =>
        new OAuthSignInResult()
        {
            AccessToken = accessToken,
            Email = email,
            IsSuccess = true,
            RefreshToken = refreshToken,
            Claims = claims,
            Provider = provider,
        };
    
    public static OAuthSignInResult Failure(string? failureMessage) =>
        new OAuthSignInResult()
        {
            AccessToken = string.Empty,
            Email = string.Empty,
            IsSuccess = false,
            RefreshToken = null,
            FailureMessage = failureMessage
        };
}