using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    [Table("Campaigns")]
    public class Campaign
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("CampaignId")]
        public int CampaignId { get; set; }

        [Column("CampaignName")]
        [StringLength(255)]
        public string CampaignName { get; set; }

        [Column("TemplateId")]
        public int TemplateId { get; set; }

        [Column("Template")]
        public EmailTemplate Template { get; set; }

        [Column("UploadedFileName")]
        [StringLength(255)]
        public string UploadedFileName { get; set; }

        [Column("UploadedFileUrl")]
        public string UploadedFileUrl { get; set; }

        [Column("CreateDate")]
        public DateTime CreateDate { get; set; }

        [Column("CreateBy")]
        [StringLength(100)]
        public string CreateBy { get; set; }

        [Column("StartDate")]
        public DateTime StartDate { get; set; }

        [Column("EndDate")]
        public DateTime EndDate { get; set; }

        [Column("Recipients")]
        public List<Recipient> Recipients { get; set; }
    }
}

