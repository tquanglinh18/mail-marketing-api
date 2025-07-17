using System;
using mail_marketing_api.Models;

namespace mail_marketing_api.Services
{
	public interface ICampaignService
	{
        Task<List<Campaign>> GetAll();

        Task<Campaign?> GetByIdAsync(int id);

        Task<ResponseDTO<Campaign>> CreateCampaignAsync(Campaign campaign);

        Task<Campaign> UpdateCampaignAsync(int id, Campaign campaign);

        Task<bool> DeleteCampaignAsync(int id);
    }
}