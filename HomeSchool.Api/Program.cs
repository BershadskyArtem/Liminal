using System.Text;
using HomeSchool.Api;
using HomeSchool.Api.Features.Attachments;
using HomeSchool.Api.Features.Reporting.Violations;
using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Data;
using HomeSchool.Core.Identity;
using HomeSchool.Core.Reporting;
using Liminal.Auth.Endpoints;
using Liminal.Auth.EntityFrameworkCore;
using Liminal.Auth.Extensions;
using Liminal.Auth.Flows.MagicLink;
using Liminal.Auth.Flows.OAuth;
using Liminal.Auth.Flows.OAuth.Providers.Github;
using Liminal.Auth.Flows.OAuth.Providers.Google;
using Liminal.Auth.Flows.Password;
using Liminal.Auth.Jwt;
using Liminal.Auth.Requirements;
using Liminal.Common;
using Liminal.Mail;
using Liminal.Mail.Implementations;
using Liminal.Storage;
using Liminal.Storage.EntityFrameworkCore;
using Liminal.Storage.S3;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

//  https://www.c-sharpcorner.com/article/restrict-uploaded-file-size-in-asp-net-core2/

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c => 
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Project Home School Api",
        Version = "v1"
    });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Cookie,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddData(builder.Configuration);
builder.Services.AddReporting();

builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddAuthorization(cfg =>
{
    cfg.AddPolicy(PolicyDefaults.AdminName, options =>
    {
        options.RequireRole(RoleDefaults.Admin, RoleDefaults.SuperAdmin);
    });
    
    cfg.AddPolicy(PolicyDefaults.PaidAndAdminName, options =>
    {
        options.RequireRole(RoleDefaults.Medium, RoleDefaults.Premium, RoleDefaults.Admin, RoleDefaults.SuperAdmin);
    });
    
    cfg.AddPolicy(PolicyDefaults.ConfirmedAccount, options =>
    {
        options.RequireClaim("confirmed", "True");
    });
    
});


builder.Services.AddLiminalFileStore(options =>
{
    options.UseEntityFrameworkStores<ApplicationDbContext, ApplicationAttachment>();
   
    options.UseS3Disk("S3", s3Options =>
    {
        
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

            oAuthOptions.AddGoogle(go =>
            {
                go.ClientId = builder.Configuration["Liminal:Auth:OAuth:Google:ClientId"] ?? throw new NullReferenceException("ClientId for google is null");
                go.ClientSecret = builder.Configuration["Liminal:Auth:OAuth:Google:ClientSecret"] ?? throw new NullReferenceException("ClientSecret for google is null");
                go.RedirectUri = builder.Configuration["Liminal:Auth:OAuth:Google:RedirectUri"] ?? throw new NullReferenceException("RedirectUri for google is null");
            });
        });

    options.AddEntityFrameworkStores<ApplicationDbContext, ApplicationUser>();
});

builder.Services.AddMailer<ConsoleMailer>();

var frontendConfig = builder.Services.AddLiminalConfig(options =>
{
    options.DefaultRedirectUrl = builder.Configuration["Liminal:DefaultRedirectUrl"] 
                          ?? throw new NullReferenceException("DefaultRedirectUrl not present in config");
    
    options.FrontendHost = builder.Configuration["Liminal:FrontendHost"] 
                                 ?? throw new NullReferenceException("Frontend url is not present in config");
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("main-cors", policy =>
    {
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins("https://github.com");
        
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins("https://google.com");
        
        policy
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .WithOrigins("https://zoom.com");
        
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(frontendConfig.FrontendHost);
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

app.UseCors("main-cors");

app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapPassword<ApplicationUser>();
app.MapMagic<ApplicationUser>();
app.MapOAuth<ApplicationUser>();
app.MapLinking<ApplicationUser>();
app.MapTokenEndpoints<ApplicationUser>();
app.MapAttachments();

app.MapReport<ApplicationUser>();

using var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
context.Database.EnsureCreated();

app.Run();
