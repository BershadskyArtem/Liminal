using Liminal.Storage.Abstractions;
using Liminal.Storage.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Storage;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiminalFileStore(this IServiceCollection services, Action<LiminalStorageBuilder> configure)
    {
        services.AddScoped<IFileStore, DefaultFileStore>();

        var options = new LiminalStorageBuilder(services);

        configure(options);
        
        return services;
    }
}