using System.Security.Claims;

namespace Liminal.Auth.Abstractions;

public interface IClaimsStore
{
    public Task<bool> AddRangeAsync(List<Claim> claims, Guid userId, Guid accountId, string providerName, bool save = false);
    public Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}