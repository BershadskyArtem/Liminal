using Liminal.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Storage;

public class LiminalStorageBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; set; } = services;
    
    public LiminalStorageBuilder AddDisk<TOptions, TDisk>(string name, TOptions options, Func<IServiceProvider, string, TDisk> factory)
        where TDisk : class, IDisk 
        where TOptions : class
    {
        Services.AddKeyedScoped<TDisk>(name, (provider, _) => factory(provider, name));
        Services.AddKeyedSingleton(typeof(TOptions), name, options);

        return this;
    }
}