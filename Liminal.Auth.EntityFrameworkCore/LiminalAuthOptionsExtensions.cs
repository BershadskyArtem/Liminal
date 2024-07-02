using Liminal.Auth.Abstractions;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Auth.EntityFrameworkCore.Implementations;
using Liminal.Auth.Models;
using Liminal.Auth.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Liminal.Auth.EntityFrameworkCore;

public static class LiminalAuthOptionsExtensions
{
    public static LiminalAuthBuilder AddEntityFrameworkStores<TDbContext, TUser>(this LiminalAuthBuilder builder)
        where TUser : AbstractUser 
        where TDbContext : LiminalIdentityContext<TUser>
    {
        builder.Services.AddScoped<IAccountStore, AccountStore<TDbContext, TUser>>();
        builder.Services.AddScoped<IPasswordStore, PasswordStore<TDbContext, TUser>>();
        builder.Services.AddScoped<IUserStore<TUser>, UserStore<TDbContext, TUser>>();
        builder.Services.AddScoped<IUserTokenStore, UserTokenStore<TDbContext, TUser>>();
        builder.Services.AddScoped<IClaimsStore, ClaimsStore<TDbContext, TUser>>();
        
        return builder;
    }
}