using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.MagicLink;

public class MagicLinkOptions(IServiceCollection services) : AbstractOptions(services)
{
    public string ActivateUrl { get; set; } = default!;
    
    public override void Build()
    {
        
    }
}