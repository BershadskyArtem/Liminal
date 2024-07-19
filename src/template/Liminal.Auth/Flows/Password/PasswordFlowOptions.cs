// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.Password;
using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

public class PasswordFlowOptions(IServiceCollection services): AbstractOptions(services)
{
    public string ActivateUrl { get; set; } = default!;

    public string ConfirmedRole { get; set; } = default!;

    public override void Build()
    {
    }
}
