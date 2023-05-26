using CustomModels.Models.MISReports.DigilockerStatistics;
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorization]
    public class DigilockerStatisticsController : Controller
    {

        ServiceCaller caller = null;
        [EventAuditLogFilter(Description = "Digilocker Statistics Report View")]
        [MenuHighlight]
        public ActionResult DigilockerStatisticsView()
        {
            try
            {
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DigilockerStatisticsAPIController");
                DigiLockerStatisticsViewModel reqModel = caller.GetCall<DigiLockerStatisticsViewModel>("DigilockerStatisticsView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting details from Digilocker Statistics View", URLToRedirect = "/Home/HomePage" });
            }
        }





        
        [HttpPost]
        [EventAuditLogFilter(Description = "Digilocker Statistics Report Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult DigilockerStatisticsReportDetails(FormCollection formCollection)
        {
            ServiceCaller caller = null;

            caller = new ServiceCaller("DigilockerStatisticsAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;


            try
            {
                string errorMsg = string.Empty;
                if (ModelState.IsValid)
                {
                    
                    CommonFunctions objCommon = new CommonFunctions();
                    String errorMessage = String.Empty;
                    string FromDate = formCollection["FromDate"];
                    string ToDate = formCollection["ToDate"];
                  

                    #region Server Site Date Validation
                    DigiLockerStatisticsViewModel chkreqModel = new DigiLockerStatisticsViewModel();
                    chkreqModel.FromDate = FromDate;
                    chkreqModel.ToDate = ToDate;


                    if (string.IsNullOrEmpty(FromDate))
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = "0",
                            errorMessage = "Please Enter From Date"

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
                            errorMessage = "Please Enter To Date"

                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }



                    DateTime chkFromDate = Convert.ToDateTime(FromDate);
                    DateTime chkToDate = Convert.ToDateTime(ToDate);

                    int value = DateTime.Compare(chkFromDate, chkToDate);

                    if (value > 0)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "To Date can not be less than From Date" });
                    }

                    #endregion


                    

                    DigiLockerStatisticsViewModel reqModel = new DigiLockerStatisticsViewModel();
                    reqModel.FromDate = FromDate;
                    reqModel.ToDate = ToDate;
                    
                    DigilockerStatisticsResponseModel ResModel = caller.PostCall<DigiLockerStatisticsViewModel, DigilockerStatisticsResponseModel>("DigiLockerStatisticsReportDetails", reqModel, out errorMessage);

                    
                    IEnumerable<DigilockerStatisticsDetailsModel> result = ResModel.DigilockerStatisticsDetailsList;
                    if (result == null)
                    {
                        return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Digilocker Statistics Details." });
                    }
                    int totalCount = ResModel.DigilockerStatisticsDetailsList.Count;


                    
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
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting Digilocker Statistics details.", URLToRedirect = "/Home/HomePage" });
            }
        }

        



        [EventAuditLogFilter(Description = "Export Digilocker Statistics Report Details to EXCEL")]
        public ActionResult DigilockerStatisticsReportToExcel(string FromDate, string ToDate)
        {
            try
            {
                caller = new ServiceCaller("DigilockerStatisticsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("DigilockerStatisticsReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
     
                
                DigiLockerStatisticsViewModel Model = new DigiLockerStatisticsViewModel();
                Model.FromDate = FromDate;
                Model.ToDate = ToDate;
                Model.startLen = 0;
                Model.totalNum = 10;
                Model.IsExcel = true;
                

                DigilockerStatisticsResponseModel ResModel = new DigilockerStatisticsResponseModel();


                ResModel = caller.PostCall<DigiLockerStatisticsViewModel, DigilockerStatisticsResponseModel>("DigiLockerStatisticsReportDetails", Model, out errorMessage);
                if (ResModel.DigilockerStatisticsDetailsList == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While DigiLocker Statistics Details..." }, JsonRequestBehavior.AllowGet);

                }


                string excelHeader = string.Format("Digilocker Usage Statistics ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader);

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "DigilockerStatisticsReport" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        
        private string CreateExcel(DigilockerStatisticsResponseModel ResModel, string fileName, string excelHeader)
        
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Digilocker Statistics Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[1, 1, 1, 6].Merge = true;
                    workSheet.Cells[2, 1, 2, 6].Merge = true;
                    workSheet.Cells[3, 1, 3, 6].Merge = true;
                    workSheet.Cells[4, 1, 4, 6].Merge = true;
                   // workSheet.Cells[5, 1, 5, 6].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 60;
                    workSheet.Column(3).Width = 50;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    //workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Column(2).Style.Font.Bold = true;

                    int rowIndex = 5;
                    
                    workSheet.Cells[5, 2].Value = "Total KOS Users";
                    workSheet.Cells[6, 2].Value = "Total KOS Users Linked To Digilocker";
                    workSheet.Cells[7, 2].Value = "Total Applications Submitted";
                    workSheet.Cells[8, 2].Value = "Total Applications Linked To Digilocker";
                    workSheet.Cells[9, 2].Value = " Total Certificates Issued";
                    workSheet.Cells[10, 2].Value = "Total Certificates Pushed To Digilocker";
                    workSheet.Cells[11, 2].Value = "Total Certificates Searched From Digilocker";

                    //Del
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Column(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Column(3).Style.Font.Name = "KNB-TTUmaEN";

                    
                    foreach (var items in ResModel.DigilockerStatisticsDetailsList)
                    {
                        
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalKOSUsers;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalKOSUsersLinkedToDigilocker;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalApplicationsSubmitted;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalApplicationsLinkedToDigilocker;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalCertificatesIssued;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalCertificatesPushedToDigilocker;
                        workSheet.Cells[rowIndex++, 3].Value = items.TotalCertificatesSearchedFromDigilocker;


                        workSheet.Column(2).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Column(3).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    }

                    using (ExcelRange Rng = workSheet.Cells[5, 2, 11, 3])
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
                    using (ExcelRange Rng = workSheet.Cells[5, 2, 11, 3])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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


        public static FileInfo GetFileInfo(string tempExcelFilePath)
        {
            var fi = new FileInfo(tempExcelFilePath);
            return fi;
        }

    }
}