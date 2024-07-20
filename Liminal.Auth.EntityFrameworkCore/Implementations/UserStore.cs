using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class UserStore<TDbContext, TUser>(TDbContext context) : IUserStore<TUser>
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<TUser> Data => context.Set<TUser>();
    
    public async Task<bool> ExistsAsync(string email)
    {
        return await Data.AnyAsync(user => user.Email == email);
    }

    public async Task<bool> AddAsync(TUser user, bool save = false)
    {
        await Data.AddAsync(user);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<TUser?> GetByEmailAsync(string email)
    {
        return await Data.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<TUser?> GetByIdAsync(Guid id)
    {
        return await Data.FirstOrDefaultAsync(user => user.Id == id);
    }

    public async Task<bool> UpdateAsync(TUser user, bool save = false)
    {
        Data.Update(user);
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