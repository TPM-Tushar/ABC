#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   CDWrittenReportController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion


using CustomModels.Models.CDWrittenReport;
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
    [KaveriAuthorizationAttribute]

    public class CDWrittenReportController : Controller
    {
        ServiceCaller caller = null;

        /// <summary>
        /// CD Written Report View
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "CD Written Report View")]
        public ActionResult CDWrittenReportView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.CDWrittenReport;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("CDWrittenReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                CDWrittenReportViewModel reqModel = caller.GetCall<CDWrittenReportViewModel>("CDWrittenReportView", new { OfficeID = OfficeID });

                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving CDWritten Report View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);
            }
            catch (Exception)
            {
                throw;

            }
        }


        /// <summary>
        /// Get  Datatable
        /// </summary>
        /// <param name="ReqModel"></param>
        /// <returns>View</returns>
        [EventAuditLogFilter(Description = "Loads CD Written Report Datatable")]
        [ValidateAntiForgeryTokenOnAllPosts]
        [HttpPost]
        public ActionResult LoadCDWrittenReportDataTable(FormCollection formCollection)
        {
            caller = new ServiceCaller("CDWrittenReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects        
                //string FromDate = formCollection["FromDate"];
                //string ToDate = formCollection["ToDate"];
                string SroID = formCollection["SroID"];
                string DistrictID = formCollection["DistrictID"];
                string CDNumber = formCollection["CDNumber"];

                int SroId = Convert.ToInt32(SroID);
                int DistrictId = Convert.ToInt32(DistrictID);
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
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });

                System.Text.RegularExpressions.Regex regxForCDNumber = new Regex("^[a-zA-Z0-9- ]+$");
                Match mtchCDNumber = regxForCDNumber.Match(CDNumber);
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
              
                CDWrittenReportViewModel reqModel = new CDWrittenReportViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.CDNumber = CDNumber;
                reqModel.SROfficeID = SroId;
                reqModel.DistrictID = DistrictId;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.SearchValue = searchValue;
                }
                if (CDNumber == null)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select valid CDNumber"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }
                else if (!mtchCDNumber.Success)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select valid CDNumber"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (CDNumber == "Select")
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


                CDWrittenReportResModel CDWrittenReportResModel = caller.PostCall<CDWrittenReportViewModel, CDWrittenReportResModel>("LoadCDWrittenReportDataTable", reqModel, out errorMessage);
                if (CDWrittenReportResModel == null || CDWrittenReportResModel.ICDWrittenReportRecordList == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting CD Written Report Response." });
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
                    CDWrittenReportResModel.ICDWrittenReportRecordList = CDWrittenReportResModel.ICDWrittenReportRecordList.OrderBy(sortColumn + " " + sortColumnDir);
                }

                //var gridData = PendingDocsResModel.IPendingDocsDatatableRecList.Select(PendingDocsDatatableRecord => new
                //{
                //    SerialNo = PendingDocsDatatableRecord.SerialNo,
                //    District = PendingDocsDatatableRecord.District,
                //    SRO = PendingDocsDatatableRecord.SRO,
                //    NoOfDocsPresented = PendingDocsDatatableRecord.NoOfDocsPresented,
                //    NoOfDocsRegistered = PendingDocsDatatableRecord.NoOfDocsRegistered,
                //    Str_NoOfDocsKeptPending = PendingDocsDatatableRecord.Str_NoOfDocsKeptPending,
                //    Str_DocsNotRegdNotPending = PendingDocsDatatableRecord.Str_DocsNotRegdNotPending
                //});

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //  String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = CDWrittenReportResModel.ICDWrittenReportRecordList.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = CDWrittenReportResModel.ICDWrittenReportRecordList.ToArray(),
                        recordsTotal = CDWrittenReportResModel.TotalCount,
                        status = "1",
                        recordsFiltered = CDWrittenReportResModel.FilteredRecCount,
                        //PDFDownloadBtn = PDFDownloadBtn,
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
                        data = CDWrittenReportResModel.ICDWrittenReportRecordList.ToArray(),
                        recordsTotal = CDWrittenReportResModel.TotalCount,
                        status = "1",
                        recordsFiltered = CDWrittenReportResModel.TotalCount,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting CD Written Report Datatable." }, JsonRequestBehavior.AllowGet);
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
        public ActionResult GetCDNumberList(int SROCode)
        {
            try
            {
                List<SelectListItem> CD_NumberList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                CD_NumberList = caller.GetCall<List<SelectListItem>>("GetCDNumberList", new { SROCode = SROCode, FirstRecord = "Select" });
                return Json(new { CDNumberList = CD_NumberList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting CDNumber list." }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Excel
        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="SroID"></param>
        /// <param name="SelectedSRO"></param>
        /// <param name="SelectedDistrict"></param>
        /// <returns>returns excel file</returns>
        //[EventAuditLogFilter(Description = "Sro DD PO Collection Report Export To Excel")]
        //public ActionResult ExportToExcel(string FromDate, string ToDate, string SroID, string SelectedSRO)
        [EventAuditLogFilter(Description = "Export CD Written Report to EXCEL")]
        public ActionResult ExportCDWrittenReportToExcel(int DistrictCode, int SROCode,string CDNumber, string SelectedDistrict ,string SelectedSRO)
        {
            try
            {
                caller = new ServiceCaller("CDWrittenReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("CDWrittenReport.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                //DateTime frmDate, toDate;
                //DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                //DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);

                CDWrittenReportViewModel reqModel = new CDWrittenReportViewModel
                {
                   
                    SROfficeID = Convert.ToInt32(SROCode),
                    DistrictID = Convert.ToInt32(DistrictCode),
                    CDNumber= CDNumber,
                    startLen = 0,
                    totalNum = 10,
                };

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                CDWrittenReportResModel ResModel = new CDWrittenReportResModel();
                //caller = new ServiceCaller("ECDailyReceiptReportAPIController");
                //caller.HttpClient.Timeout = objTimeSpan;
                reqModel.IsExcel = true;
                ResModel = caller.PostCall<CDWrittenReportViewModel, CDWrittenReportResModel>("LoadCDWrittenReportDataTable", reqModel, out errorMessage);

                if (ResModel == null || ResModel.ICDWrittenReportRecordList==null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Getting CD Written Report Datatable..." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("CD Written Report");
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, SelectedDistrict, SelectedSRO);

                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "CDWrittenReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel for CD Written Report", URLToRedirect = "/Home/HomePage" });
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
        private string CreateExcel(CDWrittenReportResModel ResModel, string fileName, string excelHeader, string SelectedDistrict, string SelectedSRO)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("CD Written Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.ICDWrittenReportRecordList.Count());
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
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 35;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(3).Style.WrapText = true;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                   
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "S.No";
                    workSheet.Cells[7, 2].Value = "Office Name";
                    workSheet.Cells[7, 3].Value = "Document Type [Document / Marriage]";
                    workSheet.Cells[7, 4].Value = "Registration Number";
                    workSheet.Cells[7, 5].Value = "Local Server Storage Path";
                    workSheet.Cells[7, 6].Value = "File Uploaded to Central Server [Yes / No]";
                    workSheet.Cells[7, 7].Value = "State Data center Storage Path";
                    workSheet.Cells[7, 8].Value = "Size of the File (KB)";
                    workSheet.Cells[7, 9].Value = "Date of Scan";
                    workSheet.Cells[7, 10].Value = "Date of Upload";
                    workSheet.Cells[7, 11].Value = "Document Delivery Date";
                    workSheet.Cells[7,12].Value = "Registration Date";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[7, 6].Style.WrapText = true;

                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.ICDWrittenReportRecordList)
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
                        workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";


                        //workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.OfficeName;
                        workSheet.Cells[rowIndex, 3].Value = items.DocType;
                        workSheet.Cells[rowIndex, 4].Value = items.RegistrationNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.LocalServerStoragePath;
                        workSheet.Cells[rowIndex, 6].Value = items.FileUploadedToCentralServer;
                        workSheet.Cells[rowIndex, 7].Value = items.SDCStoragePath;
                        workSheet.Cells[rowIndex, 8].Value = items.SizeOfFile;
                        workSheet.Cells[rowIndex, 9].Value = items.DateOfScan;
                        workSheet.Cells[rowIndex, 10].Value = items.DateOfUpload;
                        workSheet.Cells[rowIndex, 11].Value = items.DocDeliveryDate;
                        workSheet.Cells[rowIndex, 12].Value = items.DateOfRegistration;

                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                    }
                    

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex-1), 12])
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
                    using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 9])
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

    }
}