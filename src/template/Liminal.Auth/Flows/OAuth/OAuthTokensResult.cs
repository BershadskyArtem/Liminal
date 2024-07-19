// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        => new(accessToken, isSuccess, refreshToken, errorMessage);

    private OAuthTokensResult(string accessToken, bool isSuccess, string? refreshToken, string? errorMessage)
    {
        this.AccessToken = accessToken;
        this.RefreshToken = refreshToken;
        this.IsSuccess = isSuccess;
        this.ErrorMessage = errorMessage;
    }
}
