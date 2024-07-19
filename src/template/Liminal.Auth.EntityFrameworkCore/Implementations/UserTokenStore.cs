// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.EntityFrameworkCore.Implementations;
using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

public class UserTokenStore<TDbContext, TUser>(TDbContext context): IUserTokenStore
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<UserToken> Data => context.Set<UserToken>();

    public async Task<IEnumerable<UserToken>> GetByUserId(Guid userId) => await this.Data.Where(token => token.UserId == userId).ToListAsync();

    public async Task<UserToken?> GetByAccessToken(string accessToken) => await this.Data.FirstOrDefaultAsync(token => token.AccessToken == accessToken);

    public async Task<UserToken?> GetByRefreshToken(string refreshToken) => await this.Data.FirstOrDefaultAsync(token => token.RefreshToken == refreshToken);

    public async Task<bool> AddAsync(UserToken token, bool save = false)
    {
        await this.Data.AddAsync(token);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<bool> UpdateToken(UserToken token, bool save = false)
    {
        this.Data.Update(token);
        return await this.MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default) => await context.SaveChangesAsync(cancellationToken) > 0;

    public async Task<bool> RemoveAsync(UserToken tokenSet, bool save = false)
    {
        this.Data.Remove(tokenSet);

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
