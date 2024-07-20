using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HomeSchool.Core.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        var dbName = configuration["School:DB"];

        if (string.IsNullOrWhiteSpace(dbName))
        {
            throw new ArgumentNullException(dbName, "Check School:DB field. It should be pgsql or sqlite.");
        }
        
        var connectionString = configuration.GetConnectionString(dbName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentNullException(connectionString,
                $"Connection string in ConnectionStrings:{dbName} does not exist");
        }
        
        Action<DbContextOptionsBuilder> configure;
        
        if (dbName == "pgsql")
        {
            configure = options =>
            {
                options.UseNpgsql(connectionString);
            };
        }
        else if (dbName == "sqlite")
        {
            configure = options =>
            {
                options.UseSqlite(connectionString);
            };
        }
        else
        {
            throw new ArgumentException(
                $"Cannot construct database {dbName}. Only pgsql or sqlite databases are accepted");
        }
        
        services.AddDbContext<ApplicationDbContext>(configure);
        
        return services;
    }
}