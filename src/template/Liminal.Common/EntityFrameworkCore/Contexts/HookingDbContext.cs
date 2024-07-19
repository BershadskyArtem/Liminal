// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Common.EntityFrameworkCore.Contexts;
using Liminal.Common.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

public class HookingDbContext : DbContext
{
    protected HookingDbContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditableInterceptor());
        base.OnConfiguring(optionsBuilder);
    }
}
