using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Resources.Providers.Messaging;
using Student1.ParentPortal.Resources.Providers.Translation;
using Student1.ParentPortal.Resources.Providers.Url;

namespace Student1.ParentPortal.Resources.Providers.Message
{
    public class EmailMessageProvider : IMessageProvider
    {
        private readonly IMessagingProvider _messagingProvider;
        private readonly ITranslationProvider _translationProvider;
        private readonly IUrlProvider _urlProvider;
        public EmailMessageProvider(IMessagingProvider messagingProvider, ITranslationProvider translationProvider, IUrlProvider urlProvider)
        {
            _messagingProvider = messagingProvider;
            _translationProvider = translationProvider;
            _urlProvider = urlProvider;
        }
        public int DeliveryMethod => MethodOfContactTypeEnum.Email.Value;

        public async Task SendMessage(MessageAbstractionModel model)
        {
            if (!string.IsNullOrEmpty(model.RecipientEmail))
            {
                model.BodyMessage = await FillEmailTemplate(model.BodyMessage, model.LanguageCode);
                await _messagingProvider.SendMessageAsync(model.RecipientEmail, null, null, model.Subject, model.BodyMessage);
            }
        }

        private async Task<string> TranslateText(string text, string codeLanguage)
        {
            return await _translationProvider.TranslateAsync(new TranslateRequest { TextToTranslate = text, ToLangCode = codeLanguage });
        }
        private async Task<string> FillEmailTemplate(string messageBody, string languageCode)
        {
            var template = loadEmailTemplate();
            var footer1 = "This email was sent from the Family Portal at Yes Prep Public Schools";
            var footer2 = "Please do not reply to this message, as this email inbox is not monitored";

            if (languageCode != "en")
            {
                footer1 = await TranslateText(footer1, languageCode);
                footer2 = await TranslateText(footer2, languageCode);
            }

            var filledTemplate = template.Replace("{{AlertMessage}}", messageBody).Replace("{{LoginUrl}}", _urlProvider.GetLoginUrl())
                                    .Replace("{{Footer1}}", footer1).Replace("{{Footer2}}", footer2).Replace("{{LogoUrl}}", _urlProvider.GetLogoUrl());
            return filledTemplate;
        }
        private string loadEmailTemplate()
        {
            // Get alert template
            var pathToTemplate = HttpContext.Current.Server.MapPath("~/Templates/Email/UnreadMessageAlertEmailTemplate.html");
            var template = File.ReadAllText(pathToTemplate);

            return template;
        }
    }
}
