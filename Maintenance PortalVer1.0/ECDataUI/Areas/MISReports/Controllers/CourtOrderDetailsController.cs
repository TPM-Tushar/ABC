//using CustomModels.Models.MISReports.CourtOrderDetails;
//using ECDataUI.Common;
//using ECDataUI.Session;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.Web;
//using System.Web.Mvc;
//using System.Linq.Dynamic;
//using System.IO;

//namespace ECDataUI.Areas.MISReports.Controllers
//{
//    public class CourtOrderDetailsController : Controller
//    {
//        ServiceCaller caller = null;

//        /// <summary>
//        /// EC Daily Receipt Details
//        /// </summary>
//        /// <returns>returns view</returns>
//        public ActionResult CourtOrderDetailsView()
//        {
//            try
//            {
//                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.CourtOrderDetails;
//                int OfficeID = KaveriSession.Current.OfficeID;
//                caller = new ServiceCaller("CourtOrderDetailsAPIController");
//                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
//                caller.HttpClient.Timeout = objTimeSpan;
//                CourtOrderDetailsViewModel reqModel = caller.GetCall<CourtOrderDetailsViewModel>("CourtOrderDetailsView", new { OfficeID = OfficeID });

//                if (reqModel == null)
//                {
//                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Court Order Details", URLToRedirect = "/Home/HomePage" });

//                }
//                return View(reqModel);



//            }
//            catch (Exception)
//            {
//                throw;

//            }
//        }

//        /// <summary>
//        /// Get Daily Receipt Details Datatable
//        /// </summary>
//        /// <param name="ReqModel"></param>
//        /// <returns>View</returns>
//        [HttpPost]
//        public ActionResult LoadECDailyReceiptRptTable(FormCollection formCollection)
//        {
//            caller = new ServiceCaller("CourtOrderDetailsAPIController");
//            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
//            caller.HttpClient.Timeout = objTimeSpan;
//            try
//            {
//                #region User Variables and Objects               
//                string FromDate = formCollection["FromDate"];
//                string ToDate = formCollection["ToDate"];
//                string DistrictID = formCollection["DistrictID"];
//                int DistrictId = Convert.ToInt32(DistrictID);
//                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
//                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
//                Match mtch = regx.Match((string)searchValue);
//                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
//                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
//                DateTime frmDate, toDate;
//                bool boolFrmDate = false;
//                bool boolToDate = false;
//                CommonFunctions objCommon = new CommonFunctions();
//                String errorMessage = String.Empty;
//                #endregion                
//                int startLen = Convert.ToInt32(formCollection["start"]);
//                int totalNum = Convert.ToInt32(formCollection["length"]);
//                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
//                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
//                short OfficeID = KaveriSession.Current.OfficeID;
//                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });
//                //Validation For DR Login
//                //if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
//                //{
//                //    //Validation for DR when user do not select any sro which is by default "Select"
//                //    if ((SroId == 0))
//                //    {
//                //        var emptyData = Json(new
//                //        {
//                //            draw = formCollection["draw"],
//                //            recordsTotal = 0,
//                //            recordsFiltered = 0,
//                //            data = "",
//                //            status = false,
//                //            errorMessage = "Please select any SRO"
//                //        });
//                //        emptyData.MaxJsonLength = Int32.MaxValue;
//                //        return emptyData;
//                //    }
//                //}
//                //else
//                //{//Validations of Logins other than SR and DR

//                //    if ((SroId == 0 && DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
//                //    {
//                //        var emptyData = Json(new
//                //        {
//                //            draw = formCollection["draw"],
//                //            recordsTotal = 0,
//                //            recordsFiltered = 0,
//                //            data = "",
//                //            status = false,
//                //            errorMessage = "Please select any District"
//                //        });
//                //        emptyData.MaxJsonLength = Int32.MaxValue;
//                //        return emptyData;
//                //    }
//                //    else if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
//                //    {
//                //        var emptyData = Json(new
//                //        {
//                //            draw = formCollection["draw"],
//                //            recordsTotal = 0,
//                //            recordsFiltered = 0,
//                //            data = "",
//                //            status = false,
//                //            errorMessage = "Please select any SRO"
//                //        });
//                //        emptyData.MaxJsonLength = Int32.MaxValue;
//                //        return emptyData;

