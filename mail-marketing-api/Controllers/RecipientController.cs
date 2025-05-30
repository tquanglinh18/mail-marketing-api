using System.Text.Json;
using mail_marketing_api.Data;
using mail_marketing_api.Models;
using mail_marketing_api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

[ApiController]
[Route("[controller]/[action]")]
public class RecipientController : ControllerBase
{
    private readonly IUploadBatchService _updateBatchService;
    private readonly IEmailRecipientService _emailRecipientService;

    public RecipientController(
        IUploadBatchService updateBatchService,
        IEmailRecipientService emailRecipientService)
    {
        _updateBatchService = updateBatchService;
        _emailRecipientService = emailRecipientService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRecipients()
    {
        try
        {
            var recipients = await _emailRecipientService.GetAllRecipients();

            return Ok(new ResponseDTO<List<EmailRecipient>>
            {
                Code = 200,
                Data = recipients,
                Message = "Thành công!",
                IsSuccessed = true
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<List<EmailRecipient>>
            {
                Code = 500,
                Data = new List<EmailRecipient>(),
                Message = "Lỗi: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContactById(int id)
    {
        var contact = await _emailRecipientService.GetRecipientById(id);

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

    [HttpGet]
    public async Task<IActionResult> SearchByKeyword([FromQuery] string keyword)
    {
        try
        {
            var results = await _emailRecipientService.SearchByKeyword(keyword);

            if (results.Any())
            {
                return Ok(new ResponseDTO<List<EmailRecipient>>
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
            return StatusCode(500, new ResponseDTO<List<EmailRecipient>>
            {
                Code = 500,
                Data = new List<EmailRecipient>(),
                Message = "Lỗi khi tìm kiếm: " + ex.Message,
                IsSuccessed = false
            });
        }
    }

    [HttpPost]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> UploadRecipientsFromExcel(IFormFile file, [FromForm] string batchName)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new ResponseDTO<UploadBatch> { Message = "File không hợp lệ." });
        }
        if (string.IsNullOrWhiteSpace(batchName))
        {
            return BadRequest(new ResponseDTO<UploadBatch> { Message = "Tên lô không được để trống." });
        }

        UploadBatch? uploadBatch = null; // Khởi tạo null
        var recipients = new List<EmailRecipient>();
        var errors = new List<string>();
        int processedCount = 0;

        try
        {
            // << Sử dụng IUploadBatchService >>
            uploadBatch = await _updateBatchService.CreateUploadBatchAsync(
                batchName,
                file.FileName,
                "CurrentUser"
            );

            // --- Đọc Excel (Giữ nguyên logic đọc) ---
            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null) { /* ... Xử lý lỗi ... */ return BadRequest(/*...*/); }

                int rowCount = worksheet.Dimension.End.Row;
                int colCount = worksheet.Dimension.End.Column;
                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++) { headers.Add(worksheet.Cells[1, col].Value?.ToString()?.Trim().ToLower() ?? $"column{col}"); }
                int emailCol = headers.IndexOf("email") + 1;
                int nameCol = headers.IndexOf("name") + 1;
                if (emailCol == 0) { /* ... Xử lý lỗi ... */ return BadRequest(/*...*/); }

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

                    recipients.Add(new EmailRecipient
                    {
                        BatchId = uploadBatch.BatchId,
                        RecipientEmail = email,
                        RecipientName = name ?? "",
                        CustomDataJson = JsonSerializer.Serialize(customData)
                    });
                }
            }

            if (recipients.Any())
            {
                bool success = await _emailRecipientService.AddEmailRecipientsAsync(recipients);
                if (!success)
                {
                    errors.Add("Lỗi khi lưu danh sách người nhận vào cơ sở dữ liệu.");
                }
            }

            return Ok(new ResponseDTO<object>
            {
                Code = 200,
                IsSuccessed = true,
                Message = $"Xử lý hoàn tất. {recipients.Count}/{processedCount} liên hệ hợp lệ đã được thêm vào lô '{batchName}'. {errors.Count} lỗi.",
                Data = new { BatchId = uploadBatch.BatchId, Errors = errors }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new ResponseDTO<UploadBatch>
            {
                Message = "Lỗi nghiêm trọng khi xử lý file Excel: " + ex.Message,
                Data = uploadBatch
            });
        }
    }
}
