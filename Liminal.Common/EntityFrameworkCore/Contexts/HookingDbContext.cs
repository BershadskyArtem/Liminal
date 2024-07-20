using Liminal.Common.EntityFrameworkCore.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Common.EntityFrameworkCore.Contexts;

public class HookingDbContext : DbContext
{
    protected HookingDbContext(DbContextOptions options) : base(options)
    { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(new AuditableInterceptor());
        base.OnConfiguring(optionsBuilder);
    }
}