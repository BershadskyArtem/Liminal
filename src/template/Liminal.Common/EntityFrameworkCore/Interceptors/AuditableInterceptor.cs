// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Common.EntityFrameworkCore.Interceptors;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class AuditableInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result,
        CancellationToken cancellationToken = new())
    {
        var dbContext = eventData.Context;

        if (dbContext is null)
        {
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

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
