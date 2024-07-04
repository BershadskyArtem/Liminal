using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Identity;
using HomeSchool.Core.Reporting.Violations.Domain;
using Liminal.Auth.EntityFrameworkCore.Contexts;
using Liminal.Storage.EntityFrameworkCore.Abstractions;
using Liminal.Storage.Models;
using Microsoft.EntityFrameworkCore;

namespace HomeSchool.Core.Data;

public class ApplicationDbContext : LiminalIdentityContext<ApplicationUser>,
    IAttachmentDbContext<ApplicationAttachment>
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    { }

    public DbSet<Report> Reports { get; set; }
    public DbSet<FileAttachment> FileAttachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Report).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FileAttachment).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<ApplicationAttachment> Set() => Set<ApplicationAttachment>();
}