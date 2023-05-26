#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ScannedFileUploadStatusReportAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Common;
using CustomModels.Models.MISReports.ScannedFileUploadStatusReport;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class ScannedFileUploadStatusReportAPIController : ApiController
    {
        IScannedFileUploadStatusReport balObject = null;

        /// <summary>
        /// Returns TodaysTotalDocsRegDetailsTable to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ScannedFileUploadStatusReportAPIController/GetScannedFileUploadStatusDetails")]
        [EventApiAuditLogFilter(Description = "Scanned File Upload Status Report View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetScannedFileUploadStatusDetails(int OfficeID)
        {
            try
            {
                balObject = new ScannedFileUploadStatusReportBAL();
                ScannedFileUploadStatusRptReqModel UploadStatusReqModel = new ScannedFileUploadStatusRptReqModel();
                UploadStatusReqModel = balObject.GetScannedFileUploadStatusDetails(OfficeID);
                return Ok(UploadStatusReqModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ScannedFileUploadStatusReportAPIController/LoadScannedFileUploadStatusTable")]
        [EventApiAuditLogFilter(Description = "Get Scanned File Upload Status Datatable", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadScannedFileUploadStatusTable(ScannedFileUploadStatusRptReqModel model)
        {
            try
            {
                balObject = new ScannedFileUploadStatusReportBAL();
                ScannedFileUploadStatusRptResModel UploadStatusResModel = new ScannedFileUploadStatusRptResModel();
                UploadStatusResModel = balObject.LoadScannedFileUploadStatusTable(model);

                return Ok(UploadStatusResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
