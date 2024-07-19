// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace HomeSchool.Core.Lessons.Tests.Domain;
using HomeSchool.Core.Identity;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TestAttemptEntityConfiguration : IEntityTypeConfiguration<TestAttempt>
{
    public void Configure(EntityTypeBuilder<TestAttempt> builder)
    {
        builder
            .HasKey(t => t.Id);

        builder
            .HasOne(t => t.Test)
            .WithMany(ts => ts.TestAttempts)
            .HasForeignKey(t => t.TestId)
            .IsRequired();

        builder
            .HasOne(t => t.Author)
            .WithMany(a => a.TestAttempts)
            .HasForeignKey(t => t.AuthorId)
            .IsRequired();

        builder
            .HasMany(t => t.QuestionAttempts)
            .WithOne(q => q.TestAttempt)
            .HasForeignKey(q => q.TestAttemptId);

        builder
            .HasIndex(t => t.TestId);
    }
}

public class TestAttempt : AuditableEntity
{
    public virtual Guid TestId { get; set; }

    public virtual Test Test { get; set; } = default!;

    public virtual Guid AuthorId { get; set; }

    public virtual ApplicationUser Author { get; set; } = default!;

    public DateTimeOffset StartedAt { get; set; }

    public DateTimeOffset StoppedAt { get; set; }

    public bool IsCancelled { get; set; }

    public virtual ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
}
