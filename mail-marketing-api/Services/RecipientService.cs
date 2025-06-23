using mail_marketing_api.Data;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_marketing_api.Services
{
    public class RecipientService : IRecipientService
    {
        private readonly AppDbContext _appDbContext;

        public RecipientService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Recipient>> GetAll()
        {
            var lstContact = await _appDbContext.Recipients.ToListAsync();
            return lstContact;
        }

        public async Task<Recipient> GetById(int id)
        {
            var contact = await _appDbContext
                .Recipients.Include(r => r.Campaign).FirstOrDefaultAsync(c => c.RecipientId == id);
            if (contact == null) return null;
            return contact;
        }

        public async Task<List<Recipient>> SearchByKeyword(string keyword)
        {
            var results = await _appDbContext.Recipients
                .Where(r => r.RecipientEmail.Contains(keyword) || r.RecipientName.Contains(keyword))
                .ToListAsync();

            return results;
        }

        public async Task<bool> AddRecipientsAsync(List<Recipient> recipients)
        {
            if (recipients == null || !recipients.Any())
            {
                return true;
            }

            try
            {
                await _appDbContext.Recipients.AddRangeAsync(recipients);
                int savedCount = await _appDbContext.SaveChangesAsync();
                return savedCount > 0;
            }
            catch (DbUpdateException ex)
            {
                return false;
            }
        }

        public async Task<List<Recipient>> GetByCampaignIdAsync(int campaignId)
        {
            return await _appDbContext.Recipients
                                 .Where(r => r.CampaignId == campaignId)
                                 .ToListAsync();
        }
    }
}