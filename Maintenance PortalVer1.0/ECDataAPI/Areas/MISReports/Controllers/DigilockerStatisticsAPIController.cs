using CustomModels.Models.MISReports.DigilockerStatistics;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class DigilockerStatisticsAPIController : ApiController
    {
        IDigilockerStatistics balObject = null;

        

        [HttpGet]
        [Route("api/DigilockerStatisticsAPIController/DigilockerStatisticsView")]
        [EventApiAuditLogFilter(Description = "Digilocker Statistics Report View")]
        public IHttpActionResult DigilockerStatisticsView(int OfficeID)
        {
            try
            {
                balObject = new DigilockerStatisticsBAL();
                DigiLockerStatisticsViewModel ViewModel = new DigiLockerStatisticsViewModel();

                ViewModel = balObject.DigilockerStatisticsView(OfficeID);


                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/DigilockerStatisticsAPIController/DigilockerStatisticsReportDetails")]
        [EventApiAuditLogFilter(Description = "Get Digilocker Statistics Report Details")]
        public IHttpActionResult DigilockerStatisticsReportDetails(DigiLockerStatisticsViewModel model)
        {
            try
            {
                balObject = new DigilockerStatisticsBAL();
                DigilockerStatisticsResponseModel ResModel = new DigilockerStatisticsResponseModel();
                ResModel = balObject.DigilockerStatisticsReportDetails(model);

                return Ok(ResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        

    }
}