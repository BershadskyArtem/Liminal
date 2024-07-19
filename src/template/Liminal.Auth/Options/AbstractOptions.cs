// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

public abstract class AbstractOptions(IServiceCollection services)
{
    public readonly IServiceCollection Services = services;

    public string DefaultRole { get; set; } = default!;

    public abstract void Build();
}
