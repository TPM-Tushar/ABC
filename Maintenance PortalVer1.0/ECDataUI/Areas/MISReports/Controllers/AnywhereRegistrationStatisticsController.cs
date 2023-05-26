#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   AnywhereRegistrationStatisticsController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion
using CustomModels.Models.MISReports.AnywhereRegistrationStatistics;
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
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class AnywhereRegistrationStatisticsController : Controller
    {
        ServiceCaller caller = null;

        /// <summary>
        /// Anywhere EC Log 
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "Anywhere Registration Statistics View")]
        public ActionResult AnywhereRegistrationStatistics()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.AnywhereRegistrationStatistics;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("AnywhereRegistrationStatisticsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                AnywhereRegStatViewModel reqModel = caller.GetCall<AnywhereRegStatViewModel>("AnywhereRegistrationStatisticsView", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Anywhere Registration Statistics View", URLToRedirect = "/Home/HomePage" });
                }
                return View(reqModel);
            }
            catch (Exception)
            {
                throw;

            }
        }

        /// <summary>
        /// Returns Anywhere Registration Statistics Table
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Anywhere Registration Statistics Table</returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Load Anywhere Registration Statistics Datatable")]
        public ActionResult LoadAnywhereRegStatTable(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects               
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string DistrictID = formCollection["DistrictID"];
                int DistrictId = Convert.ToInt32(DistrictID);
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");//Regx to not allow < >(special characters)
                System.Text.RegularExpressions.Regex regxDistrict = new Regex("^[0-9]*$");//Regx to not allow numbers other than natural numbers
                Match mtchDistrict = regxDistrict.Match(DistrictID);
                caller = new ServiceCaller("AnywhereRegistrationStatisticsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                AnywhereRegStatViewModel reqModel = new AnywhereRegStatViewModel();
                #endregion

                short OfficeID = KaveriSession.Current.OfficeID;
                //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });

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

                //if ((SroId == 0 && DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any District"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any SRO"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;

                //}
                if ((DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
                {
                   
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any District", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });

                }
                //}

                if (string.IsNullOrEmpty(FromDate)) 
                {
                    
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date required", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });

                }
                else if (string.IsNullOrEmpty(ToDate))
                {
                   
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "To Date required", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });

                }
                if (!mtchDistrict.Success)
                {
                   
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please enter valid District", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });

                }

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);

                if (!boolFrmDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid From Date", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });
                }
                else if (!boolToDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid To Date", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });
                }
                else if (frmDate > toDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date can not be larger than To Date", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });
                }
                reqModel.FromDate = FromDate;
                reqModel.ToDate = ToDate;
                reqModel.DistrictID = DistrictId;
                AnywhereRegStatResModel AnywhereStatResModel = caller.PostCall<AnywhereRegStatViewModel, AnywhereRegStatResModel>("GetAnywhereRegStatDetails", reqModel, out errorMessage);
            
                if (AnywhereStatResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Fetching Anywhere Registration Statistics DataTable", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });
                }

                return View(AnywhereStatResModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Fetching Anywhere Registration Statistics DataTable", URLToRedirect = "/MISReports/AnywhereRegistrationStatistics/AnywhereRegistrationStatistics" });
            }
        }
        
        #region PDF
        /// <summary>
        /// Export Todays Total Docs Reg Report To PDF
        /// </summary>
        /// <param name="SROOfficeListID"></param>
        /// <param name="DROOfficeListID"></param>
        /// <param name="Date"></param>
        /// <param name="ToDate"></param>
        /// <returns>returns pdf file</returns>
        //public ActionResult ExportAnywhereRegStatTableToPDF(string FromDate, string ToDate, string DistrictID, string txtDistrict)
        //{
        //    try
        //    {
        //        CommonFunctions objCommon = new CommonFunctions();
        //        string errorMessage = string.Empty;
        //        caller = new ServiceCaller("AnywhereRegistrationStatisticsAPIController");
        //        TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
        //        caller.HttpClient.Timeout = objTimeSpan;
        //        string DROName;

        //        AnywhereRegStatViewModel model = new AnywhereRegStatViewModel
        //        {
        //            DistrictID = Convert.ToInt32(DistrictID),
        //            FromDate = FromDate,
        //            ToDate = ToDate
        //        };
                
        //        AnywhereRegStatResModel ResModel = new AnywhereRegStatResModel();

        //        //To get records of Total Documents Registered Report
        //        ResModel = caller.PostCall<AnywhereRegStatViewModel, AnywhereRegStatResModel>("GetTodaysTotalDocumentsRegisteredDetails", model, out errorMessage);
        //        if (ResModel == null)
        //        {
        //            return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

        //        }

        //        string fileName = string.Format("TodaysTotalDocumentsRegisteredReportPDF.pdf");
        //        byte[] pdfBytes = AnywhereRegStatPDFFile(ResModel, fileName);
        //        return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "TodaysTotalDocumentsRegisteredReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
        //    }
        //}

        /// <summary>
        /// Create PDF File
        /// </summary>
        /// <param name="TableModel"></param>
        /// <param name="reqModel"></param>
        /// <param name="fileName"></param>
        /// <returns>returns pdf byte array</returns>
        //private byte[] AnywhereRegStatPDFFile(AnywhereRegStatResModel TableModel,string fileName)
        //{
        //    string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));
        //    string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
        //    string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
        //    BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

        //    iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);

        //    if (!Directory.Exists(folderPath))
        //    {
        //        Directory.CreateDirectory(folderPath);
        //    }
        //    string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //    try
        //    {
        //        byte[] pdfBytes = null;


        //        using (MemoryStream ms = new MemoryStream())
        //        {
        //            using (Document doc = new Document(PageSize.A4.Rotate(), 35, 10, 10, 25))
        //            {


        //                using (PdfWriter writer = PdfWriter.GetInstance(doc, ms))
        //                {

        //                    string TotalDocumentsRegistered = "Total Documents Registered";
        //                    string TotalRegistrationfees = "Total Registration fees (in Rs.)";
        //                    string TotalStampDuty1 = "Total Stamp Duty (in Rs.)";
        //                    string Total = "Total (in Rs.)";

        //                    PdfPCell cell = null;

        //                    cell = new PdfPCell(new Phrase())
        //                    {
        //                        BackgroundColor = new BaseColor(226, 226, 226)
        //                    };

        //                    string[] colSummary = { TotalDocumentsRegistered, TotalRegistrationfees, TotalStampDuty1, Total };
        //                    PdfPTable tableSummary = new PdfPTable(4)
        //                    {
        //                        WidthPercentage = 50
        //                    };
        //                    tableSummary.HeaderRows = 1;


        //                    tableSummary.SetWidths(new Single[] { 5, 5, 5, 5 });
        //                    tableSummary.SpacingBefore = 10f;

        //                    for (int i = 0; i < colSummary.Length; ++i)
        //                    {
        //                        cell = new PdfPCell(new Phrase(colSummary[i]))
        //                        {
        //                            BackgroundColor = new BaseColor(204, 255, 255)
        //                        };
        //                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                        tableSummary.AddCell(cell);
        //                    }
        //                    //tableSummary.AddCell(new Phrase(TableModel.TotalDocuments, tableContentFont));
        //                    //tableSummary.AddCell(new Phrase(TableModel.TotalRegFee, tableContentFont));
        //                    //tableSummary.AddCell(new Phrase(TableModel.TotalStampDuty, tableContentFont));
        //                    //tableSummary.AddCell(new Phrase(TableModel.Total, tableContentFont));
        //                    PdfPCell cell1 = new PdfPCell(new Phrase(TableModel.TotalDocuments.ToString(), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell1.BackgroundColor = BaseColor.WHITE;
        //                    PdfPCell cell2 = new PdfPCell(new Phrase(TableModel.TotalRegFee.ToString("F"), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell2.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell2.BackgroundColor = BaseColor.WHITE;
        //                    PdfPCell cell3 = new PdfPCell(new Phrase(TableModel.TotalStampDuty.ToString("F"), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell3.BackgroundColor = BaseColor.WHITE;
        //                    PdfPCell cell4 = new PdfPCell(new Phrase(TableModel.Total.ToString("F"), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell4.BackgroundColor = BaseColor.WHITE;
        //                    tableSummary.AddCell(cell1);
        //                    tableSummary.AddCell(cell2);
        //                    tableSummary.AddCell(cell3);
        //                    tableSummary.AddCell(cell4);



        //                    doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
        //                    var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
        //                    //PdfWriter writer = PdfWriter.GetInstance(doc, fs);
        //                    doc.Open();
        //                    Paragraph addHeading = new Paragraph("Total documents registered and Revenue collected (Between " + reqModel.Stamp5Date + " and " + reqModel.ToDate_Str + " )", headerTextFont)
        //                    {
        //                        Alignment = 1,
        //                    };
        //                    Paragraph addSpace = new Paragraph("")
        //                    {
        //                        Alignment = 1
        //                    };
        //                    var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
        //                    var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));
        //                    var FontItalic = FontFactory.GetFont("Arial", 10, 2, new BaseColor(94, 94, 94));

        //                    var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
        //                    //var registrationDateChunk = new Chunk("Registration Date : ", blackListTextFont);

        //                    var SRONameChunk = new Chunk("SRO : ", blackListTextFont);
        //                    var DRONameChunk = new Chunk("District : ", blackListTextFont);

        //                    var ReportGeneration = new Chunk("This Report is Generated on: ", blackListTextFont);

        //                    var totalChunk = new Chunk("Total Records: ", blackListTextFont);
        //                    var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);

        //                    // var SroName = new Chunk(SROName, redListTextFont);
        //                    var descriptionChunk = new Chunk(DateTime.Now.ToString(), redListTextFont);
        //                    var TotalStampDutyChunk = new Chunk(TableModel.TotalStampDuty.ToString(), redListTextFont);
        //                    var TotalDocsGegdChunk = new Chunk(Convert.ToString(TableModel.TotalDocuments), redListTextFont);
        //                    var TotalRegFeesChunk = new Chunk(TableModel.TotalRegFee.ToString(), redListTextFont);
        //                    var RegDescChunk = new Chunk(TableModel.RegistrationDate, redListTextFont);
        //                    var SroChunk = new Chunk(reqModel.SroName, redListTextFont);
        //                    var DroChunk = new Chunk(reqModel.DroName, redListTextFont);




        //                    var ReportGenerationChunk = new Chunk(Convert.ToString(TableModel.TotalDocuments), redListTextFont);

        //                    var titlePhrase = new Phrase(titleChunk)
        //                        {
        //                            descriptionChunk
        //                        };
        //                    //var RegistrationDatePhrase = new Phrase(registrationDateChunk)
        //                    //    {
        //                    //        RegDescChunk
        //                    //    };
        //                    var SRONamePhrase = new Phrase(SRONameChunk)
        //                        {
        //                            SroChunk
        //                        };
        //                    var DRONamePhrase = new Phrase(DRONameChunk)
        //                        {
        //                            DroChunk
        //                        };
        //                    Paragraph NotePara = new Paragraph("Note : This report is based on pre processed data considered upto : " + MaxDate, FontItalic);
        //                    NotePara.Alignment = Element.ALIGN_RIGHT;
        //                    //PdfPCell cellNote = new PdfPCell(new Phrase("This report is based on pre processed data considered upto : " + MaxDate));
        //                    //NotePara.Font.Style = FontItalic;
        //                    doc.Add(addHeading);
        //                    doc.Add(addSpace);
        //                    doc.Add(titlePhrase);
        //                    doc.Add(addSpace);
        //                    //doc.Add(RegistrationDatePhrase);
        //                    doc.Add(addSpace);
        //                    doc.Add(DRONamePhrase);
        //                    doc.Add(addSpace);
        //                    doc.Add(SRONamePhrase);
        //                    doc.Add(addSpace);
        //                    doc.Add(NotePara);
        //                    doc.Add(addSpace);

        //                    //Table Data
        //                    doc.Add(tableSummary);
        //                    doc.Add(TodaysTotalDocsRegReportTable(TableModel));
        //                    doc.Close();
        //                }
        //                pdfBytes = AddpageNumber(ms.ToArray());

        //            }
        //        }
        //        return pdfBytes;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

       
        //private PdfPTable AnywhereRegStatTable(TodaysTotalDocsRegDetailsTable TableModel)
        //{

        //    string SRNo = "Sr No.";
        //    string District = "District";
        //    string SROName = "SRO Name";
        //    string Documents = "Documents Registered";
        //    string RegistrationFees = "Registration Fees(in Rs.)";
        //    string StampDuty = "Stamp Duty(in Rs.)";
        //    string Total = "Total(in Rs.)";


        //    try
        //    {

        //        string[] col = { SRNo, District, SROName, Documents, RegistrationFees, StampDuty, Total };
        //        PdfPTable table = new PdfPTable(7)
        //        {
        //            WidthPercentage = 100
        //        };
        //        string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
        //        string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
        //        BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        //        iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);


        //        //string Italic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
        //        //BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        //        //iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);


        //        table.HeaderRows = 1;
        //        table.SetWidths(new Single[] { 5, 5, 5, 5, 6, 5, 5 });
        //        table.SpacingBefore = 25f;
        //        PdfPCell cell = null;
        //        PdfPCell cell1 = null;
        //        PdfPCell cell2 = null;
        //        PdfPCell cell3 = null;
        //        PdfPCell cell4 = null;
        //        PdfPCell cell5 = null;
        //        PdfPCell cell6 = null;
        //        PdfPCell cell7 = null;

        //        for (int i = 0; i < col.Length; ++i)
        //        {
        //            cell = new PdfPCell(new Phrase(col[i]))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //            table.AddCell(cell);
        //        }
        //        foreach (var items in TableModel.TodaysTotalDocsRegTableList)
        //        {


        //            cell1 = new PdfPCell(new Phrase(items.SRNo, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell1.BackgroundColor = BaseColor.WHITE;
        //            cell2 = new PdfPCell(new Phrase(items.SROName, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell2.BackgroundColor = BaseColor.WHITE;

        //            cell3 = new PdfPCell(new Phrase(items.Documents.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell3.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell3.BackgroundColor = BaseColor.WHITE;

        //            cell4 = new PdfPCell(new Phrase(items.RegistrationFee.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell4.BackgroundColor = BaseColor.WHITE;

        //            cell5 = new PdfPCell(new Phrase(items.StampDuty.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell5.BackgroundColor = BaseColor.WHITE;

        //            cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell6 = new PdfPCell(new Phrase(items.Total.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell6.BackgroundColor = BaseColor.WHITE;

        //            cell7 = new PdfPCell(new Phrase(items.District, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell7.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell7.BackgroundColor = BaseColor.WHITE;

        //            table.AddCell(cell1);
        //            table.AddCell(cell7);
        //            table.AddCell(cell2);
        //            table.AddCell(cell3);
        //            table.AddCell(cell4);
        //            table.AddCell(cell5);
        //            table.AddCell(cell6);
        //        }
        //        for (int i = 0; i < col.Length; ++i)
        //        {

        //            if (i == 0)
        //            {
        //                cell = new PdfPCell(new Phrase(""))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            }
        //            if (i == 1)
        //            {
        //                cell = new PdfPCell(new Phrase(""))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            }
        //            if (i == 2)
        //            {
        //                cell = new PdfPCell(new Phrase("Total"))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            }

        //            if (i == 3)
        //            {
        //                cell = new PdfPCell(new Phrase(TableModel.TotalDocuments.ToString()))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                cell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            }

        //            if (i == 4)
        //            {
        //                cell = new PdfPCell(new Phrase(TableModel.TotalRegFee.ToString("F")))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            }

        //            if (i == 5)
        //            {
        //                cell = new PdfPCell(new Phrase(TableModel.TotalStampDuty.ToString("F")))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;


        //            }
        //            if (i == 5)
        //            {
        //                cell = new PdfPCell(new Phrase(TableModel.Total.ToString("F")))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;


        //            }


        //            table.AddCell(cell);
        //        }

        //        return table;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

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
        /// Export Anywhere Registration Statistics To Excel
        /// </summary>
        /// <param name="SROOfficeListID"></param>
        /// <param name="DROOfficeListID"></param>
        /// <param name="DistrictID"></param>
        /// <param name="txtDistrict"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Export Anywhere Registration Statistics to EXCEL")]
        public ActionResult ExportAnywhereRegStatToExcel(string FromDate, string ToDate, string DistrictID, string txtDistrict)
        {
            try
            {
               
                string fileName = string.Format("AnywhereRegistrationStatistics.xlsx");
                AnywhereRegStatResModel ResModel = new AnywhereRegStatResModel();
                CommonFunctions objCommon = new CommonFunctions();
                //string SROName, DROName;
                string errorMessage = string.Empty;
                AnywhereRegStatViewModel reqModel = new AnywhereRegStatViewModel
                {
                    DistrictID = Convert.ToInt32(DistrictID),
                    FromDate = FromDate,
                    ToDate=ToDate
                };

                caller = new ServiceCaller("AnywhereRegistrationStatisticsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                ResModel = caller.PostCall<AnywhereRegStatViewModel, AnywhereRegStatResModel>("GetAnywhereRegStatDetails", reqModel, out errorMessage);
                if (ResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));

                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });


                string excelHeader = string.Format("Anywhere Registraton Statistics( " + FromDate + " to " + ToDate + " )");

                string createdExcelPath = CreateExcel(ResModel, reqModel,fileName, txtDistrict);

                if (string.IsNullOrEmpty(createdExcelPath))
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });


                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "AnywhereRegistrationStatistics_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="ResModel"></param>
        /// <param name="ReqModel"></param>
        /// <param name="fileName"></param>
        /// <param name="DistrictText"></param>
        /// <returns>returns excel file path</returns>
        private string CreateExcel(AnywhereRegStatResModel ResModel, AnywhereRegStatViewModel ReqModel, string FileName,string DistrictText)
        {

            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");

            BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", FileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                int RegOffRowIndex = 5;
                int RegOffColIndex = 2;
                int JurisdictionRowIndex = 7;
                int JurisdictionColIndex = 1;
                string excelHeader = "Anywhere Registration Statistics ( "+ ReqModel.FromDate + " to "+ ReqModel.ToDate + " )";
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Anywhere Registration Statistics");
                    workSheet.Cells.Style.Font.Size = 14;

                    //ExcelRange rg = workSheet.Cells[7, 2, 8, 5];
                    //rg.Style.Border =;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + DistrictText;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;

                    workSheet.Cells[1, 1, 1, 10].Merge = true;
                    workSheet.Cells[2, 1, 2, 10].Merge = true;
                    workSheet.Cells[3, 1, 3, 10].Merge = true;

                    using (ExcelRange Rng = workSheet.Cells[7, 2, (ResModel.SROList.Count + 6), (ResModel.SROList.Count + 2)])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    using (ExcelRange Rng = workSheet.Cells[5, 1, 5, (ResModel.SROList.Count + 2)])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[6, 1, 6, (ResModel.SROList.Count + 2)])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[6, 1, (ResModel.SROList.Count + 6), 1])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    //workSheet.Cells[3, 1].Value = "Registration Date : " + Convert.ToDateTime(reqModel.Stamp5DateTime).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); ;

                    //workSheet.Cells[3, 1].Value = "District : " + reqModel.DroName;
                    //workSheet.Cells[4, 1].Value = "SRO : " + reqModel.SroName + "                                                                                                                                                                                                  Note : This report is based on pre processed data considered upto :" + MaxDate;

                    workSheet.Cells[1, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[2, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[3, 1].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[4, 1].Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Column(1).Width = 45;
                    for (int SroIndex = 2; SroIndex < (ResModel.SROList.Count+3); SroIndex++)
                    {
                        workSheet.Column(SroIndex).Width = 45;

                    }
                    for (int SroIndex = 2; SroIndex < (ResModel.SROList.Count + 3); SroIndex++)
                    {
                        workSheet.Cells[5, SroIndex, 6, SroIndex].Merge = true;

                    }
                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    //workSheet.Row().Style.Font.Bold = true;


                    //workSheet.Cells[7, 2].Value = "Total Documents Registered ";
                    //workSheet.Cells[7, 3].Value = "Total Registration fees (in Rs.)";
                    //workSheet.Cells[7, 4].Value = "Total Stamp Duty (in Rs.)";
                    //workSheet.Cells[7, 5].Value = "Total (in Rs.)";

                    //workSheet.Cells[7, 2].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[7, 3].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[7, 4].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[7, 5].Style.Font.Name = "KNB-TTUmaEN";


                    //workSheet.Cells[8, 2].Value = tableModel.TotalDocuments;
                    //workSheet.Cells[8, 3].Value = tableModel.TotalRegFee;
                    //workSheet.Cells[8, 4].Value = tableModel.TotalStampDuty;
                    //workSheet.Cells[8, 5].Value = tableModel.Total;

                    //workSheet.Cells[8, 2].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[8, 3].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[8, 4].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[8, 5].Style.Font.Name = "KNB-TTUmaEN";

                    //workSheet.Cells[10, 1].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[10, 2].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[10, 3].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[10, 4].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[10, 5].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[10, 6].Style.Font.Name = "KNB-TTUmaEN";
                    //workSheet.Cells[10, 7].Style.Font.Name = "KNB-TTUmaEN";



                    //workSheet.Cells[10, 1].Value = "SR No.";
                    //workSheet.Cells[10, 2].Value = "District";
                    //workSheet.Cells[10, 3].Value = "SRO Name";
                    //workSheet.Cells[10, 4].Value = "Documents Registered";
                    //workSheet.Cells[10, 5].Value = "Registration Fee(in Rs.)";
                    //workSheet.Cells[10, 6].Value = "Stamp Duty(in Rs.)";
                    //workSheet.Cells[10, 7].Value = "Total(in Rs.)";

                    //workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;



                    //List<int> indexList = new List<int> { 3, 4, 5, 6 };

                    //foreach (var items in tableModel.TodaysTotalDocsRegTableList)
                    //{

                    //    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //    workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                    //    workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    //    workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;


                    //    workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                    //    workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                    //    workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";

                    //    workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                    //    workSheet.Cells[rowIndex, 2].Value = items.District;
                    //    workSheet.Cells[rowIndex, 3].Value = items.SROName;
                    //    workSheet.Cells[rowIndex, 4].Value = items.Documents;
                    //    workSheet.Cells[rowIndex, 5].Value = items.RegistrationFee;
                    //    workSheet.Cells[rowIndex, 6].Value = items.StampDuty;
                    //    workSheet.Cells[rowIndex, 7].Value = items.Total;
                    //    rowIndex++;

                    //}
                    //workSheet.Cells[8, 5].Style.Numberformat.Format = "0.00";

                    //workSheet.Cells[8, 6].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[8, 7].Style.Numberformat.Format = "0.00";

                    //workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //workSheet.Cells[rowIndex, 3].Value = "Total";
                    //workSheet.Cells[rowIndex, 4].Value = tableModel.TotalDocuments;
                    //workSheet.Cells[rowIndex, 5].Value = tableModel.TotalRegFee;
                    //workSheet.Cells[rowIndex, 6].Value = tableModel.TotalStampDuty;
                    //workSheet.Cells[rowIndex, 7].Value = tableModel.Total;

                    //workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    //workSheet.Row(rowIndex).Style.Font.Bold = true;
                    //workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    //workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";

                    //using (ExcelRange Rng = workSheet.Cells[rowIndex, 4, rowIndex, 6])
                    //{
                    //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //}

                    //using (ExcelRange Rng = workSheet.Cells[11, 4, rowIndex, 6])
                    //{
                    //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    //}

                    //using (ExcelRange Rng = workSheet.Cells[11, 2, rowIndex, 2])
                    //{
                    //    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    //}
                    //workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";


                    //using (ExcelRange Rng = workSheet.Cells[10, 1, rowIndex, 7])
                    //{
                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    //}

                    for (int RowIndex = 1; RowIndex <= ResModel.RowCount; RowIndex++)
                    {
                        for (int ColumnIndex = 1; ColumnIndex <= (ResModel.ColumnCount+1); ColumnIndex++)
                        {
                            workSheet.Cells[(RowIndex + 6), (ColumnIndex+1)].Value = ResModel.AnywhereRegStatArray[RowIndex,ColumnIndex];
                            workSheet.Cells[(RowIndex + 6), (ColumnIndex+1)].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[(RowIndex + 6), (ColumnIndex + 1)].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        }
                    }

                    foreach (var SRO in ResModel.SROList)
                    {
                        workSheet.Cells[RegOffRowIndex, RegOffColIndex].Value = SRO.Text;
                        workSheet.Cells[JurisdictionRowIndex, JurisdictionColIndex].Value = SRO.Text;
                        workSheet.Cells[RegOffRowIndex, RegOffColIndex].Style.Font.Bold = true;
                        workSheet.Cells[JurisdictionRowIndex, JurisdictionColIndex].Style.Font.Bold = true;
                        workSheet.Cells[RegOffRowIndex, RegOffColIndex].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[JurisdictionRowIndex, JurisdictionColIndex].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[JurisdictionRowIndex++, JurisdictionColIndex].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[RegOffRowIndex, RegOffColIndex].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Cells[RegOffRowIndex, RegOffColIndex++].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    }

                    workSheet.Cells[RegOffRowIndex, RegOffColIndex].Value ="Total";
                    workSheet.Cells[RegOffRowIndex, RegOffColIndex].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Cells[RegOffRowIndex, RegOffColIndex].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[RegOffRowIndex, RegOffColIndex].Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Cells[5, 1].Value = "Registration Office →";
                    workSheet.Cells[6, 1].Value = "Jurisdiction ↓";
                    workSheet.Cells[5, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[6, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[5, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    workSheet.Cells[6, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;


                    package.SaveAs(templateFile); 


                }
            }
            catch (Exception ex)
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