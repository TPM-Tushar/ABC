using CustomModels.Models.MISReports.ScanningStatisticsConsolidated;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class ScanningStatisticsConsolidatedAPIController : ApiController
    {
        IScanningStatisticsConsolidated balObj = null;
        [HttpGet]
        [Route("api/ScanningStatisticsConsolidatedAPIController/ScanningStatisticsConsolidatedView")]
        // GET: MISReports/ScanningStatisticsConsolidatedAPI
        public IHttpActionResult ScanningStatisticsConsolidatedView(int OfficeID)
        {
            try
            {
                ScanningStatisticsConsolidatedReqModel responseModel = new ScanningStatisticsConsolidatedReqModel();
                balObj = new ScanningStatisticsConsolidatedBAL();
                responseModel = balObj.ScanningStatisticsConsolidatedView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ScanningStatisticsConsolidatedAPIController/GetScanningStatisticsConsolidatedDetails")]
        public IHttpActionResult GetScanningStatisticsConsolidatedDetails(ScanningStatisticsConsolidatedReqModel scanningStatisticsConsolidatedReqModel)
        {
            try
            {
                balObj = new ScanningStatisticsConsolidatedBAL();
                ScanningStatisticsConsolidatedResModel ResultModel = new ScanningStatisticsConsolidatedResModel();
                ResultModel = balObj.GetScanningStatisticsConsolidatedDetails(scanningStatisticsConsolidatedReqModel);
                return Ok(ResultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}