using Amazon.S3;
using Amazon.S3.Model;
using mail_marketing_api.Services;

public class S3Service : IS3Service
{
    private readonly IAmazonS3 _s3Client;

    public S3Service(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<List<S3Object>> ListFilesAsync(string bucketName)
    {
        var allS3Objects = new List<S3Object>();
        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName
            };

            ListObjectsV2Response response;
            do
            {
                response = await _s3Client.ListObjectsV2Async(request);
                allS3Objects.AddRange(response.S3Objects);
                request.ContinuationToken = response.NextContinuationToken;
            } while (response.IsTruncated == true);

            return allS3Objects;
        }
        catch (AmazonS3Exception e)
        {
            Console.WriteLine($"Error listing files from S3 bucket '{bucketName}': {e.Message}");
            // Trả về danh sách rỗng hoặc ném lại lỗi tùy theo yêu cầu xử lý lỗi của bạn
            return new List<S3Object>();
        }
        catch (Exception e) // Bắt cả các lỗi chung khác
        {
            Console.WriteLine($"An unexpected error occurred while listing files from S3 bucket '{bucketName}': {e.Message}");
            return new List<S3Object>();
        }
    }

    public async Task<bool> UploadFileAsync(string bucketName, string key, Stream inputStream, string contentType)
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = inputStream,
                ContentType = contentType,
            };

            var response = await _s3Client.PutObjectAsync(putRequest);
            return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
        }
        catch (AmazonS3Exception e)
        {
            // Xử lý lỗi
            Console.WriteLine($"Error uploading to S3: {e.Message}");
            return false;
        }
    }

}
