using System;
using mail_marketing_api.Models;

namespace mail_marketing_api.Services
{
	public interface IUploadBatchService
	{
        Task<UploadBatch> CreateUploadBatchAsync(string batchName, string fileName, string uploadedBy);
    }
}

