using FluentValidation;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Implementations;
using Liminal.Auth.Models;
using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiminalAuth<TUser>(this IServiceCollection services, Action<LiminalAuthBuilder> cfg)
        where TUser : AbstractUser
    {
        var options = new LiminalAuthBuilder(services);

        cfg(options);

        options.Build();

        services.AddHttpContextAccessor();
        services.AddHttpClient();
        services.AddValidatorsFromAssembly(AssemblyMarker.Assembly);
        services.AddScoped<IAuthContext<TUser>, DefaultAuthContext<TUser>>();
        services.AddScoped<IAccountLinker<TUser>, DefaultAccountLinker<TUser>>();
        
        return services;
    }
}