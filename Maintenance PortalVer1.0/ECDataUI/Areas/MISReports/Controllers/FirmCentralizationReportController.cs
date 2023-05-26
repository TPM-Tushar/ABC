using CustomModels.Models.MISReports.FirmCentralizationReport;
using ECDataUI.Common;
using ECDataUI.Filters;
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
    [KaveriAuthorization]
    public class FirmCentralizationReportController : Controller
    {
        ServiceCaller caller = new ServiceCaller("FirmCentralizationReportApiController");
        [MenuHighlight]
        public ActionResult FirmCentralizationReportView()
        {
            try
            {
                FirmCentralizationReportViewModel viewModel = new FirmCentralizationReportViewModel();
                FirmCentralizationReportViewModel resultViewModel = caller.GetCall<FirmCentralizationReportViewModel>("FirmCentralizationReportView", viewModel);
                return View(resultViewModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        public ActionResult GetFirmCentralizationDetails(FormCollection formCollection)
        {
            try
            {
                FirmCentralizationReportViewModel firmCentralizationReportViewModel = new FirmCentralizationReportViewModel();
                FirmCentralizationReportResultModel firmCentralizationReportResultModel = new FirmCentralizationReportResultModel();
                caller = new ServiceCaller("FirmCentralizationReportApiController");
                firmCentralizationReportViewModel.DROfficeID = Convert.ToInt32(formCollection["DistrictID"]);
                firmCentralizationReportViewModel.FromDate = Convert.ToString(formCollection["FromDate"]);
                firmCentralizationReportViewModel.ToDate = Convert.ToString(formCollection["ToDate"]);
                firmCentralizationReportViewModel.CCFileDetailsBy = Convert.ToString(formCollection["SearchByParameter"]);
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                DateTime fromDate = new DateTime();
                DateTime toDate = new DateTime();
                bool boolfrmDate = DateTime.TryParse(DateTime.ParseExact(firmCentralizationReportViewModel.FromDate, "dd/MM/yyyy", null).ToString(), out fromDate);
                bool booltoDate = DateTime.TryParse(DateTime.ParseExact(firmCentralizationReportViewModel.ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                if (fromDate > toDate)
                    return Json(new { success = false, errorMessage = "From date cannot be greater than to date" });
                if (boolfrmDate)
                    firmCentralizationReportViewModel.DateTime_FromDate = fromDate;
                else
                    throw new Exception("From Date parsing failed");

                if (booltoDate)
                    firmCentralizationReportViewModel.DateTime_ToDate = toDate;
                else
                    throw new Exception("To Date parsing failed");


                firmCentralizationReportResultModel = caller.PostCall<FirmCentralizationReportViewModel, FirmCentralizationReportResultModel>("GetFirmCentralizationDetails", firmCentralizationReportViewModel);
                if (firmCentralizationReportResultModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Firm Centralization Details." });
                }
                IEnumerable<FirmCentralizationReportResultDetailModel> result = firmCentralizationReportResultModel.DetailsList;


                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;

                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Firm Centralization Details." });
                }
                int totalCount = firmCentralizationReportResultModel.DetailsList.Count;
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
                        result = result.Where(m => m.FirmNumber.ToString().ToLower().Contains(searchValue.ToLower()));
                        firmCentralizationReportResultModel.DetailsList = result.ToList();
                        firmCentralizationReportResultModel.TotalCount = result.Count();
                    }
                }
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + firmCentralizationReportViewModel.DROfficeID + "','" + firmCentralizationReportViewModel.FromDate + "','" + firmCentralizationReportViewModel.ToDate + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                var gridData = firmCentralizationReportResultModel.DetailsList.Select(KaveriFruitsIntegrationDetailModel => new
                {
                    SrNo = KaveriFruitsIntegrationDetailModel.Sno,
                    RegistrationID = KaveriFruitsIntegrationDetailModel.RegistrationID,
                    FirmNumber = KaveriFruitsIntegrationDetailModel.FirmNumber,
                    CDNumber = KaveriFruitsIntegrationDetailModel.CDNumber,
                    IsLocalFirmDataCentralized = KaveriFruitsIntegrationDetailModel.IsLocalFirmDataCentralized,
                    IsLocalScanDocumentUpload = KaveriFruitsIntegrationDetailModel.IsLocalScanDocumentUpload,
                    IsCDWriting = KaveriFruitsIntegrationDetailModel.IsCDWriting,
                    IsFirmDataCentralized = KaveriFruitsIntegrationDetailModel.IsFirmDataCentralized,
                    IsScanDocumentUploaded = KaveriFruitsIntegrationDetailModel.IsScanDocumentUploaded,
                    IsUploadedScanDocumentPresent = KaveriFruitsIntegrationDetailModel.IsUploadedScanDocumentPresent,
                    DateOfRegistration = KaveriFruitsIntegrationDetailModel.DateOfRegistration,
                    FilePath = KaveriFruitsIntegrationDetailModel.FilePath,
                    IsFilePresent = KaveriFruitsIntegrationDetailModel.IsFilePresent,
                    IsFileReadable = KaveriFruitsIntegrationDetailModel.IsFileReadable

                });


                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                    //data = gridData.ToArray().ToList(),
                    recordsTotal = totalCount,
                    recordsFiltered = totalCount,
                    status = "1",
                    ExcelDownloadBtn = ExcelDownloadBtn

                });
                JsonData.MaxJsonLength = Int32.MaxValue;

                return JsonData;
                //}
                //else
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request ID tempered.", URLToRedirect = "/Home/HomePage" });
                //}
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }

        }
        [HttpGet]
        public ActionResult ExportOrderDetailsToExcel(string DistrictID, string FromDate, string ToDate,string Searchby)
        {
            try
            {
                caller = new ServiceCaller("FirmCentralizationReportApiController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "Firm_Centralization_Details" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                FirmCentralizationReportViewModel firmCentralizationReportViewModel = new FirmCentralizationReportViewModel();
                DateTime fromDate = new DateTime();
                DateTime toDate = new DateTime();
                bool boolfrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out fromDate);
                bool booltoDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                if (fromDate > toDate)
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Details", URLToRedirect = "/Home/HomePage" });

                if (boolfrmDate)
                    firmCentralizationReportViewModel.DateTime_FromDate = fromDate;
                else
                    throw new Exception("From Date parsing failed");

                if (booltoDate)
                    firmCentralizationReportViewModel.DateTime_ToDate = toDate;
                else
                    throw new Exception("To Date parsing failed");
                firmCentralizationReportViewModel.DROfficeID = Convert.ToInt32(DistrictID);
                firmCentralizationReportViewModel.CCFileDetailsBy = Searchby;
                //string SroName = "-";
                //string DroName = "-";
                FirmCentralizationReportResultModel ResModel = caller.PostCall<FirmCentralizationReportViewModel, FirmCentralizationReportResultModel>("GetFirmCentralizationDetails", firmCentralizationReportViewModel);

                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Details", URLToRedirect = "/Home/HomePage" });
                }
                else if (ResModel.DetailsList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting  Details", URLToRedirect = "/Home/HomePage" });

                }
                string excelHeader = string.Format("Firm Centralization Report");

                string createdExcelPath = CreateExcelDetails(ResModel.DetailsList, fileName, excelHeader, ResModel.DistrictName, FromDate, ToDate,Searchby);
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        private string CreateExcelDetails(List<FirmCentralizationReportResultDetailModel> firmCentralizationReportResultDetailModelList, string fileName, string excelHeader, string DroName, string FromDate, string ToDate,string SearchBy)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Firm Centralization Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + DroName;
                    workSheet.Cells[3, 1].Value = "From Date : " + FromDate;
                    workSheet.Cells[4, 1].Value = "To Date : " + ToDate;
                    workSheet.Cells[5, 1].Value = "Search By : " + SearchBy;
                    //workSheet.Cells[4, 1].Value = "Log Type : " + SelectedLogType;

                    workSheet.Cells[6, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[7, 1].Value = "Total Records : " + (firmCentralizationReportResultDetailModelList.Count());

                    workSheet.Cells[1, 1, 1, 14].Merge = true;
                    workSheet.Cells[2, 1, 2, 14].Merge = true;
                    workSheet.Cells[3, 1, 3, 15].Merge = true;
                    workSheet.Cells[4, 1, 4, 14].Merge = true;
                    workSheet.Cells[5, 1, 5, 14].Merge = true;
                    workSheet.Cells[6, 1, 6, 14].Merge = true;
                    workSheet.Cells[7, 1, 7, 14].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 30;
                    workSheet.Column(14).Width = 50;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 10;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.WrapText = true;


                    workSheet.Cells[9, 1].Value = "S No.";
                    workSheet.Cells[9, 2].Value = "Registration ID";
                    workSheet.Cells[9, 3].Value = "Firm Number";
                    workSheet.Cells[9, 4].Value = "Date of Registration";
                    workSheet.Cells[9, 5].Value = "CD Number";
                    workSheet.Cells[9, 6].Value = "Local FirmMaster";
                    workSheet.Cells[9, 7].Value = "Local SocietyFirmScanmaster";
                    workSheet.Cells[9, 8].Value = "Local ScanMaster_CDID";
                    workSheet.Cells[9, 9].Value = "Central FirmMaster";
                    workSheet.Cells[9, 10].Value = "Central SocietyFirmScanmaster";
                    workSheet.Cells[9, 11].Value = "Central ScannedFileUploadDetails";
                    workSheet.Cells[9, 12].Value = "Central FileServer IsFileAvailable";
                    workSheet.Cells[9, 13].Value = "Central FileServer IsFileReadable";
                    workSheet.Cells[9, 14].Value = "Central ScannedFileUploadDetails PhysicalFilePath";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.WrapText = true;


                    workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(9).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(9).Style.Font.Bold =true;


                    foreach (var items in firmCentralizationReportResultDetailModelList)
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
                        workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 14].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.Sno;
                        workSheet.Cells[rowIndex, 2].Value = items.RegistrationID;
                        workSheet.Cells[rowIndex, 3].Value = items.FirmNumber;
                        workSheet.Cells[rowIndex, 4].Value = items.DateOfRegistration;
                        workSheet.Cells[rowIndex, 5].Value = items.CDNumber;
                        workSheet.Cells[rowIndex, 6].Value = items.bool_IsLocalFirmDataCentralized ? "Yes" : "NO";
                        if (items.bool_IsLocalFirmDataCentralized)
                            workSheet.Cells[rowIndex, 6].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        else
                            workSheet.Cells[rowIndex, 6].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        workSheet.Cells[rowIndex, 7].Value = items.bool_IsLocalScanDocumentUpload ? "Yes" : "NO";
                        if (items.bool_IsLocalScanDocumentUpload)
                            workSheet.Cells[rowIndex, 7].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        else
                            workSheet.Cells[rowIndex, 7].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        workSheet.Cells[rowIndex, 8].Value = items.bool_IsCDWriting ? "Yes" : "NO";
                        if (items.bool_IsCDWriting)
                            workSheet.Cells[rowIndex, 8].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        else
                            workSheet.Cells[rowIndex, 8].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        workSheet.Cells[rowIndex, 9].Value = items.bool_IsFirmDataCentralized ? "Yes" : "NO";
                        if (items.bool_IsFirmDataCentralized)
                            workSheet.Cells[rowIndex, 9].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        else
                            workSheet.Cells[rowIndex, 9].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        workSheet.Cells[rowIndex, 10].Value = items.bool_IsScanDocumentUploaded ? "Yes" : "NO";
                        if (items.bool_IsScanDocumentUploaded)
                            workSheet.Cells[rowIndex, 10].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        else
                            workSheet.Cells[rowIndex, 10].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        workSheet.Cells[rowIndex, 11].Value = items.bool_IsUploadedScanDocumentPresent ? "Yes" : "NO";
                        if (items.bool_IsUploadedScanDocumentPresent)
                            workSheet.Cells[rowIndex, 11].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        else
                            workSheet.Cells[rowIndex, 11].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        workSheet.Cells[rowIndex, 12].Value = items.bool_IsFilePresent ? "Yes" : "NO";
                        if (items.bool_IsFilePresent)
                            workSheet.Cells[rowIndex, 12].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                        else
                            workSheet.Cells[rowIndex, 12].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        if (SearchBy== "FileReadable")
                        {
                            workSheet.Cells[rowIndex, 13].Value = items.bool_IsFileReadable ? "Yes" : "NO";
                            if (items.bool_IsFileReadable)
                                workSheet.Cells[rowIndex, 13].Style.Font.Color.SetColor(System.Drawing.Color.Green);
                            else
                                workSheet.Cells[rowIndex, 13].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed); 
                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 13].Value = "N.A";
                        }
                        workSheet.Cells[rowIndex, 14].Value = items.FilePath;

                        workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 10].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 11].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 12].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 13].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 14].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 14].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        rowIndex++;
                    }

                    using (ExcelRange Rng = workSheet.Cells[9, 1, (rowIndex - 1), 14])
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
        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }
    }
}