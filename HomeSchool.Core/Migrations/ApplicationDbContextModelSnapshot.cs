﻿// <auto-generated />
using System;
using HomeSchool.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HomeSchool.Core.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("HomeSchool.Core.Attachments.Domain.ApplicationAttachment", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AnswerOptionId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("BecameNotTransientAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("DiskName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Extension")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsTransient")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MimeType")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("QuestionAttemptId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("QuestionId")
                        .HasColumnType("TEXT");

                    b.Property<long>("Size")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("ApplicationAttachment");
                });

            modelBuilder.Entity("HomeSchool.Core.Identity.ApplicationUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.AnswerOption", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AttachmentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsProper")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("TEXT");

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
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AttachmentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("Difficulty")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ShuffleAnswers")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Text")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("TEXT");

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
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AnswerId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AttachmentId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("FinishedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("QuestionId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("StartedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TestAttemptId")
                        .HasColumnType("TEXT");

                    b.Property<string>("TextAnswer")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("TEXT");

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
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<int>("MaxAttempts")
                        .HasColumnType("INTEGER");

                    b.Property<int>("MaxTimeInSeconds")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("Name");

                    b.ToTable("Tests");
                });

            modelBuilder.Entity("HomeSchool.Core.Lessons.Tests.Domain.TestAttempt", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCancelled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset>("StartedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("StoppedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TestId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("TestId");

                    b.ToTable("TestAttempts");
                });

            modelBuilder.Entity("HomeSchool.Core.Reporting.Violations.Domain.Report", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AuthorId")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsResolved")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ResolveEmailSent")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ResolveMessage")
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ResolvedAt")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ResolvedBy")
                        .HasColumnType("TEXT");

                    b.Property<int>("Severity")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("TargetUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasMaxLength(4000)
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UpdatedAt")
                        .HasColumnType("TEXT");

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
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("Liminal.Auth.Models.AccountClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("AccountClaims");
                });

            modelBuilder.Entity("Liminal.Auth.Models.AccountToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("AccountId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Provider")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("TokenName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("TEXT");

                    b.Property<string>("TokenValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("ValidUntil")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.HasIndex(new[] { "Provider", "TokenName", "TokenValue" }, "IX_AccountTokenProviderNameValue");

                    b.ToTable("Passwords");
                });

            modelBuilder.Entity("Liminal.Auth.Models.UserClaim", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("ClaimValue")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("UserClaims");
                });

            modelBuilder.Entity("Liminal.Auth.Models.UserToken", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("AccessToken")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("AccessTokenValidBefore")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ApplicationUserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("RefreshToken")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset?>("RefreshTokenValidBefore")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationUserId");

                    b.ToTable("UserTokens");
                });

            modelBuilder.Entity("TestsQuestionsTable", b =>
                {
                    b.Property<Guid>("QuestionsId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TestsId")
                        .HasColumnType("TEXT");

                    b.HasKey("QuestionsId", "TestsId");

                    b.HasIndex("TestsId");

                    b.ToTable("TestsQuestionsTable");
                });

            modelBuilder.Entity("HomeSchool.Core.Attachments.Domain.ApplicationAttachment", b =>
                {
                    b.HasOne("HomeSchool.Core.Identity.ApplicationUser", null)
                        .WithMany("Attachments")
                        .HasForeignKey("ApplicationUserId");
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

                    b.Navigation("TestAttempts");

                    b.Navigation("Tests");

                    b.Navigation("Tokens");
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
