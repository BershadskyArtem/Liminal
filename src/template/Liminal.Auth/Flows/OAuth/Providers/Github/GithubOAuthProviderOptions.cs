// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth.Providers.Github;
using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

public class GithubOAuthProviderOptions(IServiceCollection services): AbstractOptions(services)
{
    public string ClientId { get; set; } = default!;

    public string ClientSecret { get; set; } = default!;

    public string RedirectUrl { get; set; } = default!;

    public override void Build()
    {
    }
}
