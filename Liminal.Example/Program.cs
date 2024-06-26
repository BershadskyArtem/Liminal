using System.Text;
using Liminal.Auth.EntityFrameworkCore;
using Liminal.Auth.Extensions;
using Liminal.Auth.Flows.MagicLink;
using Liminal.Auth.Flows.OAuth;
using Liminal.Auth.Flows.OAuth.Providers.Github;
using Liminal.Auth.Flows.Password;
using Liminal.Auth.Jwt;
using Liminal.Example;
using Liminal.Mail;
using Liminal.Mail.Implementations;
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
        .AddJwtTokenGenerator<ApplicationUser>(tokenOptions =>
        {
            tokenOptions.AccessTokenLifetime = TimeSpan.FromMinutes(3);
            tokenOptions.CryptoKey = 
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Liminal:Auth:Jwt:Secret"]!));
        })
        .AddMagickLink<ApplicationUser>(linkOptions =>
        {
            linkOptions.ActivateUrl = builder.Configuration["Liminal:Auth:MagicLink:ConfirmPath"]!;
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

builder.Services.AddMailer<ConsoleMailer>();

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

app.UseMiddleware<ExceptionMiddleware>();

app.MapPassword<ApplicationUser>();
app.MapMagic<ApplicationUser>();
app.MapOAuth<ApplicationUser>();

app.Run();

