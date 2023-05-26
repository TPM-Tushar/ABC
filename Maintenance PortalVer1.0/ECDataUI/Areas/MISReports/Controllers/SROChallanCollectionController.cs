using CustomModels.Models.MISReports.SROChallanCollection;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class SROChallanCollectionController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        [MenuHighlight]
        [EventAuditLogFilter(Description = "SRO Challan Collection View")]
        public ActionResult SROChallanCollectionView()
        {
            try
            {
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.SRODD_POCollection;
                caller = new ServiceCaller("SROChallanCollectionAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                SROChallanCollectionResponseModel reqModel = caller.GetCall<SROChallanCollectionResponseModel>("SROChallanCollectionView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving SRO Challan collection View", URLToRedirect = "/Home/HomePage" });
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

        [HttpPost]
        [EventAuditLogFilter(Description = "Get Sro Challan Collection Reports Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetSROChallanCollectionReportsDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("SROChallanCollectionAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeID = formCollection["DROfficeID"];

                int SroId = Convert.ToInt32(SROOfficeListID);
                int DroId = Convert.ToInt32(DROfficeID);
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
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

                caller = new ServiceCaller("CommonsApiController");
                
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
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


                if (string.IsNullOrEmpty(fromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (string.IsNullOrEmpty(ToDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "To Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else
                {
                    #region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
                    DateTime CurrentDate = DateTime.Now;
                    int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                    int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                    int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                    if (CMonth > 3)
                    {
                        if (FromDateyear < CYear)
                        {
                            var emptyData = Json(new
                            {
                                draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Records of current financial year only can be seen at a time"
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;

                        }
                        else
                        {
                            if (FromDateyear > CYear)
                            {
                                if (FromDateyear == CYear + 1)
                                {
                                    if (FromDateMonth > 3)
                                    {
                                        var emptyData = Json(new
                                        {
                                            draw = formCollection["draw"],
                                            recordsTotal = 0,
                                            recordsFiltered = 0,
                                            data = "",
                                            status = false,
                                            errorMessage = "Records of current financial year only can be seen at a time"
                                        });
                                        emptyData.MaxJsonLength = Int32.MaxValue;
                                        return emptyData;

                                    }

                                }
                                else
                                {
                                    var emptyData = Json(new
                                    {
                                        draw = formCollection["draw"],
                                        recordsTotal = 0,
                                        recordsFiltered = 0,
                                        data = "",
                                        status = false,
                                        errorMessage = "Records of current financial year only can be seen at a time"
                                    });
                                    emptyData.MaxJsonLength = Int32.MaxValue;
                                    return emptyData;
                                }
                            }
                            else if (FromDateyear == CYear)
                            {
                                if (FromDateMonth < 4)
                                {
                                    var emptyData = Json(new
                                    {
                                        draw = formCollection["draw"],
                                        recordsTotal = 0,
                                        recordsFiltered = 0,
                                        data = "",
                                        status = false,
                                        errorMessage = "Records of current financial year only can be seen at a time"
                                    });
                                    emptyData.MaxJsonLength = Int32.MaxValue;
                                    return emptyData;

                                }
                            }



                        }

                    }
                    else if (CMonth <= 3)
                    {
                        if (FromDateyear < CYear)
                        {
                            if (FromDateyear == CYear - 1)
                            {
                                if (FromDateMonth < 4)
                                {
                                    var emptyData = Json(new
                                    {
                                        draw = formCollection["draw"],
                                        recordsTotal = 0,
                                        recordsFiltered = 0,
                                        data = "",
                                        status = false,
                                        errorMessage = "Records of current financial year only can be seen at a time"
                                    });
                                    emptyData.MaxJsonLength = Int32.MaxValue;
                                    return emptyData;

                                }

                            }
                            else
                            {
                                var emptyData = Json(new
                                {
                                    draw = formCollection["draw"],
                                    recordsTotal = 0,
                                    recordsFiltered = 0,
                                    data = "",
                                    status = false,
                                    errorMessage = "Records of current financial year only can be seen at a time"
                                });
                                emptyData.MaxJsonLength = Int32.MaxValue;
                                return emptyData;


                            }

                        }

                    }
                    #endregion
                }

                #endregion

                #region Validate date Inputs

                #endregion

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                SROChallanCollectionResponseModel reqModel = new SROChallanCollectionResponseModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;

                caller = new ServiceCaller("SroDD_POCollectionAPIController");
                caller.HttpClient.Timeout = objTimeSpan;

                int totalCount = caller.PostCall<SROChallanCollectionResponseModel, int>("GetSROChallanCollectionReportsDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                IEnumerable<SROChallanCollectionDetailsModel> result = caller.PostCall<SROChallanCollectionResponseModel, List<SROChallanCollectionDetailsModel>>("GetSroChallanCollectionReportsDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Sro challan collection reports details." });
                }

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //  Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
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
                        result = result.Where(m => m.DocumentNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.ReceiptNumber.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.PresentDatetime.ToLower().Contains(searchValue.ToLower()) ||
                        m.DDChalNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.StampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegistrationFee.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.SrNo.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.DDAmount.ToString().ToLower().Contains(searchValue.ToLower())
                       );
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(IndexIIReportsDetailsModel => new
                {

                    SrNo = IndexIIReportsDetailsModel.SrNo,
                    SroName = IndexIIReportsDetailsModel.SroName,
                    DocumentNumber = IndexIIReportsDetailsModel.DocumentNumber,
                    ReceiptNumber = IndexIIReportsDetailsModel.ReceiptNumber,
                    PresentDatetime = IndexIIReportsDetailsModel.PresentDatetime,
                    DDChalNumber = IndexIIReportsDetailsModel.DDChalNumber,
                    StampDuty = IndexIIReportsDetailsModel.StampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegistrationFee = IndexIIReportsDetailsModel.RegistrationFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    DDAmount = IndexIIReportsDetailsModel.DDAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                });

                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Sro challan collection reports details." });
            }
        }

        #region Excel
        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Sro Challan Collection Report Export To Excel")]
        public ActionResult ExportToExcel(string FromDate, string ToDate, string SROOfficeListID, string SelectedDistrict, string SelectedSRO)
        {
            try
            {
                caller = new ServiceCaller("SROChallanCollectionAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("SroChallanCollection.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);


                SROChallanCollectionResponseModel model = new SROChallanCollectionResponseModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    //    DROfficeID = Convert.ToInt32(DROfficeID),

                    startLen = 0,
                    totalNum = 10,
                };

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                List<SROChallanCollectionDetailsModel> objListItemsToBeExported = new List<SROChallanCollectionDetailsModel>();

                caller = new ServiceCaller("SROChallanCollectionAPIController");
                caller.HttpClient.Timeout = objTimeSpan;
                int totalCount = caller.PostCall<SROChallanCollectionResponseModel, int>("GetSROChallanCollectionReportsDetailsTotalCount", model);
                model.totalNum = totalCount;
                model.IsExcel = true;
                objListItemsToBeExported = caller.PostCall<SROChallanCollectionResponseModel, List<SROChallanCollectionDetailsModel>>("GetSroChallanCollectionReportsDetails", model, out errorMessage);
                if (objListItemsToBeExported == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);

                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("SRO Challan Collection Report Between ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SelectedDistrict, SelectedSRO);
                // string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);


                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "SroDD_POCollectionReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="SROName"></param>
        /// <returns>returns excel file path</returns>
        private string CreateExcel(List<SROChallanCollectionDetailsModel> objListItemsToBeExported, string fileName, string excelHeader, string SelectedDistrict, string SelectedSRO)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SRO Challan Collection Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (objListItemsToBeExported.Count() - 1);
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    workSheet.Cells[5, 1, 5, 9].Merge = true;

                    //workSheet.Cells[4, 1, 4, 7].Merge = true;

                    workSheet.Column(7).Style.WrapText = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 1].Value = "Sr No";
                    workSheet.Cells[7, 2].Value = "SRO Name";
                    workSheet.Cells[7, 3].Value = "Document Number";
                    workSheet.Cells[7, 4].Value = "Receipt Number";
                    workSheet.Cells[7, 5].Value = "Present Datetime";
                    workSheet.Cells[7, 7].Value = "Stamp Duty ( in Rs. )";
                    workSheet.Cells[7, 6].Value = "DD Challan Number";
                    workSheet.Cells[7, 8].Value = "Registration Fees ( in Rs. )";
                    workSheet.Cells[7, 9].Value = "DD Amount ( in Rs. ) ";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";


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
                        workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SroName;
                        workSheet.Cells[rowIndex, 3].Value = items.DocumentNumber;
                        workSheet.Cells[rowIndex, 4].Value = items.ReceiptNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.PresentDatetime;
                        workSheet.Cells[rowIndex, 6].Value = items.DDChalNumber;
                        workSheet.Cells[rowIndex, 7].Value = items.StampDuty;
                        workSheet.Cells[rowIndex, 8].Value = items.RegistrationFee;
                        workSheet.Cells[rowIndex, 9].Value = items.DDAmount;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    workSheet.Cells[rowIndex, 1].Value = "";
                    workSheet.Row(rowIndex - 1).Style.Font.Bold = true;
                    workSheet.Cells[(rowIndex - 1), 1].Value = "";

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 9])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    //using (ExcelRange Rng = workSheet.Cells[8, 6, (rowIndex - 1), 8])
                    //{
                    //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                    //}
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


        [EventAuditLogFilter(Description = "Sro Challan Collection Report Validation of Search Parameters")]
        public ActionResult ValidateSearchParameters(string FromDate, string ToDate, string SROOfficeListID, string DROfficeID)
        {
            try
            {
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                System.Text.RegularExpressions.Regex regx = new Regex("^[0-9]*$");
                Match mtchSRO = regx.Match(SROOfficeListID);
                Match mtchDistrict = regx.Match(DROfficeID);

                #region Server Side Validation

                if (string.IsNullOrEmpty(DROfficeID))
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid District" }, JsonRequestBehavior.AllowGet);

                }
                else if (!mtchDistrict.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid District" }, JsonRequestBehavior.AllowGet);

                }

                if (string.IsNullOrEmpty(SROOfficeListID))
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid SRO Office" }, JsonRequestBehavior.AllowGet);

                }
                else if (!mtchSRO.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid SRO Office" }, JsonRequestBehavior.AllowGet);

                }

                if (string.IsNullOrEmpty(FromDate))
                {
                    return Json(new { success = false, errorMessage = "From Date required" }, JsonRequestBehavior.AllowGet);

                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    return Json(new { success = false, errorMessage = "To Date required" }, JsonRequestBehavior.AllowGet);

                }
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                if (!boolFrmDate)
                {
                    return Json(new { success = false, errorMessage = "Invalid From Date" }, JsonRequestBehavior.AllowGet);

                }
                if (!boolToDate)
                {
                    return Json(new { success = false, errorMessage = "Invalid To Date" }, JsonRequestBehavior.AllowGet);

                }
                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                if (frmDate > toDate)
                {
                    return Json(new { success = false, errorMessage = "From Date can not be larger than To Date" }, JsonRequestBehavior.AllowGet);

                }


                if (!string.IsNullOrEmpty(FromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    DateTime DateTime_FromDate = Convert.ToDateTime(FromDate);
                    DateTime DateTime_Todate = Convert.ToDateTime(ToDate);

                    if ((DateTime_Todate - DateTime_FromDate).TotalDays > 365)
                    {
                        return Json(new { success = false, errorMessage = "You can only see records of one year" }, JsonRequestBehavior.AllowGet);
                    }
                }


                #region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
                //DateTime CurrentDate = DateTime.Now;
                //int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                //int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                //int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                //int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                //if (CMonth > 3)
                //{
                //    if (FromDateyear < CYear)
                //    {
                //        return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);

                //    }
                //    else
                //    {
                //        if (FromDateyear > CYear)
                //        {
                //            if (FromDateyear == CYear + 1)
                //            {
                //                if (FromDateMonth > 3)
                //                    return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);

                //            }
                //            else
                //            {
                //                return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);

                //            }
                //        }
                //        else if (FromDateyear == CYear)
                //        {
                //            if (FromDateMonth < 4)
                //                return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);

                //        }
                //    }
                //}
                //else if (CMonth <= 3)
                //{
                //    if (FromDateyear < CYear)
                //    {
                //        if (FromDateyear == CYear - 1)
                //        {
                //            if (FromDateMonth < 4)
                //                return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);

                //        }
                //        else
                //        {
                //            return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);

                //        }
                //    }
                //}
                #endregion


                #endregion
                return Json(new { success = true, errorMessage = "To Date required" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Validating  Search Parameters of SroDD_POCollection Report", URLToRedirect = "/Home/HomePage" });
            }
        }

    }
}