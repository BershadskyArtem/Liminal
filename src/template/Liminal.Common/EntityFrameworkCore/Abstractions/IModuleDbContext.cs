// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Common.EntityFrameworkCore.Abstractions;
using Microsoft.EntityFrameworkCore;

public interface IModuleDbContext
{
    DbSet<T> Set<T>()
        where T : class;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
