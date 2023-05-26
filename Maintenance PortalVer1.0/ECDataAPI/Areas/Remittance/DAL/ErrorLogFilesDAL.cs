#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ErrorLogFilesDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.ErrorLogFiles;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.EcDataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class ErrorLogFilesDAL : IErrorLogFiles
    {
        /// <summary>
        /// ErrorLogFilesView
        /// </summary>
        /// <returns></returns>
        public ErrorLogFilesViewModel ErrorLogFilesView()
        {
            ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();
            model.DriveInfoModelList = new List<DriveInfoModel>();

            ECDataService service = new ECDataService();
            ErrorDetailsResponseModel responseModel = service.GetErrorFileDetails("root");
            if (responseModel.isError)
            {
                model.isError = responseModel.isError;
                model.sErrorMessage = responseModel.sErrorMsg;
                model.DriveInfoModelList = new List<DriveInfoModel>();
                return model;
            }
            else
            {
                model.DriveInfoModelList = responseModel.DriveInfoModelList.Select(x => new DriveInfoModel()
                {
                    DriveName = x.DriveName,
                    FreeSpace = x.FreeSpace,
                    TotalSpace = x.TotalSpace,
                    FileSystem = x.FileSystem,
                    FreeSpacePercentage = x.FreeSpacePercentage,
                    DriveType = x.DriveType
                }).ToList();
            }
            return model;
        }

        /// <summary>
        /// LoadFolderNameGrid
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public FileInfoModelWrapper LoadFolderNameGrid(ErrorFileRequestModel requestModel)
        {
            try
            {
                FileInfoModelWrapper returnModel = new FileInfoModelWrapper();

                if (requestModel.ServiceName.ToLower() == "ecdata" || requestModel.ServiceName.ToLower() == "uploader")
                {
                    ECDATAServiceDAL dalOBJ = new ECDATAServiceDAL();
                    returnModel = dalOBJ.LoadFolderNameGrid(requestModel);
                }
                // Commented By Shubham to be deployed later 0n 13-05-2019
                else if (requestModel.ServiceName.ToLower() == "prereg" || requestModel.ServiceName.ToLower() == "kaverionline")
                {
                    PreRegServiceDAL dalOBJ = new PreRegServiceDAL();
                    returnModel = dalOBJ.LoadFolderNameGrid(requestModel);
                }
                else if (requestModel.ServiceName.ToLower() == "ecdataportal" || requestModel.ServiceName.ToLower() == "kaveriweb")
                {
                    ECDATAPortalDAL dalOBJ = new ECDATAPortalDAL();
                    returnModel = dalOBJ.LoadFolderNameGrid(requestModel);
                }
                //Added by Madhusoodan on 15/12/2021 (To fetch AnywhereEC Logs)
                else if (requestModel.ServiceName.ToLower() == "anywhereecservice")
                {
                    AnyWhereECServiceDAL dalOBJ = new AnyWhereECServiceDAL();
                    returnModel = dalOBJ.LoadFolderNameGrid(requestModel);
                }

                return returnModel;
            }
            catch (Exception) { throw; }
            finally { }
        }

        /// <summary>
        /// DownLoadFile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownLoadFile(ErrorFileRequestModel model)
        {
            FileDownloadModel responseModel = new FileDownloadModel();

            if (model.ServiceName.ToLower() == "ecdata" || model.ServiceName.ToLower() == "uploader")
            {
                ECDATAServiceDAL dalOBJ = new ECDATAServiceDAL();
                responseModel = dalOBJ.DownLoadFile(model);
            }
            // Commented By Shubham to be deployed later 0n 13-05-2019
            else if (model.ServiceName.ToLower() == "prereg" || model.ServiceName.ToLower() == "kaverionline")
            {
                PreRegServiceDAL dalOBJ = new PreRegServiceDAL();
                responseModel = dalOBJ.DownLoadFile(model);
            }
            else if (model.ServiceName.ToLower() == "ecdataportal" || model.ServiceName.ToLower() == "kaveriweb")
            {
                ECDATAPortalDAL dalOBJ = new ECDATAPortalDAL();
                responseModel = dalOBJ.DownLoadFile(model);
            }
            //Added by Madhusoodan on 15/12/2021 (To fetch AnywhereEC Logs)
            else if (model.ServiceName.ToLower() == "anywhereecservice")
            {
                AnyWhereECServiceDAL dalOBJ = new AnyWhereECServiceDAL();
                responseModel = dalOBJ.DownLoadFile(model);
            }
            return responseModel;
        }

        /// <summary>
        /// DownLoadZippedFile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownLoadZippedFile(ErrorFileRequestModel model)
        {
            FileDownloadModel responseModel = new FileDownloadModel();
            if (model.ServiceName.ToLower() == "ecdata" || model.ServiceName.ToLower() == "uploader")
            {
                ECDATAServiceDAL dalOBJ = new ECDATAServiceDAL();
                responseModel = dalOBJ.DownLoadZippedFile(model);
            }
            // Commented By Shubham to be deployed later 0n 13-05-2019
            else if (model.ServiceName.ToLower() == "prereg" || model.ServiceName.ToLower() == "kaverionline")
            {
                PreRegServiceDAL dalOBJ = new PreRegServiceDAL();
                responseModel = dalOBJ.DownLoadZippedFile(model);
            }
            else if (model.ServiceName.ToLower() == "ecdataportal" || model.ServiceName.ToLower() == "kaveriweb")
            {
                ECDATAPortalDAL dalOBJ = new ECDATAPortalDAL();
                responseModel = dalOBJ.DownLoadZippedFile(model);
            }
            //Added by Madhusoodan on 15/12/2021 (To fetch AnywhereEC Logs)
            else if (model.ServiceName.ToLower() == "anywhereecservice")
            {
                AnyWhereECServiceDAL dalOBJ = new AnyWhereECServiceDAL();
                responseModel = dalOBJ.DownLoadZippedFile(model);
            }

            return responseModel;
        }
      
        /// <summary>
        /// LoadDriveInfoGrid
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ErrorLogFilesViewModel LoadDriveInfoGrid(ErrorFileRequestModel requestModel)
        {
            ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();
            if (requestModel.ServiceName.ToLower() == "ecdata" || requestModel.ServiceName.ToLower() == "uploader")
            {
                ECDATAServiceDAL dalOBJ = new ECDATAServiceDAL();
                model = dalOBJ.LoadDriveInfoGrid(requestModel);
            }
            // Commented By Shubham to be deployed later 0n 13-05-2019
            else if (requestModel.ServiceName.ToLower() == "prereg" || requestModel.ServiceName.ToLower() == "kaverionline")
            {
                PreRegServiceDAL dalOBJ = new PreRegServiceDAL();
                model = dalOBJ.LoadDriveInfoGrid(requestModel);
            }
            else if (requestModel.ServiceName.ToLower() == "ecdataportal" || requestModel.ServiceName.ToLower() == "kaveriweb")
            {
                ECDATAPortalDAL dalOBJ = new ECDATAPortalDAL();
                model = dalOBJ.LoadDriveInfoGrid(requestModel);
            }
            //Added by Madhusoodan on 15/12/2021 (To fetch AnywhereEC Logs)
            else if (requestModel.ServiceName.ToLower() == "anywhereecservice")
            {
                AnyWhereECServiceDAL dalOBJ = new AnyWhereECServiceDAL();
                model = dalOBJ.LoadDriveInfoGrid(requestModel);
            }
            return model;
        }
        
        /// <summary>
        /// GetErrorDirectoryList
        /// </summary>
        /// <param name="ApplicationName"></param>
        /// <returns></returns>
        public ErrorLogFilesViewModel GetErrorDirectoryList(String ApplicationName)
        {
            {
                ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();
                if (ApplicationName.ToLower() == "ecdata" || ApplicationName.ToLower() == "uploader")
                {
                    ECDATAServiceDAL dalOBJ = new ECDATAServiceDAL();
                    model = dalOBJ.GetErrorDirectoryList(ApplicationName);
                }
                else if (ApplicationName.ToLower() == "prereg" || ApplicationName.ToLower() == "kaverionline")
                {
                    PreRegServiceDAL dalOBJ = new PreRegServiceDAL();
                    model = dalOBJ.GetErrorDirectoryList(ApplicationName);
                }
                else if (ApplicationName.ToLower() == "ecdataportal" || ApplicationName.ToLower() == "kaveriweb")
                {
                    ECDATAPortalDAL dalOBJ = new ECDATAPortalDAL();
                    model = dalOBJ.GetErrorDirectoryList(ApplicationName);
                }
                //Added by Madhusoodan on 15/12/2021 (To fetch AnywhereEC Logs)
                else if (ApplicationName.ToLower() == "anywhereecservice")
                {
                    AnyWhereECServiceDAL dalOBJ = new AnyWhereECServiceDAL();
                    model = dalOBJ.GetErrorDirectoryList(ApplicationName);
                }

                return model;
            }
        }
    }
}