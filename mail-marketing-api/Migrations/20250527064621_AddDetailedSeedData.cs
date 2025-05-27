using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mail_marketing_api.Migrations
{
    public partial class AddDetailedSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "TemplateId", "CreatedBy", "CreatedDate", "HtmlContent", "ImageStorageType", "TemplateName" },
                values: new object[,]
                {
                    { 1, "Admin", new DateTime(2025, 5, 20, 10, 0, 0, 0, DateTimeKind.Utc), "<h1>Chào mừng [TenNguoiNhan]!</h1><p>Cảm ơn bạn đã đăng ký dịch vụ của chúng tôi.</p>", "None", "Mẫu Chào Mừng Thành Viên Mới" },
                    { 2, "MarketingTeam", new DateTime(2025, 5, 21, 11, 0, 0, 0, DateTimeKind.Utc), "<h1>Bản tin Tháng 5</h1><p>Các cập nhật mới nhất...</p>", "None", "Mẫu Tin Tức Tháng 5" }
                });

            migrationBuilder.InsertData(
                table: "UploadBatches",
                columns: new[] { "BatchId", "BatchName", "UploadDate", "UploadedBy", "UploadedFileName" },
                values: new object[,]
                {
                    { 1, "Khách hàng đăng ký T4/2025", new DateTime(2025, 5, 1, 9, 30, 0, 0, DateTimeKind.Utc), "Sales", "customers_apr2025.csv" },
                    { 2, "Người tham dự Workshop Marketing", new DateTime(2025, 5, 15, 14, 0, 0, 0, DateTimeKind.Utc), "Events", "workshop_attendees.xlsx" }
                });

            migrationBuilder.InsertData(
                table: "EmailRecipients",
                columns: new[] { "RecipientId", "BatchId", "CustomDataJson", "RecipientEmail", "RecipientName" },
                values: new object[,]
                {
                    { 1, 1, "{ \"TenNguoiNhan\": \"Văn A\", \"city\": \"Hanoi\" }", "nguyenvana@example.com", "Nguyễn Văn A" },
                    { 2, 1, "{ \"TenNguoiNhan\": \"Thị B\", \"city\": \"HCM\" }", "tranthib@example.com", "Trần Thị B" },
                    { 3, 2, "{ \"TenNguoiNhan\": \"Văn C\", \"company\": \"ABC Corp\" }", "levanc@example.com", "Lê Văn C" },
                    { 4, 2, "{ \"TenNguoiNhan\": \"Thị D\", \"company\": \"XYZ Ltd\" }", "phamthid@example.com", "Phạm Thị D" }
                });

            migrationBuilder.InsertData(
                table: "EmailLogs",
                columns: new[] { "LogId", "ErrorMessage", "IsSuccess", "RecipientId", "SentDate", "TemplateId" },
                values: new object[] { 1, "", true, 1, new DateTime(2025, 5, 22, 9, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "EmailLogs",
                columns: new[] { "LogId", "ErrorMessage", "IsSuccess", "RecipientId", "SentDate", "TemplateId" },
                values: new object[] { 2, "", true, 3, new DateTime(2025, 5, 23, 10, 0, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.InsertData(
                table: "EmailLogs",
                columns: new[] { "LogId", "ErrorMessage", "IsSuccess", "RecipientId", "SentDate", "TemplateId" },
                values: new object[] { 3, "Email address does not exist.", false, 4, new DateTime(2025, 5, 23, 10, 5, 0, 0, DateTimeKind.Utc), 2 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "EmailLogs",
                keyColumn: "LogId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmailLogs",
                keyColumn: "LogId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmailLogs",
                keyColumn: "LogId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EmailRecipients",
                keyColumn: "RecipientId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "EmailRecipients",
                keyColumn: "RecipientId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmailRecipients",
                keyColumn: "RecipientId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "EmailRecipients",
                keyColumn: "RecipientId",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "TemplateId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "EmailTemplates",
                keyColumn: "TemplateId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "UploadBatches",
                keyColumn: "BatchId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "UploadBatches",
                keyColumn: "BatchId",
                keyValue: 2);
        }
    }
}
