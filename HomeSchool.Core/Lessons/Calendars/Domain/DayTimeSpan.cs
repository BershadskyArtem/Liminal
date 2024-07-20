using HomeSchool.Core.Identity;
using HomeSchool.Core.Lessons.Common.Domain;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSchool.Core.Lessons.Calendars.Domain;

public class DayTimeSpanEntityTypeConfiguration : IEntityTypeConfiguration<DayTimeSpan>
{
    public void Configure(EntityTypeBuilder<DayTimeSpan> builder)
    {
        builder
            .HasKey(d => d.Id);

        builder
            .HasOne(d => d.Author)
            .WithMany(a => a.TimeSpans)
            .HasForeignKey(d => d.AuthorId)
            .IsRequired();

        builder
            .HasOne(d => d.Slot)
            .WithMany(s => s.TimeSpans)
            .HasForeignKey(d => d.SlotId)
            .IsRequired();

        builder
            .HasMany(d => d.Tags)
            .WithMany(t => t.TimeSpans)
            .UsingEntity("TimeSpansTags");
    }
}

public class DayTimeSpan : Entity
{
    public Guid AuthorId { get; set; }
    public ApplicationUser Author { get; set; } = default!;

    public Guid SlotId { get; set; }
    public Slot Slot { get; set; } = default!;

    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    /// <summary>
    /// Every 15, 30, 45, n minutes
    /// </summary>
    public TimeSpan RecurrenceTime { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
}