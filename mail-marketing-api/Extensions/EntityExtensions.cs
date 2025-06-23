using System;
using mail_marketing_api.DTO;
using mail_marketing_api.Models;

namespace mail_marketing_api.Extensions
{
    public static class EntityExtensions
    {
        public static EmailTemplateDTO MapToDTO(this EmailTemplate entity)
        {
            return new EmailTemplateDTO
            {
                TemplateId = entity.TemplateId,
                TemplateName = entity.TemplateName,
                HtmlContent = entity.HtmlContent,
                ImageStorageType = entity.ImageStorageType,
                CreatedDate = entity.CreatedDate,
                CreatedBy = entity.CreatedBy
            };
        }

        public static RecipientDTO MapToDTO(this Recipient entity)
        {
            return new RecipientDTO
            {
                RecipientId = entity.RecipientId,
                RecipientName = entity.RecipientName,
                RecipientEmail = entity.RecipientEmail,
                CustomDataJson = entity.CustomDataJson,
                CampaignId = entity.CampaignId
            };
        }

        public static EmailLogsDTO MapToDTO(this EmailLogs entity)
        {
            return new EmailLogsDTO
            {
                LogId = entity.LogId,
                RecipientId = entity.RecipientId,
                TemplateId = entity.TemplateId,
                SentDate = entity.SentDate,
                IsSuccess = entity.IsSuccess,
                ErrorMessage = entity.ErrorMessage,
                RecipientName = entity.EmailRecipient?.RecipientName,
                RecipientEmail = entity.EmailRecipient?.RecipientEmail,
                TemplateName = entity.EmailTemplate?.TemplateName
            };
        }

        public static CampaignDTO MapToDTO(this Campaign entity)
        {
            return new CampaignDTO
            {
                CampaignId = entity.CampaignId,
                CampaignName = entity.CampaignName,
                TemplateId = entity.TemplateId,
                UploadedFileName = entity.UploadedFileName,
                UploadedFileUrl = entity.UploadedFileUrl,
                CreateDate = entity.CreateDate,
                CreateBy = entity.CreateBy,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                Template = entity.Template?.MapToDTO(),
                Recipients = entity.Recipients?.Select(r => r.MapToDTO()).ToList()
            };
        }

        public static List<RecipientDTO> MapToDTO(this List<Recipient> recipients)
        {
            return recipients.Select(r => new RecipientDTO
            {
                RecipientId = r.RecipientId,
                CampaignId = r.CampaignId,
                RecipientName = r.RecipientName,
                RecipientEmail = r.RecipientEmail,
                CustomDataJson = r.CustomDataJson
            }).ToList();
        }
    }

}