//                //    }
//                //}
//                if (string.IsNullOrEmpty(FromDate))
//                {
//                    var emptyData = Json(new
//                    {
//                        draw = formCollection["draw"],
//                        recordsTotal = 0,
//                        recordsFiltered = 0,
//                        data = "",
//                        status = false,
//                        errorMessage = "From Date required"
//                    });
//                    emptyData.MaxJsonLength = Int32.MaxValue;
//                    return emptyData;
//                }
//                else if (string.IsNullOrEmpty(ToDate))
//                {
//                    var emptyData = Json(new
//                    {
//                        draw = formCollection["draw"],
//                        recordsTotal = 0,
//                        recordsFiltered = 0,
//                        data = "",
//                        status = "0",
//                        errorMessage = "To Date required"
//                    });
//                    emptyData.MaxJsonLength = Int32.MaxValue;
//                    return emptyData;
//                }
//                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
//                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);
//                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
//                if (!boolFrmDate)
//                {
//                    var emptyData = Json(new
//                    {
//                        draw = formCollection["draw"],
//                        recordsTotal = 0,
//                        recordsFiltered = 0,
//                        data = "",
//                        status = "0",
//                        errorMessage = "Invalid From Date"

//                    });
//                    emptyData.MaxJsonLength = Int32.MaxValue;
//                    return emptyData;
//                }
//                else if (!boolToDate)
//                {
//                    var emptyData = Json(new
//                    {
//                        draw = formCollection["draw"],
//                        recordsTotal = 0,
//                        recordsFiltered = 0,
//                        data = "",
//                        status = "0",
//                        errorMessage = "Invalid To Date"
//                    });
//                    emptyData.MaxJsonLength = Int32.MaxValue;
//                    return emptyData;
//                }
//                else if (frmDate > toDate)
//                {
//                    var emptyData = Json(new
//                    {
//                        draw = formCollection["draw"],
//                        recordsTotal = 0,
//                        recordsFiltered = 0,
//                        data = "",
//                        status = "0",
//                        errorMessage = "From Date can not be larger than To Date"
//                    });
//                    emptyData.MaxJsonLength = Int32.MaxValue;
//                    return emptyData;
//                }
//                else if ((toDate - frmDate).TotalDays > 180)//six months validation by RamanK on 20-09-2019
//                {
//                    var emptyData = Json(new
//                    {
//                        draw = formCollection["draw"],
//                        recordsTotal = 0,
//                        recordsFiltered = 0,
//                        data = "",
//                        status = "0",
//                        errorMessage = "Data of six months can be seen at a time"
//                    });
//                    emptyData.MaxJsonLength = Int32.MaxValue;
//                    return emptyData;

//                }
//                CourtOrderDetailsViewModel reqModel = new CourtOrderDetailsViewModel();
//                reqModel.startLen = startLen;
//                reqModel.totalNum = totalNum;
//                reqModel.FromDate = FromDate;
//                reqModel.ToDate = ToDate;
//                reqModel.DistrictID = DistrictId;
//                int totalCount = caller.PostCall<CourtOrderDetailsViewModel, int>("GetCourtOrderDetailsTotalCount", reqModel, out errorMessage);

//                if (searchValue != null && searchValue != "")
//                {
//                    reqModel.startLen = 0;
//                    reqModel.totalNum = totalCount;
//                }

//                CourtOrderDetailsResModel CourtOrderDetailsResModel = caller.PostCall<CourtOrderDetailsViewModel, CourtOrderDetailsResModel>("LoadECDailyReceiptRptTable", reqModel, out errorMessage);

//                IEnumerable<CourtOrderDetailsModel> result = CourtOrderDetailsResModel.CourtOrderDetailsList;
//                if (result == null)
//                {
//                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Court Order Details." });
//                }
//                if (searchValue != null && searchValue != "")
//                {
//                    reqModel.startLen = 0;
//                    reqModel.totalNum = totalCount;
//                }
//                if (!string.IsNullOrEmpty(searchValue))
//                {
//                    if (mtch.Success)
//                    {
//                        if (!string.IsNullOrEmpty(searchValue))
//                        {
//                            var emptyData = Json(new
//                            {
//                                draw = formCollection["draw"],
//                                recordsTotal = 0,
//                                recordsFiltered = 0,
//                                data = "",
//                                status = false,
//                                errorMessage = "Please enter valid Search String "
//                            });
//                            emptyData.MaxJsonLength = Int32.MaxValue;
//                            return emptyData;
//                        }
//                    }
//                    else
//                    {
//                        result = result.Where(m => m.SrNo.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.OrderNumber.ToLower().Contains(searchValue.ToLower()) ||
//                        m.IssuingAuthority.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.IssueDate.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.DataEntryDate.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.SROName.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.RegistrationArticle.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.Cancelation.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.Description.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.PartyDetails.ToString().ToLower().Contains(searchValue.ToLower()) ||
//                        m.PropertyDetails.ToLower().Contains(searchValue.ToLower()));
//                        totalCount = result.Count();
//                    }
//                }
//                //  Sorting
//                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
//                {
//                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
//                }

