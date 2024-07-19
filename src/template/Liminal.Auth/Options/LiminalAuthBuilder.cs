// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Options;
using Liminal.Auth.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public class LiminalAuthBuilder(IServiceCollection services): AbstractOptions(services)
{
    private readonly List<object> _generatorOptions = new();

    public TokenGeneratorBuilder AddTokenGenerator<TGenerator, TOptions>(string name, TOptions optionsInstance)
        where TGenerator : ITokenGenerator
        where TOptions : AbstractOptions
    {
        var builder = new TokenGeneratorBuilder(this.Services);

        this.Services.AddKeyedScoped(typeof(ITokenGenerator), name, typeof(TGenerator));

        this.Services.AddSingleton(optionsInstance);

        this._generatorOptions.Add(optionsInstance);

        return builder;
    }

    public override void Build()
    {
        foreach (var generatorOption in this._generatorOptions)
        {
            (generatorOption as AbstractOptions) !.Build();
        }
    }
}
