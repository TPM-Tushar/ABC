using CustomModels.Models.Remittance.ChallanDetailsReport;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class ChallanDetailsAPIController : ApiController
    {
        IChallanDetails balObject = null;

        [HttpGet]
        [Route("api/ChallanDetailsAPIController/ChallanDetailsReportView")]
        [EventApiAuditLogFilter(Description = "Get ChallanDetailsReportView model", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult ChallanDetailsReportView(int officeid)
        {
            try
            {
                balObject = new ChallanDetailsBAL();
                return Ok(balObject.ChallanDetailsReportView());
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/ChallanDetailsAPIController/GetChallanReportDetails")]
        [EventApiAuditLogFilter(Description = "Get Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetChallanReportDetails(ChallanDetailsModel model)
        {
            try
            {
                balObject = new ChallanDetailsBAL();
                return Ok(balObject.GetChallanReportDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}