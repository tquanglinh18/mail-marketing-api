using System;
namespace mail_marketing_api.DTO
{
	public class EmailLogsDTO
	{
        public int LogId { get; set; }
        public int RecipientId { get; set; }
        public int TemplateId { get; set; }
        public DateTime SentDate { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
        public string RecipientName { get; set; }
        public string RecipientEmail { get; set; }
        public string TemplateName { get; set; }
    }
}

