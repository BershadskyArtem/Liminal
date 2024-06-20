using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Options;

public abstract class AbstractOptions(IServiceCollection services)
{
    public readonly IServiceCollection Services = services;

    public abstract void Build();
}