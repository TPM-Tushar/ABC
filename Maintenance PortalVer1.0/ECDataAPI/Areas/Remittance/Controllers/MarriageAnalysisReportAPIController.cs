#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   MarriageAnalysisReportAPIController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Marriage Analysis Report.

*/
#endregion
using CustomModels.Models.Remittance.MarriageAnalysisReport;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class MarriageAnalysisReportAPIController : ApiController
    {
        // GET: Remittance/MarriageAnalysisReportAPI
        IMarriageAnalysisReport balObj = null;
        [HttpGet]
        [Route("api/MarriageAnalysisReportAPIController/MarriageAnalysisReportView")]
        public IHttpActionResult MarriageAnalysisReportView(int OfficeID)
        {
            try
            {
                MarriageAnalysisReportModel responseModel = new MarriageAnalysisReportModel();
                balObj = new MarriageAnalysisReportBAL();
                responseModel = balObj.MarriageAnalysisReportModelView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/MarriageAnalysisReportAPIController/GetMarriageAnalysisReportsDetails")]
        public IHttpActionResult GetMarriageAnalysisReportsDetails(MarriageAnalysisReportModel marriageAnalysisReportModel)
        {
            try
            {
                balObj = new MarriageAnalysisReportBAL();
                List<MarriageAnalysisReportTableModel> marriageAnalysisReportTableModelsList = new List<MarriageAnalysisReportTableModel>();
                marriageAnalysisReportTableModelsList = balObj.GetMarriageAnalysisReportsDetails(marriageAnalysisReportModel);
                return Ok(marriageAnalysisReportTableModelsList);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}