using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Common;
using Liminal.Auth.Models;
using Liminal.Auth.Results;
using Microsoft.IdentityModel.Tokens;

namespace Liminal.Auth.Jwt;

public class JwtTokenGenerator<TUser>(
    JwtTokenGeneratorOptions options,
    IUserTokenStore userTokenStore,
    IUserStore<TUser> userStore) : ITokenGenerator 
    where TUser : AbstractUser
{
    public string Name { get; } = JwtDefaults.Scheme;

    public async Task<GenerateTokenResult> GenerateToken(ClaimsPrincipal principal)
    {
        var signingCredentials = new SigningCredentials(options.CryptoKey, SecurityAlgorithms.HmacSha256);

        var expires = DateTimeOffset.UtcNow.Add(options.AccessTokenLifetime);

        var claims = principal.Claims;

        var tokenOptions = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            expires: expires.DateTime.ToUniversalTime(),
            claims: claims);
        
        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

        var refreshToken = CryptoUtils.GenerateRandomString(32);

        Guid userId = Guid.Empty;

        var subClaim = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrWhiteSpace(subClaim))
        {
            return GenerateTokenResult.Failure();
        } else if (!Guid.TryParse(subClaim, out userId))
        {
            return GenerateTokenResult.Failure();
        }

        var tokenSet = UserToken.Create(userId, token, expires, refreshToken, DateTimeOffset.UtcNow.AddDays(60));

        await userTokenStore.AddAsync(tokenSet, true);
        
        return GenerateTokenResult
            .Success(token, refreshToken, expires.DateTime.ToUniversalTime(), options.AccessTokenLifetime.Seconds, Name);
    }

    public async Task<GenerateTokenResult> RefreshToken(string refreshToken)
    {
        var tokenSet = await userTokenStore.GetByRefreshToken(refreshToken);

        if (tokenSet is null)
        {
            return GenerateTokenResult.Failure();
        }

        var user = await userStore.GetByIdAsync(tokenSet.UserId);

        if (user is null)
        {
            return GenerateTokenResult.Failure();
        }

        var principal = user.ToPrincipal();

        var token = await GenerateToken(principal);

        await userTokenStore.RemoveAsync(tokenSet, true);

        return token;
    }
}