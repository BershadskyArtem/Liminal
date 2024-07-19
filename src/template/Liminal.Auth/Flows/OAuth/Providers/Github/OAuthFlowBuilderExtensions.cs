// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Liminal.Auth.Flows.OAuth.Providers.Github;

public static class OAuthFlowBuilderExtensions
{
    public static OAuthFlowBuilder AddGitHub(
        this OAuthFlowBuilder builder,
        Action<GithubOAuthProviderOptions> cfg)
    {
        var options = new GithubOAuthProviderOptions(builder.Services);

        cfg(options);

        builder.AddOAuthProvider<GithubOAuthProvider, GithubOAuthProviderOptions>("github", options);

        return builder;
    }
}
