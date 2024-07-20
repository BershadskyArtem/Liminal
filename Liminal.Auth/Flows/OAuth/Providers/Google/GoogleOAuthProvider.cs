using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Liminal.Auth.Flows.OAuth.Providers.Google;

// Sources
// https://www.oauth.com/oauth2-servers/signing-in-with-google/verifying-the-user-info/
// https://developers.google.com/identity/openid-connect/openid-connect#obtainuserinfo
// Appwrite github.

public class GoogleOAuthProvider(IHttpClientFactory httpClientFactory,GoogleOAuthProviderOptions options) : IOAuthProvider
{
    // TODO: Make OAuthFlow additional checks for user id because apparently
    // Email of the user from google can change.
    
    public string Name { get; set; } = "google";
    
    public Task<string> GetRedirectUrl(string? state)
    {
        if (_scopes is null)
        {
            _scopes = "";

            foreach (var optionsScope in options.Scopes)
            {
                _scopes += " " + optionsScope;
            }
        }
        // https://github.com/p2/OAuth2/issues/208
        //https://developers.google.com/identity/protocols/oauth2/web-server#httprest_2
        var result2 =
            $"https://accounts.google.com/o/oauth2/v2/auth?scope={_scopes}&access_type=offline&include_granted_scopes=true&response_type=code&state={state}&redirect_uri={options.RedirectUri}&client_id={options.ClientId}";
        
        //var result = $"https://accounts.google.com/o/oauth2/auth?client_id={options.ClientId}&redirect_uri=https://www.example.com/back&scope={_scopes}&access_type=offline&response_type=token&state=asdafwswdwefwsdg,";
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
                GrantType = "authorization_code"
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
                Name);
        }
        finally
        {
            client.Dispose();
        }
    }

    private async Task<GoogleUser> GetUserInfoAsync(HttpClient client, string accessToken)
    {
        var userResponse = await client.GetAsync($"https://www.googleapis.com/oauth2/v3/userinfo?access_token={accessToken}");

        if (userResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Cannot get Google user");
        }

        var user = await userResponse.Content.ReadFromJsonAsync<GoogleUserResponse>();

        if (user is null)
        {
            throw new Exception("Cannot parse user from Google");
        }

        var googleUser = new GoogleUser
        {
            Email = user.Email,
            IsVerified = user.IsVerified,
            Username = user.Username ?? string.Empty
        };

        return googleUser;
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
                ClientSecret = options.ClientSecret
            };

            var refreshResponse = await client.PostAsJsonAsync("https://oauth2.googleapis.com/token?", req);

            if (refreshResponse.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Cannot refresh Google token");
            }

            var refresh = await refreshResponse.Content.ReadFromJsonAsync<GoogleRefreshResponse>();

            if (refresh is null)
            {
                throw new Exception("Cannot parse google refresh response");
            }

            var expiryDate = DateTimeOffset.UtcNow.AddSeconds(refresh.Expires);
            return TokenSet.Create(refresh.AccessToken, expiryDate);
        }
        catch (Exception e)
        {
            throw;
        }
        finally
        {
            client.Dispose();
        }
    }

    private class GoogleUser
    {
        public string Email { get; set; }
        public string Username { get; set; }
        public bool IsVerified { get; set; }
    }

    private record GoogleRefreshResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; }
        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
        [JsonPropertyName("token_type")] public string TokenType { get; set; }
        [JsonPropertyName("expires")] public int Expires { get; set; }
    }

    private record GoogleRefreshRequest
    {
        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
        [JsonPropertyName("client_id")] public string ClientId { get; set; }
        [JsonPropertyName("client_secret")] public string ClientSecret { get; set; }
        [JsonPropertyName("grant_type")] public string GrantType { get; set; } = "refresh_token";
    }
    
    private record GoogleUserResponse
    {
        [JsonPropertyName("email")] public string Email { get; set; } = default!;
        [JsonPropertyName("name")] public string? Username { get; set; }
        [JsonPropertyName("email_verified")] public bool IsVerified { get; set; }
    }
    
    private record GoogleOAuthRequest
    {
        [JsonPropertyName("client_id")] public string ClientId { get; init; } = default!;
        [JsonPropertyName("client_secret")] public string ClientSecret { get; init; } = default!;
        [JsonPropertyName("code")] public string Code { get; set; } = default!;
        [JsonPropertyName("grant_type")] public string GrantType { get; set; } = default!;
        [JsonPropertyName("redirect_uri")] public string RedirectUri { get; init; } = default!;
    }
    
    private record GoogleTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = default!;
        [JsonPropertyName("scope")] public string Scope { get; set; } = default!;
        [JsonPropertyName("token_type")] public string TokenType { get; set; } = default!;
        [JsonPropertyName("expires_in")] public int ExpiresIn { get; set; } = 3920;
        [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; } = default!;
    }

    private record GoogleUserInfoResponse
    {
        [JsonPropertyName("email")] public string Email { get; set; } = default!;
        [JsonPropertyName("email_verified")] public bool IsEmailVerified { get; set; }
    }
    
    private string? _scopes = null;
    private static readonly string TokenEndpoint = "https://oauth2.googleapis.com/token";
    private static readonly string UserInfoEndpoint = "https://www.googleapis.com/oauth2/v3/userinfo?access_token=";
}