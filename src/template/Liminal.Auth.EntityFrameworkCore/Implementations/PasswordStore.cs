// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.EntityFrameworkCore.Implementations;
using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

public class PasswordStore<TDbContext, TUser>(TDbContext context): IPasswordStore
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<AccountToken> Data => context.Set<AccountToken>();

    public async Task<AccountToken?> GetByAccountIdAsync(Guid accountId, string tokenName) => await this.Data.FirstOrDefaultAsync(password =>
                                                                                                       password.AccountId == accountId && password.TokenName == tokenName);

    public async Task<bool> AddAsync(AccountToken accountToken, bool save = false)
    {
        await this.Data.AddAsync(accountToken);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SetByAccountIdAsync(Guid id, string tokenName, string tokenValue, bool save = false)
    {
        // Chore: Consider introducing ExecuteUpdateAsync from Relational Extensions.
        var password = await this.Data.FirstOrDefaultAsync(password => password.AccountId == id && password.TokenName == tokenName);

        if (password is null)
        {
            return false;
        }

        password.TokenValue = tokenValue;

        return await this.UpdateAsync(password, save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken) > 0;

    public async Task<bool> UpdateAsync(AccountToken accountToken, bool save = false)
    {
        this.Data.Update(accountToken);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<AccountToken?> GetByValueAsync(string providerName, string tokenValue) => await this.Data.FirstOrDefaultAsync(token => token.TokenValue == tokenValue && token.Provider == providerName);

    public async Task<bool> RemoveAsync(AccountToken password, bool save = false)
    {
        this.Data.Remove(password);
        return await this.MaybeSaveChangesAsync(save);
    }

    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
        {
            return true;
        }

        return await this.SaveChangesAsync(cancellationToken);
    }
}
