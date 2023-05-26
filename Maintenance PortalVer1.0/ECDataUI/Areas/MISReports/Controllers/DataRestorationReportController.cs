#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DataRestorationReportController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   MVC Controller for MIS Reports module.
	* ECR No			:	431
*/
#endregion

using CustomModels.Models.MISReports.DataRestorationReport;
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
using System.Linq.Dynamic;
using System.Text;
using System.Configuration;
using System.Globalization;

namespace ECDataUI.Areas.MISReports.Controllers
{
    // UNCOMMENT KAVERI AUTHORIZE ATTRIBUTE 
    //[KaveriAuthorizationAttribute]
    public class DataRestorationReportController : Controller
    {
        ServiceCaller caller = null;
        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
        //static string directoryPath = ConfigurationManager.AppSettings["KaveriUILogPath"];
        //string debugLogFilePath = directoryPath + "\\2021\\Mar\\DatabaseRestorationDebugUI.txt";
        //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

        /// <summary>
        /// Data Restoration Report View
        /// </summary>
        /// <returns>returns Data Restoration Report View</returns>
        [MenuHighlightAttribute]
        [EventAuditLogFilter(Description = "Data Restoration Report View")]
        public ActionResult DataRestorationReport()
        {
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataRestorationReport-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.AnyWhereECLog;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DataRestorationReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                DataRestorationReportViewModel reqModel = caller.GetCall<DataRestorationReportViewModel>("DataRestorationReport", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Data Restoration Report View", URLToRedirect = "/Home/HomePage" });
                }

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataRestorationReport-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Data Restoration Report View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Data Restoration Report Status view
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Data Restoration Report Status view</returns>
        [EventAuditLogFilter(Description = "Data Restoration Report Status view")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult DataRestorationReportStatus(FormCollection formCollection)
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataRestorationReportStatus-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021


                #region VARIABLE DECLARATIONS
                DataRestorationPartialViewModel resModel = new DataRestorationPartialViewModel();
                DataRestorationReportViewModel reqModel = new DataRestorationReportViewModel();
                String errorMessage = string.Empty;
                #endregion

                #region INPUT PARAMENTERS
                string SroID = formCollection["SroID"];

                // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
                string INIT_ID = formCollection["INIT_ID"];

                #endregion

                #region VALIDATIONS
                // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
                //if (String.IsNullOrEmpty(SroID))
                //    return Json(new { serverError = false, errorMessage = "Please select sro." });

                //if (!Int32.TryParse(SroID, out int SroID_INT))
                //    return Json(new { serverError = false, errorMessage = "Please select sro." });

                if (String.IsNullOrEmpty(SroID))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Restoration Report Status view." });

                if (!Int32.TryParse(SroID, out int SroID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Restoration Report Status view." });

                // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
                if (String.IsNullOrEmpty(INIT_ID))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Restoration Report Status view." });

                if (!Int32.TryParse(INIT_ID, out int INIT_ID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Restoration Report Status view." });


                #endregion

                reqModel.SROfficeID = SroID_INT;
                // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
                reqModel.INIT_ID_INT = INIT_ID_INT;


                // SEND ROLE ID TO DAL FETCH FROM SESSION 
                reqModel.CurrentRoleID = KaveriSession.Current.RoleID;


                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataRestorationReportStatus-before web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportViewModel, DataRestorationPartialViewModel>("DataRestorationReportStatus", reqModel, out errorMessage);

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataRestorationReportStatus-after web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataRestorationReportStatus-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                return View(resModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Data Restoration Report Status view." });
            }
        }

        /// <summary>
        /// Initiate Database Restoration
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Initiate Database Restoration success or failure message</returns>
        [EventAuditLogFilter(Description = "Initiate Database Restoration")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult InitiateDatabaseRestoration(FormCollection formCollection)
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-InitiateDatabaseRestoration-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                #region VARIABLE DECLARATIONS
                DataRestorationReportResModel resModel = new DataRestorationReportResModel();
                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                String errorMessage = string.Empty;
                bool boolInitiationDate;
                DateTime InitiationDate;
                #endregion

                #region INPUT PARAMENTERS
                string SroID = formCollection["SroID"];
                string GenerateKeyValue = formCollection["GenerateKeyValue"];
                string InitiationDateStr = formCollection["InitiationDate"];

                #endregion

                #region VALIDATIONS
                if (String.IsNullOrEmpty(SroID))
                    return Json(new { serverError = false, errorMessage = "Please select sro." });

                if (!Int32.TryParse(SroID, out int j))
                    return Json(new { serverError = false, errorMessage = "Please select sro." });

                //boolInitiationDate = DateTime.TryParse(DateTime.ParseExact(InitiationDateStr, "dd/MM/yyyy", null).ToString(), out InitiationDate);
                //var date1 = DateTime.ParseExact(InitiationDateStr, "dd/MM/yyyy", null);
                //boolInitiationDate = DateTime.TryParse(InitiationDateStr, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None,out InitiationDate);

                boolInitiationDate = DateTime.TryParse(DateTime.ParseExact(InitiationDateStr, "dd/MM/yyyy HH:mm:ss", null).ToString(), out InitiationDate);

                if (!boolInitiationDate)
                {
                    return Json(new { serverError = false, errorMessage = "Invalid Initiation Date." });
                }
                #endregion

                reqModel.SROfficeID = j;
                reqModel.GenerateKeyValue = GenerateKeyValue;
                reqModel.InitiationDate = InitiationDate;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-InitiateDatabaseRestoration-before web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationReportResModel>("InitiateDatabaseRestoration", reqModel, out errorMessage);

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-InitiateDatabaseRestoration-after web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (resModel == null)
                    return Json(new { serverError = true, errorMessage = "Error occured while Initiating Database Restoration." });
                // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 07-08-2020 AT 5:55 PM
                else if (resModel.IsDataSavedSuccefully == false && (!String.IsNullOrEmpty(resModel.InitiationProcessStartedOrNotMSG)))
                    return Json(new { serverError = false, errorMessage = resModel.InitiationProcessStartedOrNotMSG });
                // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 07-08-2020 AT 5:55 PM
                else if (resModel.IsDataSavedSuccefully == false)
                    return Json(new { serverError = false, errorMessage = "Initiation process not started." });
                else
                    return Json(new { serverError = false, IsDataSavedSuccefully = resModel.IsDataSavedSuccefully, GenerateKeyValue = GenerateKeyValue });

              
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while Initiating Database Restoration." });
            }
        }

        /// <summary>
        /// Generate Key After Expiration
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Generate Key After Expiration</returns>
        [EventAuditLogFilter(Description = "Generate Key After Expiration")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult GenerateKeyAfterExpiration(FormCollection formCollection)
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-GenerateKeyAfterExpiration-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                #region VARIABLE DECLARATIONS
                DataRestorationReportResModel resModel = new DataRestorationReportResModel();
                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                String errorMessage = string.Empty;

                #endregion

                #region INPUT PARAMENTERS
                string initID = formCollection["initID"];
                string keyID = formCollection["keyID"];

                #endregion

                #region VALIDATIONS
                if (String.IsNullOrEmpty(initID))
                    return Json(new { serverError = false, errorMessage = "Error occured while generating new key." });

                if (!Int32.TryParse(initID, out int initID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while generating new key." });

                if (String.IsNullOrEmpty(keyID))
                    return Json(new { serverError = false, errorMessage = "Error occured while generating new key." });

                if (!Int32.TryParse(keyID, out int keyID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while generating new key." });
                #endregion

                reqModel.INITID_INT = initID_INT;
                reqModel.KEYID_INT = keyID_INT;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-GenerateKeyAfterExpiration-before wb api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationReportResModel>("GenerateKeyAfterExpiration", reqModel, out errorMessage);

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-GenerateKeyAfterExpiration-after wb api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (resModel == null)
                    return Json(new { serverError = true, errorMessage = "Error occured while generating new key." });
                else if (resModel.IsKeyGeneratedSuccefully == false)
                    return Json(new { serverError = false, errorMessage = "Error occured while generating new key." });
                else
                    return Json(new { serverError = false, IsKeyGeneratedSuccefully = resModel.IsKeyGeneratedSuccefully, GeneratedKeyWithMsgAfterExpiration = resModel.GeneratedKeyWithMsgAfterExpiration });
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while generating new key." });
            }
        }

        /// <summary>
        /// Approve Script
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Approve Script success or failure message</returns>
        [EventAuditLogFilter(Description = "Approve Script")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult ApproveScript(FormCollection formCollection)
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-ApproveScript-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021


                #region VARIABLE DECLARATIONS
                DataRestorationReportResModel resModel = new DataRestorationReportResModel();
                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                String errorMessage = string.Empty;

                #endregion

                #region INPUT PARAMENTERS
                string scriptID = formCollection["scriptID"];
                string InitID = formCollection["InitID"];

                #endregion

                #region VALIDATIONS
                if (String.IsNullOrEmpty(scriptID))
                    return Json(new { serverError = false, errorMessage = "Error occured while approving script." });

                if (!Int32.TryParse(scriptID, out int scriptID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while approving script." });

                if (String.IsNullOrEmpty(InitID))
                    return Json(new { serverError = false, errorMessage = "Error occured while approving script." });

                if (!Int32.TryParse(InitID, out int InitID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while approving script." });
                #endregion

                reqModel.INITID_INT = InitID_INT;
                reqModel.SCRIPT_ID_INT = scriptID_INT;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-ApproveScript-before web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationReportResModel>("ApproveScript", reqModel, out errorMessage);

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-ApproveScript-after web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (resModel == null)
                    return Json(new { serverError = true, errorMessage = "Error occured while approving script." });
                else if (resModel.IsScriptApprovedSuccefully == false)
                    return Json(new { serverError = false, errorMessage = "Error occured while approving script." });
                else
                    return Json(new
                    {
                        serverError = false,
                        IsScriptApprovedSuccefully = resModel.IsScriptApprovedSuccefully,
                        ScriptApprovedMSG = resModel.ScriptApprovedMSG
                        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
                        // SEND MESSAGE FOR SR IN PLACE OF APPROVE BUTTON
                        ,
                        ApproveBtnORMessage = resModel.ApproveBtnORMessage
                    });
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while approving script." });
            }
        }

        /// <summary>
        /// Data Insertion Table
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Data Insertion Table</returns>
        [EventAuditLogFilter(Description = "Data Insertion Table")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult DataInsertionTable(FormCollection formCollection)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DataRestorationReportController-DataInsertionTable-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region VARIABLE DECLARATIONS
                DataRestorationPartialViewModel resModel = new DataRestorationPartialViewModel();
                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                String errorMessage = string.Empty;
                #endregion

                #region INPUT PARAMENTERS
                string scriptID = formCollection["scriptID"];
                string InitID = formCollection["InitID"];
                // FLAG ADDED ON 10-07-2020 AT 4:45 PM
                string IsRectifiedScriptUploaded = formCollection["IsRectifiedScriptUploaded"];


                #endregion

                #region VALIDATIONS
                if (String.IsNullOrEmpty(scriptID))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Insertion details Table." });

                if (!Int32.TryParse(scriptID, out int scriptID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Insertion details Table." });

                if (String.IsNullOrEmpty(InitID))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Insertion details Table." });

                if (!Int32.TryParse(InitID, out int InitID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Insertion details Table." });
                #endregion

                reqModel.INITID_INT = InitID_INT;
                reqModel.SCRIPT_ID_INT = scriptID_INT;
                reqModel.IsRectifiedScriptUploaded = IsRectifiedScriptUploaded == "true" ? true : false;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataInsertionTable-before web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationPartialViewModel>("DataInsertionTable", reqModel, out errorMessage);
                
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataInsertionTable-after web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DataInsertionTable-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                return View(resModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Data Insertion details Table." });
            }
        }

        /// <summary>
        /// Download Script Path Verify
        /// </summary>
        /// <param name="InitID"></param>
        /// <param name="scriptID"></param>
        /// <returns>returns Download Script Path Verify success or failure message</returns>
        [EventAuditLogFilter(Description = "Download Script Path Verify")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        //[HttpPost]
        //public ActionResult DownloadScriptPathVerify(FormCollection formCollection)
        public ActionResult DownloadScriptPathVerify(string InitID, String scriptID)
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DownloadScriptPathVerify-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                #region VARIABLE DECLARATIONS

                DataRestorationReportResModel resModel = new DataRestorationReportResModel();
                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                String errorMessage = string.Empty;
                #endregion

                #region INPUT PARAMENTERS
                //string scriptID = formCollection["scriptID"];
                //string InitID = formCollection["InitID"];
                #endregion

                #region VALIDATIONS
                if (String.IsNullOrEmpty(scriptID))
                    return Json(new { serverError = false, errorMessage = "Error occured while verifying path." }, JsonRequestBehavior.AllowGet);

                if (!Int32.TryParse(scriptID, out int scriptID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while verifying path." }, JsonRequestBehavior.AllowGet);

                if (String.IsNullOrEmpty(InitID))
                    return Json(new { serverError = false, errorMessage = "Error occured while verifying path." }, JsonRequestBehavior.AllowGet);

                if (!Int32.TryParse(InitID, out int InitID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while verifying path." }, JsonRequestBehavior.AllowGet);
                #endregion

                reqModel.INITID_INT = InitID_INT;
                reqModel.SCRIPT_ID_INT = scriptID_INT;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DownloadScriptPathVerify-before web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationReportResModel>("DownloadScriptPathVerify", reqModel, out errorMessage);

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DownloadScriptPathVerify-after web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (resModel == null)
                {
                    return Json(new { serverError = false, success = false, errorMessage = "Error occured while verifying path." }, JsonRequestBehavior.AllowGet);
                    //return Json(new { serverError = false, success = false, errorMessage = "Error occured while verifying path." }, JsonRequestBehavior.AllowGet);

                    //return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error in getting Root directory information.", URLToRedirect = "/Home/HomePage" });
                    //return Json(new { serverError = true, success = false, errorMessage = "Error in getting Root directory information." }, JsonRequestBehavior.AllowGet);
                }
                else
                    return Json(new { serverError = false, success = true, IsFileExistAtDownloadPath = resModel.IsFileExistAtDownloadPath }, JsonRequestBehavior.AllowGet);
                //return Json(new { serverError = false, success = true, IsFileExistAtDownloadPath = resModel.IsFileExistAtDownloadPath }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while verifying path." }, JsonRequestBehavior.AllowGet);
                //return Json(new { serverError = true, success = false, errorMessage = "Error occured while verifying path." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Download Script For Rectification
        /// </summary>
        /// <param name="InitID"></param>
        /// <param name="scriptID"></param>
        /// <param name="SroID"></param>
        /// <returns>returns Download Script For Rectification</returns>
        public ActionResult DownloadScriptForRectification(string InitID, String scriptID, String SroID)
        {
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DownloadScriptForRectification-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                caller = new ServiceCaller("DataRestorationReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;


                #region VARIABLE DECLARATIONS

                DataRestorationReportResModel resModel = new DataRestorationReportResModel();
                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                String errorMessage = string.Empty;
                #endregion

                #region INPUT PARAMENTERS
                //string scriptID = scriptID;
                //string InitID = InitID;
                #endregion
                reqModel.INITID_INT = Convert.ToInt32(InitID);
                reqModel.SCRIPT_ID_INT = Convert.ToInt32(scriptID);

                //String fileName = (OfficeType == "SR" ? "SRO" : "DRO") + "_" + OfficeCodeSelected + "_" + Path.GetFileName(path.Replace('*', '\\').Replace('$', ' '));
                //String fileName = Path.GetFileName(path.Replace('*', '\\').Replace('$', ' '));

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DownloadScriptForRectification-before web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationReportResModel>("DownloadScriptForRectification", reqModel, out errorMessage);

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-DownloadScriptForRectification-after web api call");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (resModel == null)
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading script file", URLToRedirect = "/Home/HomePage" });
                else if (resModel.FileContentField == null)
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading script file", URLToRedirect = "/Home/HomePage" });
                else
                {
                    String filename = SroID + "_" + DateTime.Now.ToString().Replace(' ', '_').Replace(':', '_') + ".sql";
                    return File(resModel.FileContentField, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading script file", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Upload Rectified Script View
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Upload Rectified Script View</returns>
        [EventAuditLogFilter(Description = "Upload Rectified Script View")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult UploadRectifiedScriptView(FormCollection formCollection)
        {
            //caller = new ServiceCaller("DataRestorationReportAPIController");
            //TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            //caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-UploadRectifiedScriptView-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                #region VARIABLE DECLARATIONS
                //DataRestorationPartialViewModel resModel = new DataRestorationPartialViewModel();
                //DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                UploadRectifiedScriptViewModel uploadRectifiedScriptViewModel = new UploadRectifiedScriptViewModel();
                String errorMessage = string.Empty;
                #endregion

                #region INPUT PARAMENTERS
                string scriptID = formCollection["scriptID"];
                string InitID = formCollection["InitID"];
                #endregion

                #region VALIDATIONS
                //if (String.IsNullOrEmpty(scriptID))
                //    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Insertion details Table." });

                //if (!Int32.TryParse(scriptID, out int scriptID_INT))
                //    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Insertion details Table." });

                //if (String.IsNullOrEmpty(InitID))
                //    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Insertion details Table." });

                //if (!Int32.TryParse(InitID, out int InitID_INT))
                //    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Insertion details Table." });
                #endregion

                uploadRectifiedScriptViewModel.INITID_STR = InitID;
                uploadRectifiedScriptViewModel.SCRIPT_ID_STR = scriptID;

                //resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationPartialViewModel>("DataInsertionTable", reqModel, out errorMessage);

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DataRestorationReportController-UploadRectifiedScriptView-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                return View(uploadRectifiedScriptViewModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Data Insertion details Table." });
            }
        }

        /// <summary>
        /// Save Uploded Rectified Script
        /// </summary>
        /// <returns>returns Save Uploded Rectified Script</returns>
        [EventAuditLogFilter(Description = "Save Uploded Rectified Script")]
        // [ValidateAntiForgeryTokenOnAllPosts] // adding this token giving internal server error
        [HttpPost]
        public ActionResult SaveUplodedRectifiedScript()
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-SaveUplodedRectifiedScript-IN");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                #region VARIABLE DECLARATIONS
                DataRestorationReportResModel resModel = new DataRestorationReportResModel();
                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                //UploadRectifiedScriptViewModel uploadRectifiedScriptViewModel = new UploadRectifiedScriptViewModel();
                String errorMessage = string.Empty;
                HttpPostedFileBase file = null;
                #endregion

                #region INPUT PARAMENTERS
                String INITID_STR = Request.Params["INITID_STR"].ToString();
                String SCRIPT_ID_STR = Request.Params["SCRIPT_ID_STR"].ToString();
                String ScriptRectificationHistory_STR = Request.Params["ScriptRectificationHistory_STR"].ToString();

                foreach (string sFileName in Request.Files)
                {
                    file = Request.Files[sFileName];
                }
               
                #endregion

                #region VALIDATIONS

                if (file == null)
                {
                    return Json(new { serverError = false, errorMessage = "Please upload file." });
                }
                else
                {
                    int DblExtensions = 0;
                    if (file != null)
                        DblExtensions = file.FileName.Split('.').Length - 1;


                    if (file.ContentLength < 0)
                        return Json(new { errorMessage = "Please Upload rectified script file." });

                    if (!IsValidExtension(Path.GetExtension(file.FileName.ToLower())))
                    {
                        return Json(new { errorMessage = "Please upload sql file only. Error in file name - " + file.FileName + " Kindly upload file again." });
                    }
                    //else if (!IsValidContentLength(file.ContentLength))
                    //{
                    //    return Json(new { errorMessage = "Each File size should be less than 20 MB. Kindly upload file again." });
                    //}                 
                    else if (DblExtensions > 1)
                    {
                        return Json(new { errorMessage = "Cannot Upload Files because it is either not a supported File type or because the file has been damaged(for example,it was sent as an attachment and wasn't correctly decoded)!!" });
                    }
                }
                if (String.IsNullOrEmpty(ScriptRectificationHistory_STR))
                    return Json(new { serverError = false, errorMessage = "Please enter corrective action." });

                // ADDED BELOW CODE BY SHUBHAM BHAGAT ON 07-08-2020
                Regex regx = new Regex("^[^<>]+$");
                Match mtch = regx.Match(ScriptRectificationHistory_STR);
                if (!mtch.Success)
                {
                    return Json(new { serverError = false, errorMessage = "Special characters like <, > is not allowed." });
                }
                // ADDED ABOVE CODE BY SHUBHAM BHAGAT ON 07-08-2020

                if (String.IsNullOrEmpty(SCRIPT_ID_STR))
                    return Json(new { serverError = false, errorMessage = "Error occured while saving uploaded rectified script." });

                if (!Int32.TryParse(SCRIPT_ID_STR, out int scriptID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while saving uploaded rectified script." });

                if (String.IsNullOrEmpty(INITID_STR))
                    return Json(new { serverError = false, errorMessage = "Error occured while saving uploaded rectified script." });

                if (!Int32.TryParse(INITID_STR, out int InitID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while saving uploaded rectified script." });



                #endregion

                byte[] fileData = null;

                if (file != null)
                {
                    using (var binaryReader = new BinaryReader(file.InputStream))
                    {
                        fileData = binaryReader.ReadBytes(file.ContentLength);
                    }
                }

                reqModel.SCRIPT_ID_INT = scriptID_INT;
                reqModel.INITID_INT = InitID_INT;
                reqModel.ScriptRectificationHistory = ScriptRectificationHistory_STR;
                reqModel.FileContentField = fileData;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-SaveUplodedRectifiedScript-before web api call");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationReportResModel>("SaveUplodedRectifiedScript", reqModel, out errorMessage);
              
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-SaveUplodedRectifiedScript-after web api call");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (resModel == null)
                    return Json(new { serverError = true, errorMessage = "Error occured while saving uploaded rectified script." });
                else if (resModel.IsRectifiedScriptUploadedSuccefully == false)
                    return Json(new { serverError = false, errorMessage = "Error occured while saving uploaded rectified script." });
                else
                    return Json(new { serverError = false, IsRectifiedScriptUploadedSuccefully = resModel.IsRectifiedScriptUploadedSuccefully, RectifiedScriptUploadedMsg = resModel.RectifiedScriptUploadedMsg });
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while saving uploaded rectified script." });
            }
        }

        /// <summary>
        /// Is Valid Extension
        /// </summary>
        /// <param name="extension"></param>
        /// <returns>returns Is Valid Extension success or failure message</returns>
        private bool IsValidExtension(string extension)
        {
            return (extension.Equals(".sql"));
        }

        #region ADDED BY PANKAJ ON 15-07-2020
        /// <summary>
        /// Confirm Data Insertion
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Confirm Data Insertion success message</returns>
        [EventAuditLogFilter(Description = "Confirm Data Insertion")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult ConfirmDataInsertion(FormCollection formCollection)
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-ConfirmDataInsertion-IN");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                #region VARIABLE DECLARATIONS
                DataRestorationReportResModel resModel = new DataRestorationReportResModel();
                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                String errorMessage = string.Empty;

                #endregion

                #region INPUT PARAMENTERS
                string InitID = formCollection["InitId"];
                string SROcode = formCollection["officeID"];

                #endregion

                #region VALIDATIONS
                if (String.IsNullOrEmpty(SROcode))
                    return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });

                if (!Int32.TryParse(SROcode, out int SROfficeID))
                    return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });

                if (String.IsNullOrEmpty(InitID))
                    return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });

                if (!Int32.TryParse(InitID, out int InitID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });
                #endregion

                reqModel.INITID_INT = InitID_INT;
                reqModel.SROfficeID = SROfficeID;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-ConfirmDataInsertion-before web api call");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationReportResModel>("ConfirmDataInsertion", reqModel, out errorMessage);
             
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-ConfirmDataInsertion-after web api call");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-ConfirmDataInsertion-OUT");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (resModel == null)
                    return Json(new { serverError = true, errorMessage = "Error occured while confirming data insertion." });
                else if (resModel.IsDataInsertionConfirmed == false)
                    return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });
                else
                    return Json(new { serverError = false, IsDataInsertionConfirmed = resModel.IsDataInsertionConfirmed, DataInsertionConfrimationMsg = resModel.DataInsertionConfrimationMsg });

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while confirming data insertion." });
            }
        }

        ////2nd method for partial view
        //[EventAuditLogFilter(Description = "Get confirmation button message")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        //[HttpPost]
        //public ActionResult GetConfirmationButtonMessage(FormCollection formCollection)
        //{
        //    caller = new ServiceCaller("DataRestorationReportAPIController");
        //    TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
        //    caller.HttpClient.Timeout = objTimeSpan;
        //    try
        //    {
        //        #region VARIABLE DECLARATIONS
        //        DataRestorationReportResModel resModel = new DataRestorationReportResModel();
        //        DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
        //        String errorMessage = string.Empty;

        //        #endregion

        //        #region INPUT PARAMENTERS
        //        string InitID = formCollection["InitId"];
        //        string SROcode = formCollection["officeID"];

        //        #endregion

        //        #region VALIDATIONS
        //        if (String.IsNullOrEmpty(SROcode))
        //            return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });

        //        if (!Int32.TryParse(SROcode, out int SROfficeID))
        //            return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });

        //        if (String.IsNullOrEmpty(InitID))
        //            return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });

        //        if (!Int32.TryParse(InitID, out int InitID_INT))
        //            return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });
        //        #endregion

        //        reqModel.INITID_INT = InitID_INT;
        //        reqModel.SROfficeID = SROfficeID;

        //        resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationReportResModel>("GetConfirmationButtonMessage", reqModel, out errorMessage);

        //        if (resModel == null)
        //            return Json(new { serverError = true, errorMessage = "Error occured while confirming data insertion." });
        //        else if (resModel.IsDataInsertionConfirmed == false)
        //            return Json(new { serverError = false, errorMessage = "Error occured while confirming data insertion." });
        //        else
        //            return Json(new { serverError = false, IsDataInsertionConfirmed = resModel.IsDataInsertionConfirmed, DataInsertionConfrimationMsg = resModel.DataInsertionConfrimationMsg });

        //    }
        //    catch (Exception ex)
        //    {
        //        ExceptionLogs.LogException(ex);
        //        return Json(new { serverError = true, errorMessage = "Error occured while confirming data insertion." });
        //    }
        //}

        #endregion


        #region ADDED BY SHUBHAM BHAGAT ON 23-07-2020
        /// <summary>
        /// Loads INITIATE MASTER DETAILS Datatable
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Loads INITIATE MASTER DETAILS Datatable</returns>
        // METHOD TO PUPULATE DATATABLE
        [EventAuditLogFilter(Description = "Loads INITIATE MASTER DETAILS Datatable")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult LoadInitiateMasterTable(FormCollection formCollection)
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        
                string SroID = formCollection["SroID"];

                int SroId = Convert.ToInt32(SroID);

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                //int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                //short OfficeID = KaveriSession.Current.OfficeID;
                //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });
                //Validation For DR Login
                //if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                //{
                //    //Validation for DR when user do not select any sro which is by default "Select"
                //    if ((SroId == 0))
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any SRO"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //}
                //else
                //{//Validations of Logins other than SR and DR

                //    if ((SroId == 0 && DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any District"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //    else if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any SRO"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;

                //    }
                //}
                //if (string.IsNullOrEmpty(FromDate))
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "From Date required"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (string.IsNullOrEmpty(ToDate))
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "To Date required"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                //boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);
                //bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                //if (!boolFrmDate)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Invalid From Date"

                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (!boolToDate)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Invalid To Date"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (frmDate > toDate)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "From Date can not be larger than To Date"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if ((toDate - frmDate).TotalDays > 180)//six months validation by RamanK on 20-09-2019
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Data of six months can be seen at a time"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;

                //}

              

                DataRestorationReportReqModel reqModel = new DataRestorationReportReqModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                //reqModel.FromDate = FromDate;
                //reqModel.ToDate = ToDate;
                reqModel.SROfficeID = SroId;
                //reqModel.DistrictID = DistrictId;
                reqModel.SearchValue = searchValue;

                // SEND ROLE ID TO DAL FETCH FROM SESSION 
                reqModel.CurrentRoleID = KaveriSession.Current.RoleID;

                //reqModel.IsExcel = false;
                //int totalCount = caller.PostCall<ECDailyReceiptRptView, int>("GetECDailyReceiptsTotalCount", reqModel, out errorMessage);

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

                DataRestorationPartialViewModel resModel = caller.PostCall<DataRestorationReportReqModel, DataRestorationPartialViewModel>("LoadInitiateMasterTable", reqModel, out errorMessage);

                IEnumerable<InitMasterModel> result = resModel.InitMasterModelList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Initiate master Details." });
                }
                int totalCount = resModel.InitMasterModelList.Count;
                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}
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
                                errorMessage = "Please enter valid Search String "
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }
                    else
                    {
                        result = result.Where(m => m.SrNo.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                        m.InitiationDateTime.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.STATUS_DESCRIPTION.ToLower().Contains(searchValue.ToLower()) ||
                        m.CompleteDateTime.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.ConfirmDateTime.ToString().ToLower().Contains(searchValue.ToLower())
                        //||
                        //m.Amount.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        //m.ReceiptType.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        //m.ReceiptNo.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        //m.PeriodOfSearch.ToLower().Contains(searchValue.ToLower())
                        );
                        totalCount = result.Count();
                    }
                }
                //  Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

                var gridData = result.Select(InitMasterModel => new
                {
                    SrNo = InitMasterModel.SrNo,
                    SROName = InitMasterModel.SROName,
                    InitiationDateTime = InitMasterModel.InitiationDateTime,
                    // chnaged on 11-08-2020 at 12:20 pm 
                    //STATUS_DESCRIPTION = "<div class='tooltip'>" + InitMasterModel.STATUS_DESCRIPTION + "<span class='tooltiptext>Click here</span></div>",
                    STATUS_DESCRIPTION = InitMasterModel.STATUS_DESCRIPTION ,
                    Is_Completed_STR = InitMasterModel.Is_Completed_STR,
                    CompleteDateTime = InitMasterModel.CompleteDateTime,
                    ConfirmDateTime = InitMasterModel.ConfirmDateTime,
                    INIT_ID = InitMasterModel.INIT_ID,
                    SroCode = InitMasterModel.SroCode,
                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                    AbortBtn = InitMasterModel.AbortBtn,
                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020	

                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],

                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        //data = gridData.ToArray().ToList(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        //ExcelDownloadBtn = ExcelDownloadBtn
                        // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 8:00 PM
                         InitiateBTNForSR = resModel.InitiateBTNForSR,
                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                        CurrentRoleID= KaveriSession.Current.RoleID
                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        // ADDED CONDITION BY SHUBHAM BHAGAT ON 24-07-2020 AT 3:40 PM
                        data = gridData.ToArray(),
                        //data = gridData.Skip(startLen).Take(pageSize).ToArray(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = resModel.TotalRecords,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        //ExcelDownloadBtn = ExcelDownloadBtn
                        // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 8:00 PM
                        InitiateBTNForSR = resModel.InitiateBTNForSR,
                        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                        CurrentRoleID = KaveriSession.Current.RoleID
                        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Initiate master Details." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        /// <summary>
        /// Abort View
        /// </summary>
        /// <param name="INIT_ID"></param>
        /// <returns>returns Abort View</returns>
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        [EventAuditLogFilter(Description = "Abort View")]
        public ActionResult AbortView(String INIT_ID)

        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.AnyWhereECLog;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DataRestorationReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-AbortView-before web api call");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                AbortViewModel reqModel = caller.GetCall<AbortViewModel>("AbortView", new { INIT_ID = INIT_ID });

                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-AbortView-after web api call");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Abort View", URLToRedirect = "/Home/HomePage" });
                }
                reqModel.INIT_ID_ForAbort = INIT_ID;
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Abort View", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// Save Abort Data
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns>returns Save Abort Data</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Save Abort Data")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SaveAbortData(AbortViewModel viewModel)
        {

            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    streamWriter.WriteLine("-DataRestorationReportController-SaveAbortData-IN");
                //    streamWriter.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                if (viewModel != null)
                {
                    #region USER VARIABLES & VALIDATIONS
                    viewModel.INIT_ID_INT = Convert.ToInt32(viewModel.INIT_ID_ForAbort);
                    if (viewModel.AbortDescription != null)
                    {
                        // IS NULL OR EMPTY
                        if (string.IsNullOrEmpty(viewModel.AbortDescription))
                        {
                            return Json(new { success = false, message = "Reason for Abort empty." });
                        }

                        if (string.IsNullOrEmpty(viewModel.AbortDescription.Trim()))
                        {
                            return Json(new { success = false, message = "Reason for Abort cannot be empty." });
                        }


                        //// LENGTH SHOULD BE MORE THAN MAX LENGTH
                        //if (viewModel.AbortDescription.Length < viewModel.AbortDescription.MaxCapacity)
                        //{
                        //    return Json(new { success = false, message = "Reason for Abort exceeds max length." });
                        //}

                        //// LENGTH SHOULD BE MORE THAN MAX LENGTH
                        //if (viewModel.AbortDescription.Length < viewModel.AbortDescription.MaxCapacity)
                        //{
                        //    return Json(new { success = false, message = "Reason for Abort exceeds max length." });
                        //}

                        // < > & is not allowed
                        Regex regx = new Regex("^[^<>]+$");
                        Match mtch = regx.Match(viewModel.AbortDescription.ToString());
                        if (!mtch.Success)
                        {
                            return Json(new { success = false, errorMessage = "Special characters like <, >, &  is not allowed." });
                        }
                    }
                    else
                    {
                        return Json(new { success = false, message = "Please enter Reason for Abort." });
                    }
                    #endregion

                    caller = new ServiceCaller("DataRestorationReportAPIController");
                    TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                    caller.HttpClient.Timeout = objTimeSpan;



                    //viewModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;

                    //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                    //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                    //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    streamWriter.WriteLine("-DataRestorationReportController-SaveAbortData-before web api call");
                    //    streamWriter.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                    AbortViewModel response = caller.PostCall<AbortViewModel, AbortViewModel>("SaveAbortData", viewModel);

                    //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021
                    //using (System.IO.StreamWriter streamWriter = System.IO.File.AppendText(debugLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    streamWriter.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                    //    streamWriter.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    streamWriter.WriteLine("-DataRestorationReportController-SaveAbortData-after web api call");
                    //    streamWriter.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 08-04-2021

                    if (response != null)
                    {
                        return Json(new { success = true, message = response.Message });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Reason for Abort not saved." });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "Reason for Abort not saved." });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { IsServerError = true, success = false, message = "Reason for Abort not saved." });
            }
        }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020

        [EventAuditLogFilter(Description = "Data Restoration Report Status view")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult DataRestorationReportStatusForScript(FormCollection formCollection)
        {
            caller = new ServiceCaller("DataRestorationReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region VARIABLE DECLARATIONS
                DataRestorationPartialViewModel resModel = new DataRestorationPartialViewModel();
                DataRestorationReportViewModel reqModel = new DataRestorationReportViewModel();
                String errorMessage = string.Empty;
                #endregion

                #region INPUT PARAMENTERS
                string SroID = formCollection["SroID"];

                // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
                string INIT_ID = formCollection["INIT_ID"];

                #endregion

                #region VALIDATIONS
                // COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
                //if (String.IsNullOrEmpty(SroID))
                //    return Json(new { serverError = false, errorMessage = "Please select sro." });

                //if (!Int32.TryParse(SroID, out int SroID_INT))
                //    return Json(new { serverError = false, errorMessage = "Please select sro." });

                if (String.IsNullOrEmpty(SroID))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Restoration Report Status view." });

                if (!Int32.TryParse(SroID, out int SroID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Restoration Report Status view." });

                // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
                if (String.IsNullOrEmpty(INIT_ID))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Restoration Report Status view." });

                if (!Int32.TryParse(INIT_ID, out int INIT_ID_INT))
                    return Json(new { serverError = false, errorMessage = "Error occured while getting Data Restoration Report Status view." });


                #endregion

                reqModel.SROfficeID = SroID_INT;
                // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
                reqModel.INIT_ID_INT = INIT_ID_INT;


                // SEND ROLE ID TO DAL FETCH FROM SESSION 
                reqModel.CurrentRoleID = KaveriSession.Current.RoleID;

                resModel = caller.PostCall<DataRestorationReportViewModel, DataRestorationPartialViewModel>("DataRestorationReportStatusForScript", reqModel, out errorMessage);


                return View("DataRestorationReportStatus",resModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Data Restoration Report Status view." });
            }
        }
    }

}