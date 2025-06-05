using mail_marketing_api.Models;
using Microsoft.Extensions.Configuration;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace mail_marketing_api.Services
{
    public class MailgunService : IMailgunService
    {
        private readonly IConfiguration _configuration;
        private readonly string _apiKey;
        private readonly string _domain;
        private readonly RestClient _client;

        public MailgunService(IConfiguration configuration)
        {
            _configuration = configuration;
            _apiKey = _configuration["Mailgun:ApiKey"];
            _domain = _configuration["Mailgun:Domain"];

            if (string.IsNullOrEmpty(_apiKey) || string.IsNullOrEmpty(_domain))
            {
                throw new ArgumentException("Mailgun ApiKey hoặc Domain chưa được cấu hình trong appsettings.json.");
            }

            var options = new RestClientOptions($"https://api.mailgun.net/v3")
            {
                Authenticator = new HttpBasicAuthenticator("api", _apiKey)
            };
            _client = new RestClient(options);
        }

        public async Task<ResponseDTO<object>> SendBatchEmailAsync(MailgunRequest request)
        {
            if (request == null || !request.To.Any())
            {
                return new ResponseDTO<object> { Code = (int)HttpStatusCode.BadRequest, IsSuccessed = false, Message = "Yêu cầu không hợp lệ hoặc không có người nhận." };
            }
            if (string.IsNullOrEmpty(request.HtmlBody) && string.IsNullOrEmpty(request.TextBody))
            {
                return new ResponseDTO<object> { Code = (int)HttpStatusCode.BadRequest, IsSuccessed = false, Message = "Nội dung email (Text hoặc HTML) không được để trống." };
            }

            var restRequest = new RestRequest($"{_domain}/messages", Method.Post);

            restRequest.AddParameter("from", request.From);
            foreach (var recipientEmail in request.To)
            {
                restRequest.AddParameter("to", recipientEmail);
            }
            restRequest.AddParameter("subject", request.Subject);

            if (!string.IsNullOrEmpty(request.HtmlBody))
            {
                restRequest.AddParameter("html", request.HtmlBody);
            }
            else
            {
                restRequest.AddParameter("text", request.TextBody);
            }
            if (!string.IsNullOrEmpty(request.RecipientVariables))
            {
                restRequest.AddParameter("recipient-variables", request.RecipientVariables);
            }

            try
            {
                var response = await _client.ExecuteAsync(restRequest);

                if (response.IsSuccessful)
                {
                    return new ResponseDTO<object>
                    {
                        Code = (int)response.StatusCode,
                        IsSuccessed = true,
                        Message = "Mailgun đã chấp nhận yêu cầu gửi mail.",
                        Data = response.Content
                    };
                }
                else
                {
                    Console.WriteLine($"Mailgun Error: Status={response.StatusCode}, Content={response.Content}, ErrorMessage={response.ErrorMessage}");
                    return new ResponseDTO<object>
                    {
                        Code = (int)response.StatusCode,
                        IsSuccessed = false,
                        Message = $"Mailgun Error: {response.ErrorMessage ?? response.Content}",
                        Data = response.Content
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Mailgun Exception: {ex.ToString()}");
                return new ResponseDTO<object>
                {
                    Code = (int)HttpStatusCode.InternalServerError,
                    IsSuccessed = false,
                    Message = $"Lỗi hệ thống khi gửi mail qua Mailgun: {ex.Message}",
                    Data = ex.ToString()
                };
            }
        }
    }
}