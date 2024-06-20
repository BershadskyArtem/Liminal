using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class PasswordStore<TDbContext, TUser>(TDbContext context) : IPasswordStore
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{

    public async Task<string?> GetHashedPasswordByEmailAsync(string email)
    {
        var password = await context
            .Set<Password>()
            .FirstOrDefaultAsync(p => p.Email == email && p.TokenName == "password");

        return password?.TokenValue;
    }

    public async Task SetPasswordAsync(string email, Guid accountId, string hashedPassword)
    {
        var user = await context.Set<TUser>().FirstOrDefaultAsync(u => u.Email == email);

        if (user is null)
        {
            throw new ArgumentException(nameof(email));
        }
        
        var password = await context.Set<Password>()
            .FirstOrDefaultAsync(p => p.Email == email && p.TokenName == "password" && p.AccountId == accountId);

        if (password is null)
        {
            password = new Password()
            {
                TokenValue = hashedPassword,
                TokenName = "password",
                UserId = user.Id,
                AccountId = accountId,
                Email = email,
                Id = Guid.NewGuid()
            };
            await context.Set<Password>().AddAsync(password);
            await context.SaveChangesAsync();
            return;
        }

        password.TokenValue = hashedPassword;
        context.Set<Password>().Update(password);

        await context.SaveChangesAsync();
    }

    public Task<bool> SetOrAddTokenAsync(Guid userId, Guid accountId, string tokenName, string tokenValue)
    {
        throw new NotImplementedException();
    }

    public Task<string?> GetTokenByEmailAsync(string email, string providerName, string accessToken)
    {
        throw new NotImplementedException();
    }
}