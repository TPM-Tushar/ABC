#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ErrorLogFilesBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.ErrorLogFiles;
using ECDataAPI.Areas.Remittance.DAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.Remittance.BAL
{
    public class ErrorLogFilesBAL : IErrorLogFiles
    {
        IErrorLogFiles dalObject = new ErrorLogFilesDAL();

        /// <summary>
        /// ErrorLogFilesView
        /// </summary>
        /// <returns></returns>
        public ErrorLogFilesViewModel ErrorLogFilesView()
        {
            return dalObject.ErrorLogFilesView();
        }

        //public ErrorLogFilesViewModel GetFileNameList(String FolderName)
        //{
        //    return dalObject.GetFileNameList(FolderName);
        //}

        /// <summary>
        /// LoadFolderNameGrid
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public FileInfoModelWrapper LoadFolderNameGrid(ErrorFileRequestModel requestModel)
        {
            return dalObject.LoadFolderNameGrid(requestModel);
        }

        /// <summary>
        /// DownLoadFile
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public FileDownloadModel DownLoadFile(ErrorFileRequestModel requestModel)
        {
            return dalObject.DownLoadFile(requestModel);
        }

        /// <summary>
        /// DownLoadZippedFile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownLoadZippedFile(ErrorFileRequestModel model)
        {
            return dalObject.DownLoadZippedFile(model);
        }

        /// <summary>
        /// LoadDriveInfoGrid
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ErrorLogFilesViewModel LoadDriveInfoGrid(ErrorFileRequestModel requestModel)
        {
            return dalObject.LoadDriveInfoGrid( requestModel);
        }

        /// <summary>
        /// GetErrorDirectoryList
        /// </summary>
        /// <param name="ApplicationName"></param>
        /// <returns></returns>
        public ErrorLogFilesViewModel GetErrorDirectoryList(String ApplicationName)
        {
            {
                return dalObject.GetErrorDirectoryList(ApplicationName);
            }
        }

    }
}