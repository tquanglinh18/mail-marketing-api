using mail_marketing_api.Models;

public class RecipientDTO
{
    public int RecipientId { get; set; }
    public string RecipientName { get; set; }
    public string RecipientEmail { get; set; }
    public string CustomDataJson { get; set; }
    public int CampaignId { get; set; }
}