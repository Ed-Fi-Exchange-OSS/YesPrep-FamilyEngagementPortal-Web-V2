using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using Student1.ParentPortal.Resources.ExtensionMethods;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Models.Shared;

namespace Student1.ParentPortal.Resources.Providers.Messaging
{
    public class EmailMessagingProvider : IMessagingProvider
    {
        private readonly string _defaultFromAddress;
        private readonly string _defaultFromDisplayName;
        private readonly string _mailServer;
        private readonly string _mailPort;
        private readonly string _mailUser;
        private readonly string _mailPassword;
        private readonly ILogRepository _logRepository;

        public EmailMessagingProvider(ILogRepository logRepository)
        {
            // Initialize with parameters set in the Web.Config
            _defaultFromAddress = ConfigurationManager.AppSettings["messaging.email.defaultFromEmail"];
            _defaultFromDisplayName = ConfigurationManager.AppSettings["messaging.email.defaultFromDisplayName"];
            _mailServer = ConfigurationManager.AppSettings["messaging.email.server"];
            _mailPort = ConfigurationManager.AppSettings["messaging.email.port"];
            _mailUser = ConfigurationManager.AppSettings["messaging.email.user"];
            _mailPassword = ConfigurationManager.AppSettings["messaging.email.pass"];
            _logRepository = logRepository;
        }

        public async Task SendMessageAsync(string to, string[] cc, string[] bcc, string subject, string body)
        {
            await SendMessageAsync(new string[] { to }, cc, bcc, subject, body);
        }

        public async Task SendMessageAsync(string[] to, string[] cc, string[] bcc, string subject, string body)
        {
            var defaultFrom = new MailAddress(_defaultFromAddress,_defaultFromDisplayName);
            await SendEmailAsync(defaultFrom, to, cc, bcc,null, subject, body);
        }

        public async Task SendMessageAsync(string from, string[] to, string[] cc, string[] bcc, string subject, string body) {
            var fromAddress = new MailAddress(from);
            await SendEmailAsync(fromAddress, to, cc, bcc, null, subject, body);
        }

        public async Task SendMessageAsync(string[] to, string[] cc, string[] bcc, string[] replyTo, string subject, string body)
        {
            var defaultFrom = new MailAddress(_defaultFromAddress, _defaultFromDisplayName);
            await SendEmailAsync(defaultFrom, to, cc, bcc, replyTo, subject, body);
        }

        private async Task SendEmailAsync(MailAddress from, string[] to, string[] cc, string[] bcc, string[] replyTo, string subject, string body)
        {
            // Create mail credentials and client
            var credentials = new NetworkCredential(_mailUser, _mailPassword);
            var smtpClient = new SmtpClient(_mailServer, Convert.ToInt32(_mailPort)) { Credentials = credentials };
            var client = new SendGridClient(_mailPassword);
            // Create mail message
            var mailMessage = new MailMessage()
            {
                From = from,
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            List<EmailAddress> tos = new List<EmailAddress>();


            // Allow for multiple To, CC and BCC
            if (!to.IsNullOrEmpty())
                to.ToList().ForEach(x => tos.Add(new EmailAddress(x)));
                //to.ToList().ForEach(x => mailMessage.To.Add(new MailAddress(x)));

            if (!cc.IsNullOrEmpty())
                cc.ToList().ForEach(x => mailMessage.CC.Add(new MailAddress(x)));

            if (!bcc.IsNullOrEmpty())
                bcc.ToList().ForEach(x => mailMessage.Bcc.Add(new MailAddress(x)));

            if (!replyTo.IsNullOrEmpty())
                replyTo.ToList().ForEach(x => mailMessage.ReplyToList.Add(new MailAddress(x)));


            var fromGrid = new EmailAddress(from.Address, from.DisplayName);
            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(fromGrid, tos, subject, body, body);
            try
            {
                var response = await client.SendEmailAsync(msg);
                if (!response.IsSuccessStatusCode) {
                    //string errorMessage = "Error on sending Emails for email: " + String.Join(",", to) + ", error: " + response.StatusCode;
                    //_logRepository.AddLogEntry(errorMessage, LogTypeEnum.Error.DisplayName);
                    throw new Exception();
                }
                else
                {
                    string message = "Email sent to : " + String.Join(",", to) + ", StatusCode: " + response.StatusCode;
                    _logRepository.AddLogEntry(message, LogTypeEnum.Info.DisplayName);
                }
            }
            catch (Exception ex)
            {
                string message = "Error on sending Emails for email: " + String.Join(",", to) + ", error: " + ex.Message;
                _logRepository.AddLogEntry(message, LogTypeEnum.Error.DisplayName);
                throw ex;
            }

            // await smtpClient.SendMailAsync(mailMessage);

        }
    }
}
