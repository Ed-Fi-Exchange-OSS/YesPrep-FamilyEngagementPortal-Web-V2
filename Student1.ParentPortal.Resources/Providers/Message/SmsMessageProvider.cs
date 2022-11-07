using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Resources.Providers.Messaging;
using Student1.ParentPortal.Resources.Providers.Url;

namespace Student1.ParentPortal.Resources.Providers.Message
{
    public class SmsMessageProvider : IMessageProvider
    {
        private readonly ISMSProvider _smsProvider;
        private readonly IUrlProvider _urlProvider;
        public SmsMessageProvider(ISMSProvider smsProvider, IUrlProvider urlProvider) 
        {
            _smsProvider = smsProvider;
            _urlProvider = urlProvider;
        }
        public int DeliveryMethod => MethodOfContactTypeEnum.SMS.Value;

        public async Task SendMessage(MessageAbstractionModel model)
        {
            if (model.RecipientTelephone != null) 
            {
                //We remove the html properties
                var smsMessage = model.BodyMessage.Replace("<br/>", "\n\n");
                smsMessage += $"\n\n {_urlProvider.GetShortenedHomeUrl()}";

                string to = model.RecipientTelephone;
                await _smsProvider.SendMessageAsync(to, model.Subject, smsMessage);
            }
        }
    }
}
