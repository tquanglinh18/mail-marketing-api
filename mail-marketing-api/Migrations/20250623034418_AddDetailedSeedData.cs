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
                name: "Templates",
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
                    table.PrimaryKey("PK_Templates", x => x.TemplateId);
                });

            migrationBuilder.CreateTable(
                name: "Campaigns",
                columns: table => new
                {
                    CampaignId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TemplateId = table.Column<int>(type: "int", nullable: false),
                    UploadedFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    UploadedFileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Campaigns", x => x.CampaignId);
                    table.ForeignKey(
                        name: "FK_Campaigns_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recipients",
                columns: table => new
                {
                    RecipientId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CampaignId = table.Column<int>(type: "int", nullable: false),
                    RecipientName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    RecipientEmail = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CustomDataJson = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipients", x => x.RecipientId);
                    table.ForeignKey(
                        name: "FK_Recipients_Campaigns_CampaignId",
                        column: x => x.CampaignId,
                        principalTable: "Campaigns",
                        principalColumn: "CampaignId",
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
                        name: "FK_EmailLogs_Recipients_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "Recipients",
                        principalColumn: "RecipientId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EmailLogs_Templates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "Templates",
                        principalColumn: "TemplateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Templates",
                columns: new[] { "TemplateId", "CreatedBy", "CreatedDate", "HtmlContent", "ImageStorageType", "TemplateName" },
                values: new object[] { 1, "Admin", new DateTime(2025, 6, 1, 8, 0, 0, 0, DateTimeKind.Utc), "<h1>Xin chào -TenNguoiNhan-!</h1><p>Chúng tôi rất vui khi có bạn đồng hành.</p>", "None", "Chào mừng thành viên mới" });

            migrationBuilder.InsertData(
                table: "Templates",
                columns: new[] { "TemplateId", "CreatedBy", "CreatedDate", "HtmlContent", "ImageStorageType", "TemplateName" },
                values: new object[] { 2, "MarketingTeam", new DateTime(2025, 6, 2, 10, 30, 0, 0, DateTimeKind.Utc), "<p>Thân gửi -TenNguoiNhan-,</p><p>Mời bạn tham gia chương trình -CampaignName- và nhận ưu đãi tại -Email-.</p>", "None", "Thông báo khuyến mãi đặc biệt" });

            migrationBuilder.InsertData(
                table: "Campaigns",
                columns: new[] { "CampaignId", "CampaignName", "CreateBy", "CreateDate", "EndDate", "StartDate", "TemplateId", "UploadedFileName", "UploadedFileUrl" },
                values: new object[] { 1, "Chiến dịch Chào Hè 2025", "linhtq", new DateTime(2025, 6, 5, 9, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "he2025.xlsx", "/uploads/he2025.xlsx" });

            migrationBuilder.InsertData(
                table: "Campaigns",
                columns: new[] { "CampaignId", "CampaignName", "CreateBy", "CreateDate", "EndDate", "StartDate", "TemplateId", "UploadedFileName", "UploadedFileUrl" },
                values: new object[] { 2, "Khuyến mãi VIP tháng 6", "marketing_user", new DateTime(2025, 6, 8, 14, 0, 0, 0, DateTimeKind.Utc), new DateTime(2025, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2025, 6, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "vip_june.csv", "/uploads/vip_june.csv" });

            migrationBuilder.InsertData(
                table: "Recipients",
                columns: new[] { "RecipientId", "CampaignId", "CustomDataJson", "RecipientEmail", "RecipientName" },
                values: new object[] { 1, 1, "{ \"TenNguoiNhan\": \"Văn A\", \"ThanhPho\": \"Hà Nội\" }", "levana@example.com", "Lê Văn A" });

            migrationBuilder.InsertData(
                table: "Recipients",
                columns: new[] { "RecipientId", "CampaignId", "CustomDataJson", "RecipientEmail", "RecipientName" },
                values: new object[] { 2, 1, "{ \"TenNguoiNhan\": \"Thị B\", \"ThanhPho\": \"Đà Nẵng\" }", "nguyenthib@example.com", "Nguyễn Thị B" });

            migrationBuilder.InsertData(
                table: "Recipients",
                columns: new[] { "RecipientId", "CampaignId", "CustomDataJson", "RecipientEmail", "RecipientName" },
                values: new object[] { 3, 2, "{ \"TenNguoiNhan\": \"Văn C\", \"MembershipLevel\": \"Gold\", \"Email\": \"phamvanc@example.com\" }", "phamvanc@example.com", "Phạm Văn C" });

            migrationBuilder.InsertData(
                table: "EmailLogs",
                columns: new[] { "LogId", "ErrorMessage", "IsSuccess", "RecipientId", "SentDate", "TemplateId" },
                values: new object[] { 1, "", true, 1, new DateTime(2025, 6, 10, 9, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.InsertData(
                table: "EmailLogs",
                columns: new[] { "LogId", "ErrorMessage", "IsSuccess", "RecipientId", "SentDate", "TemplateId" },
                values: new object[] { 2, "SMTP connection timeout.", false, 3, new DateTime(2025, 6, 13, 8, 30, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.CreateIndex(
                name: "IX_Campaigns_TemplateId",
                table: "Campaigns",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_RecipientId",
                table: "EmailLogs",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_EmailLogs_TemplateId",
                table: "EmailLogs",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Recipients_CampaignId",
                table: "Recipients",
                column: "CampaignId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailLogs");

            migrationBuilder.DropTable(
                name: "Recipients");

            migrationBuilder.DropTable(
                name: "Campaigns");

            migrationBuilder.DropTable(
                name: "Templates");
        }
    }
}
