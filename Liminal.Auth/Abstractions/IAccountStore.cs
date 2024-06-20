using Liminal.Auth.Models;

namespace Liminal.Auth.Abstractions;

public interface IAccountStore
{
    Task AddAsync(Account account);
    Task<Account?> GetByProviderAsync(string email, string providerName);
}