using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Identity;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSchool.Core.Lessons.Tests.Domain;

public class AnswerOptionEntityTypeConfiguration : IEntityTypeConfiguration<AnswerOption>
{
    public void Configure(EntityTypeBuilder<AnswerOption> builder)
    {
        builder
            .HasKey(a => a.Id);

        // builder
        //     .HasOne(a => a.Author)
        //     .WithMany(u => u.QuestionAnswers)
        //     .HasForeignKey(a => a.AuthorId)
        //     .IsRequired();

        // builder
        //     .HasOne(a => a.Question)
        //     .WithMany(q => q.Answers)
        //     .HasForeignKey(a => a.QuestionId)
        //     .IsRequired();

        builder
            .HasOne(a => a.Attachment)
            .WithOne(att => att.AnswerOption)
            .HasForeignKey<AnswerOption>(a => a.AttachmentId);

        builder
            .Property(a => a.Text)
            .HasMaxLength(512);
        
        builder
            .HasIndex(a => a.QuestionId);

        builder
            .HasIndex(a => a.AuthorId);
    }
}

public class AnswerOption : AuditableEntity
{
    public Guid AuthorId { get; set; }
    public ApplicationUser Author { get; set; }
    public Guid QuestionId { get; set; }
    public Question Question { get; set; }
    public string? Text { get; set; }
    public bool IsProper { get; set; }
    public Guid? AttachmentId { get; set; }
    public ApplicationAttachment? Attachment { get; set; }
    public ICollection<QuestionAttempt> Attempts { get; set; } = new List<QuestionAttempt>();
}