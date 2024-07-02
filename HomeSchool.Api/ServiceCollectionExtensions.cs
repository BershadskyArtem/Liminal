using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace HomeSchool.Api;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var tokenFullValidationParams = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Liminal:Auth:Jwt:Secret"] ?? throw new ArgumentException())),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true
        };
        
        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(opt =>
        {
            opt.SaveToken = true;
            opt.TokenValidationParameters = tokenFullValidationParams;
            opt.MapInboundClaims = false;
            opt.Events = new JwtBearerEvents()
            {
                OnMessageReceived = context =>
                {
                    var token = context.Request.Cookies["Authorization"];
                    if (string.IsNullOrWhiteSpace(token))
                    {
                        return Task.CompletedTask;
                    }

                    // Get the actual token or jiberish. 
                    // And let ShitSoft handle anything else.
                    var leftPart = token.Split(" ").Last();

                    context.Token = leftPart;
                    return Task.CompletedTask;
                },
            };
        });

        return services;
    }
}