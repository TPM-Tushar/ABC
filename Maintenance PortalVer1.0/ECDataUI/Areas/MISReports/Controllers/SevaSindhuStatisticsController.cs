/*
 * Author: Rushikesh Chaudhari
 * Class Name: SevaSindhuStatisticsController.cs
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ECDataUI.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System.Linq.Dynamic;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ECDataUI.Session;
using OfficeOpenXml.Style;
using ECDataUI.Filters;
using CustomModels.Models.MISReports.SevaSidhuStatistics;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorization]
    public class SevaSindhuStatisticsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        // GET: MISReports/SevaSindhuStatisticsReportView
        /// <summary>
        /// Seva Sindhu Report
        /// </summary>
        /// <returns>returns Seva sindhu statistics report view</returns>
        [EventAuditLogFilter(Description = "Seva Sindhu Statistics Report View")]
      
        [MenuHighlight]
        public ActionResult SevaSindhuStatisticsReportView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DailyRevenueArticleWise;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
                SevaSindhuStatisticsReportModel reqModel = caller.GetCall<SevaSindhuStatisticsReportModel>("SevaSindhuStatisticsReportView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Seva Sindhu Statistics Report View", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// Get Seva Sindhu Statistics Report Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Seva Sindhu Statistics Report Details list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Seva Sindhu Report Details")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetSevaSindhuReportDetails(FormCollection formCollection)
         {
            caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeListID = formCollection["DROfficeListID"];

                int SroId = Convert.ToInt32(SROOfficeListID);
                int DroId= Convert.ToInt32(DROfficeListID);
                /*
                if (DroId == 0)
                {
                    return Json(new { success = false, errorMessage = "Please select DRO from the list" });
                }
                */
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date"
                });
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                if (string.IsNullOrEmpty(fromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date is required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "To Date is required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                DateTime frmDate, toDate;
                bool boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                bool boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                #region Validate date Inputs
                if (!boolFrmDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid From Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (!boolToDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);

                if (frmDate > toDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion


                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                SevaSindhuStatisticsReportModel reqModel = new SevaSindhuStatisticsReportModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.fromDate = fromDate;
                reqModel.ToDate_Str = ToDate;
                
                reqModel.fromDateTime = frmDate;
                reqModel.ToDate = toDate;
                //reqModel.selectedYear = Convert.ToInt32(selectedYear);
                //reqModel.selectedMonth = Convert.ToInt32(selectedMonth);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);


                int totalCount = caller.PostCall<SevaSindhuStatisticsReportModel, int>("SevaSindhuStatisticsReportDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                IEnumerable<SevaSindhuStatisticsReportDetailModel> result = caller.PostCall<SevaSindhuStatisticsReportModel, List<SevaSindhuStatisticsReportDetailModel>>("SevaSindhuReportDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Seva Sindhu Statistics Date Wise Details" });
                }
                totalCount = result.Count();
                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
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
                    else
                    {

                        searchValue = searchValue.Trim();
                        //result = result.Where(m => (m.SROoffice.ToLower().Contains(searchValue.ToLower())));

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(IndexIIReportsDetailsModel => new
                {
                    SRNo = IndexIIReportsDetailsModel.SRNo,
                    SROoffice = IndexIIReportsDetailsModel.SROoffice,
                    Application_received_date = IndexIIReportsDetailsModel.Application_received_date,
                    No_of_Application_Received = IndexIIReportsDetailsModel.No_of_Application_Received,
                    No_of_Application_Processed = IndexIIReportsDetailsModel.No_of_Application_Processed,
                    No_of_Application_Registered = IndexIIReportsDetailsModel.No_of_Application_Registered,
                    No_of_Application_Rejected = IndexIIReportsDetailsModel.No_of_Application_Rejected,
                    
            });
                //If SRO is NULL then Display ALL otherwise Print SRO_Name
                //string gridDT = gridData.FirstOrDefault(x => x.SROoffice != null)?.SROoffice.ToString() ?? string.Empty;
                string gridDT = gridData.Any(x => x.SROoffice != null)
                               ? gridData.First(x => x.SROoffice != null).SROoffice.ToString()
                               : " ";

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:auto;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + "" + "','" + "" + "','" + "" + "','" + DROfficeListID + "','" + SROOfficeListID + "','" +SROOfficeListID + "','" + DROfficeListID + "')><i style = 'padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        //data = gridData.ToArray(),
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Seva Sindhu Statistics Date Wise Details" });
            }
        }


        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                string errormessage = string.Empty;
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }
        /// <summary>
        /// Get Seva Sindhu Statistics Report Year Wise
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Seva Sindhu Statistics Report Details Year wise list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Seva Sindhu Statistics Report Year Wise")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult SevaSindhuStatisticsReportDetailsYearWise(FormCollection formCollection)
        {
            caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                string selectedYear = formCollection["selectedYear"];
                //string selectedMonth = formCollection["selectedMonth"];
               
                string IsYearWise = formCollection["isYearWise"];
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                string Amount = formCollection["Amount"];

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = ""
                });
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                SevaSindhuStatisticsReportModel reqModel = new SevaSindhuStatisticsReportModel();

                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.selectedYear = Convert.ToInt32(selectedYear);
                //reqModel.selectedMonth = Convert.ToInt32(selectedMonth);

                int totalCount = 0;// caller.PostCall<SevaSindhuStatisticsReportModel, int>("DailyRevenueReportDetailsYearWise", reqModel, out errorMessage);

                IEnumerable<SevaSindhuStatisticsReportDetailModel> result = caller.PostCall<SevaSindhuStatisticsReportModel, List<SevaSindhuStatisticsReportDetailModel>>("SevaSindhuStatisticsReportDetailsYearWise", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Seva Sindhu Statistics Report Year Wise Details" });
                }
                totalCount = result.Count();

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
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
                    else
                    {
                        searchValue = searchValue.Trim();
                        result = result.Where(m => (m.SROoffice.ToLower().Contains(searchValue.ToLower())));

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(IndexIIReportsDetailsModel => new
                {
                    SRNo = IndexIIReportsDetailsModel.SRNo,
                    SROoffice = IndexIIReportsDetailsModel.SROoffice,
                    Application_Received_Year = IndexIIReportsDetailsModel.Application_Received_Year,
                    No_of_Application_Received = IndexIIReportsDetailsModel.No_of_Application_Received,
                    No_of_Application_Processed = IndexIIReportsDetailsModel.No_of_Application_Processed,
                    No_of_Application_Registered = IndexIIReportsDetailsModel.No_of_Application_Registered,
                    No_of_Application_Rejected = IndexIIReportsDetailsModel.No_of_Application_Rejected
                });

                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + DROfficeListID + "','" + IsYearWise + "','" + selectedYear + "','" + selectedMonth + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:auto;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + String.Empty + "','" + String.Empty + "','" + IsYearWise + "','" + selectedYear + "','" + "" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        //data = gridData.ToArray(),
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Seva Sindhu Statistics Report Year Wise Details" });
            }
        }

        [HttpPost]
        [EventAuditLogFilter(Description = "Get Seva Sindhu Statistics Report Details MonthWise")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadSevaSindhuStatisticsReportTblMonthWise(FormCollection formCollection)
        {
            caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
               
                string selectedYear = formCollection["selectedYear"];
                string IsMonthWise = formCollection["IsMonthWise"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeListID = formCollection["DROfficeListID"];
                int SroId = Convert.ToInt32(SROOfficeListID);
                int DroId = Convert.ToInt32(DROfficeListID);
                
                /*
                if (DroId == 0)
                {
                    return Json(new { success = false, errorMessage = "Please select DRO from the list" });
                }
                */

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = ""
                });
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
               
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                SevaSindhuStatisticsReportModel reqModel = new SevaSindhuStatisticsReportModel();

                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);
                reqModel.selectedYear = Convert.ToInt32(selectedYear);

                int totalCount = 0;

                IEnumerable<SevaSindhuStatisticsReportDetailModel> result = caller.PostCall<SevaSindhuStatisticsReportModel, List<SevaSindhuStatisticsReportDetailModel>>("LoadSevaSindhuStatisticsReportTblMonthWise", reqModel, out errorMessage);
                
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Seva Sindhu Statistics Month Wise Details" });
                }

                totalCount = result.Count();

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
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
                    else
                    {
                        searchValue = searchValue.Trim();
                        //result = result.Where(m => (m.SROoffice.ToLower().Contains(searchValue.ToLower())));

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(SevaSindhuStatisticsReportDetailModel => new
                { 
                    SRNo = SevaSindhuStatisticsReportDetailModel.SRNo,
                    SROoffice = SevaSindhuStatisticsReportDetailModel.SROoffice,
                    Application_Received_Month = SevaSindhuStatisticsReportDetailModel.Application_Received_Month,
                    No_of_Application_Received = SevaSindhuStatisticsReportDetailModel.No_of_Application_Received,
                    No_of_Application_Processed = SevaSindhuStatisticsReportDetailModel.No_of_Application_Processed,
                    No_of_Application_Registered = SevaSindhuStatisticsReportDetailModel.No_of_Application_Registered,
                    No_of_Application_Rejected = SevaSindhuStatisticsReportDetailModel.No_of_Application_Rejected

                });

                //string gridDT = gridData.FirstOrDefault(x => x.SROoffice != null)?.SROoffice.ToString() ?? string.Empty;
                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + DROfficeListID + "','" + IsYearWise + "','" + selectedYear + "','" + selectedMonth + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                string gridDT = gridData.Any(x => x.SROoffice != null)
                               ? gridData.First(x => x.SROoffice != null).SROoffice.ToString()
                               : " ";

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:auto;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "" + "','" + "" + "','" + "" + "','" + selectedYear + "','"+ IsMonthWise + "','" + DROfficeListID + "','" +SROOfficeListID+"','" + SROOfficeListID + "','" + DROfficeListID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });

                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        //data = gridData.ToArray(),
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Seva Sindhu Statistics Report Month Wise Details" });
            }
        }

        #region Excel
        /// <summary>
        /// Seva Sindhu Statistics Report To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="isYearWise"></param>
        /// <param name="selectedYear"></param>
        /// <param name="selectedMonth"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Seva Sindhu Statistics Report To Excel")]
        public ActionResult SevaSindhuStatisticsReportToExcel(string FromDate, string ToDate, string selectedSROText, string selectedDistrictText, string SROOfficeListID, string DROfficeListID)
        {
            try
            {
                caller = new ServiceCaller("SevaSindhuStatisticsAPIController");

                string fileName = "";
                DateTime frmDate, toDate;

                SevaSindhuStatisticsReportModel reqModel = new SevaSindhuStatisticsReportModel();
                
                    fileName = string.Format("SevaSindhuStatisticsReport.xlsx");
                    DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                    DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                    reqModel.fromDateTime = frmDate;
                    reqModel.ToDate = toDate;
                    reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                    reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);
                    reqModel.isExcelDownload = true; 

                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                List<SevaSindhuStatisticsReportDetailModel> objListItemsToBeExported = new List<SevaSindhuStatisticsReportDetailModel>();
                reqModel.IsExcel = true;
                caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                reqModel.IsExcel = true;
                
                objListItemsToBeExported = caller.PostCall<SevaSindhuStatisticsReportModel, List<SevaSindhuStatisticsReportDetailModel>>("SevaSindhuReportDetails", reqModel, out errorMessage);
               
                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }

                string excelHeader = "Seva Sindhu Statistics Date Wise Report";// string.Format("Daily Revenue Report Between ({0} and {1})", FromDate, ToDate);
               
                string createdExcelPath = CreateSevaSindhuStatisticsReportExcel(objListItemsToBeExported, fileName, excelHeader, FromDate, ToDate, selectedSROText, selectedDistrictText, SROOfficeListID, DROfficeListID);
               
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                
                    return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "SevaSindhuStatisticsReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }

        }

        #endregion
        /// <summary>
        /// Create Seva Sindhu Statistics Report Excel
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="isYearWise"></param>
        /// <returns></returns>
        private string CreateSevaSindhuStatisticsReportExcel(List<SevaSindhuStatisticsReportDetailModel> objListItemsToBeExported, string fileName, string excelHeader, string FromDate, string ToDate, string selectedSROText, string selectedDistrictText, string SROOfficeListID, string DROfficeListID)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SevaSindhuStatisticsReport");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 8].Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[4, 1].Value = "Sr No";
                    //workSheet.Cells[4, 2].Value = "SRO Office";
                    workSheet.Cells[4, 2].Value = "Application Received Date";
                    workSheet.Cells[4, 3].Value = "No of Application Received";
                    workSheet.Cells[4, 4].Value = "No of Application Processed";
                    workSheet.Cells[4, 5].Value = "No of Application Registered";
                    workSheet.Cells[4, 6].Value = "No of Application Rejected";
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                                    
                    workSheet.Cells[2, 2].Value = "Total Records : " + (objListItemsToBeExported.Count());
                    workSheet.Cells[2, 3].Value = "From date: " + FromDate;
                    workSheet.Cells[2, 4].Value = "To date: " + ToDate;

                    string str1 = objListItemsToBeExported.Any(x => x.DistrictName != null)
                               ? objListItemsToBeExported.First(x => x.DistrictName != null).DistrictName.ToString()
                               : "ALL";

                    workSheet.Cells[2, 5].Value = "DRO: " + str1;

                    string str = objListItemsToBeExported.Any(x => x.SROName != null)
                               ? objListItemsToBeExported.First(x => x.SROName != null).SROName.ToString()
                               : "ALL";
                    workSheet.Cells[2, 6].Value = "SRO: " + str;

                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;

                    workSheet.Column(1).Width = 25;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 25;
                    workSheet.Column(4).Width = 25;
                    workSheet.Column(5).Width = 25;
                    workSheet.Column(6).Width = 25;
                    

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    for (int i = 1; i < 8; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 5;
                    //workSheet.Cells[7, 1].Value = "Sr No";
                    //workSheet.Cells[7, 2].Value = "SRO Office";
                    //workSheet.Cells[7, 3].Value = "Application Received Date";
                    //workSheet.Cells[7, 4].Value = "No of Application Received";
                    //workSheet.Cells[7, 5].Value = "No of Application Processed";
                    //workSheet.Cells[7, 6].Value = "No of Application Registered";
                    //workSheet.Cells[7, 7].Value = "No of Application Rejected";
                   
                    foreach (var items in objListItemsToBeExported)
                    {
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        //workSheet.Cells[rowIndex, 2].Value = items.SROoffice;
                        workSheet.Cells[rowIndex, 2].Value = items.Application_received_date;
                        workSheet.Cells[rowIndex, 3].Value = items.No_of_Application_Received;
                        workSheet.Cells[rowIndex, 4].Value = items.No_of_Application_Processed;
                        workSheet.Cells[rowIndex, 5].Value = items.No_of_Application_Registered;
                        workSheet.Cells[rowIndex, 6].Value = items.No_of_Application_Rejected;
                       
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[1, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }

                        rowIndex++;
                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

        /// <summary>
        /// Get File Info
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns>returns Fileinfo</returns>
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }


        public ActionResult ExportSevaSindhuDtlMonthWiseToExcel(string FinYearListID, string IsMonthWise, string selectedSROText, string selectedDistrictText, string SROOfficeListID, string DROfficeListID)
        {
            try
            {
                caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
                string fileName = string.Format("SevaSindhuReportMonthWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                SevaSindhuStatisticsReportModel reqModel = new SevaSindhuStatisticsReportModel();
                
                reqModel.selectedYear = Convert.ToInt32(FinYearListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);
                reqModel.isExcelDownload = true;

                List<SevaSindhuStatisticsReportDetailModel> objListItemsToBeExported = new List<SevaSindhuStatisticsReportDetailModel>();
                reqModel.IsExcel = true;
                caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
                SevaSindhuStatisticsReportResModel saleDeedRevCollectionOuterModel = new SevaSindhuStatisticsReportResModel();
                objListItemsToBeExported = caller.PostCall<SevaSindhuStatisticsReportModel, List<SevaSindhuStatisticsReportDetailModel>>("LoadSevaSindhuStatisticsReportTblMonthWise", reqModel, out errorMessage);

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }

                caller = new ServiceCaller("CommonsApiController");
                
                if (IsMonthWise == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Fetching SRO Name", URLToRedirect = "/Home/HomePage" });
                }

                string excelHeader = string.Format("Seva Sindhu Statistics Month Wise Report For Year ( " + FinYearListID + " )");
                string createdExcelPath = CreateSevaSindhuMonthWiseExcel(objListItemsToBeExported, fileName, excelHeader, FinYearListID, selectedSROText, selectedDistrictText, SROOfficeListID, DROfficeListID);
               
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "SevaSindhuStatisticsArticleWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateSevaSindhuMonthWiseExcel(List<SevaSindhuStatisticsReportDetailModel> excelResult, string fileName, string excelHeader, string FinYearListID, string selectedSROText, string selectedDistrictText, string SROOfficeListID,string DROfficeListID)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SevaSindhuStatisticsReport");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 8].Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[4, 1].Value = "Sr No";
                    //workSheet.Cells[4, 2].Value = "SRO Office";
                    workSheet.Cells[4, 2].Value = "Application Received Month";
                    workSheet.Cells[4, 3].Value = "No of Application Received";
                    workSheet.Cells[4, 4].Value = "No of Application Processed";
                    workSheet.Cells[4, 5].Value = "No of Application Registered";
                    workSheet.Cells[4, 6].Value = "No of Application Rejected";

                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[2, 2].Value = "Total Records : " + (excelResult.Count());
                    int finYear = int.Parse(FinYearListID) + 1;
                    workSheet.Cells[2, 3].Value = "For Financial Year: " + FinYearListID+" - "+finYear.ToString();

                    string str1 = excelResult.Any(x => x.DistrictName != null)
                              ? excelResult.First(x => x.DistrictName != null).DistrictName.ToString()
                              : "ALL";

                    workSheet.Cells[2, 4].Value = "DRO: " + str1;
                    //string str = excelResult.FirstOrDefault(x => x.SROName != null)?.SROName.ToString() ?? string.Empty;
                    string str = excelResult.Any(x => x.SROName != null)
                               ? excelResult.First(x => x.SROName != null).SROName.ToString()
                               : "ALL";
                    workSheet.Cells[2, 5].Value = "SRO : " + str;

                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;

                    workSheet.Column(1).Width = 25;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 25;
                    workSheet.Column(4).Width = 25;
                    workSheet.Column(5).Width = 25;
                    workSheet.Column(6).Width = 25;
                    //workSheet.Column(7).Width = 25;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    for (int i = 1; i < 8; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 5;
                    //workSheet.Cells[7, 1].Value = "Sr No";
                    //workSheet.Cells[7, 2].Value = "SRO Office";
                    //workSheet.Cells[7, 3].Value = "Application Received Month";
                    //workSheet.Cells[7, 4].Value = "No of Application Received";
                    //workSheet.Cells[7, 5].Value = "No of Application Processed";
                    //workSheet.Cells[7, 6].Value = "No of Application Registered";
                    //workSheet.Cells[7, 7].Value = "No of Application Rejected";

                    foreach (var items in excelResult)
                    {
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        //workSheet.Cells[rowIndex, 2].Value = items.SROoffice;
                        workSheet.Cells[rowIndex, 2].Value = items.Application_Received_Month;
                        workSheet.Cells[rowIndex, 3].Value = items.No_of_Application_Received;
                        workSheet.Cells[rowIndex, 4].Value = items.No_of_Application_Processed;
                        workSheet.Cells[rowIndex, 5].Value = items.No_of_Application_Registered;
                        workSheet.Cells[rowIndex, 6].Value = items.No_of_Application_Rejected;

                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[1, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }

                        rowIndex++;
                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }


        public ActionResult ExportSevaSindhuDtlYearWiseToExcel(string selectedYear, string isYearWise)
        {
            try
            {
                caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
                string fileName = string.Format("SevaSindhuReportYearWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                SevaSindhuStatisticsReportModel reqModel = new SevaSindhuStatisticsReportModel();

                reqModel.selectedYear = Convert.ToInt32(selectedYear);

                List<SevaSindhuStatisticsReportDetailModel> objListItemsToBeExported = new List<SevaSindhuStatisticsReportDetailModel>();
                reqModel.IsExcel = true;
                caller = new ServiceCaller("SevaSindhuStatisticsAPIController");
                SevaSindhuStatisticsReportResModel saleDeedRevCollectionOuterModel = new SevaSindhuStatisticsReportResModel();
                objListItemsToBeExported = caller.PostCall<SevaSindhuStatisticsReportModel, List<SevaSindhuStatisticsReportDetailModel>>("SevaSindhuStatisticsReportDetailsYearWise", reqModel, out errorMessage);

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }

                string SROName = string.Empty;
                caller = new ServiceCaller("CommonsApiController");

                if (isYearWise == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Fetching SRO Name", URLToRedirect = "/Home/HomePage" });
                }

                string excelHeader = string.Format("Seva Sindhu Statistics Report For Year ( " + selectedYear + " )");
                string createdExcelPath = CreateSevaSindhuYearWiseExcel(objListItemsToBeExported, fileName, excelHeader, selectedYear);

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "SevaSindhuStatisticsArticleWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateSevaSindhuYearWiseExcel(List<SevaSindhuStatisticsReportDetailModel> excelResult, string fileName, string excelHeader, string FinYearListID)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SevaSindhuStatisticsReport");

                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1, 1, 8].Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[4, 1].Value = "Sr No";
                    //workSheet.Cells[4, 2].Value = "SRO Office";
                    workSheet.Cells[4, 2].Value = "Application Received Year";
                    workSheet.Cells[4, 3].Value = "No of Application Received";
                    workSheet.Cells[4, 4].Value = "No of Application Processed";
                    workSheet.Cells[4, 5].Value = "No of Application Registered";
                    workSheet.Cells[4, 6].Value = "No of Application Rejected";
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[2, 2].Value = "Total Records : " + (excelResult.Count());
                    int finYr = int.Parse(FinYearListID) + 1;
                    workSheet.Cells[2, 3].Value = "For Financial Year: " + FinYearListID+" : "+finYr;
                    

                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;

                    workSheet.Column(1).Width = 25;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 25;
                    workSheet.Column(4).Width = 25;
                    workSheet.Column(5).Width = 25;
                    workSheet.Column(6).Width = 25;
                    //workSheet.Column(7).Width = 25;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    for (int i = 1; i < 8; i++)
                    {
                        workSheet.Column(i).Style.WrapText = true;
                        workSheet.Column(i).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 5;
                    //workSheet.Cells[7, 1].Value = "Sr No";
                    //workSheet.Cells[7, 2].Value = "SRO Office";
                    //workSheet.Cells[7, 3].Value = "Application Received Year";
                    //workSheet.Cells[7, 4].Value = "No of Application Received";
                    //workSheet.Cells[7, 5].Value = "No of Application Processed";
                    //workSheet.Cells[7, 6].Value = "No of Application Registered";
                    //workSheet.Cells[7, 7].Value = "No of Application Rejected";


                    foreach (var items in excelResult)
                    {
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }


                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        //workSheet.Cells[rowIndex, 2].Value = items.SROoffice;
                        workSheet.Cells[rowIndex, 2].Value = items.Application_Received_Year;
                        workSheet.Cells[rowIndex, 3].Value = items.No_of_Application_Received;
                        workSheet.Cells[rowIndex, 4].Value = items.No_of_Application_Processed;
                        workSheet.Cells[rowIndex, 5].Value = items.No_of_Application_Registered;
                        workSheet.Cells[rowIndex, 6].Value = items.No_of_Application_Rejected;

                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        }
                        for (int i = 1; i < 14; i++)
                        {
                            workSheet.Cells[1, i].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }

                        rowIndex++;
                    }


                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

    }
}