using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Liminal.Common.EntityFrameworkCore.Interceptors;

public class AuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var dbContext = eventData.Context;
        
        if(dbContext is null)
            return base.SavingChangesAsync(eventData, result, cancellationToken);

        var entriesToAudit = dbContext
            .ChangeTracker
            .Entries<AuditableEntity>();
        
        foreach (var entityEntry in entriesToAudit)
        {
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    entityEntry.Property(p => p.CreatedAt).CurrentValue = DateTimeOffset.UtcNow;
                    entityEntry.Property(p => p.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
                    break;
                case EntityState.Modified:
                    entityEntry.Property(p => p.UpdatedAt).CurrentValue = DateTimeOffset.UtcNow;
                    break;
            }
        }
        
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}