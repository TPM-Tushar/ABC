using CustomModels.Models.Remittance.BlockingProcessesForKOS;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class BlockingProcessesForKOSAPIController : ApiController
    {
        IBlockingProcessesForKOS _IBlockingProcessesForKOS = new BlockingProcessesForKOSBAL();

        [HttpPost]
        [Route("api/BlockingProcessesForKOSAPIController/GetBlockingProcessForKOSDetails")]
        [EventApiAuditLogFilter(Description = "Get Blocking Process for KOS Details")]
        public IHttpActionResult GetBlockingProcessForKOSDetails(BlockingProcessesForKOSReqModel model)
        {
            try
            {
                return Ok(_IBlockingProcessesForKOS.GetBlockingProcessForKOSDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}