using System;
using mail_marketing_api.Data;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;

namespace mail_marketing_api.Services
{
    public class ContactService : IContactService
    {

        private readonly AppDbContext _appDbContext;

        public ContactService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }

        public List<EmailRecipient> GetAllContacts()
        {
            var lstContact = _appDbContext.EmailRecipients.Include(recipient => recipient.UploadBatch).ToList();
            return lstContact;
        }

        public EmailRecipient GetContactById(int id)
        {
            var contact = _appDbContext
                .EmailRecipients.Include(recipient => recipient.UploadBatch).FirstOrDefault(c => c.RecipientId == id);
            if (contact == null) return null;
            return contact;
        }

        public List<EmailRecipient> SearchByKeyword(string keyword)
        {
            var results = _appDbContext.EmailRecipients
                .Where(r => r.RecipientEmail.Contains(keyword) || r.RecipientName.Contains(keyword))
                .ToList();

            return results;
        }
    }
}

