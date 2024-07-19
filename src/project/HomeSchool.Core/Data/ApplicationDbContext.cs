// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace HomeSchool.Core.Data;
using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Identity;
using HomeSchool.Core.Lessons.Tests.Domain;
using HomeSchool.Core.Reporting.Violations.Domain;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Storage.EntityFrameworkCore.Abstractions;
using Liminal.Storage.Models;
using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : LiminalIdentityContext<ApplicationUser>,
    IAttachmentDbContext<ApplicationAttachment>
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
    }

    public DbSet<AnswerOption> AnswerOptions { get; set; }

    public DbSet<Question> Questions { get; set; }

    public DbSet<QuestionAttempt> QuestionAttempts { get; set; }

    public DbSet<Test> Tests { get; set; }

    public DbSet<TestAttempt> TestAttempts { get; set; }

    public DbSet<ApplicationAttachment> Set() => this.Set<ApplicationAttachment>();

    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Report).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Test).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