//                var gridData = result.Select(ECDailyReceiptDetailsModel => new
//                {
//                    SrNo = ECDailyReceiptDetailsModel.SrNo,
//                    OrderNumber = ECDailyReceiptDetailsModel.OrderNumber,
//                    IssuingAuthority = ECDailyReceiptDetailsModel.IssuingAuthority,
//                    IssueDate = ECDailyReceiptDetailsModel.IssueDate,
//                    DataEntryDate = ECDailyReceiptDetailsModel.DataEntryDate,
//                    SROName = ECDailyReceiptDetailsModel.SROName,
//                    RegistrationArticle = ECDailyReceiptDetailsModel.RegistrationArticle,
//                    Cancelation = ECDailyReceiptDetailsModel.Cancelation,
//                    Description = ECDailyReceiptDetailsModel.Description,
//                    PropertyDetails = ECDailyReceiptDetailsModel.PropertyDetails,
//                    PartyDetails = ECDailyReceiptDetailsModel.PartyDetails
//                });

//                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
//                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
//                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
//                if (searchValue != null && searchValue != "")
//                {
//                    var JsonData = Json(new
//                    {
//                        draw = formCollection["draw"],
//                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
//                        recordsTotal = totalCount,
//                        status = "1",
//                        recordsFiltered = totalCount,
//                        PDFDownloadBtn = PDFDownloadBtn,
//                        ExcelDownloadBtn = ExcelDownloadBtn
//                    });
//                    JsonData.MaxJsonLength = Int32.MaxValue;
//                    return JsonData;
//                }
//                else
//                {
//                    var JsonData = Json(new
//                    {
//                        draw = formCollection["draw"],
//                        data = gridData.ToArray(),
//                        recordsTotal = totalCount,
//                        status = "1",
//                        recordsFiltered = totalCount,
//                        PDFDownloadBtn = PDFDownloadBtn,
//                        ExcelDownloadBtn = ExcelDownloadBtn
//                    });
//                    JsonData.MaxJsonLength = Int32.MaxValue;
//                    return JsonData;
//                }
//            }
//            catch (Exception e)
//            {
//                ExceptionLogs.LogException(e);
//                return Json(new { serverError = true, errorMessage = "Error occured while getting Court Order Details." }, JsonRequestBehavior.AllowGet);
//            }
//        }


//        #region Excel
//        /// <summary>
//        /// Export To Excel
//        /// </summary>
//        /// <param name="FromDate"></param>
//        /// <param name="ToDate"></param>
//        /// <param name="SroID"></param>
//        /// <param name="SelectedSRO"></param>
//        /// <param name="SelectedDistrict"></param>
//        /// <returns>returns excel file</returns>
//        //[EventAuditLogFilter(Description = "Sro DD PO Collection Report Export To Excel")]
//        //public ActionResult ExportToExcel(string FromDate, string ToDate, string SroID, string SelectedSRO)
//        public ActionResult ExportECDailyReceiptRptToExcel(string FromDate, string ToDate,string DistrictID,string SelectedDistrict)
//        {
//            try
//            {
//                caller = new ServiceCaller("CourtOrderDetailsAPIController");
//                //TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
//                //caller.HttpClient.Timeout = objTimeSpan;
//                string fileName = string.Format("CourtOrderDetails.xlsx");
//                CommonFunctions objCommon = new CommonFunctions();
//                string errorMessage = string.Empty;
//                DateTime frmDate, toDate;
//                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
//                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

