using mail_marketing_api.Data;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_marketing_api.Services
{
    public class EmailRecipientService : IEmailRecipientService
    {
        private readonly AppDbContext _appDbContext;

        public EmailRecipientService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<EmailRecipient>> GetAllRecipients()
        {
            var lstContact = await _appDbContext.EmailRecipients.Include(recipient => recipient.UploadBatch).ToListAsync();
            return lstContact;
        }

        public async Task<EmailRecipient> GetRecipientById(int id)
        {
            var contact = await _appDbContext
                .EmailRecipients.Include(recipient => recipient.UploadBatch).FirstOrDefaultAsync(c => c.RecipientId == id);
            if (contact == null) return null;
            return contact;
        }

        public async Task<List<EmailRecipient>> SearchByKeyword(string keyword)
        {
            var results = await _appDbContext.EmailRecipients
                .Where(r => r.RecipientEmail.Contains(keyword) || r.RecipientName.Contains(keyword))
                .ToListAsync();

            return results;
        }

        public async Task<bool> AddEmailRecipientsAsync(List<EmailRecipient> recipients)
        {
            if (recipients == null || !recipients.Any())
            {
                return true;
            }

            try
            {
                await _appDbContext.EmailRecipients.AddRangeAsync(recipients);
                int savedCount = await _appDbContext.SaveChangesAsync();
                return savedCount > 0;
            }
            catch (DbUpdateException ex)
            {
                return false;
            }
        }

        public async Task<List<EmailRecipient>> GetRecipientsByBatchIdAsync(int batchId)
        {
            return await _appDbContext.EmailRecipients
                                 .Where(r => r.BatchId == batchId)
                                 .ToListAsync();
        }
    }
}