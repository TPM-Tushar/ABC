using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using CustomModels.Models.Remittance.ChallanDataEntryCorrectionDetails;
using System.Web.Http;
using System.Web.Http.Results;
using CustomModels.Models.Remittance.MasterData;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class ChallanDataEntryCorrectionDetailsAPIController : ApiController

    {
        IChallanDataEntryCorrectionDetails balObj = null;
        [HttpGet]
        [Route("api/ChallanDataEntryCorrectionDetailsAPIController/ChallanDataEntryCorrectionDetailsView")]

        public IHttpActionResult ChallanDataEntryCorrectionDetailsView()
        {
            try

            {
                balObj = new ChallanDataEntryCorrectionDetailsBAL();
                ChallanDataEntryCorrectionDetailsReportModel challanDataEntryCorrectionDetailsModel = new ChallanDataEntryCorrectionDetailsReportModel();
                challanDataEntryCorrectionDetailsModel = balObj.ChallanDataEntryCorrectionDetailsview();
                return Ok(challanDataEntryCorrectionDetailsModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/ChallanDataEntryCorrectionDetailsAPIController/GetCDECorrectionDetailsData")]

        public IHttpActionResult GetCDECorrectionDetailsData(ChallanDataEntryCorrectionDetailsReportModel masterDataReportModel)
        {
            try
            {
                balObj = new ChallanDataEntryCorrectionDetailsBAL();
                ChallanDataEntryCorrectionDetailsResultModel challanDataEntryCorrectionDetailsResultModel = new ChallanDataEntryCorrectionDetailsResultModel();
                challanDataEntryCorrectionDetailsResultModel = balObj.GetCDECorrectionDetailsData(masterDataReportModel);
                return Ok(challanDataEntryCorrectionDetailsResultModel);

            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}