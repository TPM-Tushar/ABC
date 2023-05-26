
#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   KaveriSupportController.cs
    * Author Name       :   - Akash Patil
    * Creation Date     :   - 02-05-2019
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for kaveri support module.
*/
#endregion


#region References
using CustomModels.Models.KaveriSupport;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
#endregion

namespace ECDataUI.Areas.KaveriSupport.Controllers
{
    [KaveriAuthorizationAttribute]
    public class KaveriSupportController : Controller
    {
        #region Properties and user variables

        string MaxFileSizeToUploadFileForEncryption = ConfigurationManager.AppSettings["MaxFileSizeToUploadFileForEncryption"];

        #endregion

        #region Methods

        #region Ticket registration and Generate Key Pair

        /// <summary>
        /// Returns View i.e Ticket Registration
        /// </summary>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Kaveri Support View")]

        public ActionResult KaveriSupport()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.TicketRegistration;

                string errorMessage = string.Empty;


                ServiceCaller caller = new ServiceCaller("KaveriSupportApiController");

                AppDeveloperViewModel responseModel = caller.GetCall<AppDeveloperViewModel>("GetTicketRegistrationDetails");

                if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = responseModel.ErrorMessage, URLToRedirect = "/Home/HomePage" });
                }


                AppDeveloperViewModel _appDeveloperModel = new AppDeveloperViewModel();

                _appDeveloperModel.ModuleNameDropDown = responseModel.ModuleNameDropDown;
                _appDeveloperModel.SRONameDropDown = responseModel.SRONameDropDown;

                return View("KaveriSupport", _appDeveloperModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = e.GetBaseException().Message, URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// POST call for Ticket registration.
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Register Ticket Details And Generate Key Pair")]

        public ActionResult RegisterTicketDetailsAndGenerateKeyPair(AppDeveloperViewModel viewModel)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("KaveriSupportApiController");
            viewModel.IsUploadedSuccessfully = false;

            try
            {



                var regex = @"([#$%<>])";
                #region Validations
                if (viewModel.SRONameID == 0)
                    return Json(new { success = false, message = "SRO Name is required" });

                if (viewModel.ModuleID == 0)
                    return Json(new { success = false, message = "Module Name is required" });

                if (string.IsNullOrEmpty(viewModel.TicketNumber))
                    return Json(new { success = false, message = "Ticket number is required" });


                if (viewModel.TicketNumber.Length > 8)
                    return Json(new { success = false, message = "Ticket Number should be less than or equal to 8 digits." });


                if (!AllNumeric(viewModel.TicketNumber))
                    return Json(new { success = false, message = "Invalid ticket number , Please enter valid ticket number." });

                if (viewModel.TicketNumber.FirstOrDefault() == '0')
                    return Json(new { success = false, message = "Invalid ticket number (Ticket number shouldn't start with 0) , Please enter valid ticket number." });



                if (string.IsNullOrEmpty(viewModel.TicketDescription))
                    return Json(new { success = false, message = "Ticket description is required" });

                if (!string.IsNullOrEmpty(viewModel.TicketDescription))
                {
                    Match match = Regex.Match(viewModel.TicketDescription, regex, RegexOptions.IgnoreCase);
                    if (match.Success)
                        return Json(new { success = false, message = "These $ # % < > characters are not allowed in Ticket description" });
                }
                #endregion

                if (ModelState.IsValid)
                {

                    AppDeveloperViewModel responseModel = caller.PostCall<AppDeveloperViewModel, AppDeveloperViewModel>("RegisterTicketDetailsAndGenerateKeyPair", viewModel, out errorMessage);
                    if (!String.IsNullOrEmpty(errorMessage))
                        return Json(new { success = false, message = errorMessage });


                    if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                    {
                        return Json(new { success = false, message = responseModel.ErrorMessage });

                    }
                    else
                    {
                        return Json(new { success = true, message = responseModel.ResponseMessage });

                    }
                }
                else
                {
                    String messages = String.Join("\n", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).FirstOrDefault());
                    return Json(new { success = false, message = messages });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return Json(new { success = false, message = e.GetBaseException().Message });

            }
        }



        #endregion

        #region Decrypt Enclosue

        /// <summary>
        /// Return view to decrypt enclosure
        /// </summary>
        /// <returns></returns>

        public ActionResult DecryptEnclosure()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DecryptEnclosure;

                AppDeveloperViewModel _appDeveloperModel = new AppDeveloperViewModel();
                return View(_appDeveloperModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = e.GetBaseException().Message, URLToRedirect = "/Home/HomePage" });
            }

        }


        /// <summary>
        /// Post call for Decrypt enclosure file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Decrypt Enclosure File")]

        public ActionResult DecryptEnclosureFile()
        {
            try
            {
                CommonFunctions.WriteErrorLog("---------------------------------------------------------------------------------");
                CommonFunctions.WriteErrorLog("IN DecryptEnclosureFile ");

                AppDeveloperViewModel viewModel = new AppDeveloperViewModel();
                string rootPath = ConfigurationManager.AppSettings["KaveriSupportPath"];

                CommonFunctions.WriteErrorLog("rootPath ::  " + rootPath);

                HttpPostedFileBase FileBase = null;
                string arr = Request.Params["filesArray"];

                string TicketNumber = Request.Params["TicketNumber"];


                CommonFunctions.WriteErrorLog("arr ::  " + arr);

                CommonFunctions.WriteErrorLog("TicketNumber ::  " + TicketNumber);

                if (string.IsNullOrEmpty(TicketNumber))
                {
                    return Json(new { success = false, message = "Ticket number is required." });
                }

                if (TicketNumber.Length > 8)
                {
                    return Json(new { success = false, message = "Ticket Number should be less than or equal to 8 digits." });
                }


                if (!AllNumeric(TicketNumber))
                {
                    return Json(new { success = false, message = "Invalid ticket number , Please enter valid ticket number." });
                }

                if (TicketNumber.FirstOrDefault() == '0')
                {
                    return Json(new { success = false, message = "Invalid ticket number (Ticket number shouldn't start with 0) , Please enter valid ticket number." });
                }


                CommonFunctions.WriteErrorLog("Validation Done");

                //string arr = FileArrBase;
                string[] filesArray = arr.Split(',');
                string FileName = string.Empty;
                if (Request.Files.Count > 0)
                {
                    if (Request.Files.Count >= 1)
                    {
                        foreach (string sFileName in Request.Files)
                        {
                            FileBase = Request.Files[sFileName];
                            int DblExtensions = FileBase.FileName.Count(f => f == '.');
                            if (!IsValidExtension(Path.GetExtension(FileBase.FileName.ToLower())))
                            {
                                return Json(new { success = false, message = "Please upload zip file only. Error in file name" + FileBase.FileName + " Kindly select file again." }, JsonRequestBehavior.AllowGet);
                            }
                            else if (DblExtensions > 1)
                            {
                                return Json(new { success = false, errormsgType = 3, message = "Cannot Upload Files because it is either not a supported File type or because the file has been damaged(for example,it was sent as an attachment and wasn't correctly decoded)!!" }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        int iNoOfFiles = 0;
                        int i = 0;

                        foreach (string sFileName in Request.Files)
                        {
                            FileBase = Request.Files[sFileName];

                            FileName = FileBase.FileName.Split('.')[0];

                            CommonFunctions.WriteErrorLog("FileBase_FileName ::  " + FileName);

                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(Request.Files[sFileName].InputStream))
                            {
                                fileData = binaryReader.ReadBytes(Request.Files[sFileName].ContentLength);
                            }

                            CommonFunctions.WriteErrorLog("fileData ::  " + fileData);

                            //uint mimetype;
                            //FindMimeFromData(0, null, fileData, 256, null, 0, out mimetype, 0);
                            //System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                            //string mime = Marshal.PtrToStringUni(mimeTypePtr);
                            //Marshal.FreeCoTaskMem(mimeTypePtr);
                            ////Modified by Harshit on 18 Feb 2018
                            //if (mime != "application/x-zip-compressed")
                            //{
                            //    return Json(new { success = false, message = "Cannot Upload Files because it is either not a supported File type or because the file has been damaged(for example,it was sent as an attachment and wasn't correctly decoded)!!" }, JsonRequestBehavior.AllowGet);
                            //}

                            string sDirectoryStructure = GetDirectoryStructure();

                            CommonFunctions.WriteErrorLog("fileData ::  " + sDirectoryStructure);

                            if (!Directory.Exists(rootPath + sDirectoryStructure))
                                Directory.CreateDirectory(rootPath + sDirectoryStructure);

                            string physicalPath = rootPath + sDirectoryStructure + "\\" + FileName + ".zip";

                            CommonFunctions.WriteErrorLog("physicalPath ::  " + physicalPath);

                            string absolutePath = physicalPath.Substring(rootPath.Length).Replace('\\', '/').Insert(0, "~/KaveriSupport/");

                            CommonFunctions.WriteErrorLog("absolutePath ::  " + absolutePath);

                            viewModel.Filepath = physicalPath;


                            try
                            {
                                using (FileStream stream = new FileStream(physicalPath, FileMode.Create))
                                {
                                    stream.Write(fileData, 0, fileData.Length);
                                }

                                iNoOfFiles++;
                                i++;
                            }
                            catch (Exception)
                            {
                                if (!Directory.Exists(rootPath + sDirectoryStructure + "\\"))
                                    Directory.CreateDirectory(rootPath + sDirectoryStructure + "\\");

                                using (FileStream stream = new FileStream(physicalPath, FileMode.Create))
                                {
                                    stream.Write(fileData, 0, fileData.Length);
                                }

                                i++;
                            }

                            CommonFunctions.WriteErrorLog(i + " File written Successufully on server path : " + physicalPath);

                        }

                    }
                }
                else
                {
                    return Json(new { success = false, message = "Please select file to decrypt." });
                }

                CommonFunctions.WriteErrorLog("File Return on Server FilePath ::  " + viewModel.Filepath);

                string errorMessage = string.Empty;
                ServiceCaller caller = new ServiceCaller("KaveriSupportApiController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                viewModel.IsUploadedSuccessfully = false;


                viewModel.TicketNumber = TicketNumber;

                CommonFunctions.WriteErrorLog("TicketNumber ::  " + TicketNumber);

                DecryptEnclosureModel responseModel = caller.PostCall<AppDeveloperViewModel, DecryptEnclosureModel>("DecryptEnclosureFile", viewModel, out errorMessage);
                if (!String.IsNullOrEmpty(errorMessage))
                    return Json(new { success = false, message = errorMessage });

                CommonFunctions.WriteErrorLog("PostCall responseModel.ResponseStatus ::  " + responseModel.ResponseStatus);

                if (!responseModel.ResponseStatus)
                {
                    return Json(new { success = false, message = responseModel.ErrorMessage });
                }

                string DecryptFilepath = string.Empty;

                CommonFunctions.WriteErrorLog("PostCall responseModel.Filepath ::  " + responseModel.Filepath);
                CommonFunctions.WriteErrorLog("---------------------------------------------------------------------------------");
                if (!string.IsNullOrEmpty(responseModel.Filepath))//NewDecryptAndSave(viewModel.Filepath, out DecryptFilepath))
                {
                    return Json(new { success = true, message = "File Decrypted and Saved Successfully.", responseModel.Filepath, TicketNumber }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, message = "Error Occured while getting Filepath ." }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);

                return Json(new { success = false, message = "Error Occured while Decrypting Enclosure File." }, JsonRequestBehavior.AllowGet);
            }

        }

        private static bool AllNumeric(string test)
        {
            return !test.Any(ch => !Char.IsDigit(ch));
        }

        /// <summary>
        /// Call to download file from path provided in parameter
        /// </summary>
        /// <param name="Filepath"></param>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Download Enclosure (SQL) File")]

        public ActionResult DownloadFile(string Filepath, string TicketNumber)
        {
            try
            {
                //WebClient myWebClient = new WebClient();
                //byte[] bytes = myWebClient.DownloadData(Filepath); //Read file from remote file server
                CommonFunctions.WriteErrorLog("---------------------------------------------------------------------------------");
                CommonFunctions.WriteErrorLog("In DownloadFile");

                CommonFunctions.WriteErrorLog("Filepath ::  " + Filepath);

                CommonFunctions.WriteErrorLog("TicketNumber ::  " + TicketNumber);
                CommonFunctions.WriteErrorLog("---------------------------------------------------------------------------------");

                byte[] bytes = System.IO.File.ReadAllBytes(Filepath); //Only reads file from local.
                HttpContext.Response.AddHeader("content-disposition", "attachment; filename=DecryptedEnclosure_" + TicketNumber + ".sql");
                return File(bytes, "text/plain");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = e.GetBaseException().Message, URLToRedirect = "/Home/HomePage" });
            }
        }

        private bool IsValidExtension(string extension)
        {
            return (extension.Equals(".zip") || extension.Equals(".ZIP"));
        }

        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        private extern static System.UInt32 FindMimeFromData(System.UInt32 pBC,
        [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
        [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
        System.UInt32 cbSize, [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
        System.UInt32 dwMimeFlags,
        out System.UInt32 ppwzMimeOut,
        System.UInt32 dwReserverd);

        public string GetDirectoryStructure(int Type = 0, string TicketNumber = "")
        {
            try
            {
                string directoryStructure = string.Empty;
                if (Type == 1)
                {
                    string finYear = string.Empty;
                    finYear = GenerateFinancialYear(DateTime.Now);
                    directoryStructure = Path.Combine("KaveriSupport", finYear, "EncyrptedSQLPatch"); // _" + TicketNumber);
                }
                else if (Type == 2)
                {
                    string finYear = string.Empty;
                    finYear = GenerateFinancialYear(DateTime.Now);
                    directoryStructure = Path.Combine("KaveriSupport", finYear, "EncyrptedSQLPatchZIP", TicketNumber);
                }
                else
                {
                    string finYear = string.Empty;
                    finYear = GenerateFinancialYear(DateTime.Now);
                    directoryStructure = Path.Combine("KaveriSupport", finYear, "EncyrptedEnclosure");
                }
                return directoryStructure;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string GenerateFinancialYear(DateTime date)
        {
            int year = date.Year;
            if (date.Month > 3)
                return string.Concat(year, "-", (year + 1).ToString().Substring(2, 2));
            else
                return string.Concat((year - 1).ToString(), "-", year.ToString().Substring(2, 2));
        }
        #endregion


        #region Encrypt SQL Patch

        /// <summary>
        /// Returns View to input Details for View Modifications Audit Logs
        /// </summary>
        /// <returns></returns>
        public ActionResult EncryptSQLPatch()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.UploadPatchFile;

                AppDeveloperViewModel _appDeveloperModel = new AppDeveloperViewModel();
                return View("EncryptPatch", _appDeveloperModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = e.GetBaseException().Message, URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// POST Call to upload  patch file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Upload SQL Patch File")]

        public ActionResult UploadSQLPatchFile()//EncryptSQLPatchFile()
        {
            try
            {
                AppDeveloperViewModel viewModel = new AppDeveloperViewModel();
                string rootPath = ConfigurationManager.AppSettings["KaveriSupportPath"];
                HttpPostedFileBase FileBase = null;
                string arr = Request.Params["filesArray"];
                string[] filesArray = arr.Split(',');
                string FileName = string.Empty;
                if (Request.Files.Count > 0)
                {
                    if (Request.Files.Count >= 1)
                    {
                        foreach (string sFileName in Request.Files)
                        {
                            FileBase = Request.Files[sFileName];
                            int DblExtensions = FileBase.FileName.Count(f => f == '.');
                            if (!IsSQLValidExtension(Path.GetExtension(FileBase.FileName.ToLower())))
                            {
                                return Json(new { success = false, message = "Please upload sql file only. Error in file name" + FileBase.FileName + " Kindly select file again." }, JsonRequestBehavior.AllowGet);
                            }
                            else if (DblExtensions > 1)
                            {
                                return Json(new { success = false, errormsgType = 3, message = "Cannot Upload Files because it is either not a supported File type or because the file has been damaged(for example,it was sent as an attachment and wasn't correctly decoded)!!" }, JsonRequestBehavior.AllowGet);
                            }


                            if (!IsValidContentLength(FileBase.ContentLength))
                            {
                                return Json(new { success = false, message = "File size should be less than 6 MB. Kindly upload file again." }, JsonRequestBehavior.AllowGet);
                            }
                        }

                        int iNoOfFiles = 0;
                        int i = 0;

                        foreach (string sFileName in Request.Files)
                        {
                            FileBase = Request.Files[sFileName];

                            FileName = FileBase.FileName.Split('.')[0];

                            byte[] fileData = null;
                            using (var binaryReader = new BinaryReader(Request.Files[sFileName].InputStream))
                            {
                                fileData = binaryReader.ReadBytes(Request.Files[sFileName].ContentLength);
                            }

                            uint mimetype;
                            FindMimeFromData(0, null, fileData, 256, null, 0, out mimetype, 0);
                            System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                            string mime = Marshal.PtrToStringUni(mimeTypePtr);
                            Marshal.FreeCoTaskMem(mimeTypePtr);
                            if (mime != "text/plain" && mime != "application/octet-stream")
                            {
                                return Json(new { success = false, message = "Cannot Upload Files because it is either not a supported File type or because the file has been damaged(for example,it was sent as an attachment and wasn't correctly decoded)!!" }, JsonRequestBehavior.AllowGet);
                            }

                            string sDirectoryStructure = GetDirectoryStructure(1);

                            if (!Directory.Exists(rootPath + sDirectoryStructure))
                                Directory.CreateDirectory(rootPath + sDirectoryStructure);

                            string physicalPath = rootPath + sDirectoryStructure + "\\" + FileName + ".sql";
                            string absolutePath = physicalPath.Substring(rootPath.Length).Replace('\\', '/').Insert(0, "~/KaveriSupport/");
                            viewModel.Filepath = physicalPath;

                            try
                            {
                                using (FileStream stream = new FileStream(physicalPath, FileMode.Create))
                                {

                                    stream.Write(fileData, 0, fileData.Length);
                                    stream.Close();

                                }

                                iNoOfFiles++;
                                i++;
                            }
                            catch (Exception)
                            {
                                if (!Directory.Exists(rootPath + sDirectoryStructure + "\\"))
                                    Directory.CreateDirectory(rootPath + sDirectoryStructure + "\\");

                                using (FileStream stream = new FileStream(physicalPath, FileMode.Create))
                                {

                                    stream.Write(fileData, 0, fileData.Length);
                                    stream.Close();

                                }
                                i++;
                            }
                            if (!string.IsNullOrEmpty(physicalPath))
                            {
                                return Json(new { success = true, message = "File uploaded and Saved Successfully.", filePath = physicalPath });
                            }
                            else
                            {
                                return Json(new { success = true, message = "Unable to Encrypt the selected file.", filePath = "" });
                            }

                        }

                    }
                    else
                    {
                        return Json(new { success = false, message = "Unable to Encrypt the selected file." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Please select file to decrypt." });
                }
                return Json(new { success = false, message = "Unable to Encrypt the selected file." });



            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return Json(new { success = false, message = e.GetBaseException().Message });
            }
        }

        /// <summary>
        /// Returns true if is valid sql extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns></returns>
        private bool IsSQLValidExtension(string extension)
        {
            return (extension.Equals(".sql") || extension.Equals(".SQL"));
        }

        /// <summary>
        /// Returns true if file is of valid size
        /// </summary>
        /// <param name="ContentLength"></param>
        /// <returns></returns>
        private bool IsValidContentLength(int ContentLength)
        {
            // return ((ContentLength / 6000) / 6000) < 6;  //1 Mb

            try
            {

                return ContentLength <= (Convert.ToInt32(MaxFileSizeToUploadFileForEncryption));
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// POST call to Encrypt SQL Patch File
        /// </summary>
        /// <param name="sqlFilePath"></param>
        /// <param name="TicketNumber"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Encrypt SQL Patch File")]

        public ActionResult EncryptSQLPatchFile(string sqlFilePath, string TicketNumber)
        {
            try
            {
                AppDeveloperViewModel viewModel = new AppDeveloperViewModel();


                string errorMessage = string.Empty;
                ServiceCaller caller = new ServiceCaller("KaveriSupportApiController");
                viewModel.IsUploadedSuccessfully = false;
                viewModel.TicketNumber = TicketNumber;
                viewModel.Filepath = sqlFilePath;
                viewModel.EcryptedPatchFilePath = GetDirectoryStructure(2, TicketNumber);



                if (string.IsNullOrEmpty(TicketNumber))
                    return Json(new { success = false, message = "Ticket number is required" });

                if (TicketNumber.Length > 8)
                    return Json(new { success = false, message = "Ticket Number should be less than or equal to 8 digits." });


                if (!AllNumeric(TicketNumber))
                    return Json(new { success = false, message = "Invalid ticket number , Please enter valid ticket number." });

                if (TicketNumber.FirstOrDefault() == '0')
                    return Json(new { success = false, message = "Invalid ticket number (Ticket number shouldn't start with 0) , Please enter valid ticket number." });


                string result = caller.PostCall<AppDeveloperViewModel, string>("IsTicketExists", viewModel, out errorMessage);

                if (!string.IsNullOrEmpty(errorMessage))
                    return Json(new { success = false, message = errorMessage });

                if (!string.IsNullOrEmpty(result))
                    return Json(new { success = false, message = result });

                AppDeveloperViewModel responseModel = caller.PostCall<AppDeveloperViewModel, AppDeveloperViewModel>("EncryptSQLPatchFile", viewModel, out errorMessage);


                if (!String.IsNullOrEmpty(errorMessage))
                    return Json(new { success = false, message = errorMessage });


                if (!string.IsNullOrEmpty(responseModel.ResponseMessage))
                    return Json(new { success = true, message = responseModel.ResponseMessage });
                else
                    return Json(new { success = false, message = "Failed to encrypt and Save Zip file." });

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return Json(new { success = false, message = e.GetBaseException().Message });

            }
        }


        #endregion

        #region Listing Ticket Details

        [HttpGet]
        public ActionResult TicketDetailsList()
        {
            try
            {

                return View();

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = e.GetBaseException().Message, URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// Call to get list of Ticket details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadTicketDetailsList()
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

                ServiceCaller caller = new ServiceCaller("KaveriSupportApiController");
                // short userLoggedInRole = KaveriSession.Current.RoleID;
                TicketDetailsListModel response = caller.GetCall<TicketDetailsListModel>("LoadTicketDetailsList");




                if (response.TicketDetailsList == null)
                {
                    var emptyData = Json(new
                    {
                        draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No record found."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //if (!string.IsNullOrEmpty(response.errorMessage))
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = response.errorMessage, URLToRedirect = "/KaveriSupport/KaveriSupport/KaveriSupport" });
                //}





                //Sorting            
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    response.TicketDetailsList = response.TicketDetailsList.AsEnumerable().OrderBy(sortColumn + " " + sortColumnDir).ToList();
                //}




                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    response.TicketDetailsList = response.TicketDetailsList.Where(m => m.TicketNumber.ToLower().Contains(searchValue.ToLower())).ToList();
                }

                //total number of rows count     
                recordsTotal = response.TicketDetailsList.Count();
                //Paging     
                var data = response.TicketDetailsList.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data   
                //return Json(data: data); 
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
                    errorMessage = e.GetBaseException().Message
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }

        }

        /// <summary>
        /// Call to get private key details
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadPrivateKeyDetailsList()
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

                ServiceCaller caller = new ServiceCaller("KaveriSupportApiController");
                // short userLoggedInRole = KaveriSession.Current.RoleID;
                TicketDetailsListModel response = caller.GetCall<TicketDetailsListModel>("LoadPrivateKeyDetailsList");


                // ADDED BY SHUBHAM BHAGAT ON 15-05-2019              
                if (response.PrivateKeyDetailsList == null)
                {
                    var emptyData = Json(new
                    {
                        draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No data found."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //if (!string.IsNullOrEmpty(response.errorMessage))
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = response.errorMessage, URLToRedirect = "/KaveriSupport/KaveriSupport/KaveriSupport" });
                //}


                //Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    response.TicketDetailsList = response.TicketDetailsList.OrderBy < (sortColumn + " " + sortColumnDir);
                //}






                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    response.PrivateKeyDetailsList = response.PrivateKeyDetailsList.Where(m => m.TicketNumber.ToLower().Contains(searchValue.ToLower())).ToList();
                }

                //total number of rows count     
                recordsTotal = response.PrivateKeyDetailsList.Count();
                //Paging     
                var data = response.PrivateKeyDetailsList.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data   
                //return Json(data: data); 
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
                    errorMessage = e.GetBaseException().Message
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }

        }
        #endregion


        #endregion
    }
}