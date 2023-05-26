#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ScannedFileUploadStatusReportController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion
using CustomModels.Models.MISReports.ScannedFileUploadStatusReport;
using CustomModels.Models.MISReports.TodaysDocumentsRegistered;
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
    public class ScannedFileUploadStatusReportController : Controller
    {
        ServiceCaller caller = null;
        /// <summary>
        /// Scanned File Upload Status Report
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "Scanned File Upload Status Report View")]
        public ActionResult ScannedFileUploadStatusReportView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ScannedFileUploadStatus;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("ScannedFileUploadStatusReportAPIController");
                ScannedFileUploadStatusRptReqModel reqModel = caller.GetCall<ScannedFileUploadStatusRptReqModel>("GetScannedFileUploadStatusDetails", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Total documents registered and Revenue collected View", URLToRedirect = "/Home/HomePage" });
            }
        }
        /// <summary>
        /// Get Scanned File Upload Status Report Datatable
        /// </summary>
        /// <param name="ReqModel"></param>
        /// <returns>View</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Scanned File Upload Status Datatable")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadScannedFileUploadStatusReportTable(ScannedFileUploadStatusRptReqModel reqModel)
        {
            try
            {
                string errorMsg = string.Empty;
                if (ModelState.IsValid)
                {
                    caller = new ServiceCaller("ScannedFileUploadStatusReportAPIController");
                    TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                    caller.HttpClient.Timeout = objTimeSpan;
                    String PDFDownloadBtn = "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + reqModel.DROfficeID.ToString() + "','" + reqModel.SROfficeID.ToString() + "','" + reqModel.OfficeType +  "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                    String ExcelDownloadBtn = "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + reqModel.DROfficeID.ToString() + "','" + reqModel.SROfficeID.ToString() + "','" + reqModel.OfficeType + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                    //To get table records
                    ScannedFileUploadStatusRptResModel resModel = caller.PostCall<ScannedFileUploadStatusRptReqModel, ScannedFileUploadStatusRptResModel>("LoadScannedFileUploadStatusTable", reqModel);
                    resModel.PDFDownloadBtn = PDFDownloadBtn;
                    resModel.EXCELDownloadBtn = ExcelDownloadBtn;
                    return PartialView(resModel);
                }
                else
                {
                    errorMsg = ModelState.FormatErrorMessageInString();
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = errorMsg, URLToRedirect = "/MISReports/ScannedFileUploadStatusReport/ScannedFileUploadStatusReportView" });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting Todays Documents Registered details.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get SRO Office List By District ID
        /// </summary>
        /// <param name = "DistrictID" ></ param >
        /// < returns > returns SRO Office list</returns>
        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
                {
            try
            {
                string errormessage = string.Empty;
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictID", new { DistrictID = DistrictID }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting SRO List." }, JsonRequestBehavior.AllowGet);
            }

        }

        #region PDF
        /// <summary>
        /// Export Scanned File Upload Status Report
        /// </summary>
        /// <param name="DistrictCode"></param>
        /// <param name="SROCode"></param>
        /// <param name="DistrictText"></param>
        /// <param name="SROText"></param>
        /// <returns>returns pdf file</returns>
        [EventAuditLogFilter(Description = "Export Scanned File Upload Status Report To PDF")]
        [HttpGet]
        public ActionResult ExportScannedFileUploadStatusReportToPDF(String DistrictCode, String SROCode, String DistrictText, String SROText)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                caller = new ServiceCaller("ScannedFileUploadStatusReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //callerCommon = new ServiceCaller("CommonsApiController");
                //string SROName, DROName;
                ScannedFileUploadStatusRptReqModel ReqModel = new ScannedFileUploadStatusRptReqModel();
                //TodaysDocumentsRegisteredReqModel model = new TodaysDocumentsRegisteredReqModel
                //{
                //    SROfficeID = Convert.ToInt32(SROOfficeListID),
                //    DROfficeID = Convert.ToInt32(DROOfficeListID)

                //};

                //model.Stamp5DateTime = Convert.ToDateTime(Date);
                //model.Stamp5Date = Date;
                //model.ToDate = Convert.ToDateTime(ToDate);
                //model.ToDate_Str = ToDate;

                //if (DROOfficeListID == "0" && SROOfficeListID == "0")
                //{
                //    model.SroName = "All";
                //    model.DroName = "All";

                //}
                //if (DROOfficeListID != "0" && SROOfficeListID == "0")
                //{
                //    DROName = callerCommon.GetCall<string>("GetDroName", new { DistrictID = DROOfficeListID });
                //    model.SroName = "All";
                //    model.DroName = DROName;

                //}
                //if (DROOfficeListID != "0" && SROOfficeListID != "0")
                //{
                //    SROName = callerCommon.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                //    DROName = callerCommon.GetCall<string>("GetDroName", new { DistrictID = DROOfficeListID });
                //    model.SroName = SROName;
                //    model.DroName = DROName;

                //}
                ReqModel.SROfficeID = Convert.ToInt32(SROCode);
                ReqModel.DROfficeID = Convert.ToInt32(DistrictCode);
                ScannedFileUploadStatusRptResModel ScannedFileResModel = new ScannedFileUploadStatusRptResModel();
                //To get records of Total Documents Registered Report
                //ScannedFileResModel = caller.GetCall<ScannedFileUploadStatusRptResModel>("GetScannedFileUploadStatusDetails");
                ScannedFileResModel = caller.PostCall<ScannedFileUploadStatusRptReqModel, ScannedFileUploadStatusRptResModel>("LoadScannedFileUploadStatusTable", ReqModel);

                if (ScannedFileResModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Fetching Scanned File Upload Status report..." });
                }
                //objListItemsToBeExported.RegistrationDate = Date;
                string fileName = string.Format("ScannedFileUploadStatusReport.pdf");
                byte[] pdfBytes = CreatePDFFile(ScannedFileResModel.ScannedFileList, fileName,DistrictText,SROText);
                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ScannedFileUploadStatusReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
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
        /// <param name="ScannedFilesRptList"></param>
        /// <param name="fileName"></param>
        /// <param name="SROText"></param>
        /// <returns>returns pdf byte array</returns>
        private byte[] CreatePDFFile(List<ScannedFileUploadStatusDetailsModel> ScannedFilesRptList,  string fileName, String DistrictText ,String SROText)
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
                            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                            var headerTextFont = FontFactory.GetFont("KNB-TTUmaEN", 15, new BaseColor(0, 128, 255));
                            //PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                            doc.Open();
                            Paragraph addHeading = new Paragraph("Scanned File Upload Status Report", headerTextFont)
                            {
                                Alignment = 1,
                            };
                            Paragraph addSpace = new Paragraph("")
                            {
                                Alignment = 1
                            };
                            var blackListTextFont = FontFactory.GetFont("KNB-TTUmaEN", 12, new BaseColor(0, 0, 0));
                            var redListTextFont = FontFactory.GetFont("KNB-TTUmaEN", 12, new BaseColor(94, 154, 214));
                            var FontItalic = FontFactory.GetFont("KNB-TTUmaEN", 10, 2, new BaseColor(94, 94, 94));

                            var titleChunk = new Chunk("          Print Date Time : ", blackListTextFont);
                            var District = new Chunk("          District : ", blackListTextFont);
                            var SRO = new Chunk("          SRO : ", blackListTextFont);
                            var TotalRecords = new Chunk("          Total Records : ", blackListTextFont);

                            var TotalRecordChunk = new Chunk(ScannedFilesRptList.Count + "    ", redListTextFont);
                            var descriptionChunk = new Chunk(DateTime.Now.ToString() + "    ", redListTextFont);
                            var DistrictChunk = new Chunk(DistrictText + "    ", redListTextFont);
                            var SROChunk = new Chunk(SROText + "    ", redListTextFont);
                            var titlePhrase = new Phrase(titleChunk)
                                {
                                    descriptionChunk
                                };
                            var DistrictPhrase = new Phrase(District)
                                {
                                    DistrictChunk
                                };
                            var SROPhrase = new Phrase(SRO)
                                {
                                    SROChunk
                                };
                            var TotalRecordsPhrase = new Phrase(TotalRecords)
                                {
                                    TotalRecordChunk
                                };
                            Paragraph AddLine = new Paragraph("  ");
                            Paragraph NotePara = new Paragraph("Note : This report is based on pre processed data considered upto : ", FontItalic);
                            NotePara.Alignment = Element.ALIGN_RIGHT;
                            //PdfPCell cellNote = new PdfPCell(new Phrase("This report is based on pre processed data considered upto : " + MaxDate));
                            //NotePara.Font.Style = FontItalic;
                            doc.Add(addHeading);
                            doc.Add(AddLine);


                            doc.Add(DistrictPhrase);
                            doc.Add(SROPhrase);
                            doc.Add(TotalRecordsPhrase);
                            doc.Add(titlePhrase);

                            //Table Data

                            doc.Add(ScannedFilesReportTable(ScannedFilesRptList));
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
        /// Todays Total Docs Reg Report Table
        /// </summary>
        /// <param name="ScannedFilesRptList"></param>
        /// <returns>returns pdf table</returns>
        private PdfPTable ScannedFilesReportTable(List<ScannedFileUploadStatusDetailsModel> ScannedFilesRptList)
        {

            string SRNo = "S.No.";
            string SubRegOffice = "Sub Registrar Office";
            string LastUploadDateTime = "Last Upload Date Time";
            //string FilePendingForDays = "File Pending For Days";
            
            try
            {
                string[] col = { SRNo, SubRegOffice, LastUploadDateTime};
                PdfPTable table = new PdfPTable(3)
                {
                    WidthPercentage = 100
                };
                table.WidthPercentage = 65;
                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);


                //string Italic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                //BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                //iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);


                table.HeaderRows = 1;
                table.SetWidths(new Single[] { 2, 5, 5});
                table.SpacingBefore = 5f;
                PdfPCell cell = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
               

                for (int i = 0; i < col.Length; ++i)
                {
                    cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }
                foreach (var items in ScannedFilesRptList)
                {


                    cell1 = new PdfPCell(new Phrase(items.SrNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell2 = new PdfPCell(new Phrase(items.SubRegistrarOffice, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(items.LastUploadDateTime.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.FilePendingForDays, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell4.BackgroundColor = BaseColor.WHITE;

                   

                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    //table.AddCell(cell4);

                }
                //for (int i = 0; i < col.Length; ++i)
                //{

                //    if (i == 0)
                //    {
                //        cell = new PdfPCell(new Phrase(""))
                //        {
                //            BackgroundColor = new BaseColor(226, 226, 226)
                //        };
                //        cell.HorizontalAlignment = Element.ALIGN_CENTER;

                //    }
                //    if (i == 1)
                //    {
                //        cell = new PdfPCell(new Phrase(""))
                //        {
                //            BackgroundColor = new BaseColor(226, 226, 226)
                //        };
                //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;

                //    }
                //    if (i == 2)
                //    {
                //        cell = new PdfPCell(new Phrase("Total"))
                //        {
                //            BackgroundColor = new BaseColor(226, 226, 226)
                //        };
                //        cell.HorizontalAlignment = Element.ALIGN_CENTER;

                //    }

                //    if (i == 3)
                //    {
                //        cell = new PdfPCell(new Phrase(TableModel.TotalDocuments.ToString()))
                //        {
                //            BackgroundColor = new BaseColor(226, 226, 226)
                //        };
                //        cell.HorizontalAlignment = Element.ALIGN_CENTER;

                //    }

                //    if (i == 4)
                //    {
                //        cell = new PdfPCell(new Phrase(TableModel.TotalRegFee.ToString("F")))
                //        {
                //            BackgroundColor = new BaseColor(226, 226, 226)
                //        };
                //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;

                //    }

                    


                //    table.AddCell(cell);
                //}

                return table;
            }
            catch (Exception ex)
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
        /// Export Scanned File Upload Status Report To Excel
        /// </summary>
        /// <param name="DistrictCode"></param>
        /// <param name="SROCode"></param>
        /// <param name="DistrictText"></param>
        /// <param name="SROText"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Export Scanned File Upload Status Report To Excel")]
        
        public ActionResult ExportScannedFileUploadStatusRptToExcel(String DistrictCode, String SROCode, String DistrictText, String SROText,String OfficeType, String DocTypeID, String DocTypeText)
        {
            try
            {
                string fileName = string.Format("ScannedFileUploadStatusReport.xlsx");
                ScannedFileUploadStatusRptResModel ScannedFileResModel = new ScannedFileUploadStatusRptResModel();
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                ScannedFileUploadStatusRptReqModel ReqModel = new ScannedFileUploadStatusRptReqModel();
                ReqModel.DROfficeID = Convert.ToInt32(DistrictCode);
                ReqModel.SROfficeID = Convert.ToInt32(SROCode);
                ReqModel.OfficeType = OfficeType;
                ReqModel.DocumentTypeID = Convert.ToInt32(DocTypeID); 
                

                caller = new ServiceCaller("ScannedFileUploadStatusReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                ScannedFileResModel = caller.PostCall<ScannedFileUploadStatusRptReqModel, ScannedFileUploadStatusRptResModel>("LoadScannedFileUploadStatusTable", ReqModel);

                if (ScannedFileResModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));

                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //    return Redirect("/ECDataAuditDetails/Error");

                string excelHeader = string.Format("Scanned File Upload Status Report");

                string createdExcelPath = string.Empty;
                if (OfficeType == "SR")
                {
                     createdExcelPath = CreateExcel(ScannedFileResModel, fileName, excelHeader, DistrictText, SROText, DocTypeText);

                }
                else if (OfficeType == "DR")
                {
                    createdExcelPath = CreateExcelForDR(ScannedFileResModel, fileName, excelHeader, DistrictText, SROText, DocTypeText);

                }
                else
                {
                    createdExcelPath = string.Empty;
                }

                //if (string.IsNullOrEmpty(createdExcelPath))
                //    return Redirect("/ECDataAuditDetails/Error");

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ScannedFileUploadStatusReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="ScannedFileResModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="DistrictText"></param>
        /// <param name="SROText"></param>
        /// <returns>returns excel file path</returns>
        private string CreateExcel(ScannedFileUploadStatusRptResModel ScannedFileResModel,string fileName, string excelHeader,String DistrictText,String SROText, String DocTypeText)
        {
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");

            BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
            
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                int rowIndex = 8;

                decimal fileSizeCounter = 0;    ////
                int noOfFilesCounter = 0;       ////

                if (KaveriSession.Current.RoleID == Convert.ToInt32(CommonEnum.RoleDetails.SR))
                {
                    //create a new ExcelPackage
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var workbook = package.Workbook;
                        var workSheet = package.Workbook.Worksheets.Add("Scanned File Upload Status Report");
                        workSheet.Cells.Style.Font.Size = 14;
                        workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[1, 1].Value = excelHeader;
                        workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                        //workSheet.Cells[2, 1].Value = "District : " + DistrictText;
                        workSheet.Cells[2, 1].Value = "SRO : " + SROText;

                        workSheet.Cells[4, 1].Value = "Registration Type : " + DocTypeText;     //

                        workSheet.Cells[5, 1].Value = "Total Records : " + ScannedFileResModel.ScannedFileList.Count;  //4, 1 => 5,1
                        workSheet.Cells[1, 1, 1, 10].Merge = true;
                        workSheet.Cells[2, 1, 2, 10].Merge = true;
                        workSheet.Cells[3, 1, 3, 10].Merge = true;
                        workSheet.Cells[4, 1, 4, 10].Merge = true;
                        workSheet.Cells[5, 1, 5, 10].Merge = true;

                        workSheet.Cells[1, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[2, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Column(1).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Column(2).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Column(3).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Column(4).Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Column(5).Style.Font.Name = "KNB-TTUmaEN";   //
                        workSheet.Column(6).Style.Font.Name = "KNB-TTUmaEN";   //

                        workSheet.Column(1).Width = 15;
                        workSheet.Column(2).Width = 35;
                        workSheet.Column(3).Width = 35;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 50;
                        workSheet.Column(7).Width = 50;
                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Row(2).Style.Font.Bold = true;
                        workSheet.Row(3).Style.Font.Bold = true;
                        workSheet.Row(4).Style.Font.Bold = true;
                        workSheet.Row(5).Style.Font.Bold = true;

                        workSheet.Row(7).Style.Font.Bold = true;  //

                        //workSheet.Cells[7, 2].Value = "Total Documents Registered ";
                        //workSheet.Cells[7, 3].Value = "Total Registration fees (in Rs.)";
                        //workSheet.Cells[7, 4].Value = "Total Stamp Duty (in Rs.)";
                        workSheet.Cells[6, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[6, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[6, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[6, 5].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[9, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[9, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[9, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[10, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[9, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[9, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[9, 7].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[7, 1].Value = "SR No.";                 //6 => 7 in all
                        workSheet.Cells[7, 2].Value = "Sub Registrar Office";
                        workSheet.Cells[7, 6].Value = "Last Upload Date Time";
                        //workSheet.Cells[6, 4].Value = "File Pending For Days";

                        workSheet.Cells[7, 3].Value = "Registration Module";    //
                        workSheet.Cells[7, 4].Value = "No of Files";            //
                        workSheet.Cells[7, 5].Value = "File Size (MB)";         //

                        //workSheet.Cells[] = 

                        workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        foreach (var items in ScannedFileResModel.ScannedFileList)
                        {
                            workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                            workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                            workSheet.Cells[rowIndex, 2].Value = items.SubRegistrarOffice;
                            workSheet.Cells[rowIndex, 6].Value = items.LastUploadDateTime;
                            //workSheet.Cells[rowIndex, 4].Value = items.FilePendingForDays;

                            workSheet.Cells[rowIndex, 3].Value = items.RegistrationModule;   //
                            workSheet.Cells[rowIndex, 4].Value = items.NoOfFiles;           //
                            workSheet.Cells[rowIndex, 5].Value = items.FileSize;            //

                            fileSizeCounter = Convert.ToDecimal(items.FileSize) + fileSizeCounter;   ////
                            noOfFilesCounter = Convert.ToInt32(items.NoOfFiles) + noOfFilesCounter;  ////

                            rowIndex++;
                        }

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Row(rowIndex).Style.Font.Bold = true;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //

                        workSheet.Cells[rowIndex, 3].Value = "Total";                ////
                        workSheet.Cells[rowIndex, 4].Value = noOfFilesCounter;      ////
                        workSheet.Cells[rowIndex, 5].Value = fileSizeCounter;      ////

                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.000";   // commented
                        //workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        //workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";

                        using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex), 6])  // 6 => 7   3 => 6  removed rowIndex - 1
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                        package.SaveAs(templateFile);
                    }
                }
                else
                {
                    //rowIndex = 8;
                    rowIndex = 9;
                    //create a new ExcelPackage
                    using (ExcelPackage package = new ExcelPackage())
                    {
                        var workbook = package.Workbook;
                        var workSheet = package.Workbook.Worksheets.Add("Scanned File Upload Status Report");
                        workSheet.Cells.Style.Font.Size = 14;
                        workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[1, 1].Value = excelHeader;
                        workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                        workSheet.Cells[2, 1].Value = "District : " + DistrictText;
                        workSheet.Cells[3, 1].Value = "SRO : " + SROText;
                        
                        workSheet.Cells[5, 1].Value = "Registration Type : " + DocTypeText;     //

                        workSheet.Cells[6, 1].Value = "Total Records : " + ScannedFileResModel.ScannedFileList.Count;  // 5,1 => 6,1

                        workSheet.Cells[1, 1, 1, 10].Merge = true;
                        workSheet.Cells[2, 1, 2, 10].Merge = true;
                        workSheet.Cells[3, 1, 3, 10].Merge = true;
                        workSheet.Cells[4, 1, 4, 10].Merge = true;
                        workSheet.Cells[5, 1, 5, 10].Merge = true;

                        workSheet.Cells[6, 1, 6, 10].Merge = true;      //

                        workSheet.Cells[1, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[2, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[3, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Column(1).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Column(2).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Column(3).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Column(4).Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Column(5).Style.Font.Name = "KNB-TTUmaEN";   //
                        workSheet.Column(6).Style.Font.Name = "KNB-TTUmaEN";   //

                        workSheet.Column(1).Width = 15;
                        workSheet.Column(2).Width = 35;
                        workSheet.Column(3).Width = 35;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 50;
                        workSheet.Column(7).Width = 50;
                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Row(2).Style.Font.Bold = true;
                        workSheet.Row(3).Style.Font.Bold = true;
                        workSheet.Row(4).Style.Font.Bold = true;
                        workSheet.Row(5).Style.Font.Bold = true;
                        workSheet.Row(6).Style.Font.Bold = true;    //
                        workSheet.Row(8).Style.Font.Bold = true;


                        //workSheet.Cells[7, 2].Value = "Total Documents Registered ";
                        //workSheet.Cells[7, 3].Value = "Total Registration fees (in Rs.)";
                        //workSheet.Cells[7, 4].Value = "Total Stamp Duty (in Rs.)";
                        workSheet.Cells[8, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[8, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[8, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[8, 5].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[8, 6].Style.Font.Name = "KNB-TTUmaEN"; //

                        workSheet.Cells[10, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[10, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[10, 3].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[10, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[10, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[10, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[10, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[8, 1].Value = "SR No.";
                        workSheet.Cells[8, 2].Value = "Sub Registrar Office";
                        workSheet.Cells[8, 6].Value = "Last Upload Date Time";

                        workSheet.Cells[8, 3].Value = "Registration Module";    //
                        workSheet.Cells[8, 4].Value = "No of Files";            //
                        workSheet.Cells[8, 5].Value = "File Size (MB)";              //

                        //workSheet.Cells[6, 4].Value = "File Pending For Days";
                        workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        foreach (var items in ScannedFileResModel.ScannedFileList)
                        {
                            workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                            workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            

                            workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                            workSheet.Cells[rowIndex, 2].Value = items.SubRegistrarOffice;
                            workSheet.Cells[rowIndex, 6].Value = items.LastUploadDateTime;
                            //workSheet.Cells[rowIndex, 4].Value = items.FilePendingForDays

                            workSheet.Cells[rowIndex, 3].Value = items.RegistrationModule;   //
                            workSheet.Cells[rowIndex, 4].Value = items.NoOfFiles;           //
                            workSheet.Cells[rowIndex, 5].Value = items.FileSize;            //

                            fileSizeCounter = Convert.ToDecimal(items.FileSize) + fileSizeCounter;   ////
                            noOfFilesCounter = Convert.ToInt32(items.NoOfFiles) + noOfFilesCounter;  ////

                            rowIndex++;
                        }

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Row(rowIndex).Style.Font.Bold = true;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //

                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //

                        workSheet.Cells[rowIndex, 3].Value = "Total";                ////
                        workSheet.Cells[rowIndex, 4].Value = noOfFilesCounter;      ////
                        workSheet.Cells[rowIndex, 5].Value = fileSizeCounter;      ////

                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.000";  //commented              
                        //workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        //workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        using (ExcelRange Rng = workSheet.Cells[8, 1, (rowIndex), 6])  //7 => 8 , 3 => 6  //removed rowIndex - 1
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                        package.SaveAs(templateFile);
                    }
                }
   
            }
            catch (Exception Ex)
            {
                throw;
            }

            return ExcelFilePath;
        }
        private string CreateExcelForDR(ScannedFileUploadStatusRptResModel ScannedFileResModel, string fileName, string excelHeader, String DistrictText, String SROText, String DocTypeText)
        {
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");

            BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                int rowIndex = 7;

                decimal fileSizeCounter = 0;    ////
                int noOfFilesCounter = 0;       ////

                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Scanned File Upload Status Report");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(4).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[2, 1].Value = "DRO : " + DistrictText;
                    //workSheet.Cells[3, 1].Value = "SRO : " + SROText;
                    
                    workSheet.Cells[4, 1].Value = "Total Records : " + ScannedFileResModel.ScannedFileList.Count;
                    workSheet.Cells[1, 1, 1, 10].Merge = true;
                    workSheet.Cells[2, 1, 2, 10].Merge = true;
                    workSheet.Cells[3, 1, 3, 10].Merge = true;
                    workSheet.Cells[4, 1, 4, 10].Merge = true;

                    //workSheet.Cells[5, 1, 5, 10].Merge = true;
                    workSheet.Cells[1, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[2, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[3, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Column(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Column(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Column(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Column(4).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Column(5).Style.Font.Name = "KNB-TTUmaEN";    //
                    workSheet.Column(6).Style.Font.Name = "KNB-TTUmaEN";    //

                    workSheet.Column(1).Width = 15;
                    workSheet.Column(2).Width = 35;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 50;
                    workSheet.Column(6).Width = 50;
                    workSheet.Column(7).Width = 50;
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;   //

                    //workSheet.Cells[7, 2].Value = "Total Documents Registered ";
                    //workSheet.Cells[7, 3].Value = "Total Registration fees (in Rs.)";
                    //workSheet.Cells[7, 4].Value = "Total Stamp Duty (in Rs.)";
                    workSheet.Cells[6, 2].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[6, 3].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[6, 4].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[6, 5].Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Cells[9, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[9, 2].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[9, 3].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[10, 4].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[9, 5].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[9, 6].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[9, 7].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[6, 1].Value = "SR No.";
                    workSheet.Cells[6, 2].Value = "District Registrar Office";
                    workSheet.Cells[6, 6].Value = "Last Upload Date Time";
                    //workSheet.Cells[6, 4].Value = "File Pending For Days";

                    workSheet.Cells[6, 3].Value = "Registration Module";    //
                    workSheet.Cells[6, 4].Value = "No of Files";            //
                    workSheet.Cells[6, 5].Value = "File Size (MB)";              //

                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    foreach (var items in ScannedFileResModel.ScannedFileList)
                    {
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SubRegistrarOffice;
                        workSheet.Cells[rowIndex, 6].Value = items.LastUploadDateTime;
                        //workSheet.Cells[rowIndex, 4].Value = items.FilePendingForDays;

                        workSheet.Cells[rowIndex, 3].Value = items.RegistrationModule;   //
                        workSheet.Cells[rowIndex, 4].Value = items.NoOfFiles;           //
                        workSheet.Cells[rowIndex, 5].Value = items.FileSize;            //

                        fileSizeCounter = Convert.ToDecimal(items.FileSize) + fileSizeCounter;   ////
                        noOfFilesCounter = Convert.ToInt32(items.NoOfFiles) + noOfFilesCounter;  ////

                        rowIndex++;
                    }

                    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //
                    workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //
                    workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   //

                    ////

                    workSheet.Cells[rowIndex, 3].Value = "Total";                    ////
                    workSheet.Cells[rowIndex, 4].Value = noOfFilesCounter;          ////
                    workSheet.Cells[rowIndex, 5].Value = fileSizeCounter;           ////

                    ////



                    workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.000";     //commented
                    //workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex), 6])    //removed rowIndex - 1
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
