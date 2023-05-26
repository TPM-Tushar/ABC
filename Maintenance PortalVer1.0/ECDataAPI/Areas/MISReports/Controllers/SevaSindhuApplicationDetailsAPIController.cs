using CustomModels.Models.MISReports.SevaSidhuApplicationDetails;
using CustomModels.Models.Remittance.ChallanDataEntryCorrectionDetails;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Areas.Remittance.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class SevaSindhuApplicationDetailsAPIController :ApiController
    {
        ISevaSindhuApplicationDetails balObj = null;
        [HttpGet]
        [Route("api/SevaSindhuApplicationDetailsAPIController/SevaSindhuApplicationDetailsReportView")]
        public IHttpActionResult SevaSindhuApplicationDetailsReportView(int OfficeID)
        {
            try
            {
                SevaSindhuApplicationDetailsReportModel responseModel = new SevaSindhuApplicationDetailsReportModel();
                balObj = new SevaSindhuApplicationDetailsBAL();
                responseModel = balObj.SevaSindhuApplicationDetailsReportView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/SevaSindhuApplicationDetailsAPIController/GetSevaSindhuApplicationDetails")]


        public IHttpActionResult GetSevaSindhuApplicationDetails(SevaSindhuApplicationDetailsReportModel reportModel)
            {
            try
            {
                balObj = new SevaSindhuApplicationDetailsBAL();
                SevaSindhuApplicationDetailsResultModel sevaSindhuApplicationDetailsResultModel = new SevaSindhuApplicationDetailsResultModel();
                sevaSindhuApplicationDetailsResultModel = balObj.GetSevaSindhuApplicationDetails(reportModel);
                return Ok(sevaSindhuApplicationDetailsResultModel);

            }
            catch (Exception ex)
            {
                throw;
            }

        }
        [HttpPost]
        [Route("api/SevaSindhuApplicationDetailsAPIController/GetSevaSindhuApplicationDetails_For_TA")]

        public IHttpActionResult GetSevaSindhuApplicationDetails_For_TA(SevaSindhuApplicationDetailsReportModel reportModel)
        {
            try
            {
                balObj = new SevaSindhuApplicationDetailsBAL();
                SevaSindhuApplicationDetailsResultModel sevaSindhuApplicationDetailsResultModel = new SevaSindhuApplicationDetailsResultModel();
                sevaSindhuApplicationDetailsResultModel = balObj.GetSevaSindhuApplicationDetails_For_TA(reportModel);
                return Ok(sevaSindhuApplicationDetailsResultModel);

            }
            catch (Exception ex)
            {
                throw;
            }

        }





    }
}