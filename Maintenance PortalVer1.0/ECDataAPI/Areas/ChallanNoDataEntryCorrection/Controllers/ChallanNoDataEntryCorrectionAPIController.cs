using CustomModels.Models.ChallanNoDataEntryCorrection;
using ECDataAPI.Areas.ChallanNoDataEntryCorrection.BAL;
using ECDataAPI.Areas.ChallanNoDataEntryCorrection.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.ChallanNoDataEntryCorrection.Controllers
{
    public class ChallanNoDataEntryCorrectionAPIController : ApiController
    {
        //IChallanNoDataEntryCorrection balObject = null;
        IChallanNoDataEntryCorrection balObject = null;


        [HttpGet]
        [Route("api/ChallanNoDataEntryCorrectionAPIController/ChallanNoDataEntryCorrectionView")]
        public IHttpActionResult ChallanNoDataEntryCorrectionView(int OfficeID)
        {
            try
            {
                balObject = new ChallanNoDataEntryCorrectionBAL();
                ChallanDetailsModel resModel = balObject.ChallanNoDataEntryCorrectionView(OfficeID);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        

        [HttpPost]
        [Route("api/ChallanNoDataEntryCorrectionAPIController/GetChallanReportDetails")]
        [EventApiAuditLogFilter(Description = "Get Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetChallanReportDetails(ChallanDetailsModel model)
        {
            try
            {
                balObject = new ChallanNoDataEntryCorrectionBAL();
                return Ok(balObject.GetChallanReportDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/ChallanNoDataEntryCorrectionAPIController/SaveChallanDetails")]
        public IHttpActionResult SaveChallanDetails(ChallanDetailsModel challanDetailsModel)
        {
            try
            {
                balObject = new ChallanNoDataEntryCorrectionBAL();
                ChallanNoDataEntryCorrectionResponse resModel = new ChallanNoDataEntryCorrectionResponse();
                resModel = balObject.SaveChallanDetails(challanDetailsModel);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/ChallanNoDataEntryCorrectionAPIController/UpdateChallanDetails")]
        public IHttpActionResult UpdateChallanDetails(string ChallanCorrectionID, string InstrumentNumber, string InstrumentDate, int SROCode, int DistrictCode)
        {
            try
            {
                balObject = new ChallanNoDataEntryCorrectionBAL();
                ChallanNoDataEntryCorrectionResponse resModel = new ChallanNoDataEntryCorrectionResponse();
                resModel = balObject.UpdateChallanDetails(ChallanCorrectionID, InstrumentNumber,InstrumentDate,SROCode, DistrictCode);
                return Ok(resModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



    }
}
