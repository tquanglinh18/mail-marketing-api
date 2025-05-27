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

    public async Task<bool> UploadFileAsync(string bucketName, string key, Stream inputStream, string contentType)
    {
        try
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key, // Tên file trên S3
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
