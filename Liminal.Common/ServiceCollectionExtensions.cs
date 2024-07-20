using Liminal.Common.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Common;

public static class ServiceCollectionExtensions
{
    public static FrontendOptions AddLiminalConfig(this IServiceCollection services, Action<FrontendOptions> configure)
    {
        var options = new FrontendOptions();

        configure(options);
        
        services.AddSingleton(options);
        
        return options;
    }
}