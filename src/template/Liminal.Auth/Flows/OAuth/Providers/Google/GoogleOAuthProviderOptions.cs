// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.OAuth.Providers.Google;

public class GoogleOAuthProviderOptions(IServiceCollection services): AbstractOptions(services)
{
#pragma warning disable CA1002
    public List<string> Scopes { get; } =
#pragma warning restore CA1002
    [
        "https://www.googleapis.com/auth/userinfo.profile",
        "https://www.googleapis.com/auth/userinfo.email"
    ];

    public string ClientId { get; set; } = default!;

    public string ClientSecret { get; set; } = default!;

    public string RedirectUri { get; set; } = default!;

    public override void Build()
    {
    }
}
