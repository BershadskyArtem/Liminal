// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.MagicLink;

public class MagicLinkOptions(IServiceCollection services): AbstractOptions(services)
{
    public string ActivateUrl { get; set; } = default!;

    public override void Build()
    {
    }
}
