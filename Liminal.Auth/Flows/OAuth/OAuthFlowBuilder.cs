using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Liminal.Auth.Flows.OAuth;

public class OAuthFlowBuilder(IServiceCollection services) : AbstractOptions(services)
{
    public SymmetricSecurityKey StateCryptoKey { get; set; } = default!;
    
    private readonly List<object> _optionsInstances = new();
    
    public void AddOAuthProvider<TOAuthProvider, TOAuthProviderOptions>(
        string key, 
        TOAuthProviderOptions optionsInstance) 
        where TOAuthProvider : class, IOAuthProvider
        where TOAuthProviderOptions : AbstractOptions
    {
        Services.AddKeyedScoped<IOAuthProvider, TOAuthProvider>(key);
        //Services.AddScoped<IOAuthProvider, TOAuthProvider>();
        Services.AddSingleton<TOAuthProviderOptions>(optionsInstance);
        _optionsInstances.Add(optionsInstance);
    }

    public override void Build()
    {
        foreach (object optionsInstance in _optionsInstances)
        {
            (optionsInstance as AbstractOptions)!.Build();
        }
    }
}