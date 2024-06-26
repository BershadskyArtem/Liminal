using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IUserStore<TUser> where TUser : AbstractUser
{
    public Task<bool> ExistsAsync(string email);
    public Task<bool> AddAsync(TUser user, bool save = false);
    public Task<TUser?> GetByEmailAsync(string email);
    public Task<TUser?> GetByIdAsync(Guid id);
    public Task<bool> UpdateAsync(TUser user, bool save = false);
    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}