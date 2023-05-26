using CustomModels.Models.Remittance.ChallanDetailsReport;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ChallanDetailsController : Controller
    {
        ServiceCaller caller = null;

        [HttpGet]
        [MenuHighlight]
        [EventAuditLogFilter(Description = "Challan Details View")]
        public ActionResult ChallanDetailsReport()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ECDailyReceiptDetails;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("ChallanDetailsAPIController");
                //TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                //caller.HttpClient.Timeout = objTimeSpan;
                ChallanDetailsModel reqModel = caller.GetCall<ChallanDetailsModel>("ChallanDetailsReportView", new { OfficeID = OfficeID });

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Challan Details Report View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        [HttpPost]
        [EventAuditLogFilter(Description = "Get Challan Details Report")]
        [ValidateAntiForgeryTokenOnAllPosts]
        //public ActionResult GetChallanReportDetails(FormCollection formCollection)
        public ActionResult GetChallanReportDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("ChallanDetailsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        

                //string Date = formCollection["Date"];
                string InstrumentNumber = formCollection["InstrumentNumber"];
                //string Type = formCollection["Type"];
                //string Office = formCollection["OfficeName"];
                //string SelectedOfficeType = formCollection["SelectedOfficeType"];
               

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                DateTime toDate;
                bool boolDate = false;

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


                //if (string.IsNullOrEmpty(Date))
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Date required"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                if (string.IsNullOrEmpty(InstrumentNumber))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Instrument Number is required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                Regex regex = new Regex(@"^[CR]{2}[(0-9)]{16}$");
                if(!regex.IsMatch(InstrumentNumber))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Instrument Number is not valid"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                int year = DateTime.Now.Year % 100;
                string InstrumentNumberYearSTR = InstrumentNumber.Substring(4, 2);
                if (Convert.ToInt32(InstrumentNumberYearSTR) > year)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Instrument Number is not valid"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //boolDate = DateTime.TryParse(DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

                //if (!boolDate)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Invalid Date"

                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


                ChallanDetailsModel reqModel = new ChallanDetailsModel();
                ChallanDetailsResModel resModel = new ChallanDetailsResModel();

                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;

                //reqModel.ToDate = ToDate;
                reqModel.Date = DateTime.Now.ToString();
                //reqModel.StampTypeId = Int32.Parse(Type);
                reqModel.InstrumentNumber = InstrumentNumber;
                //reqModel.OfficeTypeID = Int32.Parse(Office);
                //reqModel.ActionId = Int32.Parse(SelectedAction);
                //if (SelectedOfficeType.ToLower().Equals("dro"))
                //{
                //    reqModel.IsDRO = true;
                //}

                //reqModel.SearchValue = searchValue;
                //reqModel.IsExcel = false;
               
                resModel = caller.PostCall<ChallanDetailsModel, ChallanDetailsResModel>("GetChallanReportDetails", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Challan Details." });
                }
                IEnumerable<ChallanDetailsDataTableModel> result = resModel.challanDetailsDataTableList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Challan Details." });
                }
                int totalCount = resModel.challanDetailsDataTableList.Count;
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
                        result = result.Where(m =>
                         m.SROName.ToLower().Contains(searchValue.ToLower()) || m.ChallanNumber.ToLower().Contains(searchValue.ToLower())
                         || m.DistrictName.ToLower().Contains(searchValue.ToLower())

                        );
                        totalCount = result.Count();
                    }
                }
                //  Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }


                var gridData = result.Select(QueryExecutionStatusReportDetailModel => new
                {
                    SrNo = QueryExecutionStatusReportDetailModel.SrNo,
                    SROName = QueryExecutionStatusReportDetailModel.SROName,
                    IsPayDoneAtDROffice = QueryExecutionStatusReportDetailModel.IsPayDoneAtDROffice,
                    DistrictName = QueryExecutionStatusReportDetailModel.DistrictName,
                    ChallanNumber = QueryExecutionStatusReportDetailModel.ChallanNumber,
                    ChallanDate = QueryExecutionStatusReportDetailModel.ChallanDate,
                    Amount = QueryExecutionStatusReportDetailModel.Amount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    IsStampPayment = QueryExecutionStatusReportDetailModel.IsStampPayment,
                    IsReceiptPayment = QueryExecutionStatusReportDetailModel.IsReceiptPayment,
                    ReceiptNumber = QueryExecutionStatusReportDetailModel.ReceiptNumber,
                    Receipt_StampPayDate = QueryExecutionStatusReportDetailModel.Receipt_StampPayDate,
                    InsertDateTime = QueryExecutionStatusReportDetailModel.InsertDateTime,
                    ServiceName = QueryExecutionStatusReportDetailModel.ServiceName,
                    DocumentPendingNumber = QueryExecutionStatusReportDetailModel.DocumentPendingNumber,
                    FinalRegistrationNumber = QueryExecutionStatusReportDetailModel.FinalRegistrationNumber,

                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(), //gridData.ToArray(),
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = resModel.TotalRecords,
                        ExcelDownloadBtn = ExcelDownloadBtn,

                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Challan Details." }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Excel
        /// <summary>
        /// ExportDataReadingHistoryToExcel
        /// </summary>
        /// <param name="InstrumentNumber"></param>
        /// <param name="Date"></param>
        /// <param name="Type"></param>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Export Challan Details Report To Excel")]
        public ActionResult ExportChallanDetailsToExcel(string InstrumentNumber)
        {
            try
            {
                caller = new ServiceCaller("ChallanDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("ChallanDetailsReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate;
                //DateTime.TryParse(DateTime.ParseExact(Date, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);

                ChallanDetailsModel model = new ChallanDetailsModel
                {
                    InstrumentNumber = InstrumentNumber,
                    Date = DateTime.Now.ToString(),
                    //StampTypeId = Convert.ToInt32(Type),
                    IsExcel = true

                };


                ChallanDetailsResModel ResModel = new ChallanDetailsResModel();

                ResModel = caller.PostCall<ChallanDetailsModel, ChallanDetailsResModel>("GetChallanReportDetails", model);
                if (ResModel.challanDetailsDataTableList == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Getting Challan Details data" }, JsonRequestBehavior.AllowGet);
                }


                string excelHeader = string.Format("Challan Details Report");
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, InstrumentNumber);


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ChallanDetailsReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="InstrumentNo"></param>
        /// <param name="Type"></param>
        /// <returns>string</returns>
        private string CreateExcel(ChallanDetailsResModel ResModel, string fileName, string excelHeader, string InstrumentNo)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {

                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Challan Details Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Challan Number : " + InstrumentNo;
                    //workSheet.Cells[3, 1].Value = "Type : " + Type;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (ResModel.challanDetailsDataTableList.Count());
                    workSheet.Cells[1, 1, 1, 14].Merge = true;
                    workSheet.Cells[2, 1, 2, 14].Merge = true;
                    workSheet.Cells[3, 1, 3, 14].Merge = true;
                    workSheet.Cells[4, 1, 4, 14].Merge = true;
                    workSheet.Cells[5, 1, 5, 14].Merge = true;
                   // workSheet.Cells[6, 1, 6, 13].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 10;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 40;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 37;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 35;
                    workSheet.Column(14).Width = 35;
                    workSheet.Column(15).Width = 35;
                    //for (int i = 8; i <= 32; i++)
                    //{
                    //    workSheet.Column(i).Width = 22;
                    //    workSheet.Cells[7, i].Style.WrapText = true;
                    //}
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    //workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[6, 1].Value = "Sr No";
                    workSheet.Cells[6, 2].Value = "SRO Name";
                    workSheet.Cells[6, 3].Value = "Is Payement Done At DRO Office";
                    workSheet.Cells[6, 4].Value = "District Name";
                    workSheet.Cells[6, 5].Value = "Challan Number";
                    workSheet.Cells[6, 6].Value = "Challan Date";
                    workSheet.Cells[6, 7].Value = "Amount (in Rs.)";
                    workSheet.Cells[6, 8].Value = "Is Stamp Payment";
                    workSheet.Cells[6, 9].Value = "Is Receipt Payment";
                    workSheet.Cells[6, 10].Value = "Receipt Number";
                    workSheet.Cells[6, 11].Value = "Receipt Stamp Payment Date";
                    workSheet.Cells[6, 12].Value = "Insert Date Time";
                    workSheet.Cells[6, 13].Value = "Service Name";
                    workSheet.Cells[6, 14].Value = "Document Pending Number";                    
                    workSheet.Cells[6, 15].Value = "Final Registration Number";                    

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    //workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.challanDetailsDataTableList)
                    {
                        //workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        for (int i = 1; i <= 15; i++)
                        {
                            workSheet.Cells[rowIndex, i].Style.Font.Name = "KNB-TTUmaEN";
                        }

                        //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SROName;
                        workSheet.Cells[rowIndex, 3].Value = items.IsPayDoneAtDROffice;
                        workSheet.Cells[rowIndex, 4].Value = items.DistrictName;
                        workSheet.Cells[rowIndex, 5].Value = items.ChallanNumber;
                        workSheet.Cells[rowIndex, 6].Value = items.ChallanDate;
                        workSheet.Cells[rowIndex, 7].Value = items.Amount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        workSheet.Cells[rowIndex, 8].Value = items.IsStampPayment;
                        workSheet.Cells[rowIndex, 9].Value = items.IsReceiptPayment;
                        workSheet.Cells[rowIndex, 10].Value = items.ReceiptNumber;
                        workSheet.Cells[rowIndex, 11].Value = items.Receipt_StampPayDate;
                        workSheet.Cells[rowIndex, 12].Value = items.InsertDateTime;
                        workSheet.Cells[rowIndex, 13].Value = items.ServiceName;
                        workSheet.Cells[rowIndex, 14].Value = items.DocumentPendingNumber;
                        workSheet.Cells[rowIndex, 15].Value = items.FinalRegistrationNumber;


                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                    }

                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(rowIndex).Style.Font.Bold = true;

                    //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 15])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 13])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[6, 1, 7, 15])
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
        /// GetFileInfo
        /// </summary>
        /// <param name="tempExcelFilePath"></param>
        /// <returns>FileInfo</returns>
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }

        #endregion

    }
}