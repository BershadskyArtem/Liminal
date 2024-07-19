// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
    IUserStore<TUser> userStore): ITokenGenerator
    where TUser : AbstractUser
{
    public string Name => JwtDefaults.Scheme;

    public async Task<GenerateTokenResult> GenerateToken(ClaimsPrincipal principal, bool save = true)
    {
        ArgumentNullException.ThrowIfNull(principal);

        var expires = DateTimeOffset.UtcNow.Add(options.AccessTokenLifetime);

        var token = this.SignJwt(principal, expires);

        var refreshToken = CryptoUtils.GenerateRandomString(32);

        Guid userId;

        var subClaim = principal.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

        if (string.IsNullOrWhiteSpace(subClaim))
        {
            return GenerateTokenResult.Failure("sub claims does not exist.");
        }
        else if (!Guid.TryParse(subClaim, out userId))
        {
            return GenerateTokenResult.Failure("sub claim is not valid.");
        }

        var tokenSet = UserToken.Create(userId, token, expires, refreshToken, DateTimeOffset.UtcNow.AddDays(60));

        await userTokenStore.AddAsync(tokenSet, save);

        return GenerateTokenResult
            .Success(token, refreshToken, expires.DateTime.ToUniversalTime(), (int)options.AccessTokenLifetime.TotalSeconds, this.Name);
    }

    public async Task<GenerateTokenResult> RefreshToken(string refreshToken)
    {
        var tokenSet = await userTokenStore.GetByRefreshToken(refreshToken);

        if (tokenSet is null)
        {
            return GenerateTokenResult.Failure("Invalid token.");
        }

        var user = await userStore.GetByIdAsync(tokenSet.UserId);

        if (user is null)
        {
            return GenerateTokenResult.Failure("Invalid token.");
        }

        var principal = user.ToPrincipal();

        var token = await this.GenerateToken(principal, false);

        await userTokenStore.RemoveAsync(tokenSet);

        await userTokenStore.SaveChangesAsync();

        return token;
    }

    private string SignJwt(ClaimsPrincipal principal, DateTimeOffset expires)
    {
        var signingCredentials = new SigningCredentials(options.CryptoKey, SecurityAlgorithms.HmacSha256);

        var claims = principal.Claims;

        var tokenOptions = new JwtSecurityToken(
            signingCredentials: signingCredentials,
            expires: expires.AddHours(3).DateTime,
            claims: claims);

        // TODO: This is a very dirty hack. Remove it later.
        // This is caused by time differences. UTC -> Moscow = 3 hours.
        var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        return token;
    }
}
