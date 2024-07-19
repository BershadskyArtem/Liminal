// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        => new()
        {
            Tokens = tokens,
            User = user,
            IsSuccess = true,
            Provider = provider,
        };

    public static OAuthSignInResult Failure(string? failureMessage) =>
        new()
        {
            IsSuccess = false,
            FailureMessage = failureMessage,
        };
}
