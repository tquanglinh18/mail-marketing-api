using System;
using mail_marketing_api.Models;
using Microsoft.EntityFrameworkCore;

namespace mail_marketing_api.Data
{
    public class AppDbContext : DbContext
    {
        // <= ĐÃ THAY ĐỔI TÊN CONSTRUCTOR
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<EmailLogs> EmailLogs { get; set; }
        public DbSet<Recipient> Recipients { get; set; }
        public DbSet<EmailTemplate> Templates { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EmailLogs>()
                  .HasOne(e => e.EmailRecipient)
                  .WithMany()
                  .HasForeignKey(e => e.RecipientId)
                  .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EmailLogs>()
                .HasOne(e => e.EmailTemplate)
                .WithMany()
                .HasForeignKey(e => e.TemplateId)
                .OnDelete(DeleteBehavior.Restrict); 
        }
    }
}

