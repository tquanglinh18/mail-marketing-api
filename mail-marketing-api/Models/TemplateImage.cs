using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    public class TemplateImage
    {
        public string Name { get; set; }

        public string Data { get; set; }

        public bool IsBase64 { get; set; }
    }
}

