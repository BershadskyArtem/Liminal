using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.Password;

public class PasswordFlowOptions(IServiceCollection services) : AbstractOptions(services)
{
    public string ActivateUrl { get; set; }
    
    public override void Build()
    {
    }
}