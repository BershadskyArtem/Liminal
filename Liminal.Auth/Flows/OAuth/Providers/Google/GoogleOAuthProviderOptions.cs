using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.OAuth.Providers.Google;

public class GoogleOAuthProviderOptions(IServiceCollection services) : AbstractOptions(services)
{
    public List<string> Scopes { get; set; } = new ()
    {
        "https://www.googleapis.com/auth/userinfo.profile",
        "https://www.googleapis.com/auth/userinfo.email"
    };
    public string ClientId { get; set; } = default!;
    public string ClientSecret { get; set; } = default!;
    public string RedirectUri { get; set; } = default!;

    public override void Build()
    {
        
    }
}