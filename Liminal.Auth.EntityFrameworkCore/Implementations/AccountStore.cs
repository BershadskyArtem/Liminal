using Liminal.Auth.Abstractions;
using Liminal.Auth.Models;

namespace Liminal.Auth.EntityFrameworkCore.Implementations;

public class AccountStore : IAccountStore
{
    public Task AddAsync(Account account)
    {
        throw new NotImplementedException();
    }

    public Task<Account?> GetByProviderAsync(string email, string providerName)
    {
        throw new NotImplementedException();
    }
}