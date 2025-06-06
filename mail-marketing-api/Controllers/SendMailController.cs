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
        private readonly IMailgunService _mailgunService;
        private readonly IConfiguration _configuration;

        public SendMailController(
            ITemplateService templateService,
            IEmailRecipientService emailRecipientService,
            ISendGridService sendGridService,
            IMailgunService mailgunService,
            IConfiguration configuration)
        {
            _templateService = templateService;
            _emailRecipientService = emailRecipientService;
            _sendGridService = sendGridService;
            _mailgunService = mailgunService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendBulkMailWithSendGrid([FromBody] SendMailRequest request)
        {
            // --- 1. Lấy Template ---
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
            var fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@linhtq.com";
            var fromName = _configuration["SendGrid:FromName"] ?? "LinhTQ";

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
                                sgRecipient.Substitutions[$"-{field.Key}-"] = field.Value;
                            }
                        }
                    }
                    catch (JsonException ex) { Console.WriteLine($"Lỗi parse JSON cho {recipient.RecipientEmail}: {ex.Message}"); }
                }

                // Thêm các trường cơ bản (nếu template dùng chúng)
                sgRecipient.Substitutions["-Name-"] = recipient.RecipientName ?? "";
                sgRecipient.Substitutions["-Email-"] = recipient.RecipientEmail ?? "";

                sendGridRecipients.Add(sgRecipient);
            }


            // --- 5. Gọi SendGridService ---
            var sendGridResponse = await _sendGridService.SendBatchEmailAsync(
                fromEmail,
                fromName,
                request.CustomSubject ?? template.TemplateName,
                template.HtmlContent,
                null,
                sendGridRecipients
            );

            // --- 6. Xử lý Phản hồi & Ghi Log (Tạm thời) ---
            if (sendGridResponse.IsSuccessed)
            {
                Console.WriteLine($"SendGrid Success: {sendGridResponse.Message}");
                return Ok(sendGridResponse);
            }
            else
            {
                Console.WriteLine($"SendGrid Failed ({sendGridResponse.Code}): {sendGridResponse.Message}");
                return StatusCode(sendGridResponse.Code > 0 ? sendGridResponse.Code : (int)HttpStatusCode.InternalServerError, sendGridResponse);
            }
        }

        [HttpPost] 
        public async Task<IActionResult> SendBulkMailWithMailgun([FromBody] SendMailRequest request)
        {
            // --- 1. Lấy Template ---
            var template = await _templateService.GetTeamplateById(request.TemplateId);
            if (template == null)
            {
                return NotFound(new ResponseDTO<object> { Code = 404, Message = $"Không tìm thấy TemplateId {request.TemplateId}." });
            }
            if (string.IsNullOrEmpty(template.HtmlContent))
            {
                return BadRequest(new ResponseDTO<object> { Code = 400, Message = $"Nội dung HTML của TemplateId {request.TemplateId} không được để trống." });
            }

            // --- 2. Lấy Recipients ---
            var recipients = await _emailRecipientService.GetRecipientsByBatchIdAsync(request.UploadBatchId);
            if (recipients == null || !recipients.Any())
            {
                return NotFound(new ResponseDTO<object> { Code = 404, Message = $"Không tìm thấy người nhận nào cho BatchId {request.UploadBatchId}." });
            }

            // --- 3. Lấy thông tin người gửi từ Config cho Mailgun ---
            var fromEmail = _configuration["Mailgun:FromEmail"] ?? "Mailgun Test <noreply@yourdomain.com>";

            // --- 4. Xây dựng MailgunRequest ---
            var mailgunRequest = new MailgunRequest
            {
                From = fromEmail,
                Subject = request.CustomSubject ?? template.TemplateName,
                HtmlBody = template.HtmlContent, // Hoặc TextBody nếu bạn muốn gửi text
                To = recipients.Select(r => r.RecipientEmail).ToList(), // Danh sách tất cả email người nhận
                Tags = new List<string> { "test_mailgun_batch", $"batch_{request.UploadBatchId}" },
                // Các tùy chọn tracking có thể đặt ở đây nếu muốn
                // Tracking = true,
                // TrackingClicks = true,
                // TrackingOpens = true
            };

            // --- 5. Xây dựng RecipientVariables ---
            var recipientVariablesDict = new Dictionary<string, Dictionary<string, object>>();
            foreach (var recipient in recipients)
            {
                var currentVars = new Dictionary<string, object>();

                // Thêm Global Fields
                if (request.GlobalMergeFields != null)
                {
                    foreach (var field in request.GlobalMergeFields)
                    {
                        // Key ở đây KHÔNG cần dấu %recipient.%
                        currentVars[field.Key] = field.Value ?? "";
                    }
                }

                // Thêm Recipient Fields (từ CustomDataJson)
                if (!string.IsNullOrWhiteSpace(recipient.CustomDataJson))
                {
                    try
                    {
                        var customData = JsonSerializer.Deserialize<Dictionary<string, object>>(recipient.CustomDataJson);
                        if (customData != null)
                        {
                            foreach (var field in customData)
                            {
                                currentVars[field.Key] = field.Value?.ToString() ?? "";
                            }
                        }
                    }
                    catch (JsonException ex) { Console.WriteLine($"Lỗi parse JSON cho {recipient.RecipientEmail}: {ex.Message}"); }
                }

                // Thêm các trường cơ bản
                // Key ở đây phải khớp với phần sau dấu chấm trong tag %recipient.KEY%
                currentVars["Name"] = recipient.RecipientName ?? "";   // Sẽ khớp với %recipient.Name% trong template
                currentVars["Email"] = recipient.RecipientEmail ?? ""; // Sẽ khớp với %recipient.Email% trong template
                currentVars["id"] = recipient.RecipientId;           // Sẽ khớp với %recipient.id% trong template

                // Các key khác bạn định nghĩa trong CustomDataJson hoặc GlobalMergeFields
                // ví dụ: "ThanhPho", "MaKH" sẽ khớp với %recipient.ThanhPho%, %recipient.MaKH%

                recipientVariablesDict[recipient.RecipientEmail] = currentVars;
            }
            mailgunRequest.SetRecipientVariables(recipientVariablesDict); // Chuyển dictionary thành chuỗi JSON

            // --- 6. Gọi MailgunService ---
            var mailgunResponse = await _mailgunService.SendBatchEmailAsync(mailgunRequest);

            //// --- 7. Ghi Log cho từng người nhận ---
            //string responseDataForLog = mailgunResponse.Data?.ToString();
            //foreach (var recipient in recipients)
            //{
            //    try
            //    {
            //        await _emailLogService.CreateLogAsync(
            //            recipient.RecipientId,
            //            request.TemplateId,
            //            mailgunResponse.IsSuccessed,
            //            mailgunResponse.Message,
            //            responseDataForLog
            //        );
            //    }
            //    catch (Exception logEx)
            //    {
            //        Console.WriteLine($"Lỗi khi ghi EmailLog cho RecipientId {recipient.RecipientId} (Mailgun Send): {logEx.Message}");
            //    }
            //}

            // --- 8. Xử lý Phản hồi ---
            if (mailgunResponse.IsSuccessed)
            {
                return Ok(mailgunResponse);
            }
            else
            {
                return StatusCode(mailgunResponse.Code > 0 ? mailgunResponse.Code : (int)HttpStatusCode.InternalServerError, mailgunResponse);
            }
        }
    }
}