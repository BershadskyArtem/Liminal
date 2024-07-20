using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liminal.Auth.EntityFrameworkCore.Configuration;

public class AccountEntityConfiguration<TUser> : IEntityTypeConfiguration<Account>
    where TUser : AbstractUser
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasKey(a => a.Id);

        builder
            .HasOne<TUser>()
            .WithMany(a => a.Accounts)
            .HasForeignKey(a => a.UserId);

        builder
            .HasMany<AccountToken>(a => a.Passwords)
            .WithOne(p => p.Account)
            .HasForeignKey(t => t.AccountId);

        builder.HasIndex(a => new
        {
            a.Email,
            a.Provider
        }, "IX_AccountProviderEmail");

        builder
            .HasIndex(a => a.IsConfirmed, "IX_AccountConfirmed");
    }
}