using System.Security.Claims;
using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class ClaimsStore<TDbContext, TUser>(TDbContext context) : IClaimsStore
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<AccountClaim> Data => context.Set<AccountClaim>();
    
    public async Task<bool> AddRangeAsync(List<Claim> claims, Guid userId, Guid accountId, string providerName, bool save = false)
    {
        var userClaims = claims.Select(c => AccountClaim.FromClaim(c, accountId)).ToList();
        await Data.AddRangeAsync(userClaims);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }
    
    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
            return true;

        return await SaveChangesAsync(cancellationToken);
    }

}