//                CourtOrderDetailsViewModel model = new CourtOrderDetailsViewModel
//                {
//                    FromDate = frmDate.ToString(),
//                    ToDate = toDate.ToString(),
//                    DistrictID = Convert.ToInt32(DistrictID),
//                    startLen = 0,
//                    totalNum = 10,
//                };

//                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
//                //if (SROName == null)
//                //{
//                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

//                //}

//                CourtOrderDetailsResModel ResModel = new CourtOrderDetailsResModel();
//                caller = new ServiceCaller("CourtOrderDetailsAPIController");
//                //caller.HttpClient.Timeout = objTimeSpan;
//                int totalCount = caller.PostCall<CourtOrderDetailsViewModel, int>("GetCourtOrderDetailsTotalCount", model);
//                model.totalNum = totalCount;
//                model.IsExcel = true;
//                ResModel = caller.PostCall<CourtOrderDetailsViewModel, CourtOrderDetailsResModel>("LoadECDailyReceiptRptTable", model, out errorMessage);
//                if (ResModel.CourtOrderDetailsList == null)
//                {
//                    return Json(new { success = false, errorMessage = "Error Occured While Getting EC Daily Receipt Details..." }, JsonRequestBehavior.AllowGet);
//                }


//                string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
//                if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
//                {
//                    throw new Exception();

//                }
//                string excelHeader = string.Format("Court Order  ({0} and {1})", FromDate, ToDate);
//                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, SelectedDistrict, SelectedSRO);
//                // string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);


//                if (string.IsNullOrEmpty(createdExcelPath))
//                {
//                    throw new Exception();

//                }
//                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
//                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
//                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ECDailyReceiptDetails" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
//            }
//            catch (Exception e)
//            {
//                ExceptionLogs.LogException(e);
//                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
//            }
//        }

//        /// <summary>
//        /// Create Excel
//        /// </summary>
//        /// <param name="ResModel"></param>
//        /// <param name="fileName"></param>
//        /// <param name="excelHeader"></param>
//        /// <param name="SelectedDistrict"></param>
//        /// <param name="SelectedSRO"></param>
//        /// <returns>returns excel file path</returns>
//        private string CreateExcel(ECDailyReceiptRptResModel ResModel, string fileName, string excelHeader, string SelectedDistrict, string SelectedSRO)
//        {
//            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
//            FileInfo templateFile = GetFileInfo(ExcelFilePath);
//            try
//            {
//                //create a new ExcelPackage
//                using (ExcelPackage package = new ExcelPackage())
//                {
//                    var workbook = package.Workbook;
//                    var workSheet = package.Workbook.Worksheets.Add("EC Daily Receipt Report");
//                    workSheet.Cells.Style.Font.Size = 14;

//                    workSheet.Cells[1, 1].Value = excelHeader;
//                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
//                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
//                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
//                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
//                    workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.DailyReceiptDetailsList.Count());
//                    workSheet.Cells[1, 1, 1, 9].Merge = true;
//                    workSheet.Cells[2, 1, 2, 9].Merge = true;
//                    workSheet.Cells[3, 1, 3, 9].Merge = true;
//                    workSheet.Cells[4, 1, 4, 9].Merge = true;
//                    workSheet.Cells[5, 1, 5, 9].Merge = true;
//                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//                    workSheet.Column(1).Width = 20;
//                    workSheet.Column(2).Width = 30;
//                    workSheet.Column(3).Width = 30;
//                    workSheet.Column(4).Width = 30;
//                    workSheet.Column(5).Width = 30;
//                    workSheet.Column(6).Width = 40;
//                    workSheet.Column(7).Width = 30;
//                    workSheet.Column(8).Width = 30;
//                    workSheet.Column(9).Width = 45;
//                    workSheet.Column(10).Width = 30;
//                    workSheet.Row(1).Style.Font.Bold = true;
//                    workSheet.Row(2).Style.Font.Bold = true;
//                    workSheet.Row(3).Style.Font.Bold = true;
//                    workSheet.Row(4).Style.Font.Bold = true;
//                    workSheet.Row(5).Style.Font.Bold = true;
//                    workSheet.Row(6).Style.Font.Bold = true;
//                    workSheet.Row(7).Style.Font.Bold = true;
//                    int rowIndex = 8;
//                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//                    workSheet.Cells[7, 1].Value = "S.No";
//                    workSheet.Cells[7, 2].Value = "Receipt Number";
//                    workSheet.Cells[7, 3].Value = "Receipt Type";
//                    workSheet.Cells[7, 4].Value = "Receipt Date";
//                    workSheet.Cells[7, 5].Value = "Application Number";
//                    workSheet.Cells[7, 6].Value = "SROffice Applicaton Number";
//                    workSheet.Cells[7, 7].Value = "Application Name";
//                    workSheet.Cells[7, 8].Value = "Issued By";
//                    workSheet.Cells[7, 9].Value = "Period Of Search";
//                    workSheet.Cells[7, 10].Value = "Amount ( in Rs. )";
//                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
//                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
//                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
//                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
//                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
//                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
//                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
//                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
//                    foreach (var items in ResModel.DailyReceiptDetailsList)
//                    {
//                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";
//                        workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";

