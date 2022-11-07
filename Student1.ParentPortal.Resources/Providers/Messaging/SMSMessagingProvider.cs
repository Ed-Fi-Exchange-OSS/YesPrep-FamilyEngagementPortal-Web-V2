using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Resources.ExtensionMethods;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Student1.ParentPortal.Resources.Providers.Logger;

namespace Student1.ParentPortal.Resources.Providers.Messaging
{
    public class SMSMessagingProvider : ISMSProvider
    {
        private readonly string _defaultFromPhone;
        private readonly string _accountKey;
        private readonly string _accountAuthKey;
        private readonly ILogger _logger;

        public SMSMessagingProvider(ILogger logger)
        {
            _defaultFromPhone = ConfigurationManager.AppSettings["messaging.sms.sender"];
            _accountKey = ConfigurationManager.AppSettings["messaging.sms.account"];
            _accountAuthKey = ConfigurationManager.AppSettings["messaging.sms.key"];
            _logger = logger;
        }

        public async Task SendMessageAsync(string from, string to, string subject, string body)
        {
            await SendSMSAsync(from, to, subject, body);
        }

        public async Task SendMessageAsync(string to, string subject, string body)
        {
            await SendSMSAsync(_defaultFromPhone, to, subject, body);
        }

        private async Task SendSMSAsync(string from, string to, string subject, string body)
        {
            try
            {
                // Create sms credentials and client
                TwilioClient.Init(_accountKey, _accountAuthKey);

                var smsResponse = await MessageResource.CreateAsync(
                    body: $"{subject}\n\n{body}",
                    from: new Twilio.Types.PhoneNumber(from),
                    to: new Twilio.Types.PhoneNumber(to)
                );
                await _logger.LogInformationAsync($"SMS sent to: {to}. Response Status:{smsResponse.Status}. Response Error Code:{smsResponse.ErrorCode}. Response Error Message:{smsResponse.ErrorMessage}");
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"An error occured sending a sms to {to}. Error:{ex.Message}:StackTrace:{ex.StackTrace}");
                throw ex;
            }

        }
    }
}
