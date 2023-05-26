#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ARegisterAnalysisReportAPIController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for ARegister Analysis Report.

*/
#endregion
using CustomModels.Models.Remittance.ARegisterAnalysisReport;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class ARegisterAnalysisReportAPIController : ApiController
    {
        // GET: Remittance/ARegisterAnalysisReportControllerAPI
        IARegisterAnalysisReport balObj = null;
        [HttpGet]
        [Route("api/ARegisterAnalysisReportAPIController/ARegisterAnalysisReportView")]
        public IHttpActionResult ARegisterAnalysisReportView(int OfficeID)
        {
            try
            {
               ARegisterAnalysisReportModel  responseModel = new ARegisterAnalysisReportModel();
                balObj = new ARegisterAnalysisReportBAL();
                responseModel = balObj.ARegisterAnalysisReportView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ARegisterAnalysisReportAPIController/GetARegisterAnalysisReportDetails")]
        public IHttpActionResult GetARegisterAnalysisReportDetails(ARegisterAnalysisReportModel aRegisterAnalysisReportModel)
        {
            try
            {
                balObj = new ARegisterAnalysisReportBAL();
                ARegisterResultModel aRegisterResultModels = new ARegisterResultModel();
                aRegisterResultModels = balObj.GetARegisterAnalysisReportDetails(aRegisterAnalysisReportModel);
                return Ok(aRegisterResultModels);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/ARegisterAnalysisReportAPIController/GetSynchronizationCheckResult")]
        public IHttpActionResult GetSynchronizationCheckResult(ARegisterAnalysisReportModel aRegisterAnalysisReportModel)
        {
            try
            {
                balObj = new ARegisterAnalysisReportBAL();
                ARegisterSynchcheckResultModel aRegisterSynchcheckResultModel = new ARegisterSynchcheckResultModel();
                aRegisterSynchcheckResultModel = balObj.GetSynchronizationCheckResult(aRegisterAnalysisReportModel);
                return Ok(aRegisterSynchcheckResultModel);
            }
            catch (Exception)
            {

                throw;
            }
        }



    }
}