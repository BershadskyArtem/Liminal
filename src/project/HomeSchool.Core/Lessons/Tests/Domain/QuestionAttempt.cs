// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace HomeSchool.Core.Lessons.Tests.Domain;
using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Identity;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class QuestionAttemptTypeConfiguration : IEntityTypeConfiguration<QuestionAttempt>
{
    public void Configure(EntityTypeBuilder<QuestionAttempt> builder)
    {
        builder
            .HasKey(qa => qa.Id);

        builder
            .HasOne(qa => qa.Author)
            .WithMany(a => a.QuestionAttempts)
            .HasForeignKey(qa => qa.AuthorId)
            .IsRequired();

        builder
            .HasOne(qa => qa.Question)
            .WithMany(q => q.Attempts)
            .HasForeignKey(qa => qa.QuestionId)
            .IsRequired();

        builder
            .HasOne(qa => qa.Answer)
            .WithMany(o => o.Attempts)
            .HasForeignKey(qa => qa.AnswerId)
            .IsRequired();

        builder
            .HasOne(qa => qa.Attachment)
            .WithOne(att => att.QuestionAttempt)
            .HasForeignKey<QuestionAttempt>(qa => qa.AttachmentId);

        builder
            .HasOne(qa => qa.TestAttempt)
            .WithMany(ta => ta.QuestionAttempts)
            .HasForeignKey(qa => qa.TestAttemptId)
            .IsRequired();

        builder
            .Property(qa => qa.TextAnswer)
            .HasMaxLength(4000);

        builder
            .HasIndex(qa => qa.TestAttemptId);

        builder
            .HasIndex(qa => qa.QuestionId);
    }
}

public class QuestionAttempt : AuditableEntity
{
    public Guid QuestionId { get; set; }

    public Question Question { get; set; } = default!;

    public Guid AuthorId { get; set; }

    public ApplicationUser Author { get; set; } = default!;

    // Student can choose one of the answers
    public Guid? AnswerId { get; set; }

    public AnswerOption? Answer { get; set; }

    // Can give text answer
    public string? TextAnswer { get; set; }

    // And can send file as an answer
    public Guid? AttachmentId { get; set; }

    public ApplicationAttachment? Attachment { get; set; }

    public Guid TestAttemptId { get; set; }

    public TestAttempt TestAttempt { get; set; }

    public DateTimeOffset? StartedAt { get; set; }

    public DateTimeOffset? FinishedAt { get; set; }
}
