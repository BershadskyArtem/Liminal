using HomeSchool.Core.Identity;
using HomeSchool.Core.Lessons.Calendars.Domain;
using HomeSchool.Core.Lessons.Tests.Domain;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSchool.Core.Lessons.Common.Domain;

public class TagEntityTypeConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder
            .HasKey(t => t.Id);

        builder
            .HasOne(t => t.Author)
            .WithMany(a => a.Tags)
            .HasForeignKey(t => t.AuthorId)
            .IsRequired();

        builder
            .Property(t => t.Name)
            .HasMaxLength(32)
            .IsRequired();

        builder
            .Property(t => t.Value)
            .HasMaxLength(64)
            .IsRequired();

        builder
            .Property(t => t.Color)
            .HasMaxLength(32)
            .IsRequired();
    }
}

public class Tag : Entity
{
    public Guid AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = default!;
    
    public string Name { get; set; } = default!;
    public string Value { get; set; } = default!;

    public string Color { get; set; } = default!;

    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    public virtual ICollection<DayTimeSpan>? TimeSpans { get; set; } = new List<DayTimeSpan>();
    public virtual ICollection<Question>? Questions { get; set; } = new List<Question>();
}