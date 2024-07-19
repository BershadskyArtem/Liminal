// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Identity;
using HomeSchool.Core.Lessons.Tests.Enums;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSchool.Core.Lessons.Tests.Domain;

public class QuestionTypeConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder
            .HasKey(q => q.Id);

        builder
            .HasMany(q => q.Tests)
            .WithMany(t => t.Questions)
            .UsingEntity("TestsQuestionsTable");

        builder
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId)
            .IsRequired();

        builder
            .HasOne(q => q.Attachment)
            .WithOne(a => a.Question)
            .HasForeignKey<Question>(q => q.AttachmentId)
            .IsRequired(false);

        builder
            .HasOne(q => q.Author)
            .WithMany(a => a.Questions)
            .HasForeignKey(q => q.AuthorId);

        builder
            .Property(q => q.Text)
            .HasMaxLength(4000);
    }
}

public class Question : AuditableEntity
{
    public virtual Guid AuthorId { get; set; }

    public virtual ApplicationUser Author { get; set; } = default!;

    public virtual string? Text { get; set; }

    public virtual QuestionDifficulty Difficulty { get; set; }

    public virtual Guid? AttachmentId { get; set; }

    public virtual ApplicationAttachment? Attachment { get; set; }

    public virtual bool ShuffleAnswers { get; set; }

    public virtual ICollection<AnswerOption> Answers { get; set; } = new List<AnswerOption>();

    public virtual ICollection<QuestionAttempt> Attempts { get; set; } = new List<QuestionAttempt>();

    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
}
