﻿// <auto-generated />
using System;
using EasyFlow.Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace EasyFlow.Infrastructure.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-rc.1.24451.1");

            modelBuilder.Entity("EasyFlow.Domain.Entities.GeneralSettings", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("BreakDurationMinutes")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsBreakSoundEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsFocusDescriptionEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsWorkSoundEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<int>("LongBreakDurationMinutes")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SelectedColorTheme")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SelectedLanguage")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("SelectedTagId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SelectedTheme")
                        .HasColumnType("INTEGER");

                    b.Property<int>("SoundVolume")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WorkDurationMinutes")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WorkSessionsBeforeLongBreak")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("SelectedTagId");

                    b.ToTable("GeneralSettings");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            BreakDurationMinutes = 5,
                            IsBreakSoundEnabled = true,
                            IsFocusDescriptionEnabled = true,
                            IsWorkSoundEnabled = true,
                            LongBreakDurationMinutes = 10,
                            SelectedColorTheme = 1,
                            SelectedLanguage = "en",
                            SelectedTagId = 1,
                            SelectedTheme = 0,
                            SoundVolume = 50,
                            WorkDurationMinutes = 25,
                            WorkSessionsBeforeLongBreak = 4
                        });
                });

            modelBuilder.Entity("EasyFlow.Domain.Entities.Session", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("DurationMinutes")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("FinishedDate")
                        .HasColumnType("TEXT");

                    b.Property<int>("SessionType")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("TagId");

                    b.ToTable("Sessions");
                });

            modelBuilder.Entity("EasyFlow.Domain.Entities.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tags");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Work"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Study"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Meditate"
                        },
                        new
                        {
                            Id = 4,
                            Name = "Exercises"
                        });
                });

            modelBuilder.Entity("EasyFlow.Domain.Entities.GeneralSettings", b =>
                {
                    b.HasOne("EasyFlow.Domain.Entities.Tag", "SelectedTag")
                        .WithMany()
                        .HasForeignKey("SelectedTagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SelectedTag");
                });

            modelBuilder.Entity("EasyFlow.Domain.Entities.Session", b =>
                {
                    b.HasOne("EasyFlow.Domain.Entities.Tag", "Tag")
                        .WithMany("Sessions")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("EasyFlow.Domain.Entities.Tag", b =>
                {
                    b.Navigation("Sessions");
                });
#pragma warning restore 612, 618
        }
    }
}
