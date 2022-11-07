﻿using Student1.ParentPortal.Data.Models;
using Student1.ParentPortal.Data.Models.EdFi25;
using Student1.ParentPortal.Models.Alert;
using Student1.ParentPortal.Resources.Providers.Alerts;
using Student1.ParentPortal.Resources.Providers.Configuration;
using Student1.ParentPortal.Resources.Providers.Messaging;
using Student1.ParentPortal.Resources.ExtensionMethods;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using Student1.ParentPortal.Models.Notifications;

namespace Student1.ParentPortal.Resources.Services.Alerts
{
    public class AlertService : IAlertService
    {
        private readonly ICollection<IAlertProvider> _alertProviders;

        private readonly IAlertRepository _alertRepository;

        private readonly ICustomParametersProvider _customParametersProvider;
        private readonly IMessagingProvider _messagingProvider;
        private readonly ISMSProvider _smsMessagingProvider;
        private readonly INotificationsRepository _notificationsRepository;
        

        public AlertService(ICollection<IAlertProvider> alertProviders, IAlertRepository alertRepository, ICustomParametersProvider customParametersProvider, IMessagingProvider messagingProvider, INotificationsRepository notificationsRepository, ISMSProvider smsMessagingProvider)
        {
            _alertProviders = alertProviders;
            _alertRepository = alertRepository;
            _customParametersProvider = customParametersProvider;
            _messagingProvider = messagingProvider;
            _notificationsRepository = notificationsRepository;
            _smsMessagingProvider = smsMessagingProvider;
        }

        public async Task<ParentAlertTypeModel> GetParentAlertTypes(int usi)
        {
            var customParams = _customParametersProvider.GetParameters();
            var parentTypeAlerts = await _alertRepository.GetParentAlertTypes(usi);
            
            return featureToggleFilter(customParams, parentTypeAlerts);

        }

        public async Task<ParentAlertTypeModel> SaveParentAlertTypes(ParentAlertTypeModel parentAlertTypes, int usi)
        {
            return await _alertRepository.SaveParentAlertTypes(parentAlertTypes, usi);
        }

        public async Task<List<ParentStudentAlertLogModel>> GetParentStudentUnreadAlerts(string parentUniqueId, string studentUniqueId)
        {
            return await _alertRepository.GetParentStudentUnreadAlerts(parentUniqueId, studentUniqueId);
        }

        public async Task ParentHasReadStudentAlerts(string parentUniqueId, string studentUniqueId)
        {
            await _alertRepository.ParentHasReadStudentAlerts(parentUniqueId, studentUniqueId);
        }
        
        public async Task SendAlertTest()
        {
            
           //For email
            var toEmails = ConfigurationManager.AppSettings["test.comms.emails"].Split(';');
            var emailMessage = ConfigurationManager.AppSettings["test.comms.emails.message"].ToString();
            await _messagingProvider.SendMessageAsync("dontreply@student1.org", toEmails, null, null, "Testing Email Services", emailMessage);

            //For SMS
            var toSMS = ConfigurationManager.AppSettings["test.comms.sms"].Split(';'); //"+15127854159";
            var smsMessage = ConfigurationManager.AppSettings["test.comms.sms.message"].ToString();

            //SMSMessagingProvider testSMS = new SMSMessagingProvider();
            var testSMS = _smsMessagingProvider;


            foreach (string to in toSMS) {
                await testSMS.SendMessageAsync(to, "SMS Test Service", smsMessage);
            }
            

            //For push

            string email = ConfigurationManager.AppSettings["test.comms.push"];
            var pushMessage = ConfigurationManager.AppSettings["test.comms.push.message"].ToString();

            string[] tokens=await _notificationsRepository.GetTokenByEmail(email);
            NotificationItemModel model = new NotificationItemModel
            {
                registration_ids = tokens,
                personUniqueId = "11111111",
                personType = "Parent",
                notification = new Notification
                {
                    title = "Testing Push Services",
                    body = pushMessage
                }
            };

            var serverKey = $"Key={ConfigurationManager.AppSettings["notifications.Key"]}";
            var jsonData = JsonConvert.SerializeObject(model);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, ConfigurationManager.AppSettings["notifications.ApiUrl"]);
            httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
            httpRequest.Content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var httpClient = new HttpClient();
            var result = await httpClient.SendAsync(httpRequest);
           

        }

        public async Task SetParentUnreadAlert()
        {
            await _alertRepository.SetParentUnreadAlert();
        }
        public async Task SendAlerts()
        {
            // Logic followed is:
            // 1) Get alerts that have to be sent right now. (Do not duplicate alerts)
            // 2) Send alerts
            // 3) Log alerts sent so that we don't send them again

            // Types of Alerts:
            // -- Threshold alerts  (i.e. If my student goes over a threshold. Attendance > 5)
            // -- New entry (new entry for a discipline incident. New entry STAAR EoC Math score just posted. New Absences. New GPA posting. New Grade book entry: Math score your student got an A congrats!)
            // -- Scheduled alerts (I.e. Every Friday send me my student's missing assignments)
            // -- Global/Institutional alerts (alert all parents on something. i.e. School closed because weather alert)
            // -- Others...?

            //Note: For now only threshold is implemented
            // The main alerts implemented are:
            //   - Attendance absence count greater or equal to threshold
            //   - Behavior count greater or equal to threshold
            //   - Missing assignment count > 1
            //   - Assignment grade below threshold
            //   - Course grade below threshold

            //await SendAttendanceAlerts();
            foreach (var alertProvider in _alertProviders)
            {
                try
                {
                    await alertProvider.SendAlerts();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }


        private ParentAlertTypeModel featureToggleFilter(CustomParameters customParameters, ParentAlertTypeModel model) 
        {
            List<AlertTypeModel> modelAlerts = new List<AlertTypeModel>();
            modelAlerts = model.Alerts.ToList();
            model.Alerts = new List<AlertTypeModel>();
            
            foreach (var alert in modelAlerts)
            {
                if (alert.AlertTypeId == 1 &&
                   customParameters.featureToggle.Select(x => x.features.Where(i => i.enabled && i.studentAbc != null)).FirstOrDefault().FirstOrDefault().studentAbc.absence)
                {
                    model.Alerts.Add(modelAlerts.FirstOrDefault(x => x.AlertTypeId == 1));
                }

                if (alert.AlertTypeId == 2 &&
                    customParameters.featureToggle.Select(x => x.features.Where(i => i.enabled && i.studentAbc != null)).FirstOrDefault().FirstOrDefault().studentAbc.behavior)
                {
                    model.Alerts.Add(modelAlerts.FirstOrDefault(x => x.AlertTypeId == 2));
                }

                if (alert.AlertTypeId == 3 &&
                    customParameters.featureToggle.Select(x => x.features.Where(i => i.enabled && i.studentAbc != null)).FirstOrDefault().FirstOrDefault().studentAbc.missingAssignments)
                {
                    model.Alerts.Add(modelAlerts.FirstOrDefault(x => x.AlertTypeId == 3));
                }

                if (alert.AlertTypeId == 4 &&
                    customParameters.featureToggle.Select(x => x.features.Where(i => i.enabled && i.studentAbc != null)).FirstOrDefault().FirstOrDefault().studentAbc.courseAverage) 
                {
                    model.Alerts.Add(modelAlerts.FirstOrDefault(x => x.AlertTypeId == 4));
                }

                if(alert.AlertTypeId == 5)
                    model.Alerts.Add(modelAlerts.FirstOrDefault(x => x.AlertTypeId == 5));
            }
            return model;
        }
    }
}
