// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.EntityFrameworkCore.Configuration;
using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class UserTokenEntityConfiguration<TUser> : IEntityTypeConfiguration<UserToken>
    where TUser : AbstractUser
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder
            .HasKey(t => t.Id);

        builder
            .HasOne<TUser>()
            .WithMany(t => t.Tokens);

        builder
            .HasIndex(t => t.UserId, "IX_UserTokenUser");
    }
}
