using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Student1.ParentPortal.Web.Controllers
{
    [RoutePrefix("api/students")]
    public class TestController : ApiController
    {
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            return Ok("hello");
        }
    }
}
