using mail_marketing_api.Models; // Cần dùng ResponseDTO
using SendGrid.Helpers.Mail; // Cần dùng các lớp của SendGrid
using System.Collections.Generic;
using System.Threading.Tasks;

namespace mail_marketing_api.Services
{
    public class SendGridRecipient
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Substitutions { get; set; } = new Dictionary<string, string>();
    }

    public interface ISendGridService
    {
        Task<ResponseDTO<object>> SendBatchEmailAsync(
            string fromEmail,
            string fromName,
            string subject,
            string htmlBody,
            string? textBody,
            List<SendGridRecipient> recipients);
    }
}