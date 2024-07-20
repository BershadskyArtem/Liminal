﻿// <auto-generated />
using System;
using HomeSchool.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace HomeSchool.Core.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240720213058_AddedTagsSlotsAndDayTime")]
    partial class AddedTagsSlotsAndDayTime
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("HomeSchool.Core.Attachments.Domain.ApplicationAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AnswerOptionId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset?>("BecameNotTransientAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("DiskName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsTransient")
                        .HasColumnType("boolean");

                    b.Property<string>("MimeType")
                        .HasColumnType("text");

                    b.Property<Guid?>("QuestionAttemptId")
                        .HasColumnType("uuid");

                    b.Property<Guid?>("QuestionId")
                        .HasColumnType("uuid");

                    b.Property<long>("Size")
                        .HasColumnType("bigint");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("ApplicationAttachment");
                });

            modelBuilder.Entity("HomeSchool.Core.Identity.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Username")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Calendars.Domain.DayTimeSpan", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("EndTime")
                        .HasColumnType("interval");

                    b.Property<TimeSpan>("RecurrenceTime")
                        .HasColumnType("interval");

                    b.Property<Guid>("SlotId")
                        .HasColumnType("uuid");

                    b.Property<TimeSpan>("StartTime")
                        .HasColumnType("interval");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("SlotId");

                    b.ToTable("DayTimeSpans");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Calendars.Domain.Slot", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(300)
                        .HasColumnType("character varying(300)");

                    b.Property<DateTimeOffset>("FinishDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRecurring")
                        .HasColumnType("boolean");

                    b.Property<int?>("RecurringEveryDays")
                        .HasColumnType("integer");

                    b.Property<int?>("RecurringEveryWeekDay")
                        .HasColumnType("integer");

                    b.Property<DateTimeOffset>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Slots");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Common.Domain.Tag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("character varying(32)");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("character varying(64)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.AnswerOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AttachmentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsProper")
                        .HasColumnType("boolean");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AttachmentId")
                        .IsUnique();

                    b.HasIndex("AuthorId");

                    b.HasIndex("QuestionId");

                    b.ToTable("AnswerOptions");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.Question", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AttachmentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Difficulty")
                        .HasColumnType("integer");

                    b.Property<bool>("ShuffleAnswers")
                        .HasColumnType("boolean");

                    b.Property<string>("Text")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AttachmentId")
                        .IsUnique();

                    b.HasIndex("AuthorId");

                    b.ToTable("Questions");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.QuestionAttempt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AnswerId")
                        .IsRequired()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AttachmentId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset?>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset?>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("TestAttemptId")
                        .HasColumnType("uuid");

                    b.Property<string>("TextAnswer")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AnswerId");

                    b.HasIndex("AttachmentId")
                        .IsUnique();

                    b.HasIndex("AuthorId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("TestAttemptId");

                    b.ToTable("QuestionAttempts");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.Test", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<int>("MaxAttempts")
                        .HasColumnType("integer");

                    b.Property<int>("MaxTimeInSeconds")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("Name");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.TestAttempt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsCancelled")
                        .HasColumnType("boolean");

                    b.Property<DateTimeOffset>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTimeOffset>("StoppedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("TestId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TestId");

                    b.ToTable("TestAttempts");
                });

            modelBuilder.Entity("HomeSchool.Core.Reporting.Violations.Domain.Report", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("uuid");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsResolved")
                        .HasColumnType("boolean");

                    b.Property<bool>("ResolveEmailSent")
                        .HasColumnType("boolean");

                    b.Property<string>("ResolveMessage")
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<DateTimeOffset?>("ResolvedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ResolvedBy")
                        .HasColumnType("uuid");

                    b.Property<int>("Severity")
                        .HasColumnType("integer");

                    b.Property<Guid?>("TargetUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("character varying(4000)");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("Id");

                    b.HasIndex("TargetUserId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("Liminal.Auth.Models.Account", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Liminal.Auth.Models.AccountClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("AccountClaims");
                });

            modelBuilder.Entity("Liminal.Auth.Models.AccountToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("uuid");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TokenName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("TokenValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("ValidUntil")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex(new[] { "Provider", "TokenName", "TokenValue" }, "IX_AccountTokenProviderNameValue");

                    b.ToTable("Passwords");
                });

            modelBuilder.Entity("Liminal.Auth.Models.UserClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Liminal.Auth.Models.UserToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTimeOffset>("AccessTokenValidBefore")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("uuid");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("text");

                    b.Property<DateTimeOffset?>("RefreshTokenValidBefore")
                        .HasColumnType("timestamp with time zone");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("QuestionsTags", b =>
                {
                    b.Property<Guid>("QuestionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TagsId")
                        .HasColumnType("uuid");

                    b.HasKey("QuestionsId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("QuestionsTags");
                });

            modelBuilder.Entity("SlotsTags", b =>
                {
                    b.Property<Guid>("SlotsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TagsId")
                        .HasColumnType("uuid");

                    b.HasKey("SlotsId", "TagsId");

                    b.HasIndex("TagsId");

                    b.ToTable("SlotsTags");
                });

            modelBuilder.Entity("TestsQuestions", b =>
                {
                    b.Property<Guid>("QuestionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TestsId")
                        .HasColumnType("uuid");

                    b.HasKey("QuestionsId", "TestsId");

                    b.HasIndex("TestsId");

                    b.ToTable("TestsQuestions");
                });

            modelBuilder.Entity("TestsQuestionsTable", b =>
                {
                    b.Property<Guid>("QuestionsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TestsId")
                        .HasColumnType("uuid");

                    b.HasKey("QuestionsId", "TestsId");

                    b.HasIndex("TestsId");

                    b.ToTable("TestsQuestionsTable");
                });

            modelBuilder.Entity("TestsTags", b =>
                {
                    b.Property<Guid>("TagsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TestsId")
                        .HasColumnType("uuid");

                    b.HasKey("TagsId", "TestsId");

                    b.HasIndex("TestsId");

                    b.ToTable("TestsTags");
                });

            modelBuilder.Entity("TimeSpansTags", b =>
                {
                    b.Property<Guid>("TagsId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("TimeSpansId")
                        .HasColumnType("uuid");

                    b.HasKey("TagsId", "TimeSpansId");

                    b.HasIndex("TimeSpansId");

                    b.ToTable("TimeSpansTags");
                });

            modelBuilder.Entity("HomeSchool.Core.Attachments.Domain.ApplicationAttachment", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", null)
                        .WithMany("Attachments")
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Calendars.Domain.DayTimeSpan", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("TimeSpans")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Calendars.Domain.Slot", "Slot")
                        .WithMany("TimeSpans")
                        .HasForeignKey("SlotId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Slot");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Calendars.Domain.Slot", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("Slots")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Common.Domain.Tag", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("Tags")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.AnswerOption", b =>
                {
                    b.HasOne("HomeSchool.Core.Attachments.Domain.ApplicationAttachment", "Attachment")
                        .WithOne("AnswerOption")
                        .HasForeignKey("HomeSchool.Core.Lessons.Tests.Domain.AnswerOption", "AttachmentId");

                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("QuestionAnswers")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Question", "Question")
                        .WithMany("Answers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attachment");

                    b.Navigation("Author");

                    b.Navigation("Question");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.Question", b =>
                {
                    b.HasOne("HomeSchool.Core.Attachments.Domain.ApplicationAttachment", "Attachment")
                        .WithOne("Question")
                        .HasForeignKey("HomeSchool.Core.Lessons.Tests.Domain.Question", "AttachmentId");

                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("Questions")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attachment");

                    b.Navigation("Author");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.QuestionAttempt", b =>
                {
                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.AnswerOption", "Answer")
                        .WithMany("Attempts")
                        .HasForeignKey("AnswerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Attachments.Domain.ApplicationAttachment", "Attachment")
                        .WithOne("QuestionAttempt")
                        .HasForeignKey("HomeSchool.Core.Lessons.Tests.Domain.QuestionAttempt", "AttachmentId");

                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("QuestionAttempts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Question", "Question")
                        .WithMany("Attempts")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.TestAttempt", "TestAttempt")
                        .WithMany("QuestionAttempts")
                        .HasForeignKey("TestAttemptId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Answer");

                    b.Navigation("Attachment");

                    b.Navigation("Author");

                    b.Navigation("Question");

                    b.Navigation("TestAttempt");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.Test", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("Tests")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.TestAttempt", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("TestAttempts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Test", "Test")
                        .WithMany("TestAttempts")
                        .HasForeignKey("TestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Test");
                });

            modelBuilder.Entity("HomeSchool.Core.Reporting.Violations.Domain.Report", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "Author")
                        .WithMany("Reports")
                        .HasForeignKey("AuthorId");

                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", "TargetUser")
                        .WithMany("Complaints")
                        .HasForeignKey("TargetUserId");

                    b.Navigation("Author");

                    b.Navigation("TargetUser");
                });

            modelBuilder.Entity("Liminal.Auth.Models.Account", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", null)
                        .WithMany("Accounts")
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("Liminal.Auth.Models.AccountToken", b =>
                {
                    b.HasOne("Liminal.Auth.Models.Account", "Account")
                        .WithMany("Passwords")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");
                });

            modelBuilder.Entity("Liminal.Auth.Models.UserClaim", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", null)
                        .WithMany("Claims")
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("Liminal.Auth.Models.UserToken", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", null)
                        .WithMany("Tokens")
                        .HasForeignKey("ApplicationUserId");
                });

            modelBuilder.Entity("QuestionsTags", b =>
                {
                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Question", null)
                        .WithMany()
                        .HasForeignKey("QuestionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Common.Domain.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SlotsTags", b =>
                {
                    b.HasOne("HomeSchool.Core.Lessons.Calendars.Domain.Slot", null)
                        .WithMany()
                        .HasForeignKey("SlotsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Common.Domain.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TestsQuestions", b =>
                {
                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Question", null)
                        .WithMany()
                        .HasForeignKey("QuestionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Test", null)
                        .WithMany()
                        .HasForeignKey("TestsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TestsQuestionsTable", b =>
                {
                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Question", null)
                        .WithMany()
                        .HasForeignKey("QuestionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Test", null)
                        .WithMany()
                        .HasForeignKey("TestsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TestsTags", b =>
                {
                    b.HasOne("HomeSchool.Core.Lessons.Common.Domain.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Tests.Domain.Test", null)
                        .WithMany()
                        .HasForeignKey("TestsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("TimeSpansTags", b =>
                {
                    b.HasOne("HomeSchool.Core.Lessons.Common.Domain.Tag", null)
                        .WithMany()
                        .HasForeignKey("TagsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("HomeSchool.Core.Lessons.Calendars.Domain.DayTimeSpan", null)
                        .WithMany()
                        .HasForeignKey("TimeSpansId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("HomeSchool.Core.Attachments.Domain.ApplicationAttachment", b =>
                {
                    b.Navigation("AnswerOption");

                    b.Navigation("Question");

                    b.Navigation("QuestionAttempt");
                });

            modelBuilder.Entity("HomeSchool.Core.Identity.ApplicationUser", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("Attachments");

                    b.Navigation("Claims");

                    b.Navigation("Complaints");

                    b.Navigation("QuestionAnswers");

                    b.Navigation("QuestionAttempts");

                    b.Navigation("Questions");

                    b.Navigation("Reports");

                    b.Navigation("Slots");

                    b.Navigation("Tags");

                    b.Navigation("TestAttempts");

                    b.Navigation("Tests");

                    b.Navigation("TimeSpans");

                    b.Navigation("Tokens");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Calendars.Domain.Slot", b =>
                {
                    b.Navigation("TimeSpans");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.AnswerOption", b =>
                {
                    b.Navigation("Attempts");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.Question", b =>
                {
                    b.Navigation("Answers");

                    b.Navigation("Attempts");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.Test", b =>
                {
                    b.Navigation("TestAttempts");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.TestAttempt", b =>
                {
                    b.Navigation("QuestionAttempts");
                });

            modelBuilder.Entity("Liminal.Auth.Models.Account", b =>
                {
                    b.Navigation("Passwords");
                });
#pragma warning restore 612, 618
        }
    }
}