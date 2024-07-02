using Liminal.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liminal.Auth.EntityFrameworkCore.Configuration;

public class AccountTokenEntityConfiguration : IEntityTypeConfiguration<AccountToken>
{
    public void Configure(EntityTypeBuilder<AccountToken> builder)
    {
        builder
            .HasOne<Account>(t => t.Account)
            .WithMany(a => a.Passwords);

        builder
            .HasKey(t => t.Id);

        builder.HasIndex(t => new
        {
            t.Provider,
            t.TokenName,
            t.TokenValue
        }, "IX_AccountTokenProviderNameValue");

        builder
            .Property(t => t.Provider).HasMaxLength(50);

        builder
            .Property(t => t.TokenName).HasMaxLength(50);
    }
}