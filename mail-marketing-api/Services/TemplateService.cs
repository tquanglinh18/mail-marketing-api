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

        public async Task<List<EmailTemplate>> GetAllTemplates()
        {
            var templates = await _appDbContext.Templates.ToListAsync();
            if (templates == null) return null;
            return templates;
        }

        public async Task<EmailTemplate> GetTeamplateById(int id)
        {
            var template = await _appDbContext.Templates.FirstOrDefaultAsync(t => t.TemplateId == id);
            if (template == null) return null;
            return template;
        }

        public EmailTemplate CreateTemplate(EmailTemplate emailTemplate)
        {
            _appDbContext.Templates.Add(emailTemplate);
            _appDbContext.SaveChanges();
            return emailTemplate;
        }

        public async Task<EmailTemplate?> UpdateTemplate(int id, EmailTemplate updatedTemplate)
        {
            var existingTemplate = await _appDbContext.Templates.FirstOrDefaultAsync(t => t.TemplateId == id);
            if (existingTemplate == null)
                return null;

            // Cập nhật dữ liệu
            existingTemplate.TemplateName = updatedTemplate.TemplateName;
            existingTemplate.HtmlContent = updatedTemplate.HtmlContent;
            existingTemplate.ImageStorageType = updatedTemplate.ImageStorageType;
            existingTemplate.CreatedDate = updatedTemplate.CreatedDate;
            existingTemplate.CreatedBy = updatedTemplate.CreatedBy;

            await _appDbContext.SaveChangesAsync();
            return existingTemplate;
        }

        public async Task<EmailTemplate> DeleteTemplate(int id)
        {
            var temp = await _appDbContext.Templates.FirstOrDefaultAsync(t => t.TemplateId == id);
            if (temp == null) return null;

            _appDbContext.Templates.Remove(temp);
            _appDbContext.SaveChanges();
            return temp;
        }
    }
}

