#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   AnyWhereECServiceDAL.cs
    * Author Name       :   Madhusoodan Bisen
    * Creation Date     :   15/12/2021
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for accessing error logs of AnywhereECService using AnywhereWCApplicationServies
*/
#endregion

using CustomModels.Models.Remittance.ErrorLogFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECDataAPI.AnyWhereECApplicationServices;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class AnyWhereECServiceDAL
    {

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
                returnModel.FolderNameList = new List<FileInfoModel>();
                returnModel.FileNameList = new List<FileInfoModel>();

                AnyWhereECApplicationServices.AnyWhereECApplicationServices service = new AnyWhereECApplicationServices.AnyWhereECApplicationServices();  //Add AnyWhereECApplicationServices

                AnyWhereECApplicationServices.ErrorDetailsResponseModel responseModel = null;

                // FOR GETING ALL FILES AND FOLDER IN ROOT DIRECTORY 
                if (String.IsNullOrEmpty(requestModel.Path))
                {
                    responseModel = service.GetErrorFileDetails("root");
                }
                else
                {
                    responseModel = service.GetErrorFileDetails(requestModel.Path);
                }

                // For Error
                if (responseModel.isError)
                {
                    returnModel.isError = responseModel.isError;
                    returnModel.sErrorMessage = responseModel.sErrorMsg;
                }
                // For Folder and File List
                else
                {
                    // For Folder List
                    if (requestModel.GetFolderList)
                    {
                        var folderNameList = responseModel.DirectoryNameArray.ToList();
                        foreach (var item in folderNameList)
                        {
                            if (string.IsNullOrEmpty(returnModel.PresentDirectory))
                                returnModel.PresentDirectory = item.Substring(0, item.LastIndexOf("\\"));

                            FileInfoModel fileInfoModel = new FileInfoModel();
                            fileInfoModel.FileName = item.Substring(item.LastIndexOf("\\") + 1);
                            //string str = item.Replace("\\", @"\\/");
                            //string str = item.Replace("\\", @"\/");
                            string str = item.Replace("\\", "*");
                            // fileInfoModel.ActionBtn = "<button type ='button' class='btn btn-group-md btn-primary' onclick=OpenFolder('" + (item) + "')>Open</button>";
                            fileInfoModel.ActionBtn = "<button type ='button' class='btn btn-group-md btn-primary' onclick=OpenFolder('" + (str) + "')>Open</button>";
                            returnModel.FolderNameList.Add(fileInfoModel);
                        }
                    }

                    // For File List
                    if (requestModel.GetFileList)
                    {
                        var fileNameList = responseModel.FileNameArray.ToList();
                        foreach (var item in fileNameList)
                        {
                            if (string.IsNullOrEmpty(returnModel.PresentDirectory))
                                returnModel.PresentDirectory = item.Substring(0, item.LastIndexOf("\\"));

                            FileInfoModel fileInfoModel = new FileInfoModel();
                            fileInfoModel.FileName = item.Substring(item.LastIndexOf("\\") + 1);
                            string str = item.Replace("\\", @"\/");

                            // ADDED BY SHUBHAM BHAGAT ON 21-05-2019 to replace space with "|"
                            str = str.Replace(" ", "|");

                            fileInfoModel.ActionBtn = "<button type ='button' class='btn btn-group-md btn-primary' onclick=DownLoadFile('" + (str) + "')>Download</button>";
                            //fileInfoModel.ActionBtn = "<button type ='button' class='btn btn-group-md btn-primary' onclick=DownLoadFile('" + (item) + "')>Download</button>";
                            returnModel.FileNameList.Add(fileInfoModel);
                        }
                    }
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
            try
            {
                AnyWhereECApplicationServices.AnyWhereECApplicationServices service = new AnyWhereECApplicationServices.AnyWhereECApplicationServices();
                FileContentRequestModel requestModel = new FileContentRequestModel();

                // ADDED BY SHUBHAM BHAGAT ON 21-05-2019 to replace "|" with space
                model.Path = model.Path.Replace("|", " ");

                requestModel.FilePath = model.Path;
                requestModel.isZipRequired = false;
                FileContentResponseModel fileContentResponseModel = service.GetFileContent(requestModel);
                if (fileContentResponseModel.isError)
                {
                    responseModel.IsErrorField = fileContentResponseModel.isError;
                    responseModel.SErrorMsgField = fileContentResponseModel.sErrorMsg;
                    return responseModel;
                }
                else
                {
                    responseModel.DownloadFileNameField = fileContentResponseModel.DownloadFileName;
                    responseModel.FileContentField = fileContentResponseModel.FileContent;
                    return responseModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally { }
        }

        /// <summary>
        /// DownLoadZippedFile
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public FileDownloadModel DownLoadZippedFile(ErrorFileRequestModel model)
        {
            FileDownloadModel responseModel = new FileDownloadModel();
            try
            {
                AnyWhereECApplicationServices.AnyWhereECApplicationServices service = new AnyWhereECApplicationServices.AnyWhereECApplicationServices();
                FileContentRequestModel requestModel = new FileContentRequestModel();
                requestModel.FilePath = model.Path;
                requestModel.isZipRequired = true;
                FileContentResponseModel fileContentResponseModel = service.GetFileContent(requestModel);
                if (fileContentResponseModel.isError)
                {
                    responseModel.IsErrorField = fileContentResponseModel.isError;
                    responseModel.SErrorMsgField = fileContentResponseModel.sErrorMsg;
                    return responseModel;
                }
                else
                {
                    responseModel.DownloadFileNameField = fileContentResponseModel.DownloadFileName;
                    responseModel.FileContentField = fileContentResponseModel.FileContent;
                    return responseModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
            }
        }

        /// <summary>
        /// LoadDriveInfoGrid
        /// </summary>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public ErrorLogFilesViewModel LoadDriveInfoGrid(ErrorFileRequestModel requestModel)
        {
            ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();

            try
            {

                AnyWhereECApplicationServices.AnyWhereECApplicationServices service = new AnyWhereECApplicationServices.AnyWhereECApplicationServices();
                ErrorDetailsResponseModel responseModel = service.GetErrorFileDetails("root");
                if (responseModel.isError)
                {
                    model.isError = responseModel.isError;
                    model.sErrorMessage = responseModel.sErrorMsg;
                    model.DriveInfoModelList = new List<DriveInfoModel>();

                }
                if (responseModel.DriveInfoModelList != null)
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
                else
                {
                    model.DriveInfoModelList = new List<DriveInfoModel>();
                }
                return model;
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        /// <summary>
        /// GetErrorDirectoryList
        /// </summary>
        /// <param name="ApplicationName"></param>
        /// <returns></returns>
        public ErrorLogFilesViewModel GetErrorDirectoryList(String ApplicationName)
        {
            ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();
            model.ErrorDirectoryList = new List<SelectListItem>();

            try
            {

                AnyWhereECApplicationServices.AnyWhereECApplicationServices service = new AnyWhereECApplicationServices.AnyWhereECApplicationServices();
                ErrorDetailsResponseModel responseModel = service.GetErrorDirectoryList(ApplicationName);

                if (responseModel != null)
                {
                    if (responseModel.ErrorDirectoryList != null)
                    {
                        if (responseModel.ErrorDirectoryList.Count() > 0)
                        {
                            foreach (var item in responseModel.ErrorDirectoryList)
                            {
                                SelectListItem selectListItem = new SelectListItem();
                                selectListItem.Text = item.Text;
                                selectListItem.Value = item.Value;
                                model.ErrorDirectoryList.Add(selectListItem);
                            }
                        }
                    }
                }
                else
                {

                }
                return model;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}