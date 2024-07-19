// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth.Providers;

public interface IOAuthProvidersProvider
{
    public IOAuthProvider GetProvider(string key);
}
