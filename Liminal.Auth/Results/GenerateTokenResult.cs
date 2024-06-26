using System.Text.Json.Serialization;

namespace Liminal.Auth.Results;

public class GenerateTokenResult : ResultBase
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
    [JsonPropertyName("expires")]
    public DateTime Expires { get; set; }
    [JsonPropertyName("lifetime")]
    public int Lifetime { get; set; }
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    protected GenerateTokenResult()
    { }
    
    public static GenerateTokenResult Failure()
    {
        return new GenerateTokenResult()
        {
            IsSuccess = false
        };
    }
    
    public static GenerateTokenResult Success(string token, string refreshToken, DateTime expires, int seconds, string type)
    {
        return new GenerateTokenResult()
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            Expires = expires,
            IsSuccess = true,
            Lifetime = seconds,
            Type = type
        };
    }
}