using CustomModels.Models.SROScriptManager;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CustomModels.Models.Common;
using OfficeOpenXml;
using OfficeOpenXml.Style;



namespace ECDataUI.Areas.SROScriptManager.Controllers
{
    [KaveriAuthorizationAttribute]
    public class SROScriptManagerController : Controller
    {
        ServiceCaller caller = null;

        // SRO Script Manager

        /// <summary>
        ///SRO ScriptManager View
        /// </summary>
        /// <returns>returns view containing SRO ScriptManager Model.</returns>
        [MenuHighlight]
        public ActionResult SROScriptManagerView()
        {
            try
            {
                SROScriptManagerModel model = new SROScriptManagerModel();

                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.InsertScriptManagerDetails;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("SROScriptManagerAPIController");

                //model = caller.GetCall<SROScriptManagerModel>("SROScriptManagerView", new { OfficeID = OfficeID });

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving SRO ScriptManager View", URLToRedirect = "/Home/HomePage" });
            }
        }

        //Changed by Omkar on 24/11/2020
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult InsertScriptManager(SROScriptManagerModel viewModel)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
            viewModel.IsInsertedSuccessfully = false;

            try
            {
                var regex = @"([#$%<>])";
                #region Validations

                if (string.IsNullOrEmpty(viewModel.ScriptDescription))
                    return Json(new { success = false, message = "Script description is required" });

                if (!string.IsNullOrEmpty(viewModel.ScriptDescription))
                {
                    Match match = Regex.Match(viewModel.ScriptDescription, regex, RegexOptions.IgnoreCase);
                    if (match.Success)
                        return Json(new { success = false, message = "These $ # % < > characters are not allowed in Script description" });
                }

                #endregion

                if (ModelState.IsValid)
                {

                    SROScriptManagerModel responseModel = caller.PostCall<SROScriptManagerModel, SROScriptManagerModel>("InsertInScriptManagerDetails", viewModel, out errorMessage);
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
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }

        [MenuHighlight]
        public ActionResult SROScriptManagerDetailsView()
        {
            try
            {
                SROScriptManagerModel model = new SROScriptManagerModel();

                // KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ScriptManagerDetails;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("SROScriptManagerAPIController");

                //model = caller.GetCall<SROScriptManagerModel>("SROScriptManagerView", new { OfficeID = OfficeID });

                return View("EditSROScriptManager", model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving SRO ScriptManager View", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// Get SRO Office List By District ID
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <returns>returns SRO Office list</returns>
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                string errormessage = string.Empty;
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Load SRO ScriptManager Table
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns json data containing SRO ScriptManager Details.</returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadScriptManagerTable(FormCollection formCollection)
        {
            caller = new ServiceCaller("SROScriptManagerAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects       

                string ServicePack = formCollection["ServicePack"];
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match(searchValue);

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;

                #region Server Side Validation
                if (string.IsNullOrEmpty(ServicePack))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please enter the Service Pack."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                Regex regx1 = new Regex(@"^(?:(\d+)\.)?(?:(\d+)\.)?(\*|\d+)$");
                Match mtchDistrict = regx1.Match(ServicePack);
                if (!mtchDistrict.Success)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Any District."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //if (DroID < 0)
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select Any District.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

                #endregion

                SROScriptManagerModel reqModel = new SROScriptManagerModel();
                reqModel.ServicePackNumber = ServicePack;
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;

                // total Count is fetched in same call 
                int totalCount = 0;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.IsForSearch = true;
                }

                ScriptManagerDetailWrapperModel responseModel = caller.PostCall<SROScriptManagerModel, ScriptManagerDetailWrapperModel>("LoadScriptManagerTable", reqModel, out errorMessage);
                totalCount = responseModel.TotalCount;
                IEnumerable<ScriptManagerDetailModel> result = responseModel.ScriptManagerDetailList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting script manager details." });
                }


                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
                    {
                        if (!string.IsNullOrEmpty(searchValue))
                        {
                            var emptyData = Json(new
                            {
                                draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }
                    else
                    {
                        //Added by Omkar on 10-08-2020
                        result = result.Where(m =>

                        //Added by Omkar on 04092020

                        m.ID.ToLower().Contains(searchValue.ToLower()) ||

                        m.Script.ToLower().Contains(searchValue.ToLower()) ||
                        m.Description.ToLower().Contains(searchValue.ToLower()) ||
                        m.ServicePack.ToLower().Contains(searchValue.ToLower()) ||
                        m.DateOfScript.ToString().Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                //var gridData = new List<String>();
                var gridData = result.Select(ScriptManagerDetailModel => new
                {
                    //Added by Omkar on 04092020

                    ScriptID = ScriptManagerDetailModel.ID,

                    SerialNo = ScriptManagerDetailModel.SerialNo,
                    Script = ScriptManagerDetailModel.Script,
                    ServicePack = ScriptManagerDetailModel.ServicePack,
                    Description = ScriptManagerDetailModel.Description,
                    DateOfScript = ScriptManagerDetailModel.DateOfScript,
                    IsActive = ScriptManagerDetailModel.IsActive,
                    Action = ScriptManagerDetailModel.Action
                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + ServicePack + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while getting SRO Script Manager details."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }

        /// <summary>
        /// POST Call to upload  patch file
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Upload SQL Patch File")]

        public ActionResult UploadSQLScriptFile()
        {
            try
            {
                SROScriptManagerModel viewModel = new SROScriptManagerModel();
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
                            //if (!IsValidContentLength(FileBase.ContentLength))
                            //{
                            //    return Json(new { success = false, message = "File size should be less than 6 MB. Kindly upload file again." }, JsonRequestBehavior.AllowGet);
                            //}
                        }

                        foreach (string sFileName in Request.Files)
                        {
                            FileBase = Request.Files[sFileName];

                            FileName = FileBase.FileName.Split('.')[0];

                            byte[] fileData = null;
                            string FileDataStr = string.Empty;
                            using (var binaryReader = new BinaryReader(Request.Files[sFileName].InputStream))
                            {
                                fileData = binaryReader.ReadBytes(Request.Files[sFileName].ContentLength);

                                //FileDataStr =  ReadNullTerminatedString(binaryReader);
                            }

                            // FileDataStr = Encoding.ASCII.GetString(fileData);


                            // changed below line by Omkar on 07-09-2020

                            // FileDataStr = Encoding.Default.GetString(fileData);

                            FileDataStr = Encoding.UTF8.GetString(fileData);


                            // changed above line by Omkar on 07-09-2020



                            if (!string.IsNullOrEmpty(FileDataStr))
                            {
                                //return Json(new { success = true, message = "File uploaded and Saved Successfully.", FileDataStr = FileDataStr });

                                // Changed on 12-08-2020 by Omkar
                                var jsonResult = Json(new { success = true, message = "File uploaded and Saved Successfully.", FileDataStr = FileDataStr });
                                jsonResult.MaxJsonLength = int.MaxValue;
                                return jsonResult;
                            }
                            else
                            {
                                return Json(new { success = false, message = "Unable to Encrypt the selected file.", FileDataStr = "" });
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
                    return Json(new { success = true, message = "", FileDataStr = "" });
                }
                return Json(new { success = false, message = "Unable to Encrypt the selected file." });



            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return Json(new { success = false, message = e.GetBaseException().Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult EditScriptsByID(int ScriptID)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");

            try
            {
                SROScriptManagerModel reqModel = new SROScriptManagerModel();
                reqModel.ScriptID = ScriptID;
                SROScriptManagerModel responseModel = caller.PostCall<SROScriptManagerModel, SROScriptManagerModel>("GetScriptDetails", reqModel, out errorMessage);
                if (!String.IsNullOrEmpty(errorMessage))
                    return Json(new { success = false, message = errorMessage });


                if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                {
                    return Json(new { success = false, message = responseModel.ErrorMessage });

                }
                else
                {
                    return PartialView("EditScripts", responseModel);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }

        public ActionResult DownLoadScriptFile(int ScriptID)
        {
            try
            {
                string errorMessage = string.Empty;
                ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
                string enclosureFilePath = string.Empty;
                string fileName = "ScriptManger_" + ScriptID + ".sql";
                #region Decrypting FilePath
                //Dictionary<string, string> decryptedParameters = null;
                //string[] tempAppId = Path.Split('/');
                //decryptedParameters = URLEncrypt.DecryptParameters(new String[] { tempAppId[0], tempAppId[1], tempAppId[2] });
                //enclosureFilePath = Convert.ToString(decryptedParameters["EnclosureFilePath"]);
                //if (string.IsNullOrEmpty(enclosureFilePath))
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting FilePath for downloading Enclosure.", URLToRedirect = "/Home/HomePage" });
                //}
                #endregion

                //decryptedParameters = null;
                //tempAppId = FileName.Split('/');
                //decryptedParameters = URLEncrypt.DecryptParameters(new String[] { tempAppId[0], tempAppId[1], tempAppId[2] });
                //fileName = Convert.ToString(decryptedParameters["EnclosureFilePath"]);
                //if (string.IsNullOrEmpty(fileName))
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting FilePath for downloading Enclosure.", URLToRedirect = "/Home/HomePage" });
                //}

                //SupportEnclosureDetailsModel model = new SupportEnclosureDetailsModel();
                //model.FilePath = enclosureFilePath;
                //model.FileName = fileName;


                SROScriptManagerModel reqModel = new SROScriptManagerModel();
                reqModel.ScriptID = ScriptID;
                SROScriptManagerModel responseModel = caller.PostCall<SROScriptManagerModel, SROScriptManagerModel>("GetScriptDetails", reqModel, out errorMessage);

                if (!String.IsNullOrEmpty(errorMessage))
                    return Json(new { success = false, message = errorMessage });


                // changed below line by Omkar on 07-09-2020


                // byte[] FileContent = Encoding.ASCII.GetBytes(responseModel.ScriptContent);

                //byte[] FileContent = Encoding.Default.GetBytes(responseModel.ScriptContent);

                byte[] FileContent = Encoding.UTF8.GetBytes(responseModel.ScriptContent);


                // changed above line by Omkar on 07-09-2020


                if (responseModel == null || !string.IsNullOrEmpty(errorMessage))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file from Server.", URLToRedirect = "/Home/HomePage" });
                }
                else
                {
                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    return File(FileContent, "text/plain");
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
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

        public string ReadNullTerminatedString(BinaryReader stream)
        {
            string str = "";
            char ch;
            while ((int)(ch = stream.ReadChar()) != 0)
                str = str + ch;
            return str;
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateScriptManager(SROScriptManagerModel viewModel)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
            viewModel.IsInsertedSuccessfully = false;

            try
            {
                var regex = @"([#$%<>])";
                #region Validations

                if (string.IsNullOrEmpty(viewModel.ScriptDescription))
                    return Json(new { success = false, message = "Script description is required" });

                if (!string.IsNullOrEmpty(viewModel.ScriptDescription))
                {
                    Match match = Regex.Match(viewModel.ScriptDescription, regex, RegexOptions.IgnoreCase);
                    if (match.Success)
                        return Json(new { success = false, message = "These $ # % < > characters are not allowed in Script description" });
                }

                #endregion

                if (ModelState.IsValid)
                {

                    SROScriptManagerModel responseModel = caller.PostCall<SROScriptManagerModel, SROScriptManagerModel>("UpdateInScriptManagerDetails", viewModel, out errorMessage);
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
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }


        // Application Version
        [MenuHighlight]
        public ActionResult AppVersionView()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.AppVersionDetails;
                caller = new ServiceCaller("SROScriptManagerAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                AppVersionDetailsModel reqModel = caller.GetCall<AppVersionDetailsModel>("AppVersionDetails", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while rescanning summary Details View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving SRO ScriptManager View", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadAppVersionDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("SROScriptManagerAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects       

                string OfficeTypeID = formCollection["OfficeTypeID"];
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match(searchValue);

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;

                #region Server Side Validation
                if (string.IsNullOrEmpty(OfficeTypeID))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select the Office Type for loading the App version details."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                AppVersionDetailsModel reqModel = new AppVersionDetailsModel();
                reqModel.IsDROOffice = OfficeTypeID == "10" ? false : (OfficeTypeID == "20" ? true : false);
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;

                // total Count is fetched in same call 
                int totalCount = 0;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.IsForSearch = true;
                }


                AppVersionDetailWrapperModel responseModel = caller.PostCall<AppVersionDetailsModel, AppVersionDetailWrapperModel>("LoadAppVersionDetails", reqModel, out errorMessage);
                totalCount = responseModel.TotalCount;
                IEnumerable<AppVersionDetaillstModel> result = responseModel.appVersionDetailList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting script manager details." });
                }


                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
                    {
                        if (!string.IsNullOrEmpty(searchValue))
                        {
                            var emptyData = Json(new
                            {
                                draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }
                    else
                    {
                        result = result.Where(m =>
                        m.AppName.ToLower().Contains(searchValue.ToLower()) ||
                        m.AppMajor.ToLower().Contains(searchValue.ToLower()) ||
                        m.AppMinor.ToLower().Contains(searchValue.ToLower()) ||
                        m.ReleaseDate.ToString().Contains(searchValue.ToLower()) ||
                        m.LastDateForPatch.Contains(searchValue.ToLower()) ||
                        m.DROOfficeName.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROOfficeName.ToLower().Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                //var gridData = new List<String>();
                var gridData = result.Select(AppVersionDetaillstModel => new
                {
                    SerialNo = AppVersionDetaillstModel.SerialNo,
                    AppName = AppVersionDetaillstModel.AppName,
                    AppMajor = AppVersionDetaillstModel.AppMajor,
                    AppMinor = AppVersionDetaillstModel.AppMinor,
                    SROId = AppVersionDetaillstModel.SROId,
                    SROOfficeName = AppVersionDetaillstModel.SROOfficeName,
                    ReleaseDate = AppVersionDetaillstModel.ReleaseDate,
                    LastDateForPatch = AppVersionDetaillstModel.LastDateForPatch,
                    IsDROOffice = AppVersionDetaillstModel.IsDROOffice,
                    DROId = AppVersionDetaillstModel.DROId,
                    DROOfficeName = AppVersionDetaillstModel.DROOfficeName,
                    SPExecutionDate = AppVersionDetaillstModel.SPExecutionDate,
                    Action = AppVersionDetaillstModel.Action
                });

                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + ServicePack + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = totalCount,
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = totalCount
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while getting SRO Script Manager details."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult AddAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
            viewModel.IsInsertedSuccessfully = false;

            try
            {
                var regex = @"([#$%<>])";
                #region Validations

                if (string.IsNullOrEmpty(viewModel.AppName))
                    return Json(new { success = false, message = "Application Name is required" });

                Match match = Regex.Match(viewModel.AppName, regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in Application Name" });

                if (viewModel.AppMajor == 0)
                    return Json(new { success = false, message = "App Major is required" });
                // Changed by Omkar on 11/11/2020
                //if (viewModel.AppMinor == 0)
                // return Json(new { success = false, message = "AppMinor is required" });

                match = Regex.Match(viewModel.AppMajor.ToString(), regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in AppMajor" });
                match = Regex.Match(viewModel.AppMinor.ToString(), regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in AppMinor" });

                regex = @"^[0-9]*$";

                match = Regex.Match(viewModel.AppMajor.ToString(), regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                    return Json(new { success = false, message = "Invalid App Major" });
                match = Regex.Match(viewModel.AppMinor.ToString(), regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                    return Json(new { success = false, message = "Invalid App Minor" });


                #endregion

                #region Remove ModelState Properties
                if (viewModel.IsDROOfficestr == "1")
                {
                    viewModel.IsDROOffice = false;   //SR Office
                    ModelState.Remove("SPExecutionDateTime");
                }
                else
                {
                    viewModel.IsDROOffice = true;  //DR Office
                    ModelState.Remove("SROfficeID");
                }
                #endregion

                if (ModelState.IsValid)
                {

                    AppVersionDetailsModel responseModel = caller.PostCall<AppVersionDetailsModel, AppVersionDetailsModel>("AddAppVersionDetails", viewModel, out errorMessage);
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
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult EditAppVersionDetailsByAppName(int VersionID, int OfficeCode, string IDO)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");

            try
            {
                // AppVersionDetailsModel reqModel = new AppVersionDetailsModel();


                int OfficeID = KaveriSession.Current.OfficeID;
                AppVersionDetailsModel reqModel = caller.GetCall<AppVersionDetailsModel>("AppVersionDetails", new { OfficeID = OfficeID });


                //reqModel.AppName = AppName;
                reqModel.VersionID = VersionID;
                if (IDO.ToLower() == "y")
                {
                    reqModel.IsDROOffice = true;
                    reqModel.DROfficeID = OfficeCode;
                }
                else if (IDO.ToLower() == "n")
                {
                    reqModel.IsDROOffice = false;
                    reqModel.SROfficeID = OfficeCode;
                }
                else
                    reqModel.IsDROOffice = false;

                AppVersionDetailsModel responseModel = caller.PostCall<AppVersionDetailsModel, AppVersionDetailsModel>("GetAppversionDetails", reqModel, out errorMessage);

                responseModel.SROfficeList = reqModel.SROfficeList;
                responseModel.DROfficeList = reqModel.DROfficeList;

                if (!String.IsNullOrEmpty(errorMessage))
                    return Json(new { success = false, message = errorMessage });


                if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                {
                    return Json(new { success = false, message = responseModel.ErrorMessage });

                }
                else
                {
                    return PartialView("EditAppVersion", responseModel);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateAppVersionDetails(AppVersionDetailsModel viewModel)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
            viewModel.IsInsertedSuccessfully = false;

            try
            {
                var regex = @"([#$%<>])";
                #region Validations

                if (string.IsNullOrEmpty(viewModel.AppName))
                    return Json(new { success = false, message = "Application Name is required" });

                Match match = Regex.Match(viewModel.AppName, regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in Application Name" });

                if (viewModel.AppMajor == 0)
                    return Json(new { success = false, message = "App Major is required" });
                // Changed by Omkar on 11/11/2020
                //if (viewModel.AppMinor == 0)
                // return Json(new { success = false, message = "AppMinor is required" });

                match = Regex.Match(viewModel.AppMajor.ToString(), regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in AppMajor" });
                match = Regex.Match(viewModel.AppMinor.ToString(), regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in AppMinor" });

                regex = @"^[0-9]*$";

                match = Regex.Match(viewModel.AppMajor.ToString(), regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                    return Json(new { success = false, message = "Invalid App Major" });
                match = Regex.Match(viewModel.AppMinor.ToString(), regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                    return Json(new { success = false, message = "Invalid App Minor" });


                #endregion

                #region Remove ModelState Properties
                if (viewModel.IsDROOfficestr == "11")
                {
                    viewModel.IsDROOffice = false;   //SR Office
                    ModelState.Remove("SPExecutionDateTime");
                }
                else if (viewModel.IsDROOfficestr == "22")
                {
                    viewModel.IsDROOffice = true;  //DR Office
                    ModelState.Remove("SROfficeID");
                }
                else
                {
                    return Json(new { success = false, message = "Error Occured while fetching office type." });

                }
                #endregion

                if (ModelState.IsValid)
                {

                    AppVersionDetailsModel responseModel = caller.PostCall<AppVersionDetailsModel, AppVersionDetailsModel>("UpdateAppVersionDetails", viewModel, out errorMessage);
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
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }


        // DRO Scripts Manager

        [MenuHighlight]
        /// <summary>
        ///SRO ScriptManager View
        /// </summary>
        /// <returns>returns view containing SRO ScriptManager Model.</returns>
        public ActionResult DROScriptManagerView()
        {
            try
            {
                DROScriptManagerModel model = new DROScriptManagerModel();

                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DROScriptsDetails;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("SROScriptManagerAPIController");

                //model = caller.GetCall<SROScriptManagerModel>("SROScriptManagerView", new { OfficeID = OfficeID });

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving SRO ScriptManager View", URLToRedirect = "/Home/HomePage" });
            }
        }

        //[HttpPost]        
        [HttpPost, ValidateInput(false)]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult AddDROScriptManager(DROScriptManagerModel viewModel)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
            viewModel.IsInsertedSuccessfully = false;

            try
            {
                var regex = @"([#$%<>])";
                #region Validations

                if (string.IsNullOrEmpty(viewModel.ScriptDescription))
                    return Json(new { success = false, message = "Script description is required" });

                if (!string.IsNullOrEmpty(viewModel.ScriptDescription))
                {
                    Match match = Regex.Match(viewModel.ScriptDescription, regex, RegexOptions.IgnoreCase);
                    if (match.Success)
                        return Json(new { success = false, message = "These $ # % < > characters are not allowed in Script description" });
                }

                #endregion

                if (ModelState.IsValid)
                {

                    DROScriptManagerModel responseModel = caller.PostCall<DROScriptManagerModel, DROScriptManagerModel>("InsertInDROScriptManagerDetails", viewModel, out errorMessage);
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
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadDROScriptManagerTable(FormCollection formCollection)
        {
            caller = new ServiceCaller("SROScriptManagerAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects       

                string ServicePack = formCollection["ServicePack"];
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match(searchValue);

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;

                #region Server Side Validation
                if (string.IsNullOrEmpty(ServicePack))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please enter the Service Pack."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                Regex regx1 = new Regex(@"^(?:(\d+)\.)?(?:(\d+)\.)?(\*|\d+)$");
                Match mtchDistrict = regx1.Match(ServicePack);
                if (!mtchDistrict.Success)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Any District."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //if (DroID < 0)
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select Any District.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

                #endregion

                DROScriptManagerModel reqModel = new DROScriptManagerModel();
                reqModel.ServicePackNumber = ServicePack;
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;

                // total Count is fetched in same call 
                int totalCount = 0;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.IsForSearch = true;
                }

                DROScriptManagerDetailWrapperModel responseModel = caller.PostCall<DROScriptManagerModel, DROScriptManagerDetailWrapperModel>("LoadDROScriptManagerTable", reqModel, out errorMessage);
                totalCount = responseModel.TotalCount;
                IEnumerable<DROScriptManagerDetailModel> result = responseModel.ScriptManagerDetailList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting script manager details." });
                }


                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
                    {
                        if (!string.IsNullOrEmpty(searchValue))
                        {
                            var emptyData = Json(new
                            {
                                draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }
                    else
                    {
                        //Added by Omkar on 10-08-2020

                        result = result.Where(m =>

                        //Added by Omkar on 04092020
                          m.ID.ToLower().Contains(searchValue.ToLower()) ||

                        m.Script.ToLower().Contains(searchValue.ToLower()) ||
                        m.Description.ToLower().Contains(searchValue.ToLower()) ||
                        m.ServicePack.ToLower().Contains(searchValue.ToLower()) ||
                        m.DateOfScript.ToString().Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                //var gridData = new List<String>();
                var gridData = result.Select(ScriptManagerDetailModel => new
                {
                    SerialNo = ScriptManagerDetailModel.SerialNo,
                    //Added by Omkar on 04092020

                    ScriptID = ScriptManagerDetailModel.ID,


                    Script = ScriptManagerDetailModel.Script,
                    ServicePack = ScriptManagerDetailModel.ServicePack,
                    Description = ScriptManagerDetailModel.Description,
                    DateOfScript = ScriptManagerDetailModel.DateOfScript,
                    IsActive = ScriptManagerDetailModel.IsActive,
                    Action = ScriptManagerDetailModel.Action
                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + ServicePack + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while getting SRO Script Manager details."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult EditDROScriptsByID(int ScriptID)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");

            try
            {
                DROScriptManagerModel reqModel = new DROScriptManagerModel();
                reqModel.ScriptID = ScriptID;
                DROScriptManagerModel responseModel = caller.PostCall<DROScriptManagerModel, DROScriptManagerModel>("GetDROScriptDetails", reqModel, out errorMessage);
                if (!String.IsNullOrEmpty(errorMessage))
                    return Json(new { success = false, message = errorMessage });


                if (!string.IsNullOrEmpty(responseModel.ErrorMessage))
                {
                    return Json(new { success = false, message = responseModel.ErrorMessage });

                }
                else
                {
                    return PartialView("EditDROScripts", responseModel);
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }

        public ActionResult DownLoadDROScriptFile(int ScriptID)
        {
            try
            {
                string errorMessage = string.Empty;
                ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
                string enclosureFilePath = string.Empty;
                string fileName = "ScriptManger_" + ScriptID + ".sql";
                #region Decrypting FilePath
                //Dictionary<string, string> decryptedParameters = null;
                //string[] tempAppId = Path.Split('/');
                //decryptedParameters = URLEncrypt.DecryptParameters(new String[] { tempAppId[0], tempAppId[1], tempAppId[2] });
                //enclosureFilePath = Convert.ToString(decryptedParameters["EnclosureFilePath"]);
                //if (string.IsNullOrEmpty(enclosureFilePath))
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting FilePath for downloading Enclosure.", URLToRedirect = "/Home/HomePage" });
                //}
                #endregion

                //decryptedParameters = null;
                //tempAppId = FileName.Split('/');
                //decryptedParameters = URLEncrypt.DecryptParameters(new String[] { tempAppId[0], tempAppId[1], tempAppId[2] });
                //fileName = Convert.ToString(decryptedParameters["EnclosureFilePath"]);
                //if (string.IsNullOrEmpty(fileName))
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting FilePath for downloading Enclosure.", URLToRedirect = "/Home/HomePage" });
                //}

                //SupportEnclosureDetailsModel model = new SupportEnclosureDetailsModel();
                //model.FilePath = enclosureFilePath;
                //model.FileName = fileName;


                DROScriptManagerModel reqModel = new DROScriptManagerModel();
                reqModel.ScriptID = ScriptID;
                DROScriptManagerModel responseModel = caller.PostCall<DROScriptManagerModel, DROScriptManagerModel>("GetDROScriptDetails", reqModel, out errorMessage);

                if (!String.IsNullOrEmpty(errorMessage))
                    return Json(new { success = false, message = errorMessage });

                //byte[] FileContent = Encoding.ASCII.GetBytes(responseModel.ScriptContent);

                // BELOW CODE IS COMMENTED AND CHANGED BY OMKAR ON 28-12-2020
                               
                //byte[] FileContent = Encoding.Default.GetBytes(responseModel.ScriptContent);

                byte[] FileContent = Encoding.UTF8.GetBytes(responseModel.ScriptContent);
         
                // ABOVE CODE IS COMMENTED AND CHANGED BY OMKAR ON 28-12-2020


                if (responseModel == null || !string.IsNullOrEmpty(errorMessage))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file from Server.", URLToRedirect = "/Home/HomePage" });
                }
                else
                {
                    HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
                    return File(FileContent, "text/plain");
                }
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateDROScriptManager(DROScriptManagerModel viewModel)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
            viewModel.IsInsertedSuccessfully = false;

            try
            {
                var regex = @"([#$%<>])";
                #region Validations

                if (string.IsNullOrEmpty(viewModel.ScriptDescription))
                    return Json(new { success = false, message = "Script description is required" });

                if (!string.IsNullOrEmpty(viewModel.ScriptDescription))
                {
                    Match match = Regex.Match(viewModel.ScriptDescription, regex, RegexOptions.IgnoreCase);
                    if (match.Success)
                        return Json(new { success = false, message = "These $ # % < > characters are not allowed in Script description" });
                }

                #endregion

                if (ModelState.IsValid)
                {

                    DROScriptManagerModel responseModel = caller.PostCall<DROScriptManagerModel, DROScriptManagerModel>("UpdateInDROScriptManagerDetails", viewModel, out errorMessage);
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
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }


        // Apply Application Version
        [MenuHighlight]
        public ActionResult ApplyAppVersionView()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ApplyAppVersion;
                caller = new ServiceCaller("SROScriptManagerAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                // ApplyAppVersionModel reqModel = caller.GetCall<ApplyAppVersionModel>("ApplyAppVersionView", new { OfficeID = OfficeID });

                ApplyAppVersionModel reqModel = caller.GetCall<ApplyAppVersionModel>("ApplyAppVersionView");
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Apply App Version View", URLToRedirect = "/Home/HomePage" });
            }
        }


        [HttpPost]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SRDRList()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ApplyAppVersion;
                caller = new ServiceCaller("SROScriptManagerAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                // ApplyAppVersionModel reqModel = caller.GetCall<ApplyAppVersionModel>("ApplyAppVersionView", new { OfficeID = OfficeID });

                // var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                var searchValue = "";
                Regex regx = new Regex("/^[^<>] +$/");

                Match mtch = regx.Match(searchValue);

                ApplyAppVersionModel reqModel = caller.GetCall<ApplyAppVersionModel>("SRDRList");

                //   totalCount = reqModel.TotalCount;
                IEnumerable<ApplyAppVersionDetaillstModel> result = reqModel.ApplyAppVersionViewList;


                if (searchValue != null && searchValue != "")
                {
                    reqModel.IsForSearch = true;
                }

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading View", URLToRedirect = "/Home/HomePage" });

                }



                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
                    {
                        if (!string.IsNullOrEmpty(searchValue))
                        {
                            var emptyData = Json(new
                            {
                                //draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }
                    else
                    {
                        result = result.Where(m =>
                        //  m.AppName.ToLower().Contains(searchValue.ToLower()) ||
                        //  m.AppMajor.Contains(searchValue) ||
                        //  m.AppMinor.Contains(searchValue) ||
                        m.ReleaseDate.ToString().Contains(searchValue) ||
                        m.LastDateForPatch.ToString().Contains(searchValue) ||
                        m.SROOfficeName.ToLower().Contains(searchValue) ||
                        m.DROOfficeName.ToLower().Contains(searchValue)
                        );

                        // totalCount = result.Count();
                    }
                }

                var gridData = result.Select(ApplyAppVersionDetaillstModel => new
                {
                    SROId = ApplyAppVersionDetaillstModel.SROId,
                    SROOfficeName = ApplyAppVersionDetaillstModel.SROOfficeName,
                    DROOfficeName = ApplyAppVersionDetaillstModel.DROOfficeName,
                });


                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        //  draw = formCollection["draw"],
                        data = gridData.ToArray().ToList(),
                        // recordsTotal = totalCount,
                        //status = "1",
                        //recordsFiltered = totalCount,
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    }, JsonRequestBehavior.AllowGet);
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        // draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        //  recordsTotal = totalCount,
                        //status = "1",
                        // recordsFiltered = totalCount
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    }, JsonRequestBehavior.AllowGet);
                    JsonData.MaxJsonLength = Int32.MaxValue;


                    return JsonData;
                }
                // return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Apply App Version View", URLToRedirect = "/Home/HomePage" });
            }

        }

        [HttpPost]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult DRList()
        {
            try
            {
                //  KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ApplyAppVersion;
                caller = new ServiceCaller("SROScriptManagerAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                // ApplyAppVersionModel reqModel = caller.GetCall<ApplyAppVersionModel>("ApplyAppVersionView", new { OfficeID = OfficeID });

                // var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                var searchValue = "";
                Regex regx = new Regex("/^[^<>] +$/");

                Match mtch = regx.Match(searchValue);

                ApplyAppVersionModel reqModel = caller.GetCall<ApplyAppVersionModel>("DRList");

                //   totalCount = reqModel.TotalCount;
                IEnumerable<ApplyAppVersionDetaillstModel> result = reqModel.ApplyAppVersionViewList;


                if (searchValue != null && searchValue != "")
                {
                    reqModel.IsForSearch = true;
                }

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while loading View", URLToRedirect = "/Home/HomePage" });

                }



                if (!string.IsNullOrEmpty(searchValue))
                {
                    if (mtch.Success)
                    {
                        if (!string.IsNullOrEmpty(searchValue))
                        {
                            var emptyData = Json(new
                            {
                                //draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }
                    else
                    {
                        result = result.Where(m =>
                        //  m.AppName.ToLower().Contains(searchValue.ToLower()) ||
                        //  m.AppMajor.Contains(searchValue) ||
                        //  m.AppMinor.Contains(searchValue) ||
                        //  m.ReleaseDate.ToString().Contains(searchValue) ||
                        //   m.LastDateForPatch.ToString().Contains(searchValue) ||
                        m.SROOfficeName.ToLower().Contains(searchValue) ||
                        m.DROOfficeName.ToLower().Contains(searchValue)
                        );

                        // totalCount = result.Count();
                    }
                }

                var gridData = result.Select(ApplyAppVersionDetaillstModel => new
                {
                    DROId = ApplyAppVersionDetaillstModel.DROId,
                    SROOfficeName = ApplyAppVersionDetaillstModel.SROOfficeName,
                    DROOfficeName = ApplyAppVersionDetaillstModel.DROOfficeName,
                });


                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        //  draw = formCollection["draw"],
                        data = gridData.ToArray().ToList(),
                        // recordsTotal = totalCount,
                        //status = "1",
                        //recordsFiltered = totalCount,
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    }, JsonRequestBehavior.AllowGet);
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        // draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        //  recordsTotal = totalCount,
                        //status = "1",
                        // recordsFiltered = totalCount
                        //ExcelDownloadBtn = ExcelDownloadBtn
                    }, JsonRequestBehavior.AllowGet);
                    JsonData.MaxJsonLength = Int32.MaxValue;


                    return JsonData;
                }
                // return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Apply App Version View", URLToRedirect = "/Home/HomePage" });
            }

        }


        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult ApplyAppVersion(FormCollection formCollection)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
            string appMajor = formCollection["txtAppMajor"];
            string appMinor = formCollection["txtAppMinor"];

            string appName = formCollection["txtAppName"];
            string releaseDate = formCollection["txtReleaseDate"];
            string lastDate = formCollection["txtLastDate"];
            string selectedOffice = formCollection["selectedOfc"];

            // string selectedSroCodes = selectedOffice.Replace("\n", ",");

            // var sroList = selectedSroCodes.ToList();

            ApplyAppVersionModel viewModel = new ApplyAppVersionModel();
            viewModel.AppMajor = Convert.ToInt32(appMajor);
            viewModel.AppMinor = Convert.ToInt32(appMinor);
            viewModel.AppName = appName;
            viewModel.ReleaseDate = Convert.ToDateTime(releaseDate);
            viewModel.LastDateForPatchUpdate = Convert.ToDateTime(lastDate);
            viewModel.SROfficeList = selectedOffice;



            try
            {
                var regex = @"([#$%<>])";
                #region Validations


                Match match = Regex.Match(appName, regex, RegexOptions.IgnoreCase);

                if (appMajor == "")
                    return Json(new { success = false, message = "App Major is required" });
                if (appMinor == "")
                    return Json(new { success = false, message = "AppMinor is required" });

                match = Regex.Match(appMajor, regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in AppMajor" });
                match = Regex.Match(appMinor, regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in AppMinor" });

                regex = @"^[0-9]*$";

                match = Regex.Match(appMajor, regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                    return Json(new { success = false, message = "Invalid App Major" });
                match = Regex.Match(appMinor, regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                    return Json(new { success = false, message = "Invalid App Minor" });


                #endregion


                if (ModelState.IsValid)
                {

                    ApplyAppVersionModel responseModel = caller.PostCall<ApplyAppVersionModel, ApplyAppVersionModel>("ApplyAppVersion", viewModel, out errorMessage);
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
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }

        //Added by Omkar on 09-09-2020
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult ApplyAppVersionDR(FormCollection formCollection)
        {
            string errorMessage = string.Empty;
            ServiceCaller caller = new ServiceCaller("SROScriptManagerAPIController");
            string appMajor = formCollection["txtAppMajor"];
            string appMinor = formCollection["txtAppMinor"];

            string appName = formCollection["txtAppName"];
            string releaseDate = formCollection["txtReleaseDate"];
            string lastDate = formCollection["txtLastDate"];
            string selectedOffice = formCollection["selectedOfc"];

            // string selectedSroCodes = selectedOffice.Replace("\n", ",");

            // var sroList = selectedSroCodes.ToList();

            ApplyAppVersionModel viewModel = new ApplyAppVersionModel();
            viewModel.AppMajor = Convert.ToInt32(appMajor);
            viewModel.AppMinor = Convert.ToInt32(appMinor);
            viewModel.AppName = appName;
            viewModel.ReleaseDate = Convert.ToDateTime(releaseDate);
            viewModel.LastDateForPatchUpdate = Convert.ToDateTime(lastDate);
            viewModel.DROfficeList = selectedOffice;



            try
            {
                var regex = @"([#$%<>])";
                #region Validations


                Match match = Regex.Match(appName, regex, RegexOptions.IgnoreCase);

                if (appMajor == "")
                    return Json(new { success = false, message = "App Major is required" });
                if (appMinor == "")
                    return Json(new { success = false, message = "AppMinor is required" });

                match = Regex.Match(appMajor, regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in AppMajor" });
                match = Regex.Match(appMinor, regex, RegexOptions.IgnoreCase);
                if (match.Success)
                    return Json(new { success = false, message = "These $ # % < > characters are not allowed in AppMinor" });

                regex = @"^[0-9]*$";

                match = Regex.Match(appMajor, regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                    return Json(new { success = false, message = "Invalid App Major" });
                match = Regex.Match(appMinor, regex, RegexOptions.IgnoreCase);
                if (!match.Success)
                    return Json(new { success = false, message = "Invalid App Minor" });


                #endregion


                if (ModelState.IsValid)
                {

                    ApplyAppVersionModel responseModel = caller.PostCall<ApplyAppVersionModel, ApplyAppVersionModel>("ApplyAppVersionDR", viewModel, out errorMessage);
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
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { success = false, message = ex.GetBaseException().Message });
            }
        }
    }



}



