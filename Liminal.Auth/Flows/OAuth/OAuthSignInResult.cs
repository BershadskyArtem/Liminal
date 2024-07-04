namespace Liminal.Auth.Flows.OAuth;

public class OAuthSignInResult
{
    public TokenSet Tokens { get; set; } = default!;
    public UserInfo User { get; set; } = default!;
    public bool IsSuccess { get; private set; }
    public string? FailureMessage { get; private set; }
    public string Provider { get; set; } = "default";
    public string? RedirectAfter { get; set; }

    public static OAuthSignInResult Success(TokenSet tokens, UserInfo user, string provider)
        => new OAuthSignInResult()
        {
            Tokens = tokens,
            User = user,
            IsSuccess = true,
            Provider = provider,
        };
    
    public static OAuthSignInResult Failure(string? failureMessage) =>
        new OAuthSignInResult()
        {
            IsSuccess = false,
            FailureMessage = failureMessage
        };
}