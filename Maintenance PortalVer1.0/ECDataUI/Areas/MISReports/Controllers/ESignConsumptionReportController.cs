using ECDataUI.Common;
using ECDataUI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Models.MISReports.ESignConsumptionReport;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using ECDataUI.Session;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ESignConsumptionReportController : Controller
    {
        ServiceCaller caller = null;
        /// <summary>
        /// To load eSign Consumption main view
        /// </summary>
        /// <returns>View</returns>
        [EventAuditLogFilter(Description = "eSign Consumption Report View")]
        [MenuHighlight]
        public ActionResult ESignConsumptionReportView()
        {
            try
            {
                caller = new ServiceCaller("ESignConsumptionReportAPIController");

                ESignConsumptionReportViewModel eSignConsumptionReportViewModel = caller.GetCall<ESignConsumptionReportViewModel>("ESignConsumptionReportView");

                //If Techadmin user is logged in them set this flag to true so that eSign Details Datatable will be visibl to Techadmin user only
                if (KaveriSession.Current.RoleID == Convert.ToInt16(CommonEnum.RoleDetails.TechnicalAdmin))
                    eSignConsumptionReportViewModel.ViewESignDetailsDataTable = true;

                return View(eSignConsumptionReportViewModel);
            }
            catch (Exception)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting details from eSign Consumption Report View", URLToRedirect = "/Home/HomePage" });
                throw;
            }
        }

        /// <summary>
        /// To get total count of eSign consumed for Online EC and Online CC
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Partial View</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = " Get Total eSign Consumption Count")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetTotalESignConsumedCount(FormCollection formCollection)
        {
            try
            {

                //if (ModelState.IsValid)
                //{
                //CommonFunctions.ValidateId(formCollection["FinancialyearCode"]
                //}
               string errorMessage = String.Empty;

                //string fromDate = formCollection["FromDate"];
                //string toDate = formCollection["ToDate"];

               
                //DateTime checkFromDate = Convert.ToDateTime(fromDate);
                //DateTime checkToDate = Convert.ToDateTime(toDate);

                //if (DateTime.Compare(checkFromDate, checkToDate) > 0)
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date can not be greater than To Date" });
                //}

                ESignConsumptionReportViewModel reqModel = new ESignConsumptionReportViewModel();
                //reqModel.FromDate = fromDate;
                //reqModel.ToDate = toDate;

                reqModel.FinancialYearCode = Convert.ToInt32(formCollection["FinancialyearCode"]);

                if (reqModel.FinancialYearCode == 0)
                {
                    return Json(new { serverError = false, success = false, errorMessage = "Please select financial year." });
                }

                caller = new ServiceCaller("ESignConsumptionReportAPIController");



                ESignTotalConsumptionResModel resultModel = caller.PostCall<ESignConsumptionReportViewModel, ESignTotalConsumptionResModel>("GetTotalESignConsumedCount", reqModel, out errorMessage);

                if (resultModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Total ESign Consumed Count." });
                }
                else
                {
                    return PartialView(resultModel);
                }
                
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// To load eSign requet response details in datatable
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>JsonData</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get eSign Success/Fail Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadESignDetailsDataTable(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("ESignConsumptionReportAPIController");
                string errorMessage = string.Empty;

                string fromDate = formCollection["FromDate"];
                string toDate = formCollection["ToDate"];

                DateTime checkFromDate = Convert.ToDateTime(fromDate);
                DateTime checkToDate = Convert.ToDateTime(toDate);

                if (DateTime.Compare(checkFromDate, checkToDate) > 0)
                {   
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date can not be greater than To Date" });
                }

                //string selectedAppStatusType = reportModel.ApplicationStatusTypeID.ToString();
                string selectedAppStatusType = formCollection["ApplicationStatusTypeID"];

                ESignConsumptionReportViewModel requestModel = new ESignConsumptionReportViewModel();

                requestModel.FromDate = fromDate;
                requestModel.ToDate = toDate;
                //requestModel.FinancialYearCode = reportModel.FinancialYearCode;
                //requestModel.MonthCode = reportModel.MonthCode;
                //requestModel.FinancialYearCode = Convert.ToInt32(formCollection["FinancialYearCode"]);
                //requestModel.MonthCode =         Convert.ToInt32(formCollection["MonthCode"]);

                requestModel.ApplicationStatusTypeID = Int32.Parse(selectedAppStatusType);

                requestModel.StartLength = Convert.ToInt32(formCollection["start"]);
                //requestModel.TotalNum = Convert.ToInt32(formCollection["length"]);
                //requestModel.TotalNum = 100;

                ESignStatusDetailsResModel resultModel = caller.PostCall<ESignConsumptionReportViewModel, ESignStatusDetailsResModel>("LoadESignDetailsDataTable", requestModel, out errorMessage);

                IEnumerable<ESignStatusDetailsTableModel> resultList = resultModel.ESignStatusDetailsTableList; 

                if (resultList == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting eSign Status Details." });
                }

                int totalCount = resultModel.ESignStatusDetailsTableList.Count;
                
                var gridData = resultList.Select(ESignStatusDetailsTableModel => new
                {
                    SerialNo = ESignStatusDetailsTableModel.SerialNo,
                    ApplicationNumber = ESignStatusDetailsTableModel.ApplicationNumber,
                    ApplicationType = ESignStatusDetailsTableModel.ApplicationType,
                    ApplicationDate = ESignStatusDetailsTableModel.ApplicationDate,
                    ApplicationStatus = ESignStatusDetailsTableModel.ApplicationStatus,
                    ApplicationSubmitDate = ESignStatusDetailsTableModel.ApplicationSubmitDate,
                    ESignRequestDate = ESignStatusDetailsTableModel.ESignRequestDate,
                    ESignRequestTransactionNo = ESignStatusDetailsTableModel.ESignRequestTransactionNo,
                    ESignResponseDate = ESignStatusDetailsTableModel.ESignResponseDate,
                    ESignResponseTransactionNo = ESignStatusDetailsTableModel.ESignResponseTransactionNo,
                    ESignResponseCode = ESignStatusDetailsTableModel.ESignResponseCode,
                    Status = ESignStatusDetailsTableModel.Status,
                    ResponseErrorCode = ESignStatusDetailsTableModel.ResponseErrorCode,
                    ResponseErrorMessage = ESignStatusDetailsTableModel.ResponseErrorMessage
                    
                });

                var JsonData = Json(new
                {
                    data = gridData.ToArray(),
                    recordsTotal = resultModel.TotalCount,
                    status = "1",
                    recordsFiltered = resultModel.TotalCount
                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// To download total eSign consumed count in excel
        /// </summary>
        /// <param name="FinancialyearCode"></param>
        /// <returns></returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Export Total eSign Consumption Count to Excel")]
        public ActionResult TotalESignConsumptionCountToExcel(string FinancialyearCode)
        {
            try
            {
                caller = new ServiceCaller("ESignConsumptionReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("TotalESignConsumptionReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");

                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                ESignConsumptionReportViewModel viewModel = new ESignConsumptionReportViewModel();
                //viewModel.FromDate = FromDate;
                //viewModel.ToDate = ToDate;

                viewModel.FinancialYearCode = Convert.ToInt32(FinancialyearCode);

                viewModel.IsExcel = true;


                ESignTotalConsumptionResModel resModel = new ESignTotalConsumptionResModel();


                resModel = caller.PostCall<ESignConsumptionReportViewModel, ESignTotalConsumptionResModel>("GetTotalESignConsumedCount", viewModel, out errorMessage);

                if (resModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while downloading excel file for Total eSign Consumed Count.", JsonRequestBehavior.AllowGet });
                }


                string excelHeader = string.Format("eSign Consumption Report For Year ({0})", viewModel.FinancialYearCode);
                string createdExcelPath = CreateExcelForTotalConsumptionCount(resModel, fileName, excelHeader);

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "TotalESignConsumptionReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// To create Excel for total eSign consumed count in excel
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns></returns>
        private string CreateExcelForTotalConsumptionCount(ESignTotalConsumptionResModel resModel, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);

            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("eSign Consumption Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[3, 1].Value = "Report Date Time : " + DateTime.Now;
                    
                    workSheet.Cells[1, 1, 1, 6].Merge = true;
                    workSheet.Cells[2, 1, 2, 6].Merge = true;
                    workSheet.Cells[3, 1, 3, 6].Merge = true;
                    workSheet.Cells[4, 1, 4, 6].Merge = true;

                    workSheet.Cells[5, 2, 5, 5].Merge = true;
                    workSheet.Cells[5, 2].Value = "Total eSign Consumed Table";
                    workSheet.Cells[5, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 15;
                    workSheet.Column(2).Width = 50;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 35;
                    workSheet.Column(6).Width = 15;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;

                    workSheet.Cells[6,2].Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;

                    workSheet.Row(3).Style.Font.Size = 12;

                    workSheet.Cells[6, 2].Value = "Month";
                    workSheet.Cells[6, 3].Value = "Online EC";
                    workSheet.Cells[6, 4].Value = "Online CC";
                    workSheet.Cells[6, 5].Value = "Total EC/CC eSign Consumed";

                    workSheet.Cells[6, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[6, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[6, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Row(5).Style.Font.Bold = true;

                    int rowIndex = 7;

                    foreach(var item in resModel.ESignConsumptionMonthWiseTableList)
                    {
                        workSheet.Cells[rowIndex, 2].Value = item.MonthAndYear;
                        workSheet.Cells[rowIndex, 3].Value = item.TotalESignConsumedForEC;
                        workSheet.Cells[rowIndex, 4].Value = item.TotalESignConsumedForCC;
                        workSheet.Cells[rowIndex, 5].Value = item.TotalESignConsumedMonthlyForECCC;

                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        rowIndex++;
                    }

                    
                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[rowIndex, 2].Value = "Total eSign consumed for financial year:";
                    workSheet.Cells[rowIndex, 3].Value = resModel.TotalESignConsumedForEC;
                    workSheet.Cells[rowIndex, 4].Value = resModel.TotalESignConsumedForCC;
                    workSheet.Cells[rowIndex, 5].Value = resModel.TotalESignConsumedForFinYear;

                    using (ExcelRange Rng = workSheet.Cells[6, 2, rowIndex, 5])
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
                    using (ExcelRange Rng = workSheet.Cells[3, 1, 3, 6])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[5, 2, 5, 5])
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
        /// To download eSign request response details in excel
        /// </summary>
        /// <param name="fromDate"></param>
        /// <param name="toDate"></param>
        /// <param name="SelectedAppStatusType"></param>
        /// <returns></returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Export eSign Request Response Details to Excel")]
        public ActionResult ESignStatusDetailsDatatableToExcel(string fromDate, string toDate, string SelectedAppStatusType)
        {
            try
            {
                caller = new ServiceCaller("ESignConsumptionReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("ESignStatusReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                //string strApplicationTypeId = SelectedAppStatusType.ToString();

                ESignConsumptionReportViewModel requestModel = new ESignConsumptionReportViewModel();
                

                //requestModel.FinancialYearCode = year;
                //requestModel.MonthCode = monthID;

                DateTime checkFromDate = Convert.ToDateTime(fromDate);
                DateTime checkToDate = Convert.ToDateTime(toDate);
               
                if (DateTime.Compare(checkFromDate, checkToDate) > 0)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date can not be greater than To Date" });
                }

                requestModel.FromDate = fromDate;
                requestModel.ToDate = toDate;

                requestModel.ApplicationStatusTypeID = Int32.Parse(SelectedAppStatusType);
                //requestModel.ApplicationStatusTypeID = SelectedAppStatusType;

                requestModel.IsExcel = true;

                
                ESignStatusDetailsResModel resultModel = caller.PostCall<ESignConsumptionReportViewModel, ESignStatusDetailsResModel>("LoadESignDetailsDataTable", requestModel, out errorMessage);

                IEnumerable<ESignStatusDetailsTableModel> resultList = resultModel.ESignStatusDetailsTableList;

                
                if (resultList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while downloading excel file for Total eSign Consumed Count." }, JsonRequestBehavior.AllowGet);
                }

                //var monthName = (GetMonthNameByMonthID)monthID;
                //string eSignDataForMonth = monthName.ToString();


                string excelHeader = string.Format("eSign Details Report From {0} to {1}", fromDate, toDate);

                string createdExcelPath = CreateExcelFileForESignStatusDetails(resultModel, fileName, excelHeader, Int32.Parse(SelectedAppStatusType));

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ESignStatusDetails" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// To create Excel for eSign request response details
        /// </summary>
        /// <param name="resultModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SelectedAppStatusType"></param>
        /// <returns></returns>
        private string CreateExcelFileForESignStatusDetails(ESignStatusDetailsResModel resultModel, string fileName, string excelHeader, int SelectedAppStatusType)
        {
            string ExcelPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelPath);

            try
            {
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("eSign Details Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    
                    workSheet.Cells[4, 1].Value = "Report Date Time : " + DateTime.Now;
                    workSheet.Cells[4, 1].Style.Font.Size = 12;

                    workSheet.Cells[1, 1, 1, 14].Merge = true;
                    workSheet.Cells[2, 1, 2, 14].Merge = true;
                    workSheet.Cells[3, 1, 3, 14].Merge = true;
                    workSheet.Cells[4, 1, 4, 14].Merge = true;
                    workSheet.Cells[5, 1, 5, 14].Merge = true;
                    workSheet.Cells[6, 1, 6, 14].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 15;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 50;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 50;
                    workSheet.Column(11).Width = 50;
                    workSheet.Column(12).Width = 20;
                    workSheet.Column(13).Width = 15;
                    workSheet.Column(14).Width = 30;


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
                    workSheet.Cells[7, 2].Value = "ApplicationNumber";
                    workSheet.Cells[7, 3].Value = "Application Type";
                    workSheet.Cells[7, 4].Value = "Application Date";
                    workSheet.Cells[7, 5].Value = "Request Date";
                    workSheet.Cells[7, 6].Value = "Request Transaction No.";
                    workSheet.Cells[7, 7].Value = "Response Date";
                    workSheet.Cells[7, 8].Value = "Response Tran. No.";

                    workSheet.Cells[7, 9].Value = "eSign Response Code";
                    workSheet.Cells[7, 10].Value = "Status";
                    workSheet.Cells[7, 11].Value = "Application Status";
                    workSheet.Cells[7, 12].Value = "Application Submit Date";
                    workSheet.Cells[7, 13].Value = "Response Error Code";
                    workSheet.Cells[7, 14].Value = "Response Error Message";

                   
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    workSheet.Row(7).Style.WrapText = true;
                    workSheet.Column(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Column(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Column(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Column(11).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Column(14).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                    workSheet.Column(5).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Column(10).Style.WrapText = true;
                    workSheet.Column(11).Style.WrapText = true;
                    workSheet.Column(14).Style.WrapText = true;


                    string applicationStatType = string.Empty;
                    if (SelectedAppStatusType == 0)
                        applicationStatType = "All";
                    else if (SelectedAppStatusType == 1)
                        applicationStatType = "Success";
                    else if (SelectedAppStatusType == 2)
                        applicationStatType = "Fail";

                    workSheet.Cells[5, 1].Value = "Application Status Type: " + applicationStatType;
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    foreach (var items in resultModel.ESignStatusDetailsTableList)
                    {
                     
                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.ApplicationNumber;
                        workSheet.Cells[rowIndex, 3].Value = items.ApplicationType;
                        workSheet.Cells[rowIndex, 4].Value = items.ApplicationDate;


                        workSheet.Cells[rowIndex, 5].Value = items.ESignRequestDate;
                        workSheet.Cells[rowIndex, 6].Value = items.ESignRequestTransactionNo;
                        workSheet.Cells[rowIndex, 7].Value = items.ESignResponseDate;
                        workSheet.Cells[rowIndex, 8].Value = items.ESignResponseTransactionNo;

                        workSheet.Cells[rowIndex, 9].Value = items.ESignResponseCode;
                        workSheet.Cells[rowIndex, 10].Value = items.Status;
                        workSheet.Cells[rowIndex, 11].Value = items.ApplicationStatus;
                        workSheet.Cells[rowIndex, 12].Value = items.ApplicationSubmitDate;
                        workSheet.Cells[rowIndex, 13].Value = items.ResponseErrorCode;
                        workSheet.Cells[rowIndex, 14].Value = items.ResponseErrorMessage;

                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                       
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

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 14])
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
                    using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 14])
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
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }


    }
}