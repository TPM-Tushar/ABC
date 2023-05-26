using CustomModels.Models.Remittance.FirmCentralization;
using ECDataUI.Common;
using ECDataUI.Filters;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorization]

    public class FirmCentralizationController : Controller
    {
        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        // GET: Remittance/FirmCentralization
        public ActionResult FirmCentralizationView()
        {
           try
            {
                caller = new ServiceCaller("FirmCentralizationAPIController");
                FirmCentralizationModel reqModel = caller.GetCall<FirmCentralizationModel>("FirmCentralizationView");
                return View(reqModel);

            }
            catch(Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Firm Centralization View", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        public ActionResult GetFirmCentralizationDetails(FormCollection formCollection)
        {
            try
            {
                FirmCentralizationModel firmCentralizationReportViewModel = new FirmCentralizationModel();
                FirmCentralizationResultModel firmCentralizationResultModel = new FirmCentralizationResultModel();
                caller = new ServiceCaller("FirmCentralizationAPIController");
                firmCentralizationReportViewModel.DROfficeID = Convert.ToInt32(formCollection["DistrictID"]);
                firmCentralizationReportViewModel.FromDate = Convert.ToString(formCollection["FromDate"]);
                firmCentralizationReportViewModel.ToDate = Convert.ToString(formCollection["ToDate"]);
                firmCentralizationReportViewModel.SearchBy = Convert.ToString(formCollection["SearchByParameter"]);
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


                firmCentralizationResultModel = caller.PostCall<FirmCentralizationModel, FirmCentralizationResultModel>("GetFirmCentralizationDetails", firmCentralizationReportViewModel);
                if (firmCentralizationResultModel == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Firm Centralization Details." });
                }
                IEnumerable<FirmCentralizationTableModel> result = firmCentralizationResultModel.DetailsList;


              
                int pageSize = totalNum;
                int skip = startLen;

                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Firm Centralization Details." });
                }
                int totalCount = firmCentralizationResultModel.DetailsList.Count;
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
                        result = result.Where(m => m.L_FirmNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.L_CDNumber.ToLower().Contains(searchValue.ToLower()) ||
                         //
                         m.L_DateOfRegistration.ToLower().Contains(searchValue.ToLower()) ||
                         m.L_ScanFileName.ToLower().Contains(searchValue.ToLower()) ||
                         m.C_ScanFileName.ToLower().Contains(searchValue.ToLower()) ||
                         m.C_DateOfRegistration.ToLower().Contains(searchValue.ToLower()) ||
                         m.UploadDateTime.ToLower().Contains(searchValue.ToLower()) ||
                        //
                        m.C_FirmNumber.ToLower().Contains(searchValue.ToLower()) ||
                         m.C_CDNumber.ToLower().Contains(searchValue.ToLower()));
                        totalCount = result.Count();
                    }
                    }
                  
                
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + firmCentralizationReportViewModel.DROfficeID + "','" + firmCentralizationReportViewModel.FromDate + "','" + firmCentralizationReportViewModel.ToDate + "','" + firmCentralizationReportViewModel.SearchBy + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                var gridData = result.Select(FirmCentralizationTableModel => new
                {
                    SrNo = FirmCentralizationTableModel.Sr_No,
                    RegistrationID = FirmCentralizationTableModel.RegistrationID,
                    L_FirmNumber = FirmCentralizationTableModel.L_FirmNumber,
                    L_DateOfRegistration = FirmCentralizationTableModel.L_DateOfRegistration,
                    C_DateOfRegistration = FirmCentralizationTableModel.C_DateOfRegistration,
                    L_CDNumber = FirmCentralizationTableModel.L_CDNumber,
                    L_ScanFileName = FirmCentralizationTableModel.L_ScanFileName,
             
                    C_FirmNumber = FirmCentralizationTableModel.C_FirmNumber,
                    C_CDNumber = FirmCentralizationTableModel.C_CDNumber,
                    C_ScanFileName = FirmCentralizationTableModel.C_ScanFileName,
                    UploadDateTime = FirmCentralizationTableModel.UploadDateTime


                });


        
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date",

                    ExcelDownloadBtn = ExcelDownloadBtn

                });
                if (searchValue != null && searchValue != "")
                {

                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalNum,
                        recordsFiltered = totalCount,
                        status = "1",

                        ExcelDownloadBtn = ExcelDownloadBtn

                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalNum,
                        recordsFiltered = totalCount,
                        status = "1",

                        ExcelDownloadBtn = ExcelDownloadBtn


                    });
                }

                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }

        }

        public ActionResult ExportFirmCentralizationDetailsToExcel(string DistrictID, string FromDate, string ToDate, string SearchBy)
        {
            try
            {
                caller = new ServiceCaller("FirmCentralizationAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "Firm_Centralization_Details" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                FirmCentralizationModel firmCentralizationReportViewModel = new FirmCentralizationModel();
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
                firmCentralizationReportViewModel.SearchBy = SearchBy;
                
                FirmCentralizationResultModel ResModel = caller.PostCall<FirmCentralizationModel, FirmCentralizationResultModel>("GetFirmCentralizationDetails", firmCentralizationReportViewModel);

                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Details", URLToRedirect = "/Home/HomePage" });
                }
                else if (ResModel.DetailsList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting  Details", URLToRedirect = "/Home/HomePage" });

                }
                string excelHeader = string.Format("Firm Centralization Report");

                string createdExcelPath = CreateExcelForFirmCentralizationDetails(ResModel.DetailsList, fileName, excelHeader,  FromDate, ToDate, SearchBy);
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

        public ActionResult ExportLocalFirmDetailsToExcel(string DistrictID, string FromDate, string ToDate)
        {
            try
            {
                caller = new ServiceCaller("FirmCentralizationAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "Firm_Centralization_Local_Details" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                FirmCentralizationModel firmCentralizationReportViewModel = new FirmCentralizationModel();
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
            

                FirmCentralizationResultModel ResModel = caller.PostCall<FirmCentralizationModel, FirmCentralizationResultModel>("GetFirmCentralizationLocalDetails", firmCentralizationReportViewModel);

                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Details", URLToRedirect = "/Home/HomePage" });
                }
                else if (ResModel.Local_DetailsList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting  Details", URLToRedirect = "/Home/HomePage" });

                }
                string excelHeader = string.Format("Firm Centralization Local Report");

                string createdExcelPath = CreateExcelForLocalFirmCentralizationDetails(ResModel.Local_DetailsList, fileName, excelHeader, FromDate, ToDate,"");
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
        //
        private string CreateExcelForLocalFirmCentralizationDetails(List<LocalFirmCentralizationTableModel> result, string fileName, string excelHeader, string FromDate, string ToDate, string SearchBy)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {

                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Local Firm Centralization Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;

                    workSheet.Cells[3, 1].Value = "From Date : " + FromDate;
                    workSheet.Cells[4, 1].Value = "To Date : " + ToDate;
                   


                    workSheet.Cells[6, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[7, 1].Value = "Total Records : " + (result.Count());

                    workSheet.Cells[1, 1, 1, 11].Merge = true;
                    workSheet.Cells[2, 1, 2, 11].Merge = true;
                    workSheet.Cells[3, 1, 3, 11].Merge = true;
                    workSheet.Cells[4, 1, 4, 11].Merge = true;
                    workSheet.Cells[5, 1, 5, 11].Merge = true;
                    workSheet.Cells[6, 1, 6, 11].Merge = true;
                    workSheet.Cells[7, 1, 7, 11].Merge = true;


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
                    workSheet.Cells[9, 3].Value = "DROCode";
                    workSheet.Cells[9, 4].Value = "FirmNumber";
                    workSheet.Cells[9, 5].Value = "IsFirmDataCentralizaed";
                    workSheet.Cells[9, 6].Value = "CDNumber";
                    workSheet.Cells[9, 7].Value = "DateOfRegistration";
                    workSheet.Cells[9, 8].Value = "IsScanDocumentUploaded";
                    workSheet.Cells[9, 9].Value = "ScanFileName";
                    workSheet.Cells[9, 10].Value = "CDID";
                    workSheet.Cells[9, 11].Value = "ScanDateTime";

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
                    workSheet.Row(9).Style.Font.Bold = true;


                    foreach (var items in result)
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


                        workSheet.Cells[rowIndex, 1].Value = items.Sr_No;
                        workSheet.Cells[rowIndex, 2].Value = items.RegistrationID;
                        workSheet.Cells[rowIndex, 3].Value = items.DROCode;
                        workSheet.Cells[rowIndex, 4].Value = items.FirmNumber;
                        //workSheet.Cells[rowIndex, 5].Value = items.L_CDNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.IsFirmDataCentralizaed ? "Yes" : "NO";
                        if (items.IsFirmDataCentralizaed)
                            workSheet.Cells[rowIndex, 5].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        else
                            workSheet.Cells[rowIndex, 5].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);
                        workSheet.Cells[rowIndex,6].Value = items.CDNumber;
                        workSheet.Cells[rowIndex, 7].Value = items.DateOfRegistration;
                        workSheet.Cells[rowIndex, 8].Value = items.IsScanDocumentUploaded.Value ? "Yes" : "NO";
                        if (items.IsScanDocumentUploaded.Value)
                            workSheet.Cells[rowIndex, 8].Style.Font.Color.SetColor(System.Drawing.Color.DarkGreen);
                        else
                            workSheet.Cells[rowIndex, 8].Style.Font.Color.SetColor(System.Drawing.Color.DarkRed);

                        workSheet.Cells[rowIndex, 9].Value = items.ScanFileName;
                        workSheet.Cells[rowIndex, 10].Value = items.CDID;
                        workSheet.Cells[rowIndex, 11].Value = items.ScanDateTime;

                        workSheet.Cells[rowIndex, 1].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 2].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;



                        rowIndex++;
                    }

                    using (ExcelRange Rng = workSheet.Cells[9, 1, (rowIndex - 1), 11])
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
        //
        public ActionResult ExportCentralFirmDetailsToExcel(string DistrictID, string FromDate, string ToDate)
        {
            try
            {
                caller = new ServiceCaller("FirmCentralizationAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 20, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "Firm_Centralization_Central_Details" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                FirmCentralizationModel firmCentralizationReportViewModel = new FirmCentralizationModel();
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


                FirmCentralizationResultModel ResModel = caller.PostCall<FirmCentralizationModel, FirmCentralizationResultModel>("GetFirmCentralizationCentralDetails", firmCentralizationReportViewModel);

                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting Details", URLToRedirect = "/Home/HomePage" });
                }
                else if (ResModel.Central_DetailsList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error Occured While Getting  Details", URLToRedirect = "/Home/HomePage" });

                }
                string excelHeader = string.Format("Firm Centralization Central Report");

                string createdExcelPath = CreateExcelForCetralFirmCentralizationDetails(ResModel.Central_DetailsList, fileName, excelHeader, FromDate, ToDate, "");
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
        //
        private string CreateExcelForCetralFirmCentralizationDetails(List<CentralFirmCentralizationTableModel> result, string fileName, string excelHeader, string FromDate, string ToDate, string SearchBy)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {

                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Central Firm Centralization Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;

                    workSheet.Cells[3, 1].Value = "From Date : " + FromDate;
                    workSheet.Cells[4, 1].Value = "To Date : " + ToDate;
                    


                    workSheet.Cells[6, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[7, 1].Value = "Total Records : " + (result.Count());

                    workSheet.Cells[1, 1, 1, 11].Merge = true;
                    workSheet.Cells[2, 1, 2, 11].Merge = true;
                    workSheet.Cells[3, 1, 3, 11].Merge = true;
                    workSheet.Cells[4, 1, 4, 11].Merge = true;
                    workSheet.Cells[5, 1, 5, 11].Merge = true;
                    workSheet.Cells[6, 1, 6, 11].Merge = true;
                    workSheet.Cells[7, 1, 7, 11].Merge = true;


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
                    workSheet.Cells[9, 3].Value = "DRCode";
                    workSheet.Cells[9, 4].Value = "FirmNumber";
                    workSheet.Cells[9, 5].Value = "Pages";
                    workSheet.Cells[9, 6].Value = "CDNumber";
                    workSheet.Cells[9, 7].Value = "DateOfRegistration";
                    workSheet.Cells[9, 8].Value = "IsScanned";
                    workSheet.Cells[9, 9].Value = "Remarks";
                    workSheet.Cells[9, 10].Value = "ReceiptID";
                    workSheet.Cells[9, 11].Value = "FirmType";

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
                    workSheet.Row(9).Style.Font.Bold = true;


                    foreach (var items in result)
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


                        workSheet.Cells[rowIndex, 1].Value = items.Sr_No;
                        workSheet.Cells[rowIndex, 2].Value = items.RegistrationID;
                        workSheet.Cells[rowIndex, 3].Value = items.DRCode;
                        workSheet.Cells[rowIndex, 4].Value = items.FirmNumber;
                        //workSheet.Cells[rowIndex, 5].Value = items.L_CDNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.Pages;
                     
                        workSheet.Cells[rowIndex, 6].Value = items.CDNumber;
                        workSheet.Cells[rowIndex, 7].Value = items.DateOfRegistration;
                        workSheet.Cells[rowIndex, 8].Value = items.IsScanned;
                       
                        workSheet.Cells[rowIndex, 9].Value = items.Remarks;
                        workSheet.Cells[rowIndex, 10].Value = items.ReceiptID;
                        workSheet.Cells[rowIndex, 11].Value = items.FirmType;

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


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;



                        rowIndex++;
                    }

                    using (ExcelRange Rng = workSheet.Cells[9, 1, (rowIndex - 1), 11])
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
        //
        private string CreateExcelForFirmCentralizationDetails(List<FirmCentralizationTableModel> result, string fileName, string excelHeader, string FromDate, string ToDate, string SearchBy)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
          
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Firm Centralization Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
             
                    workSheet.Cells[3, 1].Value = "From Date : " + FromDate;
                    workSheet.Cells[4, 1].Value = "To Date : " + ToDate;
                    workSheet.Cells[5, 1].Value = "Search By : " + SearchBy;
                  

                    workSheet.Cells[6, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[7, 1].Value = "Total Records : " + (result.Count());

                    workSheet.Cells[1, 1, 1, 11].Merge = true;
                    workSheet.Cells[2, 1, 2, 11].Merge = true;
                    workSheet.Cells[3, 1, 3, 11].Merge = true;
                    workSheet.Cells[4, 1, 4, 11].Merge = true;
                    workSheet.Cells[5, 1, 5, 11].Merge = true;
                    workSheet.Cells[6, 1, 6, 11].Merge = true;
                    workSheet.Cells[7, 1, 7, 11].Merge = true;
                

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
                    workSheet.Cells[9, 3].Value = "Local Firm Number";
                    workSheet.Cells[9, 4].Value = "Date of Registration";
                    workSheet.Cells[9, 5].Value = "Local CD Number";
                    workSheet.Cells[9, 6].Value = "Local ScanFileName";
         
                    workSheet.Cells[9, 7].Value = "Central Firm Number";
                    workSheet.Cells[9, 8].Value = "Central Date Of Registration";
                    workSheet.Cells[9, 9].Value = "Central CD Number";
                    workSheet.Cells[9, 10].Value = "Central ScanFileName";
   
                    workSheet.Cells[9, 11].Value = "UploadDateTime";

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
                    workSheet.Row(9).Style.Font.Bold = true;


                    foreach (var items in result)
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


                        workSheet.Cells[rowIndex, 1].Value = items.Sr_No;
                        workSheet.Cells[rowIndex, 2].Value = items.RegistrationID;
                        workSheet.Cells[rowIndex, 3].Value = items.L_FirmNumber;
                        workSheet.Cells[rowIndex, 4].Value = items.L_DateOfRegistration;
                        workSheet.Cells[rowIndex, 5].Value = items.L_CDNumber;
                        workSheet.Cells[rowIndex, 6].Value = items.L_ScanFileName;
    
                        workSheet.Cells[rowIndex, 7].Value = items.C_FirmNumber;
                        workSheet.Cells[rowIndex, 8].Value = items.C_DateOfRegistration;
                        workSheet.Cells[rowIndex, 9].Value = items.C_CDNumber;
                        workSheet.Cells[rowIndex, 10].Value = items.C_ScanFileName;
                        workSheet.Cells[rowIndex, 11].Value = items.UploadDateTime;

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
                  


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

               
                    
                        rowIndex++;
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