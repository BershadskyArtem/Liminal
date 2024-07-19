// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using HomeSchool.Core.Identity;
using HomeSchool.Core.Reporting.Violations.Enums;
using Liminal.Common.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeSchool.Core.Reporting.Violations.Domain;

public class ReportsEntityConfiguration : IEntityTypeConfiguration<Report>
{
    public void Configure(EntityTypeBuilder<Report> builder)
    {
        builder.HasIndex(r => r.Id);

        builder
            .HasOne<ApplicationUser>(r => r.Author)
            .WithMany(u => u.Reports)
            .HasForeignKey(c => c.AuthorId);

        builder
            .HasOne(r => r.TargetUser)
            .WithMany(u => u.Complaints)
            .HasForeignKey(c => c.TargetUserId);
    }
}

public class Report : AuditableEntity
{
    public Guid? AuthorId { get; set; }

    public ApplicationUser? Author { get; set; }

    [MaxLength(4000)]
    public string Text { get; private set; } = default!;

    public Guid? TargetUserId { get; set; }

    public ApplicationUser? TargetUser { get; set; }

    public ReportSeverity Severity { get; set; }

    public bool IsResolved { get; set; }

    public DateTimeOffset? ResolvedAt { get; private set; }

    public Guid? ResolvedBy { get; private set; }

    [MaxLength(4000)]
    public string? ResolveMessage { get; set; }

    public bool ResolveEmailSent { get; set; }

    protected Report()
    { }

    public static Report CreateUserReport(Guid authorId, Guid targetUserId, string text, ReportSeverity severity)
        => new()
        {
            Id = Guid.NewGuid(),
            Text = text,
            Severity = severity,
            AuthorId = authorId,
            TargetUserId = targetUserId,
        };

    public static Report CreateAnonymousUserReport(Guid targetUserId, string text, ReportSeverity severity) => new Report
    {
        Id = Guid.NewGuid(),
        Text = text,
        Severity = severity,
        TargetUserId = targetUserId,
    };

    public static Report Create(Guid targetId, string text, ReportSeverity severity, Guid? authorId) => new Report
    {
        Id = Guid.NewGuid(),
        TargetUserId = targetId,
        Text = text,
        Severity = severity,
        AuthorId = authorId,
    };

    public static Report Create(string text, ReportSeverity severity, Guid? authorId = null) => new Report
    {
        Id = Guid.NewGuid(),
        Text = text,
        Severity = severity,
        AuthorId = authorId,
    };

    public void Resolve(Guid resolvedBy, string resolveMessage)
    {
        this.ResolvedBy = resolvedBy;
        this.ResolvedAt = DateTimeOffset.UtcNow;
        this.ResolveMessage = resolveMessage;
        this.IsResolved = true;
    }

    public void Reopen() => this.IsResolved = false;

    public void EmailSent() => this.ResolveEmailSent = true;
}
