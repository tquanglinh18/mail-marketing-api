public class EmailTemplateDTO
{
    public int TemplateId { get; set; }
    public string TemplateName { get; set; }
    public string HtmlContent { get; set; }
    public string ImageStorageType { get; set; }
    public DateTime CreatedDate { get; set; }
    public string CreatedBy { get; set; }
}