//                        workSheet.Cells[rowIndex, 10].Style.Numberformat.Format = "0.00";
//                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
//                        workSheet.Cells[rowIndex, 2].Value = items.ReceiptNo;
//                        workSheet.Cells[rowIndex, 3].Value = items.ReceiptType;
//                        workSheet.Cells[rowIndex, 4].Value = items.ReceiptDate;

//                        workSheet.Cells[rowIndex, 5].Value = items.AppNo;
//                        workSheet.Cells[rowIndex, 6].Value = items.SrOfficeAppNo;
//                        workSheet.Cells[rowIndex, 7].Value = items.AppName;
//                        workSheet.Cells[rowIndex, 8].Value = items.IssuedBy;
//                        workSheet.Cells[rowIndex, 9].Value = items.PeriodOfSearch;
//                        workSheet.Cells[rowIndex, 10].Value = items.Amount;
//                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;
//                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
//                        workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
//                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
//                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
//                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
//                        rowIndex++;
//                    }
//                    workSheet.Cells[rowIndex, 1].Value = "";
//                    workSheet.Cells[rowIndex, 2].Value = "";
//                    workSheet.Cells[rowIndex, 3].Value = "";
//                    workSheet.Cells[rowIndex, 4].Value = "";
//                    workSheet.Cells[rowIndex, 5].Value = "";
//                    workSheet.Cells[rowIndex, 6].Value = "";
//                    workSheet.Cells[rowIndex, 7].Value = "";
//                    workSheet.Cells[rowIndex, 8].Value = "";
//                    workSheet.Cells[rowIndex, 9].Value = "Total";
//                    workSheet.Cells[rowIndex, 10].Value = ResModel.TotalAmount;

//                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
//                    workSheet.Row(rowIndex).Style.Font.Bold = true;
//                    workSheet.Cells[rowIndex, 10].Style.Numberformat.Format = "0.00";
//                    workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

//                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex), 10])
//                    {

//                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
//                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
//                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
//                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
//                    }
//                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 1])
//                    {
//                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
//                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
//                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
//                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

//                    }
//                    package.SaveAs(templateFile);
//                }
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//            return ExcelFilePath;
//        }

//        /// <summary>
//        /// Get File Info
//        /// </summary>
//        /// <param name="tempExcelFilePath"></param>
//        /// <returns>returns file info</returns>
//        public static FileInfo GetFileInfo(string tempExcelFilePath)
//        {
//            var fi = new FileInfo(tempExcelFilePath);
//            return fi;
//        }
//        #endregion


//        #region Download report in PDF
//        /// <summary>
//        /// Export Report To PDF
//        /// </summary>
//        /// <param name="FromDate"></param>
//        /// <param name="ToDate"></param>
//        /// <param name="SroID"></param>
//        /// <param name="SelectedSRO"></param>
//        /// <param name="SelectedDistrict"></param>
//        /// <returns>returns pdf file</returns>
//        //[EventAuditLogFilter(Description = "EC Daily Receipt Report")]
//        public ActionResult ExportECDailyReceiptRptToPDF(string FromDate, string ToDate, string SroID, string SelectedSRO, string SelectedDistrict)
//        {
//            try
//            {
//                CommonFunctions objCommon = new CommonFunctions();
//                string errorMessage = string.Empty;

//                ECDailyReceiptRptView model = new ECDailyReceiptRptView
//                {
//                    SROfficeID = Convert.ToInt32(SroID),
//                    FromDate = FromDate,
//                    ToDate = ToDate,
//                    startLen = 0,
//                    totalNum = 10
//                };

