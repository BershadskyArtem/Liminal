using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace Liminal.Auth.Flows.OAuth.Providers.Google;

public class GoogleOAuthProvider(IHttpClientFactory httpClientFactory,GoogleOAuthProviderOptions options) : IOAuthProvider
{
    // TODO: Make OAuthFlow additional checks for user id because apparently
    // Email of the user from google can change.
    // https://developers.google.com/identity/openid-connect/openid-connect#obtainuserinfo
    
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
            var req = new GoogleOAuthRequest();

            var response = await client.PostAsJsonAsync(TokenEndpoint, req);
            var tokenResponse = await response.Content.ReadFromJsonAsync<GoogleTokenResponse>();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                return OAuthSignInResult.Failure($"Got success code {response.StatusCode}");
            }
            
            if (tokenResponse is null)
            {
                return OAuthSignInResult.Failure("Cannot get oauth tokens");
            }
            
            var accessToken = tokenResponse.AccessToken;
            
            // We do not do token validation here because i do not want to.
            // https://www.oauth.com/oauth2-servers/signing-in-with-google/verifying-the-user-info/
            
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var userInfoResponse = await client.GetAsync(UserInfoEndpoint);
            
            if (userInfoResponse.StatusCode != HttpStatusCode.OK)
            {
                return OAuthSignInResult.Failure($"Cannot get user info for Google. Got status code: {userInfoResponse.StatusCode}");
            }
            
            var userInfo = await response.Content.ReadFromJsonAsync<GoogleUserInfoResponse>();

            if (userInfo is null)
            {
                return OAuthSignInResult.Failure("Cannot get user info");
            }

            var expiryDate = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn);
            
            return OAuthSignInResult.Success(
                tokenResponse.AccessToken, 
                userInfo.Email, 
                Name,
                new List<Claim>(),
                expiryDate,
                tokenResponse.RefreshToken);
        }
        finally
        {
            client.Dispose();
        }
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
        [JsonPropertyName("token_type")] public int ExpiresIn { get; set; } = 3920;
        [JsonPropertyName("token_type")] public string RefreshToken { get; set; } = default!;
    }

    private record GoogleUserInfoResponse
    {
        [JsonPropertyName("email")] public string Email { get; set; } = default!;
        [JsonPropertyName("email_verified")] public bool IsEmailVerified { get; set; }
    }
    
    private string? _scopes = null;
    public static readonly string TokenEndpoint = "https://oauth2.googleapis.com/token";
    public static readonly string UserInfoEndpoint = " https://www.googleapis.com/oauth2/v3/userinfo";

}