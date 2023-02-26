using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace JakeDrinkStore.Utility
{
    public class EmailSender : IEmailSender
    {
        public string SendGridSecret { get; set; }
        public EmailSender(IConfiguration _config)
        {
            SendGridSecret = _config.GetValue<string>("SendGrid:SecretKey");
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // MimeKit Email Configuration Start
            //var emailToSend = new MimeMessage();
            //emailToSend.From.Add(MailboxAddress.Parse("noreply@jakelin.com.au"));
            //emailToSend.To.Add(MailboxAddress.Parse(email));
            //emailToSend.Subject = subject;
            //emailToSend.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = htmlMessage };

            //// send email
            //using (var emailClient = new SmtpClient())
            //{
            //    emailClient.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            //    emailClient.Authenticate("linweichen08@gmail.com", "umbkhqpolxvdlnlm");
            //    emailClient.Send(emailToSend);
            //    emailClient.Disconnect(true);
            //}

            //return Task.CompletedTask;
            // MimeKit Email Configuration End

            // SendGrid Configuration
            var client = new SendGridClient(SendGridSecret);
            var from = new EmailAddress("linweichen08@gmail.com", "Jake Book Store");
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            return client.SendEmailAsync(msg);
        }
    }
}
