using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mail_marketing_api.Models
{
    [Table("UploadBatches")]
    public class UploadBatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("BatchId")]
        public int BatchId { get; set; }

        [Column("BatchName")]
        [StringLength(255)]
        public string BatchName { get; set; }

        [Column("UploadedFileName")]
        [StringLength(255)]
        public string UploadedFileName { get; set; }

        [Column("UploadDate")]
        public DateTime UploadDate { get; set; }

        [Column("UploadedBy")]
        [StringLength(100)]
        public string UploadedBy { get; set; }
    }
}

