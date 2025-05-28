using System;
using mail_marketing_api.Data;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;

namespace mail_marketing_api.Services
{
    public class TemplateService : ITemplateService
    {
        private readonly AppDbContext _appDbContext;
        public TemplateService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public EmailTemplate CreateTemplate(EmailTemplate emailTemplate)
        {
            _appDbContext.EmailTemplates.Add(emailTemplate);
            _appDbContext.SaveChanges();
            return emailTemplate;
        }

        public async Task<EmailTemplate> DeleteTemplate(int id)
        {
            var temp = await _appDbContext.EmailTemplates.FirstOrDefaultAsync(t => t.TemplateId == id);
            if (temp == null) return null;

            _appDbContext.EmailTemplates.Remove(temp);
            _appDbContext.SaveChanges();
            return temp;
        }

        public async Task<List<EmailTemplate>> GetAllTemplates()
        {
            var templates = await _appDbContext.EmailTemplates.ToListAsync();
            if (templates == null) return null;
            return templates;
        }

        public async Task<EmailTemplate> GetTeamplateById(int id)
        {
            var template = await _appDbContext.EmailTemplates.FirstOrDefaultAsync(t => t.TemplateId == id);
            if (template == null) return null;
            return template;
        }

    }
}

