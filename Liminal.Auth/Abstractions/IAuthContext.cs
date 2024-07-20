using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IAuthContext<TUser> 
    where TUser : AbstractUser
{
    public Guid? UserId { get; set; }
    public bool IsConfirmed { get; set; }
    public Task<string?> Role();

    public Task<TUser?> Current();
}