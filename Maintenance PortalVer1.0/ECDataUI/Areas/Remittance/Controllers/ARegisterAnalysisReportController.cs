#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ARegisterAnalysisReportController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 7 sep 2022
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :    Controller for ARegister Analysis Report.

*/
#endregion

using CustomModels.Models.Remittance.ARegisterAnalysisReport;

using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorization]
    public class ARegisterAnalysisReportController : Controller
    {
        // GET: Remittance/ARegisterAnalysisReport
        ServiceCaller caller = null;
        [HttpGet]
        [MenuHighlight]
        public ActionResult ARegisterAnalysisReportView()

        {
            try
            {
                caller = new ServiceCaller("ARegisterAnalysisReportAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                ARegisterAnalysisReportModel reqModel = caller.GetCall<ARegisterAnalysisReportModel>("ARegisterAnalysisReportView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving ARegiste rAnalysis Report View", URLToRedirect = "/Home/HomePage" });
            }
        }

        //[HttpPost]
        [HttpGet]
        public ActionResult GetARegisterAnalysisReportDetails(string FromDate,string SROfficeID,string ARegister,string AnyWhereECARegister,string KOS_ARegister,bool IsReceiptsSynchronized,bool IsARegisterGenerated)
        {
            try
            {
               
                caller = new ServiceCaller("ARegisterAnalysisReportAPIController");
                string fromDate = FromDate;
                string SROOfficeListID = SROfficeID;
                String errorMessage = String.Empty;
                string A_Register = ARegister;
                string AnyWhereEC_ARegister = AnyWhereECARegister;
                string KOSARegister = KOS_ARegister;
                string fileName = string.Empty;
                string createdExcelPath = string.Empty;
                CommonFunctions objCommon = new CommonFunctions();
          
                ARegisterAnalysisReportModel reqModel = new ARegisterAnalysisReportModel();
                reqModel.FromDate = fromDate;
                reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                if (A_Register == "0")
                {
                    reqModel.ARegister = true;
                }
                else if (AnyWhereEC_ARegister == "0")
                {
                    reqModel.AnyWhereECARegister = true;
                }
                else if (KOSARegister == "0")
                {
                    reqModel.KOSARegister = true;
                }
            
                ARegisterResultModel result = caller.PostCall<ARegisterAnalysisReportModel, ARegisterResultModel>("GetARegisterAnalysisReportDetails", reqModel, out errorMessage);

                List<RPTARegisterResult> ARegisterresult = result.ARegister_Result;

                List<RPTARegisterAnywhereECResult> AnyWhereECARegisterresult = result.AnyWhereEC_ARegisterDetailList;

                //int totalCount = result.ARegister_Result.Count;
                string excelHeader = string.Empty;
                string message = string.Empty;

                if (result.ARegister_Result != null)
                {
                    fileName = "ARegisterReportDetailsExcel_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("A Register Analysis Report Details" +" "+ fromDate);
                    createdExcelPath = CreateExcelForARegister(ARegisterresult, fileName, excelHeader, SROOfficeListID, IsReceiptsSynchronized, IsARegisterGenerated);




                }
                else if (result.AnyWhereEC_ARegisterDetailList != null)
                {
                    fileName = "AnyWhereEC_ARegisterDetailExcel_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                    excelHeader = string.Format("AnyWhereEC_ARegister Report Detail" +" "+ fromDate);
                    createdExcelPath = CreateExcelForAnyWhereEC_ARegister(AnyWhereECARegisterresult, fileName, excelHeader, SROOfficeListID, IsReceiptsSynchronized, IsARegisterGenerated);
                }
                else if (result.KOS_ARegisterDetailList != null)
                {
                    List<AregisterKOSDetailModel> aregisterKOSDetailModels = result.KOS_ARegisterDetailList;
                    fileName = "KOS_ARegisterDetailExcel_" + "_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + "_" + ".xlsx";
                    excelHeader = string.Format("KOS ARegister Report Detail" + " " + fromDate);
                    createdExcelPath = CreateExcelForKOS_ARegister(aregisterKOSDetailModels, fileName, excelHeader, SROOfficeListID, IsReceiptsSynchronized, IsARegisterGenerated);

                }
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);


                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }


            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting A Register Analysis Report details." });
            }

        }
        //



        private string CreateExcelForARegister(List<RPTARegisterResult> result, string fileName, string excelHeader, string SROOfficeListID,bool IsReceiptsSynchronized,bool IsARegisterGenerated)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add(" ARegister Report Detail");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    //workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    //workSheet.Cells[3, 1].Value = "SROCODE : " + SROOfficeListID;
                    workSheet.Cells[2, 1].Value = "SROCODE : " + SROOfficeListID;

                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());
                    workSheet.Cells[5, 1].Value = "IsReceiptsSynchronized : " + IsReceiptsSynchronized;
                    workSheet.Cells[6, 1].Value = "IsARegisterGenerated : " + IsARegisterGenerated;
                    //workSheet.Cells[1, 1, 1, 12].Merge = true;
                    //workSheet.Cells[2, 1, 2, 12].Merge = true;
                    //workSheet.Cells[3, 1, 3, 12].Merge = true;
                    //workSheet.Cells[4, 1, 4, 12].Merge = true;
                    //workSheet.Cells[5, 1, 5, 12].Merge = true;
                    workSheet.Cells[1, 1, 1, 28].Merge = true;
                    workSheet.Cells[2, 1, 2, 28].Merge = true;
                    workSheet.Cells[3, 1, 3, 28].Merge = true;
                    workSheet.Cells[4, 1, 4, 28].Merge = true;
                    workSheet.Cells[5, 1, 5, 28].Merge = true;
                    workSheet.Cells[6, 1, 6, 28].Merge = true;
                    // workSheet.Cells[5, 10, 5, 28].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(8).Style.WrapText = true;
                    workSheet.Column(14).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 35;
                    workSheet.Column(7).Width = 35;
                    workSheet.Column(8).Width = 35;
                    workSheet.Column(9).Width = 35;
                    workSheet.Column(10).Width = 35;
                    workSheet.Column(11).Width = 35;
                    workSheet.Column(12).Width = 35;
                    workSheet.Column(13).Width = 35;
                    workSheet.Column(14).Width = 35;
                    workSheet.Column(15).Width = 35;
                    workSheet.Column(16).Width = 35;
                    workSheet.Column(17).Width = 35;
                    workSheet.Column(18).Width = 35;
                    workSheet.Column(19).Width = 35;
                    workSheet.Column(20).Width = 35;
                    workSheet.Column(21).Width = 35;
                    workSheet.Column(22).Width = 35;
                    workSheet.Column(23).Width = 35;
                    workSheet.Column(24).Width = 35;
                    workSheet.Column(25).Width = 35;
                    workSheet.Column(26).Width = 35;
                    workSheet.Column(27).Width = 35;
                    workSheet.Column(28).Width = 35;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "Presenter Datetime";
                    workSheet.Cells[7, 3].Value = "Presenter Name";
                    workSheet.Cells[7, 4].Value = "StampArticleName";
                    workSheet.Cells[7, 5].Value = "Consideration";
                    workSheet.Cells[7, 6].Value = "StampDuty_Cash (a)";
                    workSheet.Cells[7, 7].Value = "StampDuty_Others (b)";
                    workSheet.Cells[7, 8].Value = "Total Stampduty (a+b)";
                    workSheet.Cells[7, 9].Value = "GovtDuty (1)";
                    workSheet.Cells[7, 10].Value = "Infrastructure (2)";
                    workSheet.Cells[7, 11].Value = "Muncipal (3)";
                    workSheet.Cells[7, 12].Value = "Corporation (4)";
                    workSheet.Cells[7, 13].Value = "TalukBoard (5)";
                    workSheet.Cells[7, 14].Value = "Total (1+2+3+4+5)";
                    workSheet.Cells[7, 15].Value = "DocumentNumber";
                    workSheet.Cells[7, 16].Value = "BookID";

                    workSheet.Cells[7, 17].Value = "VolumeName";
                    workSheet.Cells[7, 18].Value = "CompletionDate";
                    workSheet.Cells[7, 19].Value = "ReturnDate";
                    workSheet.Cells[7, 20].Value = "RegistrationFees (1)";
                    workSheet.Cells[7, 21].Value = "Deficient_RegistrationFees (2)";
                    workSheet.Cells[7, 22].Value = "Total (1+2) ";
                    workSheet.Cells[7, 23].Value = "CopyFees (3)";
                    workSheet.Cells[7, 24].Value = "Reg_Copy_Fees (1+2+3)";
                    workSheet.Cells[7, 25].Value = "MutationFee";
                    workSheet.Cells[7, 26].Value = "OtherFees";
                    workSheet.Cells[7, 27].Value = "Marriage Fees";
                    workSheet.Cells[7, 28].Value = "Remarks";
                    //workSheet.Cells[7, 27].Value = "StampArticleName";




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
                        workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 14].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 15].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 16].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 17].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 18].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 19].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 20].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 21].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 22].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 23].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 24].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 25].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 26].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 27].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 10].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 11].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 12].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 13].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 14].Style.Numberformat.Format = "0.00";
                        
                        workSheet.Cells[rowIndex, 20].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 21].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 22].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 23].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 24].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 25].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 26].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 27].Style.Numberformat.Format = "0.00";
                        
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.PresentDateTime.ToString();
                        workSheet.Cells[rowIndex, 3].Value = items.PresenterName;
                        workSheet.Cells[rowIndex, 4].Value = items.StampArticleName;
                        workSheet.Cells[rowIndex, 5].Value = items.Consideration;
                        workSheet.Cells[rowIndex, 6].Value = items.StampDuty_Cash;
                        workSheet.Cells[rowIndex, 7].Value = items.StampDuty_Others;
                        //
                        workSheet.Cells[rowIndex, 8].Value = items.StampDuty_Cash + items.StampDuty_Others;
                        workSheet.Cells[rowIndex, 9].Value = items.GovtDuty;
                        workSheet.Cells[rowIndex, 10].Value = items.Infrastructure;
                        workSheet.Cells[rowIndex, 11].Value = items.Muncipal;
                        workSheet.Cells[rowIndex, 12].Value = items.Corporation;
                        workSheet.Cells[rowIndex, 13].Value = items.TalukBoard;
                        workSheet.Cells[rowIndex, 14].Value = items.GovtDuty + items.Infrastructure + items.Muncipal + items.Corporation + items.TalukBoard;
                        workSheet.Cells[rowIndex, 15].Value = items.DocumentNumber;
                        workSheet.Cells[rowIndex, 16].Value = items.BookID;
                        workSheet.Cells[rowIndex, 17].Value = items.VolumeName;
                        workSheet.Cells[rowIndex, 18].Value = items.CompletionDate.ToString();
                        workSheet.Cells[rowIndex, 19].Value = items.ReturnDate.ToString();
                        workSheet.Cells[rowIndex, 20].Value = items.RegistrationFees;
                        workSheet.Cells[rowIndex, 21].Value = items.Deficient_RegistrationFees;
                        workSheet.Cells[rowIndex, 22].Value = items.RegistrationFees+items.Deficient_RegistrationFees;
                        workSheet.Cells[rowIndex, 23].Value = items.CopyFees;
                        workSheet.Cells[rowIndex, 24].Value = items.RegistrationFees + items.Deficient_RegistrationFees+items.CopyFees;
                        workSheet.Cells[rowIndex, 25].Value = items.MutationFee;
                        workSheet.Cells[rowIndex, 26].Value = items.OtherFees;
                        workSheet.Cells[rowIndex, 27].Value = items.HinduMarriageFee;
                        workSheet.Cells[rowIndex, 28].Value = items.Remarks;
                        // workSheet.Cells[rowIndex, 28].Value = items.StampDuty_Others;




                        //

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 14].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 15].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 16].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 17].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 18].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 19].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //
                     
                        workSheet.Cells[rowIndex, 20].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 21].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 22].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 23].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 24].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 25].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 26].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 27].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 28].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //
                        //workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        rowIndex++;
                    
                    }
                    // workSheet.Row(rowIndex - 1).Style.Font.Bold = true;
                    //workSheet.Cells[(rowIndex - 1), 1].Value = "";
                    //workSheet.Cells[(rowIndex - 1), 3].Value = "";

                    //using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
                    //{
                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //}
                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

        private string CreateExcelForAnyWhereEC_ARegister(List<RPTARegisterAnywhereECResult> result, string fileName, string excelHeader, string SROOfficeListID, bool IsReceiptsSynchronized, bool IsARegisterGenerated)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("AnyWhereEC ARegister Report Detail");
                    workSheet.Cells.Style.Font.Size = 14;


                    workSheet.Cells[1, 1].Value = excelHeader;
                    //workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    //workSheet.Cells[3, 1].Value = "SROCODE : " + SROOfficeListID;
                    workSheet.Cells[2, 1].Value = "SROCODE : " + SROOfficeListID;

                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());
                    workSheet.Cells[5, 1].Value = "IsReceiptsSynchronized : " + IsReceiptsSynchronized;
                    workSheet.Cells[6, 1].Value = "IsARegisterGenerated : " + IsARegisterGenerated;
                 
                     workSheet.Cells[1, 1, 1, 8].Merge = true;
                    workSheet.Cells[2, 1, 2, 8].Merge = true;
                    workSheet.Cells[3, 1, 3, 8].Merge = true;
                    workSheet.Cells[4, 1, 4, 8].Merge = true;
                    workSheet.Cells[5, 1, 5, 8].Merge = true;
                    workSheet.Cells[6, 1, 6, 8].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 45;
                    workSheet.Column(7).Width = 35;
                    workSheet.Column(8).Width = 45;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "ReceiptDateTime";
                    workSheet.Cells[7, 3].Value = "PresenterName";
                    workSheet.Cells[7, 4].Value = "SRO Application Number";
                    workSheet.Cells[7, 5].Value = "Other Fees";
                    workSheet.Cells[7, 6].Value = "ExemptionDescription";
                    workSheet.Cells[7, 7].Value = "ReceiptID";
                    workSheet.Cells[7, 8].Value = "ReceiptNumber";


                    //workSheet.Cells[7, 27].Value = "StampArticleName";




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

                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.ReceiptDateTime.ToString();
                        workSheet.Cells[rowIndex, 3].Value = items.PresenterName;
                        workSheet.Cells[rowIndex, 4].Value = items.SROApplicationNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.OtherFees;
                        workSheet.Cells[rowIndex, 6].Value = items.ExemptionDescription;
                        workSheet.Cells[rowIndex, 7].Value = items.ReceiptID;
                        workSheet.Cells[rowIndex, 8].Value = items.ReceiptNumber;


                        //workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    // workSheet.Row(rowIndex - 1).Style.Font.Bold = true;
                    //workSheet.Cells[(rowIndex - 1), 1].Value = "";
                    //workSheet.Cells[(rowIndex - 1), 3].Value = "";

                    //using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
                    //{
                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //}
                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

        private string CreateExcelForKOS_ARegister(List<AregisterKOSDetailModel> result, string fileName, string excelHeader, string SROOfficeListID, bool IsReceiptsSynchronized, bool IsARegisterGenerated)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("KOS ARegister Report Detail");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "SROCODE : " + SROOfficeListID;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[4, 1].Value = "Total Records : " + (result.Count());
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "IsReceiptsSynchronized : " + IsReceiptsSynchronized;
                    workSheet.Cells[6, 1].Value = "IsARegisterGenerated : " + IsARegisterGenerated;

                    workSheet.Cells[1, 1, 1, 6].Merge = true;
                    workSheet.Cells[2, 1, 2, 6].Merge = true;
                    workSheet.Cells[3, 1, 3, 6].Merge = true;
                    workSheet.Cells[4, 1, 4, 6].Merge = true;
                    workSheet.Cells[5, 1, 5, 6].Merge = true;
                    workSheet.Cells[6, 1, 6, 6].Merge = true;


                    workSheet.Column(5).Style.WrapText = true;
                    //workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 45;
                    workSheet.Column(6).Width = 35;
                    workSheet.Column(7).Width = 35;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = " PresenterDatetime";
                    workSheet.Cells[7, 3].Value = "PresenterName";
                    workSheet.Cells[7, 4].Value = "CCStampDuty";
                    workSheet.Cells[7, 5].Value = "Document Number";//(ApplicationNumber)
                    workSheet.Cells[7, 6].Value = "Other Fees";


                    //workSheet.Cells[7, 27].Value = "StampArticleName";




                    foreach (var items in result)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format= "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.srNo;
                        workSheet.Cells[rowIndex, 2].Value = items.TransactionDateTime.ToString();
                        workSheet.Cells[rowIndex, 3].Value = items.PartyName;
                        workSheet.Cells[rowIndex, 4].Value = items.CCStampDuty;
                        workSheet.Cells[rowIndex, 5].Value = items.ApplicationNumber;
                        workSheet.Cells[rowIndex, 6].Value = items.TotalAmt;


                        //workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        //workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    // workSheet.Row(rowIndex - 1).Style.Font.Bold = true;
                    //workSheet.Cells[(rowIndex - 1), 1].Value = "";
                    //workSheet.Cells[(rowIndex - 1), 3].Value = "";

                    //using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 7])
                    //{
                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //}
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
        [HttpPost]
        public ActionResult ValidateSearchParameters(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("ARegisterAnalysisReportAPIController");
                string fromDate = formCollection["FromDate"];
                string SROOfficeListID = formCollection["SROfficeID"];
                String errorMessage = String.Empty;
                string ARegister = formCollection["ARegister"];
                string AnyWhereECARegister = formCollection["AnyWhereECARegister"];
                string KOSARegister = formCollection["KOS_ARegister"];
                string fileName = string.Empty;
                string createdExcelPath = string.Empty;
                CommonFunctions objCommon = new CommonFunctions();
                if (string.IsNullOrEmpty(SROOfficeListID))
                {
                    return Json(new { success = false, serverError = false, errorMessage = "Please Select SRO Name" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(fromDate))
                {
                    return Json(new { success = false, serverError = false, errorMessage = "Please Select Date" }, JsonRequestBehavior.AllowGet);
                }
                if(string.IsNullOrEmpty(ARegister) && string.IsNullOrEmpty(AnyWhereECARegister) && string.IsNullOrEmpty(KOSARegister))
                {
                    return Json(new { success = false, serverError = false, errorMessage = "Please Select A Register Type" }, JsonRequestBehavior.AllowGet);
                }
                ARegisterAnalysisReportModel reqModel = new ARegisterAnalysisReportModel();
                reqModel.FromDate = fromDate;
                reqModel.DateTime_Date = Convert.ToDateTime(fromDate);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                if (ARegister == "0")
                {
                    reqModel.ARegister = true;
                }
                else if (AnyWhereECARegister == "0")
                {
                    reqModel.AnyWhereECARegister = true;
                }
                else if (KOSARegister == "0")
                {
                    reqModel.KOSARegister = true;
                }
                //
                #region  Synchronization check

                ARegisterSynchcheckResultModel SynchcheckResult = caller.PostCall<ARegisterAnalysisReportModel, ARegisterSynchcheckResultModel>("GetSynchronizationCheckResult", reqModel, out errorMessage);

                if (SynchcheckResult.ResponseMessage != null && SynchcheckResult.ResponseStatus == false)
                {
                    return Json(new { success = false, errorMessage = SynchcheckResult.ResponseMessage, ReceiptsSynchronized = SynchcheckResult.ReceiptsSynchronized, ARegisterGenerated=SynchcheckResult.ARegisterGenerated }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                return Json(new { success = true, errorMessage = "" , ReceiptsSynchronized = SynchcheckResult.ReceiptsSynchronized, ARegisterGenerated = SynchcheckResult.ARegisterGenerated }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while Validating A Register Analysis Report details." });
            }
        }
    }
}

    
