using Liminal.Auth.Models;
using Liminal.Auth.Options;

namespace Liminal.Auth.Jwt;

public static class LiminalAuthOptionsExtensions
{
    public static TokenGeneratorBuilder AddJwtTokenGenerator<TUser>(
        this LiminalAuthBuilder builder, 
        Action<JwtTokenGeneratorOptions> cfg) 
        where TUser : AbstractUser
    {
        var options = new JwtTokenGeneratorOptions(builder.Services);

        cfg(options);
        
        return builder.AddTokenGenerator<JwtTokenGenerator<TUser>, JwtTokenGeneratorOptions>(JwtDefaults.Scheme, options);
    }
}