using System.Security.Claims;
using Liminal.Auth.Models;

namespace Liminal.Auth.Results;

public class CallbackResult<TUser> where TUser : AbstractUser
{
    public ClaimsPrincipal? Principal { get; set; }
    public required string Email { get; set; }
    public string? RedirectAfter { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }

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