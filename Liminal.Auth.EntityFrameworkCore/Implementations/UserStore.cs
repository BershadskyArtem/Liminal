using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class UserStore<TUser> : IUserStore<TUser> where TUser : AbstractUser
{
    
    
    public Task<bool> UserExistsByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }

    public Task AddUserAsync(TUser user)
    {
        throw new NotImplementedException();
    }

    public Task<TUser?> GetUserByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}