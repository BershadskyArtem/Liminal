// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace HomeSchool.Core.Lessons.Tests.Domain;
using HomeSchool.Core.Identity;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class TestEntityTypeConfiguration : IEntityTypeConfiguration<Test>
{
    public void Configure(EntityTypeBuilder<Test> builder)
    {
        builder
            .HasKey(t => t.Id);

        builder
            .HasOne(q => q.Author)
            .WithMany(u => u.Tests)
            .HasForeignKey(t => t.AuthorId)
            .IsRequired();

        builder
            .HasMany(t => t.Questions)
            .WithMany(q => q.Tests)
            .UsingEntity("TestsQuestions");

        builder
            .HasMany(t => t.TestAttempts)
            .WithOne(a => a.Test)
            .HasForeignKey(t => t.TestId)
            .IsRequired();

        builder
            .Property(t => t.Name)
            .HasMaxLength(256)
            .IsRequired();

        builder
            .Property(t => t.Description)
            .HasMaxLength(512);

        builder
            .HasIndex(t => t.Name);

        builder
            .HasIndex(t => t.AuthorId);

        // TODO: Lesson.
    }
}

public class Test : AuditableEntity
{
    public virtual Guid AuthorId { get; set; }

    public virtual ApplicationUser Author { get; set; }

    public virtual string Name { get; set; } = default!;

    public virtual string Description { get; set; } = default!;

    public virtual int MaxAttempts { get; protected set; }

    public virtual int MaxTimeInSeconds { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
}