//                List<ECDailyReceiptDetailsModel> objListItemsToBeExported = new List<ECDailyReceiptDetailsModel>();

//                caller = new ServiceCaller("ECDailyReceiptReportAPIController");


//                int totalCount = caller.PostCall<ECDailyReceiptRptView, int>("GetECDailyReceiptsTotalCount", model, out errorMessage);
//                model.totalNum = totalCount;
//                ECDailyReceiptRptResModel ECDataReceiptResModel = new ECDailyReceiptRptResModel();
//                // To get total records of EC Daily Receipt Details 
//                ECDataReceiptResModel = caller.PostCall<ECDailyReceiptRptView, ECDailyReceiptRptResModel>("GetECDailyReceiptDetails", model, out errorMessage);
//                objListItemsToBeExported = ECDataReceiptResModel.DailyReceiptDetailsList;
//                if (objListItemsToBeExported == null)
//                {
//                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
//                }

//                //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
//                string fileName = string.Format("ECDailyReceiptReport.pdf");
//                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
//                string pdfHeader = string.Format("EC Daily Receipt Report Between (" + FromDate + " and " + ToDate + ")");

//                //To get SRONAME
//                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

//                //Create Temp PDF File
//                // byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader, SROName);
//                byte[] pdfBytes = CreatePDFFile(ECDataReceiptResModel, fileName, pdfHeader, SelectedDistrict, SelectedSRO);

//                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ECDailyReceiptReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");

//            }
//            catch (Exception e)
//            {
//                ExceptionLogs.LogException(e);
//                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
//            }
//        }

//        /// <summary>
//        /// Create PDF File
//        /// </summary>
//        /// <param name="ResModel"></param>
//        /// <param name="fileName"></param>
//        /// <param name="pdfHeader"></param>
//        /// <param name="SelectedDistrict"></param>
//        /// <param name="SelectedSRO"></param>
//        /// <returns>returns pdf byte array</returns>
//        private byte[] CreatePDFFile(ECDailyReceiptRptResModel ResModel, string fileName, string pdfHeader, string SelectedDistrict, string SelectedSRO)
//        {
//            string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));

//            if (!Directory.Exists(folderPath))
//            {
//                Directory.CreateDirectory(folderPath);
//            }
//            string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
//            try
//            {
//                byte[] pdfBytes = null;

//                using (MemoryStream ms = new MemoryStream())
//                {
//                    using (Document doc = new Document(PageSize.A4.Rotate(), 35, 10, 10, 25))
//                    {
//                        using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
//                        {

//                            //  string Info = string.Format("Print Date Time : {0}   Total Records : {1}  SRO Name : {2}", DateTime.Now.ToString(), SROName);
//                            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
//                            var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
//                            doc.Open();
//                            Paragraph addHeading = new Paragraph(pdfHeader, headerTextFont)
//                            {
//                                Alignment = 1,
//                            };
//                            //Paragraph Info = new Paragraph(Info, redListTextFont)
//                            //{
//                            //    Alignment = 1,
//                            //};
//                            Paragraph addSpace = new Paragraph(" ")
//                            {
//                                Alignment = 1
//                            };
//                            var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
//                            //var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(128,191,255));
//                            var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));


//                            var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
//                            var totalChunk = new Chunk("Total Records: ", blackListTextFont);
//                            var SroNameChunk = new Chunk("SRO : ", blackListTextFont);
//                            var DistrictChunk = new Chunk("District : ", blackListTextFont);



//                            var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
//                            string count = ResModel.DailyReceiptDetailsList.Count().ToString();
//                            var countChunk = new Chunk(count, redListTextFont);
//                            var District = new Chunk(SelectedDistrict + "      ", redListTextFont);
//                            var SRO = new Chunk(SelectedSRO + "      ", redListTextFont);


//                            var titlePhrase = new Phrase(titleChunk)
//                        {
//                            descriptionChunk
//                        };
//                            var totalPhrase = new Phrase(totalChunk)
//                        {
//                            countChunk
//                        };

//                            var SROPhrase = new Phrase(SroNameChunk)
//                        {
//                            SRO
//                        };
//                            var DistrictPhrase = new Phrase(DistrictChunk)
//                        {
//                            District
//                        };
//                            var FontItalic = FontFactory.GetFont("Arial", 10, 2, new BaseColor(94, 94, 94));
//                            Paragraph NotePara = new Paragraph("Note : This report is based on pre processed data considered upto : ", FontItalic);
//                            NotePara.Alignment = Element.ALIGN_RIGHT;

