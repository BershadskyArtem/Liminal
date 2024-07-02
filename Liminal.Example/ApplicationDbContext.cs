using Liminal.Auth.EntityFrameworkCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Example;

public class ApplicationDbContext : LiminalIdentityContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    { }
}