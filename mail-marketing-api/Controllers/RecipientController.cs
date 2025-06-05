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
    private readonly IUploadBatchService _uploadBatchService;
    private readonly IEmailRecipientService _emailRecipientService;

    public RecipientController(
        IUploadBatchService uploadBatchService,
        IEmailRecipientService emailRecipientService)
    {
        _uploadBatchService = uploadBatchService;
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
    public async Task<IActionResult> GetRecipientById(int id)
    {
        var rcipient = await _emailRecipientService.GetRecipientById(id);

        if (rcipient == null)
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
            Data = rcipient,
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

        UploadBatch? uploadBatch = null;
        var recipients = new List<EmailRecipient>();
        var errors = new List<string>();
        int processedCount = 0;

        try
        {
            uploadBatch = await _uploadBatchService.CreateUploadBatchAsync(
                batchName,
                file.FileName,
                "Admin"
            );

            // --- Đọc Excel =
            using (var stream = file.OpenReadStream())
            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                if (worksheet == null) { return BadRequest(new ResponseDTO<UploadBatch> { Message = "Không tìm thấy Sheet nào!" }); }

                int rowCount = worksheet.Dimension.End.Row;
                int colCount = worksheet.Dimension.End.Column;
                var headers = new List<string>();
                for (int col = 1; col <= colCount; col++) { headers.Add(worksheet.Cells[1, col].Value?.ToString()?.Trim().ToLower() ?? $"column{col}"); }
                int emailCol = headers.IndexOf("email") + 1;
                int nameCol = headers.IndexOf("họ và tên") + 1;
                if (nameCol == 0)
                {
                    nameCol = headers.IndexOf("name") + 1;
                }
                if (emailCol == 0) { return BadRequest(new ResponseDTO<UploadBatch> { Message = "Không tìm thấy cột Email!" }); }

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
                Message = "Lỗi khi xử lý file Excel: " + ex.Message,
                Data = uploadBatch
            });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetUploadBatchSummaries()
    {
        try
        {
            var batches = await _uploadBatchService.GetAllBatchesSummaryAsync();

            if (batches == null || !batches.Any())
            {
                return Ok(new ResponseDTO<List<UploadBatch>>
                {
                    Code = 200,
                    IsSuccessed = true,
                    Message = "Không có lô tải lên nào được tìm thấy.",
                    Data = new List<UploadBatch>()
                });
            }

            return Ok(new ResponseDTO<List<UploadBatch>>
            {
                Code = 200,
                IsSuccessed = true,
                Message = "Lấy danh sách tóm tắt các lô tải lên thành công.",
                Data = batches
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lỗi khi lấy danh sách tóm tắt UploadBatch: {ex.ToString()}");
            return StatusCode(500, new ResponseDTO<List<UploadBatch>>
            {
                Code = 500,
                IsSuccessed = false,
                Message = "Đã xảy ra lỗi máy chủ khi cố gắng lấy danh sách lô tải lên.",
                Data = null
            });
        }
    }
}
