using Liminal.Auth.Flows.OAuth.Providers;
using Liminal.Auth.Models;
using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Flows.OAuth;

public static class TokenGeneratorOptionsExtensions
{
    public static TokenGeneratorBuilder AddOAuth<TUser>(this TokenGeneratorBuilder builder, Action<OAuthFlowBuilder> cfg) 
        where TUser : AbstractUser
    {
        var options = new OAuthFlowBuilder(builder.Services);

        cfg(options);

        builder.Services.AddScoped<OAuthFlow<TUser>>();
        builder.Services.AddScoped<IOAuthProvidersProvider, OAuthProvidersProvider>();
        builder.Services.AddScoped<IStateGenerator, StateGenerator>();
        builder.Services.AddSingleton<StateGenerator>();
        builder.Services.AddScoped<OAuthProvidersProvider>();
        
        return builder.AddFlow<OAuthFlow<TUser>, OAuthFlowBuilder>(options);
    }
}