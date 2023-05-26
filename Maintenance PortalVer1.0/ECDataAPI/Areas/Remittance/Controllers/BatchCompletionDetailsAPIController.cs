using CustomModels.Models.Remittance.BatchCompletionDetails;
using CustomModels.Models.Remittance.MasterData;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    //added by vijay on 16/02/2023


    public class BatchCompletionDetailsAPIController:ApiController 
    {
        IBatchCompletionDetails balObj = null;
        [HttpGet]
        [Route("api/BatchCompletionDetailsAPIController/BatchCompletionDetailsView")]

        public IHttpActionResult BatchCompletionDetailsView()
        {
            try

            {
                balObj = new BatchCompletionDetailsBAL();
                BatchCompletionDetailsReportModel batchCompletionDetailsReportModel = new BatchCompletionDetailsReportModel();
                batchCompletionDetailsReportModel = balObj.BatchCompletionDetailsView();
                return Ok(batchCompletionDetailsReportModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/BatchCompletionDetailsAPIController/GetBatchCompletionDetails")]

        public IHttpActionResult GetBatchCompletionDetails(BatchCompletionDetailsReportModel batchCompletionDetailsReportModel)
        {
            try
            {
                balObj = new BatchCompletionDetailsBAL();
                BatchCompletionDetailsResultModel ResultModel = new BatchCompletionDetailsResultModel();
                ResultModel = balObj.GetBatchCompletionDetails(batchCompletionDetailsReportModel);
                return Ok(ResultModel);

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}