using System.Security.Claims;
using Liminal.Auth.Results;

namespace Liminal.Auth.Abstractions;

public interface ITokenGenerator
{
    /// <summary>
    /// The name of the token generator (i.e. JWT, Cookie, etc.)
    /// </summary>
    public string Name { get; }
    /// <summary>
    /// Generates an access token from principal.
    /// </summary>
    /// <param name="principal">The user's <see cref="ClaimsPrincipal"/> to generate access token from. </param>
    /// <returns>Task with the token result in it.</returns>
    public Task<GenerateTokenResult> GenerateToken(ClaimsPrincipal principal);
    
    /// <summary>
    /// Refreshes access token using refresh token. 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public Task<RefreshTokenResult> RefreshToken(string refreshToken);
}