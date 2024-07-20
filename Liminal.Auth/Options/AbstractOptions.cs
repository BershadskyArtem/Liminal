using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Options;

public abstract class AbstractOptions(IServiceCollection services)
{
    public readonly IServiceCollection Services = services;

    public string DefaultRole { get; set; } = default!;

    public abstract void Build();
}