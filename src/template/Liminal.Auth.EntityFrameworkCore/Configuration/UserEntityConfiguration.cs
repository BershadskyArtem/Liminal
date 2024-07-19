// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.EntityFrameworkCore.Configuration;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserEntityConfiguration<TUser> : IEntityTypeConfiguration<TUser>
    where TUser : AbstractUser
{
    public void Configure(EntityTypeBuilder<TUser> builder)
    {
        builder
            .HasKey(u => u.Id);

        builder
            .Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder
            .HasMany(u => u.Accounts)
            .WithOne(nameof(Account.UserId));

        builder
            .HasMany(u => u.Tokens)
            .WithOne(nameof(UserToken.UserId));

        builder
            .HasMany(u => u.Claims)
            .WithOne()
            .HasForeignKey(c => c.UserId);

        builder
            .HasIndex(u => u.IsConfirmed, "IX_UserConfirmed");

        builder
            .HasIndex(u => u.Email, "IX_UserEmails");
    }
}
