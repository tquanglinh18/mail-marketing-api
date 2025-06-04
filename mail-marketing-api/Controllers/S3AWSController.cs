using mail_marketing_api.Models;
using mail_marketing_api.Services;
using Microsoft.AspNetCore.Mvc;

namespace mail_marketing_api.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class S3AWSController : ControllerBase
    {
        private readonly IS3Service _s3Service;
        private readonly string _bucketName;
        private readonly string _awsRegion;

        public S3AWSController(IS3Service s3Service, IConfiguration configuration)
        {
            _s3Service = s3Service;
            _bucketName = configuration["AWS:BucketName"];
            _awsRegion = configuration["AWS:Region"];
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
                        Message = $"Tải file: {file.FileName} lên thành công!",
                        FileUrl = fileUrl,
                        Key = key,
                        Success = true
                    });
                }

                return StatusCode(500, new
                {
                    Message = $"Tải file {file.FileName} lên thất bại.",
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

        [HttpGet]
        public async Task<IActionResult> ListBucketFiles()
        {
            if (string.IsNullOrEmpty(_bucketName))
            {
                return BadRequest(new ResponseDTO<object> { Code = 400, IsSuccessed = false, Message = "AWS BucketName chưa được cấu hình trong appsettings.json." });
            }
            if (string.IsNullOrEmpty(_awsRegion)) // Kiểm tra region nếu bạn muốn tạo URL
            {
                // Cảnh báo, nhưng vẫn có thể tiếp tục nếu không tạo URL
                Console.WriteLine("Warning: AWS Region is not configured. S3 file URLs may not be generated correctly.");
            }


            try
            {
                var s3Objects = await _s3Service.ListFilesAsync(_bucketName);

                if (s3Objects == null) // Service trả về null nếu có lỗi nghiêm trọng không bắt được
                {
                    return StatusCode(500, new ResponseDTO<object> { Code = 500, IsSuccessed = false, Message = "Không thể lấy danh sách file từ S3 do lỗi không xác định từ service." });
                }

                // Chuyển đổi S3Object thành một DTO đơn giản hơn để trả về (tùy chọn)
                var filesData = s3Objects.Select(s3Obj => new
                {
                    s3Obj.Key, 
                    s3Obj.Size, 
                    s3Obj.LastModified,
                    Url = (string.IsNullOrEmpty(_awsRegion) || string.IsNullOrEmpty(s3Obj.Key))
                          ? "Region not configured or key is empty"
                          : $"https://{_bucketName}.s3.{_awsRegion}.amazonaws.com/{s3Obj.Key.Replace(" ", "+")}"
                }).ToList();

                return Ok(new ResponseDTO<object>
                {
                    Code = 200,
                    IsSuccessed = true,
                    Message = $"Lấy thành công {filesData.Count} file từ bucket '{_bucketName}'.",
                    Data = filesData
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy danh sách file từ S3: {ex}");
                return StatusCode(500, new ResponseDTO<object>
                {
                    Code = 500,
                    IsSuccessed = false,
                    Message = "Đã xảy ra lỗi máy chủ khi cố gắng lấy danh sách file từ S3.",
                    Data = ex.Message
                });
            }
        }
    }
}
