using mail_marketing_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace mail_marketing_api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class UploadController : ControllerBase
    {
        private readonly IS3Service _s3Service;
        private readonly string _bucketName;

        public UploadController(IS3Service s3Service, IConfiguration configuration)
        {
            _s3Service = s3Service;
            _bucketName = configuration["AWS:BucketName"];
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    Message = "File không tồn tại hoặc rỗng.",
                    Success = false
                });
            }

            var key = $"{Guid.NewGuid()}-{Path.GetFileName(file.FileName)}";

            try
            {
                using var stream = file.OpenReadStream();
                var success = await _s3Service.UploadFileAsync(_bucketName, key, stream, file.ContentType);

                if (success)
                {
                    var fileUrl = $"https://{_bucketName}.s3.amazonaws.com/{key}";

                    return Ok(new
                    {
                        Message = "Tải file lên thành công!",
                        FileUrl = fileUrl,
                        Key = key,
                        Success = true
                    });
                }

                return StatusCode(500, new
                {
                    Message = "Tải file lên thất bại.",
                    Success = false
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    Message = $"Đã xảy ra lỗi trong quá trình upload: {ex.Message}",
                    Success = false
                });
            }
        }
    }
}
