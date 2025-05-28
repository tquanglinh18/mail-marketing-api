using mail_marketing_api.Data;
using mail_marketing_api.Models;
using mail_marketing_api.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class TemplateController : ControllerBase
{
    private readonly ITemplateService _templateService;

    public TemplateController(ITemplateService templateService)
    {
        _templateService = templateService;
    }

    [HttpPost]
    public IActionResult CreateTemplate([FromBody] EmailTemplate emailTemplate)
    {
        try
        {
            var result = _templateService.CreateTemplate(emailTemplate);
            return Ok(new ResponseDTO<EmailTemplate>
            {
                Code = 201,
                Data = result,
                Message = "Tạo template thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ResponseDTO<EmailTemplate>
            {
                Code = 500,
                Data = new EmailTemplate(),
                Message = "Lỗi khi tạo template: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTemplatesAsync()
    {
        var templates = await _templateService.GetAllTemplates();
        return Ok(new ResponseDTO<List<EmailTemplate>>
        {
            Code = 200,
            Data = templates,
            Message = "Lấy danh sách template thành công!",
            IsSuccessed = true
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTemplateById(int id)
    {
        var template = await _templateService.GetTeamplateById(id);

        if (template == null)
        {
            return NotFound(new ResponseDTO<EmailTemplate>
            {
                Code = 404,
                Data = new EmailTemplate(),
                Message = $"Không tìm thấy template {id}!",
                IsSuccessed = false
            });
        }

        return Ok(new ResponseDTO<EmailTemplate>
        {
            Code = 200,
            Data = template,
            Message = $"Lấy template {id} thành công!",
            IsSuccessed = true
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        var template = await _templateService.DeleteTemplate(id);

        if (template == null)
        {
            return NotFound(new ResponseDTO<EmailTemplate>
            {
                Code = 404,
                Data = new EmailTemplate(),
                Message = $"Template {id} không tồn tại!",
                IsSuccessed = false
            });
        }

        return Ok(new ResponseDTO<EmailTemplate>
        {
            Code = 200,
            Data = template,
            Message = $"Xoá template {id} thành công!",
            IsSuccessed = true
        });
    }
}
