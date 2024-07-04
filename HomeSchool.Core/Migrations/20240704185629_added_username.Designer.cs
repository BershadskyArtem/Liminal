﻿// <auto-generated />
using System;
using HomeSchool.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HomeSchool.Core.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240704185629_added_username")]
    partial class added_username
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

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

            modelBuilder.Entity("HomeSchool.Core.Identity.ApplicationUser", b =>
                {
                    b.Navigation("Accounts");

                    b.Navigation("Claims");

                    b.Navigation("Complaints");

                    b.Navigation("Reports");

                    b.Navigation("Tokens");
                });

            modelBuilder.Entity("Liminal.Auth.Models.Account", b =>
                {
                    b.Navigation("Passwords");
                });
#pragma warning restore 612, 618
        }
    }
}
