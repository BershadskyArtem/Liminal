using Liminal.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Liminal.Storage.EntityFrameworkCore.Configurations;

public class FileAttachmentConfiguration : IEntityTypeConfiguration<FileAttachment>
{
    public virtual void Configure(EntityTypeBuilder<FileAttachment> builder)
    {
        builder
            .HasKey(f => f.Id);

        builder
            .Property(f => f.FileName)
            .HasMaxLength(128);

        builder
            .Property(f => f.UserId)
            .HasMaxLength(64);

        builder
            .Property(f => f.ExternalId)
            .HasMaxLength(64);

        builder
            .Property(f => f.Extension)
            .HasMaxLength(8);

        builder
            .Property(f => f.DiskName)
            .HasMaxLength(100);

        builder
            .Property(f => f.MimeType)
            .HasMaxLength(32);
        
        builder
            .HasIndex(f => f.FileName);
        builder
            .HasIndex(f => f.BecameNotTransientAt);
        builder
            .HasIndex(f => f.CreatedAt);
        builder
            .HasIndex(f => f.UserId);
    }
}