using System;
using mail_marketing_api.Models;

namespace mail_marketing_api.Services
{
    public interface IEmailRecipientService
    {
        Task<List<EmailRecipient>> GetAllRecipients();
        Task<EmailRecipient> GetRecipientById(int id);
        Task<List<EmailRecipient>> SearchByKeyword(string keyword);
        Task<bool> AddEmailRecipientsAsync(List<EmailRecipient> recipients);
        Task<List<EmailRecipient>> GetRecipientsByBatchIdAsync(int batchId);
    }
}

