using System;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;

namespace mail_marketing_api.Data
{
	public class AppDbContext : DbContext	{
        // <= ĐÃ THAY ĐỔI TÊN CONSTRUCTOR
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmailLogs> EmailLogs { get; set; }
        public DbSet<EmailRecipient> EmailRecipients { get; set; }
        public DbSet<EmailTemplate> EmailTemplates { get; set; }
        public DbSet<UploadBatch> UploadBatches { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- DỮ LIỆU MỒI CHI TIẾT ---

            // 1. Thêm Email Templates
            modelBuilder.Entity<EmailTemplate>().HasData(
                new EmailTemplate
                {
                    TemplateId = 1,
                    TemplateName = "Mẫu Chào Mừng Thành Viên Mới",
                    HtmlContent = "<h1>Chào mừng [TenNguoiNhan]!</h1><p>Cảm ơn bạn đã đăng ký dịch vụ của chúng tôi.</p>",
                    ImageStorageType = "None",
                    CreatedDate = new DateTime(2025, 5, 20, 10, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "Admin"
                },
                new EmailTemplate
                {
                    TemplateId = 2,
                    TemplateName = "Mẫu Tin Tức Tháng 5",
                    HtmlContent = "<h1>Bản tin Tháng 5</h1><p>Các cập nhật mới nhất...</p>",
                    ImageStorageType = "None",
                    CreatedDate = new DateTime(2025, 5, 21, 11, 0, 0, DateTimeKind.Utc),
                    CreatedBy = "MarketingTeam"
                }
            );

            // 2. Thêm Upload Batches (Các lô Upload)
            modelBuilder.Entity<UploadBatch>().HasData(
                new UploadBatch
                {
                    BatchId = 1, // Lô 1
                    BatchName = "Khách hàng đăng ký T4/2025",
                    UploadedFileName = "customers_apr2025.csv",
                    UploadDate = new DateTime(2025, 5, 1, 9, 30, 0, DateTimeKind.Utc),
                    UploadedBy = "Sales"
                },
                new UploadBatch
                {
                    BatchId = 2, // Lô 2
                    BatchName = "Người tham dự Workshop Marketing",
                    UploadedFileName = "workshop_attendees.xlsx",
                    UploadDate = new DateTime(2025, 5, 15, 14, 0, 0, DateTimeKind.Utc),
                    UploadedBy = "Events"
                }
            );

            // 3. Thêm Email Recipients (Người nhận, liên kết với Batches)
            modelBuilder.Entity<EmailRecipient>().HasData(
                new EmailRecipient // Người nhận 1 (thuộc Lô 1)
                {
                    RecipientId = 1,
                    BatchId = 1, // <-- Liên kết Lô 1
                    RecipientName = "Nguyễn Văn A",
                    RecipientEmail = "nguyenvana@example.com",
                    CustomDataJson = "{ \"TenNguoiNhan\": \"Văn A\", \"city\": \"Hanoi\" }"
                },
                new EmailRecipient // Người nhận 2 (thuộc Lô 1)
                {
                    RecipientId = 2,
                    BatchId = 1, // <-- Liên kết Lô 1
                    RecipientName = "Trần Thị B",
                    RecipientEmail = "tranthib@example.com",
                    CustomDataJson = "{ \"TenNguoiNhan\": \"Thị B\", \"city\": \"HCM\" }"
                },
                new EmailRecipient // Người nhận 3 (thuộc Lô 2)
                {
                    RecipientId = 3,
                    BatchId = 2, // <-- Liên kết Lô 2
                    RecipientName = "Lê Văn C",
                    RecipientEmail = "levanc@example.com",
                    CustomDataJson = "{ \"TenNguoiNhan\": \"Văn C\", \"company\": \"ABC Corp\" }"
                },
                 new EmailRecipient // Người nhận 4 (thuộc Lô 2)
                 {
                     RecipientId = 4,
                     BatchId = 2, // <-- Liên kết Lô 2
                     RecipientName = "Phạm Thị D",
                     RecipientEmail = "phamthid@example.com",
                     CustomDataJson = "{ \"TenNguoiNhan\": \"Thị D\", \"company\": \"XYZ Ltd\" }"
                 }
            );

            // 4. Thêm Email Logs (Lịch sử gửi, liên kết Người nhận và Mẫu)
            modelBuilder.Entity<EmailLogs>().HasData(
                new EmailLogs // Log 1: Gửi thành công cho người 1, mẫu 1
                {
                    LogId = 1,
                    RecipientId = 1, // <-- Liên kết Người nhận 1
                    TemplateId = 1,  // <-- Liên kết Mẫu 1
                    SentDate = new DateTime(2025, 5, 22, 9, 0, 0, DateTimeKind.Utc),
                    IsSuccess = true,
                    ErrorMessage = "" // Rỗng vì thành công
                },
                new EmailLogs // Log 2: Gửi thành công cho người 3, mẫu 2
                {
                    LogId = 2,
                    RecipientId = 3, // <-- Liên kết Người nhận 3
                    TemplateId = 2,  // <-- Liên kết Mẫu 2
                    SentDate = new DateTime(2025, 5, 23, 10, 0, 0, DateTimeKind.Utc),
                    IsSuccess = true,
                    ErrorMessage = ""
                },
                new EmailLogs // Log 3: Gửi thất bại cho người 4, mẫu 2
                {
                    LogId = 3,
                    RecipientId = 4, // <-- Liên kết Người nhận 4
                    TemplateId = 2,  // <-- Liên kết Mẫu 2
                    SentDate = new DateTime(2025, 5, 23, 10, 5, 0, DateTimeKind.Utc),
                    IsSuccess = false,
                    ErrorMessage = "Email address does not exist." // Lý do thất bại
                }
            );

            // --- KẾT THÚC DỮ LIỆU MỒI ---
        }
    }
}

