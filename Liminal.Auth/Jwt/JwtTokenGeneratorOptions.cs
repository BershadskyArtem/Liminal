using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Jwt;

public class JwtTokenGeneratorOptions(IServiceCollection services) : AbstractOptions(services)
{
    public override void Build()
    {
        Services.AddSingleton<JwtTokenGenerator>();
    }
}