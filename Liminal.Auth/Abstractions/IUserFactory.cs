using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IUserFactory<out TUser>
    where TUser : AbstractUser

{
    public TUser CreateConfirmed(string email, string? userName = null);
    public TUser CreateUnConfirmed(string email, string? userName = null);
}