using CustomModels.Models.Dashboard;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Dashboard.Controllers
{
    // COMMENTED BY SHUBHAM BHAGAT ON 14-10-2020 FOR Graph development
    [KaveriAuthorizationAttribute]
    public class DashboardController : Controller
    {
        //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
        //static string directoryPath = ConfigurationManager.AppSettings["KaveriUILogPath"];
        //string dashboardLogFilePath = directoryPath + "\\2020\\Aug\\DashboardTab2LogUI.txt";
        ////string dashboardLogFilePath = directoryPath + "\\DashboardTab2Log.txt";
        //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

        ServiceCaller caller = null;
        [HttpGet]
        public ActionResult DashboardView()
        {
            try
            {

                return View();
            }
            catch (Exception)
            {
                throw;
            }

        }
        [HttpGet]
        public ActionResult DashboardSummaryView()
        {
            try
            {
                DashboardSummaryModel viewModel = new DashboardSummaryModel();
                caller = new ServiceCaller("DashboardAPIController");
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.Dashboard;
                int OfficeID = KaveriSession.Current.OfficeID;
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                viewModel = caller.GetCall<DashboardSummaryModel>("DashboardSummaryView", new { OfficeID = OfficeID });
                //viewModel.TargetAchieved = "40";

                return View(viewModel);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpGet]
        public ActionResult DashboardDetailsView()
        {
            try
            {
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.WriteLine(string.Join("-", Enumerable.Repeat<string>("-", 60)));
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardController-DashboardDetailsView-IN");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.Dashboard;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                DashboardDetailsViewModel ViewModel = caller.GetCall<DashboardDetailsViewModel>("DashboardDetailsView", new { OfficeID = OfficeID });

                if (ViewModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Dashboard Details View", URLToRedirect = "/Home/HomePage" });

                }

                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                //{
                //    string format = "{0} : {1}";
                //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                //    file.WriteLine("-DashboardController-DashboardDetailsView-OUT");
                //    file.Flush();
                //}
                //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                return View(ViewModel);



            }
            catch (Exception)
            {
                throw;

            }

        }
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                  caller = new ServiceCaller("CommonsApiController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" });
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Get  Datatable
        /// </summary>
        /// <param name="ReqModel"></param>
        /// <returns>View</returns>
        [EventAuditLogFilter(Description = "Loads CD Written Report Datatable")]
        [HttpPost]
        public ActionResult LoadDashboardSumaryTable(FormCollection formCollection)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DashboardController-LoadDashboardSumaryTable-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020


            caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        
                string SroID = formCollection["SroID"];
                string DistrictID = formCollection["DistrictID"];
                string NatureOfDocID = formCollection["NatureOfDocID"];
                string FinYearId = formCollection["FinYearId"];

                int SroId = Convert.ToInt32(SroID);
                int DistrictId = Convert.ToInt32(DistrictID);
                int FinYearID = Convert.ToInt32(FinYearId);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //DateTime frmDate, toDate;
                //bool boolFrmDate = false;
                //bool boolToDate = false;
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;
                //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });

                System.Text.RegularExpressions.Regex regxForCDNumber = new Regex("^[a-zA-Z0-9-, ]+$");
                Match mtchCDNumber = regxForCDNumber.Match(NatureOfDocID);
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

                //    if ((DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
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
                //    //else
                //    // if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
                //    //{
                //    //    var emptyData = Json(new
                //    //    {
                //    //        draw = formCollection["draw"],
                //    //        recordsTotal = 0,
                //    //        recordsFiltered = 0,
                //    //        data = "",
                //    //        status = false,
                //    //        errorMessage = "Please select any SRO"
                //    //    });
                //    //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    //    return emptyData;

                //    //}
                //}

                DashboardDetailsViewModel reqModel = new DashboardDetailsViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                //reqModel.NatureOfDocID = Convert.ToInt32(NatureOfDocID);
                reqModel.SNatureOfDocID = NatureOfDocID;
                reqModel.SROfficeID = SroId;
                reqModel.DROfficeID = DistrictId;
                reqModel.FinYearId = FinYearID;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.SearchValue = searchValue;
                }
                if (NatureOfDocID == null)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select valid Nature Of DocumentID"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }
                //else if (!mtchCDNumber.Success)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select valid CDNumber"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                else if (NatureOfDocID == "Select")
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select CDNumber"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }


                DashboardSummaryTblResModel DashboardSummaryResModel = caller.PostCall<DashboardDetailsViewModel, DashboardSummaryTblResModel>("LoadDashboardSumaryTable", reqModel, out errorMessage);
                if (DashboardSummaryResModel == null || DashboardSummaryResModel.IDashboardSummaryRecData==null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Dashboard Summary Table Model." });
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
                                errorMessage = "Please enter valid Search String "
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }

                }

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    //DashboardSummaryResModel.IDashboardSummaryTblList = DashboardSummaryResModel.IDashboardSummaryTblList.OrderBy(sortColumn + " " + sortColumnDir);
                }

                var gridData = DashboardSummaryResModel.IDashboardSummaryRecData.Select(DashboardSummaryRecData => new
                {
                    Today = DashboardSummaryRecData.Today,
                    Yesterday = DashboardSummaryRecData.Yesterday,
                    CurrentMonth = DashboardSummaryRecData.CurrentMonth,
                    PreviousMonth = DashboardSummaryRecData.PreviousMonth,
                    CurrentFinYear = DashboardSummaryRecData.CurrentFinYear,
                    PrevFinYear = DashboardSummaryRecData.PrevFinYear,
                    UptoPrevFinYear = DashboardSummaryRecData.UptoPrevFinYear,
                    UptoCurrentFinYear = DashboardSummaryRecData.UptoCurrentFinYear,
                    Description = DashboardSummaryRecData.Description
                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //  String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                //String ExcelDownloadBtn = DashboardSummaryResModel.IDashboardSummaryRecData.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                //ADDED BY PANKAJ SAKHARE ON 18-09-2020
                String ExcelDownloadBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style='margin-top:1%' onclick=EXCELDashboardSumaryTable('" + reqModel.DROfficeID + "','" + reqModel.SROfficeID + "','" + reqModel.SNatureOfDocID + "','" + reqModel.FinYearId + "')><i class='fa fa-download fa-lg' aria-hidden='true' style='margin-top:1%;color:#484848;'></i></a>";
                
                // < button type = 'button' class='btn btn-success' style='margin-top:1%' onclick="EXCELDashboardSumaryTable()"><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = DashboardSummaryResModel.IDashboardSummaryRecData.Count(),
                        status = "1",
                        recordsFiltered = DashboardSummaryResModel.IDashboardSummaryRecData.Count(),
                        //PDFDownloadBtn = PDFDownloadBtn,
                        //ADDED BY PANKAJ ON 18-09-2020
                        ExcelBtn = ExcelDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadDashboardSumaryTable-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = DashboardSummaryResModel.IDashboardSummaryRecData.Count(),
                        status = "1",
                        recordsFiltered = DashboardSummaryResModel.IDashboardSummaryRecData.Count(),
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        //ADDED BY PANKAJ SAKHARE ON 18-09-2020
                        ExcelBtn = ExcelDownloadBtn,
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadDashboardSumaryTable-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Dashboard summary Table Response." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult LoadRevenueCollectedChartData(int toggleBtnId,int DistrictCode,int SROCode)
         {
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DashboardController-LoadRevenueCollectedChartData-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

            string errorMessage = string.Empty;
            
            DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
            ViewModel.toggleBtnId = toggleBtnId;
            ViewModel.DistrictCode = DistrictCode;
            ViewModel.SROfficeID = SROCode;
            caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadRevenueCollectedChartData", ViewModel, out errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
            {


                if (toggleBtnId == 1)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        LineChart = result._SalesStatisticsLineChartModel
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadRevenueCollectedChartData-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;

                }
                if (toggleBtnId == 2)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        TableData = result._TableDataWrapper.tableSalesStatisticsDocReg,
                        TableColumns = result._TableDataWrapper.ColumnArray,
                        // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                        //ExcelBtn = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELRevenueCollectedChartData('" + toggleBtnId + "','" + DistrictCode + "','" + SROCode + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>"
                        ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;' onclick=EXCELRevenueCollectedChartData('" + toggleBtnId + "','" + DistrictCode + "','" + SROCode + "')><i class='fa fa-download fa-lg' aria-hidden='true' style='color:#484848;'></i></a>"
                        //ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;'  onclick=EXCELRevenueTargetVsAchieved('" + toggleBtnId + "','" + selectedType + "','" + DistrictCode + "')><i class='fa fa-download fa-lg' aria-hidden='true'></i></a>"
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadRevenueCollectedChartData-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020


                    return JsonData;


                }

             
            }

            return View();
        }


        [HttpPost]
        public ActionResult LoadDocumentRegisteredChartData(int toggleBtnId, int DistrictCode, int SROCode)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DashboardController-LoadDocumentRegisteredChartData-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

            string errorMessage = string.Empty;
            DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
            ViewModel.toggleBtnId = toggleBtnId;
            ViewModel.DistrictCode = DistrictCode;
            ViewModel.SROfficeID = SROCode;
            caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadDocumentRegisteredChartData", ViewModel, out errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
            {


                if (toggleBtnId == 1)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        LineChart = result._SalesStatisticsLineChartModel
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadDocumentRegisteredChartData-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;

                }
                if (toggleBtnId == 2)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        TableData = result._TableDataWrapper.tableSalesStatisticsDocReg,
                        TableColumns = result._TableDataWrapper.ColumnArray,
                        // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                        ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;' onclick=EXCELDocumentRegisteredChartData('" + toggleBtnId + "','" + DistrictCode + "','" + SROCode + "')><i class='fa fa-download fa-lg' aria-hidden='true' style='color:#484848;'></i></a>"
                        //ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;'  onclick=EXCELRevenueTargetVsAchieved('" + toggleBtnId + "','" + selectedType + "','" + DistrictCode + "')><i class='fa fa-download fa-lg' aria-hidden='true'></i></a>"
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadDocumentRegisteredChartData-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;


                }


            }

            return View();
        }

        [HttpPost]
        public ActionResult PopulateSurchargeCessBarChart(int toggleBtnId,int DistrictCode,int SROCode,string NatureOfDoc)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DashboardController-PopulateSurchargeCessBarChart-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

            string errorMessage = string.Empty;
            DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
            ViewModel.toggleBtnId = toggleBtnId;
            ViewModel.DistrictCode = DistrictCode;
            ViewModel.SROfficeID = SROCode;
            ViewModel.SNatureOfDocID = NatureOfDoc;
            caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("PopulateSurchargeCessBarChart", ViewModel, out errorMessage);

                if (string.IsNullOrEmpty(errorMessage))
                {

                if (toggleBtnId == 1)//Graph
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        BarChart = result._SurchargeAndCessBarChartModel
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-PopulateSurchargeCessBarChart-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;

                }
                if (toggleBtnId == 2)//Table
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        TableData = result._TableDataWrapper.tableDataSurchargeAndCess,
                        TableColumns = result._TableDataWrapper.ColumnArray,
                        //ADDED BY PANKAJ ON 18-09-2020
                        ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;'  onclick=EXCELSurchargeCessBarChart('" + toggleBtnId + "','" + DistrictCode + "','" + SROCode + "','" + NatureOfDoc + "') ><i class='fa fa-download fa-lg' aria-hidden='true' style='color:#484848;'></i></a>"
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-PopulateSurchargeCessBarChart-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;
                }
        }
           
            return View();
        }

        [HttpPost]
        public ActionResult LoadHighValPropChartData(int toggleBtnId,int DistrictCode,int SROCode)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DashboardController-LoadHighValPropChartData-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
            
            string errorMessage = string.Empty;


            DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
            ViewModel.toggleBtnId = toggleBtnId;
            ViewModel.DistrictCode = DistrictCode;
            ViewModel.SROfficeID = SROCode;
            caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadHighValPropChartData", ViewModel, out errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
            {


                if (toggleBtnId == 1)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        LineChart = result._HighValPropLineChartModel
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadHighValPropChartData-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;

                }
                if (toggleBtnId == 2)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        TableData = result._TableDataWrapper.TableData_HIGH_VALUE_REVENUE_COLLECTED,
                        TableColumns = result._TableDataWrapper.ColumnArray,
                        // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                        ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;' onclick=EXCELHighValPropChartData('" + toggleBtnId + "','" + DistrictCode + "','" + SROCode + "')><i class='fa fa-download fa-lg' aria-hidden='true' style='color:#484848;'></i></a>"
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadHighValPropChartData-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;


                }


            }

            return View();
        }

        [HttpPost]
        public ActionResult LoadHighValPropChartDataForDocs(int toggleBtnId, int DistrictCode, int SROCode)
        {
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
            //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
            //{
            //    string format = "{0} : {1}";
            //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
            //    file.WriteLine("-DashboardController-LoadHighValPropChartDataForDocs-IN");
            //    file.Flush();
            //}
            //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

            string errorMessage = string.Empty;


            DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
            ViewModel.toggleBtnId = toggleBtnId;
            ViewModel.DistrictCode = DistrictCode;
            ViewModel.SROfficeID = SROCode;
            caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadHighValPropChartDataForDocs", ViewModel, out errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
            {


                if (toggleBtnId == 1)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        LineChart = result._HighValPropLineChartModel
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadHighValPropChartDataForDocs-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;

                }
                if (toggleBtnId == 2)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        TableData = result._TableDataWrapper.TableData_HIGH_VALUE_REVENUE_COLLECTED,
                        TableColumns = result._TableDataWrapper.ColumnArray,
                        // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                        ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;' onclick=EXCELHighValPropChartDataForDocs('" + toggleBtnId + "','" + DistrictCode + "','" + SROCode + "')><i class='fa fa-download fa-lg' aria-hidden='true' style='color:#484848;'></i></a>"
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;

                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020
                    //using (System.IO.StreamWriter file = System.IO.File.AppendText(dashboardLogFilePath))
                    //{
                    //    string format = "{0} : {1}";
                    //    file.Write(string.Format(format, "Timestamp: ", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture)));
                    //    file.WriteLine("-DashboardController-LoadHighValPropChartDataForDocs-OUT");
                    //    file.Flush();
                    //}
                    //// ADDED BY SHUBHAM BHAGAT ON 18-08-2020

                    return JsonData;


                }


            }

            return View();
        }

        [HttpPost]
        public ActionResult LoadRevenueTargetVsAchieved(int toggleBtnId,String selectedType,int DistrictCode)
         {
            string errorMessage = string.Empty;


            DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
            ViewModel.toggleBtnId = toggleBtnId;
            ViewModel.selectedType = selectedType;
            ViewModel.DistrictCode = DistrictCode;
              caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadRevenueTargetVsAchieved", ViewModel, out errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
            {


                if (toggleBtnId == 1)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        LineChart = result._RevenueTargetVsAchievedModel
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;

                }
                if (toggleBtnId == 2)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        TableData = result._TableDataWrapper.tableDataRevTargetVsAchieved,
                        TableColumns = result._TableDataWrapper.ColumnArray,
                        // ADDED BY SHUBHAM BHAGAT ON 17-09-2020
                        //< i class="fa fa-download" aria-hidden="true"></i>
                        //ExcelBtn = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELRevenueTargetVsAchieved('" + toggleBtnId + "','" + selectedType + "','" + DistrictCode + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>"
                        ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;'  onclick=EXCELRevenueTargetVsAchieved('" + toggleBtnId + "','" + selectedType + "','" + DistrictCode + "')><i class='fa fa-download fa-lg' aria-hidden='true' style='color:#484848;'></i></a>"

                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;


                }


            }

            return View();
        }

        [HttpPost]
        public ActionResult PopulateProgressChart(int toggleBtnId, String selectedType, int DistrictCode)
        {
            string errorMessage = string.Empty;


            DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
            ViewModel.toggleBtnId = toggleBtnId;
            ViewModel.selectedType = selectedType;
            ViewModel.DistrictCode = DistrictCode;
              caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("PopulateProgressChart", ViewModel, out errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
            {


                if (toggleBtnId == 1)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        BarChart = result._ProgressChartModel
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;

                }
                if (toggleBtnId == 2)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        TableData = result._TableDataWrapper.TableDataArrayofProgressChart,
                        TableColumns = result._TableDataWrapper.ColumnArray,
                        // ADDED BY PANKAJ SAKHARE ON 17-09-2020
                        //ExcelBtn = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELProgressCurrentVsPreviousFinYear('" + toggleBtnId + "','" + selectedType + "','" + DistrictCode + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>"
                        ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;'  onclick=EXCELProgressCurrentVsPreviousFinYear('" + toggleBtnId + "','" + selectedType + "','" + DistrictCode + "')><i class='fa fa-download fa-lg' aria-hidden='true' style='color:#484848;'></i></a>"


                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;


                }


            }

            return View();
        }

        #region Dashboard Summary Tab

        [HttpGet]
        public ActionResult PopulateTiles(string selectedType, int SelectedOffice,int FinYearId)
            {
            try
            {
                caller = new ServiceCaller("DashboardAPIController");
                string errorMessage = string.Empty;
                DashboardSummaryModel summaryModel = new DashboardSummaryModel();
                int OfficeID = KaveriSession.Current.OfficeID;
                TilesReqModel tilesReqModel = new TilesReqModel();
                tilesReqModel.OfficeCode = SelectedOffice;
                tilesReqModel.selectedType = selectedType;
                tilesReqModel.OfficeID = OfficeID;
                //Added by RamanK on 18-06-2020
                tilesReqModel.FinYearId = FinYearId;
                String IsStateWise="0";
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                DashboardSummaryModel TilesModel = caller.PostCall<TilesReqModel, DashboardSummaryModel>("PopulateTiles", tilesReqModel, out errorMessage);
                if (SelectedOffice == 0)
                {
                    IsStateWise = "1";
                }
                //return Json(new { success = true, TilesModel = TilesModel ,IsStateWise= IsStateWise, RevenueCollectionWrapperModel = TilesModel._RevenueCollectionWrapperModel ,CurrentAchievementsModel= TilesModel.CurrentAchievementsModel,ProgressChartModel= TilesModel._ProgressBarTargetVsAchieved}, JsonRequestBehavior.AllowGet);
               
                return Json(new { success = true, TilesModel = TilesModel ,IsStateWise= IsStateWise  }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }

        }

        [HttpPost]

        public ActionResult LoadPopup(string selectedType, int SelectedOffice,string PopupType,string FinYear)
        {
            DashboardPopupViewModel model = new DashboardPopupViewModel();
            DashboardPopupReqModel reqModel = new DashboardPopupReqModel();
            reqModel.selectedType = selectedType;
            reqModel.SelectedOffice = SelectedOffice;
            reqModel.PopupType = PopupType;
            reqModel.FinYearId = Convert.ToInt32(FinYear);
            
            string errorMessage = "";
              caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            model = caller.PostCall<DashboardPopupReqModel, DashboardPopupViewModel>("LoadPopup", reqModel, out errorMessage);

            return View("DashboardPopupView", model);
        }
        #endregion


        #region ADDED BY SHUBHAM BHAGAT 09-04-2020
        //[HttpPost]
        public ActionResult NatureOfDocumentByRadioType(String radioType)
        {
            try
            {
                String errorMessage = String.Empty;
                NatureOfArticle_REQ_RES_Model reqModel = new NatureOfArticle_REQ_RES_Model();
                reqModel.RadioType = radioType;
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                NatureOfArticle_REQ_RES_Model resModel = caller.PostCall<NatureOfArticle_REQ_RES_Model, NatureOfArticle_REQ_RES_Model>("NatureOfDocumentByRadioType", reqModel, out errorMessage);
                if (resModel == null)
                    return Json(new { serverError = false, errorMessage = "Error in getting nature of document list." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { natureOfDocument = resModel.NatuereOfDocsList, natureOfDocumentArr = resModel.NatureOfDocID, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting nature of document list." }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion


        // ADDED BY SHUBHAM BHAGAT ON 17-09-2020


        #region Excel common
        /// <summary>
        /// GetFileInfo
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns>returns file info</returns>
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
        #endregion



        #region Excel Dashboard Tab1
        [EventAuditLogFilter(Description = "EXCEL Revenue Target Vs Achieved")]
        public ActionResult EXCELRevenueTargetVsAchieved(string toggleBtnId, string selectedType, string DistrictCode, string DistrictTextForExcel)
        {
            try
            {
                DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
                ViewModel.toggleBtnId = Convert.ToInt32(toggleBtnId);
                ViewModel.selectedType = selectedType;
                ViewModel.DistrictCode = Convert.ToInt32(DistrictCode);
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;


                string fileName = string.Format("RevenueTargetVsAchieved.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                //List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();
                GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadRevenueTargetVsAchieved", ViewModel, out errorMessage);

                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("Revenue : Target Vs Achieved");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateEXCELRevenueTargetVsAchieved(result, fileName, excelHeader, DistrictTextForExcel);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "Revenue_TargetVsAchieved_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateEXCELRevenueTargetVsAchieved(GraphTableResponseModel resModel, string fileName, string excelHeader, string DistrictTextForExcel)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Revenue : Target Vs Achieved");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader + "(" + DistrictTextForExcel + ")";
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "District : " + DistrictTextForExcel;
                    workSheet.Cells[3, 1].Value = "Total Records : " + resModel._TableDataWrapper.tableDataRevTargetVsAchieved.ToList().Count();
                    workSheet.Cells[1, 1, 1, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[2, 1, 2, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[3, 1, 3, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[4, 1, 4, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;

                    int rowIndex = 6;
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[5, 1].Value = "Fin year";
                    workSheet.Cells[5, 2].Value = "Target";
                    workSheet.Cells[5, 3].Value = "Achieved";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in resModel._TableDataWrapper.tableDataRevTargetVsAchieved)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.Fin_Years;
                        //workSheet.Cells[rowIndex, 2].Value = items.RevTarget;
                        //workSheet.Cells[rowIndex, 3].Value = items.RevAchieved;
                        workSheet.Cells[rowIndex, 2].Value = Convert.ToDecimal(items.RevTarget.Replace(",", ""));
                        workSheet.Cells[rowIndex, 3].Value = Convert.ToDecimal(items.RevAchieved.Replace(",", ""));

                        workSheet.Cells[rowIndex, 2].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    //rupees in crores
                    //rowIndex++;
                    workSheet.Cells[rowIndex, 1, rowIndex, 3].Merge = true;
                    workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 1].Value = "Note: Rupees in Crores";
                    workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 3])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }

        //PANKAJ
        [EventAuditLogFilter(Description = "EXCEL Progress Current Vs Previous Financial Year")]
        public ActionResult EXCELProgressCurrentVsPreviousFinYear(string toggleBtnId, string selectedType, string DistrictCode, string DistrictTextForExcel)
        {
            try
            {
                DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
                ViewModel.toggleBtnId = Convert.ToInt32(toggleBtnId);
                ViewModel.selectedType = selectedType;
                ViewModel.DistrictCode = Convert.ToInt32(DistrictCode);
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;


                string fileName = string.Format("ProgressCurrentVsPreviousFinYear.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                //List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();
                GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("PopulateProgressChart", ViewModel, out errorMessage);

                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("Progress: Current Vs Previous Fin. Years");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateEXCELProgressCurrentVsPreviousFinYear(result, fileName, excelHeader, DistrictTextForExcel);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "Progress_CurrentVsPreviousFinYears_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateEXCELProgressCurrentVsPreviousFinYear(GraphTableResponseModel resModel, string fileName, string excelHeader, string DistrictTextForExcel)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Progress: Current Vs Previous Fin. Years");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader + "(" + DistrictTextForExcel + ")";
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "District : " + DistrictTextForExcel;
                    workSheet.Cells[3, 1].Value = "Total Records : " + resModel._TableDataWrapper.TableDataArrayofProgressChart.ToList().Count();
                    workSheet.Cells[1, 1, 1, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[2, 1, 2, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[3, 1, 3, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[4, 1, 4, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;

                    int rowIndex = 6;
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[5, 1].Value = "Fin year";
                    workSheet.Cells[5, 2].Value = "Documents";
                    workSheet.Cells[5, 3].Value = "Revenue";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in resModel._TableDataWrapper.TableDataArrayofProgressChart)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.REGFYEAR;
                        //workSheet.Cells[rowIndex, 2].Value = items.NO_OF_DOCS_REGISTERED;
                        workSheet.Cells[rowIndex, 2].Value = Convert.ToInt32(items.NO_OF_DOCS_REGISTERED);
                        //workSheet.Cells[rowIndex, 3].Value = items.TOTAL_REVENUE;
                        workSheet.Cells[rowIndex, 3].Value = Convert.ToDecimal(items.TOTAL_REVENUE.Replace(",", ""));

                        workSheet.Cells[rowIndex, 2].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    //rupees in crores
                    //rowIndex++;
                    workSheet.Cells[rowIndex, 1, rowIndex, 3].Merge = true;
                    workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 1].Value = "Note: Rupees in Crores";
                    workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 3])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }

        //PANKAJ


        #endregion


        #region Excel Dashboard Tab2


        [EventAuditLogFilter(Description = "EXCEL Revenue Collected Chart Data")]
        public ActionResult EXCELRevenueCollectedChartData(string toggleBtnId, string DistrictCode, string SROCode, string DistrictTextForExcel, string SROTextForExcel)
        {
            try
            {
                DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
                ViewModel.toggleBtnId = Convert.ToInt32(toggleBtnId);
                ViewModel.DistrictCode = Convert.ToInt32(DistrictCode);
                ViewModel.SROfficeID = Convert.ToInt32(SROCode);
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                string fileName = string.Format("RevenueCollectedChartData.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                //List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();
                GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadRevenueCollectedChartData", ViewModel, out errorMessage);

                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("Sales Statistics : Revenue Collected");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateEXCELRevenueCollectedChartData(result, fileName, excelHeader, DistrictTextForExcel, SROTextForExcel);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "Sales_Statistics_Revenue_Collected_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateEXCELRevenueCollectedChartData(GraphTableResponseModel resModel, string fileName, string excelHeader, string DistrictTextForExcel, string SROTextForExcel)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Sales Statistics : Revenue Collected");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "District : " + DistrictTextForExcel;
                    workSheet.Cells[4, 1].Value = "SRO : " + SROTextForExcel;
                    workSheet.Cells[5, 1].Value = "Total Records : " + resModel._TableDataWrapper.tableSalesStatisticsDocReg.ToList().Count();
                    workSheet.Cells[1, 1, 1, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[2, 1, 2, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[3, 1, 3, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[4, 1, 4, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[5, 1, 5, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr No";
                    workSheet.Cells[7, 2].Value = "Fin Year";
                    workSheet.Cells[7, 3].Value = "NonAgri < 10Lakhs";
                    workSheet.Cells[7, 4].Value = "NonAgri > 10Lakhs";
                    workSheet.Cells[7, 5].Value = "Agri < 10Lakhs";
                    workSheet.Cells[7, 6].Value = "Agri > 10Lakhs";
                    workSheet.Cells[7, 7].Value = "Flats / Apartments";
                    workSheet.Cells[7, 8].Value = "Lease";
                    

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in resModel._TableDataWrapper.tableSalesStatisticsDocReg)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
                        

                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        workSheet.Cells[rowIndex, 2].Value = items.FinYear;
                        workSheet.Cells[rowIndex, 3].Value = Convert.ToDecimal(items.NonAgriLessThan10Lakhs.Replace(",", ""));
                        workSheet.Cells[rowIndex, 4].Value = Convert.ToDecimal(items.NonAgriGreaterThan10Lakhs.Replace(",", ""));
                        workSheet.Cells[rowIndex, 5].Value = Convert.ToDecimal(items.AgriLessThan10Lakhs.Replace(",", ""));
                        workSheet.Cells[rowIndex, 6].Value = Convert.ToDecimal(items.AgriGreaterThan10Lakhs.Replace(",", ""));
                        workSheet.Cells[rowIndex, 7].Value = Convert.ToDecimal(items.FlatsApartment.Replace(",", ""));
                        workSheet.Cells[rowIndex, 8].Value = Convert.ToDecimal(items.Lease.Replace(",", ""));

                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    // ADDED BY SHUBHAM BHAGAT ON 18-09-2020
                    // FOR ADDING NOTES IN LAST LINE
                    workSheet.Cells[rowIndex, 1].Value = "Note: Rupees in Crores";
                    workSheet.Cells[rowIndex, 1, rowIndex, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    //workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 8])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }


        [EventAuditLogFilter(Description = "EXCEL Document Registered Chart Data")]
        public ActionResult EXCELDocumentRegisteredChartData(string toggleBtnId, string DistrictCode, string SROCode, string DistrictTextForExcel, string SROTextForExcel)
        {
            try
            {
                DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
                ViewModel.toggleBtnId = Convert.ToInt32(toggleBtnId);
                ViewModel.DistrictCode = Convert.ToInt32(DistrictCode);
                ViewModel.SROfficeID = Convert.ToInt32(SROCode);
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                string fileName = string.Format("DocumentRegisteredChartData.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                //List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();
                GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadDocumentRegisteredChartData", ViewModel, out errorMessage);

                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("Sales Statistics : Documents Registered");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateEXCELDocumentRegisteredChartData(result, fileName, excelHeader, DistrictTextForExcel, SROTextForExcel);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "Sales_Statistics_Documents_Registered_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateEXCELDocumentRegisteredChartData(GraphTableResponseModel resModel, string fileName, string excelHeader, string DistrictTextForExcel, string SROTextForExcel)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Sales Statistics : Documents Registered");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "District : " + DistrictTextForExcel;
                    workSheet.Cells[4, 1].Value = "SRO : " + SROTextForExcel;
                    workSheet.Cells[5, 1].Value = "Total Records : " + resModel._TableDataWrapper.tableSalesStatisticsDocReg.ToList().Count();
                    workSheet.Cells[1, 1, 1, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[2, 1, 2, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[3, 1, 3, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[4, 1, 4, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[5, 1, 5, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr No";
                    workSheet.Cells[7, 2].Value = "Fin Year";
                    workSheet.Cells[7, 3].Value = "NonAgri < 10Lakhs";
                    workSheet.Cells[7, 4].Value = "NonAgri > 10Lakhs";
                    workSheet.Cells[7, 5].Value = "Agri < 10Lakhs";
                    workSheet.Cells[7, 6].Value = "Agri > 10Lakhs";
                    workSheet.Cells[7, 7].Value = "Flats / Apartments";
                    workSheet.Cells[7, 8].Value = "Lease";


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in resModel._TableDataWrapper.tableSalesStatisticsDocReg)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        workSheet.Cells[rowIndex, 2].Value = items.FinYear;
                        workSheet.Cells[rowIndex, 3].Value = Convert.ToInt32(items.NonAgriLessThan10Lakhs.Replace(",",""));
                        workSheet.Cells[rowIndex, 4].Value = Convert.ToInt32(items.NonAgriGreaterThan10Lakhs.Replace(",", ""));
                        workSheet.Cells[rowIndex, 5].Value = Convert.ToInt32(items.AgriLessThan10Lakhs.Replace(",", ""));
                        workSheet.Cells[rowIndex, 6].Value = Convert.ToInt32(items.AgriGreaterThan10Lakhs.Replace(",", ""));
                        workSheet.Cells[rowIndex, 7].Value = Convert.ToInt32(items.FlatsApartment.Replace(",", ""));
                        workSheet.Cells[rowIndex, 8].Value = Convert.ToInt32(items.Lease.Replace(",", ""));

                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    //// ADDED BY SHUBHAM BHAGAT ON 18-09-2020
                    //// FOR ADDING NOTES IN LAST LINE
                    //workSheet.Cells[rowIndex, 1].Value = "Note: Rupees in Crores";
                    //workSheet.Cells[rowIndex, 1, rowIndex, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    ////workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    //workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 8])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }


        [EventAuditLogFilter(Description = "EXCEL High Val Prop Chart Data")]
        public ActionResult EXCELHighValPropChartData(string toggleBtnId, string DistrictCode, string SROCode, string DistrictTextForExcel, string SROTextForExcel)
        {
            try
            {
                DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
                ViewModel.toggleBtnId = Convert.ToInt32(toggleBtnId);
                ViewModel.DistrictCode = Convert.ToInt32(DistrictCode);
                ViewModel.SROfficeID = Convert.ToInt32(SROCode);
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                string fileName = string.Format("HighValPropChartData.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                //List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();
                GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadHighValPropChartData", ViewModel, out errorMessage);

                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("High value properties : Revenue Collected");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateEXCELHighValPropChartData(result, fileName, excelHeader, DistrictTextForExcel, SROTextForExcel);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "High_value_properties_Revenue_Collected_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateEXCELHighValPropChartData(GraphTableResponseModel resModel, string fileName, string excelHeader, string DistrictTextForExcel, string SROTextForExcel)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("High value properties : Revenue Collected");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "District : " + DistrictTextForExcel;
                    workSheet.Cells[4, 1].Value = "SRO : " + SROTextForExcel;
                    workSheet.Cells[5, 1].Value = "Total Records : " + resModel._TableDataWrapper.TableData_HIGH_VALUE_REVENUE_COLLECTED.ToList().Count();
                    workSheet.Cells[1, 1, 1, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[2, 1, 2, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[3, 1, 3, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[4, 1, 4, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[5, 1, 5, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    //workSheet.Column(8).Width = 30;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr No";
                    workSheet.Cells[7, 2].Value = "Fin Year";
                    workSheet.Cells[7, 3].Value = "One Lakh To Ten Lakhs";
                    workSheet.Cells[7, 4].Value = "Ten Lakhs To One Crore";
                    workSheet.Cells[7, 5].Value = "One Crore To Five Crore";
                    workSheet.Cells[7, 6].Value = "Five Crore To Ten Crore";
                    workSheet.Cells[7, 7].Value = "Above Ten Crore";
                    //workSheet.Cells[7, 8].Value = "Lease";


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in resModel._TableDataWrapper.TableData_HIGH_VALUE_REVENUE_COLLECTED)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        workSheet.Cells[rowIndex, 2].Value = items.FinYear;
                        workSheet.Cells[rowIndex, 3].Value = Convert.ToDecimal(items.OneLakhToTenLakhs.Replace(",", ""));
                        workSheet.Cells[rowIndex, 4].Value = Convert.ToDecimal(items.TenLakhsToOneCrore.Replace(",", ""));
                        workSheet.Cells[rowIndex, 5].Value = Convert.ToDecimal(items.OneCroreToFiveCrore.Replace(",", ""));
                        workSheet.Cells[rowIndex, 6].Value = Convert.ToDecimal(items.FiveCroreToTenCrore.Replace(",", ""));
                        workSheet.Cells[rowIndex, 7].Value = Convert.ToDecimal(items.AboveTenCrore.Replace(",", ""));
                        //workSheet.Cells[rowIndex, 8].Value = Convert.ToDecimal(items.Lease);

                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        //workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    // ADDED BY SHUBHAM BHAGAT ON 18-09-2020
                    // FOR ADDING NOTES IN LAST LINE
                    workSheet.Cells[rowIndex, 1].Value = "Note: Rupees in Crores";
                    workSheet.Cells[rowIndex, 1, rowIndex, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    //workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }


        [EventAuditLogFilter(Description = "EXCEL High Val Prop Chart Data For Docs")]
        public ActionResult EXCELHighValPropChartDataForDocs(string toggleBtnId, string DistrictCode, string SROCode, string DistrictTextForExcel, string SROTextForExcel)
        {
            try
            {
                DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
                ViewModel.toggleBtnId = Convert.ToInt32(toggleBtnId);
                ViewModel.DistrictCode = Convert.ToInt32(DistrictCode);
                ViewModel.SROfficeID = Convert.ToInt32(SROCode);
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                string fileName = string.Format("HighValPropChartDataForDocs.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                //List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();
                GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("LoadHighValPropChartDataForDocs", ViewModel, out errorMessage);

                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("High value properties : Documents Registered");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateEXCELHighValPropChartDataForDocs(result, fileName, excelHeader, DistrictTextForExcel, SROTextForExcel);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "High_value_properties_Documents_Registered_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateEXCELHighValPropChartDataForDocs(GraphTableResponseModel resModel, string fileName, string excelHeader, string DistrictTextForExcel, string SROTextForExcel)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("High value properties : Documents Registered");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "District : " + DistrictTextForExcel;
                    workSheet.Cells[4, 1].Value = "SRO : " + SROTextForExcel;
                    workSheet.Cells[5, 1].Value = "Total Records : " + resModel._TableDataWrapper.TableData_HIGH_VALUE_REVENUE_COLLECTED.ToList().Count();
                    workSheet.Cells[1, 1, 1, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[2, 1, 2, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[3, 1, 3, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[4, 1, 4, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[5, 1, 5, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr No";
                    workSheet.Cells[7, 2].Value = "Fin Year";
                    workSheet.Cells[7, 3].Value = "One Lakh To Ten Lakhs";
                    workSheet.Cells[7, 4].Value = "Ten Lakhs To One Crore";
                    workSheet.Cells[7, 5].Value = "One Crore To Five Crore";
                    workSheet.Cells[7, 6].Value = "Five Crore To Ten Crore";
                    workSheet.Cells[7, 7].Value = "Above Ten Crore";
                    //workSheet.Cells[7, 8].Value = "Lease";


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in resModel._TableDataWrapper.TableData_HIGH_VALUE_REVENUE_COLLECTED)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        workSheet.Cells[rowIndex, 2].Value = items.FinYear;
                        workSheet.Cells[rowIndex, 3].Value = Convert.ToInt32(items.OneLakhToTenLakhs.Replace(",",""));
                        workSheet.Cells[rowIndex, 4].Value = Convert.ToInt32(items.TenLakhsToOneCrore.Replace(",",""));
                        workSheet.Cells[rowIndex, 5].Value = Convert.ToInt32(items.OneCroreToFiveCrore.Replace(",",""));
                        workSheet.Cells[rowIndex, 6].Value = Convert.ToInt32(items.FiveCroreToTenCrore.Replace(",",""));
                        workSheet.Cells[rowIndex, 7].Value = Convert.ToInt32(items.AboveTenCrore.Replace(",",""));
                        //workSheet.Cells[rowIndex, 8].Value = Convert.ToDecimal(items.Lease);

                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0";
                        //workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    //// ADDED BY SHUBHAM BHAGAT ON 18-09-2020
                    //// FOR ADDING NOTES IN LAST LINE
                    //workSheet.Cells[rowIndex, 1].Value = "Note: Rupees in Crores";
                    //workSheet.Cells[rowIndex, 1, rowIndex, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    ////workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    //workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }

        //SUMMARY TABLE EXCEL
        //ADDED BY PANKAJ SAKHARE ON 18-09-2020
        [EventAuditLogFilter(Description = "EXCEL Dashboard Summary Table")]
        public ActionResult EXCELDashboardSummaryTable(string DistrictId, string SroId, string NatureOfDocID, string FinYearID, string DistrictText, string SroText)
        {
            try
            {
                DashboardDetailsViewModel reqModel = new DashboardDetailsViewModel();
                reqModel.SNatureOfDocID = NatureOfDocID;
                reqModel.SROfficeID = Convert.ToInt32(SroId);
                reqModel.DROfficeID = Convert.ToInt32(DistrictId);
                reqModel.FinYearId = Convert.ToInt32(FinYearID);
                reqModel.IsForExcel= true;
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller = new ServiceCaller("DashboardAPIController");
                string fileName = string.Format("DashboardSummary.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;


                DashboardSummaryTblResModel result = caller.PostCall<DashboardDetailsViewModel, DashboardSummaryTblResModel>("LoadDashboardSumaryTable", reqModel, out errorMessage);

                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("Dashboard Summary");
                string createdExcelPath = CreateEXCELDashboardSummaryTable(result, fileName, excelHeader, FinYearID, DistrictText, SroText);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "DashboardSummary_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        private string CreateEXCELDashboardSummaryTable(DashboardSummaryTblResModel resModel, string fileName, string excelHeader, String FinYearID, String DistrictText, String SroText)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Dashboard Summary");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "District : " + DistrictText;
                    workSheet.Cells[4, 1].Value = "SRO : " + SroText;
                    workSheet.Cells[5, 1].Value = "Total Records : " + resModel.IDashboardSummaryRecData.Count();

                    workSheet.Cells[1, 1, 1, 8].Merge = true;
                    workSheet.Cells[2, 1, 2, 8].Merge = true;
                    workSheet.Cells[3, 1, 3, 8].Merge = true;
                    workSheet.Cells[4, 1, 4, 8].Merge = true;
                    workSheet.Cells[5, 1, 5, 8].Merge = true;
                    workSheet.Cells[6, 1, 6, 8].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 50;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 40;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                    workSheet.Row(8).Style.Font.Bold = true;

                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 2].Value = "Day Wise";
                    workSheet.Cells[7, 2, 7, 3].Merge = true;


                    workSheet.Cells[7, 4].Value = "Month Wise";
                    workSheet.Cells[7, 4, 7, 5].Merge = true;

                    workSheet.Cells[7, 6].Value = "Financial year wise";
                    workSheet.Cells[7, 6, 7, 7].Merge = true;

                    workSheet.Cells[7, 8].Value = "Cumulative";

                    workSheet.Cells[7, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[8, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[8, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[8, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[8, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[8, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[8, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[8, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;



                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Cells[7, 1, 8, 1].Merge = true;
                    int rowIndex = 9;


                    workSheet.Cells[8, 2].Value = "Today";
                    workSheet.Cells[8, 3].Value = "Yesterday";
                    workSheet.Cells[8, 4].Value = "Current Month";
                    workSheet.Cells[8, 5].Value = "Previous Month";
                    workSheet.Cells[8, 6].Value = "2020-21 Fin. Year";
                    workSheet.Cells[8, 7].Value = "2019-20 Fin. Year";
                    workSheet.Cells[8, 8].Value = "Upto 2020-21 Fin. Year";

                    foreach (var items in resModel.IDashboardSummaryRecData)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 1].Value = items.Description;
                        workSheet.Cells[rowIndex, 2].Value = items.Today;
                        workSheet.Cells[rowIndex, 3].Value = items.Yesterday;
                        workSheet.Cells[rowIndex, 4].Value = items.CurrentMonth;
                        workSheet.Cells[rowIndex, 5].Value = items.PreviousMonth;
                        workSheet.Cells[rowIndex, 6].Value = items.CurrentFinYear;
                        workSheet.Cells[rowIndex, 7].Value = items.PrevFinYear;
                        workSheet.Cells[rowIndex, 8].Value = items.UptoCurrentFinYear;


                        workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;


                        rowIndex++;

                    }

                    //ADDED FOR SHOWING NOTE
                    workSheet.Cells[rowIndex, 1, rowIndex, 8].Merge = true;
                    workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    //workSheet.Cells[rowIndex, 1].Value = "* This Data is Coming from September 2019 onwards";
                    workSheet.Cells[rowIndex, 1].Value = "* This data is not completely available for given period";
                    workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 8])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }
        //SUMMARY TABLE EXCEL

        //SURCHAGRECESS
        [EventAuditLogFilter(Description = "EXCEL Surcharge and Cess Collected")]
        public ActionResult EXCELSurchargeCess(int toggleBtnId, int DistrictCode, int SROCode, string NatureOfDoC, String DistrictText, String SroText)
        {
            try
            {

                string errorMessage = string.Empty;
                DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
                ViewModel.toggleBtnId = toggleBtnId;
                ViewModel.DistrictCode = DistrictCode;
                ViewModel.SROfficeID = SROCode;
                ViewModel.SNatureOfDocID = NatureOfDoC;
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("PopulateSurchargeCessBarChart", ViewModel, out errorMessage);

                //

                string fileName = string.Format("SurchargeAndChessCollected.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("Surcharge And Cess Collected");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateEXCELSurchargeCess(result, fileName, excelHeader, DistrictText, SroText);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "SurchargeAndChessCollected_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        private string CreateEXCELSurchargeCess(GraphTableResponseModel resModel, string fileName, string excelHeader, string DistrictText, String SroText)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Progress: Current Vs Previous Fin. Years");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "District : " + DistrictText;
                    workSheet.Cells[4, 1].Value = "SRO : " + SroText;
                    workSheet.Cells[5, 1].Value = "Total Records : " + resModel._TableDataWrapper.tableDataSurchargeAndCess.ToList().Count();

                    workSheet.Cells[1, 1, 1, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[2, 1, 2, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[3, 1, 3, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[4, 1, 4, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[5, 1, 5, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[6, 1, 6, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 1].Value = "Sr No";
                    workSheet.Cells[7, 2].Value = "Fin Year";
                    workSheet.Cells[7, 3].Value = "Surcharge";
                    workSheet.Cells[7, 4].Value = "Cess";
                    workSheet.Cells[7, 5].Value = "Total";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in resModel._TableDataWrapper.tableDataSurchargeAndCess)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        workSheet.Cells[rowIndex, 2].Value = items.Fin_Years;
                        workSheet.Cells[rowIndex, 3].Value = Convert.ToDecimal(items.Surcharge.Replace(",", ""));
                        workSheet.Cells[rowIndex, 4].Value = Convert.ToDecimal(items.Cess.Replace(",", ""));
                        workSheet.Cells[rowIndex, 5].Value = Convert.ToDecimal(items.Total.Replace(",", ""));


                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        rowIndex++;
                    }

                    //rupees in crores
                    //rowIndex++;
                    workSheet.Cells[rowIndex, 1, rowIndex, 5].Merge = true;
                    workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 1].Value = "Note: Rupees in Crores";
                    workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 5])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }


        #endregion

        [HttpGet]
        public ActionResult PopulateAvgRegTime(string selectedType, int SelectedOffice, int FinYearId)
        {
            try
            {
                caller = new ServiceCaller("DashboardAPIController");
                string errorMessage = string.Empty;
                DashboardSummaryModel summaryModel = new DashboardSummaryModel();
                int OfficeID = KaveriSession.Current.OfficeID;
                TilesReqModel tilesReqModel = new TilesReqModel();
                tilesReqModel.OfficeCode = SelectedOffice;
                tilesReqModel.selectedType = selectedType;
                tilesReqModel.OfficeID = OfficeID;
                //Added by RamanK on 18-06-2020
                tilesReqModel.FinYearId = FinYearId;
                String IsStateWise = "0";
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                DashboardSummaryModel TilesModel = caller.PostCall<TilesReqModel, DashboardSummaryModel>("PopulateAvgRegTime", tilesReqModel, out errorMessage);
                if (SelectedOffice == 0)
                {
                    IsStateWise = "1";
                }
                //return Json(new { success = true, TilesModel = TilesModel ,IsStateWise= IsStateWise, RevenueCollectionWrapperModel = TilesModel._RevenueCollectionWrapperModel ,CurrentAchievementsModel= TilesModel.CurrentAchievementsModel,ProgressChartModel= TilesModel._ProgressBarTargetVsAchieved}, JsonRequestBehavior.AllowGet);

                return Json(new { success = true, TilesModel = TilesModel, IsStateWise = IsStateWise }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                throw;
            }

        }

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
        [HttpPost]
        public ActionResult PopulateProgressChartMonthWise(int toggleBtnId, String selectedType, int DistrictCode,String FinYear)
        {
            string errorMessage = string.Empty;


            DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
            ViewModel.toggleBtnId = toggleBtnId;
            ViewModel.selectedType = selectedType;
            ViewModel.DistrictCode = DistrictCode;
            ViewModel.FinYear = FinYear;            
            caller = new ServiceCaller("DashboardAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("PopulateProgressChartMonthWise", ViewModel, out errorMessage);

            if (string.IsNullOrEmpty(errorMessage))
            {


                if (toggleBtnId == 1)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        BarChart = result._ProgressChartModel
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;

                }
                if (toggleBtnId == 2)
                {
                    var JsonData = Json(new
                    {
                        status = true,
                        TableData = result._TableDataWrapper.TableDataArrayofProgressChart,
                        TableColumns = result._TableDataWrapper.ColumnArray,
                        // ADDED BY PANKAJ SAKHARE ON 17-09-2020
                        //ExcelBtn = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELProgressCurrentVsPreviousFinYear('" + toggleBtnId + "','" + selectedType + "','" + DistrictCode + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>"
                        ExcelBtn = "<a  data-toggle='tooltip' data-placement='top' title='Download'  style = 'width:75%;'  onclick=EXCELProgressCurrentVsPreviousMonths('" + toggleBtnId + "','" + selectedType + "','" + DistrictCode + "','" + FinYear + "')><i class='fa fa-download fa-lg' aria-hidden='true' style='color:#484848;'></i></a>"


                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;


                }


            }

            return View();
        }


        [EventAuditLogFilter(Description = "EXCEL Progress Current Vs Previous Months")]
        public ActionResult EXCELProgressCurrentVsPreviousMonths(string toggleBtnId, string selectedType, string DistrictCode, string DistrictTextForExcel, string FinYear)
        {
            try
            {
                DashboardDetailsViewModel ViewModel = new DashboardDetailsViewModel();
                ViewModel.toggleBtnId = Convert.ToInt32(toggleBtnId);
                ViewModel.selectedType = selectedType;
                ViewModel.DistrictCode = Convert.ToInt32(DistrictCode);
                ViewModel.FinYear = FinYear;
                caller = new ServiceCaller("DashboardAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;


                string fileName = string.Format("EXCELProgressCurrentVsPreviousMonths.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                //List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();
                GraphTableResponseModel result = caller.PostCall<DashboardDetailsViewModel, GraphTableResponseModel>("PopulateProgressChartMonthWise", ViewModel, out errorMessage);

                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("Progress: Current Vs Previous Fin. Years");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateEXCELProgressCurrentVsPreviousMonths(result, fileName, excelHeader, DistrictTextForExcel,FinYear);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "Progress_CurrentVsPreviousFinYears_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateEXCELProgressCurrentVsPreviousMonths(GraphTableResponseModel resModel, string fileName, string excelHeader, string DistrictTextForExcel,string FinYear)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Progress: Current Vs Previous Fin. Years");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader + "(" + DistrictTextForExcel + ")";
                    workSheet.Cells[2, 1].Value = "Financial year : " + FinYear;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "District : " + DistrictTextForExcel;
                    workSheet.Cells[4, 1].Value = "Total Records : " + resModel._TableDataWrapper.TableDataArrayofProgressChart.ToList().Count();
                    workSheet.Cells[1, 1, 1, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[2, 1, 2, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[3, 1, 3, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[4, 1, 4, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Cells[5, 1, 5, resModel._TableDataWrapper.ColumnArray.Count()].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;

                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[6, 1].Value = "Month";
                    workSheet.Cells[6, 2].Value = "Documents";
                    workSheet.Cells[6, 3].Value = "Revenue";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in resModel._TableDataWrapper.TableDataArrayofProgressChart)
                    {

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.MonthName;
                        //workSheet.Cells[rowIndex, 2].Value = items.NO_OF_DOCS_REGISTERED;
                        workSheet.Cells[rowIndex, 2].Value = Convert.ToInt32(items.NO_OF_DOCS_REGISTERED);
                        //workSheet.Cells[rowIndex, 3].Value = items.TOTAL_REVENUE;
                        workSheet.Cells[rowIndex, 3].Value = Convert.ToDecimal(items.TOTAL_REVENUE.Replace(",", ""));

                        workSheet.Cells[rowIndex, 2].Style.Numberformat.Format = "0";
                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    //rupees in crores
                    //rowIndex++;
                    workSheet.Cells[rowIndex, 1, rowIndex, 3].Merge = true;
                    workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 1].Value = "Note: Rupees in Crores";
                    workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 3])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelFilePath;
        }


        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
    }
}