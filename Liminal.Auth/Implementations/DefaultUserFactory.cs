using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;

namespace Liminal.Auth.Implementations;

public class DefaultUserFactory<TUser> : IUserFactory<TUser> 
    where TUser : AbstractUser
{
    public TUser CreateConfirmed(string email, string? userName = null)
    {
        var result = CreateUnConfirmed(email, userName);
        
        result.Confirm();
        
        return result;
    }

    public TUser CreateUnConfirmed(string email, string? userName = null)
    {
        var result = Activator.CreateInstance<TUser>();
        result.Email = email;
        result.Username = userName;
        result.UnConfirm();

        return result;
    }
}