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
    [JsonPropertyName("message")]
    public string? Message { get; set; }
    
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    protected GenerateTokenResult()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    { }
    
    public static GenerateTokenResult Failure(string message)
    {
        return new GenerateTokenResult()
        {
            IsSuccess = false,
            AccessToken = string.Empty,
            RefreshToken = string.Empty,
            Type = string.Empty,
            Message = message
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
            Type = type,
            Message = "success"
        };
    }
}