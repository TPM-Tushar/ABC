#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DataRestorationReportAPIController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   APIController for MIS Reports module.
	* ECR No			:	431
*/
#endregion
using CustomModels.Models.MISReports.DataRestorationReport;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class DataRestorationReportAPIController : ApiController
    {
        /// <summary>
        /// DataRestorationReport
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns DataRestorationReportViewModel Model</returns>
        [HttpGet]
        [Route("api/DataRestorationReportAPIController/DataRestorationReport")]
        [EventApiAuditLogFilter(Description = "Data Restoration Report")]
        public IHttpActionResult DataRestorationReport(int OfficeID)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().DataRestorationReport(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DataRestorationReportStatus
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/DataRestorationReportStatus")]
        [EventApiAuditLogFilter(Description = "Data Restoration Report Status")]
        public IHttpActionResult DataRestorationReportStatus(DataRestorationReportViewModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().DataRestorationReportStatus(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// InitiateDatabaseRestoration
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/InitiateDatabaseRestoration")]
        [EventApiAuditLogFilter(Description = "Initiate Database Restoration")]
        public IHttpActionResult InitiateDatabaseRestoration(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().InitiateDatabaseRestoration(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GenerateKeyAfterExpiration
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/GenerateKeyAfterExpiration")]
        [EventApiAuditLogFilter(Description = "Generate Key After Expiration")]
        public IHttpActionResult GenerateKeyAfterExpiration(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().GenerateKeyAfterExpiration(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// ApproveScript
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/ApproveScript")]
        [EventApiAuditLogFilter(Description = "Approve Script")]
        public IHttpActionResult ApproveScript(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().ApproveScript(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DataInsertionTable
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/DataInsertionTable")]
        [EventApiAuditLogFilter(Description = "Data Insertion Table")]
        public IHttpActionResult DataInsertionTable(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().DataInsertionTable(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DownloadScriptPathVerify
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/DownloadScriptPathVerify")]
        [EventApiAuditLogFilter(Description = "Download Script Path Verify")]
        public IHttpActionResult DownloadScriptPathVerify(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().DownloadScriptPathVerify(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DownloadScriptForRectification
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/DownloadScriptForRectification")]
        [EventApiAuditLogFilter(Description = "Download Script For Rectification")]
        public IHttpActionResult DownloadScriptForRectification(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().DownloadScriptForRectification(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// SaveUplodedRectifiedScript
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/SaveUplodedRectifiedScript")]
        [EventApiAuditLogFilter(Description = "Save Uploded Rectified Script")]
        public IHttpActionResult SaveUplodedRectifiedScript(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().SaveUplodedRectifiedScript(model));
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region ADDED BY PANKAJ ON 15-07-2020
        /// <summary>
        /// ConfirmDataInsertion
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationReportResModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/ConfirmDataInsertion")]
        [EventApiAuditLogFilter(Description = "Save Confirm Data Insertion")]
        public IHttpActionResult ConfirmDataInsertion(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().ConfirmDataInsertion(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        ////2nd method for partial view
        //[HttpPost]
        //[Route("api/DataRestorationReportAPIController/GetConfirmationButtonMessage")]
        //[EventApiAuditLogFilter(Description = "Get Confirmation Button Message")]
        //public IHttpActionResult GetConfirmationButtonMessage(DataRestorationReportReqModel model)
        //{
        //    try
        //    {
        //        return Ok(new DataRestorationReportBAL().GetConfirmationButtonMessage(model));
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        #endregion

        #region ADDED BY SHUBHAM BHAGAT ON 23-07-2020
        /// <summary>
        /// LoadInitiateMasterTable
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns DataRestorationPartialViewModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/LoadInitiateMasterTable")]
        [EventApiAuditLogFilter(Description = "Load Initiate Master Table")]
        public IHttpActionResult LoadInitiateMasterTable(DataRestorationReportReqModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().LoadInitiateMasterTable(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        /// <summary>
        /// AbortView
        /// </summary>
        /// <param name="INIT_ID"></param>
        /// <returns>returns AbortViewModel Model</returns>
        [HttpGet]
        [Route("api/DataRestorationReportAPIController/AbortView")]
        [EventApiAuditLogFilter(Description = "Abort View")]
        public IHttpActionResult AbortView(String INIT_ID)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().AbortView(INIT_ID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// SaveAbortData
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns AbortViewModel Model</returns>
        [HttpPost]
        [Route("api/DataRestorationReportAPIController/SaveAbortData")]
        [EventApiAuditLogFilter(Description = "Save Abort Data")]
        public IHttpActionResult SaveAbortData(AbortViewModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().SaveAbortData(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

        [HttpPost]
        [Route("api/DataRestorationReportAPIController/DataRestorationReportStatusForScript")]
        [EventApiAuditLogFilter(Description = "Data Restoration Report Status")]
        public IHttpActionResult DataRestorationReportStatusForScript(DataRestorationReportViewModel model)
        {
            try
            {
                return Ok(new DataRestorationReportBAL().DataRestorationReportStatusForScript(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}