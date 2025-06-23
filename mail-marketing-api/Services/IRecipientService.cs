using System;
using mail_marketing_api.Models;

namespace mail_marketing_api.Services
{
    public interface IRecipientService
    {
        Task<List<Recipient>> GetAll();
        Task<Recipient> GetById(int id);
        Task<List<Recipient>> SearchByKeyword(string keyword);
        Task<bool> AddRecipientsAsync(List<Recipient> recipients);
        Task<List<Recipient>> GetByCampaignIdAsync(int campaignId);
    }
}

