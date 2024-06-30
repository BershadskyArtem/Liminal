using Liminal.Auth.Extensions;
using Liminal.Auth.Jwt;
using Liminal.Auth.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Liminal.Auth.Flows.OAuth;

public static class OAuthEndpoints
{
    public static IEndpointRouteBuilder MapOAuth<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app.MapRedirect<TUser>();
        app.MapCallback<TUser>();
        
        return app;
    }
    
    public static IEndpointRouteBuilder MapRedirect<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app
            .MapGet("api/auth/oauth/{provider}", RedirectToProvider<TUser>)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Summary = "OAuth redirect to provider";
                options.Description = "Redirects to a given provider for authentification";

                return options;
            })
            .WithTags("OAuth");
        
        return app;
    }
    
    public static IEndpointRouteBuilder MapCallback<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app
            .MapGet("api/auth/oauth/callback/{provider}", Callback<TUser>)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Summary = "OAuth callback";
                options.Description = "Callback for OAuth providers";

                return options;
            })
            .WithTags("OAuth");
        
        return app;
    }
    
    public static async Task<RedirectHttpResult> RedirectToProvider<TUser>(
        [FromRoute] string provider,
        [FromServices] OAuthFlow<TUser> flow)
        where TUser : AbstractUser
    {
        var result = await flow.GetRedirectUrl(provider);
        return TypedResults.Redirect(result);
    }
    
    public static async Task<Results<BadRequest, RedirectHttpResult, BadRequest<string>>> Callback<TUser>(
        [FromRoute] string provider,
        [FromQuery] string code,
        [FromQuery] string state,
        [FromServices] OAuthFlow<TUser> flow,
        HttpContext context)
        where TUser : AbstractUser
    {
        var result = await flow.Callback(provider, code, state, () =>
        {
            var result = Activator.CreateInstance<TUser>();
            result.Id = Guid.NewGuid();
            return result;
        });

        if (!result.IsSuccess)
        {
            return TypedResults.BadRequest();
        }

        var generateTokenResult = await context.SignInAsyncLiminal(result.Principal!, JwtDefaults.Scheme);

        if (!generateTokenResult.IsSuccess)
        {
            return TypedResults.BadRequest("Cannot generate cookie.");
        }
        
        if (string.IsNullOrWhiteSpace(result.RedirectAfter))
        {
            return TypedResults.Redirect("/", false);
        }
        
        return TypedResults.Redirect(result.RedirectAfter, false);
    }
}