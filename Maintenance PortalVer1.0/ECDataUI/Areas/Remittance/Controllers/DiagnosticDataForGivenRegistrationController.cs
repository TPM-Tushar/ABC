#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DiagnosticDataForGivenRegistrationController.cs
    * Author Name       :   Pankaj Sakhare 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.DiagnosticDataForGivenRegistration;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class DiagnosticDataForGivenRegistrationController : Controller
    {
        #region PROPERTIES
        private ServiceCaller caller = new ServiceCaller("DiagnosticDataForGivenRegistrationAPIController");
        #endregion

        /// <summary>
        /// DiagnosticDataForGivenRegistrationView
        /// </summary>
        /// <returns>view</returns>
        [HttpGet]
        [MenuHighlight]
        [EventAuditLogFilter(Description = "Diagnostic Data For Given Registration View")]
        public ActionResult DiagnosticDataForGivenRegistrationView()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.RemittanceXMLLog;
                int OfficeID = KaveriSession.Current.OfficeID;
                DiagnosticDataForRegistrationModel model = new DiagnosticDataForRegistrationModel();
                List<SelectListItem> statusListItem = new List<SelectListItem>();

                statusListItem.Add(new SelectListItem { Text = "Document Registration", Value = "D" });
                statusListItem.Add(new SelectListItem { Text = "Marriage Registration", Value = "M" });
                statusListItem.Add(new SelectListItem { Text = "Notice Registration", Value = "N" });

                model.RegistrationModuleList = statusListItem;
                //model = caller.GetCall<DiagnosticDataForRegistrationModel>("", new { OfficeID = OfficeID });

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// DownloadDiagnosticDataInsertScript
        /// </summary>
        /// <param name="RegistrationModuleCode"></param>
        /// <param name="FinalRegistrationNumber"></param>
        /// <returns>sql insert script file</returns>
        [EventAuditLogFilter(Description = "Download Diagnostic Data Insert Script")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult DownloadDiagnosticDataInsertScript(string RegistrationModuleCode, string FinalRegistrationNumber)
        {
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects 

                //int topRows = Convert.ToInt32(TopRows);

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                

                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });

                if (string.IsNullOrEmpty(RegistrationModuleCode.Trim()))
                {
                    var emptyData = Json(new
                    {
                        serverError = false,
                        data = "",
                        status = false,
                        errorMessage = "Registration Module is required",
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    emptyData.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(FinalRegistrationNumber.Trim()))
                {
                    var emptyData = Json(new
                    {
                        serverError = false,
                        data = "",
                        status = false,
                        errorMessage = "Final Registration Number is Required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    emptyData.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
                    return emptyData;
                }

                if (RegistrationModuleCode != "D" && RegistrationModuleCode != "M" && RegistrationModuleCode != "N")
                {
                    var emptyData = Json(new
                    {
                        serverError = false,
                        data = "",
                        status = false,
                        errorMessage = "Registration Module Code is Incorrect"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                System.Text.RegularExpressions.Regex regxForFRN = new Regex("^[a-zA-Z0-9- ]+$");              
                Match mtchFRN = regxForFRN.Match(FinalRegistrationNumber);
                if (!mtchFRN.Success)
                {
                    return Json(new { serverError = false, status = false, errorMessage = "Please enter valid Final Registration Number" }, JsonRequestBehavior.AllowGet);
                }



                DiagnosticDataForRegistrationModel reqModel = new DiagnosticDataForRegistrationModel();
                DownloadDiagnosticDataScript resModel = new DownloadDiagnosticDataScript();

                reqModel.RegistrationModuleCode = RegistrationModuleCode;
                reqModel.FinalRegistrationNumber = FinalRegistrationNumber;

                resModel = caller.PostCall<DiagnosticDataForRegistrationModel, DownloadDiagnosticDataScript>("DownloadDiagnosticDataInsertScript", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { serverError = true, errorMessage = "Error occured while getting Office Wise Diagnostic Status Details." }, JsonRequestBehavior.AllowGet);
                }
                else if (resModel.FileContent == null)
                {
                    return Json(new { serverError = false, status = false, errorMessage = "No Record Found." }, JsonRequestBehavior.AllowGet);
                }

                string filename = FinalRegistrationNumber + "_"+ DateTime.Now.ToString().Replace(' ', '_').Replace(':', '_') + ".sql";
                string ScriptFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", filename));
                System.IO.File.WriteAllLines(ScriptFilePath, resModel.FileContent);
                resModel.FileContentField = System.IO.File.ReadAllBytes(ScriptFilePath);
                objCommon.DeleteFileFromTemporaryFolder(ScriptFilePath);

                //var data = Json(new
                //{
                //    serverError = false,
                //    FileContent = resModel.FileContent,
                //    FileName = filename,
                //    JsonRequestBehavior.AllowGet,
                //});
                //data.MaxJsonLength = Int32.MaxValue;
                //return data;
                return File(resModel.FileContentField, System.Net.Mime.MediaTypeNames.Application.Octet, filename);


            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while Downloading Diagnostic Data Insert Script." }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}