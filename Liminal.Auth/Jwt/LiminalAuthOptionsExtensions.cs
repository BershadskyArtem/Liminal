using Liminal.Auth.Options;

namespace Liminal.Auth.Jwt;

public static class LiminalAuthOptionsExtensions
{
    public static TokenGeneratorBuilder AddJwtTokenGenerator(
        this LiminalAuthBuilder builder, 
        Action<JwtTokenGeneratorOptions> cfg)
    {
        var options = new JwtTokenGeneratorOptions(builder.Services);

        cfg(options);
        
        return builder.AddTokenGenerator<JwtTokenGenerator, JwtTokenGeneratorOptions>("jwt", options);
    }
}