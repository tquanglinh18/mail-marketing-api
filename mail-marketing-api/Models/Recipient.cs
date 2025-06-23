using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    [Table("Recipients")]
    public class Recipient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("RecipientId")]
        public int RecipientId { get; set; }

        [Column("CampaignId")]
        public int CampaignId { get; set; }

        [Column("RecipientName")]
        [StringLength(255)]
        public string RecipientName { get; set; }

        [Column("RecipientEmail")]
        [StringLength(255)]
        public string RecipientEmail { get; set; }

        [Column("CustomDataJson")]
        public string CustomDataJson { get; set; }

        // --- Navigation Property (Thuộc tính điều hướng) ---
        // Dùng để liên kết tới bảng Campaign thông qua khóa ngoại CampaignId
        [ForeignKey("CampaignId")]
        public virtual Campaign Campaign { get; set; }
    }
}

