// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.OAuth.Providers;

/// <summary>
/// Actually funny name.
/// </summary>
public class OAuthProvidersProvider(IServiceProvider serviceProvider)
    : IOAuthProvidersProvider
{
    public IOAuthProvider GetProvider(string key)
        => serviceProvider.GetRequiredKeyedService<IOAuthProvider>(key);
}
