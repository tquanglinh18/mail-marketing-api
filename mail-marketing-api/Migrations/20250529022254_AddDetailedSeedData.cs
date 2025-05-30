using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mail_marketing_api.Migrations
{
    public partial class AddDetailedSeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    TemplateId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    HtmlContent = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageStorageType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "UploadBatches",
                columns: table => new
                {
                    BatchId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UploadedFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UploadedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UploadBatches", x => x.BatchId);
                });

            migrationBuilder.CreateTable(
                name: "EmailRecipients",
                columns: table => new
                {
                    RecipientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BatchId = table.Column<int>(type: "int", nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CustomDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailRecipients", x => x.RecipientId);
                    table.ForeignKey(
                        name: "FK_EmailRecipients_UploadBatches_BatchId",
                        column: x => x.BatchId,
                        principalTable: "UploadBatches",
                        principalColumn: "BatchId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EmailLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecipientId = table.Column<int>(type: "int", nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    SentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailLogs", x => x.LogId);
                    table.ForeignKey(
                        name: "FK_EmailLogs_EmailRecipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "EmailRecipients",
                        principalColumn: "RecipientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailLogs_EmailTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "EmailTemplates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "TemplateId", "CreatedBy", "CreatedDate", "HtmlContent", "ImageStorageType", "TemplateName" },
                values: new object[,]
                {
                    { 1, "Admin", new DateTime(2025, 5, 20, 10, 0, 0, 0, DateTimeKind.Utc), "<h1>Chào mừng -TenNguoiNhan-!</h1><p>Cảm ơn bạn đã đăng ký dịch vụ của chúng tôi.</p>", "None", "Mẫu Chào Mừng Thành Viên Mới" },
                    { 2, "MarketingTeam", new DateTime(2025, 5, 21, 11, 0, 0, 0, DateTimeKind.Utc), "<h1>Kính gửi -TenNguoiNhan-,</h1><p>Cảm ơn bạn đã là một thành viên <b>-MembershipLevel-</b> tại thành phố <b>-ThanhPho-</b>!</p><p>Chúng tôi vui mừng thông báo chương trình khuyến mãi <b>-CampaignName-</b>.Hãy sử dụng mã <b>-DiscountCode-</b> để nhận ưu đãi đặc biệt.</p><p>Ưu đãi này chỉ dành riêng cho bạn tại địa chỉ email: -Email-.</p><p>Trân trọng,<br>\nĐội ngũ Marketing</p></p>", "None", "Mẫu Tin Tức Tháng 5" }
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
                    { 1, 1, "{ \"TenNguoiNhan\": \"Văn Linh\", \"city\": \"Hanoi\" }", "linhtq.vtco@gmail.com", "Nguyễn Văn Linh" },
                    { 2, 1, "{ \"TenNguoiNhan\": \"Thị Linh\", \"city\": \"HCM\" }", "tquanglinh18@gmail.com", "Trần Thị Linh" },
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

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_RecipientId",
                table: "EmailLogs",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_TemplateId",
                table: "EmailLogs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailRecipients_BatchId",
                table: "EmailRecipients",
                column: "BatchId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "EmailRecipients");

            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.DropTable(
                name: "UploadBatches");
        }
    }
}
