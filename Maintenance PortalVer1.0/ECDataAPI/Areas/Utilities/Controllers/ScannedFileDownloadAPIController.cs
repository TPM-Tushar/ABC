#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ScannedFileDownloadAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -22-10-2019
    * Description       :   API Controller for MIS Reports module.
*/
#endregion
using CustomModels.Models.Utilities.ScannedfileDownload;
using ECDataAPI.Areas.Utilities.BAL;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Utilities.Controllers
{
    public class ScannedFileDownloadAPIController : ApiController
    {
        IScannedFileDownload balObject = null;
        /// <summary>
        /// returns ViewModel to show ViewPage
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        /// 




        [HttpGet]
        [Route("api/ScannedFileDownloadAPIController/ScannedFileDownloadView")]
        [EventApiAuditLogFilter(Description = "Get Sale Deed Rev Collection Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult ScannedFileDownloadView(int OfficeID)
        {
            try
            {
                balObject = new ScannedFileDownloadBAL();
                ScannedFileDownloadView ViewModel = new ScannedFileDownloadView();

                ViewModel = balObject.ScannedFileDownloadView(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpGet]
        [Route("api/ScannedFileDownloadAPIController/LoadScannedFileDownloadLogTable")]
        [EventApiAuditLogFilter(Description = "Get Sale Deed Rev Collection Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadScannedFileDownloadLogTable(long UserID)
        {
            try
            {
                balObject = new ScannedFileDownloadBAL();
                ScannedFileDownloadView ViewModel = new ScannedFileDownloadView();
                ViewModel = balObject.LoadScannedFileDownloadLogTable(UserID);
                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns Result Model which contains Byte Array From ScannedFileDownloadBAL
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ScannedFileDownloadAPIController/GetScannedFileByteArray")]
        [EventApiAuditLogFilter(Description = "Get Scanned File Byte Array", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetScannedFileByteArray(ScannedFileDownloadView ReqModel)
        {
            try
            {
                balObject = new ScannedFileDownloadBAL();
                ScannedFileDownloadResModel ScannedFileResModel = new ScannedFileDownloadResModel();
                ScannedFileResModel = balObject.GetScannedFileByteArray(ReqModel);
                return Ok(ScannedFileResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns Status (whether Scanned File id downloaded)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ScannedFileDownloadAPIController/SaveScannedFileDownloadDetails")]
        [EventApiAuditLogFilter(Description = "Get Status of scanned File(whether it is downloaded or not)", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SaveScannedFileDownloadDetails(ScannedFileDownloadView ReqModel)
        {
            try
            {
                balObject = new ScannedFileDownloadBAL();
                ScannedFileDownloadResModel ScannedFileResModel = new ScannedFileDownloadResModel();
                bool status = balObject.SaveScannedFileDownloadDetails(ReqModel);
                return Ok(status);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}