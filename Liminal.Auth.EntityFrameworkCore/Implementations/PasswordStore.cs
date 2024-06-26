using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class PasswordStore<TDbContext, TUser>(TDbContext context) : IPasswordStore
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<AccountToken> Data => context.Set<AccountToken>();
    public async Task<AccountToken?> GetByAccountIdAsync(Guid accountId, string tokenName)
    {
        return await Data.FirstOrDefaultAsync(password =>
            password.AccountId == accountId && password.TokenName == tokenName);
    }

    public async Task<bool> AddAsync(AccountToken accountToken, bool save = false)
    {
        await Data.AddAsync(accountToken);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SetByAccountIdAsync(Guid id, string tokenName, string tokenValue, bool save = false)
    {
        // Chore: Consider introducing ExecuteUpdateAsync from Relational Extensions.
        var password = await Data.FirstOrDefaultAsync(password => password.AccountId == id && password.TokenName == tokenName);

        if (password is null)
        {
            return false;
        }

        password.TokenValue = tokenValue;

        return await UpdateAsync(password, save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(AccountToken accountToken, bool save = false)
    {
        Data.Update(accountToken);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<AccountToken?> GetByValueAsync(string providerName, string tokenValue)
    {
        return await Data.FirstOrDefaultAsync(token => token.TokenValue == tokenValue && token.Provider == providerName);
    }

    public async Task<bool> RemoveAsync(AccountToken password, bool save = false)
    {
        Data.Remove(password);
        return await MaybeSaveChangesAsync(save);
    }

    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
            return true;

        return await SaveChangesAsync(cancellationToken);
    }
}