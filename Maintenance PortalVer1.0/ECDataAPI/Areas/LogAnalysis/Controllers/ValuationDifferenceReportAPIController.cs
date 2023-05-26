using CustomModels.Models.LogAnalysis.ValuationDifferenceReport;
using ECDataAPI.Areas.LogAnalysis.BAL;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.LogAnalysis.Controllers
{
    public class ValuationDifferenceReportAPIController : ApiController
    {
        [HttpGet]
        [Route("api/ValuationDifferenceReportAPIController/ValuationDifferenceReportView")]
        [EventApiAuditLogFilter(Description = "Valuation Difference View")]
        public IHttpActionResult ValuationDifferenceReportView(int OfficeID)
        {
            try
            {
                return Ok(new ValuationDifferenceReportBAL().ValuationDifferenceReportView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ValuationDifferenceReportAPIController/GetValuationDiffRptData")]
        [EventApiAuditLogFilter(Description = "Get Valuation Diff Rpt Data")]
        public IHttpActionResult GetValuationDiffRptData(ValuationDiffReportViewModel model)
        {
            try
            {
                return Ok(new ValuationDifferenceReportBAL().GetValuationDiffRptData(model));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/ValuationDifferenceReportAPIController/GetValuationDiffDetailedData")]
        [EventApiAuditLogFilter(Description = "Get Valuation Diff Detailed Data")]
        public IHttpActionResult GetValuationDiffDetailedData(ValuationDiffReportViewModel model)
        {
            try
            {
                return Ok(new ValuationDifferenceReportBAL().GetValuationDiffDetailedData(model));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/ValuationDifferenceReportAPIController/GetValuationDocument")]
        [EventApiAuditLogFilter(Description = "Get Valuation Document")]
        public IHttpActionResult GetValuationDocument(ValuationDiffFileModel reqModel)
        {
            try
            {
                return Ok(new ValuationDifferenceReportBAL().GetValuationDocument(  reqModel));
            }
            catch (Exception)
            {
                throw;
            }
        } 



    }
}
