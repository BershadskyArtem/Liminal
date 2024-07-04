namespace Liminal.Auth.Flows.OAuth;

public class TokenSet
{
    public string AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTimeOffset AccessTokenValidUntil { get; set; }
    public DateTimeOffset? RefreshTokenValidUntil { get; set; }

    protected TokenSet()
    { }

    public static TokenSet Create(string accessToken, DateTimeOffset accessTokenValidUntil)
        => Create(accessToken, accessTokenValidUntil, null, null); 
    
    public static TokenSet Create(
        string accessToken, 
        DateTimeOffset accessTokenValidUntil, 
        string? refreshToken, 
        DateTimeOffset? refreshTokenValidUntil)
    {
        return new TokenSet()
        {
            AccessToken = accessToken,
            AccessTokenValidUntil = accessTokenValidUntil,
            RefreshToken = refreshToken,
            RefreshTokenValidUntil = refreshTokenValidUntil
        };
    }
    
}