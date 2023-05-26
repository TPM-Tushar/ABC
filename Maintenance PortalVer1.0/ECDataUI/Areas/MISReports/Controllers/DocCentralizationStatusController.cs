#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DocCentralizationStatusController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.DocCentralizationStatus;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class DocCentralizationStatusController : Controller
    {
        ServiceCaller caller = null;
        /// <summary>
        /// Documents Centralization Status
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "Document Centralization Status")]
        public ActionResult DocCentralizationStatus()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DocumentCentralizationStatus;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("DocCentralizationStatusAPIController");
                DocCentrStatusReqModel reqModel = caller.GetCall<DocCentrStatusReqModel>("DocCentralizationStatusView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Document Centralization Status", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get Documents Centralization Status Datatable
        /// </summary>
        /// <param name="ReqModel"></param>
        /// <returns>View</returns>
        [HttpPost]
        //[EventAuditLogFilter(Description = "Get Document Centralization Status Report Details")]
        public ActionResult LoadDocCentralizationStatusDataTable(DocCentrStatusReqModel ReqModel)
        {
            try
            {
                string errorMsg = string.Empty;
                if (ModelState.IsValid)
                {
                    caller = new ServiceCaller("DocCentralizationStatusAPIController");
                    TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                    caller.HttpClient.Timeout = objTimeSpan;
                    DocCentrStatusResModel responceModel = new DocCentrStatusResModel();

                    DateTime InputDate = Convert.ToDateTime(ReqModel.Date);
                    DateTime DateFromDataPresent = Convert.ToDateTime("14/09/2019");//Data is present after 14/09/2019 this date

                    if (DateTime.Compare(InputDate, DateFromDataPresent) < 0)
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Data is not present before 14-09-2019", URLToRedirect = "/MISReports/DocCentralizationStatus/DocCentralizationStatus" });

                    String ExcelDownloadBtn = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + ReqModel.SROID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                    //To get table records
                    responceModel = caller.PostCall<DocCentrStatusReqModel, DocCentrStatusResModel>("LoadDocCentralizationStatusDataTable", ReqModel);


                    if (responceModel != null)
                        responceModel.ExcelDownloadBtn = ExcelDownloadBtn;
                    else
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Document Centralization Status View", URLToRedirect = "/Home/HomePage" });

                    return PartialView(responceModel);
                }
                else
                {
                    errorMsg = ModelState.FormatErrorMessageInString();
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = errorMsg, URLToRedirect = "/MISReports/DocCentralizationStatus/DocCentralizationStatus" });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Document Centralization Status View", URLToRedirect = "/Home/HomePage" });
            }
        }

        #region Excel     
        /// <summary>
        /// Export Todays Documents Centralization Status To Excel
        /// </summary>
        /// <param name="SROID"></param>
        /// <param name="SROName"></param>
        /// <param name="Date"></param>
        /// <returns>EXCEL</returns>
        //[EventAuditLogFilter(Description = "Docment Centralization Status To Excel")]
        public ActionResult DocCentralizationStatusToExcel(string SROID, string SROName, string Date)
        {
            try
            {
                string fileName = string.Format("DocumentCentralizationStatusExcel.xlsx");
                DocCentrStatusResModel resModel = new DocCentrStatusResModel();
                CommonFunctions objCommon = new CommonFunctions();

                string errorMessage = string.Empty;
                DocCentrStatusReqModel reqModel = new DocCentrStatusReqModel
                {
                    SROID = Convert.ToInt32(SROID),
                    Date = Date
                };
                caller = new ServiceCaller("DocCentralizationStatusAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                resModel = caller.PostCall<DocCentrStatusReqModel, DocCentrStatusResModel>("LoadDocCentralizationStatusDataTable", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While fetching table data", JsonRequestBehavior.AllowGet });
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));

                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing...", JsonRequestBehavior.AllowGet });

                //}
                string excelHeader = string.Format("Document Centralization Status ( " + Date + " )");

                string createdExcelPath = CreateExcel(resModel, reqModel, fileName, excelHeader, SROName, Date);

                if (string.IsNullOrEmpty(createdExcelPath))
                {

                    return Json(new { success = false, errorMessage = "Error Occured While Processing...", JsonRequestBehavior.AllowGet });

                }

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "DocumentCentralizationStatus_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="resModel"></param>
        /// <param name="reqModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SROName"></param>
        /// <param name="Date"></param>
        /// <returns>EXCEL path</returns>
        private string CreateExcel(DocCentrStatusResModel resModel, DocCentrStatusReqModel reqModel, string fileName, string excelHeader, string SROName, string Date)
        {
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");

            BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                int rowIndex = 8;
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Document Centralization Status ( " + Date + " )");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now + "";
                    workSheet.Cells[3, 1].Value = "SRO : " + SROName;
                    workSheet.Cells[4, 1].Value = "Number of Records : " + resModel.DetailsList.Count;

                    workSheet.Cells[1, 1, 1, 10].Merge = true;
                    workSheet.Cells[2, 1, 2, 10].Merge = true;
                    workSheet.Cells[3, 1, 3, 10].Merge = true;
                    workSheet.Cells[4, 1, 4, 10].Merge = true;


                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[2, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[3, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[4, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 35;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 50;
                    workSheet.Column(5).Width = 50;
                    workSheet.Column(6).Width = 50;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;

                    workSheet.Cells[6, 1].Value = "S. No.";
                    workSheet.Cells[6, 2].Value = "SRO Code";
                    workSheet.Cells[6, 3].Value = "SRO Name";
                    workSheet.Cells[6, 4].Value = "Number of documents registered today and centralized today";
                    workSheet.Cells[6, 5].Value = "Number of documents registered previously and centralized today";
                    workSheet.Cells[6, 6].Value = "Last Document Centralized Date Time";

                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Cells[6, 4].Style.WrapText = true;
                    workSheet.Cells[6, 5].Style.WrapText = true;
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(6).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    foreach (var items in resModel.DetailsList)
                    {
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SROCode;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.DocsCentlzdToday;
                        workSheet.Cells[rowIndex, 5].Value = items.DocsRegdPreviouslyCrtlzdToday;
                        workSheet.Cells[rowIndex, 6].Value = items.LatestCentralizationDate;

                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        rowIndex++;
                    }

                    if (resModel.DetailsList.Count != 0)//disallowing total to get printed when total records are 0
                    {
                        workSheet.Cells[rowIndex, 1].Value = "";
                        workSheet.Cells[rowIndex, 2].Value = "";
                        workSheet.Cells[rowIndex, 3].Value = "Total";
                        workSheet.Cells[rowIndex, 4].Value = resModel.TotalDocsCentralized;
                        workSheet.Cells[rowIndex, 5].Value = resModel.TotalDocsRegdPreviously;
                        workSheet.Cells[rowIndex, 6].Value = "";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.Font.Bold = true;
                        workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 10])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex), 6])
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