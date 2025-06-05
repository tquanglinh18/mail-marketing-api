using System;
using System.Text.Json;

namespace mail_marketing_api.Models
{
    public class MailgunRequest
    {
        public string From { get; set; }
        public List<string> To { get; set; } = new List<string>();
        public string Subject { get; set; }
        public string? TextBody { get; set; }
        public string? HtmlBody { get; set; }
        public string? RecipientVariables { get; set; }
        public List<string>? Tags { get; set; }
        public bool? Tracking { get; set; }
        public bool? TrackingOpens { get; set; }
        public bool? TrackingClicks { get; set; }
        public List<string>? Cc { get; set; }
        public List<string>? Bcc { get; set; }
        public string? ReplyTo { get; set; }

        public void SetRecipientVariables(Dictionary<string, Dictionary<string, object>> variables)
        {
            if (variables != null && variables.Count > 0)
            {
                RecipientVariables = JsonSerializer.Serialize(variables);
            }
            else
            {
                RecipientVariables = null;
            }
        }

    }
}

