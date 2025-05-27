using System;
namespace mail_marketing_api.Services
{
    public interface IS3Service
    {
        Task<bool> UploadFileAsync(string bucketName, string key, Stream inputStream, string contentType);
    }
}

