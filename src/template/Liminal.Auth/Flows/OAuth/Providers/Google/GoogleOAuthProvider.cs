// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

// ReSharper disable UnusedMember.Local
// ReSharper disable UnusedAutoPropertyAccessor.Local
// ReSharper disable ClassNeverInstantiated.Local
namespace Liminal.Auth.Flows.OAuth.Providers.Google;

// Sources
// https://www.oauth.com/oauth2-servers/signing-in-with-google/verifying-the-user-info/
// https://developers.google.com/identity/openid-connect/openid-connect#obtainuserinfo
// Appwrite github.
[SuppressMessage(
    "Usage",
    "CA2234:Pass system uri objects instead of strings",
    Justification = "Needed for readability.")]
[SuppressMessage(
    "Major Code Smell",
    "S125:Sections of code should not be commented out",
    Justification = "May be useful later.")]
[SuppressMessage(
    "Usage",
    "CA2201:Do not raise reserved exception types",
    Justification = "Fuck you")]
[SuppressMessage(
    "Major Code Smell",
    "S112:General or reserved exceptions should never be thrown",
    Justification = "Fuck you 2")]
public class GoogleOAuthProvider(IHttpClientFactory httpClientFactory, GoogleOAuthProviderOptions options): IOAuthProvider
{
    // TODO: Make OAuthFlow additional checks for user id because apparently
    // Email of the user from google can change.
    public string Name { get; set; } = "google";

    public Task<string> GetRedirectUrl(string? state)
    {
        if (this._scopes is null)
        {
            this._scopes = string.Empty;

            foreach (var optionsScope in options.Scopes)
            {
                this._scopes += " " + optionsScope;
            }
        }

        // https://github.com/p2/OAuth2/issues/208
        // https://developers.google.com/identity/protocols/oauth2/web-server#httprest_2
        var result2 =
            $"https://accounts.google.com/o/oauth2/v2/auth?scope={this._scopes}&access_type=offline&include_granted_scopes=true&response_type=code&state={state}&redirect_uri={options.RedirectUri}&client_id={options.ClientId}";

        // var result = $"https://accounts.google.com/o/oauth2/auth?client_id={options.ClientId}&redirect_uri=https://www.example.com/back&scope={_scopes}&access_type=offline&response_type=token&state=asdafwswdwefwsdg,";
        return Task.FromResult(result2);
    }

    public async Task<OAuthSignInResult> SignInOAuthAsync(string code, string? state)
    {
        var client = httpClientFactory.CreateClient();

        try
        {
            var req = new GoogleOAuthRequest
            {
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
                Code = code,
                RedirectUri = options.RedirectUri,
                GrantType = "authorization_code",
            };

            var response = await client.PostAsJsonAsync(TokenEndpoint, req);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return OAuthSignInResult.Failure($"Google OAuth token request fail. Got success code {response.StatusCode}");
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();
            if (tokenResponse is null)
            {
                return OAuthSignInResult.Failure("Cannot get oauth tokens for google.");
            }

            var accessToken = tokenResponse.AccessToken;
            var userInfo = await GetUserInfoAsync(client, accessToken);

            var expiryDate = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            var refreshTokenValidUntil = DateTimeOffset.UtcNow.AddMonths(6);

            return OAuthSignInResult.Success(
                TokenSet.Create(tokenResponse.AccessToken, expiryDate, tokenResponse.RefreshToken, refreshTokenValidUntil),
                UserInfo.Create(userInfo.Email, userInfo.Username, userInfo.IsVerified),
                this.Name);
        }
        finally
        {
            client.Dispose();
        }
    }

    public async Task<TokenSet> RefreshTokenAsync(string refreshToken)
    {
        var client = httpClientFactory.CreateClient();

        try
        {
            var req = new GoogleRefreshRequest
            {
                RefreshToken = refreshToken,
                ClientId = options.ClientId,
                ClientSecret = options.ClientSecret,
            };

            // May be needed ? on the end of the URL
            var refreshResponse = await client.PostAsJsonAsync(TokenEndpoint, req);

            if (refreshResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Cannot refresh Google token");
            }

            var refresh = await refreshResponse.Content.ReadFromJsonAsync<GoogleRefreshResponse>()
                          ?? throw new Exception("Cannot parse google refresh response");

            var expiryDate = DateTimeOffset.UtcNow.AddSeconds(refresh.Expires);
            return TokenSet.Create(refresh.AccessToken, expiryDate);
        }
        finally
        {
            client.Dispose();
        }
    }

    private static async Task<GoogleUser> GetUserInfoAsync(HttpClient client, string accessToken)
    {
        var userResponse = await client.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");

        if (userResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Cannot get Google user");
        }

        var user = await userResponse.Content.ReadFromJsonAsync<GoogleUserResponse>()
                   ?? throw new Exception("Cannot parse user from Google");

        var googleUser = new GoogleUser
        {
            Email = user.Email,
            IsVerified = user.IsVerified,
            Username = user.Username ?? string.Empty,
        };

        return googleUser;
    }

    private sealed class GoogleUser
    {
        public string Email { get; init; } = default!;

        public string Username { get; init; } = default!;

        public bool IsVerified { get; init; }
    }

    private sealed record GoogleRefreshResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = default!;

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = default!;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = default!;

        [JsonPropertyName("expires")]
        public int Expires { get; set; }
    }

    private sealed record GoogleRefreshRequest
    {
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; } = default!;

        [JsonPropertyName("client_id")]
        public string ClientId { get; init; } = default!;

        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; init; } = default!;

        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = "refresh_token";
    }

    private sealed record GoogleUserResponse
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = default!;
        [JsonPropertyName("name")]
        public string? Username { get; set; }
        [JsonPropertyName("email_verified")]
        public bool IsVerified { get; set; }
    }

    private sealed record GoogleOAuthRequest
    {
        [JsonPropertyName("client_id")]
        public string ClientId { get; init; } = default!;
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; init; } = default!;
        [JsonPropertyName("code")]
        public string Code { get; set; } = default!;
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; } = default!;
        [JsonPropertyName("redirect_uri")]
        public string RedirectUri { get; init; } = default!;
    }

    private sealed record GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = default!;
        [JsonPropertyName("scope")]
        public string Scope { get; set; } = default!;
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = default!;
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; } = 3920;
        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; set; } = default!;
    }

    private const string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private string? _scopes;
}
