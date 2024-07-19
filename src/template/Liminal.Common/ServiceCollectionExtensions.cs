// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Common;
using Liminal.Common.Options;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static FrontendOptions AddLiminalConfig(this IServiceCollection services, Action<FrontendOptions> configure)
    {
        var options = new FrontendOptions();

        configure(options);

        services.AddSingleton(options);

        return options;
    }
}
