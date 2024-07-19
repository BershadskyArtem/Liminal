// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class ClaimsStore<TDbContext, TUser>(TDbContext context): IClaimsStore
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    private DbSet<AccountClaim> Data => context.Set<AccountClaim>();

    public async Task<bool> AddRangeAsync(IReadOnlyCollection<Claim> claims, Guid userId, Guid accountId, string providerName, bool save = false)
    {
        var userClaims = claims.Select(c => AccountClaim.FromClaim(c, accountId)).ToList();
        await this.Data.AddRangeAsync(userClaims);
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
