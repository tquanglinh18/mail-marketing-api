using System;
using mail_marketing_api.Models;

namespace mail_marketing_api.Services
{
	public interface IMailgunService
	{
        Task<ResponseDTO<object>> SendBatchEmailAsync(MailgunRequest request);
    }
}

