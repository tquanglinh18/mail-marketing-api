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
        private readonly IRecipientService _emailRecipientService;
        private readonly IConfiguration _configuration;

        public SendMailController(
            ITemplateService templateService,
            IRecipientService emailRecipientService,
            IConfiguration configuration)
        {
            _templateService = templateService;
            _emailRecipientService = emailRecipientService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendMailToRecipient([FromBody] SendMailRequest request)
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
            var recipients = await _emailRecipientService.GetByCampaignIdAsync(request.CampaignId);
            if (recipients == null || !recipients.Any())
            {
                return NotFound(new ResponseDTO<object> { Code = 404, Message = $"Không tìm thấy người nhận nào cho CampaignId {request.CampaignId}." });
            }

            // --- 3. Lấy thông tin người gửi từ Config ---
            var fromEmail = _configuration["Sender:FromEmail"] ?? "noreply@linhtq.com";
            var fromName = _configuration["Sender:FromName"] ?? "LinhTQ";

            // --- 4. Xây dựng danh sách Recipient ---


            // --- 5. Gọi Send Mail Service ---


            // --- 6. Xử lý Phản hồi & Ghi Log (Tạm thời) ---


            return Ok(new ResponseDTO<object>
            {
                Code = 200,
                Message = $"Đã call"
            });

        }
    }
}