using System.Text.Json.Serialization;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Flows.OAuth;
using Liminal.Auth.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Liminal.Auth.Endpoints;

public static class LinkingEndpoints
{
    public static IEndpointRouteBuilder MapLinking<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app.MapLinkAccount<TUser>();
        app.MapUnLinkAccount<TUser>();

        return app;
    }
    
    public class LinkAccountRequest
    {
        [JsonPropertyName("provider")] public string Provider { get; set; } = default!;
        [JsonPropertyName("redirect_after")] public string? RedirectAfter { get; set; }
    }
    
    public static IEndpointRouteBuilder MapLinkAccount<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app
            .MapPost("api/auth/link", LinkAccount<TUser>)
            .RequireAuthorization()
            .WithOpenApi(options =>
            {
                options.Summary = "Link account to user";
                options.Description = "Links account from body to a user";
                return options;
            })
            .WithTags("Linking");
        
        return app;
    }

    private static async Task<
        Results<UnauthorizedHttpResult, NotFound, Ok, BadRequest, RedirectHttpResult>
    > LinkAccount<TUser>(
            [FromBody] LinkAccountRequest req,
            [FromServices] IAuthContext<TUser> auth, 
            [FromServices] IAccountStore accountStore,
            [FromServices] OAuthFlow<TUser> oAuthFlow,
            [FromServices] IAccountLinker<TUser> accountLinker,
            HttpContext context)
        where TUser : AbstractUser
    {
        if (!auth.IsConfirmed)
        {
            return TypedResults.Unauthorized();
        }

        var user = await auth.Current();

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        // Important: OAuthFlow automatically links account to the target.
        var redirectUrl = await oAuthFlow.GetRedirectUrl(req.Provider, req.RedirectAfter ?? "/", linkingTargetId: user.Id);
        
        return TypedResults.Redirect(redirectUrl);
    }

    public class UnLinkAccountRequest
    {
        [JsonPropertyName("provider")] public string Provider { get; set; } = default!;
    }
    
    public static IEndpointRouteBuilder MapUnLinkAccount<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app
            .MapDelete("api/auth/link", UnLinkAccount<TUser>)
            .RequireAuthorization()
            .WithOpenApi(options =>
            {
                options.Summary = "UnLink account";
                options.Description = "UnLinks OAuth account from a user";
                return options;
            })
            .WithTags("Linking");
        
        return app;
    }

    private static async Task<
        Results<UnauthorizedHttpResult, NotFound, Ok, BadRequest>
    > UnLinkAccount<TUser>(
        [FromBody] LinkAccountRequest req,
        [FromServices] IAuthContext<TUser> auth,
        [FromServices] IAccountStore accountStore,
        [FromServices] IAccountLinker<TUser> accountLinker,
        HttpContext context)
        where TUser : AbstractUser
    {
        if (!auth.IsConfirmed)
        {
            return TypedResults.Unauthorized();
        }

        var user = await auth.Current();

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }

        var account = await accountStore.GetByProviderAsync(user.Id, req.Provider);

        if (account is null)
        {
            return TypedResults.NotFound();
        }
        
        var result = await accountLinker.UnlinkAccount(user, account, Activator.CreateInstance<TUser>,  true);

        if (result)
        {
            return TypedResults.Ok();
        }

        return TypedResults.BadRequest();
    }
}