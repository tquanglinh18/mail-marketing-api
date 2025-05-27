using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    [Table("EmailLogs")]
    public class EmailLogs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("LogId")]
        public int LogId { get; set; }

        [Column("RecipientId")]
        public int RecipientId { get; set; }

        [Column("TemplateId")]
        public int TemplateId { get; set; }

        [Column("SentDate")]
        public DateTime SentDate { get; set; }

        [Column("IsSuccess")]
        public bool IsSuccess { get; set; } // BIT trong SQL thường ánh xạ sang bool trong C#

        [Column("ErrorMessage")]
        public string ErrorMessage { get; set; }



        [ForeignKey("RecipientId")]
        public virtual EmailRecipient EmailRecipient { get; set; }

        [ForeignKey("TemplateId")]
        public virtual EmailTemplate EmailTemplate { get; set; }
    }
}

