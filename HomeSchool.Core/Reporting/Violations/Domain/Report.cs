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

public class Report: AuditableEntity
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
    
    public void Resolve(Guid resolvedBy, string resolveMessage)
    {
        ResolvedBy = ResolvedBy;
        ResolvedAt = DateTimeOffset.UtcNow;
        ResolveMessage = resolveMessage;
        IsResolved = true;
    }

    public void Reopen()
    {
        IsResolved = false;
    }

    public void EmailSent()
    {
        ResolveEmailSent = true;
    }

    public static Report CreateUserReport(Guid authorId, Guid targetUserId, string text, ReportSeverity severity) 
    {
        return new Report
        {
            Id = Guid.NewGuid(),
            Text = text,
            Severity = severity,
            AuthorId = authorId,
            TargetUserId = targetUserId
        };
    }
    
    public static Report CreateAnonymousUserReport(Guid targetUserId, string text, ReportSeverity severity)
    {
        return new Report
        {
            Id = Guid.NewGuid(),
            Text = text,
            Severity = severity,
            TargetUserId = targetUserId
        };
    }

    public static Report Create(Guid targetId, string text, ReportSeverity severity, Guid? authorId)
    {
        return new Report
        {
            Id = Guid.NewGuid(),
            TargetUserId = targetId,
            Text = text,
            Severity = severity,
            AuthorId = authorId
        };
    }
    
    public static Report Create(string text, ReportSeverity severity, Guid? authorId = null)
    {
        return new Report
        {
            Id = Guid.NewGuid(),
            Text = text,
            Severity = severity,
            AuthorId = authorId
        };
    }
}