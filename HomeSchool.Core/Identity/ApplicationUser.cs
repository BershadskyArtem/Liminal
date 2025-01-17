using HomeSchool.Core.Attachments.Domain;
using HomeSchool.Core.Lessons.Calendars.Domain;
using HomeSchool.Core.Lessons.Common.Domain;
using HomeSchool.Core.Lessons.Tests.Domain;
using HomeSchool.Core.Reporting.Violations.Domain;
using Liminal.Auth.Models;

namespace HomeSchool.Core.Identity;

public class ApplicationUser : AbstractUser
{
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    public virtual ICollection<Report> Complaints { get; set; } = new List<Report>();
    
    public virtual ICollection<ApplicationAttachment> Attachments { get; set; } = new List<ApplicationAttachment>();
    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    public virtual ICollection<Test> Tests { get; set; } = new List<Test>();
    public virtual ICollection<AnswerOption> QuestionAnswers { get; set; } = new List<AnswerOption>();
    public virtual ICollection<QuestionAttempt> QuestionAttempts { get; set; } = new List<QuestionAttempt>();
    public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
    public virtual ICollection<Slot> Slots { get; set; } = new List<Slot>();
    public virtual ICollection<DayTimeSpan>? TimeSpans { get; set; } = new List<DayTimeSpan>();
    public virtual ICollection<Tag>? Tags { get; set; } = new List<Tag>();
}