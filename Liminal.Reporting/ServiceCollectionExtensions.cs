using Liminal.Reporting.Abstractions;
using Liminal.Reporting.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Reporting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiminalReporting<TDbContext>(this IServiceCollection services)
        where TDbContext : class, IReportingDbContext
    {
        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<IReportingDbContext, TDbContext>();
        
        return services;
    }
}