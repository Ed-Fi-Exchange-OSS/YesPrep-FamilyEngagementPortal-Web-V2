using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Student1.ParentPortal.Models.Alert;
using Student1.ParentPortal.Resources.Providers.Logger;
using Student1.ParentPortal.Resources.Services.Alerts;
using Student1.ParentPortal.Resources.Services.Communications;
using Student1.ParentPortal.Web.Security;

namespace Student1.ParentPortal.Web.Controllers
{
    
    [RoutePrefix("api/alerts")]
    public class AlertsController : ApiController
    {
        private readonly IAlertService _alertsService;
        private readonly ICommunicationsService _communicationsService;
        private readonly ILogger _logger;

        public AlertsController(IAlertService alertsService, ICommunicationsService communicationsService, ILogger logger)
        {
            _alertsService = alertsService;
            _communicationsService = communicationsService;
            _logger = logger;
        }

        // GET: api/Alerts
        [AllowAnonymous]
        [HttpGet]
        [Route("send")]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                await _logger.LogInformationAsync("starting api/alerts");
                await _alertsService.SendAlerts();
                await _communicationsService.SendGroupMessages();
                await _communicationsService.SendDirectMessageAdvisories();
                await _logger.LogInformationAsync("ended api/alerts");
                return Ok("Alerts sent; Group Messages Sent; Message Notifications(Advisories) sent.");
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync($"Error in api/alerts. Error message:{ex.Message}. Error stack trace:{ex.StackTrace}");
                return Ok($"Yup Something Happened:{ex.Message} - {ex.StackTrace}");
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("TestComms")]
        public async Task<IHttpActionResult> TestComms()
        {
            try
            {
                await _alertsService.SendAlertTest();
                return Ok("Alerts have been triggered successfully. Please check email, sms and push notifications on configured devices.");
            }
            catch (Exception ex)
            {
                return Ok($"Yup Something Happened:{ex.Message} - {ex.StackTrace}");
            }

        }


        [HttpGet]
        [Route("parent")]
        public async Task<IHttpActionResult> GetParentAlerts()
        {
            var person = SecurityPrincipal.Current;
            var role = person.Claims.SingleOrDefault(x => x.Type == "role").Value;

            if (role.Equals("Parent", System.StringComparison.InvariantCultureIgnoreCase))
                return Ok(await _alertsService.GetParentAlertTypes(person.PersonUSI));

            return NotFound(); // A Teacher Shouldnt have access
        }
        
        
        [HttpGet]
        [Route("parent/{uniqueId}")]
        public async Task<IHttpActionResult> ParentHasReadStudentAlerts(string uniqueId)
        {
            var person = SecurityPrincipal.Current;
            var role = person.Claims.SingleOrDefault(x => x.Type == "role").Value;

            if (role.Equals("Parent", System.StringComparison.InvariantCultureIgnoreCase))
            {
                await _alertsService.ParentHasReadStudentAlerts(person.PersonUniqueId, uniqueId);
                return Ok();
            }

            return NotFound(); // A Teacher Shouldnt have access
        }

        [HttpPost]
        [Route("parent")]
        public async Task<IHttpActionResult> SaveParentAlerts(ParentAlertTypeModel model)
        {
            var person = SecurityPrincipal.Current;
            var role = person.Claims.SingleOrDefault(x => x.Type == "role").Value;

            if (role.Equals("Parent", System.StringComparison.InvariantCultureIgnoreCase))
                return Ok(await _alertsService.SaveParentAlertTypes(model, person.PersonUSI));

            return NotFound(); // A Teacher Shouldnt have access
        }

        [HttpGet]
        [Route("parent/unread/{studentUniqueId}")]
        public async Task<IHttpActionResult> GetParentUnreadAlerts(string studentUniqueId) 
        {
            var person = SecurityPrincipal.Current;
            var role = person.Claims.SingleOrDefault(x => x.Type == "role").Value;
            if (role.Equals("Parent", System.StringComparison.InvariantCultureIgnoreCase))
                return Ok(await _alertsService.GetParentStudentUnreadAlerts(person.PersonUniqueId, studentUniqueId));

            return NotFound(); // A Teacher Shouldnt have access
        }
    }
}
