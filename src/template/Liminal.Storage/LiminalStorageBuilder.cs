// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage;
using Liminal.Storage.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public class LiminalStorageBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; set; } = services;

    public LiminalStorageBuilder AddDisk<TOptions, TDisk>(string name, TOptions options, Func<IServiceProvider, string, TDisk> factory)
        where TDisk : class, IDisk
        where TOptions : class
    {
        this.Services.AddKeyedScoped<IDisk, TDisk>(name, (provider, _) => factory(provider, name));
        this.Services.AddKeyedSingleton(typeof(TOptions), name, options);

        return this;
    }
}
