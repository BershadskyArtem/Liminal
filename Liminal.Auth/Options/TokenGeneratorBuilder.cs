using Liminal.Auth.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Options;

public class TokenGeneratorBuilder(IServiceCollection services) : AbstractOptions(services)
{
    private List<object> _optionsInstances = new();
    
    public TokenGeneratorBuilder AddFlow<TAuthFlow>() where TAuthFlow : class, IAuthFlow 
    {
        Services.AddScoped<TAuthFlow>();
        return this;
    }
    
    public TokenGeneratorBuilder AddFlow<TAuthFlow, TAuthFlowOptions>(TAuthFlowOptions optionsInstance) 
        where TAuthFlow : class, IAuthFlow
        where TAuthFlowOptions : AbstractOptions
    {
        AddFlow<TAuthFlow>();
        Services.AddSingleton<TAuthFlowOptions>(optionsInstance);
        _optionsInstances.Add(optionsInstance);
        return this;
    }

    public override void Build()
    {
        foreach (object optionsInstance in _optionsInstances)
        {
            (optionsInstance as AbstractOptions)!.Build();
        }
    }
}