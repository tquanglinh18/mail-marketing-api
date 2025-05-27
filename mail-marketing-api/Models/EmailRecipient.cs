using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    [Table("EmailRecipients")]
    public class EmailRecipient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("RecipientId")]
        public int RecipientId { get; set; }

        [Column("BatchId")]
        public int BatchId { get; set; }

        [Column("RecipientName")]
        [StringLength(255)]
        public string RecipientName { get; set; }

        [Column("RecipientEmail")]
        [StringLength(255)]
        public string RecipientEmail { get; set; }

        [Column("CustomDataJson")]
        public string CustomDataJson { get; set; }

        // --- Navigation Property (Thuộc tính điều hướng) ---
        // Dùng để liên kết tới bảng UploadBatches thông qua khóa ngoại BatchId
        [ForeignKey("BatchId")]
        public virtual UploadBatch UploadBatch { get; set; }
    }
}

