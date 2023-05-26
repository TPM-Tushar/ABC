using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class BlockingProcessesAPIController : ApiController
    {
        IBlockingProcesses _IBlockingProcesses = new BlockingProcessesBAL();
        [HttpGet]
        [Route("api/BlockingProcessesAPIController/GetBlockingProcessDetails")]
        [EventApiAuditLogFilter(Description = "Get Blocking Process Details")]
        public IHttpActionResult GetBlockingProcessDetails()
        {
            try
            {
                return Ok(_IBlockingProcesses.GetBlockingProcessDetails());
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
