using System.Security.Claims;
using System.Text.Json.Serialization;
using Liminal.Auth.Models;

namespace Liminal.Auth.Results;

public class CallbackResult<TUser> where TUser : AbstractUser
{
    [JsonIgnore]
    public ClaimsPrincipal? Principal { get; set; }
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    [JsonPropertyName("redirect_after")]
    public string? RedirectAfter { get; set; }
    [JsonPropertyName("is_success")]
    public bool IsSuccess { get; set; }
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    public static CallbackResult<TUser> Success(string email, ClaimsPrincipal principal, string? redirectAfter)
    {
        return new CallbackResult<TUser>()
        {
            IsSuccess = true,
            Principal = principal,
            Email = email,
            RedirectAfter = redirectAfter
        };
    }

    public static CallbackResult<TUser> Failure(string message)
    {
        return new CallbackResult<TUser>()
        {
            IsSuccess = false,
            Principal = null,
            Email = string.Empty,
            Message = message
        };
    }
}