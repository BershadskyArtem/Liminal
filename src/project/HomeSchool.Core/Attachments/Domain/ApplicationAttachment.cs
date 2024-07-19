// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Lessons.Tests.Domain;
using Liminal.Storage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ApplicationAttachmentTypeConfiguration : IEntityTypeConfiguration<ApplicationAttachment>
{
    public void Configure(EntityTypeBuilder<ApplicationAttachment> builder)
    {
        builder
            .HasKey(at => at.Id);

        builder
            .HasOne(at => at.Question)
            .WithOne(q => q.Attachment)
            .HasForeignKey<ApplicationAttachment>(at => at.QuestionId);

        builder
            .HasOne(at => at.QuestionAttempt)
            .WithOne(qa => qa.Attachment)
            .HasForeignKey<ApplicationAttachment>(at => at.QuestionAttemptId);

        builder
            .HasOne(at => at.AnswerOption)
            .WithOne(o => o.Attachment)
            .HasForeignKey<ApplicationAttachment>(at => at.AnswerOptionId);
    }
}

public class ApplicationAttachment : FileAttachment
{
    public Guid? QuestionId { get; set; }

    public Question? Question { get; set; }

    public Guid? QuestionAttemptId { get; set; }

    public QuestionAttempt? QuestionAttempt { get; set; }

    public Guid? AnswerOptionId { get; set; }

    public AnswerOption? AnswerOption { get; set; }
}
