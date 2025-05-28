using mail_marketing_api.Models;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace mail_marketing_api.Services
{
    public class SendGridService : ISendGridService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;

        public SendGridService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["SendGrid:ApiKey"];

            if (string.IsNullOrEmpty(_apiKey))
            {
                throw new ArgumentException("SendGrid ApiKey chưa được cấu hình trong appsettings.json.");
            }
        }

        public async Task<ResponseDTO<object>> SendBatchEmailAsync(
            string fromEmail,
            string fromName,
            string subject,
            string htmlBody,
            string? textBody,
            List<SendGridRecipient> recipients)
        {
            if (recipients == null || !recipients.Any())
            {
                return new ResponseDTO<object> { Code = 400, IsSuccessed = false, Message = "Không có người nhận." };
            }

            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(fromEmail, fromName);

                // Tạo đối tượng SendGridMessage cơ bản
                var msg = new SendGridMessage()
                {
                    From = from,
                    Subject = subject,
                    HtmlContent = htmlBody,
                    PlainTextContent = textBody,
                    Personalizations = new List<Personalization>()
                };

                // --- Xử lý gửi hàng loạt với Personalization ---

                foreach (var recipient in recipients)
                {
                    var personalization = new Personalization()
                    {
                        Tos = new List<EmailAddress> { new EmailAddress(recipient.Email, recipient.Name) },
                        Substitutions = recipient.Substitutions ?? new Dictionary<string, string>()
                    };

                    msg.Personalizations.Add(personalization);
                }

                // Gửi email
                var response = await client.SendEmailAsync(msg);

                // Kiểm tra kết quả
                if (response.IsSuccessStatusCode)
                {
                    // SendGrid chấp nhận (thường là 202 Accepted)
                    return new ResponseDTO<object>
                    {
                        Code = (int)response.StatusCode,
                        IsSuccessed = true,
                        Message = "SendGrid đã chấp nhận yêu cầu gửi mail.",
                        Data = await response.Body.ReadAsStringAsync()
                    };
                }
                else
                {
                    // SendGrid trả về lỗi
                    string errorBody = await response.Body.ReadAsStringAsync();
                    Console.WriteLine($"SendGrid Error: {response.StatusCode} - {errorBody}");
                    return new ResponseDTO<object>
                    {
                        Code = (int)response.StatusCode,
                        IsSuccessed = false,
                        Message = $"SendGrid Error: {errorBody}",
                        Data = errorBody
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"SendGrid Exception: {ex.Message}");
                return new ResponseDTO<object>
                {
                    Code = (int)HttpStatusCode.InternalServerError, // 500
                    IsSuccessed = false,
                    Message = $"Lỗi hệ thống khi gửi mail qua SendGrid: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}