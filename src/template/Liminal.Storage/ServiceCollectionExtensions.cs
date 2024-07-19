// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Storage;
using Liminal.Storage.Abstractions;
using Liminal.Storage.Implementations;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLiminalFileStore(this IServiceCollection services, Action<LiminalStorageBuilder> configure)
    {
        services.AddScoped<IFileStore, DefaultFileStore>();

        var options = new LiminalStorageBuilder(services);

        configure(options);

        return services;
    }
}
