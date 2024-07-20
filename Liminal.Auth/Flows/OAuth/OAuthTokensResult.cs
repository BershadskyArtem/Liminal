namespace Liminal.Auth.Flows.OAuth;

public class OAuthTokensResult
{ 
    public string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }

    public static OAuthTokensResult Success(string accessToken, string? refreshToken = null)
        => Create(accessToken, true, refreshToken, null);

    public static OAuthTokensResult Failure(string errorMessage)
        => Create(string.Empty, false, null, errorMessage);
    
    public static OAuthTokensResult Create(string accessToken, bool isSuccess, string? refreshToken, string? errorMessage)
    {
        return new OAuthTokensResult(accessToken, isSuccess, refreshToken, errorMessage);
    }

    private OAuthTokensResult(string accessToken, bool isSuccess, string? refreshToken, string? errorMessage)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        IsSuccess = isSuccess;
        ErrorMessage = errorMessage;
    }
}