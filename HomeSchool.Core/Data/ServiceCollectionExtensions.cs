using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HomeSchool.Core.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(
            options =>
            {
                options.UseInMemoryDatabase("inmem.db");
            });
        
        return services;
    }
}