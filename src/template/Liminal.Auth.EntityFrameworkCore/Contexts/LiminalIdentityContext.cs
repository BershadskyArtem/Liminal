// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.Models;
using Liminal.Common.EntityFrameworkCore.Contexts;
using Microsoft.EntityFrameworkCore;

public class LiminalIdentityContext<TUser> : HookingDbContext
    where TUser : AbstractUser
{
    public LiminalIdentityContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<TUser> Users { get; set; }

    public DbSet<Account> Accounts { get; set; }

    public DbSet<AccountToken> Passwords { get; set; }

    public DbSet<UserClaim> UserClaims { get; set; }

    public DbSet<AccountClaim> AccountClaims { get; set; }

    public DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyMarker.Current);

        base.OnModelCreating(modelBuilder);
    }
}
