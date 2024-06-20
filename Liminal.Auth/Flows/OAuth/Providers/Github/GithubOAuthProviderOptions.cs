using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.OAuth.Providers.Github;

public class GithubOAuthProviderOptions(IServiceCollection services) : AbstractOptions(services)
{
    public string ClientId { get; set; } = default!; 
    public string ClientSecret { get; set; } = default!;
    public string RedirectUrl { get; set; } = default!;
    
    public override void Build()
    {
        
    }
}