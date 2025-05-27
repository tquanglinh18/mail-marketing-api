using mail_marketing_api.Data;
using mail_marketing_api.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]/[action]")]
public class ContactController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ContactController(AppDbContext appDbContext)
    {
        _dbContext = appDbContext;
    }

    [HttpGet]
    public IActionResult GetAllContacts()
    {
        try
        {
            var contacts = _dbContext.EmailRecipients.ToList();

            return Ok(new ResponseDTO<List<EmailRecipient>>
            {
                Code = 200,
                Data = contacts,
                Message = "Lấy danh sách liên hệ thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<List<EmailRecipient>>
            {
                Code = 500,
                Data = new List<EmailRecipient>(),
                Message = "Lỗi server: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpGet("{id}")]
    public IActionResult GetContactById(int id)
    {
        try
        {
            var contact = _dbContext.EmailRecipients.FirstOrDefault(r => r.RecipientId == id);

            if (contact == null)
            {
                return NotFound(new ResponseDTO<EmailRecipient>
                {
                    Code = 404,
                    Data = null,
                    Message = "Không tìm thấy liên hệ!",
                    IsSuccessed = false
                });
            }

            return Ok(new ResponseDTO<EmailRecipient>
            {
                Code = 200,
                Data = contact,
                Message = "Lấy liên hệ thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<EmailRecipient>
            {
                Code = 500,
                Data = null,
                Message = "Lỗi server: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpGet("search")]
    public IActionResult SearchContact([FromQuery] string keyword)
    {
        try
        {
            var results = _dbContext.EmailRecipients
                .Where(r => r.RecipientEmail.Contains(keyword) || r.RecipientName.Contains(keyword))
                .ToList();

            return Ok(new ResponseDTO<List<EmailRecipient>>
            {
                Code = 200,
                Data = results,
                Message = "Tìm kiếm thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<List<EmailRecipient>>
            {
                Code = 500,
                Data = new List<EmailRecipient>(),
                Message = "Lỗi khi tìm kiếm: " + ex.Message,
                IsSuccessed = false
            });
        }
    }
}
