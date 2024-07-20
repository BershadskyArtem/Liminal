using System.Text.Json.Serialization;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Extensions;
using Liminal.Auth.Jwt;
using Liminal.Auth.Models;
using Liminal.Auth.Results;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Liminal.Auth.Endpoints;

public static class TokenEndpoints
{
    public static IEndpointRouteBuilder MapTokenEndpoints<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app.MapRefreshToken();
        app.MapMe<TUser>();
        app.MapSignOut();
        
        return app;
    }

    public static IEndpointRouteBuilder MapRefreshToken(this IEndpointRouteBuilder app)
    {
        app
            .MapPost("api/auth/token", RefreshToken)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Summary = "Refresh JWT token";
                options.Description = "Refreshes JWT token using refresh token";
                
                return options;
            })
            .WithTags("Tokens");
        
        
        return app;
    }

    private static async Task<Results<
        Ok<GenerateTokenResult>, 
        BadRequest<string>, 
        BadRequest<GenerateTokenResult>>> RefreshToken(
        [FromQuery] string grantType,
        HttpContext context)
    {
        if (grantType != "refresh_token")
        {
            return TypedResults.BadRequest("Wrong grant type");
        }

        var result = await context.RefreshAsyncLiminal();

        Console.WriteLine(result.IsSuccess);
        
        if (result.IsSuccess)
        {
            return TypedResults.Ok(result);
        }

        return TypedResults.BadRequest(result);
    }
    
    // https://datatracker.ietf.org/doc/html/rfc7662
    public static IEndpointRouteBuilder MapMe<TUser>(this IEndpointRouteBuilder app)
        where TUser : AbstractUser
    {
        app
            .MapGet("api/auth/token/introspect", GetMe<TUser>)
            .RequireAuthorization()
            .WithOpenApi(options =>
            {
                options.Description = "Gets a user info. Used instead of token introspection. For more user info use another method.";
                options.Summary = "Gets a minimal user info";

                return options;
            })
            .WithTags("User");

        return app;
    }

    public class GetMeResponse
    {
        [JsonPropertyName("sub")] public string Id { get; set; }
        [JsonPropertyName("username")] public string? Username { get; set; }
        [JsonPropertyName("email")] public string Email { get; set; }
        [JsonPropertyName("is_confirmed")] public bool IsConfirmed { get; set; }
        [JsonPropertyName("role")] public string Role { get; set; }
        [JsonPropertyName("avatar_url")] public string? AvatarUrl { get; set; }
        [JsonPropertyName("token_type")] public string TokenType { get; set; } = "Bearer";

        protected GetMeResponse()
        { }

        public static GetMeResponse Create<TUser>(TUser user)
            where TUser : AbstractUser
        {
            return new GetMeResponse
            {
                Id = user.Id.ToString(),
                Email = user.Email,
                IsConfirmed = user.IsConfirmed,
                Role = user.Role,
                Username = user.Username
            };
        }
    }

    private static async Task<Results<UnauthorizedHttpResult, Ok<GetMeResponse>>> GetMe<TUser>(
        IAuthContext<TUser> auth,
        HttpContext context)
        where TUser : AbstractUser
    {
        var user = await auth.Current();

        if (user is null)
        {
            return TypedResults.Unauthorized();
        }
        
        var response = GetMeResponse.Create(user);

        return TypedResults.Ok(response);
    }
    
    public static void MapSignOut(this IEndpointRouteBuilder app)
    {
        app.MapGet("api/auth/logout", LogOut)
            .AllowAnonymous()
            .WithOpenApi(options =>
            {
                options.Summary = "Sign out user";
                options.Description = "Sign out user. Deletes cookie and refresh token";
                
                return options;
            });
    }

    public static async Task<Ok> LogOut(
        [FromQuery] string _,
        HttpContext context)
    {
        await context.SignOutLiminalAsync(JwtDefaults.Scheme);
        return TypedResults.Ok();
    }
}