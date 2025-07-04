﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using mail_marketing_api.Data;

#nullable disable

namespace mail_marketing_api.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250630043204_AddDetailedSeedData")]
    partial class AddDetailedSeedData
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.36")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("mail_marketing_api.Models.Campaign", b =>
                {
                    b.Property<int>("CampaignId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("CampaignId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CampaignId"), 1L, 1);

                    b.Property<string>("CampaignName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("CampaignName");

                    b.Property<string>("CreateBy")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("CreateBy");

                    b.Property<DateTime>("CreateDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreateDate");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("EndDate");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("StartDate");

                    b.Property<int>("TemplateId")
                        .HasColumnType("int")
                        .HasColumnName("TemplateId");

                    b.Property<string>("UploadedFileName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("UploadedFileName");

                    b.Property<string>("UploadedFileUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("UploadedFileUrl");

                    b.HasKey("CampaignId");

                    b.HasIndex("TemplateId");

                    b.ToTable("Campaigns");
                });

            modelBuilder.Entity("mail_marketing_api.Models.EmailLogs", b =>
                {
                    b.Property<int>("LogId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("LogId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LogId"), 1L, 1);

                    b.Property<string>("ErrorMessage")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ErrorMessage");

                    b.Property<bool>("IsSuccess")
                        .HasColumnType("bit")
                        .HasColumnName("IsSuccess");

                    b.Property<int>("RecipientId")
                        .HasColumnType("int")
                        .HasColumnName("RecipientId");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("SentDate");

                    b.Property<int>("TemplateId")
                        .HasColumnType("int")
                        .HasColumnName("TemplateId");

                    b.HasKey("LogId");

                    b.HasIndex("RecipientId");

                    b.HasIndex("TemplateId");

                    b.ToTable("EmailLogs");
                });

            modelBuilder.Entity("mail_marketing_api.Models.EmailTemplate", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("TemplateId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("TemplateId"), 1L, 1);

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("CreatedBy");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("CreatedDate");

                    b.Property<string>("HtmlContent")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("HtmlContent");

                    b.Property<string>("ImageStorageType")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("ImageStorageType");

                    b.Property<string>("TemplateName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("TemplateName");

                    b.HasKey("TemplateId");

                    b.ToTable("Templates");
                });

            modelBuilder.Entity("mail_marketing_api.Models.Recipient", b =>
                {
                    b.Property<int>("RecipientId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("RecipientId");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("RecipientId"), 1L, 1);

                    b.Property<int>("CampaignId")
                        .HasColumnType("int")
                        .HasColumnName("CampaignId");

                    b.Property<string>("CustomDataJson")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CustomDataJson");

                    b.Property<string>("RecipientEmail")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("RecipientEmail");

                    b.Property<string>("RecipientName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("RecipientName");

                    b.HasKey("RecipientId");

                    b.HasIndex("CampaignId");

                    b.ToTable("Recipients");
                });

            modelBuilder.Entity("mail_marketing_api.Models.Campaign", b =>
                {
                    b.HasOne("mail_marketing_api.Models.EmailTemplate", "Template")
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Template");
                });

            modelBuilder.Entity("mail_marketing_api.Models.EmailLogs", b =>
                {
                    b.HasOne("mail_marketing_api.Models.Recipient", "EmailRecipient")
                        .WithMany()
                        .HasForeignKey("RecipientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("mail_marketing_api.Models.EmailTemplate", "EmailTemplate")
                        .WithMany()
                        .HasForeignKey("TemplateId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("EmailRecipient");

                    b.Navigation("EmailTemplate");
                });

            modelBuilder.Entity("mail_marketing_api.Models.Recipient", b =>
                {
                    b.HasOne("mail_marketing_api.Models.Campaign", "Campaign")
                        .WithMany("Recipients")
                        .HasForeignKey("CampaignId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Campaign");
                });

            modelBuilder.Entity("mail_marketing_api.Models.Campaign", b =>
                {
                    b.Navigation("Recipients");
                });
#pragma warning restore 612, 618
        }
    }
}