//                            doc.Add(addHeading);
//                            doc.Add(addSpace);
//                            doc.Add(DistrictPhrase);
//                            doc.Add(SROPhrase);
//                            doc.Add(titlePhrase);
//                            //doc.Add(SroNamePhrase);
//                            doc.Add(totalPhrase);
//                            //doc.Add(addSpace);
//                            //doc.Add(NotePara);
//                            doc.Add(addSpace);
//                            doc.Add(ReportTable(ResModel));
//                            doc.Close();
//                        }
//                        pdfBytes = AddpageNumber(ms.ToArray());
//                    }

//                }
//                return pdfBytes;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        //To get PdfPtable of EC Daily Receipt Details 
//        /// <summary>
//        /// Report Table
//        /// </summary>
//        /// <param name="ResModel"></param>
//        /// <returns>returns pdf table</returns>
//        private PdfPTable ReportTable(ECDailyReceiptRptResModel ResModel)
//        {
//            string SerialNumber = "S. No.";
//            string ReceiptNumber = "Receipt Number";
//            string ReceiptType = "Receipt Type";
//            string ReceiptDate = "Receipt Date";
//            string SROfficeApplicationNumber = "SR Office Application Number";
//            string ApplicantName = "Applicant Name";
//            string IssuedBy = "Issued By";
//            string PeriodOfSearch = "Period Of Search";
//            string Amount = "Amount ( in Rs. )";

//            try
//            {
//                string[] col = { SerialNumber, ReceiptNumber, ReceiptType, ReceiptDate, SROfficeApplicationNumber, ApplicantName, IssuedBy, PeriodOfSearch, Amount };
//                PdfPTable table = new PdfPTable(9)
//                {
//                    WidthPercentage = 100
//                };
//                // table.DefaultCell.FixedHeight = 500f;

//                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
//                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
//                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
//                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 14);

//                //to repeat Headers
//                table.HeaderRows = 1;
//                // then set the column's __relative__ widths
//                table.SetWidths(new Single[] { 4, 4, 5, 6, 6, 6, 7, 5, 8 });
//                /*
//                * by default tables 'collapse' on surrounding elements,
//                * so you need to explicitly add spacing
//                */
//                //table.SpacingBefore = 10;

//                // PdfPCell cell = null;
//                PdfPCell cell1 = null;
//                PdfPCell cell2 = null;
//                PdfPCell cell3 = null;
//                PdfPCell cell4 = null;
//                PdfPCell cell5 = null;
//                PdfPCell cell6 = null;
//                PdfPCell cell7 = null;
//                PdfPCell cell8 = null;
//                PdfPCell cell9 = null;
//                PdfPCell cell10 = null;


//                for (int i = 0; i < col.Length; ++i)
//                {
//                    PdfPCell cell = new PdfPCell(new Phrase(col[i]))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
//                    table.AddCell(cell);
//                }

//                foreach (var items in ResModel.DailyReceiptDetailsList)
//                {
//                    cell1 = new PdfPCell(new Phrase(items.SrNo.ToString(), tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
//                    cell1.BackgroundColor = BaseColor.WHITE;
//                    cell2 = new PdfPCell(new Phrase(items.ReceiptNo, tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell2.HorizontalAlignment = Element.ALIGN_CENTER;
//                    cell2.BackgroundColor = BaseColor.WHITE;

//                    cell3 = new PdfPCell(new Phrase(items.AppNo, tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
//                    cell3.BackgroundColor = BaseColor.WHITE;

//                    cell4 = new PdfPCell(new Phrase(items.SrOfficeAppNo.ToString(), tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;
//                    cell4.BackgroundColor = BaseColor.WHITE;

//                    cell5 = new PdfPCell(new Phrase(items.AppName.ToString(), tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell5.HorizontalAlignment = Element.ALIGN_LEFT;
//                    cell5.BackgroundColor = BaseColor.WHITE;

//                    cell6 = new PdfPCell(new Phrase(items.IssuedBy.ToString(), tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell6.BackgroundColor = BaseColor.WHITE;
//                    cell6.HorizontalAlignment = Element.ALIGN_LEFT;


