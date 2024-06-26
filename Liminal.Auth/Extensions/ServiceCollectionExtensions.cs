using FluentValidation;
using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiminalAuth(this IServiceCollection services, Action<LiminalAuthBuilder> cfg)
    {
        var options = new LiminalAuthBuilder(services);

        cfg(options);

        options.Build();

        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.AddValidatorsFromAssembly(AssemblyMarker.Assembly);
        
        return services;
    }
}