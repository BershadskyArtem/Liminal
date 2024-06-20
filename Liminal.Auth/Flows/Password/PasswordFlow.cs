using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;

namespace Liminal.Auth.Flows.Password;

public class PasswordFlow<TUser>(
    IUserStore<TUser> userStore,
    IPasswordStore passwordStore,
    IAccountStore accountStore)
    : IAuthFlow
    where TUser : AbstractUser
{
    public string Name { get; } = "password";

    public async Task Register(TUser user, string password)
    {
        var exists = await userStore.UserExistsByEmailAsync(user.Email);

        if (exists)
        {
            throw new Exception("Duplicates email");
        }

        var hashedPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(password);
        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);

        await userStore.AddUserAsync(user);

        var account = new Account()
        {
            Id = Guid.NewGuid(),
            Passwords = new List<Models.Password>(),
            Provider = "register",
            UserId = user.Id
        };

        await accountStore.AddAsync(account);

        await passwordStore.SetPasswordAsync(user.Email, account.Id, hashedPassword);
    }
    
    public async Task<bool> CanSignIn(string email, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        var hashedPassword = await passwordStore.GetHashedPasswordByEmailAsync(email);

        ArgumentException.ThrowIfNullOrWhiteSpace(hashedPassword);

        return BCrypt.Net.BCrypt.EnhancedVerify(password, hashedPassword);
    }
    
}