using System.Security.Claims;
using Liminal.Auth.Models;

namespace Liminal.Auth.Flows.OAuth;

public class CallbackResult<TUser> where TUser : AbstractUser
{
    public ClaimsPrincipal? Principal { get; set; }
    public TUser? User { get; set; }
    public required string Email { get; set; }
    public bool IsSuccess { get; set; }

    public static CallbackResult<TUser> Success(string email, ClaimsPrincipal principal)
    {
        return new CallbackResult<TUser>()
        {
            IsSuccess = true,
            Principal = principal,
            Email = email
        };
    }

    public static CallbackResult<TUser> Failure()
    {
        return new CallbackResult<TUser>()
        {
            IsSuccess = false,
            Principal = null,
            Email = string.Empty
        };
    }
}