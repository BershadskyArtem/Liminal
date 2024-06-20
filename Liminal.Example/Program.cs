using System.Text;
using Liminal.Auth.EntityFrameworkCore;
using Liminal.Auth.Extensions;
using Liminal.Auth.Flows.OAuth;
using Liminal.Auth.Flows.OAuth.Providers.Github;
using Liminal.Auth.Flows.Password;
using Liminal.Auth.Jwt;
using Liminal.Example;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseInMemoryDatabase("inmem.db");
});

builder.Services.AddLiminalAuth(options =>
{
    options
        .AddJwtTokenGenerator(tokenOptions =>
        {
            tokenOptions.Build();
        })
        .AddPasswordFlow<ApplicationUser>()
        .AddOAuth<ApplicationUser>(oAuthOptions =>
        {
            oAuthOptions.StateCryptoKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Liminal:Auth:OAuth:StateKey"]!));
            
            oAuthOptions.AddGitHub(gh =>
            {
                gh.ClientId = builder.Configuration["Liminal:Auth:OAuth:Github:ClientId"] ?? throw new NullReferenceException("ClientId for github is null");
                gh.ClientSecret = builder.Configuration["Liminal:Auth:OAuth:Github:ClientSecret"] ?? throw new NullReferenceException("Client secret for github is null");
            });
        });

    options.AddEntityFrameworkStores<ApplicationDbContext, ApplicationUser>();
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("allow-github", policy =>
    {
        policy.AllowAnyMethod().WithOrigins("https://github.com");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("allow-github");

//app.UseHttpsRedirection();

app.MapGet("/redir", Handler)
    .WithName("Redirect to github")
    .WithOpenApi();

app.MapGet("/callback", Callback)
    .WithName("Callback for github")
    .WithOpenApi();

async Task<IResult> Handler([FromServices] OAuthFlow<ApplicationUser> flow)
{
    var result = await flow.GetRedirectUrl("github");
    return TypedResults.Redirect(result);
}

async Task<IResult> Callback(
    [FromQuery] string code,
    [FromQuery] string state,
    [FromServices] OAuthFlow<ApplicationUser> flow)
{
    var result = await flow.Callback("github", code, state, () =>
    {
        return new ApplicationUser()
        {
            Email = string.Empty
        };
    });
    
    
    
    return TypedResults.Redirect(result.User.Email);
}



app.Run();

namespace Liminal.Example
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}