using Microsoft.EntityFrameworkCore;

namespace Liminal.Common.EntityFrameworkCore.Abstractions;

public interface IModuleDbContext
{
    DbSet<T> Set<T>() where T : class;
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}