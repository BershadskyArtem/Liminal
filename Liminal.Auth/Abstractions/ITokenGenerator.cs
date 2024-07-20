using System.Security.Claims;
using Liminal.Auth.Results;
using Microsoft.AspNetCore.Http;

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
    public Task<GenerateTokenResult> GenerateToken(ClaimsPrincipal principal, bool save = true);
    
    /// <summary>
    /// Refreshes access token using refresh token. 
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <returns></returns>
    public Task<GenerateTokenResult> RefreshToken(string refreshToken);

    /// <summary>
    /// Signs out token so that refresh and access tokens are invalid. 
    /// </summary>
    /// <param name="context"></param>
    /// <param name="accessToken"></param>
    /// <param name="refreshToken"></param>
    /// <returns>Task that shows the progress of operation.</returns>
    public Task SignOutAsync(HttpContext context , string? accessToken, string? refreshToken);
}