using Liminal.Common.EntityFrameworkCore.Abstractions;
using Liminal.Reporting.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Liminal.Reporting.Abstractions;

public interface IReportingDbContext : IModuleDbContext
{
    public DbSet<Report> Reports { get; set; }
}