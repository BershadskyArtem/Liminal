// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth.Providers.Google;

public static class OAuthFlowBuilderExtensions
{
    public static OAuthFlowBuilder AddGoogle(
        this OAuthFlowBuilder builder,
        Action<GoogleOAuthProviderOptions> cfg)
    {
        var options = new GoogleOAuthProviderOptions(builder.Services);
        cfg(options);
        builder.AddOAuthProvider<GoogleOAuthProvider, GoogleOAuthProviderOptions>("google", options);
        return builder;
    }
}
