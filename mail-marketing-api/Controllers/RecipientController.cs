using System.Text.Json;
using mail_marketing_api.Data;
using mail_marketing_api.Models;
using mail_marketing_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using mail_marketing_api.Extensions;

[ApiController]
[Route("[controller]/[action]")]
public class RecipientController : ControllerBase
{
    private readonly ICampaignService _campaignService;
    private readonly IRecipientService _recipientService;

    public RecipientController(
        ICampaignService CampaignService,
        IRecipientService emailRecipientService)
    {
        _campaignService = CampaignService;
        _recipientService = emailRecipientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var recipients = await _recipientService.GetAll();

            return Ok(new ResponseDTO<List<RecipientDTO>>
            {
                Code = 200,
                Data = recipients.MapToDTO(),
                Message = "Thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<List<Recipient>>
            {
                Code = 500,
                Data = new List<Recipient>(),
                Message = "Lỗi: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var recipient = await _recipientService.GetById(id);

            return Ok(new ResponseDTO<RecipientDTO>
            {
                Code = 200,
                Data = recipient.MapToDTO(),
                Message = "Thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<Recipient>
            {
                Code = 500,
                Data = new Recipient(),
                Message = "Lỗi: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> SearchByKeyword([FromQuery] string keyword)
    {
        try
        {
            var results = await _recipientService.SearchByKeyword(keyword);

            if (results.Any())
            {
                return Ok(new ResponseDTO<List<Recipient>>
                {
                    Code = 200,
                    Data = results,
                    Message = "Tìm kiếm thành công!",
                    IsSuccessed = true
                });
            }

            return Ok(new ResponseDTO<object>
            {
                Code = 200,
                Data = { },
                Message = "Không tìm thấy đối tượng nào!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<List<Recipient>>
            {
                Code = 500,
                Data = new List<Recipient>(),
                Message = "Lỗi khi tìm kiếm: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpPost]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> UploadRecipientsFromExcel(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ResponseDTO<Campaign> { Message = "File không hợp lệ." });
        }
        Campaign? Campaign = null;
        var recipients = new List<Recipient>();
        var errors = new List<string>();
        int processedCount = 0;

        try
        {
            // --- Đọc Excel ---
            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null) { return BadRequest(new ResponseDTO<Campaign> { Message = "Không tìm thấy Sheet nào!" }); }

                int rowCount = worksheet.Dimension.End.Row;
                int colCount = worksheet.Dimension.End.Column;
                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++) { headers.Add(worksheet.Cells[1, col].Value?.ToString()?.Trim() ?? $"column{col}"); }
                int emailCol = headers.IndexOf("Email") + 1;
                int nameCol = headers.IndexOf("Họ và tên") + 1;
                if (nameCol == 0)
                {
                    nameCol = headers.IndexOf("Name") + 1;
                }
                if (emailCol == 0) { return BadRequest(new ResponseDTO<Campaign> { Message = "Không tìm thấy cột Email!" }); }

                for (int row = 2; row <= rowCount; row++)
                {
                    processedCount++;
                    var email = worksheet.Cells[row, emailCol].Value?.ToString()?.Trim();
                    var name = (nameCol > 0) ? worksheet.Cells[row, nameCol].Value?.ToString()?.Trim() : "";

                    if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                    {
                        errors.Add($"Dòng {row}: Email trống hoặc không hợp lệ ('{email}').");
                        continue;
                    }

                    var customData = new Dictionary<string, string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        if (col != emailCol && col != nameCol)
                        {
                            var headerName = headers[col - 1];
                            var cellValue = worksheet.Cells[row, col].Value?.ToString()?.Trim();
                            if (!string.IsNullOrWhiteSpace(cellValue)) { customData[headerName] = cellValue; }
                        }
                    }

                    recipients.Add(new Recipient
                    {
                        CampaignId = Campaign.CampaignId,
                        RecipientEmail = email,
                        RecipientName = name ?? "",
                        CustomDataJson = JsonSerializer.Serialize(customData)
                    });
                }
            }

            if (recipients.Any())
            {
                bool success = await _recipientService.AddRecipientsAsync(recipients);
                if (!success)
                {
                    errors.Add("Lỗi khi lưu danh sách người nhận vào cơ sở dữ liệu.");
                }
            }

            return Ok(new ResponseDTO<object>
            {
                Code = 200,
                IsSuccessed = true,
                Message = $"Xử lý hoàn tất. {recipients.Count}/{processedCount} liên hệ hợp lệ đã được thêm vào. {errors.Count} lỗi.",
                Data = new {
                    //CampaignId = Campaign.CampaignId,
                    Errors = errors }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<Campaign>
            {
                Message = "Lỗi khi xử lý file Excel: " + ex.Message,
                Data = Campaign
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetByCampaignId(int id)
    {
        try
        {
            var recipients = await _recipientService.GetByCampaignIdAsync(id);

            return Ok(new ResponseDTO<List<RecipientDTO>>
            {
                Code = 200,
                Data = recipients.MapToDTO(),
                Message = "Thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<Recipient>
            {
                Code = 500,
                Data = new Recipient(),
                Message = "Lỗi: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

}
