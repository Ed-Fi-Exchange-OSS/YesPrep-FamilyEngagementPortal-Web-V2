﻿using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Data.Models.EdFi25;
using Student1.ParentPortal.Models.Alert;
using Student1.ParentPortal.Models.Notifications;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Models.Student;
using Student1.ParentPortal.Resources.Providers.Configuration;
using Student1.ParentPortal.Resources.Providers.Date;
using Student1.ParentPortal.Resources.Providers.Image;
using Student1.ParentPortal.Resources.Providers.Messaging;
using Student1.ParentPortal.Resources.Providers.Notifications;
using Student1.ParentPortal.Resources.Providers.Translation;
using Student1.ParentPortal.Resources.Providers.Url;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Student1.ParentPortal.Resources.Providers.Alerts
{
    public class AdaAbscencesThresholdAlertProvider : IAlertProvider
    {
        private readonly IAlertRepository _alertRepository;
        private readonly ITypesRepository _typesRepository;
        private readonly ICustomParametersProvider _customParametersProvider;
        private readonly IMessagingProvider _messagingProvider;
        private readonly ISMSProvider _smsProvider;
        private readonly IUrlProvider _urlProvider;
        private readonly IImageProvider _imageProvider;
        private readonly IPushNotificationProvider _pushNotificationProvider;
        private readonly ITranslationProvider _translationProvider;
        private readonly IDateProvider _dateProvider;
        private readonly ILogRepository _logRepository;

        public AdaAbscencesThresholdAlertProvider(IAlertRepository alertRepository, 
                                                  ICustomParametersProvider customParametersProvider, 
                                                  ITypesRepository typesRepository, 
                                                  IMessagingProvider messagingProvider, 
                                                  IUrlProvider urlProvider, 
                                                  IImageProvider imageProvider, 
                                                  ISMSProvider smsProvider,
                                                  IPushNotificationProvider pushNotificationProvider,
                                                  ITranslationProvider translationProvider,
                                                  IDateProvider dateProvider, 
                                                  ILogRepository logRepository)
        {
            _alertRepository = alertRepository;
            _customParametersProvider = customParametersProvider;
            _messagingProvider = messagingProvider;
            _urlProvider = urlProvider;
            _imageProvider = imageProvider;
            _smsProvider = smsProvider;
            _typesRepository = typesRepository;
            _pushNotificationProvider = pushNotificationProvider;
            _translationProvider = translationProvider;
            _dateProvider = dateProvider;
            _logRepository = logRepository;
        }

        public async Task<int> SendAlerts()
        {
            var customParameters = _customParametersProvider.GetParameters();
            var enabledFeauter = customParameters.featureToggle.Select(x => x.features.Where(i => i.enabled && i.studentAbc != null)).FirstOrDefault().FirstOrDefault().studentAbc.absence;
            if (!enabledFeauter)
                return 0;

            var alertTypes = await _typesRepository.GetAlertTypes();
            var alertAbsence = alertTypes.Where(x => x.AlertTypeId == AlertTypeEnum.Absence.Value).FirstOrDefault();

            var unexcusedThreshold = alertAbsence.Thresholds.Where(x => x.ThresholdTypeId == ThresholdTypeEnum.UnexcusedAbsence.Value).FirstOrDefault();

            var currentSchoolYear = await _alertRepository.getCurrentSchoolYear();

            // Find students that have surpassed the threshold.
            var studentsOverThreshold = await _alertRepository.studentsOverThresholdAdaAbsence(unexcusedThreshold.ThresholdValue, customParameters.descriptors.absentDescriptorCodeValue, _dateProvider.Today());


            var alertCountSent = 0;

            // Send alerts to the parents of these students.
            foreach (var s in studentsOverThreshold)
            {
                var imageUrl = await _imageProvider.GetStudentImageUrlForAlertsAsync(s.StudentUniqueId);
                // For each parent that wants to receive alerts
                foreach (var p in s.StudentParentAssociations)
                {
                    var parentAlert = p.Parent.ParentAlert;
                    var wasSentBefore = await _alertRepository.wasSentBefore(p.Parent.ParentUniqueId, s.StudentUniqueId, s.ValueCount.ToString(), currentSchoolYear, AlertTypeEnum.Absence.Value);

                    if (parentAlert == null || parentAlert.AlertTypeIds == null || !parentAlert.AlertTypeIds.Contains(AlertTypeEnum.Absence.Value) || wasSentBefore)
                        continue;

                    string to;
                    string template;
                    string subjectTemplate = "Family Portal: Attendance Alert";
                    
                    if (parentAlert.PreferredMethodOfContactTypeId == MethodOfContactTypeEnum.SMS.Value && p.Parent.Telephone != null)
                    {
                        try
                        {
                            to = p.Parent.Telephone;
                            template = FillSMSTemplate(s);
                            template = await TranslateText(p.Parent.LanguageCode, template);
                            subjectTemplate = await TranslateText(p.Parent.LanguageCode, subjectTemplate);
                            await _smsProvider.SendMessageAsync(to, subjectTemplate, template);
                            alertCountSent++;
                        }
                        catch (Exception ex)
                        {
                            string message = "Error on messagequeue, personuniqueid: " + p.Parent.ParentUniqueId + ", email:" + p.Parent.Email + ", telephone: " + p.Parent.Telephone + ", error: " + ex.Message;
                            _logRepository.AddLogEntry(message, LogTypeEnum.Error.DisplayName);
                        }
                    }
                    else if (parentAlert.PreferredMethodOfContactTypeId == MethodOfContactTypeEnum.Email.Value && p.Parent.Email != null)
                    {
                        try
                        {
                            to = p.Parent.Email;
                            template = FillEmailTemplate(s, unexcusedThreshold, imageUrl);
                            template = await TranslateText(p.Parent.LanguageCode, template);
                            subjectTemplate = await TranslateText(p.Parent.LanguageCode, subjectTemplate);
                            await _messagingProvider.SendMessageAsync(to, null, null, subjectTemplate, template);
                            alertCountSent++;
                        }
                        catch (Exception ex)
                        {
                            string message = "Error on messagequeue, personuniqueid: " + p.Parent.ParentUniqueId + ", email:" + p.Parent.Email + ", telephone: " + p.Parent.Telephone + ", error: " + ex.Message;
                            _logRepository.AddLogEntry(message, LogTypeEnum.Error.DisplayName);
                        }
                    } 
                    else if (parentAlert.PreferredMethodOfContactTypeId == MethodOfContactTypeEnum.Notification.Value) //push notifications validation
                    {
                        try
                        {
                            string pushNoSubjectTemplate = $"Absences Alert: {s.FirstName} {s.LastSurname}";
                            string pushNoBodyTemplate = $"Has {s.ValueCount} absences.";
                            pushNoSubjectTemplate = await TranslateText(p.Parent.LanguageCode, pushNoSubjectTemplate);
                            pushNoBodyTemplate = await TranslateText(p.Parent.LanguageCode, pushNoBodyTemplate);

                            await _pushNotificationProvider.SendNotificationAsync(new NotificationItemModel
                            {
                                personUniqueId = p.Parent.ParentUniqueId,
                                personType = "Parent",
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
                            string message = "Error on messagequeue, personuniqueid: " + p.Parent.ParentUniqueId + ", email:" + p.Parent.Email + ", telephone: " + p.Parent.Telephone + ", error: " + ex.Message;
                            _logRepository.AddLogEntry(message, LogTypeEnum.Error.DisplayName);
                        }
                    }

                    // Save in log
                    await _alertRepository.AddAlertLog(currentSchoolYear, AlertTypeEnum.Absence.Value, p.Parent.ParentUniqueId, s.StudentUniqueId, s.ValueCount.ToString());
                }
            }

            // Commit all log entries.
            await _alertRepository.SaveChanges();

            return alertCountSent;
        }

        private string loadEmailTemplate()
        {
            // Get alert template
            var pathToTemplate = HttpContext.Current.Server.MapPath("~/Templates/Email/AlertEmailTemplate.html");
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

        private string FillEmailTemplate(StudentAlertModel s, ThresholdTypeModel unexcusedThreshold, string imageUrl)
        {
            var template = loadEmailTemplate();

            var filledTemplate = template.Replace("{{StudentAlertMessage}}", $"Your student has {s.ValueCount} absences.")
                                  .Replace("{{StudentFullName}}", $"{s.FirstName} {s.LastSurname}")
                                  .Replace("{{StudentLocalId}}", s.StudentUSI.ToString())
                                  .Replace("{{StudentImageUrl}}", imageUrl)
                                  .Replace("{{MetricTitle}}", "Absences at or above threshold")
                                  .Replace("{{MetricValue}}", $"<ul><li style='text-align:left'>Ada Absences: {s.ValueCount}/{unexcusedThreshold.ThresholdValue}</li></ul>")
                                  .Replace("{{StudentDetailUrl}}", _urlProvider.GetStudentDetailUrl(s.StudentUSI))
                                  .Replace("{{WhatCanParentDo}}", unexcusedThreshold.WhatCanParentDo);
            return filledTemplate;
        }

        private string FillSMSTemplate(StudentAlertModel s)
        {
            var template = loadSmsTemplate();
            var filledTemplate = template.Replace("{{AlertContent}}", $"has {s.ValueCount} absences:" +
                                        $"\r\nFor details visit the Parent Portal.")
                                  .Replace("{{StudentFullName}}", $"{s.FirstName} {s.LastSurname}");
            return filledTemplate;
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
