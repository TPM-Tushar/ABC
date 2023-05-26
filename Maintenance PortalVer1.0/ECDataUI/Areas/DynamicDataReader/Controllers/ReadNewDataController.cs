using CustomModels.Models.DynamicDataReader;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.DynamicDataReader.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ReadNewDataController : Controller
    {
        #region PROPERTIES
        private ServiceCaller caller = new ServiceCaller("ReadNewDataAPIController");
        #endregion


        /// <summary>
        /// ReadNewDataView
        /// </summary>
        /// <returns>view</returns>
        [HttpGet]
        [MenuHighlight]
        [EventAuditLogFilter(Description = "Read New Data View")]
        public ActionResult ReadNewDataView()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.RemittanceXMLLog;
                int OfficeID = KaveriSession.Current.OfficeID;
                ReadNewDataModel model = new ReadNewDataModel();
                List<SelectListItem> databaseList = new List<SelectListItem>();
                databaseList.Add(new SelectListItem { Text = "Select", Value = "0" });
                databaseList.Add(new SelectListItem { Text = "ECDATA", Value = "ECDATA" });
                databaseList.Add(new SelectListItem { Text = "KAIGR_SEARCHDB", Value = "KAIGR_SEARCHDB" });
                databaseList.Add(new SelectListItem { Text = "PEN_DOCS", Value = "PEN_DOCS" });
                databaseList.Add(new SelectListItem { Text = "ECDATA_DOCS", Value = "ECDATA_DOCS" });

                model.DatabaseList = databaseList;

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }



        /// <summary>
        /// GetSSRSReportData
        /// </summary>
        /// <returns>view</returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Get SSRS Report Data")]
        public ActionResult GetSSRSReportData(FormCollection formCollection)
        {
            try
            {
                ReadNewDataModel model = new ReadNewDataModel();
                ReadNewDataResModel resModel = new ReadNewDataResModel();

                string DBName = formCollection["DatabaseName"];
                string Purpose = formCollection["Purpose"];
                string QueryTxt = formCollection["QueryText"];


                if (string.IsNullOrEmpty(DBName))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Database is required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (string.IsNullOrEmpty(Purpose))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Purpose is required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //database name validation
                if (DBName != "ECDATA" && DBName != "KAIGR_SEARCHDB" && DBName != "PEN_DOCS" && DBName != "ECDATA_DOCS")
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Select valid database"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (QueryTxt.ToLower().Contains("select * from"))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Select * from query are not allowed."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                model.QueryText = QueryTxt;
                model.DatabaseName = DBName;
                model.LoginName = KaveriSession.Current.FullName;
                model.Purpose = Purpose;
                model.UserID = ConfigurationManager.AppSettings["UserIDForSSRSReport"];
                model.Password = ConfigurationManager.AppSettings["PasswordForSSRSReport"];
                //model.QueryID = Convert.ToInt32(QueryID);
                string errorMessage = string.Empty;

                resModel = caller.PostCall<ReadNewDataModel, ReadNewDataResModel>("SaveNewQuerySearchParameter", model, out errorMessage);

                #region COMMENTED CODE
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.RemittanceXMLLog;
                //int OfficeID = KaveriSession.Current.OfficeID;
                //QueryAnalyserNewQueryViewModel model = new QueryAnalyserNewQueryViewModel();
                //model = caller.GetCall<QueryAnalyserNewQueryViewModel>("QueryAnalyserNewQueryView", new { OfficeID = OfficeID });


                //ReportViewer reportViewer = new ReportViewer();
                //reportViewer.ProcessingMode = ProcessingMode.Local;
                //reportViewer.SizeToReportContent = true;
                //reportViewer.Width = Unit.Percentage(900);
                //reportViewer.Height = Unit.Percentage(900);

                //var connectionString = ConfigurationManager.ConnectionStrings["DbEmployeeConnectionString"].ConnectionString;


                //SqlConnection conx = new SqlConnection(connectionString);
                //SqlDataAdapter adp = new SqlDataAdapter("SELECT * FROM Employee_tbt", conx);

                //adp.Fill(ds, ds.Employee_tbt.TableName);

                //reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\MyReport.rdlc";
                //reportViewer.LocalReport.DataSources.Add(new ReportDataSource("MyDataSet", ds.Tables[0]));


                //ViewBag.ReportViewer = reportViewer;

                /////////
                //string ssrsurl = @"http://egovhp09/ReportServer";
                //ReportViewer reportViewer = new ReportViewer();
                //reportViewer.ProcessingMode = ProcessingMode.Remote;
                //reportViewer.SizeToReportContent = true;
                //reportViewer.Width = Unit.Percentage(1800);
                //reportViewer.Height = Unit.Percentage(1800);
                //reportViewer.AsyncRendering = true;
                //reportViewer.ServerReport.ReportServerUrl = new Uri(ssrsurl);
                //reportViewer.ServerReport.ReportPath = "/Report2";

                //ViewBag.ReportViewer = reportViewer;
                #endregion

                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }
    }
}