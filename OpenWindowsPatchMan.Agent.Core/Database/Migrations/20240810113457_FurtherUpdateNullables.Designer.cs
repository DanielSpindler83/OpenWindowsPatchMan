﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using OpenWindowsPatchMan.Agent.Core.Database;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;


#nullable disable

namespace OpenWindowsPatchMan.Agent.Service.Migrations
{
    [DbContext(typeof(PatchManDbContext))]
    [Migration("20240810113457_FurtherUpdateNullables")]
    partial class FurtherUpdateNullables
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.7");

            modelBuilder.Entity("OpenWindowsPatchMan.Agent.Service.WindowsUpdateInfo", b =>
                {
                    b.Property<int>("WindowsUpdateInfoEntryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BundledUpdates")
                        .HasColumnType("TEXT");

                    b.Property<string>("Categories")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DatePublished")
                        .HasColumnType("TEXT");

                    b.Property<string>("DeploymentAction")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("DownloadSizeMB")
                        .HasColumnType("REAL");

                    b.Property<string>("InstallationRebootBehavior")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsBeta")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDownloaded")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsHidden")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsInstalled")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsMandatory")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsUninstallable")
                        .HasColumnType("INTEGER");

                    b.Property<string>("KBArticleIDs")
                        .HasColumnType("TEXT");

                    b.Property<string>("MoreInfoUrls")
                        .HasColumnType("TEXT");

                    b.Property<string>("ReleaseNotes")
                        .HasColumnType("TEXT");

                    b.Property<string>("SecurityBulletinIDs")
                        .HasColumnType("TEXT");

                    b.Property<string>("SupersededUpdateIDs")
                        .HasColumnType("TEXT");

                    b.Property<string>("SupportUrl")
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("UninstallationNotes")
                        .HasColumnType("TEXT");

                    b.Property<string>("UninstallationRebootBehavior")
                        .HasColumnType("TEXT");

                    b.Property<string>("UninstallationSteps")
                        .HasColumnType("TEXT");

                    b.Property<DateTimeOffset>("UpdateCheckTime")
                        .HasColumnType("TEXT");

                    b.HasKey("WindowsUpdateInfoEntryId");

                    b.ToTable("WindowsUpdateInfo", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
