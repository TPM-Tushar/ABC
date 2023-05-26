#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   TodaysDocumentsRegisteredController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    [KaveriAuthorizationAttribute]
    public class TodaysDocumentsRegisteredController : Controller
    {
        ServiceCaller caller = null;
        ServiceCaller callerCommon = null;

        /// <summary>
        /// Todays Documents Registered View
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "Todays Documents Registered View")]
        public ActionResult TodaysDocumentsRegisteredView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.TotalDocumentsRegistered;
                int OfficeID = KaveriSession.Current.OfficeID;
                caller = new ServiceCaller("TodaysDocumentsRegisteredAPIController");
                TodaysDocumentsRegisteredReqModel reqModel = caller.GetCall<TodaysDocumentsRegisteredReqModel>("TodaysDocumentsRegisteredView", new { OfficeID = OfficeID });
                //  reqModel.Stamp5Date = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Total documents registered and Revenue collected View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get SRO Office List By District ID
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <returns>returns SRO Office list</returns>
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

        /// <summary>
        /// Get Todays Total Documents Registered Details
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <param name="SROID"></param>
        /// <param name="Stamp5Date"></param>
        /// <param name="ToDate"></param>
        /// <returns>returns Todays Total Documents Registered Details list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Todays Total Documents Registered Details")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetTodaysTotalDocumentsRegisteredDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("TodaysDocumentsRegisteredAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects      
                int RecordsFiltered=0;
                int totalCount = 0 ;
                string SpecialSP = formCollection["SpecialSP"];
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SroID = formCollection["SroID"];
                string DocTypeID = formCollection["DocTypeID"];
                string DistrictID = formCollection["DistrictID"];
                int SroId = Convert.ToInt32(SroID);
                int DistrictId = Convert.ToInt32(DistrictID);
                int DocTypeId = Convert.ToInt32(DocTypeID);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                int startLen = Convert.ToInt32(formCollection["start"]);
                  int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
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
                //}
                //if (string.IsNullOrEmpty(FromDate))
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "From Date required"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (string.IsNullOrEmpty(ToDate))
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "To Date required"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);
                //bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                //if (!boolFrmDate)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Invalid From Date"

                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (!boolToDate)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Invalid To Date"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //else if (frmDate > toDate)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "From Date can not be larger than To Date"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                ////else if ((toDate - frmDate).TotalDays > 180)//six months validation by RamanK on 20-09-2019
                ////{
                ////    var emptyData = Json(new
                ////    {
                ////        draw = formCollection["draw"],
                ////        recordsTotal = 0,
                ////        recordsFiltered = 0,
                ////        data = "",
                ////        status = "0",
                ////        errorMessage = "Data of six months can be seen at a time"
                ////    });
                ////    emptyData.MaxJsonLength = Int32.MaxValue;
                ////    return emptyData;

                ////}
                TodaysDocumentsRegisteredReqModel reqModel = new TodaysDocumentsRegisteredReqModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.Stamp5DateTime = frmDate;
                reqModel.ToDate = toDate;
                reqModel.SROfficeID = SroId;
                reqModel.DROfficeID = DistrictId; 
                    reqModel.DocumentTypeID = DocTypeId;
                //Added by mayank on 13/09/2021 for Firm registered report
                if (DocTypeId == 4)
                    reqModel.isDRO = 1;
                //End
                TodaysTotalDocsRegDetailsTable TodaysDocRegdResModel = caller.PostCall<TodaysDocumentsRegisteredReqModel, TodaysTotalDocsRegDetailsTable>("GetTodaysTotalDocumentsRegisteredDetails", reqModel, out errorMessage);

                IEnumerable<TodaysDocumentsRegisteredDetailsModel> result = TodaysDocRegdResModel.TodaysTotalDocsRegTableList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Total Documents registered." });
                }

                totalCount = result.Count();
                //reqModel.startLen = 0;
                //reqModel.totalNum = totalCount;
              
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
                        result = result.Where(m => m.SRNo.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.District.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.Documents.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegistrationFee.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.StampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.Total.ToString().ToLower().Contains(searchValue.ToLower()) 
                        );
                        RecordsFiltered = result.Count();
                    }
                }

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

                var gridData = result.Select(TodaysDocumentsRegisteredDetailsModel => new
                {
                    SRNo = TodaysDocumentsRegisteredDetailsModel.SRNo,
                    District = TodaysDocumentsRegisteredDetailsModel.District,
                    SROName = TodaysDocumentsRegisteredDetailsModel.SROName,
                    Documents = TodaysDocumentsRegisteredDetailsModel.Documents,
                    RegistrationFee = TodaysDocumentsRegisteredDetailsModel.RegistrationFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    StampDuty = TodaysDocumentsRegisteredDetailsModel.StampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Total = TodaysDocumentsRegisteredDetailsModel.Total.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                });

                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = RecordsFiltered,
                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        // ADDED BY SHUBHAM BHAGAT ON 23-09-2020
                        ReportInfo = TodaysDocRegdResModel.ReportInfo
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
                else
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        // ADDED BY SHUBHAM BHAGAT ON 23-09-2020
                        ReportInfo = TodaysDocRegdResModel.ReportInfo
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                
                // BELOW CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 20-08-2020
                return Json(new { serverError = true, errorMessage = "Error occured while getting Total Documents registered." }, JsonRequestBehavior.AllowGet);
                //return Json(new { serverError = true, errorMessage = "Error occured while getting EC Daily Receipt Details." }, JsonRequestBehavior.AllowGet);
                // ABOVE CODE COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 20-08-2020
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
        [EventAuditLogFilter(Description = "Export To days Total Docs Reg Report To PDF")]
        public ActionResult ExportTodaysTotalDocsRegReportToPDF(string SROOfficeListID, string DROOfficeListID, string Date, string ToDate, string MaxDate)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                caller = new ServiceCaller("TodaysDocumentsRegisteredAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                callerCommon = new ServiceCaller("CommonsApiController");
                string SROName, DROName;

                TodaysDocumentsRegisteredReqModel model = new TodaysDocumentsRegisteredReqModel
                {
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    DROfficeID = Convert.ToInt32(DROOfficeListID)

                };

                model.Stamp5DateTime = Convert.ToDateTime(Date);
                model.Stamp5Date = Date;
                model.ToDate = Convert.ToDateTime(ToDate);
                model.ToDate_Str = ToDate;

                if (DROOfficeListID == "0" && SROOfficeListID == "0")
                {
                    model.SroName = "All";
                    model.DroName = "All";

                }
                if (DROOfficeListID != "0" && SROOfficeListID == "0")
                {
                    DROName = callerCommon.GetCall<string>("GetDroName", new { DistrictID = DROOfficeListID });
                    model.SroName = "All";
                    model.DroName = DROName;

                }
                if (DROOfficeListID != "0" && SROOfficeListID != "0")
                {
                    SROName = callerCommon.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                    DROName = callerCommon.GetCall<string>("GetDroName", new { DistrictID = DROOfficeListID });
                    model.SroName = SROName;
                    model.DroName = DROName;

                }


                TodaysTotalDocsRegDetailsTable objListItemsToBeExported = new TodaysTotalDocsRegDetailsTable();



                //To get records of Total Documents Registered Report
                objListItemsToBeExported = caller.PostCall<TodaysDocumentsRegisteredReqModel, TodaysTotalDocsRegDetailsTable>("GetTodaysTotalDocumentsRegisteredDetails", model, out errorMessage);
                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }


                objListItemsToBeExported.RegistrationDate = Date;
                string fileName = string.Format("TodaysTotalDocumentsRegisteredReportPDF.pdf");
                byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, model, fileName, MaxDate);
                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "TodaysTotalDocumentsRegisteredReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
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
        /// <param name="TableModel"></param>
        /// <param name="reqModel"></param>
        /// <param name="fileName"></param>
        /// <returns>returns pdf byte array</returns>
        private byte[] CreatePDFFile(TodaysTotalDocsRegDetailsTable TableModel, TodaysDocumentsRegisteredReqModel reqModel, string fileName, string MaxDate)
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

                            string TotalDocumentsRegistered = "Total Documents Registered";
                            string TotalRegistrationfees = "Total Registration fees (in Rs.)";
                            string TotalStampDuty1 = "Total Stamp Duty (in Rs.)";
                            string Total = "Total (in Rs.)";

                            PdfPCell cell = null;

                            cell = new PdfPCell(new Phrase())
                            {
                                BackgroundColor = new BaseColor(226, 226, 226)
                            };

                            string[] colSummary = { TotalDocumentsRegistered, TotalRegistrationfees, TotalStampDuty1, Total };
                            PdfPTable tableSummary = new PdfPTable(4)
                            {
                                WidthPercentage = 50
                            };
                            tableSummary.HeaderRows = 1;


                            tableSummary.SetWidths(new Single[] { 5, 5, 5, 5 });
                            tableSummary.SpacingBefore = 10f;

                            for (int i = 0; i < colSummary.Length; ++i)
                            {
                                cell = new PdfPCell(new Phrase(colSummary[i]))
                                {
                                    BackgroundColor = new BaseColor(204, 255, 255)
                                };
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableSummary.AddCell(cell);
                            }
                            //tableSummary.AddCell(new Phrase(TableModel.TotalDocuments, tableContentFont));
                            //tableSummary.AddCell(new Phrase(TableModel.TotalRegFee, tableContentFont));
                            //tableSummary.AddCell(new Phrase(TableModel.TotalStampDuty, tableContentFont));
                            //tableSummary.AddCell(new Phrase(TableModel.Total, tableContentFont));
                            PdfPCell cell1 = new PdfPCell(new Phrase(TableModel.TotalDocuments.ToString(), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell1.BackgroundColor = BaseColor.WHITE;
                            PdfPCell cell2 = new PdfPCell(new Phrase(TableModel.TotalRegFee.ToString("F"), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell2.BackgroundColor = BaseColor.WHITE;
                            PdfPCell cell3 = new PdfPCell(new Phrase(TableModel.TotalStampDuty.ToString("F"), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell3.BackgroundColor = BaseColor.WHITE;
                            PdfPCell cell4 = new PdfPCell(new Phrase(TableModel.Total.ToString("F"), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell4.BackgroundColor = BaseColor.WHITE;
                            tableSummary.AddCell(cell1);
                            tableSummary.AddCell(cell2);
                            tableSummary.AddCell(cell3);
                            tableSummary.AddCell(cell4);



                            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                            var headerTextFont = FontFactory.GetFont("KNB-TTUmaEN", 15, new BaseColor(0, 128, 255));
                            //PdfWriter writer = PdfWriter.GetInstance(doc, fs);
                            doc.Open();
                            Paragraph addHeading = new Paragraph("Total documents registered and Revenue collected (Between " + reqModel.Stamp5Date + " and " + reqModel.ToDate_Str + " )", headerTextFont)
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

                            var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
                            //var registrationDateChunk = new Chunk("Registration Date : ", blackListTextFont);

                            var SRONameChunk = new Chunk("SRO : ", blackListTextFont);
                            var DRONameChunk = new Chunk("District : ", blackListTextFont);

                            var ReportGeneration = new Chunk("This Report is Generated on: ", blackListTextFont);

                            var totalChunk = new Chunk("Total Records: ", blackListTextFont);
                            var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);

                            // var SroName = new Chunk(SROName, redListTextFont);
                            var descriptionChunk = new Chunk(DateTime.Now.ToString(), redListTextFont);
                            var TotalStampDutyChunk = new Chunk(TableModel.TotalStampDuty.ToString(), redListTextFont);
                            var TotalDocsGegdChunk = new Chunk(Convert.ToString(TableModel.TotalDocuments), redListTextFont);
                            var TotalRegFeesChunk = new Chunk(TableModel.TotalRegFee.ToString(), redListTextFont);
                            var RegDescChunk = new Chunk(TableModel.RegistrationDate, redListTextFont);
                            var SroChunk = new Chunk(reqModel.SroName, redListTextFont);
                            var DroChunk = new Chunk(reqModel.DroName, redListTextFont);




                            var ReportGenerationChunk = new Chunk(Convert.ToString(TableModel.TotalDocuments), redListTextFont);

                            var titlePhrase = new Phrase(titleChunk)
                                {
                                    descriptionChunk
                                };
                            //var RegistrationDatePhrase = new Phrase(registrationDateChunk)
                            //    {
                            //        RegDescChunk
                            //    };
                            var SRONamePhrase = new Phrase(SRONameChunk)
                                {
                                    SroChunk
                                };
                            var DRONamePhrase = new Phrase(DRONameChunk)
                                {
                                    DroChunk
                                };
                            //Paragraph NotePara = new Paragraph("Note : This report is based on pre processed data considered upto : " + MaxDate, FontItalic);
                            Paragraph NotePara = new Paragraph(TableModel.ReportInfo , FontItalic);

                            NotePara.Alignment = Element.ALIGN_RIGHT;
                            //PdfPCell cellNote = new PdfPCell(new Phrase("This report is based on pre processed data considered upto : " + MaxDate));
                            //NotePara.Font.Style = FontItalic;
                            doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(titlePhrase);
                            doc.Add(addSpace);
                            //doc.Add(RegistrationDatePhrase);
                            doc.Add(addSpace);
                            doc.Add(DRONamePhrase);
                            doc.Add(addSpace);
                            doc.Add(SRONamePhrase);
                            doc.Add(addSpace);
                            doc.Add(NotePara);
                            doc.Add(addSpace);

                            //Table Data
                            doc.Add(tableSummary);
                            doc.Add(TodaysTotalDocsRegReportTable(TableModel));
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
        /// <param name="TableModel"></param>
        /// <returns>returns pdf table</returns>
        private PdfPTable TodaysTotalDocsRegReportTable(TodaysTotalDocsRegDetailsTable TableModel)
        {

            string SRNo = "Sr No.";
            string District = "District";
            string SROName = "SRO Name";
            string Documents = "Documents Registered";
            string RegistrationFees = "Registration Fees(in Rs.)";
            string StampDuty = "Stamp Duty(in Rs.)";
            string Total = "Total(in Rs.)";


            try
            {

                string[] col = { SRNo, District, SROName, Documents, RegistrationFees, StampDuty, Total };
                PdfPTable table = new PdfPTable(7)
                {
                    WidthPercentage = 100
                };
                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);


                //string Italic = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                //BaseFont customKannadafont = BaseFont.CreateFont(KNB-TTUmaENUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                //iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 11);


                table.HeaderRows = 1;
                table.SetWidths(new Single[] { 5, 5, 5, 5, 6, 5, 5 });
                table.SpacingBefore = 25f;
                PdfPCell cell = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;
                PdfPCell cell7 = null;

                for (int i = 0; i < col.Length; ++i)
                {
                    cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    table.AddCell(cell);
                }
                foreach (var items in TableModel.TodaysTotalDocsRegTableList)
                {


                    cell1 = new PdfPCell(new Phrase(Convert.ToString(items.SRNo), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell2 = new PdfPCell(new Phrase(items.SROName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(items.Documents.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.RegistrationFee.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell4.BackgroundColor = BaseColor.WHITE;

                    cell5 = new PdfPCell(new Phrase(items.StampDuty.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.BackgroundColor = BaseColor.WHITE;

                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell6 = new PdfPCell(new Phrase(items.Total.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell6.BackgroundColor = BaseColor.WHITE;

                    cell7 = new PdfPCell(new Phrase(items.District, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell7.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell7.BackgroundColor = BaseColor.WHITE;

                    table.AddCell(cell1);
                    table.AddCell(cell7);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                    table.AddCell(cell6);
                }
                for (int i = 0; i < col.Length; ++i)
                {

                    if (i == 0)
                    {
                        cell = new PdfPCell(new Phrase(""))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    }
                    if (i == 1)
                    {
                        cell = new PdfPCell(new Phrase(""))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;

                    }
                    if (i == 2)
                    {
                        cell = new PdfPCell(new Phrase("Total"))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    }

                    if (i == 3)
                    {
                        cell = new PdfPCell(new Phrase(TableModel.TotalDocuments.ToString()))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    }

                    if (i == 4)
                    {
                        cell = new PdfPCell(new Phrase(TableModel.TotalRegFee.ToString("F")))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;

                    }

                    if (i == 5)
                    {
                        cell = new PdfPCell(new Phrase(TableModel.TotalStampDuty.ToString("F")))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;


                    }
                    if (i == 5)
                    {
                        cell = new PdfPCell(new Phrase(TableModel.Total.ToString("F")))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;


                    }


                    table.AddCell(cell);
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
        /// Export Todays Documents Registered Report To Excel
        /// </summary>
        /// <param name="SROOfficeListID"></param>
        /// <param name="DROOfficeListID"></param>
        /// <param name="Date"></param>
        /// <param name="ToDate"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Export Todays Documents Registered Report To Excel")]
        public ActionResult ExportTodaysDocumentsRegisteredReportToExcel(string SROOfficeListID, string DROOfficeListID, string Date, string ToDate, string MaxDate, string DocTypeID, string DocTypeText, string SpecialSP)
        {
            try
            {
                string fileName = "TodaysTotalDocumentsRegisteredReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                TodaysTotalDocsRegDetailsTable tableModel = new TodaysTotalDocsRegDetailsTable();
                CommonFunctions objCommon = new CommonFunctions();
                string SROName, DROName;
                callerCommon = new ServiceCaller("CommonsApiController");

                

                string errorMessage = string.Empty;
                TodaysDocumentsRegisteredReqModel reqModel = new TodaysDocumentsRegisteredReqModel
                {
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    DROfficeID = Convert.ToInt32(DROOfficeListID)

                };
                reqModel.Stamp5DateTime = Convert.ToDateTime(Date);
                reqModel.ToDate = Convert.ToDateTime(ToDate);
                reqModel.ToDate_Str = ToDate;
                reqModel.Stamp5Date = Date;
                reqModel.SpecialSP = SpecialSP;
                //Added by Madhusoodan on 01-05-2020
                reqModel.DocumentTypeID = Convert.ToInt32(DocTypeID);

                if (DROOfficeListID == "0" && SROOfficeListID == "0")//0:All by Raman Kalegaonkar
                {
                    reqModel.SroName = "All";
                    reqModel.DroName = "All";

                }
                if (DROOfficeListID != "0" && SROOfficeListID == "0")//0:All by Raman Kalegaonkar
                {
                    DROName = callerCommon.GetCall<string>("GetDroName", new { DistrictID = DROOfficeListID });
                    reqModel.SroName = "All";
                    reqModel.DroName = DROName;

                }
                if (DROOfficeListID != "0" && SROOfficeListID != "0")//0:All by Raman Kalegaonkar
                {
                    SROName = callerCommon.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });
                    DROName = callerCommon.GetCall<string>("GetDroName", new { DistrictID = DROOfficeListID });
                    reqModel.SroName = SROName;
                    reqModel.DroName = DROName;

                }

                //Added by mayank on 13/09/2021 for Firm registered report
                if (reqModel.DocumentTypeID == 4)
                    reqModel.isDRO = 1;
                //End
                caller = new ServiceCaller("TodaysDocumentsRegisteredAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                tableModel = caller.PostCall<TodaysDocumentsRegisteredReqModel, TodaysTotalDocsRegDetailsTable>("GetTodaysTotalDocumentsRegisteredDetails", reqModel, out errorMessage);
                if (tableModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });


                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));

                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //    return Redirect("/ECDataAuditDetails/Error");

                string excelHeader = string.Format("Total documents registered and Revenue collected Between( " + Date + " to " + ToDate + " )");

                string createdExcelPath = CreateExcel(tableModel, reqModel, fileName, excelHeader, MaxDate, DocTypeText);

                //if (string.IsNullOrEmpty(createdExcelPath))
                //    return Redirect("/ECDataAuditDetails/Error");

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);

                //Response.AppendCookie(new HttpCookie("fileDownload", "true") { Path = "/", HttpOnly = false });
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
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
        /// <param name="tableModel"></param>
        /// <param name="reqModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateExcel(TodaysTotalDocsRegDetailsTable tableModel, TodaysDocumentsRegisteredReqModel reqModel, string fileName, string excelHeader, string MaxDate, string DocTypeText)
        {
            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");

            BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //Commented and some lines added by Madhusoodan on 30-04-2020
                //int rowIndex = 11;
                int rowIndex = 12;
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Total Documents Registered and Revenue Collected");
                    workSheet.Cells.Style.Font.Size = 14;
                    //ExcelRange rg = workSheet.Cells[7, 2, 8, 5];
                    //rg.Style.Border =;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[1, 1, 1, 10].Merge = true;
                    workSheet.Cells[2, 1, 2, 10].Merge = true;
                    workSheet.Cells[3, 1, 3, 10].Merge = true;
                    workSheet.Cells[4, 1, 4, 10].Merge = true;

                    workSheet.Cells[5, 1, 5, 10].Merge = true;

                    //workSheet.Cells[7, 2, 8, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    using (ExcelRange Rng = workSheet.Cells[8, 2, 9, 5])    //7, 2, 8, 5 => 8,2,9,5
                    {
                        Rng.Value = "Thin";
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[8, 2, 8, 5])
                    {
                        Rng.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    }
                    //workSheet.Cells[3, 1].Value = "Registration Date : " + Convert.ToDateTime(reqModel.Stamp5DateTime).ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture); ;
                    workSheet.Cells[3, 1].Value = "District : " + reqModel.DroName;


                    if (reqModel.Stamp5DateTime == DateTime.Now.Date && reqModel.ToDate == DateTime.Now.Date)
                    {
                        workSheet.Cells[4, 1].Value = "SRO : " + reqModel.SroName;
                    }
                    else
                    {
                        //Added by Madhusoodan on 05-05-2020
                        //To print Note only when Document Type Registration is selected.
                        if (DocTypeText == "Document")
                            workSheet.Cells[4, 1].Value = "SRO : " + reqModel.SroName + "                                                                                                                                                                                                  Note : This report is based on pre processed data considered upto :" + MaxDate;
                        else
                            workSheet.Cells[4, 1].Value = "SRO : " + reqModel.SroName;
                    }

                    workSheet.Cells[5, 1].Value = "Registration Type : " + DocTypeText;

                    workSheet.Cells[1, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[2, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[3, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[4, 1].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[5, 1].Style.Font.Name = "KNB-TTUmaEN"; 

                    workSheet.Column(1).Width = 30;
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
                    workSheet.Row(8).Style.Font.Bold = true;  // 7 => 8
                    workSheet.Row(11).Style.Font.Bold = true; //10 => 11


                    workSheet.Cells[8, 2].Value = "Total Documents Registered ";    //7 => 8  in all
                    workSheet.Cells[8, 3].Value = "Total Registration fees (in Rs.)";
                    workSheet.Cells[8, 4].Value = "Total Stamp Duty (in Rs.)";
                    //Added by mayank on 13/09/2021 for Firm registered report
                    if (DocTypeText=="Firm")
                    {
                        workSheet.Cells[8, 4].Value = "Total Others (in Rs.)";
                    }
                    else
                    {
                        workSheet.Cells[8, 4].Value = "Total Stamp Duty (in Rs.)";
                    }
                    //End
                    workSheet.Cells[8, 5].Value = "Total (in Rs.)";

                    workSheet.Cells[8, 2].Style.Font.Name = "KNB-TTUmaEN"; // 7 => in all
                    workSheet.Cells[8, 3].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[8, 4].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[8, 5].Style.Font.Name = "KNB-TTUmaEN";


                    workSheet.Cells[9, 2].Value = tableModel.TotalDocuments;   //8 => 9  in all
                    workSheet.Cells[9, 3].Value = tableModel.TotalRegFee;
                    workSheet.Cells[9, 4].Value = tableModel.TotalStampDuty;
                    workSheet.Cells[9, 5].Value = tableModel.Total;

                    workSheet.Cells[9, 2].Style.Font.Name = "KNB-TTUmaEN";  //8 => 9 in all
                    workSheet.Cells[9, 3].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[9, 4].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[9, 5].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[9, 3].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[9, 4].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[9, 5].Style.Numberformat.Format = "0.00";



                    workSheet.Cells[11, 1].Style.Font.Name = "KNB-TTUmaEN";  // 10=> 11 in all
                    workSheet.Cells[11, 2].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[11, 3].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[11, 4].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[11, 5].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[11, 6].Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Cells[11, 7].Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Cells[11, 1].Value = "SR No.";                //10 => 11 in all
                    workSheet.Cells[11, 2].Value = "District";
                    workSheet.Cells[11, 3].Value = "SRO Name";
                    workSheet.Cells[11, 4].Value = "Documents Registered";
                    workSheet.Cells[11, 5].Value = "Registration Fee(in Rs.)";
                    workSheet.Cells[11, 6].Value = "Stamp Duty(in Rs.)";
                    if(DocTypeText=="Firm")
                    {
                        workSheet.Cells[11, 6].Value = "Others(in Rs.)";
                    }
                    else
                    {
                        workSheet.Cells[11, 6].Value = "Stamp Duty(in Rs.)";

                    }
                    workSheet.Cells[11, 7].Value = "Total(in Rs.)";
                    workSheet.Row(11).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    List<int> indexList = new List<int> { 3, 4, 5, 6 };

                    foreach (var items in tableModel.TodaysTotalDocsRegTableList)
                    {

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;


                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SRNo;
                        workSheet.Cells[rowIndex, 2].Value = items.District;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        //Added by mayank on 13/09/2021 for Firm registered report
                        if (DocTypeText == "Firm")
                        {
                            workSheet.Cells[rowIndex, 3].Value = "--";
                        }
                        workSheet.Cells[rowIndex, 4].Value = items.Documents;
                        workSheet.Cells[rowIndex, 5].Value = items.RegistrationFee;
                        workSheet.Cells[rowIndex, 6].Value = items.StampDuty;
                        workSheet.Cells[rowIndex, 7].Value = items.Total;
                        rowIndex++;

                    }
                    workSheet.Cells[9, 5].Style.Numberformat.Format = "0.00";  // 8 => 9
                
                    workSheet.Cells[9, 6].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[9, 7].Style.Numberformat.Format = "0.00";

                    workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[rowIndex, 3].Value = "Total";
                    workSheet.Cells[rowIndex, 4].Value = tableModel.TotalDocuments;
                    workSheet.Cells[rowIndex, 5].Value = tableModel.TotalRegFee;
                    workSheet.Cells[rowIndex, 6].Value = tableModel.TotalStampDuty;
                    workSheet.Cells[rowIndex, 7].Value = tableModel.Total;

                    workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";

                    using (ExcelRange Rng = workSheet.Cells[rowIndex, 4, rowIndex, 6])
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    using (ExcelRange Rng = workSheet.Cells[11, 4, rowIndex, 6])
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }

                    using (ExcelRange Rng = workSheet.Cells[12, 2, rowIndex, 2])  //11 => 12
                    {
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    }
                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";


                    using (ExcelRange Rng = workSheet.Cells[11, 1, rowIndex, 7]) // 10 => 11
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    rowIndex = 12;
                    //Added by Raman Kalegaonkar on 30-09-2019
                    //if (reqModel.DroName == "All") //When All District and All SRO are selected , merge them DistrictWise
                    //{
                    //    //To Merge Rows of Same District ID
                    //    for (int Index = 0; Index < tableModel.DistrictWiseSRODict.Count; Index++)
                    //    {
                    //        workSheet.Cells[rowIndex, 2, (rowIndex + tableModel.DistrictWiseSRODict[Index] - 1), 2].Merge = true;
                    //        workSheet.Cells[rowIndex, 2, (rowIndex + tableModel.DistrictWiseSRODict[Index] - 1), 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //        workSheet.Cells[rowIndex, 2, (rowIndex + tableModel.DistrictWiseSRODict[Index] - 1), 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                    //        rowIndex = tableModel.DistrictWiseSRODict[Index] + rowIndex;
                    //    }
                    //}
                    //else if (reqModel.SroName == reqModel.DroName)
                    //{
                    //    //do nothing when District and SRO are Same (no need to merge)

                    //}
                    //else//When Specific District is selected and All SRO are selected Merge only one District
                    //{
                    //    workSheet.Cells[rowIndex, 2 , (rowIndex + tableModel.DistrictWiseSRODictForSingleDistrict[reqModel.DroName] - 1) , 2].Merge = true;
                    //    workSheet.Cells[rowIndex, 2, (rowIndex + tableModel.DistrictWiseSRODictForSingleDistrict[reqModel.DroName] - 1), 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //    workSheet.Cells[rowIndex, 2, (rowIndex + tableModel.DistrictWiseSRODictForSingleDistrict[reqModel.DroName] - 1), 2].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

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


        [HttpPost]
        [EventAuditLogFilter(Description = "Total Documents Registered Summary")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetTodaysTotalDocumentsRegisteredSummary(FormCollection formCollection)
          {
            try
            {
                #region User Variables and Objects
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROID = formCollection["SroID"];
                string DistrictID = formCollection["DistrictID"];
                string DocTypeID = formCollection["DocTypeID"];
                string SpecialSP = formCollection["SpecialSP"];
                //int DocTypeId = Convert.ToInt32(DocTypeID);
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion
                System.Text.RegularExpressions.Regex RegXForID = new Regex("^[0-9]*$");
                Match mtchSRO = RegXForID.Match(SROID);
                Match mtchDistrict = RegXForID.Match(DistrictID);
                DateTime frmDate, toDate;
                bool boolFrmDate;
                bool boolToDate;

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
                //}
                if (string.IsNullOrEmpty(FromDate))
                {
                    return Json(new { success = false, errorMessage = "Please Enter From Date." }, JsonRequestBehavior.AllowGet);

                }
                else if (string.IsNullOrEmpty(ToDate))
                {
                    return Json(new { success = false, errorMessage = "Please Enter To Date." }, JsonRequestBehavior.AllowGet);

                }
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString(), out toDate);
                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                if (!boolFrmDate)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid From Date." }, JsonRequestBehavior.AllowGet);

                }
                else if (!boolToDate)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid To Date." }, JsonRequestBehavior.AllowGet);

                }
                else if (frmDate > toDate)
                {
                    return Json(new { success = false, errorMessage = "From Date should not be greater than To Date" }, JsonRequestBehavior.AllowGet);

                }

                if (string.IsNullOrEmpty(DistrictID))
                {
                    return Json(new { success = false, errorMessage = "Please Enter District" }, JsonRequestBehavior.AllowGet);

                }
                else if (!mtchDistrict.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid District" }, JsonRequestBehavior.AllowGet);

                }

                if (string.IsNullOrEmpty(SROID))
                {
                    return Json(new { success = false, errorMessage = "Please Enter SRO Office" }, JsonRequestBehavior.AllowGet);

                }
                else if (!mtchSRO.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid SRO Office" }, JsonRequestBehavior.AllowGet);

                }
                //else if ((toDate - frmDate).TotalDays > 180)//six months validation by RamanK on 20-09-2019
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "Data of six months can be seen at a time"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;

                //}
                TodaysDocumentsRegisteredReqModel reqModel = new TodaysDocumentsRegisteredReqModel();
                reqModel.Stamp5DateTime = frmDate;
                reqModel.ToDate = toDate;
                reqModel.SROfficeID = Convert.ToInt32(SROID);
                reqModel.DROfficeID = Convert.ToInt32(DistrictID);
                reqModel.DocumentTypeID = Convert.ToInt32(DocTypeID);
                //Added by mayank on 13/09/2021 for Firm registered report
                if (reqModel.DocumentTypeID == 4)
                    reqModel.isDRO = 1;
                //End
                caller = new ServiceCaller("TodaysDocumentsRegisteredAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                TodaysTotalDocsRegDetailsTable TodaysDocRegdResModel = caller.PostCall<TodaysDocumentsRegisteredReqModel, TodaysTotalDocsRegDetailsTable>("GetTodaysTotalDocumentsRegisteredDetails", reqModel, out errorMessage);

                if (TodaysDocRegdResModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting Total Docs Registered datatable", URLToRedirect = "/Home/HomePage" });
                }

                return View(TodaysDocRegdResModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting Total Documents Registered.", URLToRedirect = "/Home/HomePage" });
            }
        }


        #endregion

    }
}
