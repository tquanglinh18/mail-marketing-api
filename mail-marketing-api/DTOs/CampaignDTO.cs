
public class CampaignDTO
{
    public int CampaignId { get; set; }
    public string CampaignName { get; set; }
    public int TemplateId { get; set; }
    public string UploadedFileName { get; set; }
    public string UploadedFileUrl { get; set; }
    public DateTime CreateDate { get; set; }
    public string CreateBy { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public EmailTemplateDTO Template { get; set; }
    public List<RecipientDTO> Recipients { get; set; }
}
