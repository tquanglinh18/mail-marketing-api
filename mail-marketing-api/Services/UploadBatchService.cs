using mail_marketing_api.Data;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace mail_marketing_api.Services
{
    public class UploadBatchService : IUploadBatchService
    {
        private readonly AppDbContext _context;

        public UploadBatchService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<UploadBatch> CreateUploadBatchAsync(string batchName, string fileName, string uploadedBy)
        {
            if (string.IsNullOrWhiteSpace(batchName))
                throw new ArgumentNullException(nameof(batchName));
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            var uploadBatch = new UploadBatch
            {
                BatchName = batchName,
                UploadedFileName = fileName,
                UploadDate = DateTime.UtcNow,
                UploadedBy = uploadedBy ?? "System" // Cung cấp giá trị mặc định
            };

            _context.UploadBatches.Add(uploadBatch);
            await _context.SaveChangesAsync();

            return uploadBatch;
        }

        public async Task<List<UploadBatch>> GetAllBatchesSummaryAsync()
        {
            return await _context.UploadBatches
                                 .OrderByDescending(b => b.UploadDate)
                                 .Select(b => new UploadBatch
                                 {
                                     BatchId = b.BatchId,
                                     BatchName = b.BatchName,
                                     UploadedFileName = b.UploadedFileName,
                                     UploadDate = b.UploadDate,
                                     UploadedBy = b.UploadedBy
                                 })
                                 .ToListAsync();
        }
    }
}