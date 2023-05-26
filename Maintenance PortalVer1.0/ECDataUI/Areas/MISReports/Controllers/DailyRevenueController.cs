#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DailyRevenueController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.DailyRevenue;
using ECDataUI.Common;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic;

using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using ECDataUI.Session;
using OfficeOpenXml.Style;
using ECDataUI.Filters;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]

    public class DailyRevenueController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        // GET: MISReports/DailyRevenue


        /// <summary>
        /// Daily Revenue Report
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "Daily Revenue Report")]
        public ActionResult DailyRevenueReport()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DailyRevenueArticleWise;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DailyRevenueAPIController");
                DailyRevenueReportReqModel reqModel = caller.GetCall<DailyRevenueReportReqModel>("DailyRevenueReport", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Daily Revenue Article Wise View", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// Get Daily Revenue Report Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Daily Revenue Report Details list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Daily Revenue Report Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetDailyRevenueReportDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("DailyRevenueAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string ArticleNameListID = formCollection["ArticleNameListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeListID = formCollection["DROfficeListID"];
                string SelectedDistrict = formCollection["selectedDistrict"];
                string SelectedSRO = formCollection["selectedSRO"];


                int SroId = Convert.ToInt32(SROOfficeListID);
                int DroId = Convert.ToInt32(DROfficeListID);
                string IsDayWise = formCollection["isDayWise"];
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

                #region Server Side Validation        



                //if (ArticleNameListID == "0" && DROfficeListID == "0")
                //{
                //    var emptyData = Json(new
                //    {
                //        status = "0",
                //        errorMessage = "Please select specific Article."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}



                //if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any District."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


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

                #endregion

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

                DailyRevenueReportReqModel reqModel = new DailyRevenueReportReqModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.fromDate = fromDate;
                reqModel.ToDate_Str = ToDate;
                reqModel.ArticleID = Convert.ToInt32(ArticleNameListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);

                reqModel.fromDateTime = frmDate;
                reqModel.ToDate = toDate;
                //reqModel.selectedYear = Convert.ToInt32(selectedYear);
                //reqModel.selectedMonth = Convert.ToInt32(selectedMonth);

                int totalCount = caller.PostCall<DailyRevenueReportReqModel, int>("DailyRevenueReportDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                IEnumerable<DailyRevenueReportDetailModel> result = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("DailyRevenueReportDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Daily Revenue Report Period Wise  Details" });
                }

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
                    {//Modified by Raman on 28-06-2019
                        result = result.Where(m => m.ArticleName.ToLower().Contains(searchValue.ToLower()) ||
                        m.Documents.ToString().Contains(searchValue.ToLower()) ||
                        m.StampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegistrationFee.ToString().ToLower().Contains(searchValue.ToLower()) ||

                        m.TotalAmount.ToString().ToLower().Contains(searchValue.ToLower())
                         );
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(IndexIIReportsDetailsModel => new
                {

                    SRNo = IndexIIReportsDetailsModel.SRNo,
                    ArticleName = IndexIIReportsDetailsModel.ArticleName,
                    StampDuty = IndexIIReportsDetailsModel.StampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegistrationFee = IndexIIReportsDetailsModel.RegistrationFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Documents = IndexIIReportsDetailsModel.Documents,
                    TotalAmount = IndexIIReportsDetailsModel.TotalAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    districtName = IndexIIReportsDetailsModel.districtName,
                    officeName = IndexIIReportsDetailsModel.officeName
                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + DroId + "','" + IsDayWise + "','" + "" + "')><i style = 'padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as Excel</button>";

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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Daily Revenue Report Period Wise Details" });
            }
        }

        /// <summary>
        /// Get Daily Revenue Report Details Day Wise
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Daily Revenue Report Details Day wise list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Daily Revenue Report Details Day Wise")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetDailyRevenueReportDetailsDayWise(FormCollection formCollection)
        {
            caller = new ServiceCaller("DailyRevenueAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects
                //string fromDate = formCollection["FromDate"];
                //string ToDate = formCollection["ToDate"];
                string ArticleNameListID = formCollection["ArticleNameListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeListID = formCollection["DROfficeListID"];
                string selectedYear = formCollection["selectedYear"];
                string selectedMonth = formCollection["selectedMonth"];
                int SroId = Convert.ToInt32(SROOfficeListID);
                string IsDayWise = formCollection["isDayWise"];
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
                #endregion

                #region Server Side Validation

                if (string.IsNullOrEmpty(ArticleNameListID))
                {
                    var emptyData = Json(new
                    {
                        status = "0",
                        errorMessage = "Please select Article."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                if (ArticleNameListID == "0")
                {
                    var emptyData = Json(new
                    {
                        status = "0",
                        errorMessage = "Please select specific Article."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //if (ArticleNameListID == "0" && DROfficeListID == "0")
                //{
                //    var emptyData = Json(new
                //    {
                //        status = "0",
                //        errorMessage = "Please select specific Article."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


                //if (string.IsNullOrEmpty(fromDate))
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
                //if (string.IsNullOrEmpty(ToDate))
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

                #endregion
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                DailyRevenueReportReqModel reqModel = new DailyRevenueReportReqModel();

                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;

                reqModel.ArticleID = Convert.ToInt32(ArticleNameListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);

                reqModel.selectedYear = Convert.ToInt32(selectedYear);
                reqModel.selectedMonth = Convert.ToInt32(selectedMonth);


                int totalCount = 0;// caller.PostCall<DailyRevenueReportReqModel, int>("DailyRevenueReportDetailsDayWise", reqModel, out errorMessage);




                IEnumerable<DailyRevenueReportDetailModel> result = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("DailyRevenueReportDetailsDayWise", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Daily Revenue Report Day Wise Details" });
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
                        result = result.Where(m => m.ArticleName.ToLower().Contains(searchValue.ToLower()) ||
                        m.Documents.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.StampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegistrationFee.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.DateValue.ToLower().Contains(searchValue.ToLower()) ||
                        m.TotalAmount.ToString().ToLower().Contains(searchValue.ToLower())


                         );

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(IndexIIReportsDetailsModel => new
                {

                    SRNo = IndexIIReportsDetailsModel.SRNo,
                    ArticleName = IndexIIReportsDetailsModel.ArticleName,
                    StampDuty = IndexIIReportsDetailsModel.StampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegistrationFee = IndexIIReportsDetailsModel.RegistrationFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Documents = IndexIIReportsDetailsModel.Documents,
                    DateValue = IndexIIReportsDetailsModel.DateValue,
                    TotalAmount = IndexIIReportsDetailsModel.TotalAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))


                });

                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + DROfficeListID + "','" + IsDayWise + "','" + selectedYear + "','" + selectedMonth + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + String.Empty + "','" + String.Empty + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + DROfficeListID + "','" + IsDayWise + "','" + selectedYear + "','" + selectedMonth + "','" + "" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Daily Revenue Report Day Wise Details" });
            }
        }

        //[HttpPost]
        //[EventAuditLogFilter(Description = "Get Daily Revenue Report Details MonthWise")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadDailyRevenueReportTblMonthWise(FormCollection formCollection)
        {
            caller = new ServiceCaller("DailyRevenueAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects
                //string fromDate = formCollection["FromDate"];
                //string ToDate = formCollection["ToDate"];
                string ArticleNameListID = formCollection["ArticleNameListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeListID = formCollection["DROfficeListID"];
                string selectedYear = formCollection["selectedYear"];
                string IsMonthWise = formCollection["IsMonthWise"];
                int SroId = Convert.ToInt32(SROOfficeListID);
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
                #endregion

                #region Server Side Validation

                if (string.IsNullOrEmpty(ArticleNameListID))
                {
                    var emptyData = Json(new
                    {
                        status = "0",
                        errorMessage = "Please select Article."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (ArticleNameListID == "0")
                {
                    var emptyData = Json(new
                    {
                        status = "0",
                        errorMessage = "Please select specific Article."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                //if (ArticleNameListID == "0" && DROfficeListID == "0")
                //{
                //    var emptyData = Json(new
                //    {
                //        status = "0",
                //        errorMessage = "Please select specific Article."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                //if (string.IsNullOrEmpty(fromDate))
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
                //if (string.IsNullOrEmpty(ToDate))
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

                #endregion
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                DailyRevenueReportReqModel reqModel = new DailyRevenueReportReqModel();

                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;

                reqModel.ArticleID = Convert.ToInt32(ArticleNameListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);

                reqModel.selectedYear = Convert.ToInt32(selectedYear);


                int totalCount = 0;// caller.PostCall<DailyRevenueReportReqModel, int>("DailyRevenueReportDetailsDayWise", reqModel, out errorMessage);




                IEnumerable<DailyRevenueReportDetailModel> result = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("LoadDailyRevenueReportTblMonthWise", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Daily Revenue Report Day Wise Details" });
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
                        result = result.Where(m => m.ArticleName.ToLower().Contains(searchValue.ToLower()) ||
                        m.Documents.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.StampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegistrationFee.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.DateValue.ToLower().Contains(searchValue.ToLower()) ||
                        m.TotalAmount.ToString().ToLower().Contains(searchValue.ToLower())


                         );

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(DailyRevenueReportDetailModel => new
                {

                    SRNo = DailyRevenueReportDetailModel.SRNo,
                    Month = DailyRevenueReportDetailModel.SelectedMonthName,
                    ArticleName = DailyRevenueReportDetailModel.ArticleName,
                    Documents = DailyRevenueReportDetailModel.Documents,
                    RegistrationFee = DailyRevenueReportDetailModel.RegistrationFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    StampDuty = DailyRevenueReportDetailModel.StampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    TotalAmount = DailyRevenueReportDetailModel.TotalAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))


                });
                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + DROfficeListID + "','" + IsDayWise + "','" + selectedYear + "','" + selectedMonth + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "" + "','" + "" + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + DROfficeListID + "','" + "" + "','" + selectedYear + "','" + "" + "','" + IsMonthWise + "','" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Daily Revenue Report Day Wise Details" });
            }
        }


        #region PDF
        /// <summary>
        /// Export Daily Revenue Report To PDF
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="ArticleNameListID"></param>
        /// <param name="DROfficeListID"></param>
        /// <param name="isDayWise"></param>
        /// <param name="selectedYear"></param>
        /// <param name="selectedMonth"></param>
        /// <returns>returns pdf</returns>
        [EventAuditLogFilter(Description = "Export Daily Revenue Report To PDF")]
        public ActionResult ExportDailyRevenueReportToPDF(string FromDate, string ToDate, string SROOfficeListID, string ArticleNameListID, string DROfficeListID, string isDayWise, string selectedYear, string selectedMonth)
        {
            try
            {
                string fileName = "";// string.Format("DailyRevenueReport.pdf");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);


                //model.Amount = Convert.ToInt32(Amount);
                DailyRevenueReportReqModel model = new DailyRevenueReportReqModel();

                model.ArticleID = Convert.ToInt32(ArticleNameListID);
                model.SROfficeID = Convert.ToInt32(SROOfficeListID);
                model.DROfficeID = Convert.ToInt32(DROfficeListID);


                if (isDayWise == "0")
                {
                    fileName = string.Format("DailyRevenueReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
                    DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                    DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                    model.fromDateTime = frmDate;
                    model.ToDate = toDate;
                }
                else
                {
                    fileName = string.Format("DailyRevenueReportDayWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");

                    model.selectedYear = Convert.ToInt32(selectedYear);
                    model.selectedMonth = Convert.ToInt32(selectedMonth);

                }


                List<DailyRevenueReportDetailModel> objListItemsToBeExported = new List<DailyRevenueReportDetailModel>();

                caller = new ServiceCaller("DailyRevenueAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;


                if (isDayWise == "0")
                {
                    objListItemsToBeExported = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("DailyRevenueReportDetails", model, out errorMessage);
                }
                else
                {
                    objListItemsToBeExported = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("DailyRevenueReportDetailsDayWise", model, out errorMessage);

                }
                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }

                //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));

                string pdfHeader = "";// string.Format("Index II Report (Between {0} and {1})", FromDate, ToDate);
                if (isDayWise == "0")
                    pdfHeader = string.Format("Daily Revenue Report Between ({0} and {1})", FromDate, ToDate);
                else
                    pdfHeader = string.Format("Daily Revenue Report For Year ({0} Month {1})", selectedYear, objListItemsToBeExported[0].SelectedMonthName);



                //To get SRONAME
                string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                //Create Temp PDF File
                byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create PDF File
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="pdfHeader"></param>
        /// <returns>returns pdf byte array</returns>
        private byte[] CreatePDFFile(List<DailyRevenueReportDetailModel> objListItemsToBeExported, string fileName, string pdfHeader)
        {
            string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            try
            {
                byte[] pdfBytes = null;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (Document doc = new Document(PageSize.A4.Rotate(), 35, 10, 10, 25))
                    {
                        using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
                        {

                            //  string Info = string.Format("Print Date Time : {0}   Total Records : {1}  SRO Name : {2}", DateTime.Now.ToString(), SROName);
                            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                            var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
                            doc.Open();
                            Paragraph addHeading = new Paragraph(pdfHeader, headerTextFont)
                            {
                                Alignment = 1,
                            };
                            //Paragraph Info = new Paragraph(Info, redListTextFont)
                            //{
                            //    Alignment = 1,
                            //};
                            Paragraph addSpace = new Paragraph(" ")
                            {
                                Alignment = 1
                            };
                            var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
                            //var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(128,191,255));
                            var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));


                            var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
                            var totalChunk = new Chunk("Total Records: ", blackListTextFont);
                            //  var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);
                            //  var SroName = new Chunk(SROName + "       ", redListTextFont);
                            var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
                            string count = objListItemsToBeExported.Count().ToString();
                            var countChunk = new Chunk(count, redListTextFont);

                            var titlePhrase = new Phrase(titleChunk)
                        {
                            descriptionChunk
                        };
                            var totalPhrase = new Phrase(totalChunk)
                        {
                            countChunk
                        };
                            //    var SroNamePhrase = new Phrase(SroNameChunk)
                            //{
                            //    SroName
                            //};
                            doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(titlePhrase);
                            //doc.Add(SroNamePhrase);
                            doc.Add(totalPhrase);
                            doc.Add(addSpace);

                            doc.Add(DailyRevenueReportTable(objListItemsToBeExported));
                            //doc.Close();
                        }
                        pdfBytes = new CommonFunctions().AddpageNumber(ms.ToArray());
                    }

                }
                return pdfBytes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Daily Revenue Report Table
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns></returns>
        //To get PdfPtable of IndexII report 
        private PdfPTable DailyRevenueReportTable(List<DailyRevenueReportDetailModel> objListItemsToBeExported)
        {

            string frn = "Final Registration Number";
            string ArticleName = "Article Name";
            string StampDateTime = "Stamp5 Date Time";
            string TotalArea = "Total Area";
            string Unit = "Unit";
            string PropertyDetails = "Property Details";
            string Schedule = "Schedule";
            string Executant = "Executant";
            string Claimant = "Claimant";
            string VillageNameE = "Village Name";
            string Consideration = "Consideration";
            string MarketValue = "Market Value";
            try
            {
                string[] col = { frn, ArticleName, StampDateTime, TotalArea, Unit, PropertyDetails, Schedule, Executant, Claimant, VillageNameE, MarketValue, Consideration };
                PdfPTable table = new PdfPTable(12)
                {
                    WidthPercentage = 100
                };
                // table.DefaultCell.FixedHeight = 500f;

                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 14);

                //to repeat Headers
                table.HeaderRows = 1;
                // then set the column's __relative__ widths
                table.SetWidths(new Single[] { 5, 4, 4, 3, 3, 8, 8, 5, 5, 4, 3, 6 });
                /*
                * by default tables 'collapse' on surrounding elements,
                * so you need to explicitly add spacing
                */
                //table.SpacingBefore = 10;
                for (int i = 0; i < col.Length; ++i)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    table.AddCell(cell);

                }
                foreach (var items in objListItemsToBeExported)
                {
                    //table.AddCell(new Phrase(items.FinalRegistrationNumber, tableContentFont));
                    //table.AddCell(new Phrase(items.ArticleNameE, tableContentFont));
                    //table.AddCell(new Phrase(items.Stamp5Datetime, tableContentFont));
                    //table.AddCell(new Phrase(items.TotalArea, tableContentFont));
                    //table.AddCell(new Phrase(items.Unit, tableContentFont));
                    //table.AddCell(new Phrase(items.PropertyDetails, tableContentFont));
                    //table.AddCell(new Phrase(items.Schedule, tableContentFont));
                    //table.AddCell(new Phrase(items.Executant, tableContentFont));
                    //table.AddCell(new Phrase(items.Claimant, tableContentFont));
                    //table.AddCell(new Phrase(items.VillageNameE, tableContentFont));
                    //table.AddCell(new Phrase(items.marketvalue, tableContentFont));
                    //table.AddCell(new Phrase(items.consideration, tableContentFont));
                }
                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Excel
        /// <summary>
        /// Daily Revenue Report To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="ArticleNameListID"></param>
        /// <param name="DROfficeListID"></param>
        /// <param name="isDayWise"></param>
        /// <param name="selectedYear"></param>
        /// <param name="selectedMonth"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Daily Revenue Report To Excel")]
        public ActionResult DailyRevenueReportToExcel(string FromDate, string ToDate, string SROOfficeListID, string ArticleNameListID, string DROfficeListID, string isDayWise, string selectedYear, string selectedMonth, string SelectedDistrict, string SelectedSROText, string MaxDate)
        {
            try
            {
                caller = new ServiceCaller("DailyRevenueAPIController");

                string fileName = "";
                DateTime frmDate, toDate;

                DailyRevenueReportReqModel reqModel = new DailyRevenueReportReqModel();
                reqModel.ArticleID = Convert.ToInt32(ArticleNameListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);

                if (isDayWise == "0")
                {
                    fileName = string.Format("DailyRevenueReport.xlsx");
                    DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                    DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                    reqModel.fromDateTime = frmDate;
                    reqModel.ToDate = toDate;
                }
                else
                {
                    fileName = string.Format("DailyRevenueReportDayWise.xlsx");
                    // validation for year and month required

                    reqModel.selectedYear = Convert.ToInt32(selectedYear);
                    reqModel.selectedMonth = Convert.ToInt32(selectedMonth);

                }
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                List<DailyRevenueReportDetailModel> objListItemsToBeExported = new List<DailyRevenueReportDetailModel>();
                reqModel.IsExcel = true;
                caller = new ServiceCaller("DailyRevenueAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                reqModel.IsExcel = true;
                if (isDayWise == "0")
                    objListItemsToBeExported = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("DailyRevenueReportDetails", reqModel, out errorMessage);
                else
                {

                    objListItemsToBeExported = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("DailyRevenueReportDetailsDayWise", reqModel, out errorMessage);


                }

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = "";// string.Format("Daily Revenue Report Between ({0} and {1})", FromDate, ToDate);

                if (isDayWise == "0")
                    excelHeader = string.Format("Daily Revenue Article wise Report Between ({0} and {1})", FromDate, ToDate);
                else
                    excelHeader = string.Format("Daily Revenue Article wise Report For Year ({0} Month {1})", selectedYear, objListItemsToBeExported[0].SelectedMonthName);


                string createdExcelPath = CreateDailyRevenueReportExcel(objListItemsToBeExported, fileName, excelHeader, isDayWise, SelectedDistrict, SelectedSROText, MaxDate, ArticleNameListID, FromDate, ToDate);
                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                if (isDayWise == "0")
                {
                    return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "DailyRevenueReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");

                }
                else
                {
                    return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "DailyRevenueReportDayWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");


                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }

        }

        #endregion
        /// <summary>
        /// Create Daily Revenue Report Excel
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="isDayWise"></param>
        /// <returns></returns>
        private string CreateDailyRevenueReportExcel(List<DailyRevenueReportDetailModel> objListItemsToBeExported, string fileName, string excelHeader, string isDayWise, string selectedDistrict, string selectedSRO, string MaxDate, string ArticleNameListID, string FromDate, string ToDate)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Daily Revenue Report Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[2, 1].Value = " District: " + selectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + selectedSRO;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;

                    //Added by Madhusoodan on 12-5-2020
                    //To hide Processed Report Note when From Date - To Date are same
                    if (FromDate == ToDate)
                        workSheet.Cells[5, 1].Value = "Total Records : " + (objListItemsToBeExported.Count() - 1);
                    else
                        workSheet.Cells[5, 1].Value = "Total Records : " + (objListItemsToBeExported.Count() - 1) + "                                                                                                                                                                                                                     Note : This report is based on pre processed data considered upto : " + MaxDate;//because it includes last (total) record

                    workSheet.Cells[1, 1, 1, 12].Merge = true;
                    workSheet.Cells[2, 1, 2, 12].Merge = true;
                    workSheet.Cells[3, 1, 3, 12].Merge = true;
                    workSheet.Cells[4, 1, 4, 12].Merge = true;
                    workSheet.Cells[5, 1, 5, 6].Merge = true;

                    workSheet.Column(1).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    if (isDayWise == "0")
                    {
                        //Added by Madhusoodan on 11-05-2020
                        //To Show 'District Name' & 'Office Name' Column if a particular Article is selected in 'Period Wise' radio btn.
                        if (ArticleNameListID == "0")
                        {
                            workSheet.Column(1).Width = 30;

                            workSheet.Column(2).Width = 90;
                            workSheet.Column(3).Width = 30;
                            workSheet.Column(4).Width = 50;
                            workSheet.Column(5).Width = 50;
                            workSheet.Column(6).Width = 50;



                            workSheet.Cells[7, 1].Value = "Sr. No";
                            workSheet.Cells[7, 2].Value = "Article Name";
                            workSheet.Cells[7, 3].Value = "Documents Registered";
                            workSheet.Cells[7, 4].Value = "Stamp Duty ( in Rs. )";
                            workSheet.Cells[7, 5].Value = "Registration Fees ( in Rs. )";
                            workSheet.Cells[7, 6].Value = "Total (Stamp Duty + Registration Fee)";
                            workSheet.Cells[7, 6].Style.Font.Name = "KNB-TTUmaEN";
                        }
                        else
                        {
                            workSheet.Column(1).Width = 30;   

                            workSheet.Column(2).Width = 30;
                            workSheet.Column(3).Width = 30;
                            workSheet.Column(4).Width = 70;
                            workSheet.Column(5).Width = 30;
                            workSheet.Column(6).Width = 30;
                            workSheet.Column(7).Width = 40;
                            workSheet.Column(8).Width = 40;

                            workSheet.Cells[7, 1].Value = "Sr. No";
                            workSheet.Cells[7, 2].Value = "District Name";
                            workSheet.Cells[7, 3].Value = "Offie Name";
                            workSheet.Cells[7, 4].Value = "Article Name";
                            workSheet.Cells[7, 5].Value = "Documents Registered";
                            workSheet.Cells[7, 6].Value = "Stamp Duty ( in Rs. )";
                            workSheet.Cells[7, 7].Value = "Registration Fees ( in Rs. )";
                            workSheet.Cells[7, 8].Value = "Total (Stamp Duty + Registration Fee)";
                            workSheet.Cells[7, 6].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Column(8).Style.Font.Name = "KNB-TTUmaEN";
                        }



                    }
                    else
                    {
                        workSheet.Column(1).Width = 30;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 90;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 30;
                        workSheet.Column(6).Width = 50;
                        workSheet.Column(7).Width = 50;


                        workSheet.Cells[7, 1].Value = "Sr. No";

                        workSheet.Cells[7, 2].Value = "Registration Date";

                        workSheet.Cells[7, 3].Value = " Article Name";
                        workSheet.Cells[7, 4].Value = "Documents Registered";
                        workSheet.Cells[7, 5].Value = "Stamp Duty ( in Rs. )";
                        workSheet.Cells[7, 6].Value = "Registration Fees ( in Rs. )";
                        workSheet.Cells[7, 7].Value = "Total (Stamp Duty + Registration Fee)";
                        workSheet.Cells[7, 6].Style.Font.Name = "KNB-TTUmaEN";


                    }
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;



                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    foreach (var items in objListItemsToBeExported)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";

                        if (isDayWise == "0")
                        {
                            //Added by Madhusoodan on 11-05-2020
                            //To Show 'District Name' & 'Office Name' Column if a particular Article is selected in 'Period Wise' radio btn.
                            if (ArticleNameListID == "0")
                            {
                                workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                                workSheet.Cells[rowIndex, 2].Value = items.ArticleName;
                                workSheet.Cells[rowIndex, 3].Value = items.Documents;
                                workSheet.Cells[rowIndex, 4].Value = items.StampDuty;
                                workSheet.Cells[rowIndex, 5].Value = items.RegistrationFee;
                                workSheet.Cells[rowIndex, 6].Value = items.TotalAmount;
                                workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                                workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                                workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                            }
                            else
                            {
                                workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                                workSheet.Cells[rowIndex, 2].Value = items.districtName;
                                workSheet.Cells[rowIndex, 3].Value = items.officeName;

                                workSheet.Cells[rowIndex, 4].Value = items.ArticleName;
                                workSheet.Cells[rowIndex, 5].Value = items.Documents;
                                workSheet.Cells[rowIndex, 6].Value = items.StampDuty;
                                workSheet.Cells[rowIndex, 7].Value = items.RegistrationFee;
                                workSheet.Cells[rowIndex, 8].Value = items.TotalAmount;
                                workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                                workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                                workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                            }

                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 1].Value = items.SRNo;

                            workSheet.Cells[rowIndex, 2].Value = items.DateValue;
                            workSheet.Cells[rowIndex, 3].Value = items.ArticleName;
                            workSheet.Cells[rowIndex, 4].Value = items.Documents;

                            workSheet.Cells[rowIndex, 5].Value = items.StampDuty;
                            workSheet.Cells[rowIndex, 6].Value = items.RegistrationFee;
                            workSheet.Cells[rowIndex, 7].Value = items.TotalAmount;
                            workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                            workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                            workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";

                        }

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    workSheet.Row(rowIndex - 1).Style.Font.Bold = true;


                    workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex - 1, 1].Value = "";

                    if (isDayWise == "0")
                    {
                        //Added by Madhusoodan on 11-05-2020
                        //To Show 'District Name' & 'Office Name' Column if a particular Article is selected in 'Period Wise' radio btn.
                        if (ArticleNameListID == "0")
                        {
                            using (ExcelRange Rng = workSheet.Cells[7, 4, rowIndex, 6])
                            {
                                Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            }
                            using (ExcelRange Rng = workSheet.Cells[7, 2, rowIndex, 2])
                            {
                                Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 6])
                            {
                                Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            }
                        }
                        else
                        {
                            using (ExcelRange Rng = workSheet.Cells[7, 4, rowIndex, 8])
                            {
                                Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                            }
                            using (ExcelRange Rng = workSheet.Cells[7, 2, rowIndex, 4])
                            {
                                Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                            }
                            using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 8])
                            {
                                Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                                Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            }
                        }
                    }
                    else
                    {

                        using (ExcelRange Rng = workSheet.Cells[7, 5, rowIndex, 7])
                        {
                            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        using (ExcelRange Rng = workSheet.Cells[7, 3, rowIndex, 3])
                        {
                            Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        }
                        using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
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


        public ActionResult ExportDailyRevDtlMonthWiseToExcel(string DROfficeListID, string SROOfficeListID, string ArticleNameListID, string FinYearListID, string FinYear, string SelectedDistrict, string MaxDate)
        {
            try
            {
                caller = new ServiceCaller("DailyRevenueAPIController");
                string fileName = string.Format("DailyRevenueReportMonthWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;


                DailyRevenueReportReqModel reqModel = new DailyRevenueReportReqModel();
                reqModel.ArticleID = Convert.ToInt32(ArticleNameListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.selectedYear = Convert.ToInt32(FinYearListID);

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}
                List<DailyRevenueReportDetailModel> objListItemsToBeExported = new List<DailyRevenueReportDetailModel>();
                reqModel.IsExcel = true;
                caller = new ServiceCaller("DailyRevenueAPIController");
                DailyRevenueReportResModel saleDeedRevCollectionOuterModel = new DailyRevenueReportResModel();
                objListItemsToBeExported = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("LoadDailyRevenueReportTblMonthWise", reqModel, out errorMessage);

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string SROName = string.Empty;
                caller = new ServiceCaller("CommonsApiController");
                if (SROOfficeListID == "0")
                {
                    SROName = "All";
                }
                else
                {
                    SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });
                }

                if (SROName == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Fetching SRO Name", URLToRedirect = "/Home/HomePage" });
                }

                string excelHeader = string.Format("Daily Revenue Article Wise Report ( " + FinYear + " )");
                string createdExcelPath = CreateDailyRevMonthWiseExcel(objListItemsToBeExported, fileName, excelHeader, SROName, FinYear, SelectedDistrict, MaxDate);
                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "DailyRevenueArticleWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        private string CreateDailyRevMonthWiseExcel(List<DailyRevenueReportDetailModel> DailyRevMonthWiseList, string fileName, string excelHeader, string SROName, string FinYear, string selectedDistrict, string MaxDate)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Daily Revenue Report Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[2, 1].Value = "District : " + selectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (DailyRevMonthWiseList.Count() - 1) + "                                                                                                                                                                                                                     Note : This report is based on pre processed data considered upto : " + MaxDate;//because it includes last (total) record
                    workSheet.Cells[1, 1, 1, 12].Merge = true;
                    workSheet.Cells[2, 1, 2, 12].Merge = true;
                    workSheet.Cells[3, 1, 3, 12].Merge = true;
                    workSheet.Cells[4, 1, 4, 12].Merge = true;
                    workSheet.Cells[5, 1, 5, 12].Merge = true;

                    workSheet.Column(1).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;



                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 90;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 50;
                    workSheet.Column(7).Width = 50;


                    workSheet.Cells[7, 1].Value = "Sr. No";
                    workSheet.Cells[7, 2].Value = "Month";
                    workSheet.Cells[7, 3].Value = " Article Name";
                    workSheet.Cells[7, 4].Value = "Documents Registered";
                    workSheet.Cells[7, 5].Value = "Stamp Duty ( in Rs. )";
                    workSheet.Cells[7, 6].Value = "Registration Fees ( in Rs. )";
                    workSheet.Cells[7, 7].Value = "Total (Stamp Duty + Registration Fee)";



                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;


                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 6].Style.Font.Name = "KNB-TTUmaEN";

                    int SrNoCount = 1;

                    foreach (var items in DailyRevMonthWiseList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 1].Value = Convert.ToString(SrNoCount);
                        SrNoCount++;
                        workSheet.Cells[rowIndex, 2].Value = items.SelectedMonthName;
                        workSheet.Cells[rowIndex, 3].Value = items.ArticleName;
                        workSheet.Cells[rowIndex, 4].Value = items.Documents;
                        workSheet.Cells[rowIndex, 5].Value = items.StampDuty;
                        workSheet.Cells[rowIndex, 6].Value = items.RegistrationFee;
                        workSheet.Cells[rowIndex, 7].Value = items.TotalAmount;
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

                    workSheet.Cells[(rowIndex - 1), 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex - 1, 1].Value = "";


                    using (ExcelRange Rng = workSheet.Cells[7, 4, rowIndex, 6])
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    using (ExcelRange Rng = workSheet.Cells[7, 2, rowIndex, 2])
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
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

        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
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




        //Added by SB on 09-12-2019

        //[HttpPost]
        //[EventAuditLogFilter(Description = "Get Daily Revenue Report Details MonthWise")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadDailyRevenueReportTblDocWise(FormCollection formCollection)
        {
            caller = new ServiceCaller("DailyRevenueAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects
                //string fromDate = formCollection["FromDate"];
                //string ToDate = formCollection["ToDate"];
                string ArticleNameListID = formCollection["ArticleNameListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeListID = formCollection["DROfficeListID"];
                string selectedYear = formCollection["selectedYear"];
                //////// string IsMonthWise = formCollection["IsMonthWise"];
                int SroId = Convert.ToInt32(SROOfficeListID);
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
                #endregion

                #region Server Side Validation

                if (string.IsNullOrEmpty(ArticleNameListID))
                {
                    var emptyData = Json(new
                    {
                        status = "0",
                        errorMessage = "Please select Article."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (ArticleNameListID == "0")
                {
                    var emptyData = Json(new
                    {
                        status = "0",
                        errorMessage = "Please select specific Article."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                //if (ArticleNameListID == "0" && DROfficeListID == "0")
                //{
                //    var emptyData = Json(new
                //    {
                //        status = "0",
                //        errorMessage = "Please select specific Article."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                //if (string.IsNullOrEmpty(fromDate))
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
                //if (string.IsNullOrEmpty(ToDate))
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

                #endregion
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                DailyRevenueReportReqModel reqModel = new DailyRevenueReportReqModel();

                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;

                reqModel.ArticleID = Convert.ToInt32(ArticleNameListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);

                reqModel.selectedYear = Convert.ToInt32(selectedYear);

                int totalCount = 0;// caller.PostCall<DailyRevenueReportReqModel, int>("DailyRevenueReportDetailsDayWise", reqModel, out errorMessage);

                totalCount = caller.PostCall<DailyRevenueReportReqModel, int>("DailyRevenueReportDetailsTotalCountDocWise", reqModel, out errorMessage);

                IEnumerable<DailyRevenueReportDetailModel> result = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("LoadDailyRevenueReportTblDocWise", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Daily Revenue Report Document Wise Details" });
                }

                // commented by shuhham bhagat on 09-12-2019 at 05:33 pm 
                ////////totalCount = result.Count();

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
                        result = result.Where(m => m.FinancialYear.ToLower().Contains(searchValue.ToLower()) ||
                        m.ArticleName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.SROName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.FinalRegistrationNumber.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegistrationDate.ToLower().Contains(searchValue.ToLower()) ||
                        m.PurchaseValue.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.StampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegistrationFee.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.Total_StampDuty_RegiFee.ToString().ToLower().Contains(searchValue.ToLower())
                         );

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(DailyRevenueReportDetailModel => new
                {

                    SRNo = DailyRevenueReportDetailModel.SRNo,
                    ////////Month = DailyRevenueReportDetailModel.SelectedMonthName,
                    FinancialYear = DailyRevenueReportDetailModel.FinancialYear,
                    ArticleName = DailyRevenueReportDetailModel.ArticleName,
                    SROName = DailyRevenueReportDetailModel.SROName,
                    FinalRegistrationNumber = DailyRevenueReportDetailModel.FinalRegistrationNumber,
                    RegistrationDate = DailyRevenueReportDetailModel.RegistrationDate,
                    PurchaseValue = DailyRevenueReportDetailModel.PurchaseValue.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    StampDuty = DailyRevenueReportDetailModel.StampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegistrationFee = DailyRevenueReportDetailModel.RegistrationFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Total_StampDuty_RegiFee = DailyRevenueReportDetailModel.Total_StampDuty_RegiFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                });
                //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + DateTime.Now.ToString("dd/MM/yyyy") + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + DROfficeListID + "','" + IsDayWise + "','" + selectedYear + "','" + selectedMonth + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadDocumentWise('" + DROfficeListID + "','" + SROOfficeListID + "','" + ArticleNameListID + "','" + selectedYear + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Daily Revenue Report Document Wise Details" });
            }
        }

        public ActionResult ExportDailyRevDtlDocumentWiseToExcel(string DROfficeListID, string SROOfficeListID, string ArticleNameListID, string FinYearListID, string MaxDate, string FinYear, string selectedDistrict)
        {
            try
            {
                caller = new ServiceCaller("DailyRevenueAPIController");
                string fileName = string.Format("DailyRevenueReportDocumentWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;


                DailyRevenueReportReqModel reqModel = new DailyRevenueReportReqModel();
                reqModel.ArticleID = Convert.ToInt32(ArticleNameListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.selectedYear = Convert.ToInt32(FinYearListID);
                reqModel.IsExcel = true;

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}
                List<DailyRevenueReportDetailModel> objListItemsToBeExported = new List<DailyRevenueReportDetailModel>();
                caller = new ServiceCaller("DailyRevenueAPIController");
                DailyRevenueReportResModel saleDeedRevCollectionOuterModel = new DailyRevenueReportResModel();
                objListItemsToBeExported = caller.PostCall<DailyRevenueReportReqModel, List<DailyRevenueReportDetailModel>>("LoadDailyRevenueReportTblDocWise", reqModel, out errorMessage);

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string SROName = string.Empty;
                caller = new ServiceCaller("CommonsApiController");
                if (SROOfficeListID == "0")
                {
                    SROName = "All";
                }
                else
                {
                    SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });
                }

                if (SROName == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Fetching SRO Name", URLToRedirect = "/Home/HomePage" });
                }

                string excelHeader = string.Format("Daily Revenue Article Wise Report ( " + FinYear + " )");
                string createdExcelPath = CreateDailyRevDocumentWiseExcel(objListItemsToBeExported, fileName, excelHeader, SROName, FinYear, selectedDistrict, MaxDate);
                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "DailyRevenueArticleWise_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        private string CreateDailyRevDocumentWiseExcel(List<DailyRevenueReportDetailModel> DailyRevMonthWiseList, string fileName, string excelHeader, string SROName, string FinYear, string selectedDistrict, string MaxDate)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Daily Revenue Report Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[2, 1].Value = "District : " + selectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (DailyRevMonthWiseList.Count() - 1) + "                                                                                                                                                                                                                     Note : This report is based on pre processed data considered upto : " + MaxDate;//because it includes last (total) record
                    workSheet.Cells[1, 1, 1, 12].Merge = true;
                    workSheet.Cells[2, 1, 2, 12].Merge = true;
                    workSheet.Cells[3, 1, 3, 12].Merge = true;
                    workSheet.Cells[4, 1, 4, 12].Merge = true;
                    workSheet.Cells[5, 1, 5, 12].Merge = true;

                    workSheet.Column(1).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;



                    workSheet.Column(1).Width = 25;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 60;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 35;
                    workSheet.Column(6).Width = 35;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 40;
                    workSheet.Column(9).Width = 40;
                    workSheet.Column(10).Width = 40;

                    workSheet.Cells[7, 1].Value = "Sr No";
                    workSheet.Cells[7, 2].Value = "Financial Year";
                    workSheet.Cells[7, 3].Value = "Article Name";
                    workSheet.Cells[7, 4].Value = "SRO Name";
                    workSheet.Cells[7, 5].Value = "Final Registration Number";
                    workSheet.Cells[7, 6].Value = "Registration Date time";
                    workSheet.Cells[7, 7].Value = "Purchase Value (in Rs.)";
                    workSheet.Cells[7, 8].Value = "StampDuty (in Rs.)";
                    workSheet.Cells[7, 9].Value = "Registration Fee (in Rs.)";
                    workSheet.Cells[7, 10].Value = "Total (Stamp Duty + Reg. Fee)";

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;


                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 6].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[7, 9].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[7, 10].Style.Font.Name = "KNB-TTUmaEN";



                    int SrNoCount = 1;

                    foreach (var items in DailyRevMonthWiseList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 1].Value = Convert.ToString(SrNoCount);
                        SrNoCount++;
                        workSheet.Cells[rowIndex, 2].Value = items.FinancialYear;
                        workSheet.Cells[rowIndex, 3].Value = items.ArticleName;
                        workSheet.Cells[rowIndex, 4].Value = items.SROName;
                        workSheet.Cells[rowIndex, 5].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 6].Value = items.RegistrationDate;
                        workSheet.Cells[rowIndex, 7].Value = items.PurchaseValue;
                        workSheet.Cells[rowIndex, 8].Value = items.StampDuty;
                        workSheet.Cells[rowIndex, 9].Value = items.RegistrationFee;
                        workSheet.Cells[rowIndex, 10].Value = items.Total_StampDuty_RegiFee;

                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 10].Style.Numberformat.Format = "0.00";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

                    workSheet.Cells[(rowIndex - 1), 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    // workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                    // workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                    // workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex - 1, 1].Value = "";


                    //using (ExcelRange Rng = workSheet.Cells[7, 4, rowIndex, 6])
                    //{
                    //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //}
                    //using (ExcelRange Rng = workSheet.Cells[7, 2, rowIndex, 2])
                    //{
                    //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //}
                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 10])
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


    }
}