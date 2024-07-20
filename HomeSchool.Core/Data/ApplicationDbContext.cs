using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Identity;
using HomeSchool.Core.Lessons.Calendars.Domain;
using HomeSchool.Core.Lessons.Common.Domain;
using HomeSchool.Core.Lessons.Tests.Domain;
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

    public DbSet<AnswerOption> AnswerOptions { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionAttempt> QuestionAttempts { get; set; }
    public DbSet<Test> Tests { get; set; }
    public DbSet<TestAttempt> TestAttempts { get; set; }
    public DbSet<DayTimeSpan> DayTimeSpans { get; set; }
    public DbSet<Slot> Slots { get; set; }
    public DbSet<Tag> Tags { get; set; }
    
    public DbSet<ApplicationAttachment> Set() => Set<ApplicationAttachment>();
    
    public DbSet<Report> Reports { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Report).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Test).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Tag).Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(Slot).Assembly);
        
        base.OnModelCreating(modelBuilder);
    }
}