#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ErrorLogFilesApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.ErrorLogFiles;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class ErrorLogFilesApiController : ApiController
    {
        IErrorLogFiles balObject = null;

        /// <summary>
        /// ErrorLogFilesView
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ErrorLogFilesApiController/ErrorLogFilesView")]
        [EventApiAuditLogFilter(Description = "Error Log Files View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult ErrorLogFilesView()
        {
            try
            {
                balObject = new ErrorLogFilesBAL();
                ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();

                model = balObject.ErrorLogFilesView();

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// LoadFolderNameGrid
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ErrorLogFilesApiController/LoadFolderNameGrid")]
        [EventApiAuditLogFilter(Description = "Load Folder Name Grid", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadFolderNameGrid(ErrorFileRequestModel requestModel)
        {
            try
            {
                balObject = new ErrorLogFilesBAL();
                FileInfoModelWrapper fileInfoModelWrapper = balObject.LoadFolderNameGrid(requestModel);
                return Ok(fileInfoModelWrapper);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DownLoadFile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ErrorLogFilesApiController/DownLoadFile")]
        [EventApiAuditLogFilter(Description = "DownLoad Error Log File", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DownLoadFile(ErrorFileRequestModel requestModel)
        {
            try
            {
                balObject = new ErrorLogFilesBAL();
                FileDownloadModel model = balObject.DownLoadFile(requestModel);
                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DownLoadZippedFile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ErrorLogFilesApiController/DownLoadZippedFile")]
        [EventApiAuditLogFilter(Description = "DownLoad Error Log Zipped File", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DownLoadZippedFile(ErrorFileRequestModel requestModel)
        {
            try
            {
                balObject = new ErrorLogFilesBAL();
                FileDownloadModel model = balObject.DownLoadZippedFile(requestModel);
                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// LoadDriveInfoGrid
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ErrorLogFilesApiController/LoadDriveInfoGrid")]
        [EventApiAuditLogFilter(Description = "Load Drive Info Grid", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadDriveInfoGrid(ErrorFileRequestModel requestModel)
        {
            try
            {
                balObject = new ErrorLogFilesBAL();
                ErrorLogFilesViewModel errorLogFilesViewModel = balObject.LoadDriveInfoGrid(requestModel);
                return Ok(errorLogFilesViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetErrorDirectoryList
        /// </summary>
        /// <param name="ApplicationName"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ErrorLogFilesApiController/GetErrorDirectoryList")]
        [EventApiAuditLogFilter(Description = "Get Error Directory List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetErrorDirectoryList(String ApplicationName)
        {
            try
            {
                balObject = new ErrorLogFilesBAL();
                ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();

                model = balObject.GetErrorDirectoryList(ApplicationName);

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }        
    }
}
