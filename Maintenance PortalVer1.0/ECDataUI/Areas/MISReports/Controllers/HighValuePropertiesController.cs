#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   HighValuePropertiesController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.HighValueProperties;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using iTextSharp.text;
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

    public class HighValuePropertiesController : Controller
    {
        // GET: MISReports/HighValueProperties
        ServiceCaller caller = null;

        /// <summary>
        /// High Value Properties View
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "High Value Properties View")]
        public ActionResult HighValuePropertiesView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.HighValueProperties;

                //int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("HighValuePropertiesAPIController");
                HighValuePropertiesReqModel reqModel = caller.GetCall<HighValuePropertiesReqModel>("HighValuePropertiesView");
                //  reqModel.Stamp5Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving High Value Properties View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get High Value Property Details
        /// </summary>
        /// <param name="RagneID"></param>
        /// <param name="FinYearListID"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get High Value Property Details")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetHighValuePropertyDetails(int RagneID,int FinYearListID)
        {
            try
            {

                caller = new ServiceCaller("HighValuePropertiesAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                HighValuePropDetailsReqModel reqModel = new HighValuePropDetailsReqModel();

                if (RagneID < 0 || FinYearListID < 0)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving High Value Property Details View", URLToRedirect = "/Home/HomePage" });

                }
                reqModel.RangeID = RagneID;
                reqModel.FinYearListID = FinYearListID;
                HighValuePropDetailsResModel responceModel = new HighValuePropDetailsResModel();
                String PDFDownloadBtn = "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + RagneID.ToString() + "','" + FinYearListID.ToString() + "')><i style='padding-right:3%; 'class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + RagneID.ToString() + "','" + FinYearListID.ToString() + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                //To get table records
                
                responceModel = caller.PostCall<HighValuePropDetailsReqModel, HighValuePropDetailsResModel>("GetHighValuePropertyDetails", reqModel);
                if (responceModel != null)
                {
                    responceModel.ExcelDownloadBtn = ExcelDownloadBtn;
                    responceModel.PDFDownloadBtn = PDFDownloadBtn;
                }
                else
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving High Value Property Details View", URLToRedirect = "/Home/HomePage" });

                }
                return PartialView(responceModel);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving High Value Property Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        #region PDF
        /// <summary>
        /// Export High Value Prop To PDF
        /// </summary>
        /// <param name="RangeID"></param>
        /// <param name="FinYearListID"></param>
        /// <returns>returns pdf file</returns>
        [EventAuditLogFilter(Description = "Export High Value Prop To PDF")]
        public ActionResult ExportHighValuePropToPDF(string RangeID, string FinYearListID)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                caller = new ServiceCaller("HighValuePropertiesAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                HighValuePropDetailsReqModel reqModel = new HighValuePropDetailsReqModel
                {
                    RangeID = Convert.ToInt32(RangeID),
                    FinYearListID = Convert.ToInt32(FinYearListID)
                };
                string pdfHeader = string.Format("High Value Properties Details");

                HighValuePropDetailsResModel resModel = new HighValuePropDetailsResModel();
                reqModel.IsPdf = true;
                //To get records of Total Documents Registered Report
                resModel = caller.PostCall<HighValuePropDetailsReqModel, HighValuePropDetailsResModel>("GetHighValuePropertyDetails", reqModel);
                if (resModel.RangeList == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }


                string fileName = string.Format("TodaysTotalDocumentsRegisteredReportPDF.pdf");
                byte[] pdfBytes = CreatePDFFile(resModel, reqModel, pdfHeader, fileName);
                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create PDF File
        /// </summary>
        /// <param name="resModel"></param>
        /// <param name="reqModel"></param>
        /// <param name="pdfHeader"></param>
        /// <param name="fileName"></param>
        /// <returns>returns pdf byte array</returns>
        private byte[] CreatePDFFile(HighValuePropDetailsResModel resModel, HighValuePropDetailsReqModel reqModel,string pdfHeader, string fileName)
        {
            string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));
            string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
            BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            try
            {
                byte[] pdfBytes = null;


                using (MemoryStream ms = new MemoryStream())
                {
                    using (Document doc = new Document(PageSize.A4.Rotate(), 35, 10, 10, 25))
                    {


                        using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
                        {

                            //string TotalDocumentsRegistered = "Total Documents Registered";
                            //string TotalRegistrationfees = "Total Registration fees (in Rs.)";
                            //string TotalStampDuty1 = "Total Stamp Duty (in Rs.)";
                            //string Total = "Total (in Rs.)";

                            PdfPCell cell = null;

                            cell = new PdfPCell(new Phrase())
                            {
                                BackgroundColor = new BaseColor(226, 226, 226)
                            };

                            //string[] colSummary = { SerialNo, FinYear, SD, RF ,DC};
                            PdfPTable tableSummary = new PdfPTable(4)
                            {
                                WidthPercentage = 50
                            };
                            tableSummary.HeaderRows = 1;


                            tableSummary.SetWidths(new Single[] { 5, 5, 5, 5 });



                            //for (int i = 0; i < colSummary.Length; ++i)
                            //{
                            //    cell = new PdfPCell(new Phrase(colSummary[i]))
                            //    {
                            //        BackgroundColor = new BaseColor(204, 255, 255)
                            //    };
                            //    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            //    tableSummary.AddCell(cell);
                            //}

                            //PdfPCell cell1 = new PdfPCell(new Phrase(TableModel.TotalDocuments, tableContentFont))
                            //{
                            //    BackgroundColor = new BaseColor(204, 255, 255)
                            //};
                            //cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            //cell1.BackgroundColor = BaseColor.WHITE;
                            //PdfPCell cell2 = new PdfPCell(new Phrase(TableModel.TotalRegFee, tableContentFont))
                            //{
                            //    BackgroundColor = new BaseColor(204, 255, 255)
                            //};
                            //cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                            //cell2.BackgroundColor = BaseColor.WHITE;
                            //PdfPCell cell3 = new PdfPCell(new Phrase(TableModel.TotalStampDuty, tableContentFont))
                            //{
                            //    BackgroundColor = new BaseColor(204, 255, 255)
                            //};
                            //cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                            //cell3.BackgroundColor = BaseColor.WHITE;
                            //PdfPCell cell4 = new PdfPCell(new Phrase(TableModel.Total, tableContentFont))
                            //{
                            //    BackgroundColor = new BaseColor(204, 255, 255)
                            //};
                            //cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                            //cell4.BackgroundColor = BaseColor.WHITE;
                            //tableSummary.AddCell(cell1);
                            //tableSummary.AddCell(cell2);
                            //tableSummary.AddCell(cell3);
                            //tableSummary.AddCell(cell4);


                            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                            var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
                            //PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                            doc.Open();
                            //Paragraph addHeading = new Paragraph("Index II Report (Between " + reqModel.Stamp5Date + " and " + reqModel.ToDate_Str + " )", headerTextFont)
                            //{
                            //    Alignment = 1,
                            //};
                            Paragraph addSpace = new Paragraph("")
                            {
                                Alignment = 1
                            };
                            var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
                            var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));

                            var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
                            //var registrationDateChunk = new Chunk("Registration Date : ", blackListTextFont);

                            var SRONameChunk = new Chunk("SRO : ", blackListTextFont);
                            var DRONameChunk = new Chunk("DRO : ", blackListTextFont);


                            var ReportGeneration = new Chunk("This Report is Generated on: ", blackListTextFont);

                            var totalChunk = new Chunk("Total Records: ", blackListTextFont);
                            var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);

                            // var SroName = new Chunk(SROName, redListTextFont);
                            var descriptionChunk = new Chunk(DateTime.Now.ToString(), redListTextFont);
                            //var TotalStampDutyChunk = new Chunk(TableModel.TotalStampDuty, redListTextFont);
                            //var TotalDocsGegdChunk = new Chunk(Convert.ToString(TableModel.TotalDocuments), redListTextFont);
                            //var TotalRegFeesChunk = new Chunk(TableModel.TotalRegFee, redListTextFont);
                            //var RegDescChunk = new Chunk(TableModel.RegistrationDate, redListTextFont);
                            //var SroChunk = new Chunk(reqModel.SroName, redListTextFont);
                            //var DroChunk = new Chunk(reqModel.DroName, redListTextFont);




                            // var ReportGenerationChunk = new Chunk(Convert.ToString(TableModel.TotalDocuments), redListTextFont);

                            var titlePhrase = new Phrase(titleChunk)
                                {
                                    descriptionChunk
                                };
                            //var RegistrationDatePhrase = new Phrase(registrationDateChunk)
                            //    {
                            //        RegDescChunk
                            //    };
                            //var SRONamePhrase = new Phrase(SRONameChunk)
                            //    {
                            //        SroChunk
                            //    };
                            //var DRONamePhrase = new Phrase(DRONameChunk)
                            //    {
                            //        DroChunk
                            //    };

                            //doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(titlePhrase);
                            doc.Add(addSpace);
                            //doc.Add(RegistrationDatePhrase);
                            doc.Add(addSpace);
                            //doc.Add(DRONamePhrase);
                            doc.Add(addSpace);
                            //doc.Add(SRONamePhrase);


                            //Table Data
                            doc.Add(tableSummary);
                            doc.Add(HighValuePropTable(resModel));
                            doc.Close();
                        }
                        pdfBytes = AddpageNumber(ms.ToArray());

                    }
                }
                return pdfBytes;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// High Value Prop Table
        /// </summary>
        /// <param name="resModel"></param>
        /// <returns>returns pdf table</returns>
        private PdfPTable HighValuePropTable(HighValuePropDetailsResModel resModel)
        {
            string SRNo = "Serial No";
            string Finyear = "Financial Year";
            string SD = "Stamp Duty(in Rs.)";
            string RegistrationFees = "Registration Fees(in Rs.)";
            string DocumentsCollected = "Documents Collected";


            try
            {

                string[] col = { SRNo, Finyear, SD, RegistrationFees, DocumentsCollected};
                PdfPTable table = new PdfPTable(5)
                {
                    WidthPercentage = 100
                };
                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);

                table.HeaderRows = 1;
                table.SetWidths(new Single[] { 5, 5, 5, 5, 5});
                table.SpacingBefore = 25f;
                PdfPCell cell = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;


                for (int i = 0; i < col.Length; ++i)
                {
                    cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }
                foreach (var items in resModel.RangeList)
                {


                    cell1 = new PdfPCell(new Phrase(items.SerialNo))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell2 = new PdfPCell(new Phrase(items.FinYear, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(Convert.ToString(items.SD), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(Convert.ToString(items.RF), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell4.BackgroundColor = BaseColor.WHITE;

                    cell5 = new PdfPCell(new Phrase(Convert.ToString(items.DC), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.BackgroundColor = BaseColor.WHITE;

                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;


                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                }


                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //To add paging to PDF of Total Documents Registered Report
        /// <summary>
        /// Add page Number
        /// </summary>
        /// <param name="inputArray"></param>
        /// <returns>returns pdf byte array with page number</returns>
        public byte[] AddpageNumber(byte[] inputArray)
        {
            byte[] pdfBytes = null;
            CommonFunctions objCommon = new CommonFunctions();
            iTextSharp.text.Font fntrow = objCommon.DefineNormaFont("Times New Roman", 12);

            using (MemoryStream stream = new MemoryStream())
            {

                PdfReader reader = new PdfReader(inputArray);
                using (PdfStamper stamper = new PdfStamper(reader, stream))
                {
                    int pages = reader.NumberOfPages;
                    for (int i = 1; i <= pages; i++)
                    {
                        ColumnText.ShowTextAligned(stamper.GetUnderContent(i), Element.ALIGN_MIDDLE, new Phrase("Page " + i.ToString() + " of " + pages, fntrow), 420f, 16f, 0);
                    }
                }
                pdfBytes = stream.ToArray();
            }

            return pdfBytes;

        }
        #endregion

        #region Excel     
        /// <summary>
        /// Export Todays High Val Prop To Excel
        /// </summary>
        /// <param name="RangeID"></param>
        /// <param name="FinYearListID"></param>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Export Todays High Val Prop To Excel")]
        public ActionResult ExportTodaysHighValPropToExcel(string RangeID, string FinYearListID,string MaxDate)
        {
            try
            {
                string fileName = string.Format("HighValuePropertiesExcel.xlsx");
                HighValuePropDetailsResModel resModel = new HighValuePropDetailsResModel();
                CommonFunctions objCommon = new CommonFunctions();

                string errorMessage = string.Empty;
                HighValuePropDetailsReqModel reqModel = new HighValuePropDetailsReqModel
                {
                    RangeID = Convert.ToInt32(RangeID),
                    FinYearListID = Convert.ToInt32(FinYearListID)
                };

                reqModel.IsExcel = true;

                caller = new ServiceCaller("HighValuePropertiesAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                resModel = caller.PostCall<HighValuePropDetailsReqModel, HighValuePropDetailsResModel>("GetHighValuePropertyDetails", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing...", JsonRequestBehavior.AllowGet });


                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));

                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing...", JsonRequestBehavior.AllowGet });

                //}


                string excelHeader = string.Format("High Value Properties ("+ resModel .FinancialYear+ ")");

                string createdExcelPath = CreateExcel(resModel, reqModel, fileName, excelHeader, MaxDate);

                if (string.IsNullOrEmpty(createdExcelPath))
                {

                    return Json(new { success = false, errorMessage = "Error Occured While Processing...",JsonRequestBehavior.AllowGet });

                }

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "HighValuePropertiesExcel_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <returns></returns>
        private string CreateExcel(HighValuePropDetailsResModel resModel, HighValuePropDetailsReqModel reqModel, string fileName, string excelHeader,string MaxDate)
        {
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");

            BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                int rowIndex = 5;
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {


                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("High Value Properties ("+resModel.FinancialYear+")");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now+ "                                                                                                                                                                      Note : This report is based on pre processed data considered upto "+MaxDate;
                    workSheet.Cells[1, 1, 1, 10].Merge = true;
                    workSheet.Cells[2, 1, 2, 10].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[2, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 35;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 35;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 50;
                    workSheet.Column(7).Width = 50;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Cells[4, 1].Value = "Serial No";
                    workSheet.Cells[4, 2].Value = "Financial Year";
                    workSheet.Cells[4, 3].Value = "Month";
                    workSheet.Cells[4, 4].Value = "Documents Registered";
                    workSheet.Cells[4, 5].Value = "Stamp Duty ( in Rs. )";
                    workSheet.Cells[4, 6].Value = "Registration Fee ( in Rs. )";
                    workSheet.Cells[4, 7].Value = "Total";

                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    foreach (var items in resModel.RangeList)
                    {
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.FinYear;
                        workSheet.Cells[rowIndex, 3].Value = items.MonthName;
                        workSheet.Cells[rowIndex, 4].Value = items.DC;
                        workSheet.Cells[rowIndex, 5].Value = items.SD;
                        workSheet.Cells[rowIndex, 6].Value = items.RF;
                        workSheet.Cells[rowIndex, 7].Value = items.Total;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;
                    }

                    workSheet.Cells[rowIndex-1, 1].Value = "";

                    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(rowIndex-1).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[4, 1, (rowIndex - 1), 7])
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