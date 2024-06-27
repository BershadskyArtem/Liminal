namespace Liminal.Auth.Models;

/// <summary>
/// Class representing a token for the user. I.e. JWTs or Cookies that we generate. 
/// </summary>
public class UserToken
{
    public virtual Guid Id { get; set; }
    public virtual Guid UserId { get; set; }
    public virtual string AccessToken { get; set; }
    public virtual string? RefreshToken { get; set; }
    public DateTimeOffset AccessTokenValidBefore { get; set; }
    public DateTimeOffset? RefreshTokenValidBefore { get; set; }

    protected UserToken()
    { }

    public static UserToken Create(
        Guid userId, 
        string accessToken, 
        DateTimeOffset accessTokenValidBefore, 
        string? refreshToken, 
        DateTimeOffset? refreshTokenValidBefore)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accessToken);

        if ((string.IsNullOrWhiteSpace(refreshToken)) ^ (refreshTokenValidBefore is null)
            )
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken));
            }
            throw new ArgumentNullException(nameof(refreshTokenValidBefore));
        }
        
        return new UserToken()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenValidBefore = accessTokenValidBefore,
            RefreshTokenValidBefore = refreshTokenValidBefore
        };
    }
}