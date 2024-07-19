// Copyright (c) Bershadsky Artyom. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using HomeSchool.Core.Attachments.Domain;
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
}
