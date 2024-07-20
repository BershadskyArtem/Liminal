using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Liminal.Auth.Jwt;

public class JwtTokenGeneratorOptions(IServiceCollection services) : AbstractOptions(services)
{
    public override void Build()
    {
    }

    public SecurityKey CryptoKey { get; set; }
    public TimeSpan AccessTokenLifetime { get; set; }
}