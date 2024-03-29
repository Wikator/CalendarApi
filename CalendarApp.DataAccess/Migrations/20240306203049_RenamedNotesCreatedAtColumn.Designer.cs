﻿// <auto-generated />
using System;
using CalendarApp.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace CalendarApp.DataAccess.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240306203049_RenamedNotesCreatedAtColumn")]
    partial class RenamedNotesCreatedAtColumn
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("CalendarApp.Models.Entities.Note", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("LastModified")
                        .HasColumnType("TEXT");

                    b.Property<uint>("ScheduledClassId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<uint>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ScheduledClassId");

                    b.HasIndex("UserId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("CalendarApp.Models.Entities.ScheduledClass", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("EndTime")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsCancelled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("StartTime")
                        .HasColumnType("TEXT");

                    b.Property<uint>("SubjectId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId");

                    b.ToTable("ScheduledClasses");
                });

            modelBuilder.Entity("CalendarApp.Models.Entities.Subject", b =>
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

            modelBuilder.Entity("CalendarApp.Models.Entities.User", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint?>("Faculty1Id")
                        .HasColumnType("INTEGER");

                    b.Property<uint?>("Faculty2Id")
                        .HasColumnType("INTEGER");

                    b.Property<uint?>("Faculty3Id")
                        .HasColumnType("INTEGER");

                    b.Property<byte[]>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("PasswordSalt")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("Role")
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

            modelBuilder.Entity("CalendarApp.Models.Entities.Note", b =>
                {
                    b.HasOne("CalendarApp.Models.Entities.ScheduledClass", "ScheduledClass")
                        .WithMany("Notes")
                        .HasForeignKey("ScheduledClassId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("CalendarApp.Models.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ScheduledClass");

                    b.Navigation("User");
                });

            modelBuilder.Entity("CalendarApp.Models.Entities.ScheduledClass", b =>
                {
                    b.HasOne("CalendarApp.Models.Entities.Subject", "Subject")
                        .WithMany("ScheduledClasses")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("CalendarApp.Models.Entities.User", b =>
                {
                    b.HasOne("CalendarApp.Models.Entities.Subject", "Faculty1")
                        .WithMany()
                        .HasForeignKey("Faculty1Id");

                    b.HasOne("CalendarApp.Models.Entities.Subject", "Faculty2")
                        .WithMany()
                        .HasForeignKey("Faculty2Id");

                    b.HasOne("CalendarApp.Models.Entities.Subject", "Faculty3")
                        .WithMany()
                        .HasForeignKey("Faculty3Id");

                    b.Navigation("Faculty1");

                    b.Navigation("Faculty2");

                    b.Navigation("Faculty3");
                });

            modelBuilder.Entity("CalendarApp.Models.Entities.ScheduledClass", b =>
                {
                    b.Navigation("Notes");
                });

            modelBuilder.Entity("CalendarApp.Models.Entities.Subject", b =>
                {
                    b.Navigation("ScheduledClasses");
                });
#pragma warning restore 612, 618
        }
    }
}
