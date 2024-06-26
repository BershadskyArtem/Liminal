using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class UserTokenStore<TDbContext, TUser>(TDbContext context) : IUserTokenStore 
    where TDbContext : LiminalIdentityContext<TUser>
    where TUser : AbstractUser
{
    protected DbSet<UserToken> Data => context.Set<UserToken>();


    public async Task<IEnumerable<UserToken>> GetByUserId(Guid userId)
    {
        return await Data.Where(token => token.UserId == userId).ToListAsync();
    }

    public async Task<UserToken?> GetByAccessToken(string accessToken)
    {
        return await Data.FirstOrDefaultAsync(token => token.AccessToken == accessToken);
    }

    public async Task<UserToken?> GetByRefreshToken(string refreshToken)
    {
        return await Data.FirstOrDefaultAsync(token => token.RefreshToken == refreshToken);
    }

    public async Task<bool> AddAsync(UserToken token, bool save = false)
    {
        await Data.AddAsync(token);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<bool> UpdateToken(UserToken token, bool save = false)
    {
        Data.Update(token);
        return await MaybeSaveChangesAsync(save);
    }

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> RemoveAsync(UserToken tokenSet, bool save = false)
    {
        Data.Remove(tokenSet);

        return await MaybeSaveChangesAsync(save);
    }

    private async Task<bool> MaybeSaveChangesAsync(bool save, CancellationToken cancellationToken = default)
    {
        if (!save)
            return true;

        return await SaveChangesAsync(cancellationToken);
    }
}