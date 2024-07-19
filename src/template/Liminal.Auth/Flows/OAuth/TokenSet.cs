// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth;

public class TokenSet
{
    public string AccessToken { get; set; }

    public string? RefreshToken { get; set; }

    public DateTimeOffset AccessTokenValidUntil { get; set; }

    public DateTimeOffset? RefreshTokenValidUntil { get; set; }

    private TokenSet(string accessToken, DateTimeOffset accessTokenValidUntil)
    {
        AccessToken = accessToken;
        AccessTokenValidUntil = accessTokenValidUntil;
    }

    public static TokenSet Create(string accessToken, DateTimeOffset accessTokenValidUntil)
        => Create(accessToken, accessTokenValidUntil, null, null);

    public static TokenSet Create(
        string accessToken,
        DateTimeOffset accessTokenValidUntil,
        string? refreshToken,
        DateTimeOffset? refreshTokenValidUntil) => new TokenSet(accessToken, accessTokenValidUntil)
        {
            RefreshToken = refreshToken,
            RefreshTokenValidUntil = refreshTokenValidUntil,
        };
}
