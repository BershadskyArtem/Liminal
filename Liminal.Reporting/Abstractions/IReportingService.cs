using ErrorOr;
using Liminal.Reporting.Domain.Enums;

namespace Liminal.Reporting.Abstractions;

public interface IReportingService
{
    public Task<ErrorOr<Guid>> ReportUserAsync(Guid userId, string text, ReportSeverity severity, Guid? authorUserId);
    public Task<ErrorOr<Guid>> ReportIssueAsync(string text, ReportSeverity severity, Guid? authorUserId);
    public Task<ErrorOr<bool>> SetResolveAsync(Guid reportId, Guid resolverUserId, string resolveMessage, bool resolve, bool sendEmail = false);
    public Task<ErrorOr<bool>> DeleteAsync(Guid reportId);
}