using System.Text;
using HomeSchool.Api;
using HomeSchool.Api.Features.Reporting.Violations;
using HomeSchool.Core.Data;
using HomeSchool.Core.Identity;
using HomeSchool.Core.Reporting;
using Liminal.Auth.Endpoints;
using Liminal.Auth.EntityFrameworkCore;
using Liminal.Auth.Extensions;
using Liminal.Auth.Flows.MagicLink;
using Liminal.Auth.Flows.OAuth;
using Liminal.Auth.Flows.OAuth.Providers.Github;
using Liminal.Auth.Flows.Password;
using Liminal.Auth.Jwt;
using Liminal.Auth.Requirements;
using Liminal.Mail;
using Liminal.Mail.Implementations;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddData();
builder.Services.AddReporting();

builder.Services.AddJwtAuth(builder.Configuration);
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
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Liminal:Auth:OAuth:StateKey"] ?? throw new NullReferenceException("No state key secret.")));
            
            oAuthOptions.AddGitHub(gh =>
            {
                gh.ClientId = builder.Configuration["Liminal:Auth:OAuth:Github:ClientId"] ?? throw new NullReferenceException("ClientId for github is null");
                gh.ClientSecret = builder.Configuration["Liminal:Auth:OAuth:Github:ClientSecret"] ?? throw new NullReferenceException("Client secret for github is null");
            });
        });

    options.AddEntityFrameworkStores<ApplicationDbContext, ApplicationUser>();
});

builder.Services.AddMailer<ConsoleMailer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    IdentityModelEventSource.ShowPII = true;
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapPassword<ApplicationUser>();
app.MapMagic<ApplicationUser>();
app.MapOAuth<ApplicationUser>();
app.MapLinking<ApplicationUser>();

app.MapReport<ApplicationUser>();

app.Run();
