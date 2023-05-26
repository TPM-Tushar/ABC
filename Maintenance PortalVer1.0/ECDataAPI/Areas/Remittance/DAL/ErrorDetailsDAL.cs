#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ErrorDetailsDAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer for Remittance  module.
*/
#endregion

using CustomModels.Models.Remittance.ErrorDetails;
using ECDataAPI.Common;
using Ionic.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.Remittance.DAL
{
    public class ErrorDetailsDAL
    {
        private string ApiException = System.Configuration.ConfigurationManager.AppSettings["KaveriApiExceptionLogPath"];
        private string UIException = System.Configuration.ConfigurationManager.AppSettings["KaveriUILogPath"];
        private string KaveriWebServiceErrorLogPath = System.Configuration.ConfigurationManager.AppSettings["KaveriWebServiceErrorLogPath"];
        private string KOSPaymentLog = System.Configuration.ConfigurationManager.AppSettings["KOSPaymentLog"];
        private string KOSSakalaLog = System.Configuration.ConfigurationManager.AppSettings["KOSSakalaLog"];

        private string KOSErrorLog = System.Configuration.ConfigurationManager.AppSettings["KOSErrorLog"];

        private string UploaderService = System.Configuration.ConfigurationManager.AppSettings["UploaderService"];



        //private string ApiException = System.Configuration.ConfigurationSettings.AppSettings["KaveriApiExceptionLogPath"];
        //private string UIException = System.Configuration.ConfigurationSettings.AppSettings["KaveriUILogPath"];

        /// <summary>
        /// GetErrorFileDetails
        /// </summary>
        /// <param name="sPath"></param>
        /// <returns></returns>
        public ErrorDetailsResponseModel GetErrorFileDetails(string sPath)
        {
            List<string> AllowedPaths = new List<string>() { 
           // "C:\\ECDataServiceErrors\\",
          ApiException.Replace( @"\","*"),
          UIException.Replace( @"\","*"),
          KaveriWebServiceErrorLogPath.Replace( @"\","*"),
          KOSPaymentLog.Replace( @"\","*"),
          KOSErrorLog.Replace( @"\","*"),
          KOSSakalaLog.Replace( @"\","*"),
          UploaderService.Replace( @"\","*"),
          "root"
            };
            bool isUnauthorizeAccess = true;


            ErrorDetailsResponseModel responseModel = new ErrorDetailsResponseModel();
            if (string.IsNullOrEmpty(sPath))
            {
                responseModel.isError = true;
                responseModel.sErrorMsg = "Path is required";
                return responseModel;
            }


            isUnauthorizeAccess = AllowedPaths.Where(c => sPath.StartsWith(c)).Any();


            if (!isUnauthorizeAccess)
            {

                responseModel.isError = true;
                responseModel.sErrorMsg = "Unauthorized access";
                return responseModel;
            }
            sPath = sPath.Replace("*", @"\");

            if (sPath == "root")
            {

                if (!Directory.Exists(ApiException))
                {
                    responseModel.isError = true;
                    responseModel.sErrorMsg = ApiException + " Directory does not exists ";

                    responseModel.DriveInfoModelList = getalldrivestotalnfreespace();
                    return responseModel;
                }

                responseModel.DirectoryNameArray = Directory.GetDirectories(ApiException).OrderByDescending(x => x).ToArray();

                responseModel.FileNameArray = Directory.GetFiles(ApiException).OrderByDescending(x => x).ToArray(); 

                responseModel.DriveInfoModelList = getalldrivestotalnfreespace();
            }
            else
            {

                if (!Directory.Exists(sPath))
                {
                    responseModel.isError = true;
                    responseModel.sErrorMsg = sPath + " Directory does not exists ";

                    responseModel.DriveInfoModelList = getalldrivestotalnfreespace();
                    return responseModel;

                }
                responseModel.DirectoryNameArray = Directory.GetDirectories(sPath).OrderByDescending(x => x).ToArray(); 
                responseModel.FileNameArray = Directory.GetFiles(sPath).OrderByDescending(x => x).ToArray(); 
            }


            return responseModel;


        }

        /// <summary>
        /// getalldrivestotalnfreespace
        /// </summary>
        /// <returns></returns>
        public List<DriveInformationModel> getalldrivestotalnfreespace()
        {
            List<DriveInformationModel> DriveInfoModelList = new List<DriveInformationModel>();
            try
            {

                DriveInformationModel infoModel = null;
                foreach (DriveInfo drive in DriveInfo.GetDrives())
                {
                    double dTotalSpace = 0;
                    double dFreeSpace = 0;
                    double frprcntg = 0;
                    long divts = 1024 * 1024 * 1024;
                    long divfs = 1024 * 1024 * 1024;
                    string tsunit = "GB";
                    string fsunit = "GB";
                    infoModel = new DriveInformationModel();

                    if (drive.IsReady)
                    {
                        dFreeSpace = drive.TotalFreeSpace;
                        dTotalSpace = drive.TotalSize;
                        frprcntg = (dFreeSpace / dTotalSpace) * 100;
                        if (drive.TotalSize < 1024)
                        {
                            divts = 1; tsunit = "Byte(s)";
                        }
                        else if (drive.TotalSize < (1024 * 1024))
                        {
                            divts = 1024; tsunit = "KB";
                        }
                        else if (drive.TotalSize < (1024 * 1024 * 1024))
                        {
                            divts = 1024 * 1024; tsunit = "MB";
                        }
                        //----------------------
                        if (drive.TotalFreeSpace < 1024)
                        {
                            divfs = 1; fsunit = "Byte(s)";
                        }
                        else if (drive.TotalFreeSpace < (1024 * 1024))
                        {
                            divfs = 1024; fsunit = "KB";
                        }
                        else if (drive.TotalFreeSpace < (1024 * 1024 * 1024))
                        {
                            divfs = 1024 * 1024; fsunit = "MB";
                        }


                        infoModel.DriveName = drive.VolumeLabel + "[" + drive.Name.Substring(0, 2) + "]";
                        infoModel.FreeSpace = String.Format("{0,10:0.0}", ((dFreeSpace / divfs)).ToString("N2")) + fsunit;
                        infoModel.TotalSpace = String.Format("{0,10:0.0}", (dTotalSpace / divts).ToString("N2")) + tsunit;
                        infoModel.FileSystem = drive.DriveFormat;
                        infoModel.FreeSpacePercentage = frprcntg.ToString("N2") + "%";
                        infoModel.DriveType = drive.DriveType.ToString();
                        DriveInfoModelList.Add(infoModel);
                    }
                }
            }
            catch (Exception ex)
            {
                String exMsg = String.Empty;
                if (ex != null)
                {
                    exMsg = exMsg + "getalldrivestotalnfreespace" + ex.Message + ex.StackTrace.ToString();
                }

                if (ex.InnerException != null)
                {
                    exMsg = exMsg + ex.InnerException.Message + ex.InnerException.StackTrace.ToString();
                }
                ApiCommonFunctions.WriteErrorLog(exMsg);
            }
            return DriveInfoModelList;
        }

        /// <summary>
        /// GetFileContent
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        public FileContentResponseModel GetFileContent(FileContentRequestModel reqModel)
        {
            FileContentResponseModel responseModel = new FileContentResponseModel();

            if (File.Exists(reqModel.FilePath))
            {
                responseModel.FileContent = File.ReadAllBytes(reqModel.FilePath);
                responseModel.DownloadFileName = Path.GetExtension(reqModel.FilePath);
            }
            else if (Directory.Exists(reqModel.FilePath))
            {
                string[] NameArray = Directory.GetFiles(reqModel.FilePath);

                ZipFile zipRuntime = new ZipFile();

                zipRuntime.UseZip64WhenSaving = Zip64Option.AsNecessary;

                if (NameArray.Length > 0)
                {
                    foreach (var item in NameArray)
                    {
                        if (File.Exists(item))
                            zipRuntime.AddEntry(Path.GetFileName(item), File.ReadAllBytes(item));
                    }
                    string zipName = string.Empty;
                    using (MemoryStream ms = new MemoryStream())
                    {
                        zipRuntime.Save(ms);
                        responseModel.FileContent = ms.ToArray();
                    }
                }
            }
            else
            {
                responseModel.isError = true;
                responseModel.sErrorMsg = "No file present for the given path";
            }

            return responseModel;

        }

        /// <summary>
        /// GetErrorDirectoryList
        /// </summary>
        /// <param name="ApplicationName"></param>
        /// <returns></returns>
        public ErrorDetailsResponseModel GetErrorDirectoryList(String ApplicationName)
        {
            ErrorDetailsResponseModel responseModel = new ErrorDetailsResponseModel();
            responseModel.ErrorDirectoryList=new List<SelectListItem>();

            // For Default select 
            SelectListItem selectItem = new SelectListItem();
            selectItem.Text = "Select";
            selectItem.Value = "0";
            responseModel.ErrorDirectoryList.Add(selectItem);

            if (ApplicationName.ToLower() == "kaveriweb")
            {
                // ADDED ErrorLogItem ITEM
                SelectListItem ErrorLogItem = new SelectListItem();
                ErrorLogItem.Text = "Kaveri Web Service Error";
                ErrorLogItem.Value = KaveriWebServiceErrorLogPath.Replace("\\", "*");
                responseModel.ErrorDirectoryList.Add(ErrorLogItem);

            }
            else
            {
                if (ApplicationName.ToLower() == "ecdataportal")
                {
                    // ADDED API ITEM
                    SelectListItem APIItem = new SelectListItem();
                    APIItem.Text = "ApiException";
                    APIItem.Value = ApiException.Replace("\\", "*");
                    responseModel.ErrorDirectoryList.Add(APIItem);

                    // ADDED UI ITEM
                    SelectListItem UIItem = new SelectListItem();
                    UIItem.Text = "UIException";
                    UIItem.Value = UIException.Replace("\\", "*");
                    responseModel.ErrorDirectoryList.Add(UIItem);
                }


            }
        

            return responseModel;
        }
    }
}