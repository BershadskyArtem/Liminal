using System.Collections.Frozen;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.OAuth.Providers;

/// <summary>
/// Actually funny name 
/// </summary>
public class OAuthProvidersProvider : IOAuthProvidersProvider
{
    private readonly IServiceProvider _serviceProvider;
    private readonly FrozenDictionary<string, IOAuthProvider> _providers;

    /// <summary>
    /// Actually funny name 
    /// </summary>
    public OAuthProvidersProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;

        var result = _serviceProvider.GetServices<IOAuthProvider>();

        _providers = result.ToFrozenDictionary(k => k.Name, k => k);

    }

    public IOAuthProvider GetProvider(string key)
    {
        return _serviceProvider.GetRequiredKeyedService<IOAuthProvider>(key);
    }
}