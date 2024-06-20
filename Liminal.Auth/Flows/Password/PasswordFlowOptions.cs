using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.Password;

public class PasswordFlowOptions(IServiceCollection services) : AbstractOptions(services)
{
    public override void Build()
    {
    }
}