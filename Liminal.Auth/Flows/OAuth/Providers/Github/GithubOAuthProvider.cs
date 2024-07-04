using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Liminal.Auth.Flows.OAuth.Providers.Github;

public class GithubOAuthProvider(GithubOAuthProviderOptions options, IHttpClientFactory httpClientFactory) : IOAuthProvider
{
    public string Name { get; set; } = "github";
    
    private string ClientId => options.ClientId;
    private string ClientSecret => options.ClientSecret;

    public Task<string> GetRedirectUrl(string? state)
    {
        return Task
            .FromResult($"https://github.com/login/oauth/authorize?client_id={ClientId}&state={state}&scope=user,email");
    }
    
    public async Task<OAuthSignInResult> SignInOAuthAsync(string code, string? state)
    {
        var httpClient = httpClientFactory.CreateClient();
        try
        {
            var request = new GithubTokenRequest()
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Code = code,
                RedirectUri = options.RedirectUrl
            };
            
            httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
            var response = await httpClient.PostAsJsonAsync($"https://github.com/login/oauth/access_token", request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception("Cannot get access_token for github user.");
            }
            
            var ghResponse = await response.Content.ReadFromJsonAsync<GithubTokenResponse>();

            if (ghResponse is null || ghResponse?.AccessToken is null)
            {
                throw new Exception("Strange user with no access token");
            }

            var userInfo = await GetUserInfo(httpClient, ghResponse.AccessToken);

            return OAuthSignInResult.Success(
                TokenSet.Create(ghResponse.AccessToken, DateTimeOffset.MaxValue, null, null), 
                OAuth.UserInfo.Create(userInfo.Email, userInfo.UserName, userInfo.IsVerified),
                Name);
        }
        finally
        {
            httpClient.Dispose();
        }
    }

    public Task<TokenSet> RefreshTokenAsync(string refreshToken)
    {
        throw new NotImplementedException("Github does not support token refreshing.");
    }

    private async Task<GithubUser> GetUserInfo(HttpClient httpClient, string token)
    {
        httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Minimal-Auth-Test");
        
        var userResponse = await httpClient.GetAsync("https://api.github.com/user");

        if (userResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Unable to get github user info");
        }

        var user = await userResponse.Content.ReadFromJsonAsync<GithubUserResponse>();

        if (user is null)
        {
            throw new Exception("Cannot get info user from Github.");
        }
        
        var emailsResponse = await httpClient.GetAsync("https://api.github.com/user/emails");

        if (emailsResponse.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception("Unable to get github user info");
        }

        var emails = await emailsResponse.Content.ReadFromJsonAsync<List<GithubEmailResponse>>();

        if (emails is null || emails.Count == 0)
        {
            throw new Exception("Github account is not activated.");
        }

        var verifiedEmails = emails.Where(r => r.IsVerified).ToList();

        GithubEmailResponse? primaryVerifiedEmail = verifiedEmails.FirstOrDefault(e => e.IsPrimary);
        GithubEmailResponse? anyVerifiedEmail = verifiedEmails.FirstOrDefault();
        
        if (primaryVerifiedEmail is null && anyVerifiedEmail is null)
        {
            throw new Exception("No Github emails found.");
        }

        var githubUser = new GithubUser()
        {
            UserName = user!.Name ?? string.Empty,
            Email = primaryVerifiedEmail?.Email ?? anyVerifiedEmail?.Email!,
            IsVerified = primaryVerifiedEmail?.IsVerified ?? anyVerifiedEmail?.IsVerified ?? false,
        };
        
        return githubUser;
    }

    private class GithubUser
    {
        public string Email { get; set; } = default!;
        public bool IsVerified { get; set; }
        public string UserName { get; set; } = string.Empty;
    }
    
    private record GithubTokenRequest
    {
        [JsonPropertyName("client_id")] public string ClientId { get; init; } = default!;
        [JsonPropertyName("client_secret")] public string ClientSecret { get; init; } = default!;
        [JsonPropertyName("code")] public string Code { get; set; } = default!;
        [JsonPropertyName("redirect_uri")] public string RedirectUri { get; init; } = default!;
    }

    private record GithubEmailResponse
    {
        [JsonPropertyName("email")] public string Email { get; set; } = default!;
        [JsonPropertyName("primary")] public bool IsPrimary { get; set; }
        [JsonPropertyName("verified")] public bool IsVerified { get; set; }
    }
    
    private record GithubTokenResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = default!;
        [JsonPropertyName("scope")] public string Scope { get; set; } = default!;
        [JsonPropertyName("token_type")] public string TokenType { get; set; } = default!;
    }
    
    private record GithubUserResponse
    {
        [JsonPropertyName("email")] public string Email { get; set; } = default!;
        [JsonPropertyName("id")] public long Id { get; set; }
        [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
        [JsonPropertyName("login")] public string? Login { get; set; }
        [JsonPropertyName("name")] public string? Name { get; set; }
    }
    
}