//                    cell7 = new PdfPCell(new Phrase(items.PeriodOfSearch.ToString(), tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell7.BackgroundColor = BaseColor.WHITE;
//                    cell7.HorizontalAlignment = Element.ALIGN_CENTER;


//                    cell8 = new PdfPCell(new Phrase(items.Amount.ToString("F"), tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell8.BackgroundColor = BaseColor.WHITE;
//                    cell8.HorizontalAlignment = Element.ALIGN_RIGHT;

//                    cell9 = new PdfPCell(new Phrase(items.ReceiptType, tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell9.BackgroundColor = BaseColor.WHITE;
//                    cell9.HorizontalAlignment = Element.ALIGN_LEFT;

//                    cell10 = new PdfPCell(new Phrase(items.ReceiptDate, tableContentFont))
//                    {
//                        BackgroundColor = new BaseColor(204, 255, 255)
//                    };
//                    cell10.BackgroundColor = BaseColor.WHITE;
//                    cell10.HorizontalAlignment = Element.ALIGN_CENTER;

//                    table.AddCell(cell1);
//                    table.AddCell(cell2);
//                    table.AddCell(cell9);
//                    table.AddCell(cell10);
//                    //table.AddCell(cell3);
//                    table.AddCell(cell4);
//                    table.AddCell(cell5);
//                    table.AddCell(cell6);
//                    table.AddCell(cell7);
//                    table.AddCell(cell8);
//                }
//                PdfPCell bottomCell = null;
//                for (int i = 0; i < col.Length; ++i)
//                {
//                    if (i == 0)
//                    {
//                        bottomCell = new PdfPCell(new Phrase(""))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;
//                    }
//                    if (i == 1)
//                    {
//                        bottomCell = new PdfPCell(new Phrase(""))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;
//                    }
//                    if (i == 2)
//                    {
//                        bottomCell = new PdfPCell(new Phrase(""))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;
//                    }
//                    if (i == 3)
//                    {
//                        bottomCell = new PdfPCell(new Phrase(""))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;
//                    }
//                    if (i == 4)
//                    {
//                        bottomCell = new PdfPCell(new Phrase(""))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;
//                    }
//                    if (i == 5)
//                    {
//                        bottomCell = new PdfPCell(new Phrase(""))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;
//                    }
//                    if (i == 6)
//                    {
//                        bottomCell = new PdfPCell(new Phrase(""))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;
//                    }
//                    if (i == 8)
//                    {
//                        bottomCell = new PdfPCell(new Phrase(ResModel.TotalAmount.ToString("F")))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;
//                    }

//                    if (i == 7)
//                    {
//                        bottomCell = new PdfPCell(new Phrase("Total"))
//                        {
//                            BackgroundColor = new BaseColor(226, 226, 226)
//                        };
//                        bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;
//                    }
//                    table.AddCell(bottomCell);
//                }
//                return table;
//            }
//            catch (Exception)
//            {
//                throw;
//            }
//        }

//        //To add paging to EC Daily Receipt Details report table in PDF
//        /// <summary>
//        /// Add page Number
//        /// </summary>
//        /// <param name="inputArray"></param>
//        /// <returns>returns pdf byte array with page number</returns>
//        public byte[] AddpageNumber(byte[] inputArray)
//        {
//            byte[] pdfBytes = null;
//            CommonFunctions objCommon = new CommonFunctions();
//            iTextSharp.text.Font fntrow = objCommon.DefineNormaFont("Times New Roman", 12);

//            using (MemoryStream stream = new MemoryStream())
//            {

//                PdfReader reader = new PdfReader(inputArray);
//                using (PdfStamper stamper = new PdfStamper(reader, stream))
//                {
//                    int pages = reader.NumberOfPages;
//                    for (int i = 1; i <= pages; i++)
//                    {
//                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_MIDDLE, new Phrase("Page " + i.ToString() + " of " + pages, fntrow), 420f, 16f, 0);
//                    }
//                }
//                pdfBytes = stream.ToArray();
//            }

//            return pdfBytes;

//        }

//        #endregion

//        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
//        {
//            try
//            {
//                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
//                ServiceCaller caller = new ServiceCaller("CommonsApiController");
//                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" });
//                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
//            }
//            catch (Exception e)
//            {
//                ExceptionLogs.LogException(e);
//                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
//            }
//        }


//    }
//}