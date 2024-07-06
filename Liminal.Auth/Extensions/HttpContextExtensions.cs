using System.Security.Claims;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Extensions;

public static class HttpContextExtensions
{
    public static Task SignOutLiminalAsync(this HttpContext context)
    {
        // TODO: Delete token in database.
        context.Response.Cookies.Delete("Authorization");
        context.Response.Cookies.Delete("RefreshToken");
        return Task.CompletedTask;
    }
    
    public static async Task<GenerateTokenResult> SignInAsyncLiminal(this HttpContext context, ClaimsPrincipal principal, string tokenType)
    {
        var tokenGenerator = context.RequestServices.GetRequiredKeyedService<ITokenGenerator>(tokenType);

        var tokens = await tokenGenerator.GenerateToken(principal);

        if (!tokens.IsSuccess)
        {
            return tokens;
        }

        //context.Response.Headers.Append("Authorization", $"{tokenType} {tokens.AccessToken}");
        
        context.Response.Cookies.Append("Authorization", $"{tokenType} {tokens.AccessToken}", new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        if (string.IsNullOrWhiteSpace(tokens.RefreshToken))
        {
            return tokens;
        }
        
        context.Response.Cookies.Append("RefreshToken", $"{tokenType} {tokens.RefreshToken}", new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddDays(90),
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
        
        return tokens;
    }
    
    public static async Task<GenerateTokenResult> RefreshAsyncLiminal(this HttpContext context)
    {
        if (!context.Request.Cookies.TryGetValue("RefreshToken", out var refreshTokenCookie))
        {
            return GenerateTokenResult.Failure("Invalid token.");
        }

        var tokenType = refreshTokenCookie.Split(" ")[0];
        var tokenValue = refreshTokenCookie.Split(" ")[1];
        
        var tokenGenerator = context.RequestServices.GetRequiredKeyedService<ITokenGenerator>(tokenType);

        var tokens = await tokenGenerator.RefreshToken(tokenValue);

        if (!tokens.IsSuccess)
        {
            return tokens;
        }
        
        context.Response.Cookies.Append("Authorization", $"{tokenType} {tokens.AccessToken}", new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddDays(30),
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        if (string.IsNullOrWhiteSpace(tokens.RefreshToken))
        {
            return tokens;
        }
        
        context.Response.Cookies.Append("RefreshToken", $"{tokenType} {tokens.RefreshToken}", new CookieOptions()
        {
            Expires = DateTimeOffset.UtcNow.AddDays(90),
            HttpOnly = true,
            IsEssential = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });
        
        return tokens;
    }
    
}