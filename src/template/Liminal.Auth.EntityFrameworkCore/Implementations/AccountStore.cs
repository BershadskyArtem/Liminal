// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.EntityFrameworkCore.Implementations;
using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

public class AccountStore<TDbContext, TUser>(TDbContext context): IAccountStore
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<Account> Data => context.Set<Account>();

    public async Task<bool> ExistsAsync(Guid userId, string providerName) => await this.Data
            .AnyAsync(account => account.UserId == userId && account.Provider == providerName);

    public async Task<bool> ExistsAsync(string email, string providerName) => await this.Data
            .AnyAsync(account => account.Email == email && account.Provider == providerName);

    public async Task<bool> AddAsync(Account account, bool save = false)
    {
        await this.Data.AddAsync(account);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<Account?> GetByProviderAsync(string email, string providerName) => await this.Data
            .FirstOrDefaultAsync(account => account.Email == email && account.Provider == providerName);

    public async Task<Account?> GetByProviderAsync(Guid userId, string providerName) => await this.Data
            .FirstOrDefaultAsync(account => account.UserId == userId && account.Provider == providerName);

    public async Task<bool> UpdateAsync(Account account, bool save = false)
    {
        this.Data.Update(account);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken) > 0;

    public async Task<Account?> GetByIdAsync(Guid id) => await this.Data.FirstOrDefaultAsync(account => account.Id == id);

    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
        {
            return true;
        }

        return await this.SaveChangesAsync(cancellationToken);
    }
}
