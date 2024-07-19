// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Liminal.Auth.Flows.OAuth;

public class OAuthFlowBuilder(IServiceCollection services): AbstractOptions(services)
{
    public SymmetricSecurityKey StateCryptoKey { get; set; } = default!;

    private readonly List<object> _optionsInstances = new();

    public void AddOAuthProvider<TOAuthProvider, TOAuthProviderOptions>(
        string key,
        TOAuthProviderOptions optionsInstance)
        where TOAuthProvider : class, IOAuthProvider
        where TOAuthProviderOptions : AbstractOptions
    {
        this.Services.AddKeyedScoped<IOAuthProvider, TOAuthProvider>(key);
        this.Services.AddSingleton(optionsInstance);
        this._optionsInstances.Add(optionsInstance);
    }

    public override void Build()
    {
        foreach (var optionsInstance in this._optionsInstances)
        {
            (optionsInstance as AbstractOptions) !.Build();
        }
    }
}
