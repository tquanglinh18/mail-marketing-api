using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    public class SendMailRequest
    {
        public int TemplateId { get; set; }

        public int UploadBatchId { get; set; }

        public string? CustomSubject { get; set; }

        public Dictionary<string, string>? GlobalMergeFields { get; set; }

        public List<TemplateImage>? Images { get; set; }

        public bool IsSandbox { get; set; } = false;
    }
}

