using System.Security.Claims;
using Liminal.Auth.Abstractions;
using Liminal.Auth.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.Extensions;

public static class HttpContextExtensions
{
    public static async Task SignOutLiminalAsync(this HttpContext context, string tokenType)
    {
        context.Response.Cookies.Delete("Authorization");
        
        var accessTokenWithScheme = context.Request.Cookies["Authorization"];
        var refreshTokenWithScheme = context.Request.Cookies["RefreshToken"];

        string? accessToken = null;
        string? refreshToken = null;
        
        if (!string.IsNullOrWhiteSpace(accessTokenWithScheme))
        {
            accessToken = accessTokenWithScheme
                .Split(" ")
                .LastOrDefault();
        }
        
        if (!string.IsNullOrWhiteSpace(refreshTokenWithScheme))
        {
            context.Response.Cookies.Delete("RefreshToken");
            
            refreshToken = refreshTokenWithScheme
                .Split(" ")
                .LastOrDefault();
        }
        
        var tokenGenerator = context.RequestServices.GetRequiredKeyedService<ITokenGenerator>(tokenType);

        await tokenGenerator.SignOutAsync(context, accessToken, refreshToken);
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
        
        context.Response.Cookies.Append("Authorization", $"{tokenType} {tokens.AccessToken}", new CookieOptions
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
        
        context.Response.Cookies.Append("RefreshToken", $"{tokenType} {tokens.RefreshToken}", new CookieOptions
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
        
        context.Response.Cookies.Append("Authorization", $"{tokenType} {tokens.AccessToken}", new CookieOptions
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
        
        context.Response.Cookies.Append("RefreshToken", $"{tokenType} {tokens.RefreshToken}", new CookieOptions
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