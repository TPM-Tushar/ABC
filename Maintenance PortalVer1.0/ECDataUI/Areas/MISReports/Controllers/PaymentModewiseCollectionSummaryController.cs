using CustomModels.Models.MISReports.PaymmentModeWiseCollectionSummary;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    public class PaymentModewiseCollectionSummaryController : Controller
    {
        ServiceCaller caller = null;
        /// <summary>
        /// EC Daily Receipt Details
        /// </summary>
        /// <returns>returns view</returns>
        /// 
        [MenuHighlight]
        [EventAuditLogFilter(Description = "Payment Modewise Collection Summary View")]
        public ActionResult PaymentModewiseCollectionSummaryView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ECDailyReceiptDetails;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("PaymmentModeWiseCollectionSummaryAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                PaymmentModeWiseCollectionSummaryView reqModel = caller.GetCall<PaymmentModeWiseCollectionSummaryView>("PaymentModeWiseCollectionSummaryView", new { OfficeID = OfficeID });

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Daily Receipt Details View", URLToRedirect = "/Home/HomePage" });
                }
                return View(reqModel);
            }
            catch (Exception)
            {
                throw;

            }
        }

        /// <summary>
        /// Get Daily Receipt Details Datatable
        /// </summary>
        /// <param name="ReqModel"></param>
        /// <returns>View</returns>
        [EventAuditLogFilter(Description = "Loads Payment Modewise Collection Summary Datatable")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult GetPaymentModeWiseRPTTableData(FormCollection formCollection)
        {
            caller = new ServiceCaller("PaymmentModeWiseCollectionSummaryAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        
                string SroID = formCollection["SroID"];
                string DistrictID = formCollection["DistrictID"];
                string FinYearID = formCollection["FinYearID"];
                string PayMentModeID = formCollection["PayMentModeID"];
                string ReceiptTypeID = formCollection["ReceiptTypeID"];
                int SroId = Convert.ToInt32(SroID);
                int DistrictId = Convert.ToInt32(DistrictID);
                int FinYearId = Convert.ToInt32(FinYearID);
                int PayMentModeid = Convert.ToInt32(PayMentModeID);
                int ReceiptTypeid = Convert.ToInt32(ReceiptTypeID);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });
                //Validation For DR Login
                if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                {
                    //Validation for DR when user do not select any sro which is by default "Select"
                    if ((SroId == 0))
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                else
                {//Validations of Logins other than SR and DR

                    //if ((SroId == 0 && DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
                    //{
                    //    var emptyData = Json(new
                    //    {
                    //        draw = formCollection["draw"],
                    //        recordsTotal = 0,
                    //        recordsFiltered = 0,
                    //        data = "",
                    //        status = false,
                    //        errorMessage = "Please select any District"
                    //    });
                    //    emptyData.MaxJsonLength = Int32.MaxValue;
                    //    return emptyData;
                    //}
                    //else if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
                    //{
                    //    var emptyData = Json(new
                    //    {
                    //        draw = formCollection["draw"],
                    //        recordsTotal = 0,
                    //        recordsFiltered = 0,
                    //        data = "",
                    //        status = false,
                    //        errorMessage = "Please select any SRO"
                    //    });
                    //    emptyData.MaxJsonLength = Int32.MaxValue;
                    //    return emptyData;

                    //}
                }
                PaymmentModeWiseCollectionSummaryView reqModel = new PaymmentModeWiseCollectionSummaryView();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.SROfficeID = SroId;
                reqModel.DistrictID = DistrictId;
                reqModel.FinYearID = FinYearId;
                reqModel.PaymentModeID = PayMentModeid;
                reqModel.ReceiptTypeID = ReceiptTypeid;
                reqModel.SearchValue = searchValue;
                reqModel.IsExcel = false;
                //int totalCount = caller.PostCall<ECDailyReceiptRptView, int>("GetECDailyReceiptsTotalCount", reqModel, out errorMessage);

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

                PaymentModeWiseCollectionSummaryResModel ResModel = caller.PostCall<PaymmentModeWiseCollectionSummaryView, PaymentModeWiseCollectionSummaryResModel>("GetPaymentModeWiseRPTTableData", reqModel, out errorMessage);

                IEnumerable<PaymentModeWiseDetaisModel> result = ResModel.PaymentModewiseList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting EC Daily Receipt Details." });
                }
                int totalCount = ResModel.PaymentModewiseList.Count;
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
                        m.DistrictName.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.NoOfReceipts.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegistrationFeeCollected.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.NoOfStampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.StampDutyCollected.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.TotalNoofReceiptsandStampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.TotalCollection.ToLower().Contains(searchValue.ToLower()));
                        totalCount = result.Count();
                    }
                }
                //  Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

                var gridData = result.Select(PaymentModeWiseDetaisModel => new
                {
                    SrNo = PaymentModeWiseDetaisModel.SrNo,
                    DistrictName = PaymentModeWiseDetaisModel.DistrictName,
                    SROName = PaymentModeWiseDetaisModel.SROName,
                    NoOfReceipts = PaymentModeWiseDetaisModel.NoOfReceipts,
                    RegistrationFeeCollected = PaymentModeWiseDetaisModel.RegistrationFeeCollected,
                    NoOfStampDuty = PaymentModeWiseDetaisModel.NoOfStampDuty,
                    StampDutyCollected = PaymentModeWiseDetaisModel.StampDutyCollected,
                    TotalNoofReceiptsandStampDuty = PaymentModeWiseDetaisModel.TotalNoofReceiptsandStampDuty,
                    TotalCollection = PaymentModeWiseDetaisModel.TotalCollection
                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = ResModel.TotalRecords,
                        status = "1",
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
                        recordsTotal = ResModel.TotalRecords,
                        status = "1",
                        recordsFiltered = ResModel.TotalRecords,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Payment Mode Wise Summary Report." }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" });
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SroID"></param>
        /// <param name="SelectedSRO"></param>
        /// <param name="SelectedDistrict"></param>
        /// <returns>returns excel file</returns>
        //[EventAuditLogFilter(Description = "Sro DD PO Collection Report Export To Excel")]
        //public ActionResult ExportToExcel(string FromDate, string ToDate, string SroID, string SelectedSRO)
        [EventAuditLogFilter(Description = "Export Payment Modewise Collection Summary to EXCEL")]
        public ActionResult ExportPaymentModeWiseRPTToExcel(string DistrictID, string SelectedDistrictText, string SroID, string SelectedSROText, string FinYearID,string FinYearText,string PaymentModeID,string PaymentModeText,string ReceiptTypeID,string ReceiptTypeText)
        {
            try
            {
                caller = new ServiceCaller("PaymmentModeWiseCollectionSummaryAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("Payment Mode Wise Summary" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                PaymmentModeWiseCollectionSummaryView model = new PaymmentModeWiseCollectionSummaryView
                {
                    DistrictID=Convert.ToInt32(DistrictID),
                    SROfficeID = Convert.ToInt32(SroID),
                    FinYearID = Convert.ToInt32(FinYearID),
                    PaymentModeID = Convert.ToInt32(PaymentModeID),
                    ReceiptTypeID = Convert.ToInt32(ReceiptTypeID),
                    IsExcel = true
                };

                PaymentModeWiseCollectionSummaryResModel ResModel = new PaymentModeWiseCollectionSummaryResModel();
               
                model.IsExcel = true;
                ResModel = caller.PostCall<PaymmentModeWiseCollectionSummaryView, PaymentModeWiseCollectionSummaryResModel>("GetPaymentModeWiseRPTTableData", model, out errorMessage);
                if (ResModel.PaymentModewiseList == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Getting Payment Mode Wise Summary..." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Payment Mode Wise Collection Summary Report");

                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, SelectedDistrictText, SelectedSROText, FinYearText, PaymentModeText, ReceiptTypeText);
                // string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);


                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "PaymentModeWiseCollectionSummaryReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="SelectedDistrict"></param>
        /// <param name="SelectedSRO"></param>
        /// <returns>returns excel file path</returns>
        private string CreateExcel(PaymentModeWiseCollectionSummaryResModel ResModel, string fileName, string excelHeader, string SelectedDistrictText, string SelectedSROText, string FinYearText, string PaymentModeText, string ReceiptTypeText)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Payment Mode Wise Collection Summary Report");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrictText;
                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSROText;
                    workSheet.Cells[4, 1].Value = "Financial Year : " + FinYearText;
                    workSheet.Cells[5, 1].Value = "Payment Mode : " + PaymentModeText;
                    workSheet.Cells[6, 1].Value = "Receipt Type : " + ReceiptTypeText;
                    workSheet.Cells[7, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[8, 1].Value = "Total Records : " + (ResModel.PaymentModewiseList.Count());
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    workSheet.Cells[5, 1, 5, 9].Merge = true;
                    workSheet.Cells[6, 1, 6, 9].Merge = true;
                    workSheet.Cells[7, 1, 7, 9].Merge = true;
                    workSheet.Cells[8, 1, 8, 9].Merge = true;
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
                    workSheet.Row(8).Style.Font.Bold = true;
                    workSheet.Row(10).Style.Font.Bold = true;
                    int rowIndex = 11;
                    workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[10, 1].Value = "S.No";
                    workSheet.Cells[10, 2].Value = "District Name";
                    workSheet.Cells[10, 3].Value = "SRO Name";
                    workSheet.Cells[10, 4].Value = "No. Of Receipts";
                    //workSheet.Cells[7, 5].Value = "Application Number";
                    workSheet.Cells[10, 5].Value = "Registration Fee Collected";
                    workSheet.Cells[10, 6].Value = "No Of Stamp Duty";
                    workSheet.Cells[10, 7].Value = "Stamp Duty Collected";
                    workSheet.Cells[10, 8].Value = "Total No. of Receipts and Stamp Duty";
                    workSheet.Cells[10, 9].Value = "Total Collection";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(10).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.PaymentModewiseList)
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

                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.NoOfReceipts;

                        //workSheet.Cells[rowIndex, 5].Value = items.AppNo;
                        workSheet.Cells[rowIndex, 5].Value = Convert.ToDecimal(items.RegistrationFeeCollected);
                        workSheet.Cells[rowIndex, 6].Value = items.NoOfStampDuty;
                        workSheet.Cells[rowIndex, 7].Value = Convert.ToDecimal(items.StampDutyCollected);
                        workSheet.Cells[rowIndex, 8].Value = Convert.ToDecimal(items.TotalNoofReceiptsandStampDuty);
                        workSheet.Cells[rowIndex, 9].Value = items.TotalCollection;
                        workSheet.Cells[rowIndex, 10].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        rowIndex++;
                    }
                    //workSheet.Cells[rowIndex, 1].Value = "";
                    //workSheet.Cells[rowIndex, 2].Value = "";
                    //workSheet.Cells[rowIndex, 3].Value = "Total";
                    //workSheet.Cells[rowIndex, 4].Value = "";
                    //workSheet.Cells[rowIndex, 5].Value = "";
                    //workSheet.Cells[rowIndex, 6].Value = "";
                    //workSheet.Cells[rowIndex, 7].Value = "";
                    //workSheet.Cells[rowIndex, 8].Value = "";
                    //workSheet.Cells[rowIndex, 9].Value = "";

                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    using (ExcelRange Rng = workSheet.Cells[10, 1, (rowIndex-1), 9])
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
                    using (ExcelRange Rng = workSheet.Cells[10, 1, 10, 9])
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

    }
}