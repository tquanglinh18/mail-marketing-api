using System;
using mail_marketing_api.Models;

namespace mail_marketing_api.Services
{
    public interface ITemplateService
    {
       EmailTemplate CreateTemplate(EmailTemplate emailTemplate);

        Task<List<EmailTemplate>> GetAllTemplates();

        Task<EmailTemplate> GetTeamplateById(int id);

        Task<EmailTemplate> DeleteTemplate(int id);

        Task<EmailTemplate?> UpdateTemplate(int id, EmailTemplate updatedTemplate);
    }
}

