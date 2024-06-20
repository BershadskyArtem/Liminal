using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IUserStore<TUser> where TUser : AbstractUser
{
    Task<bool> UserExistsByEmailAsync(string email);
    Task AddUserAsync(TUser user);
    Task<TUser?> GetUserByEmailAsync(string email);
}