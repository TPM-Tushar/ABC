
using CustomModels.Models.MISReports.KOSPaymentStatusReport;
using CustomModels.Models.MISReports.TodaysDocumentsRegistered;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Globalization;


namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class KOSPaymentStatusReportController : Controller
    {
        ServiceCaller caller = null;
        /// <summary>
        /// KOS PaymentStatus Report Details View
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "KOS Payment Status Report View")]
        [MenuHighlight]
        public ActionResult KOSPaymentStatusReportView()
        {
            try
            {
                // Notes: Add Menu in Common Enum Function:

                // KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.AnyWhereECLog;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("KOSPaymentStatusReportAPIController");
                //TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                //caller.HttpClient.Timeout = objTimeSpan;
                KOSPaymentStatusRptViewModel reqModel = caller.GetCall<KOSPaymentStatusRptViewModel>("KOSPaymentStatusReportView", new { OfficeID = OfficeID });


                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting details from KOS Payment Status View", URLToRedirect = "/Home/HomePage" });
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
        /// Get KOS Payment Status Report table 
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns>View</returns>

        [HttpPost]
        [EventAuditLogFilter(Description = " KOS Payment Status table  ")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult KOSPaymentStatusReportDetails(FormCollection formCollection)
        {
            ServiceCaller caller = null;

            caller = new ServiceCaller("KOSPaymentStatusReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;


            try
            {
                string errorMsg = string.Empty;
                if (ModelState.IsValid)
                {

                    DateTime frmDate, toDate;
                    bool boolFrmDate = false;
                    bool boolToDate = false;
                    CommonFunctions objCommon = new CommonFunctions();
                    String errorMessage = String.Empty;
                    string FromDate = formCollection["FromDate"];
                    string ToDate = formCollection["ToDate"];
                    int DistrictCode = Convert.ToInt32(formCollection["DistrictCode"]);
                    int SroCode = Convert.ToInt32(formCollection["SroCode"]);

                    string ApplicationTypeId = formCollection["ApplicationTypeId"];


                    boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                    boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);
                    bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);


                    #region Server Site Date Validation
                    KOSPaymentStatusRptViewModel chkreqModel = new KOSPaymentStatusRptViewModel();
                    chkreqModel.FromDate = FromDate;
                    chkreqModel.ToDate = ToDate;
                    chkreqModel.SROfficeID = SroCode;
                    chkreqModel.DROfficeID = DistrictCode;

                    DateTime chkFromDate = Convert.ToDateTime(FromDate);
                    DateTime chkToDate = Convert.ToDateTime(ToDate);

                    int value = DateTime.Compare(chkFromDate, chkToDate);

                    if (value > 0)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "To Date can not be greater than From Date" });
                    }

                    #endregion


                    #region Date Validation
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

                    #endregion


                    KOSPaymentStatusRptViewModel reqModel = new KOSPaymentStatusRptViewModel();
                    reqModel.ApplicationTypeId = Int32.Parse(ApplicationTypeId);
                    reqModel.FromDate = FromDate;
                    reqModel.ToDate = ToDate;
                    reqModel.SROfficeID = SroCode;
                    reqModel.DROfficeID = DistrictCode;

                    KOSPaymentStatusRptResModel ResModel = caller.PostCall<KOSPaymentStatusRptViewModel, KOSPaymentStatusRptResModel>("KOSPaymentStatusReportDetails", reqModel, out errorMessage);

                    IEnumerable<KOSPaymentStatusDetailsModel> result = ResModel.KOSPaymentStatusDetailsList;
                    if (result == null)
                    {
                        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting KOS Payment Details Details." });
                    }
                    int totalCount = ResModel.KOSPaymentStatusDetailsList.Count;


                    //Data table Region is kept below. If in future Datatable requirements comes we can use this.
                    #region Datatable
                    //if (searchValue != null && searchValue != "")
                    //{
                    //    reqModel.startLen = 0;
                    //    reqModel.totalNum = totalCount;
                    //}



                    //var gridData = result.Select(KOSPaymentStatusDetailsModel => new
                    //{
                    //    SerialNo = KOSPaymentStatusDetailsModel.SerialNo,
                    //    TotalPaymentReqSubmitted = KOSPaymentStatusDetailsModel.TotalPaymentReqSubmitted,
                    //    TotalPaymentsSuccessful = KOSPaymentStatusDetailsModel.TotalPaymentsSuccessful,
                    //    TotalPaymentsFailed = KOSPaymentStatusDetailsModel.TotalPaymentsFailed,
                    //    TotalPaymentsExpired = KOSPaymentStatusDetailsModel.TotalPaymentsExpired,
                    //    TotalPaymentsPending = KOSPaymentStatusDetailsModel.TotalPaymentsPending,
                    //    PaymentPendingSince = KOSPaymentStatusDetailsModel.PaymentPendingSince,
                    //    LongestPaymentPendingSince = KOSPaymentStatusDetailsModel.LongestPaymentPendingSince,
                    //    AvgPaymentRealizationSpan = KOSPaymentStatusDetailsModel.AvgPaymentRealizationSpan,

                    //});

                    ////String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                    //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                    //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                    //var JsonData = Json(new
                    //{
                    //    draw = formCollection["draw"],
                    //    data = gridData.ToArray(),
                    //    recordsTotal = ResModel.TotalCount,
                    //    status = "1",
                    //    recordsFiltered = ResModel.TotalCount,
                    //    PDFDownloadBtn = PDFDownloadBtn,
                    //    ExcelDownloadBtn = ExcelDownloadBtn
                    //});
                    //JsonData.MaxJsonLength = Int32.MaxValue;
                    //return JsonData;
                    #endregion

                    return PartialView(ResModel);
                }
                else
                {
                    errorMsg = ModelState.FormatErrorMessageInString();
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = errorMsg, URLToRedirect = "/Home/HomePage" });
                }

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting KOS Payment Status details.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get KOS Payment Status Report Datatable 
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns>View</returns>

        [HttpPost]
        [EventAuditLogFilter(Description = "Load KOS Payment Status Datatable ")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadKOSPaymentStatusReportDataTable(KOSPaymentStatusRptViewModel reqModel)
        {


            ServiceCaller caller = null;

            caller = new ServiceCaller("KOSPaymentStatusReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;


            try
            {
                string errorMsg = string.Empty;
                if (ModelState.IsValid)
                {

                    CommonFunctions objCommon = new CommonFunctions();
                    String errorMessage = String.Empty;
                    string FromDate = reqModel.FromDate;
                    string ToDate = reqModel.ToDate;
                    int status = reqModel.status;



                    string ApplicationTypeId = reqModel.ApplicationTypeId.ToString();


                    KOSPaymentStatusRptViewModel Model = new KOSPaymentStatusRptViewModel();
                    Model.ApplicationTypeId = Int32.Parse(ApplicationTypeId);
                    Model.FromDate = FromDate;
                    Model.ToDate = ToDate;
                    Model.status = status;
                    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-12-2020 
                    Model.Days = reqModel.Days;
                    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-12-2020 




                    KOSPaymentStatusRptResTableModel ResModel = caller.PostCall<KOSPaymentStatusRptViewModel, KOSPaymentStatusRptResTableModel>("LoadKOSPaymentStatusReportDataTable", Model, out errorMessage);

                    IEnumerable<KOSPaymentStatusDetailsTableModel> result = ResModel.KOSPaymentStatusDetailsTableList;
                    if (result == null)
                    {
                        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting KOS Payment Details Details." });
                    }
                    int totalCount = ResModel.KOSPaymentStatusDetailsTableList.Count;


                    var gridData = result.Select(KOSPaymentStatusDetailsTableModel => new
                    {
                        SerialNo = KOSPaymentStatusDetailsTableModel.SerialNo,
                        OfficeName = KOSPaymentStatusDetailsTableModel.OfficeName,
                        ApplicationType = KOSPaymentStatusDetailsTableModel.ApplicationType,
                        ApplicationNumber = KOSPaymentStatusDetailsTableModel.ApplicationNumber,
                        TransactionDate = KOSPaymentStatusDetailsTableModel.TransactionDate,
                        ChallanReferencenNumber = KOSPaymentStatusDetailsTableModel.ChallanReferencenNumber,
                        PaymentStatus = KOSPaymentStatusDetailsTableModel.PaymentStatus,
                        PaymentRealizedInDays = KOSPaymentStatusDetailsTableModel.PaymentRealizedInDays,
                    });

                    //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                    //String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                    var JsonData = Json(new
                    {
                        data = gridData.ToArray(),
                        recordsTotal = ResModel.TotalCount,
                        status = "1",
                        recordsFiltered = ResModel.TotalCount,
                        // PDFDownloadBtn = PDFDownloadBtn,
                        // ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;

                }
                else
                {
                    errorMsg = ModelState.FormatErrorMessageInString();
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = errorMsg, URLToRedirect = "/Home/HomePage" });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting KOS Payment Status details.", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        /// Get Payment Pending Since Value Changed
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns>View</returns>

        [HttpPost]
        [EventAuditLogFilter(Description = "Get changed value for Payment Pending Since column")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult PaymentPendingSince(KOSPaymentStatusRptViewModel reqModel)
        {


            ServiceCaller caller = null;

            caller = new ServiceCaller("KOSPaymentStatusReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;


            try
            {
                string errorMsg = string.Empty;
                if (ModelState.IsValid)
                {


                    CommonFunctions objCommon = new CommonFunctions();
                    String errorMessage = String.Empty;
                    string fromDate = reqModel.FromDate;
                    string toDate = reqModel.ToDate;
                    int paymentPendingSince = reqModel.paymentPendingsince;


                    string ApplicationTypeId = reqModel.ApplicationTypeId.ToString();



                    KOSPaymentStatusRptViewModel Model = new KOSPaymentStatusRptViewModel();
                    Model.ApplicationTypeId = Int32.Parse(ApplicationTypeId);
                    Model.FromDate = fromDate;
                    Model.ToDate = toDate;




                    KOSPaymentStatusRptResModel ResModel = caller.PostCall<KOSPaymentStatusRptViewModel, KOSPaymentStatusRptResModel>("PaymentPendingSince", reqModel, out errorMessage);

                    IEnumerable<KOSPaymentStatusDetailsModel> result = ResModel.KOSPaymentStatusDetailsList;
                    if (result == null)
                    {
                        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting KOS Payment Details Details." });
                    }
                    int totalCount = ResModel.KOSPaymentStatusDetailsList.Count;

                    var gridData = result.Select(KOSPaymentStatusDetailsTableModel => new
                    {

                        PaymentPendingSince = KOSPaymentStatusDetailsTableModel.PaymentPendingSince
                    });


                    var JsonData = Json(new
                    {


                        data = gridData.ToArray(),

                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    errorMsg = ModelState.FormatErrorMessageInString();
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = errorMsg, URLToRedirect = "/Home/HomePage" });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting KOS Payment Status details.", URLToRedirect = "/Home/HomePage" });
            }
        }



        #region Export to Excel Table 1
        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SroID"></param>
        /// <param name="selectedApplicationTypeText"></param>


        /// <returns>returns excel file</returns>



        [EventAuditLogFilter(Description = "Export KOS Payment Status Report Details to EXCEL")]
        // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
        //public ActionResult KOSPaymentStatusReportToExcel(string FromDate, string ToDate, int ApplicationTypeId)
        public ActionResult KOSPaymentStatusReportToExcel(string FromDate, string ToDate, int ApplicationTypeId, string PaymentPendingSinceDays, int DistrictCode, int SroCode)
        // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
        {
            try
            {
                caller = new ServiceCaller("KOSPaymentStatusReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("KOSPaymentStatusReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                string strApplicationTypeId = ApplicationTypeId.ToString();

                KOSPaymentStatusRptViewModel Model = new KOSPaymentStatusRptViewModel();
                Model.ApplicationTypeId = Int32.Parse(strApplicationTypeId);
                Model.FromDate = FromDate;
                Model.ToDate = ToDate;
                Model.DROfficeID = DistrictCode;
                Model.SROfficeID = SroCode;




                Model.startLen = 0;
                Model.totalNum = 10;
                Model.IsExcel = true;



                KOSPaymentStatusRptResModel ResModel = new KOSPaymentStatusRptResModel();


                ResModel = caller.PostCall<KOSPaymentStatusRptViewModel, KOSPaymentStatusRptResModel>("KOSPaymentStatusReportDetails", Model, out errorMessage);
                if (ResModel.KOSPaymentStatusDetailsList == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While KOS Payment Status  Details..." }, JsonRequestBehavior.AllowGet);

                }


                string excelHeader = string.Format("KOS Payment Status Report ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, PaymentPendingSinceDays);

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "KOSPaymentStatusReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="ResModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="selectedApplicationTypeText"></param>

        /// <returns>returns excel file path</returns>
        // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
        //private string CreateExcel(KOSPaymentStatusRptResModel ResModel, string fileName, string excelHeader)
        private string CreateExcel(KOSPaymentStatusRptResModel ResModel, string fileName, string excelHeader, string PaymentPendingSinceDays)
        // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020

        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("KOS Payment Status Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + ResModel.DistrictName;
                    workSheet.Cells[3, 1].Value = "SRO : " + ResModel.SROName;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    //workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.KOSPaymentStatusDetailsList.Count());
                    workSheet.Cells[1, 1, 1, 6].Merge = true;
                    workSheet.Cells[2, 1, 2, 6].Merge = true;
                    workSheet.Cells[3, 1, 3, 6].Merge = true;
                    workSheet.Cells[4, 1, 4, 6].Merge = true;
                    workSheet.Cells[5, 1, 5, 6].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 50;
                    workSheet.Column(3).Width = 50;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Column(2).Style.Font.Bold = true;

                    int rowIndex = 7;
                    //int columnIndex = 

                    //workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Cells[7, 1].Value = "Serial No";

                    workSheet.Cells[7, 2].Value = "Application Type ";
                    workSheet.Cells[8, 2].Value = "Total Payment Request Submitted";
                    workSheet.Cells[9, 2].Value = "Total Payments Successful";
                    //workSheet.Cells[7, 5].Value = "Application Number";
                    workSheet.Cells[10, 2].Value = "Total Payments Failed";
                    workSheet.Cells[11, 2].Value = " Total Payments Expired";
                    workSheet.Cells[12, 2].Value = "Total Payments Pending";
                    #region ADDED BY SHUBHAM BHAGAT ON 11-12-2020
                    workSheet.Cells[13, 2].Value = "Payment with no status";
                    #endregion
                    workSheet.Cells[14, 2].Value = "    Payment Pending Since";
                    workSheet.Cells[15, 2].Value = " Longest Payment Pending Since";
                    workSheet.Cells[16, 2].Value = " Average Payment Realization Span";

                    //Del
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(10).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Column(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Column(3).Style.Font.Name = "KNB-TTUmaEN";

                    //workSheet.Cells[7, 10].Style.WrapText = true;  

                    foreach (var items in ResModel.KOSPaymentStatusDetailsList)
                    {
                        //workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";      //8
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex++, 3].Style.Font.Name = "KNB-TTUmaEN";     //17

                        //workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";  //do this
                        // workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex++, 3].Value = items.ApplicationTypeId;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalPaymentReqSubmitted;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalPaymentsSuccessful;

                        //workSheet.Cells[rowIndex++, 3].Value = items.AppNo;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalPaymentsFailed;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalPaymentsExpired;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalPaymentsPending;
                        #region ADDED BY SHUBHAM BHAGAT ON 11-12-2020
                        workSheet.Cells[rowIndex++, 3].Value = items.PaymentWithNoStatus;
                        #endregion
                        // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
                        // Changed because in html table there is dropdown on changing dropdown value the dropdown value is not
                        // considered in stored procedures so there is no impact on data so we have fetched data from html table
                        //workSheet.Cells[rowIndex++, 3].Value = items.PaymentPendingSince;
                        workSheet.Cells[rowIndex++, 3].Value = PaymentPendingSinceDays;
                        // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020

                        workSheet.Cells[rowIndex++, 3].Value = items.LongestPaymentPendingSince;
                        workSheet.Cells[rowIndex++, 3].Value = items.AvgPaymentRealizationSpan;


                        workSheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        //workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        //workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //rowIndex++;
                    }

                    //workSheet.Cells[rowIndex, 1].Value = "";
                    //workSheet.Cells[rowIndex, 2].Value = "";
                    //workSheet.Cells[rowIndex, 3].Value = "";
                    //workSheet.Cells[rowIndex, 4].Value = "";
                    ////workSheet.Cells[rowIndex, 5].Value = "";
                    //workSheet.Cells[rowIndex, 5].Value = "";
                    //workSheet.Cells[rowIndex, 6].Value = "";
                    //workSheet.Cells[rowIndex, 7].Value = "";
                    //workSheet.Cells[rowIndex, 8].Value = "";
                    //workSheet.Cells[rowIndex, 9].Value = "";

                    //workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(rowIndex).Style.Font.Bold = true;
                    //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    using (ExcelRange Rng = workSheet.Cells[7, 2, 16, 3])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 6])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[4, 1, 4, 6])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[7, 2, 16, 3])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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

        /// <summary>
        /// Get File Info
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns>returns file info</returns>
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
        #endregion


        #region Export to Excel Table 2
        /// <summary>
        /// Export To Excel 2
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SroID"></param>
        /// <param name="selectedApplicationTypeText"></param>


        /// <returns>returns excel file</returns>



        [EventAuditLogFilter(Description = "Export KOS Payment Status Report table 2 Details to EXCEL")]
        // BELOW CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
        //public ActionResult KOSPaymentStatusReportTableToExcel(string FromDate, string ToDate, int ApplicationTypeId)
        public ActionResult KOSPaymentStatusReportTableToExcel(string FromDate, string ToDate, int ApplicationTypeId, string status, string days,int DistrictCode,int SroCode)
        // ABOVE CODE IS ADDED BY SHUBHAM BHAGAT ON 14-12-2020
        {
            try
            {
                caller = new ServiceCaller("KOSPaymentStatusReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("KOSPaymentStatusReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                string strApplicationTypeId = ApplicationTypeId.ToString();

                KOSPaymentStatusRptViewModel Model = new KOSPaymentStatusRptViewModel();
                Model.ApplicationTypeId = Int32.Parse(strApplicationTypeId);
                Model.FromDate = FromDate;
                Model.ToDate = ToDate;
                Model.status = Int32.Parse(status);
                Model.Days = days;
                Model.DROfficeID = DistrictCode;
                Model.SROfficeID = SroCode;



                Model.startLen = 0;
                Model.totalNum = 10;
                Model.IsExcel = true;



                KOSPaymentStatusRptResTableModel ResModel = new KOSPaymentStatusRptResTableModel();


                ResModel = caller.PostCall<KOSPaymentStatusRptViewModel, KOSPaymentStatusRptResTableModel>("LoadKOSPaymentStatusReportDataTable", Model, out errorMessage);
                if (ResModel.KOSPaymentStatusDetailsTableList == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While KOS Payment Status  Details..." }, JsonRequestBehavior.AllowGet);

                }


                string excelHeader = string.Format("KOS Payment Status Report ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcelFile(ResModel, fileName, excelHeader);

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "KOSPaymentStatusReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="ResModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="selectedApplicationTypeText"></param>

        /// <returns>returns excel file path</returns>
        private string CreateExcelFile(KOSPaymentStatusRptResTableModel ResModel, string fileName, string excelHeader)
        {
            string ExcelPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetExcelFileInfo(ExcelPath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("KOS Payment Status Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + ResModel.DistrictName;
                    workSheet.Cells[3, 1].Value = "SRO : " + ResModel.SROName;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    //workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.KOSPaymentStatusDetailsList.Count());
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    workSheet.Cells[5, 1, 5, 9].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    //workSheet.Column(5).Width = 30;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 45;
                    workSheet.Column(9).Width = 30;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 1].Value = "Serial No";
                    workSheet.Cells[7, 2].Value = "Office Name";
                    workSheet.Cells[7, 3].Value = "Application Type";
                    workSheet.Cells[7, 4].Value = "Total Payments Successful";

                    workSheet.Cells[7, 5].Value = "Transaction Date";
                    workSheet.Cells[7, 6].Value = " Challan Referencen Number";
                    workSheet.Cells[7, 7].Value = "Payment Status";
                    workSheet.Cells[7, 8].Value = "   Payment Realized In Days ";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.KOSPaymentStatusDetailsTableList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 3].Value = items.ApplicationType;
                        workSheet.Cells[rowIndex, 4].Value = items.ApplicationNumber;


                        workSheet.Cells[rowIndex, 5].Value = items.TransactionDate;
                        workSheet.Cells[rowIndex, 6].Value = items.ChallanReferencenNumber;
                        workSheet.Cells[rowIndex, 7].Value = items.PaymentStatus;
                        workSheet.Cells[rowIndex, 8].Value = items.PaymentRealizedInDays;

                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //Commented and added by Madhusoodan on 10/08/2020 to align them at center
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        rowIndex++;
                    }
                    workSheet.Cells[rowIndex, 1].Value = "";
                    workSheet.Cells[rowIndex, 2].Value = "";
                    workSheet.Cells[rowIndex, 3].Value = "";
                    workSheet.Cells[rowIndex, 4].Value = "";

                    workSheet.Cells[rowIndex, 5].Value = "";
                    workSheet.Cells[rowIndex, 6].Value = "";
                    workSheet.Cells[rowIndex, 7].Value = "";
                    workSheet.Cells[rowIndex, 8].Value = "";


                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 8])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 1])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 8])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }
                    package.SaveAs(templateFile);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return ExcelPath;
        }

        /// <summary>
        /// Get File Info
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns>returns file info</returns>
        public static FileInfo GetExcelFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
        #endregion
    }


}