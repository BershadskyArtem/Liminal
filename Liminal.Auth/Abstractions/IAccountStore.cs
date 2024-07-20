using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IAccountStore
{
    public Task<bool> ExistsAsync(Guid userId, string providerName);
    public Task<bool> ExistsAsync(string email, string providerName);
    public Task<bool> AddAsync(Account account, bool save = false);
    public Task<Account?> GetByProviderAsync(string email, string providerName);
    public Task<Account?> GetByProviderAsync(Guid userId, string providerName);
    public Task<bool> UpdateAsync(Account account, bool save = false);
    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);

    public Task<Account?> GetByIdAsync(Guid id);
}