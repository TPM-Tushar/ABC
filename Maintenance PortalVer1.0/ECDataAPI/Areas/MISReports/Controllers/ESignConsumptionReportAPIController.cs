using CustomModels.Models.MISReports.ESignConsumptionReport;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class ESignConsumptionReportAPIController : ApiController
    {
        IESignConsumptionReport balObject = null;

       
        [HttpGet]
        [Route("api/ESignConsumptionReportAPIController/ESignConsumptionReportView")]
        [EventApiAuditLogFilter(Description = "eSign Consumption Report View")]
        public IHttpActionResult ESignConsumptionReportView()
        {
            try
            {
                balObject = new ESignConsumptionReportBAL();

                ESignConsumptionReportViewModel viewModel = new ESignConsumptionReportViewModel();

                viewModel = balObject.ESignConsumptionReportView();

                return Ok(viewModel);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ESignConsumptionReportAPIController/GetTotalESignConsumedCount")]
        [EventApiAuditLogFilter(Description = "eSign Total Consumption Count")]
        public IHttpActionResult GetTotalESignConsumedCount(ESignConsumptionReportViewModel requestModel)
        {
            try
            {
                balObject = new ESignConsumptionReportBAL();

                ESignTotalConsumptionResModel resultModel = balObject.GetTotalESignConsumedCount(requestModel);

                return Ok(resultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ESignConsumptionReportAPIController/LoadESignDetailsDataTable")]
        [EventApiAuditLogFilter(Description = "eSign Success/Fail Status Details")]
        public IHttpActionResult LoadESignDetailsDataTable(ESignConsumptionReportViewModel requestModel)
        {
            try
            {
                balObject = new ESignConsumptionReportBAL();

                ESignStatusDetailsResModel resultModel = balObject.LoadESignDetailsDataTable(requestModel);

                return Ok(resultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}