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