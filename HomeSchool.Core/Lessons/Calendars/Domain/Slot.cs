using HomeSchool.Core.Identity;
using HomeSchool.Core.Lessons.Calendars.Enums;
using HomeSchool.Core.Lessons.Common.Domain;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSchool.Core.Lessons.Calendars.Domain;

public class SlotEntityTypeConfiguration : IEntityTypeConfiguration<Slot>
{
    public void Configure(EntityTypeBuilder<Slot> builder)
    {
        builder
            .HasKey(s => s.Id);

        builder
            .Property(s => s.Description)
            .HasMaxLength(300);

        builder
            .HasOne(s => s.Author)
            .WithMany(a => a.Slots)
            .HasForeignKey(s => s.AuthorId)
            .IsRequired();

        builder
            .HasMany(s => s.Tags)
            .WithMany(t => t.Slots)
            .UsingEntity("SlotsTags");
    }
}

public class Slot : AuditableEntity
{
    public virtual string? Description { get; set; } = default!;
    
    public virtual Guid AuthorId { get; set; }
    public virtual ApplicationUser Author { get; set; } = default!;

    public virtual DateTimeOffset StartDate { get; set; }
    public virtual DateTimeOffset FinishDate { get; set; }
    
    // Rules 
    public virtual bool IsRecurring { get; set; }
    public virtual int? RecurringEveryDays { get; set; }
    public virtual WeekDay? RecurringEveryWeekDay { get; set; }
    
    public virtual bool IsPublic { get; set; }

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();
    public virtual ICollection<DayTimeSpan> TimeSpans { get; set; } = new List<DayTimeSpan>();
}