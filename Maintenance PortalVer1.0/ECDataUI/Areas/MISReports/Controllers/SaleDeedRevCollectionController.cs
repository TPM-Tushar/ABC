#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   SaleDeedRevCollectionController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.SaleDeedRevCollection;
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
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{

    [KaveriAuthorizationAttribute]
    public class SaleDeedRevCollectionController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// Sale Deed Rev Collection View
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "Sale Deed Rev Collection View")]
        public ActionResult SaleDeedRevCollectionView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.SaleDeedRegisteredandRevenueCollected;
                caller = new ServiceCaller("SaleDeedRevCollectionAPIController");
                // ADDED BY SHUBHAM BHAGAT ON 6-6-2019
                //caller.HttpClient.Timeout = new TimeSpan(0,3,0);
                int OfficeID = KaveriSession.Current.OfficeID;
                SaleDeedRevCollectionModel reqModel = caller.GetCall<SaleDeedRevCollectionModel>("SaleDeedRevCollectionView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Sale Deed Revenue Collection View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get SROOffice List By District ID
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
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }


        /// <summary>
        /// Get Sale Deed Rev Collection Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Sale Deed Rev Collection Details list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Sale Deed Rev Collection Details")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetSaleDeedRevCollectionDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("SaleDeedRevCollectionAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {

                #region User Variables and Objects               
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeID = formCollection["DROfficeID"];
                string FinancialID = formCollection["FinancialID"];
                string PropertyTypeID = formCollection["PropertyTypeID"];
                //string BuildTypeID = formCollection["BuildTypeID"];
                string PropertyValueID= formCollection["PropertyValueID"];

                int SroId = Convert.ToInt32(SROOfficeListID);
                int DroID = Convert.ToInt32(DROfficeID);
                int PropertyTypeId = Convert.ToInt32(PropertyTypeID);
                //int BuildTypeId = Convert.ToInt32(BuildTypeID);
                int PropertyValueId= Convert.ToInt32(PropertyValueID);

                #region Server Side Validation
                if (string.IsNullOrEmpty(DROfficeID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Please select Any District."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(SROOfficeListID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Please select Any SRO."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(FinancialID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Please select Financial Year."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion
              

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                
                if (DroID < 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select DRO Office"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (SroId < 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select SRO Office"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (PropertyTypeId < 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select PropertyType"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                //else if (BuildTypeId < 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select Build Type"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                else if (PropertyValueId < 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Property Value"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                //else if (PropertyTypeId != 2 && BuildTypeId>0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select Property Type"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                SaleDeedRevCollectionModel reqModel = new SaleDeedRevCollectionModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);
                reqModel.FinacialYearID = FinancialID;
                reqModel.PropertyTypeID = PropertyTypeId;
                //reqModel.BuildTypeID = BuildTypeId;
                reqModel.PropertyValueID = PropertyValueId;



                int totalCount = caller.PostCall<SaleDeedRevCollectionModel, int>("GetSaleDeedRevCollectionDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                SaleDeedRevCollectionOuterModel saleDeedRevCollectionOuterModel = caller.PostCall<SaleDeedRevCollectionModel, SaleDeedRevCollectionOuterModel>("GetSaleDeedRevCollectionDetails", reqModel, out errorMessage);

                IEnumerable<SaleDeedRevCollectionDetail> result = saleDeedRevCollectionOuterModel.SaleDeedRevCollList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting sale deed revenue collection details." });
                }

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //  Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                //}

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
                        result = result.Where(m => m.StampDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.DocumentsRegistered.ToString().ToLower().Contains(searchValue.ToLower()) ||
                       m.RegistrationFee.ToString().ToLower().Contains(searchValue.ToLower()) ||
                       m.MonthName.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.Total.ToString().ToLower().Contains(searchValue.ToLower()) );
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(SaleDeedRevCollectionDetail => new
                {
                    SerialNo = SaleDeedRevCollectionDetail.SerialNo,
                    MonthName= SaleDeedRevCollectionDetail.MonthName,
                    DocumentsRegistered = SaleDeedRevCollectionDetail.DocumentsRegistered,
                    StampDuty = SaleDeedRevCollectionDetail.StampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegistrationFee = SaleDeedRevCollectionDetail.RegistrationFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Total = SaleDeedRevCollectionDetail.Total.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                });

               
                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
                //var JsonData = "";
                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        PDFDownloadBtn = PDFDownloadBtn,
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
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting sale deed revenue collection details." }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Download report in PDF
        /// <summary>
        /// Export Report To PDF
        /// </summary>
        /// <param name="SROOfficeListID"></param>
        /// <param name="DROfficeID"></param>
        /// <param name="FinancialID"></param>
        /// <returns>returns pdf file</returns>
        [EventAuditLogFilter(Description = "Sale Deed Rev Export Report To PDF")]
        public ActionResult ExportReportToPDF(string SROOfficeListID, string DROfficeID, string FinancialID,string SelectedDistrict, string SelectedSRO,string MaxDate)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                SaleDeedRevCollectionModel model = new SaleDeedRevCollectionModel
                {

                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    DROfficeID = Convert.ToInt32(DROfficeID),
                    FinacialYearID = FinancialID,
                    startLen = 0,
                    totalNum = 10
                };
                //model.Amount = Convert.ToInt32(Amount);

                List<SaleDeedRevCollectionDetail> objListItemsToBeExported = new List<SaleDeedRevCollectionDetail>();

                caller = new ServiceCaller("SaleDeedRevCollectionAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in indexII report datatable
                int totalCount = caller.PostCall<SaleDeedRevCollectionModel, int>("GetSaleDeedRevCollectionDetailsTotalCount", model);
                model.totalNum = totalCount;
                SaleDeedRevCollectionOuterModel saleDeedRevCollectionOuterModel = new SaleDeedRevCollectionOuterModel();
                // To get total records of indexII report table
                saleDeedRevCollectionOuterModel = caller.PostCall<SaleDeedRevCollectionModel, SaleDeedRevCollectionOuterModel>("GetSaleDeedRevCollectionDetails", model, out errorMessage);
                objListItemsToBeExported = saleDeedRevCollectionOuterModel.SaleDeedRevCollList;
                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }

                //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
                string fileName = string.Format("SaleDeedRegisteredandRevenueCollected.pdf");
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("Sale Deed Registered and Revenue Collected (" + saleDeedRevCollectionOuterModel.FinancialYear+")");

                //To get SRONAME
                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                //Create Temp PDF File
                // byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader, SROName);
                byte[] pdfBytes = CreatePDFFile(saleDeedRevCollectionOuterModel, fileName, pdfHeader, saleDeedRevCollectionOuterModel.SROCode, SelectedDistrict,SelectedSRO,MaxDate);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "SaleDeedRegisteredandRevenueCollected_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");

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
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="pdfHeader"></param>
        /// <returns>returns pdf byte array</returns>
        private byte[] CreatePDFFile(SaleDeedRevCollectionOuterModel SaleDeedOuterModel, string fileName, string pdfHeader,int SroCode,string SelectedDistrict, string SelectedSRO,string MaxDate)
        {
            string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));

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

                            //  string Info = string.Format("Print Date Time : {0}   Total Records : {1}  SRO Name : {2}", DateTime.Now.ToString(), SROName);
                            doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                            var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
                            doc.Open();
                            Paragraph addHeading = new Paragraph(pdfHeader, headerTextFont)
                            {
                                Alignment = 1,
                            };
                            //Paragraph Info = new Paragraph(Info, redListTextFont)
                            //{
                            //    Alignment = 1,
                            //};
                            Paragraph addSpace = new Paragraph(" ")
                            {
                                Alignment = 1
                            };
                            var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
                            //var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(128,191,255));
                            var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));


                            var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
                            var totalChunk = new Chunk("Total Records: ", blackListTextFont);
                            var SroNameChunk = new Chunk("SRO : ", blackListTextFont);
                            var DistrictChunk = new Chunk("District : ", blackListTextFont);



                            var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
                            string count = SaleDeedOuterModel.SaleDeedRevCollList.Count().ToString();
                            var countChunk = new Chunk(count, redListTextFont);
                            var District = new Chunk(SelectedDistrict+"      ", redListTextFont);
                            var SRO = new Chunk(SelectedSRO+"      ", redListTextFont);


                            var titlePhrase = new Phrase(titleChunk)
                        {
                            descriptionChunk
                        };
                            var totalPhrase = new Phrase(totalChunk)
                        {
                            countChunk
                        };
                            var DistrictPhrase = new Phrase(DistrictChunk)
                        {
                            District
                        };
                            var SROPhrase = new Phrase(SroNameChunk)
                        {
                            SRO
                        };
                            //    var SroNamePhrase = new Phrase(SroNameChunk)
                            //{
                            //    SroName
                            //};
                            var FontItalic = FontFactory.GetFont("Arial", 10, 2, new BaseColor(94, 94, 94));
                            Paragraph NotePara = new Paragraph("Note : This report is based on pre processed data considered upto : " + MaxDate, FontItalic);
                            NotePara.Alignment = Element.ALIGN_RIGHT;

                            doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(DistrictPhrase);
                            doc.Add(SROPhrase);
                            doc.Add(titlePhrase);
                            //doc.Add(SroNamePhrase);
                            doc.Add(totalPhrase);
                            doc.Add(addSpace);
                            doc.Add(NotePara);
                            doc.Add(addSpace);
                            //if (SroCode==0)
                            //{
                            //    doc.Add(ReportTable(SaleDeedOuterModel));

                            //}
                            //else
                            //{
                            //    doc.Add(ReportTableHavingMonthColumn(SaleDeedOuterModel));


                            //}
                            doc.Add(ReportTable(SaleDeedOuterModel));
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

        //To get PdfPtable of IndexII report 
        /// <summary>
        /// Report Table
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns>returns pdf table</returns>
        private PdfPTable ReportTable(SaleDeedRevCollectionOuterModel SaleDeedOuterModel)
        {

            string SerialNumber = "Serial Number";
            string Month = "Month";
            string DocumentsRegistered = "Documents Registered";
            string StumpDuty = "Stamp Duty ( in Rs. )";
            string RegistrationFee = "Registration Fee ( in Rs. )";
            string Total = "Total ( in Rs. )";
            //string Schedule = "Schedule";
            //string Executant = "Executant";
            //string Claimant = "Claimant";
            //string VillageNameE = "Village Name";
            //string Consideration = "Consideration";
            //string MarketValue = "Market Value";
            try
            {
                string[] col = { SerialNumber, Month, DocumentsRegistered, StumpDuty, RegistrationFee, Total };
                PdfPTable table = new PdfPTable(6)
                {
                    WidthPercentage = 100
                };
                // table.DefaultCell.FixedHeight = 500f;

                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 14);

                //to repeat Headers
                table.HeaderRows = 1;
                // then set the column's __relative__ widths
                table.SetWidths(new Single[] { 4, 6, 6, 6, 7, 6 });
                /*
                * by default tables 'collapse' on surrounding elements,
                * so you need to explicitly add spacing
                */
                //table.SpacingBefore = 10;

                // PdfPCell cell = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                //PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;
                PdfPCell cell7 = null;
                for (int i = 0; i < col.Length; ++i)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    table.AddCell(cell);

                }
                //foreach (var items in objListItemsToBeExported)
                //{
                //    table.AddCell(new Phrase(items.DocumentNumber, tableContentFont));

                //    table.AddCell(new Phrase(items.FinalRegistrationNumber, tableContentFont));

                //    table.AddCell(new Phrase(items.PresentDatetime, tableContentFont));
                //    table.AddCell(new Phrase(items.StampDuty.ToString(), tableContentFont));
                //    table.AddCell(new Phrase(items.ReceiptFee.ToString(), tableContentFont));
                //    table.AddCell(new Phrase(items.ReceiptNumber.ToString(), tableContentFont));

                //}
                foreach (var items in SaleDeedOuterModel.SaleDeedRevCollList)
                {


                    cell1 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell2 = new PdfPCell(new Phrase(items.MonthName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell2.BackgroundColor = BaseColor.WHITE;
                    //cell2 = new PdfPCell(new Phrase(items.DistrictName, tableContentFont))
                    //{
                    //    BackgroundColor = new BaseColor(204, 255, 255)
                    //};
                    //cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell2.BackgroundColor = BaseColor.WHITE;

                    //cell3 = new PdfPCell(new Phrase(items.SROName, tableContentFont))
                    //{
                    //    BackgroundColor = new BaseColor(204, 255, 255)
                    //};
                    //cell3.HorizontalAlignment = Element.ALIGN_LEFT;
                    //cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.DocumentsRegistered.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell4.BackgroundColor = BaseColor.WHITE;



                    cell5 = new PdfPCell(new Phrase(items.StampDuty.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell5.BackgroundColor = BaseColor.WHITE;

                    cell6 = new PdfPCell(new Phrase(items.RegistrationFee.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell6.BackgroundColor = BaseColor.WHITE;

                    cell6.HorizontalAlignment = Element.ALIGN_RIGHT;


                    cell7 = new PdfPCell(new Phrase(items.Total.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell7.BackgroundColor = BaseColor.WHITE;

                    cell7.HorizontalAlignment = Element.ALIGN_RIGHT;
                    

                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    //table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                    table.AddCell(cell6);
                    table.AddCell(cell7);

                }
                
                PdfPCell bottomCell = null;
                for (int i = 0; i < col.Length; ++i)
                {

                    if (i == 0)
                    {
                        bottomCell = new PdfPCell(new Phrase(""))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    }
                    if (i == 1)
                    {
                        bottomCell = new PdfPCell(new Phrase("Total"))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    }
                    //if (i == 2)
                    //{
                    //    bottomCell = new PdfPCell(new Phrase(""))
                    //    {
                    //        BackgroundColor = new BaseColor(226, 226, 226)
                    //    };
                    //    bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    //}

                    if (i == 2)
                    {
                        bottomCell = new PdfPCell(new Phrase(SaleDeedOuterModel.TotalDocuments.ToString()))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    }

                    if (i == 3)
                    {
                        bottomCell = new PdfPCell(new Phrase(SaleDeedOuterModel.TotalStampDuty.ToString("F")))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                    }

                    if (i == 4)
                    {
                        bottomCell = new PdfPCell(new Phrase(SaleDeedOuterModel.TotalRegFee.ToString("F")))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;


                    }
                    if (i == 5)
                    {
                        bottomCell = new PdfPCell(new Phrase(SaleDeedOuterModel.TotalSum.ToString("F")))
                        {
                            BackgroundColor = new BaseColor(226, 226, 226)
                        };
                        bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;

                    }

                    table.AddCell(bottomCell);
                }

                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private PdfPTable ReportTableHavingMonthColumn(SaleDeedRevCollectionOuterModel SaleDeedOuterModel)
        //{

        //    string SerialNumber = "Serial Number";

        //    string DistrictName = "District Name";
        //    string SROName = "SRO Name";
        //    string DocumentsRegistered = "Documents Registered";
        //    string StumpDuty = "Stump Duty ( in Rs. )";
        //    string RegistrationFee = "Registration Fee ( in Rs. )";
        //    string Total = "Total ( in Rs. )";
        //    string MonthName = "Month Name";

        //    try
        //    {
        //        string[] col = { SerialNumber, DistrictName, SROName, MonthName, DocumentsRegistered, StumpDuty, RegistrationFee, Total };
        //        PdfPTable table = new PdfPTable(8)
        //        {
        //            WidthPercentage = 100
        //        };
        //        // table.DefaultCell.FixedHeight = 500f;

        //        string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
        //        string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
        //        BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        //        iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 14);

        //        //to repeat Headers
        //        table.HeaderRows = 1;
        //        // then set the column's __relative__ widths
        //        table.SetWidths(new Single[] { 4, 8, 5, 4,4, 6, 7, 5 });
        //        /*
        //        * by default tables 'collapse' on surrounding elements,
        //        * so you need to explicitly add spacing
        //        */
        //        //table.SpacingBefore = 10;

        //        // PdfPCell cell = null;
        //        PdfPCell cell1 = null;
        //        PdfPCell cell2 = null;
        //        PdfPCell cell3 = null;
        //        PdfPCell cell4 = null;
        //        PdfPCell cell5 = null;
        //        PdfPCell cell6 = null;
        //        PdfPCell cell7 = null;
        //        PdfPCell cell8 = null;

        //        for (int i = 0; i < col.Length; ++i)
        //        {
        //            PdfPCell cell = new PdfPCell(new Phrase(col[i]))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            table.AddCell(cell);

        //        }
              
        //        foreach (var items in SaleDeedOuterModel.SaleDeedRevCollList)
        //        {


        //            cell1 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell1.BackgroundColor = BaseColor.WHITE;
        //            cell2 = new PdfPCell(new Phrase(items.DistrictName, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell2.BackgroundColor = BaseColor.WHITE;

        //            cell3 = new PdfPCell(new Phrase(items.SROName, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell3.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell3.BackgroundColor = BaseColor.WHITE;



        //            cell4 = new PdfPCell(new Phrase(items.MonthName.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell4.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell4.BackgroundColor = BaseColor.WHITE;

        //            cell5 = new PdfPCell(new Phrase(items.DocumentsRegistered.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell5.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell5.BackgroundColor = BaseColor.WHITE;



        //            cell6 = new PdfPCell(new Phrase(items.StampDuty.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell6.BackgroundColor = BaseColor.WHITE;

        //            cell7 = new PdfPCell(new Phrase(items.RegistrationFee.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell7.BackgroundColor = BaseColor.WHITE;

        //            cell7.HorizontalAlignment = Element.ALIGN_RIGHT;


        //            cell8 = new PdfPCell(new Phrase(items.Total.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell8.BackgroundColor = BaseColor.WHITE;

        //            cell8.HorizontalAlignment = Element.ALIGN_RIGHT;


        //            table.AddCell(cell1);
        //            table.AddCell(cell2);
        //            table.AddCell(cell3);
        //            table.AddCell(cell4);
        //            table.AddCell(cell5);
        //            table.AddCell(cell6);
        //            table.AddCell(cell7);
        //            table.AddCell(cell8);


        //        }

        //        PdfPCell bottomCell = null;
        //        for (int i = 0; i < col.Length; ++i)
        //        {

        //            if (i == 0)
        //            {
        //                bottomCell = new PdfPCell(new Phrase(""))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            }
        //            if (i == 1)
        //            {
        //                bottomCell = new PdfPCell(new Phrase(""))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            }
        //            if (i == 2)
        //            {
        //                bottomCell = new PdfPCell(new Phrase("Total"))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            }

        //            if (i == 3)
        //            {
        //                bottomCell = new PdfPCell(new Phrase(SaleDeedOuterModel.TotalDocuments))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                bottomCell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            }

        //            if (i == 4)
        //            {
        //                bottomCell = new PdfPCell(new Phrase(SaleDeedOuterModel.TotalStampDuty.ToString("F")))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            }

        //            if (i == 5)
        //            {
        //                bottomCell = new PdfPCell(new Phrase(SaleDeedOuterModel.TotalRegFee.ToString("F")))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;


        //            }
        //            if (i == 6)
        //            {
        //                bottomCell = new PdfPCell(new Phrase(SaleDeedOuterModel.TotalSum.ToString("F")))
        //                {
        //                    BackgroundColor = new BaseColor(226, 226, 226)
        //                };
        //                bottomCell.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            }

        //            table.AddCell(bottomCell);
        //        }

        //        return table;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        //To add paging to IndexII report table in PDF
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
        /// Export To Excel
        /// </summary>
        /// <param name="SROOfficeListID"></param>
        /// <param name="DROfficeID"></param>
        /// <param name="FinancialID"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Sale Deed Rev Export To Excel")]
        public ActionResult ExportToExcel(string SROOfficeListID, string DROfficeID, string FinancialID,string SelectedDistrict,string SelectedSRO,string MaxDate,string PropertyValueID,string PropertyTypeID)
        {
            try
            {
                caller = new ServiceCaller("SaleDeedRevCollectionAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("SaleDeedRegisteredandRevenueCollected.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;



                SaleDeedRevCollectionModel model = new SaleDeedRevCollectionModel
                {

                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    DROfficeID = Convert.ToInt32(DROfficeID),
                    FinacialYearID = FinancialID,
                    PropertyTypeID=Convert.ToInt32(PropertyTypeID),
                    PropertyValueID = Convert.ToInt32(PropertyValueID),
                    startLen = 0,
                    totalNum = 10
                };

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                List<SaleDeedRevCollectionDetail> objListItemsToBeExported = new List<SaleDeedRevCollectionDetail>();

                caller = new ServiceCaller("SaleDeedRevCollectionAPIController");
                caller.HttpClient.Timeout = objTimeSpan;
                int totalCount = caller.PostCall<SaleDeedRevCollectionModel, int>("GetSaleDeedRevCollectionDetailsTotalCount", model);
                model.totalNum = totalCount;
                SaleDeedRevCollectionOuterModel saleDeedRevCollectionOuterModel = new SaleDeedRevCollectionOuterModel();
                 saleDeedRevCollectionOuterModel = caller.PostCall<SaleDeedRevCollectionModel, SaleDeedRevCollectionOuterModel>("GetSaleDeedRevCollectionDetails", model, out errorMessage);
                objListItemsToBeExported = saleDeedRevCollectionOuterModel.SaleDeedRevCollList;

                if (objListItemsToBeExported == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);

                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Sale Deed Registered and Revenue Collected ("+ saleDeedRevCollectionOuterModel.FinancialYear +")");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = string.Empty;
                //if (saleDeedRevCollectionOuterModel.SROCode == 0)
                //{
                //     createdExcelPath = CreateExcel(saleDeedRevCollectionOuterModel, fileName, excelHeader,SelectedDistrict,SelectedSRO, MaxDate);


                //}
                //else
                //{
                //     createdExcelPath = CreateExcelHavingMonthColumn(saleDeedRevCollectionOuterModel, fileName, excelHeader, SelectedDistrict, SelectedSRO,MaxDate);

                //}
                createdExcelPath = CreateExcel(saleDeedRevCollectionOuterModel, fileName, excelHeader, SelectedDistrict, SelectedSRO, MaxDate);


                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "SaleDeedRegisteredandRevenueCollected_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateExcel(SaleDeedRevCollectionOuterModel SaleDeedOuterModel, string fileName, string excelHeader,string SelectedDistrict,string SelectedSRO,string MaxDate)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("SRO Document Cash Collection Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now ;
                    workSheet.Cells[5, 1].Value = "Total Records : " + SaleDeedOuterModel.SaleDeedRevCollList.Count()+ "                                                                                                                                                               Note : This report is based on pre processed data considered upto : "+MaxDate;
                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[2, 1, 2, 7].Merge = true;
                    workSheet.Cells[3, 1, 3, 7].Merge = true;
                    workSheet.Cells[4, 1, 4, 7].Merge = true;
                    workSheet.Cells[5, 1, 5, 7].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 50;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;


                    int rowIndex = 8;

                    workSheet.Cells[7, 1].Value = "Serial Number";
                    workSheet.Cells[7, 2].Value = "Month";
                    //workSheet.Cells[7, 2].Value = "District Name";
                    //workSheet.Cells[7, 3].Value = "SRO Name";
                    workSheet.Cells[7, 3].Value = "Documents Registered";
                    workSheet.Cells[7, 4].Value = "Stamp Duty ( in Rs. )";
                    workSheet.Cells[7, 5].Value = "Registration Fee ( in Rs. )";
                    workSheet.Cells[7, 6].Value = "Total ( in Rs. )";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    foreach (var items in SaleDeedOuterModel.SaleDeedRevCollList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        //workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        
                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        //workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                        workSheet.Cells[rowIndex, 2].Value = items.MonthName;
                        //workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 3].Value = items.DocumentsRegistered;
                        workSheet.Cells[rowIndex, 4].Value = items.StampDuty;
                        workSheet.Cells[rowIndex, 5].Value = items.RegistrationFee;
                        workSheet.Cells[rowIndex, 6].Value = items.Total;

                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";


                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;


                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;
                      
                    }


                    workSheet.Row(rowIndex).Style.Font.Bold = true;

                    workSheet.Cells[rowIndex, 2].Value = "Total";
                    workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[rowIndex, 3].Value = SaleDeedOuterModel.TotalDocuments;
                    workSheet.Cells[rowIndex, 4].Value =SaleDeedOuterModel.TotalStampDuty;
                    workSheet.Cells[rowIndex, 5].Value =SaleDeedOuterModel.TotalRegFee;
                    workSheet.Cells[rowIndex, 6].Value = SaleDeedOuterModel.TotalSum;
                    workSheet.Row(rowIndex).Style.Font.Bold = true;

                    workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                    workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";

                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";


                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex), 6])
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

        //private string CreateExcelHavingMonthColumn(SaleDeedRevCollectionOuterModel SaleDeedOuterModel, string fileName, string excelHeader,string SelectedDistrict,string SelectedSRO,string MaxDate)
        //{
        //    string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //    FileInfo templateFile = GetFileInfo(ExcelFilePath);
        //    try
        //    {
        //        //create a new ExcelPackage
        //        using (ExcelPackage package = new ExcelPackage())
        //        {
        //            var workbook = package.Workbook;
        //            var workSheet = package.Workbook.Worksheets.Add("SRO Document Cash Collection Report");
        //            workSheet.Cells.Style.Font.Size = 14;

        //            workSheet.Cells[1, 1].Value = excelHeader;
        //            workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
        //            workSheet.Cells[3, 1].Value = "SRO : "+ SelectedSRO;
        //            workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
        //            //  workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
        //            workSheet.Cells[5, 1].Value = "Total Records : " + SaleDeedOuterModel.SaleDeedRevCollList.Count() + "                                                                                                                                                             Note : This report is based on pre processed data considered upto : " + MaxDate;

        //            workSheet.Cells[1, 1, 1, 7].Merge = true;
        //            workSheet.Cells[2, 1, 2, 7].Merge = true;
        //            workSheet.Cells[3, 1, 3, 7].Merge = true;
        //            workSheet.Cells[4, 1, 4, 7].Merge = true;
        //            workSheet.Cells[5, 1, 5, 7].Merge = true;

        //            workSheet.Column(6).Style.WrapText = true;
        //            workSheet.Column(7).Style.WrapText = true;
        //            workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //            workSheet.Column(1).Width = 30;
        //            workSheet.Column(2).Width = 50;
        //            workSheet.Column(3).Width = 30;
        //            workSheet.Column(4).Width = 30;
        //            workSheet.Column(5).Width = 30;
        //            workSheet.Column(6).Width = 30;
        //            workSheet.Column(7).Width = 30;
        //            workSheet.Column(8).Width = 30;



        //            workSheet.Row(1).Style.Font.Bold = true;
        //            workSheet.Row(2).Style.Font.Bold = true;
        //            workSheet.Row(3).Style.Font.Bold = true;
        //            workSheet.Row(4).Style.Font.Bold = true;
        //            workSheet.Row(5).Style.Font.Bold = true;
        //            workSheet.Row(7).Style.Font.Bold = true;


        //            int rowIndex = 8;
        //            workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //            workSheet.Cells[7, 1].Value = "Serial Number";
        //            workSheet.Cells[7, 2].Value = "District Name";
        //            workSheet.Cells[7, 3].Value = "SRO Name";
        //            workSheet.Cells[7, 4].Value = "Month Name";
        //            workSheet.Cells[7, 5].Value = "Documents Registered";
        //            workSheet.Cells[7, 6].Value = "Stump Duty ( in Rs. )";
        //            workSheet.Cells[7, 7].Value = "Registration Fee ( in Rs. )";
        //            workSheet.Cells[7, 8].Value = "Total ( in Rs. )";

        //            workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
        //            workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
        //            workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
        //            workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
        //            workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
        //            workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

        //            foreach (var items in SaleDeedOuterModel.SaleDeedRevCollList)
        //            {
        //                workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";

        //                workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
        //                workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
        //                workSheet.Cells[rowIndex, 3].Value = items.SROName;
        //                workSheet.Cells[rowIndex, 4].Value = items.MonthName;
        //                workSheet.Cells[rowIndex, 5].Value = items.DocumentsRegistered;
        //                workSheet.Cells[rowIndex, 6].Value = items.StampDuty;
        //                workSheet.Cells[rowIndex, 7].Value = items.RegistrationFee;
        //                workSheet.Cells[rowIndex, 8].Value = items.Total;
        //                workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
        //                workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
        //                workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";

        //                workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
        //                workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
        //                workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
        //                workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
        //                rowIndex++;

        //            }


        //            workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";

        //            workSheet.Cells[rowIndex, 4].Value = "Total";
        //            workSheet.Cells[rowIndex, 5].Value = SaleDeedOuterModel.TotalDocuments;
        //            workSheet.Cells[rowIndex, 6].Value = SaleDeedOuterModel.TotalStampDuty;
        //            workSheet.Cells[rowIndex, 7].Value = SaleDeedOuterModel.TotalRegFee;
        //            workSheet.Cells[rowIndex, 8].Value = SaleDeedOuterModel.TotalSum;
        //            workSheet.Row(rowIndex).Style.Font.Bold = true;

        //            workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
        //            workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
        //            workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";



        //            using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex), 8])
        //            {
        //                Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //            }


        //            package.SaveAs(templateFile);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return ExcelFilePath;
        //}


        /// <summary>
        /// GetFileInfo
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