using mail_marketing_api.Models;
using mail_marketing_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace mail_marketing_api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class SendMailController : ControllerBase 
    {
        private readonly ITemplateService _templateService;
        private readonly IEmailRecipientService _emailRecipientService;
        private readonly ISendGridService _sendGridService;
        private readonly IConfiguration _configuration;

        public SendMailController(
            ITemplateService templateService,
            IEmailRecipientService emailRecipientService,
            ISendGridService sendGridService,
            IConfiguration configuration)
        {
            _templateService = templateService;
            _emailRecipientService = emailRecipientService;
            _sendGridService = sendGridService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendBulkMail([FromBody] SendMailRequest request)
        {
            // --- 1. Lấy Template ---
            // Giả sử GetTeamplateById là sync, nếu là async cần await
            var template = await _templateService.GetTeamplateById(request.TemplateId);
            if (template == null)
            {
                return NotFound(new ResponseDTO<object> { Code = 404, Message = $"Không tìm thấy TemplateId {request.TemplateId}." });
            }

            if (string.IsNullOrEmpty(template.HtmlContent))
            {
                return BadRequest(new ResponseDTO<object>
                {
                    Code = 400,
                    Message = $"Nội dung HTML (HtmlContent) của TemplateId {request.TemplateId} không được để trống."
                });
            }

            // --- 2. Lấy Recipients ---
            var recipients = await _emailRecipientService.GetRecipientsByBatchIdAsync(request.UploadBatchId);
            if (recipients == null || !recipients.Any())
            {
                return NotFound(new ResponseDTO<object> { Code = 404, Message = $"Không tìm thấy người nhận nào cho BatchId {request.UploadBatchId}." });
            }

            // --- 3. Lấy thông tin người gửi từ Config ---
            var fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@yourdomain.com"; // Lấy From Email từ config
            var fromName = _configuration["SendGrid:FromName"] ?? "Your Company"; // Lấy From Name từ config

            // --- 4. Xây dựng danh sách SendGridRecipient ---
            var sendGridRecipients = new List<SendGridRecipient>();
            foreach (var recipient in recipients)
            {
                var sgRecipient = new SendGridRecipient
                {
                    Email = recipient.RecipientEmail,
                    Name = recipient.RecipientName
                };

                // Thêm Global Fields
                if (request.GlobalMergeFields != null)
                {
                    foreach (var field in request.GlobalMergeFields)
                    {
                        // Quan trọng: Key phải khớp với tag trong template (ví dụ: -CampaignName-)
                        sgRecipient.Substitutions[$"-{field.Key}-"] = field.Value;
                    }
                }

                // Thêm Recipient Fields (từ CustomDataJson)
                if (!string.IsNullOrWhiteSpace(recipient.CustomDataJson))
                {
                    try
                    {
                        var custom = JsonSerializer.Deserialize<Dictionary<string, string>>(recipient.CustomDataJson); // Đọc ra string
                        if (custom != null)
                        {
                            foreach (var field in custom)
                            {
                                // Quan trọng: Key phải khớp với tag trong template (ví dụ: -ThanhPho-)
                                sgRecipient.Substitutions[$"-{field.Key}-"] = field.Value;
                            }
                        }
                    }
                    catch (JsonException ex) { Console.WriteLine($"Lỗi parse JSON cho {recipient.RecipientEmail}: {ex.Message}"); }
                }

                // Thêm các trường cơ bản (nếu template dùng chúng)
                sgRecipient.Substitutions["-Name-"] = recipient.RecipientName ?? "";
                sgRecipient.Substitutions["-Email-"] = recipient.RecipientEmail ?? "";
                // Thêm bất kỳ key nào khác bạn dùng trong template

                sendGridRecipients.Add(sgRecipient);
            }


            // --- 5. Gọi SendGridService ---
            var sendGridResponse = await _sendGridService.SendBatchEmailAsync(
                fromEmail,
                fromName,
                request.CustomSubject ?? template.TemplateName, // Lấy subject
                template.HtmlContent, // Lấy HTML
                null, // Bỏ qua TextBody nếu không có
                sendGridRecipients // Danh sách người nhận đã xử lý
            );

            // --- 6. Xử lý Phản hồi & Ghi Log (Tạm thời) ---
            if (sendGridResponse.IsSuccessed)
            {
                // **TODO:** Triển khai IEmailLogService để ghi log thành công vào DB
                Console.WriteLine($"SendGrid Success: {sendGridResponse.Message}");
                return Ok(sendGridResponse);
            }
            else
            {
                // **TODO:** Triển khai IEmailLogService để ghi log thất bại vào DB
                Console.WriteLine($"SendGrid Failed ({sendGridResponse.Code}): {sendGridResponse.Message}");
                return StatusCode(sendGridResponse.Code > 0 ? sendGridResponse.Code : (int)HttpStatusCode.InternalServerError, sendGridResponse);
            }
        }
    }
}