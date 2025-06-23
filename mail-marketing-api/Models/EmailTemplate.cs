using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    [Table("Templates")]
    public class EmailTemplate
	{
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("TemplateId")]
        public int TemplateId { get; set; }

        [Column("TemplateName")]
        [StringLength(255)]
        public string TemplateName { get; set; }

        [Column("HtmlContent")]
        public string HtmlContent { get; set; }

        [Column("ImageStorageType")]
        [StringLength(50)]
        public string ImageStorageType { get; set; }

        [Column("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [Column("CreatedBy")]
        [StringLength(100)]
        public string CreatedBy { get; set; }
    }
}

