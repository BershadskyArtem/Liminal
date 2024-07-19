// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.EntityFrameworkCore.Implementations;
using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

public class UserStore<TDbContext, TUser>(TDbContext context): IUserStore<TUser>
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<TUser> Data => context.Set<TUser>();

    public async Task<bool> ExistsAsync(string email) => await this.Data.AnyAsync(user => user.Email == email);

    public async Task<bool> AddAsync(TUser user, bool save = false)
    {
        await this.Data.AddAsync(user);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<TUser?> GetByEmailAsync(string email) => await this.Data.FirstOrDefaultAsync(user => user.Email == email);

    public async Task<TUser?> GetByIdAsync(Guid id) => await this.Data.FirstOrDefaultAsync(user => user.Id == id);

    public async Task<bool> UpdateAsync(TUser user, bool save = false)
    {
        this.Data.Update(user);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken) > 0;

    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
        {
            return true;
        }

        return await this.SaveChangesAsync(cancellationToken);
    }
}
