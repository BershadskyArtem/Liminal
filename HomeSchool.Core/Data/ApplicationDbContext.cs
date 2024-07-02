using HomeSchool.Core.Identity;
using HomeSchool.Core.Reporting.Violations.Domain;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Microsoft.EntityFrameworkCore;

namespace HomeSchool.Core.Data;

public class ApplicationDbContext : LiminalIdentityContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Report> Reports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Report).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}