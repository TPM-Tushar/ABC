using CustomModels.Models.MISReports.DiskUtilization;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class DiskUtilizationAPIController : ApiController
    {
        [HttpGet]
        //DocumentScanAndDeliveryAPIController/DocumentScanAndDeliveryView
        [Route("api/DiskUtilizationAPIController/DiskUtilizationView")]
        [EventApiAuditLogFilter(Description = "Disk Utilization View")]
        public IHttpActionResult DiskUtilizationView(int OfficeID)
        {
            try
            {
                return Ok(new DiskUtilizationBAL().DiskUtilizationView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DiskUtilizationAPIController/DiskUtilizationDetails")]
        [EventApiAuditLogFilter(Description = "Disk Utilization Details")]
        public IHttpActionResult DiskUtilizationDetails(DiskUtilizationREQModel model)
        {
            try
            {
                return Ok(new DiskUtilizationBAL().DiskUtilizationDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }


    }
}
