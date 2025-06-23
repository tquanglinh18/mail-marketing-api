using System;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;

namespace mail_marketing_api.Data
{
    public class AppDbContext : DbContext
    {
        // <= ĐÃ THAY ĐỔI TÊN CONSTRUCTOR
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmailLogs> EmailLogs { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<EmailTemplate> Templates { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailLogs>()
                  .HasOne(e => e.EmailRecipient)
                  .WithMany()
                  .HasForeignKey(e => e.RecipientId)
                  .OnDelete(DeleteBehavior.Cascade); // Cho phép xoá người nhận thì xoá log

            modelBuilder.Entity<EmailLogs>()
                .HasOne(e => e.EmailTemplate)
                .WithMany()
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Restrict); // KHÔNG cho xoá template nếu có log

            // --- DỮ LIỆU MỒI CHI TIẾT ---

            // 1. Mẫu Email (Templates)
            modelBuilder.Entity<EmailTemplate>().HasData(
                new EmailTemplate
                {
                    TemplateId = 1,
                    TemplateName = "Chào mừng thành viên mới",
                    HtmlContent = "<h1>Xin chào -TenNguoiNhan-!</h1><p>Chúng tôi rất vui khi có bạn đồng hành.</p>",
                    ImageStorageType = "None",
                    CreatedDate = new DateTime(2025, 6, 1, 8, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "Admin"
                },
                new EmailTemplate
                {
                    TemplateId = 2,
                    TemplateName = "Thông báo khuyến mãi đặc biệt",
                    HtmlContent = "<p>Thân gửi -TenNguoiNhan-,</p><p>Mời bạn tham gia chương trình -CampaignName- và nhận ưu đãi tại -Email-.</p>",
                    ImageStorageType = "None",
                    CreatedDate = new DateTime(2025, 6, 2, 10, 30, 0, DateTimeKind.Utc),
                    CreatedBy = "MarketingTeam"
                }
            );

            // 2. Chiến dịch (Campaigns)
            modelBuilder.Entity<Campaign>().HasData(
                new Campaign
                {
                    CampaignId = 1,
                    CampaignName = "Chiến dịch Chào Hè 2025",
                    TemplateId = 1,
                    UploadedFileName = "he2025.xlsx",
                    UploadedFileUrl = "/uploads/he2025.xlsx",
                    CreateDate = new DateTime(2025, 6, 5, 9, 0, 0, DateTimeKind.Utc),
                    CreateBy = "linhtq",
                    StartDate = new DateTime(2025, 6, 10),
                    EndDate = new DateTime(2025, 6, 20)
                },
                new Campaign
                {
                    CampaignId = 2,
                    CampaignName = "Khuyến mãi VIP tháng 6",
                    TemplateId = 2,
                    UploadedFileName = "vip_june.csv",
                    UploadedFileUrl = "/uploads/vip_june.csv",
                    CreateDate = new DateTime(2025, 6, 8, 14, 0, 0, DateTimeKind.Utc),
                    CreateBy = "marketing_user",
                    StartDate = new DateTime(2025, 6, 12),
                    EndDate = new DateTime(2025, 6, 25)
                }
            );

            // 3. Người nhận (Recipients)
            modelBuilder.Entity<Recipient>().HasData(
                new Recipient
                {
                    RecipientId = 1,
                    CampaignId = 1,
                    RecipientName = "Lê Văn A",
                    RecipientEmail = "levana@example.com",
                    CustomDataJson = "{ \"TenNguoiNhan\": \"Văn A\", \"ThanhPho\": \"Hà Nội\" }"
                },
                new Recipient
                {
                    RecipientId = 2,
                    CampaignId = 1,
                    RecipientName = "Nguyễn Thị B",
                    RecipientEmail = "nguyenthib@example.com",
                    CustomDataJson = "{ \"TenNguoiNhan\": \"Thị B\", \"ThanhPho\": \"Đà Nẵng\" }"
                },
                new Recipient
                {
                    RecipientId = 3,
                    CampaignId = 2,
                    RecipientName = "Phạm Văn C",
                    RecipientEmail = "phamvanc@example.com",
                    CustomDataJson = "{ \"TenNguoiNhan\": \"Văn C\", \"MembershipLevel\": \"Gold\", \"Email\": \"phamvanc@example.com\" }"
                }
            );

            // 4. Lịch sử gửi (EmailLogs)
            modelBuilder.Entity<EmailLogs>().HasData(
                new EmailLogs
                {
                    LogId = 1,
                    RecipientId = 1,
                    TemplateId = 1,
                    SentDate = new DateTime(2025, 6, 10, 9, 0, 0, DateTimeKind.Utc),
                    IsSuccess = true,
                    ErrorMessage = ""
                },
                new EmailLogs
                {
                    LogId = 2,
                    RecipientId = 3,
                    TemplateId = 2,
                    SentDate = new DateTime(2025, 6, 13, 8, 30, 0, DateTimeKind.Utc),
                    IsSuccess = false,
                    ErrorMessage = "SMTP connection timeout."
                }
            );
            // --- KẾT THÚC DỮ LIỆU MỒI ---
        }
    }
}

