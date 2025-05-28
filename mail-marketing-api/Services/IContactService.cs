using System;
using mail_marketing_api.Models;

namespace mail_marketing_api.Services
{
	public interface IContactService
	{
		List<EmailRecipient> GetAllContacts();
		EmailRecipient GetContactById(int id);
		List<EmailRecipient> SearchByKeyword(string keyword);

    }
}

