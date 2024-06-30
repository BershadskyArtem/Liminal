using System.Text;
using Liminal.Auth.Endpoints;
using Liminal.Auth.EntityFrameworkCore;
using Liminal.Auth.Extensions;
using Liminal.Auth.Flows.MagicLink;
using Liminal.Auth.Flows.OAuth;
using Liminal.Auth.Flows.OAuth.Providers.Github;
using Liminal.Auth.Flows.Password;
using Liminal.Auth.Jwt;
using Liminal.Auth.Requirements;
using Liminal.Example;
using Liminal.Mail;
using Liminal.Mail.Implementations;
using Liminal.Reporting;
using Liminal.Reporting.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
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

builder.Services.AddLiminalReporting<ApplicationDbContext>();

var tokenFullValidationParams = new TokenValidationParameters()
{
    ValidateIssuerSigningKey = true,
    ClockSkew = TimeSpan.Zero,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Liminal:Auth:Jwt:Secret"]!)),
    ValidateIssuer = false,
    ValidateAudience = false,
    RequireExpirationTime = false,
    ValidateLifetime = true
};

builder.Services.AddAuthentication(opt =>
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

builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy(PolicyDefaults.AdminName, options =>
    {
        options.RequireRole(RoleDefaults.Admin, RoleDefaults.SuperAdmin);
    });
});

builder.Services.AddLiminalAuth<ApplicationUser>(options =>
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
            linkOptions.DefaultRole = RoleDefaults.Admin;
            linkOptions.ActivateUrl = builder.Configuration["Liminal:Auth:MagicLink:ConfirmPath"]!;
        })
        .AddPasswordFlow<ApplicationUser>(passwordOptions =>
        {
            passwordOptions.DefaultRole = RoleDefaults.NotConfirmed;
            passwordOptions.ConfirmedRole = RoleDefaults.Basic;
            passwordOptions.ActivateUrl = builder.Configuration["Liminal:Auth:Password:ConfirmPath"]!;
        })
        .AddOAuth<ApplicationUser>(oAuthOptions =>
        {
            oAuthOptions.DefaultRole = RoleDefaults.Basic;
            
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
    IdentityModelEventSource.ShowPII = true;
}

app.UseCors("allow-github");

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapPassword<ApplicationUser>();
app.MapMagic<ApplicationUser>();
app.MapOAuth<ApplicationUser>();
app.MapLinking<ApplicationUser>();
app.MapReport<ApplicationUser>();

app.Run();

