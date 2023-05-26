#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ErrorLogFilesController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Remittance module.
*/
#endregion
using CustomModels.Models.Remittance.ErrorLogFiles;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ErrorLogFilesController : Controller
    {
        #region Properties
        private ServiceCaller caller = new ServiceCaller("ErrorLogFilesApiController");
        #endregion

        #region Method
        /// <summary>
        /// Error Log Files View
        /// </summary>
        /// <returns>returns view</returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Error Log Files View")]
        public ActionResult ErrorLogFilesView()
        {
            try
            {
                // Added BY Shubham Bhagat on 03-05-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ErrorLogFiles;

                ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();
                //model = caller.GetCall<ErrorLogFilesViewModel>("ErrorLogFilesView");

                // TO SET PATH VARIABLE VALUE IN JAVASCRIPT 
                model.PresentDirectory = "root";

                // ADDED BY SHUBHAM BHAGAT ON 17-05-2019
                model.ApplicationList = new List<SelectListItem>();

                // For Default select 
                SelectListItem selectItem = new SelectListItem();
                selectItem.Text = "Select";
                selectItem.Value = "0";
                model.ApplicationList.Add(selectItem);

                model.ErrorDirectoryList = new List<SelectListItem>();
                // For Default select 
                model.ErrorDirectoryList.Add(selectItem);

                // For ECDATA Portal Service
                SelectListItem EcdataPortal = new SelectListItem();
                EcdataPortal.Text = "ECDATA Portal";
                EcdataPortal.Value = "ECDATAPortal";
                model.ApplicationList.Add(EcdataPortal);

                // For ECDATA Service
                SelectListItem ECDATAService = new SelectListItem();
                ECDATAService.Text = "ECDATA Service";
                ECDATAService.Value = "ECDATA";
                model.ApplicationList.Add(ECDATAService);

                // For PreReg Service
                SelectListItem PreRegService = new SelectListItem();
                PreRegService.Text = "PreReg Service";
                PreRegService.Value = "Prereg";
                model.ApplicationList.Add(PreRegService);

                // For Uploader Service 
                SelectListItem UploaderService = new SelectListItem();
                UploaderService.Text = "Uploader Service";
                UploaderService.Value = "Uploader";
                model.ApplicationList.Add(UploaderService);

                // For Kaveri Online Service 
                SelectListItem KaveriOnlineService = new SelectListItem();
                KaveriOnlineService.Text = "Kaveri Online Service";
                KaveriOnlineService.Value = "KaveriOnline";
                model.ApplicationList.Add(KaveriOnlineService);

                // For Kaveri Online Service 
                SelectListItem KaveriWebService = new SelectListItem();
                KaveriWebService.Text = "Kaveri Web Service";
                KaveriWebService.Value = "KaveriWeb";
                model.ApplicationList.Add(KaveriWebService);

                //Added by tushar on 14 dec 2021 (To fetch AnywhereEC Logs)
                SelectListItem KaveriAnywhereECService = new SelectListItem();
                KaveriAnywhereECService.Text = "AnyWhereEC Service";
                KaveriAnywhereECService.Value = "anywhereecservice";
                model.ApplicationList.Add(KaveriAnywhereECService);


                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        // TO GET DRIVE INFORMATION
        /// <summary>
        /// Load Drive Info Grid
        /// </summary>
        /// <param name="ServiceName"></param>
        /// <returns>returns Drive Info List</returns>
        [EventAuditLogFilter(Description = "Load Drive Info Grid")]
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadDriveInfoGrid(String ServiceName)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();// FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();//
                var length = Request.Form.GetValues("length").FirstOrDefault();//
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                             
                ErrorFileRequestModel reqModel = new ErrorFileRequestModel();
                reqModel.ServiceName = ServiceName;
                ErrorLogFilesViewModel errorLogFilesViewModel = caller.PostCall<ErrorFileRequestModel, ErrorLogFilesViewModel>("LoadDriveInfoGrid", reqModel);

                // ADDED BY SHUBHAM BHAGAT ON 15-05-2019              
                if (errorLogFilesViewModel == null)
                {
                    var emptyData = Json(new
                    {
                        draw = draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No data found."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //Sorting working but commented
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    FolderNameList = FolderNameList.  OrderBy(sortColumn + " " + sortColumnDir);
                //}

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    errorLogFilesViewModel.DriveInfoModelList = errorLogFilesViewModel.DriveInfoModelList.Where(m => m.DriveName.ToLower().Contains(searchValue.ToLower())).ToList();
                }

                //total number of rows count     
                recordsTotal = errorLogFilesViewModel.DriveInfoModelList.Count();
                //Paging     
                var data = errorLogFilesViewModel.DriveInfoModelList.Skip(skip).Take(pageSize).ToList();

                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                return emptyData;
            }
        }

        // TO GET FOLDER NAME LIST
        //  [HttpGet]
        /// <summary>
        /// Load Folder Name Grid
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ServiceName"></param>
        /// <param name="isBackward"></param>
        /// <returns>returns Folder name List</returns>
        [EventAuditLogFilter(Description = "Load Folder Name Grid")]
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadFolderNameGrid(String path, String ServiceName, bool isBackward = false)
        {
            try
            {
                if (isBackward)
                {
                    //path=path.Substring(0, path.LastIndexOf("\\"));
                    path = path.Replace("/", "*");
                }
                // ADDED AND CHANGED LOGIC OF BACK BUTTON BY SHUBHAM BHAGAT ON 9-5-2019 
                // TO RESET PATH WHEN DRIVE NAME COME IN PATH VARIABLE
                if (path.EndsWith(":"))
                    path = "root";
                var draw = Request.Form.GetValues("draw").FirstOrDefault();// FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();//
                var length = Request.Form.GetValues("length").FirstOrDefault();//
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // For Root path
                if (String.IsNullOrEmpty(path))
                    path = String.Empty;

                ErrorFileRequestModel requestModel = new ErrorFileRequestModel();
                requestModel.Path = path;
                requestModel.GetFolderList = true;
                // For Getting Only Folder List
                requestModel.GetFileList = false;

                requestModel.ServiceName = ServiceName;

                FileInfoModelWrapper fileInfoModelWrapper = caller.PostCall<ErrorFileRequestModel, FileInfoModelWrapper>("LoadFolderNameGrid", requestModel);

                // ADDED BY SHUBHAM BHAGAT ON 15-05-2019              
                if (fileInfoModelWrapper == null)
                {
                    var emptyData = Json(new
                    {
                        draw = draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No data found."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //Sorting working but commented
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    FolderNameList = FolderNameList.  OrderBy(sortColumn + " " + sortColumnDir);
                //}

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    fileInfoModelWrapper.FolderNameList = fileInfoModelWrapper.FolderNameList.Where(m => m.FileName.ToLower().Contains(searchValue.ToLower())).ToList();
                }

                //total number of rows count     
                recordsTotal = fileInfoModelWrapper.FolderNameList.Count();
                //Paging     
                var data = fileInfoModelWrapper.FolderNameList.Skip(skip).Take(pageSize).ToList();

                // Some changes done in below code on 03-04-2019 AT 12:00 PM By Shubham Bhagat 
                string str = String.Empty;
                string BackButton = String.Empty;
                String PresentDirectory = String.Empty;
                if (fileInfoModelWrapper.FolderNameList.Count() > 0)
                {
                    // GETTING PARENT DIRECTORY PATH
                    str = fileInfoModelWrapper.PresentDirectory.Substring(0, fileInfoModelWrapper.PresentDirectory.LastIndexOf("\\"));
                    str = str.Replace("\\", @"\/");
                    // TO SKIP BACK BUTTON 
                    // ADDED AND CHANGED LOGIC OF BACK BUTTON BY SHUBHAM BHAGAT ON 9-5-2019 
                    //BackButton = (str.Equals("C:") || str.Equals("E:")) ? "" : "<button type ='button' class='btn btn-group-md btn-warning' onclick=BackFolder('" + (str) + "')>Back</button>";
                    BackButton =  "<button type ='button' class='btn btn-group-md btn-warning' onclick=BackFolder('" + (str) + "')>Back</button>";

                    // TO SET PRESENT WORKING DIRECTORY
                    PresentDirectory = fileInfoModelWrapper.PresentDirectory;
                }
                else
                {
                    // WHEN WE SEARCHING SO  "root" IS COMMING AND path.substring is throwing exception so check for root is done
                    if (!path.Equals("root"))
                    {
                        // TO SET PRESENT WORKING DIRECTORY
                        PresentDirectory = path.Replace("*", "/");

                        // TO DRAW BACK BUTTON WHEN NO FOLDER AND FILE EXIST AND IF FILE EXISTS SO BACK BUTTON WILL REDRAW WHEN THE FILE DATATABLE COMPLETE FUNCTION WILL DRAW BACK BUTTON
                        path = path.Substring(0, path.LastIndexOf("*"));
                        path = path.Replace("*", @"\/");
                        BackButton = "<button type ='button' class='btn btn-group-md btn-warning' onclick=BackFolder('" + (path) + "')>Back</button>";
                    }
                }
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data, PresentDirectory, BackButton });
                //return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data, PresentDirectory = fileInfoModelWrapper.PresentDirectory, BackButton });

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                return emptyData;
            }
        }

        /// <summary>
        /// Load File Name Grid
        /// </summary>
        /// <param name="path"></param>
        /// <param name="ServiceName"></param>
        /// <param name="isBackward"></param>
        /// <returns>returns File name List</returns>
        // TO GET FILE NAME LIST
        [EventAuditLogFilter(Description = "Load File Name Grid")]
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadFileNameGrid(String path, String ServiceName, bool isBackward = false)
        {
            try
            {
                if (isBackward)
                {
                    //path = path.Substring(0, path.LastIndexOf("\\"));
                    path = path.Replace("/", "*");
                }
                // ADDED AND CHANGED LOGIC OF BACK BUTTON BY SHUBHAM BHAGAT ON 9-5-2019 
                // TO RESET PATH WHEN DRIVE NAME COME IN PATH VARIABLE
                if (path.EndsWith(":"))
                    path = "root";

                var draw = Request.Form.GetValues("draw").FirstOrDefault();// FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();//
                var length = Request.Form.GetValues("length").FirstOrDefault();//
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // For Root path
                if (String.IsNullOrEmpty(path))
                    path = String.Empty;

                ErrorFileRequestModel requestModel = new ErrorFileRequestModel();
                requestModel.Path = path;
                requestModel.GetFolderList = false;
                // For Getting Only Folder List
                requestModel.GetFileList = true;
                requestModel.ServiceName = ServiceName;
                FileInfoModelWrapper fileInfoModelWrapper = caller.PostCall<ErrorFileRequestModel, FileInfoModelWrapper>("LoadFolderNameGrid", requestModel);

                // ADDED BY SHUBHAM BHAGAT ON 15-05-2019              
                if (fileInfoModelWrapper == null)
                {
                    var emptyData = Json(new
                    {
                        draw = draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No data found."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //Sorting working but commented
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    FolderNameList = FolderNameList.  OrderBy(sortColumn + " " + sortColumnDir);
                //}

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    fileInfoModelWrapper.FileNameList = fileInfoModelWrapper.FileNameList.Where(m => m.FileName.ToLower().Contains(searchValue.ToLower())).ToList();
                }

                //total number of rows count     
                recordsTotal = fileInfoModelWrapper.FileNameList.Count();
                //Paging     
                var data = fileInfoModelWrapper.FileNameList.Skip(skip).Take(pageSize).ToList();

                string str = String.Empty;
                string BackButton = String.Empty;
                string DownloadAllButton = String.Empty;
                string FolderPathForDownload = String.Empty;

                if (fileInfoModelWrapper.FileNameList.Count() > 0)
                {
                    // GETTING PARENT DIRECTORY PATH
                    str = fileInfoModelWrapper.PresentDirectory.Substring(0, fileInfoModelWrapper.PresentDirectory.LastIndexOf("\\"));
                    str = str.Replace("\\", @"\/");
                    // TO SKIP BACK BUTTON 
                    // ADDED AND CHANGED LOGIC OF BACK BUTTON BY SHUBHAM BHAGAT ON 9-5-2019 
                    // BackButton = (str.Equals("C:") || str.Equals("E:")) ? "" : "<button type ='button' class='btn btn-group-md btn-warning' onclick=BackFolder('" + (str) + "')>Back</button>";
                    BackButton =  "<button type ='button' class='btn btn-group-md btn-warning' onclick=BackFolder('" + (str) + "')>Back</button>";

                    // FOR DOWNLOAD FOLDER PATH
                    FolderPathForDownload = fileInfoModelWrapper.PresentDirectory.Replace("\\", @"\/");
                    DownloadAllButton = "<button type ='button' class='btn btn-group-md btn-success' onclick=DownLoadZippedFile('" + (FolderPathForDownload) + "')>Download ALL</button>";
                }

                // To stop reseting of PresentDirectory when no file exist in folder
                bool setPresentDirectory = true;
                if (fileInfoModelWrapper.FileNameList.Count() == 0)
                {
                    setPresentDirectory = false;

                    // TO HIDE DOWNLOAD BUTTON WHEN NO FILES ARE THERE
                    DownloadAllButton = "";
                }
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data, PresentDirectory = fileInfoModelWrapper.PresentDirectory, setPresentDirectory, BackButton, DownloadAllButton });

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                return emptyData;
            }
        }

        /// <summary>
        /// DownLoad File
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="ServiceName"></param>
        /// <returns>returns text file</returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Error Log DownLoad File")]
        public ActionResult DownLoadFile(String FilePath, String ServiceName)
        {
            try
            {
                string downloadFileName = FilePath.Substring(FilePath.LastIndexOf(@"/") + 1);
                FileDownloadModel model = new FileDownloadModel();
                ErrorFileRequestModel requestModel = new ErrorFileRequestModel();
                requestModel.Path = FilePath;
                requestModel.ServiceName = ServiceName;
                model = caller.PostCall<ErrorFileRequestModel, FileDownloadModel>("DownLoadFile", requestModel);

                HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + downloadFileName);
                return File(model.FileContentField, "text/plain");
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// DownLoad Zipped File
        /// </summary>
        /// <param name="FolderPath"></param>
        /// <param name="ServiceName"></param>
        /// <returns>returns zip file</returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Error Log DownLoad Zipped File")]
        public ActionResult DownLoadZippedFile(String FolderPath, String ServiceName)
        {
            try
            {
                string downloadFolderName = FolderPath.Substring(FolderPath.LastIndexOf(@"/") + 1);
                FileDownloadModel model = new FileDownloadModel();
                ErrorFileRequestModel requestModel = new ErrorFileRequestModel();
                requestModel.Path = FolderPath;
                requestModel.ServiceName = ServiceName;
                model = caller.PostCall<ErrorFileRequestModel, FileDownloadModel>("DownLoadZippedFile", requestModel);

                HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + downloadFolderName + ".zip");
                return File(model.FileContentField, "application/zip");
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
            }
        }

        // ADDED BY SHUBHAM BHAGAT ON 18-05-2019
        //[HttpGet]
        /// <summary>
        /// Get Error Directory List
        /// </summary>
        /// <param name="ApplicationName"></param>
        /// <returns>returns Error Directory List</returns>
        [EventAuditLogFilter(Description = "Get Error Directory List")]
        public ActionResult GetErrorDirectoryList(String ApplicationName)
        {
            ErrorLogFilesViewModel model = new ErrorLogFilesViewModel();
            try
            {

                // VALIDATION
                if (String.IsNullOrEmpty(ApplicationName))
                    return Json(new { errorMsg = "Please Select Application" }, JsonRequestBehavior.AllowGet);

                if (ApplicationName == "0")
                    return Json(new { errorMsg = "Please Select Application" }, JsonRequestBehavior.AllowGet);

                model = caller.GetCall<ErrorLogFilesViewModel>("GetErrorDirectoryList", new { ApplicationName = ApplicationName });
                if (model == null)
                {
                    return Json(new { errorMsg = "Error in getting Error Directory List." }, JsonRequestBehavior.AllowGet);
                }

                //By Shubham bhagat on 26-04-2019
                //JSON with JsonRequestBehavior.AllowGet is working fine when KaveriAuthorizationAttribute is not added. 
                return Json(new {  model.ErrorDirectoryList }, JsonRequestBehavior.AllowGet);
                //return Json(new { OfficeList = model.OfficeTypeList });
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return Json(new { errorMsg = "Error in getting Error Directory List." }, JsonRequestBehavior.AllowGet);

            }
        }

        #endregion
    }
}