using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;

namespace Liminal.Auth.Implementations;

public class DefaultUserFactory<TUser> : IUserFactory<TUser> 
    where TUser : AbstractUser
{
    public TUser CreateConfirmed(string email)
    {
        var result = CreateUnConfirmed(email);
        
        result.Confirm();
        
        return result;
    }

    public TUser CreateUnConfirmed(string email)
    {
        var result = Activator.CreateInstance<TUser>();
        result.Email = email;
        result.UnConfirm();

        return result;
    }
}