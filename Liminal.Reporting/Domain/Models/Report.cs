using System.ComponentModel.DataAnnotations;
using Liminal.Common.Domain.Models;
using Liminal.Reporting.Domain.Enums;

namespace Liminal.Reporting.Domain.Models;

public class Report : AuditableEntity
{
    public Guid? AuthorId { get; set; }
    [MaxLength(4000)]
    public string Text { get; private set; } = default!;
    public Guid? TargetUserId { get; set; }
    public ReportSeverity Severity { get; set; }
    public bool IsResolved { get; set; } = false;
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
        return new Report()
        {
            Id = Guid.NewGuid(),
            Text = text,
            Severity = severity,
            AuthorId = authorId,
            TargetUserId = targetUserId,
        };
    }
    
    public static Report CreateAnonymousUserReport(Guid targetUserId, string text, ReportSeverity severity)
    {
        return new Report()
        {
            Id = Guid.NewGuid(),
            Text = text,
            Severity = severity,
            TargetUserId = targetUserId,
        };
    }

    public static Report Create(Guid targetId, string text, ReportSeverity severity, Guid? authorId)
    {
        return new Report()
        {
            Id = Guid.NewGuid(),
            TargetUserId = targetId,
            Text = text,
            Severity = severity,
            AuthorId = authorId,
        };
    }
    
    public static Report Create(string text, ReportSeverity severity, Guid? authorId = null)
    {
        return new Report()
        {
            Id = Guid.NewGuid(),
            Text = text,
            Severity = severity,
            AuthorId = authorId,
        };
    }
    
}