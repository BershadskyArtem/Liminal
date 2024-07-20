using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class AccountStore<TDbContext, TUser>(TDbContext context) : IAccountStore
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<Account> Data => context.Set<Account>();
    
    public async Task<bool> ExistsAsync(Guid userId, string providerName)
    {
        return await Data
            .AnyAsync(account => account.UserId == userId && account.Provider == providerName);
    }

    public async Task<bool> ExistsAsync(string email, string providerName)
    {
        return await Data
            .AnyAsync(account => account.Email == email && account.Provider == providerName);
    }

    public async Task<bool> AddAsync(Account account, bool save = false)
    {
        await Data.AddAsync(account);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<Account?> GetByProviderAsync(string email, string providerName)
    {
        return await Data
            .FirstOrDefaultAsync(account => account.Email == email && account.Provider == providerName);
    }

    public async Task<Account?> GetByProviderAsync(Guid userId, string providerName)
    {
        return await Data
            .FirstOrDefaultAsync(account => account.UserId == userId && account.Provider == providerName);
    }

    public async Task<bool> UpdateAsync(Account account, bool save = false)
    {
        Data.Update(account);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<Account?> GetByIdAsync(Guid id)
    {
        return await Data.FirstOrDefaultAsync(account => account.Id == id);
    }

    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
            return true;

        return await SaveChangesAsync(cancellationToken);
    }

}