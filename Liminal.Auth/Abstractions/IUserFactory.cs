using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IUserFactory<out TUser>
    where TUser : AbstractUser

{
    public TUser CreateConfirmed(string email);
    public TUser CreateUnConfirmed(string email);
}