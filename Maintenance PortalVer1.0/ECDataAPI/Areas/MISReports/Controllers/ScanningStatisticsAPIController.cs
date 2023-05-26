using CustomModels.Models.MISReports.ScanningStatistics;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class ScanningStatisticsAPIController : ApiController
    {
        IScanningStatistics balObj = null;
        [HttpGet]
        [Route("api/ScanningStatisticsAPIController/ScanningStatisticsView")]
        // GET: MISReports/ScanningStatisticsAPI
        public IHttpActionResult ScanningStatisticsView(int OfficeID)
        {
            try
            {
                ScanningStatisticsReqModel responseModel = new ScanningStatisticsReqModel();
                balObj = new ScanningStatisticsBAL();
                responseModel = balObj.ScanningStatisticsView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/ScanningStatisticsAPIController/GetScanningStatisticsDetails")]
        public IHttpActionResult GetScanningStatisticsDetails(ScanningStatisticsReqModel scanningStatisticsResModel)
        {
            try
            {
                balObj = new ScanningStatisticsBAL();
                ScanningStatisticsResModel ResultModel = new ScanningStatisticsResModel();
                ResultModel = balObj.GetScanningStatisticsDetails(scanningStatisticsResModel);
                return Ok(ResultModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}