using Liminal.Auth.Models;
using Liminal.Auth.Options;

namespace Liminal.Auth.Flows.Password;

public static class TokenGeneratorOptionsExtensions
{
    public static TokenGeneratorBuilder AddPasswordFlow<TUser>(
        this TokenGeneratorBuilder builder)
        where TUser : AbstractUser
    {
        return builder.AddPasswordFlow<TUser>((opt) => { });
    }
    
    public static TokenGeneratorBuilder AddPasswordFlow<TUser>(
        this TokenGeneratorBuilder builder,
        Action<PasswordFlowOptions> cfg) 
        where TUser : AbstractUser
    {
        var options = new PasswordFlowOptions(builder.Services);

        cfg(options);
        
        return builder.AddFlow<PasswordFlow<TUser>, PasswordFlowOptions>(options);
    }
}