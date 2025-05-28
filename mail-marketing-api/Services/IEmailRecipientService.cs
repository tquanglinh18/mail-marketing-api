using System;
using mail_marketing_api.Models;

namespace mail_marketing_api.Services
{
    public interface IEmailRecipientService
    {
        Task<bool> AddEmailRecipientsAsync(List<EmailRecipient> recipients);
        Task<List<EmailRecipient>> GetRecipientsByBatchIdAsync(int batchId);
    }
}

