// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Options;
using Liminal.Auth.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public class TokenGeneratorBuilder(IServiceCollection services): AbstractOptions(services)
{
    private readonly List<object> _optionsInstances = new();

    public TokenGeneratorBuilder AddFlow<TAuthFlow>()
        where TAuthFlow : class, IAuthFlow
    {
        this.Services.AddScoped<TAuthFlow>();
        return this;
    }

    public TokenGeneratorBuilder AddFlow<TAuthFlow, TAuthFlowOptions>(TAuthFlowOptions optionsInstance)
        where TAuthFlow : class, IAuthFlow
        where TAuthFlowOptions : AbstractOptions
    {
        this.AddFlow<TAuthFlow>();
        this.Services.AddSingleton<TAuthFlowOptions>(optionsInstance);
        this._optionsInstances.Add(optionsInstance);
        return this;
    }

    public override void Build()
    {
        foreach (var optionsInstance in this._optionsInstances)
        {
            (optionsInstance as AbstractOptions) !.Build();
        }
    }
}
