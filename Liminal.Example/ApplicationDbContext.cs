using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Reporting.Abstractions;
using Liminal.Reporting.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Example;

public class ApplicationDbContext : LiminalIdentityContext<ApplicationUser>, IReportingDbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Report> Reports { get; set; }
}