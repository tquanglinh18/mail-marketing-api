using System;
namespace mail_marketing_api.Models
{
    public class SendMailRequest
    {
        public Guid TemplateId { get; set; }

        public Guid UploadBatchId { get; set; }

        public string? CustomSubject { get; set; }

        public Dictionary<string, string>? GlobalMergeFields { get; set; }

        public List<TemplateImage>? Images { get; set; }

        public bool IsSandbox { get; set; } = false;
    }
}

