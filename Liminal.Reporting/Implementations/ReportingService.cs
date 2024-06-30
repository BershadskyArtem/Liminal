using Liminal.Reporting.Abstractions;
using Liminal.Reporting.Domain.Enums;
using Liminal.Reporting.Domain.Models;
using ErrorOr;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Reporting.Implementations;

public class ReportingService(IReportingDbContext context) : IReportingService
{
    private DbSet<Report> Data => context.Set<Report>();
    
    public async Task<ErrorOr<Guid>> ReportUserAsync(Guid userId, string text, ReportSeverity severity, Guid? authorUserId)
    {
        var report = Report.Create(userId, text, severity, authorUserId);
        await Data.AddAsync(report);
        var result = await context.SaveChangesAsync();
        return result > 0 ? report.Id : Error.Failure();
    }

    public async Task<ErrorOr<Guid>> ReportIssueAsync(string text, ReportSeverity severity, Guid? authorUserId)
    {
        var report = Report.Create(text, severity, authorUserId);
        await Data.AddAsync(report);
        var result = await context.SaveChangesAsync(); 
        return result > 0 ? report.Id : Error.Failure();
    }
    
    public async Task<ErrorOr<bool>> SetResolveAsync(
        Guid reportId, 
        Guid resolverUserId, 
        string resolveMessage, 
        bool resolve, 
        bool sendEmail = false)
    {
        var report = await Data.FirstOrDefaultAsync(r => r.Id == reportId);

        if (report is null)
        {
            return Error.NotFound();
        }
        
        report.Resolve(resolverUserId, resolveMessage);
        Data.Update(report);
        var result = await context.SaveChangesAsync();
        return result > 0 ? true : Error.Failure();
    }

    public async Task<ErrorOr<bool>> DeleteAsync(Guid reportId)
    {
        var report = await Data.FirstOrDefaultAsync(r => r.Id == reportId);

        if (report is null)
        {
            return Error.NotFound();
        }

        Data.Remove(report);
        var result = await context.SaveChangesAsync();
        return result > 0 ? true : Error.Failure();
    }
}