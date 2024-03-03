﻿// <auto-generated />
using System;
using CalendarApp.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CalendarApp.Api.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240301163635_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("CalendarApp.Api.Entities.ScheduledClass", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<uint>("SubjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.ToTable("ScheduledClasses");
                });

            modelBuilder.Entity("CalendarApp.Api.Entities.Subject", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint>("FacultyType")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("CalendarApp.Api.Entities.User", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Faculty1Id")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Faculty2Id")
                        .HasColumnType("INTEGER");

                    b.Property<uint>("Faculty3Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("Faculty1Id");

                    b.HasIndex("Faculty2Id");

                    b.HasIndex("Faculty3Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("CalendarApp.Api.Entities.ScheduledClass", b =>
                {
                    b.HasOne("CalendarApp.Api.Entities.Subject", "Subject")
                        .WithMany("ScheduledClasses")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("CalendarApp.Api.Entities.User", b =>
                {
                    b.HasOne("CalendarApp.Api.Entities.Subject", "Faculty1")
                        .WithMany()
                        .HasForeignKey("Faculty1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CalendarApp.Api.Entities.Subject", "Faculty2")
                        .WithMany()
                        .HasForeignKey("Faculty2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CalendarApp.Api.Entities.Subject", "Faculty3")
                        .WithMany()
                        .HasForeignKey("Faculty3Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Faculty1");

                    b.Navigation("Faculty2");

                    b.Navigation("Faculty3");
                });

            modelBuilder.Entity("CalendarApp.Api.Entities.Subject", b =>
                {
                    b.Navigation("ScheduledClasses");
                });
#pragma warning restore 612, 618
        }
    }
}
