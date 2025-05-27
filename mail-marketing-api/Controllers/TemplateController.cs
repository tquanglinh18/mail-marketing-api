using mail_marketing_api.Data;
using mail_marketing_api.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class TemplateController : ControllerBase
{
    private readonly AppDbContext _appDbContext;

    public TemplateController(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    [HttpPost]
    public IActionResult CreateTemplate([FromBody] EmailTemplate emailTemplate)
    {
        try
        {
            _appDbContext.EmailTemplates.Add(emailTemplate);
            _appDbContext.SaveChanges();

            return Ok(new ResponseDTO<EmailTemplate>
            {
                Code = 201,
                Data = emailTemplate,
                Message = "Tạo template thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDTO<string>
            {
                Code = 500,
                Data = null,
                Message = "Lỗi khi tạo template: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpGet]
    public IActionResult GetAllTemplates()
    {
        var templates = _appDbContext.EmailTemplates.ToList();

        return Ok(new ResponseDTO<List<EmailTemplate>>
        {
            Code = 200,
            Data = templates,
            Message = "Lấy danh sách template thành công!",
            IsSuccessed = true
        });
    }

    [HttpGet("{id}")]
    public IActionResult GetTemplateById(int id)
    {
        var template = _appDbContext.EmailTemplates.FirstOrDefault(t => t.TemplateId == id);

        if (template == null)
        {
            return NotFound(new ResponseDTO<string>
            {
                Code = 404,
                Data = null,
                Message = "Không tìm thấy template!",
                IsSuccessed = false
            });
        }

        return Ok(new ResponseDTO<EmailTemplate>
        {
            Code = 200,
            Data = template,
            Message = "Lấy template thành công!",
            IsSuccessed = true
        });
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteTemplate(int id)
    {
        var template = _appDbContext.EmailTemplates.FirstOrDefault(t => t.TemplateId == id);

        if (template == null)
        {
            return NotFound(new ResponseDTO<string>
            {
                Code = 404,
                Data = null,
                Message = "Template không tồn tại!",
                IsSuccessed = false
            });
        }

        try
        {
            _appDbContext.EmailTemplates.Remove(template);
            _appDbContext.SaveChanges();

            return Ok(new ResponseDTO<EmailTemplate>
            {
                Code = 200,
                Data = template,
                Message = "Xoá template thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDTO<string>
            {
                Code = 500,
                Data = null,
                Message = "Lỗi khi xoá template: " + ex.Message,
                IsSuccessed = false
            });
        }
    }
}
