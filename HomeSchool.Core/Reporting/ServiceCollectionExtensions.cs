using HomeSchool.Core.Reporting.Violations.Services;
using Microsoft.Extensions.DependencyInjection;

namespace HomeSchool.Core.Reporting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddReporting(this IServiceCollection services)
    {
        services.AddScoped<IReportingService, ReportingService>();
        
        return services;    
    }
}