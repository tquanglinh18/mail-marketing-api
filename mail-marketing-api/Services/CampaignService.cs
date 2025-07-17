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

        public async Task<ResponseDTO<Campaign>> CreateCampaignAsync(Campaign campaign)
        {
            var response = new ResponseDTO<Campaign>();

            try
            {
                // Kiểm tra tên chiến dịch
                if (string.IsNullOrWhiteSpace(campaign.CampaignName))
                {
                    response.Code = 400;
                    response.Message = "Tên chiến dịch không được để trống.";
                    return response;
                }

                // Kiểm tra tên file
                if (string.IsNullOrWhiteSpace(campaign.UploadedFileName))
                {
                    response.Code = 400;
                    response.Message = "Tên file upload không được để trống.";
                    return response;
                }

                // Kiểm tra TemplateId
                if (campaign.TemplateId <= 0)
                {
                    response.Code = 400;
                    response.Message = "Bạn phải chọn một mẫu email hợp lệ.";
                    return response;
                }

                // Kiểm tra Template có tồn tại không
                var template = await _context.Templates
                    .FirstOrDefaultAsync(t => t.TemplateId == campaign.TemplateId);

                if (template == null)
                {
                    response.Code = 404;
                    response.Message = $"Không tìm thấy template với ID = {campaign.TemplateId}.";
                    return response;
                }

                // Gán navigation property
                campaign.Template = template;
                campaign.CreateDate = DateTime.UtcNow;

                _context.Campaigns.Add(campaign);
                await _context.SaveChangesAsync();

                // Load lại để trả về campaign có đầy đủ Template
                var createdCampaign = await _context.Campaigns
                    .Include(c => c.Template)
                    .FirstOrDefaultAsync(c => c.CampaignId == campaign.CampaignId);

                // Thành công
                response.Code = 200;
                response.IsSuccessed = true;
                response.Message = "Tạo chiến dịch thành công.";
                response.Data = createdCampaign;
                return response;
            }
            catch (Exception ex)
            {
                response.Code = 500;
                response.Message = $"Lỗi hệ thống: {ex.Message}";
                response.IsSuccessed = false;
                return response;
            }
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