using CustomModels.Models.MISReports.FirmCentralizationReport;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class FirmCentralizationReportApiController : ApiController
    {
        IFirmCentralizationReport balObject = new FirmCentralizationReportBAL();
        [HttpGet]
        [Route("api/FirmCentralizationReportApiController/FirmCentralizationReportView")]
        public IHttpActionResult FirmCentralizationReportView(FirmCentralizationReportViewModel firmCentralizationReportViewModel)
        {
            try
            {
                return Ok(balObject.FirmCentralizationReportView(firmCentralizationReportViewModel));
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost]
        [Route("api/FirmCentralizationReportApiController/GetFirmCentralizationDetails")]
        public IHttpActionResult GetFirmCentralizationDetails(FirmCentralizationReportViewModel firmCentralizationReportViewModel)
        {
            try
            {
                return Ok(balObject.GetFirmCentralizationDetails(firmCentralizationReportViewModel));
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}
