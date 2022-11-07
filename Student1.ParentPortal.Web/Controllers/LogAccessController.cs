using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Student1.ParentPortal.Models.Shared;
using Student1.ParentPortal.Models.User;
using Student1.ParentPortal.Resources.Providers.LoggerAccess;
using Student1.ParentPortal.Resources.Services;
using Student1.ParentPortal.Resources.Services.Parents;
using Student1.ParentPortal.Web.Security;

namespace Student1.ParentPortal.Web.Controllers
{
    [RoutePrefix("api/logaccess")]
    public class LogAccessController : ApiController
    {
        private readonly ILogAccess _logger; 

        public LogAccessController(ILogAccess logger)
        {
            _logger = logger;
        }


        [AllowAnonymous]
        [HttpPost] 
        [Route("logsave")]
        public async Task<IHttpActionResult> LogSave(LogAccessModel model)
        {         
            await _logger.LogSaveAsync(model);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logreportstaffloginsummary")]
        public async Task<IHttpActionResult> LogReportStaffLoginSummary()
        {
            var model = await _logger.GetDistrictStaffLoginSummary();
            if (model == null)
                return NotFound();

            return Ok(model);
        }


        [AllowAnonymous]
        [HttpGet]
        [Route("logreport")]
        public async Task<IHttpActionResult> LogReport()
        {
            var model = await _logger.GetStaffLog();
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logbydistrict")]
        public async Task<IHttpActionResult> LogByDistrict()
        {
            var model = await _logger.GetStaffLogByDistrict();
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logbycampus/{campus:int}")]
        public async Task<IHttpActionResult> LogByCampus(int campus)
        {
            var model = await _logger.GetStaffLogByCampus(campus);
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logreportparent")]
        public async Task<IHttpActionResult> LogReportParent()
        {
            var model = await _logger.GetParentLog();
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logparentbydistrict")]
        public async Task<IHttpActionResult> LogParentByDistrict()
        {
            var model = await _logger.GetParentLogByDistrict();
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logparentbycampus/{campus:int}")]
        public async Task<IHttpActionResult> LogParentByCampus(int campus)
        {
            var model = await _logger.GetParentLogByCampus(campus);
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logReportcampusleader/{id:int}")]
        public async Task<IHttpActionResult> LogReportcampusleader(int id)
        {

            var model = await _logger.GetCampusLeaderLog(id);
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("parentReportcampusleader/{id:int}")]
        public async Task<IHttpActionResult> parentReportcampusleader(int id)
        {

            var model = await _logger.GetCampusLeaderParentLog(id);
            if (model == null)
                return NotFound();

            return Ok(model);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("messagesStaffReportcampusleader/{id:int}")]
        public async Task<IHttpActionResult> MessagesStaffReportcampusleader(int id)
        {

            var model = await _logger.GetStaffMessage(id);
            if (model == null)
                return NotFound();

            return Ok(model);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("messagesParentReportcampusleader/{id:int}")]
        public async Task<IHttpActionResult> MessagesParentReportcampusleader(int id)
        {

            var model = await _logger.GetParentMessageByCampusId(id);
            if (model == null)
                return NotFound();

            return Ok(model);
        }
        [AllowAnonymous]
        [HttpGet]
        [Route("staffmessagereport")]
        public async Task<IHttpActionResult> StaffMessageReport()
        {
            var model = await _logger.GetStaffMessage();
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("staffmessagereportbydistrict")]
        public async Task<IHttpActionResult> StaffMessageReportByDistrict()
        {
            var model = await _logger.GetStaffMessageByDistrict();
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("staffmessagereportbycampus/{campus:int}")]
        public async Task<IHttpActionResult> StaffMessageReportByCampus(int campus)
        {
            var model = await _logger.GetStaffMessageByCampus(campus);
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("parentmessagereport")]
        public async Task<IHttpActionResult> ParentMessageReport()
        {
            var model = await _logger.GetParentMessage();
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("parentmessagereportbydistrict")]
        public async Task<IHttpActionResult> ParentMessageReportByDistrict()
        {
            var model = await _logger.GetParentMessageByDistrict();
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("parentmessagereportbycampus/{campus:int}")]
        public async Task<IHttpActionResult> ParentMessageReportByCampus(int campus)
        {
            var model = await _logger.GetParentMessageByCampus(campus);
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("logReportTeacher/{id:int}")]
        public async Task<IHttpActionResult> LogReportTeacher(int id)
        {

            var model = await _logger.GetTeacherLog(id);
            if (model == null)
                return NotFound();

            return Ok(model);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("messagesReportTeacher/{id:int}")]
        public async Task<IHttpActionResult> MessagesReportTeacher(int id)
        {

            var model = await _logger.GetTeacherMessages(id);
            if (model == null)
                return NotFound();

            return Ok(model);
        }
    }
}
