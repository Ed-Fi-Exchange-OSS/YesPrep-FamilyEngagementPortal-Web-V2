﻿using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Models.Alert;
using Student1.ParentPortal.Models.Notifications;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Resources.Providers.Configuration;
using Student1.ParentPortal.Resources.Providers.Image;
using Student1.ParentPortal.Resources.Providers.Messaging;
using Student1.ParentPortal.Resources.Providers.Notifications;
using Student1.ParentPortal.Resources.Providers.Translation;
using Student1.ParentPortal.Resources.Providers.Url;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Student1.ParentPortal.Resources.Providers.Alerts
{
    public class UnreadMessagesAlertProvider : IAlertProvider
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ICustomParametersProvider _customParametersProvider;
        private readonly ITypesRepository _typesRepository;
        private readonly IMessagingProvider _messagingProvider;
        private readonly IUrlProvider _urlProvider;
        private readonly IImageProvider _imageProvider;
        private readonly ISMSProvider _smsProvider;
        private readonly IApplicationSettingsProvider _applicationSettingsProvider;
        private readonly IPushNotificationProvider _pushNotificationProvider;
        private readonly ITranslationProvider _translationProvider;
        private readonly ILogRepository _logRepository;

        public UnreadMessagesAlertProvider(IAlertRepository alertRepository, 
                                           ICustomParametersProvider customParametersProvider, 
                                           ITypesRepository typesRepository, 
                                           IMessagingProvider messagingProvider, 
                                           IUrlProvider urlProvider, 
                                           IImageProvider imageProvider, 
                                           ISMSProvider smsProvider, 
                                           IApplicationSettingsProvider applicationSettingsProvider, 
                                           IPushNotificationProvider pushNotificationProvider,
                                           ITranslationProvider translationProvider,
                                           ILogRepository logRepository)
        {
            _alertRepository = alertRepository;
            _customParametersProvider = customParametersProvider;
            _messagingProvider = messagingProvider;
            _urlProvider = urlProvider;
            _imageProvider = imageProvider;
            _smsProvider = smsProvider;
            _typesRepository = typesRepository;
            _applicationSettingsProvider = applicationSettingsProvider;
            _pushNotificationProvider = pushNotificationProvider;
            _translationProvider = translationProvider;
            _logRepository = logRepository;
        }

        public async Task<int> SendAlerts()
        {
            // add if is weekend do nothing

            if(DateTime.Now.DayOfWeek==DayOfWeek.Saturday || DateTime.Now.DayOfWeek==DayOfWeek.Sunday)
                return 0;

            var timeToSend = DateTime.Today.AddHours(Double.Parse(_applicationSettingsProvider.GetSetting("unread.message.alert.hour")));

            var wasSentBefore = await _alertRepository.unreadMessageAlertWasSentBefore();

            if (DateTime.UtcNow < timeToSend.ToUniversalTime() || wasSentBefore)
                return 0;

            var customParameters = _customParametersProvider.GetParameters();

            var alertTypes = await _typesRepository.GetAlertTypes();
            var alertAssignment = alertTypes.Where(x => x.AlertTypeId == AlertTypeEnum.Message.Value).FirstOrDefault();

            var currentSchoolYear = await _alertRepository.getCurrentSchoolYear();

            // Find students that have surpassed the threshold.
            var parentsAndStaffWithUnreadMessages = await _alertRepository.ParentsAndStaffWithUnreadMessages();

            var alertCountSent = 0;

            // Send alerts to the parents of these students.
            foreach (var p in parentsAndStaffWithUnreadMessages)
            {

                if (p.AlertTypeIds == null || !p.AlertTypeIds.Contains(AlertTypeEnum.Message.Value))
                    continue;

                string to;
                string template;
                string subjectTemplate = "Family Portal: Unread Messages Alert";

                if (p.PreferredMethodOfContactTypeId == MethodOfContactTypeEnum.SMS.Value && p.Telephone != null)
                {
                    try
                    {
                        to = p.Telephone;
                        subjectTemplate = await TranslateText(p.LanguageCode, subjectTemplate);
                        template = FillSMSTemplate(p);
                        template = await TranslateText(p.LanguageCode, template);
                        await _smsProvider.SendMessageAsync(to, subjectTemplate, template);
                        alertCountSent++;
                    }
                    catch (Exception ex) {
                        string message = "Error on messagequeue, personuniqueid: " + p.PersonUniqueId + ", email:" + p.Email + ", telephone: " + p.Telephone + ", error: " +ex.Message;
                        _logRepository.AddLogEntry(message, LogTypeEnum.Error.DisplayName);
                    }
                } 
                else if (p.PreferredMethodOfContactTypeId == MethodOfContactTypeEnum.Email.Value &&  p.Email != null)
                {
                    try { 
                        to = p.Email;
                        subjectTemplate = await TranslateText(p.LanguageCode, subjectTemplate);
                        template = FillEmailTemplate(p);
                        template = await TranslateText(p.LanguageCode, template);
                        await _messagingProvider.SendMessageAsync(to, null, null, subjectTemplate, template);
                        alertCountSent++;
                    }
                    catch (Exception ex)
                    {
                        string message = "Error on messagequeue, personuniqueid: " + p.PersonUniqueId + ", email:" + p.Email + ", telephone: " + p.Telephone + ", error: " + ex.Message;
                        _logRepository.AddLogEntry(message, LogTypeEnum.Error.DisplayName);
                    }
                } 
                else if (p.PreferredMethodOfContactTypeId == MethodOfContactTypeEnum.Notification.Value)
                {
                    try { 
                        string lastword = " message.";
                        if (p.UnreadMessageCount > 1)
                            lastword = " messages.";
                        string pushNoSubjectTemplate = $"Unread Messages Alert";
                        string pushNoBodyTemplate = $"You have {p.UnreadMessageCount} unread"+lastword;
                        pushNoSubjectTemplate = await TranslateText(p.LanguageCode, pushNoSubjectTemplate);
                        pushNoBodyTemplate = await TranslateText(p.LanguageCode, pushNoBodyTemplate);


                        await _pushNotificationProvider.SendNotificationAsync(new NotificationItemModel
                        {
                            personUniqueId = p.PersonUniqueId,
                            personType = p.PersonType,
                            notification = new Notification
                            {
                                title = pushNoSubjectTemplate,
                                body = pushNoBodyTemplate
                            }
                        });
                        alertCountSent++;
                    }
                    catch (Exception ex)
                    {
                        string message = "Error on messagequeue, personuniqueid: " + p.PersonUniqueId + ", email:" + p.Email + ", telephone: " + p.Telephone + ", error: " + ex.Message;
                        _logRepository.AddLogEntry(message, LogTypeEnum.Error.DisplayName);
                    }
                }

            }

            if(alertCountSent > 0)
            {
                await _alertRepository.AddAlertLog(currentSchoolYear, AlertTypeEnum.Message.Value, "0", "0", alertCountSent.ToString());
                // Commit all log entries.
                await _alertRepository.SaveChanges();
            }
          

            return alertCountSent;
        }

        private string loadEmailTemplate()
        {
            // Get alert template
            var pathToTemplate = HttpContext.Current.Server.MapPath("~/Templates/Email/UnreadMessageAlertEmailTemplate.html");
            var template = File.ReadAllText(pathToTemplate);

            return template;
        }

        private string loadSmsTemplate()
        {
            // Get alert template
            var pathToTemplate = HttpContext.Current.Server.MapPath("~/Templates/SMS/AlertSMSTemplate.txt");
            var template = File.ReadAllText(pathToTemplate);

            return template;
        }

        private string FillEmailTemplate(UnreadMessageAlertModel u)
        {
            var template = loadEmailTemplate();
            var footer1 = "This email was sent from the Family Portal at Yes Prep Public Schools";
            var footer2 = "Please do not reply to this message, as this email inbox is not monitored";

            if(u.LanguageCode!="en")
            {
                footer1 = TranslateText(u.LanguageCode, footer1).Result;
                footer2 = TranslateText(u.LanguageCode, footer2).Result;
            }

            var filledTemplate = template.Replace("{{AlertMessage}}", $"You have {u.UnreadMessageCount} unread"+LastWord(u.UnreadMessageCount))
                                  .Replace("{{LoginUrl}}", _urlProvider.GetLoginUrl()).Replace("{{Footer1}}", footer1)
                                  .Replace("{{Footer2}}", footer2).Replace("{{LogoUrl}}", _urlProvider.GetLogoUrl());
            return filledTemplate;
        }

        private string FillSMSTemplate(UnreadMessageAlertModel u)
        {
            var template = loadSmsTemplate();
            var filledTemplate = template.Replace("{{AlertContent}}", $"you have {u.UnreadMessageCount} unread" + LastWord(u.UnreadMessageCount) +
                                        $"\r\nFor details visit the Family Portal.")
                                  .Replace("{{StudentFullName}}", $"{u.FirstName} {u.LastSurname}");
            return filledTemplate;
        }

        private string LastWord(int lenght)
        {
            string lastword = " message.";
            if (lenght > 1)
                lastword = " messages.";

            return lastword;
        }

        private async Task<string> TranslateText(string languageCode, string template)
        {
            string textTranslated = string.Empty;
            if (!string.IsNullOrEmpty(languageCode) && languageCode != "en")
            {
                textTranslated = await _translationProvider.TranslateAsync(new TranslateRequest
                {
                    FromLangCode = "en",
                    TextToTranslate = template,
                    ToLangCode = languageCode
                });
            }
            else
                textTranslated = template;
            return textTranslated;
        }
    }
}
