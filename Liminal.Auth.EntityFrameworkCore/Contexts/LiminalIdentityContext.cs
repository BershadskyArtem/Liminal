using Liminal.Auth.Models;
using Liminal.Common.EntityFrameworkCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Auth.EntityFrameworkCore.Contexts;

public class LiminalIdentityContext<TUser> : HookingDbContext
    where TUser : AbstractUser
{
    public LiminalIdentityContext(DbContextOptions options) : base(options)
    { }
    
    public DbSet<TUser> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<AccountToken> Passwords { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }
    public DbSet<AccountClaim> AccountClaims { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }
}