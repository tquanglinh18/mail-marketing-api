using mail_marketing_api.Data;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace mail_marketing_api.Services
{
    public class CampaignService : ICampaignService
    {
        private readonly AppDbContext _context;

        public CampaignService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Campaign> CreateCampaignAsync(Campaign campaign)
        {
            if (string.IsNullOrWhiteSpace(campaign.CampaignName))
                throw new ArgumentNullException(nameof(campaign.CampaignName));
            if (string.IsNullOrWhiteSpace(campaign.UploadedFileName))
                throw new ArgumentNullException(nameof(campaign.UploadedFileName));


            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();

            return campaign;
        }

        public async Task<Campaign> UpdateCampaignAsync(int id, Campaign updatedCampaign)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
                throw new KeyNotFoundException("Không tìm thấy chiến dịch.");

            // Cập nhật giá trị
            campaign.CampaignName = updatedCampaign.CampaignName;
            campaign.TemplateId = updatedCampaign.TemplateId;
            campaign.UploadedFileName = updatedCampaign.UploadedFileName;
            campaign.StartDate = updatedCampaign.StartDate;
            campaign.EndDate = updatedCampaign.EndDate;
            campaign.CreateBy = updatedCampaign.CreateBy;

            await _context.SaveChangesAsync();
            return campaign;
        }

        public async Task<bool> DeleteCampaignAsync(int id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign == null)
                return false;

            _context.Campaigns.Remove(campaign);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<List<Campaign>> GetAll()
        {
            return await _context.Campaigns
                                 .OrderByDescending(b => b.CreateDate)
                                 .Select(b => new Campaign
                                 {
                                     CampaignId = b.CampaignId,
                                     CampaignName = b.CampaignName,
                                     TemplateId = b.TemplateId,
                                     Template = b.Template,
                                     UploadedFileName = b.UploadedFileName,
                                     CreateDate = b.CreateDate,
                                     CreateBy = b.CreateBy,
                                     StartDate = b.StartDate,
                                     EndDate = b.EndDate,
                                 })
                                 .ToListAsync();
        }

        public async Task<Campaign?> GetByIdAsync(int id)
        {
            return await _context.Campaigns
                                 .Include(c => c.Template)
                                 .FirstOrDefaultAsync(c => c.CampaignId == id);
        }

    }
}