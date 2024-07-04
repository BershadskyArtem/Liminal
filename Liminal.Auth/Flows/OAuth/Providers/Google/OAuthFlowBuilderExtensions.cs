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