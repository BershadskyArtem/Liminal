using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liminal.Auth.EntityFrameworkCore.Configuration;

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