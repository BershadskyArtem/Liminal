// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth;

public interface IOAuthProvider
{
    public string Name { get; set; }

    public Task<string> GetRedirectUrl(string? state);

    public Task<OAuthSignInResult> SignInOAuthAsync(string code, string? state);

    public Task<TokenSet> RefreshTokenAsync(string refreshToken);
}
