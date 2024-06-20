using Liminal.Auth.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Options;

public class LiminalAuthBuilder(IServiceCollection services) : AbstractOptions(services)
{
    private readonly List<object> _generatorOptions = new();

    public TokenGeneratorBuilder AddTokenGenerator<TGenerator, TOptions>(string name, TOptions optionsInstance) 
        where TGenerator : ITokenGenerator 
        where TOptions : AbstractOptions
    {
        var builder = new TokenGeneratorBuilder(Services);
        
        Services.AddScoped(typeof(TGenerator));

        Services.AddSingleton(optionsInstance);

        _generatorOptions.Add(optionsInstance);
        
        return builder;
    }
    
    public override void Build()
    {
        foreach (object generatorOption in _generatorOptions)
        {
            (generatorOption as AbstractOptions)!.Build();
        }
    }
}