using CustomModels.Models.MISReports.IncomeTaxReport;
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
    [KaveriAuthorization]
    public class IncomeTaxReportController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        [MenuHighlight]
        [HttpGet]
        public ActionResult IncomeTaxReportView()
        {

            int OfficeID = KaveriSession.Current.OfficeID;
            caller = new ServiceCaller("IncomeTaxReportAPIController");
            IncomeTaxReportResponseModel reqModel = caller.GetCall<IncomeTaxReportResponseModel>("IncomeTaxReportView", new { officeID = OfficeID });
            reqModel.DROName = " ";
            reqModel.SROName = " ";
            reqModel.FinYearName = " ";
            return View(reqModel);
        }

        [HttpPost]
        [EventAuditLogFilter(Description = "Get Income Tax Report Details")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetIncomeTaxReportDetails(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects

                string SROfficeID = formCollection["SROfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                string FinYearID = formCollection["FinYearID"];

                int SroId = Convert.ToInt32(SROfficeID);
                int DroId = Convert.ToInt32(DROfficeID);
                int FinID = Convert.ToInt32(FinYearID);


                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion


                #region Server Side Validation              

                caller = new ServiceCaller("CommonsApiController");
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

                //Validation For DR Login
                if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                {
                    //Validation for DR when user do not select any sro which is by default "Select"
                    if ((SroId == 0))
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                else
                {//Validations of Logins other than SR and DR

                    if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any District"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                    else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any SRO"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                }

                if (FinID == 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select any Financial Year"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                #endregion


                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);

                IncomeTaxReportResultModel incomeTaxReportResultModel = new IncomeTaxReportResultModel();
                IncomeTaxReportResponseModel reqModel = new IncomeTaxReportResponseModel();


                reqModel.SROfficeListID = Convert.ToInt32(SROfficeID);
                reqModel.DROfficeListID = Convert.ToInt32(DROfficeID);
                reqModel.FinYearListID = Convert.ToInt32(FinYearID);
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;


                caller = new ServiceCaller("IncomeTaxReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                //To get total count of records in IncomeTax report datatable
                int totalCount = caller.PostCall<IncomeTaxReportResponseModel, int>("GetIncomeTaxReportDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //To get records of IncomeTax report table 
                incomeTaxReportResultModel = caller.PostCall<IncomeTaxReportResponseModel, IncomeTaxReportResultModel>("GetIncomeTaxReportDetails", reqModel, out errorMessage);

                IEnumerable<IncomeTaxReportDetailsModel> result = incomeTaxReportResultModel.incomeTaxReportDetailsList;



                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Income Tax Report Details" });
                }
                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;


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
                        result = result.Where(m => m.PersonName.ToLower().Contains(searchValue.ToLower()) ||
                        m.FathersName.ToLower().Contains(searchValue.ToLower()) ||
                        m.AreaLocality.ToLower().Contains(searchValue.ToLower()) ||
                        m.TransactionID.ToLower().Contains(searchValue.ToLower()));
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(IncomeTaxReportDetailsModel => new
                {
                    ReportSrNo = IncomeTaxReportDetailsModel.ReportSrNo,
                    OriginalReportSrNo = IncomeTaxReportDetailsModel.OriginalReportSrNo,
                    CustomerID = IncomeTaxReportDetailsModel.CustomerId,
                    PersonName = IncomeTaxReportDetailsModel.PersonName,
                    DateOfBirth = IncomeTaxReportDetailsModel.DateOfBirth,
                    FatherName = IncomeTaxReportDetailsModel.FathersName,
                    PanAckNo = IncomeTaxReportDetailsModel.PanAckNo,
                    AadharNumber = IncomeTaxReportDetailsModel.AadharNo,
                    IdentificationType = IncomeTaxReportDetailsModel.IdentificationType,
                    IdentificationNumber = IncomeTaxReportDetailsModel.IdentificationNumber,
                    FlatDoorBuilding = IncomeTaxReportDetailsModel.FlatDoorBuilding,
                    NameOfPrem = IncomeTaxReportDetailsModel.NameOfPremises,
                    RoadStreet = IncomeTaxReportDetailsModel.RoadStreet,
                    AreaLocality = IncomeTaxReportDetailsModel.AreaLocality,
                    CityTown = IncomeTaxReportDetailsModel.CityTown,
                    PostalCode = IncomeTaxReportDetailsModel.PostalCode,
                    StateCode = IncomeTaxReportDetailsModel.StateCode,
                    CountryCode = IncomeTaxReportDetailsModel.CountryCode,
                    MobileNo = IncomeTaxReportDetailsModel.MobileNo,
                    STDCode = IncomeTaxReportDetailsModel.StdCode,
                    TelephoneNo = IncomeTaxReportDetailsModel.TelephoneNo,
                    AgriIncome = IncomeTaxReportDetailsModel.EstimatedAgriIncome.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    NonAgriIncome = IncomeTaxReportDetailsModel.EstimatedNonAgriIncome.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Remarks = IncomeTaxReportDetailsModel.Remarks,
                    Form60AckNo = IncomeTaxReportDetailsModel.Form60AckNo,
                    TransactionDate = IncomeTaxReportDetailsModel.TransactionDate,
                    TransactionID = IncomeTaxReportDetailsModel.TransactionID,
                    TransactionType = IncomeTaxReportDetailsModel.TransactionType,
                    TransactionAmount = IncomeTaxReportDetailsModel.TransactionAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    TransactionMode = IncomeTaxReportDetailsModel.TransactionMode,
                    //SROName = incomeTaxReportResultModel.SROName,
                    //DROName = incomeTaxReportResultModel.DROName,

                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DROfficeID + "','" + SROfficeID + "','" + FinYearID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' id='PDFDownloadBtn' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROfficeID + "','" + FinYearID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";

                var JsonData = Json(new
                {

                });

                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        //DroName = incomeTaxReportResultModel.DROName,
                        //SroName = incomeTaxReportResultModel.SROName,
                        //FYName = incomeTaxReportResultModel.FinYearName,
                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        DroName = incomeTaxReportResultModel.DROName,
                        SroName = incomeTaxReportResultModel.SROName,
                        FYName = incomeTaxReportResultModel.FinYearName,

                        PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                    });
                }



                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Income Tax Reports Details" });
            }
        }


        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }



        [EventAuditLogFilter(Description = "Export Income Tax Report To PDF")]
        public ActionResult ExportIncomeTaxReportToPDF(string DROfficeID, string SROfficeID, string FinYearID)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                IncomeTaxReportResponseModel model = new IncomeTaxReportResponseModel
                {
                    SROfficeListID = Convert.ToInt32(SROfficeID),
                    DROfficeListID = Convert.ToInt32(DROfficeID),
                    FinYearListID = Convert.ToInt32(FinYearID),
                    startLen = 0,
                    totalNum = 10,
                };

                //List<IncomeTaxReportDetailsModel> objListItemsToBeExported = new List<IncomeTaxReportDetailsModel>();
                IncomeTaxReportResultModel incomeTaxReportResultModel = new IncomeTaxReportResultModel();
                model.IsPdf = true;
                caller = new ServiceCaller("IncomeTaxReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in IncomeTax report datatable
                int totalCount = caller.PostCall<IncomeTaxReportResponseModel, int>("GetIncomeTaxReportDetailsTotalCount", model);
                model.totalNum = totalCount;

                // To get total records of IncomeTax report table
                incomeTaxReportResultModel = caller.PostCall<IncomeTaxReportResponseModel, IncomeTaxReportResultModel>("GetIncomeTaxReportDetails", model, out errorMessage);

                if (incomeTaxReportResultModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }

                //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
                string fileName = string.Format("IncomeTaxReportPDF.pdf");
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("Income Tax Report ({0})", FinYearID);

                //To get SRONAME
                string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROfficeID });

                //Create Temp PDF File
                byte[] pdfBytes = CreatePDFFile(incomeTaxReportResultModel.incomeTaxReportDetailsList, fileName, pdfHeader, SROName);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "IncomeTaxReportPDF_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
                //return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "IncomeTaxReportPDF_" + DateTime.Now.ToString("dd-MM-yyyy-hh_mm_ss") + ".pdf");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
            }
        }


        private byte[] CreatePDFFile(List<IncomeTaxReportDetailsModel> incomeTaxReportDetailsList, string fileName, string pdfHeader, string SROName)
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
                            var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);
                            var SroName = new Chunk(SROName + "       ", redListTextFont);
                            var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
                            string count = (incomeTaxReportDetailsList.Count()).ToString();
                            var countChunk = new Chunk(count, redListTextFont);

                            var titlePhrase = new Phrase(titleChunk)
                        {
                            descriptionChunk
                        };
                            var totalPhrase = new Phrase(totalChunk)
                        {
                            countChunk
                        };
                            var SroNamePhrase = new Phrase(SroNameChunk)
                        {
                            SroName
                        };
                            doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(titlePhrase);
                            doc.Add(SroNamePhrase);
                            doc.Add(totalPhrase);
                            doc.Add(addSpace);

                            doc.Add(IncomeTaxReportTable(incomeTaxReportDetailsList));
                            doc.Close();

                            pdfBytes = AddpageNumber(ms.ToArray());
                        }
                    }
                }
                return pdfBytes;
            }
            catch (Exception ex)
            {
                throw;
            }
        }



        private PdfPTable IncomeTaxReportTable(List<IncomeTaxReportDetailsModel> incomeTaxReportDetailsList)
        {

            string ReportSrNo = "Report Serial Number";
            string OriginalReportSrNo = "Original Report Serial Number";
            string CustomerID = "Customer ID";
            string PersonName = "Person Name";
            string DateOfBirth = "Date of birth/ Incorporation";
            string FatherName = "Father's Name(for individuals)";
            string PanAckNo = "PAN Acknowledgement Number";
            string AadharNumber = "Aadhaar Number";
            string IdentificationType = "Identification Type";
            string IdentificationNumber = "Identification Number";
            string FlatDoorBuilding = "Flat / Door / Building";
            string NameOfPrem = "Name Of Premises / Building / Village";
            string RoadStreet = "Road / Street";
            string AreaLocality = "Area / Locality";
            string CityTown = "City / Town";
            string PostalCode = "Postal Code";
            string StateCode = "State Code";
            string CountryCode = "Country Code";
            string MobileNo = "Mobile Number";
            string STDCode = "STD Code";
            string TelephoneNo = "Telephone Number";
            string AgriIncome = "Estimated agricultural income";
            string NonAgriIncome = "Estimated non-agricultural income";
            string Remarks = "Remarks";
            string Form60AckNo = "Form 60 Acknowledgement Number";
            string TransactionDate = "Transaction Date";
            string TransactionID = "Transaction ID";
            string TransactionType = "Transaction Type";
            string TransactionAmount = "Transaction Amount";
            string TransactionMode = "Transaction Mode";

            try
            {
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                string[] col = { ReportSrNo, OriginalReportSrNo, CustomerID, PersonName, DateOfBirth, FatherName, PanAckNo, AadharNumber, IdentificationType, IdentificationNumber, FlatDoorBuilding, NameOfPrem, RoadStreet, AreaLocality, CityTown, PostalCode, StateCode, CountryCode, MobileNo, STDCode, TelephoneNo, AgriIncome, NonAgriIncome, Remarks, Form60AckNo, TransactionDate, TransactionID, TransactionType, TransactionAmount, TransactionMode };
                PdfPTable table = new PdfPTable(30)
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
                //table.SetWidths(new Single[] { 23,1,1,1,1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1 });
                //table.SetWidths(new Single[] { 5, 4, 4, 3, 3, 8, 8, 5, 5, 4, 3, 6 });
                //table.SetWidths(new Single[] { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 });
                //table.SetWidths(new Single[] { 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2  });
                //table.SetWidths(new float[] { 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 0.75f, 9.75f,0.75f,0.75f });



                /*
                * by default tables 'collapse' on surrounding elements,
                * so you need to explicitly add spacing
                */
                //table.SpacingBefore = 10;
                PdfPCell cell = null;
                for (int i = 0; i < col.Length; ++i)
                {
                    cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };

                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                }

                //int i = 1;

                foreach (var items in incomeTaxReportDetailsList)
                {


                    cell = new PdfPCell(new Phrase(items.ReportSrNo.ToString(), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.OriginalReportSrNo.ToString(), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.CustomerId.ToString(), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.PersonName, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.DateOfBirth, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);


                    cell = new PdfPCell(new Phrase(items.FathersName, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.PanAckNo, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.AadharNo, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.IdentificationType, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.IdentificationNumber, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.FlatDoorBuilding, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.NameOfPremises, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.RoadStreet, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.AreaLocality, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.CityTown, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.PostalCode, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.StateCode, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.CountryCode.ToString(), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.MobileNo, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.StdCode, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.TelephoneNo, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.EstimatedAgriIncome.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.EstimatedNonAgriIncome.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.Remarks, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.Form60AckNo, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.TransactionDate, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.TransactionID, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.TransactionType, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.TransactionAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.TransactionMode, tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell.Rotation = 90;
                    table.AddCell(cell);



                    //table.AddCell(new Phrase(items.marketvalue, tableContentFont));
                    //table.AddCell(new Phrase(items.consideration, tableContentFont));
                    //cell1 = new PdfPCell(new Phrase(items.marketvalue.ToString("F"), tableContentFont))
                    //{
                    //    BackgroundColor = new BaseColor(204, 255, 255)
                    //};
                    //cell1.BackgroundColor = BaseColor.WHITE;

                    //cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //cell2 = new PdfPCell(new Phrase(items.consideration.ToString("F"), tableContentFont))
                    //{
                    //    BackgroundColor = new BaseColor(204, 255, 255)
                    //};
                    //cell2.BackgroundColor = BaseColor.WHITE;

                    //cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //table.AddCell(cell1);
                    //table.AddCell(cell2);

                }
                return table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


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


        [EventAuditLogFilter(Description = "Export Income Tax Report To Excel")]
        public ActionResult ExportIncomeTaxReportToExcel(string DROfficeID, string SROfficeID, string FinYearID)
        {
            try
            {
                caller = new ServiceCaller("IncomeTaxReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("IncomeTaxReportExcel.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;


                IncomeTaxReportResponseModel model = new IncomeTaxReportResponseModel
                {
                    SROfficeListID = Convert.ToInt32(SROfficeID),
                    DROfficeListID = Convert.ToInt32(DROfficeID),
                    FinYearListID = Convert.ToInt32(FinYearID),
                    startLen = 0,
                    totalNum = 10,
                };
                model.IsExcel = true;
                string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROfficeID }, out errorMessage);
                if (SROName == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }

                //List<IncomeTaxReportDetailsModel> objListItemsToBeExported = new List<IncomeTaxReportDetailsModel>();

                IncomeTaxReportResultModel incomeTaxReportResultModel = new IncomeTaxReportResultModel();

                caller = new ServiceCaller("IncomeTaxReportAPIController");
                TimeSpan objTimeSpan2 = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan2;
                int totalCount = caller.PostCall<IncomeTaxReportResponseModel, int>("GetIncomeTaxReportDetailsTotalCount", model);
                model.totalNum = totalCount;
                incomeTaxReportResultModel = caller.PostCall<IncomeTaxReportResponseModel, IncomeTaxReportResultModel>("GetIncomeTaxReportDetails", model, out errorMessage);

                if (incomeTaxReportResultModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }



                string excelHeader = string.Format("Income Tax Report ({0})", FinYearID);
                string createdExcelPath = CreateExcel(incomeTaxReportResultModel.incomeTaxReportDetailsList, fileName, excelHeader, SROName);

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                //return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "IncomeTaxReportExcel" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "IncomeTaxReportExcel" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }


        private string CreateExcel(List<IncomeTaxReportDetailsModel> incomeTaxReportDetailsList, string fileName, string excelHeader, string SROName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Income Tax Report Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (incomeTaxReportDetailsList.Count());
                    workSheet.Cells[1, 1, 1, 31].Merge = true;
                    workSheet.Cells[2, 1, 2, 31].Merge = true;
                    workSheet.Cells[3, 1, 3, 31].Merge = true;
                    workSheet.Cells[4, 1, 4, 31].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    workSheet.Column(1).Width = 25;  //Report SR No
                    workSheet.Column(2).Width = 35;  //Original Sr No
                    workSheet.Column(3).Width = 20;  //CustomerID
                    workSheet.Column(4).Width = 40;  //PersonName
                    workSheet.Column(5).Width = 34;  //Date of Birth
                    workSheet.Column(6).Width = 40;  //Fathers Name
                    workSheet.Column(7).Width = 38;  //Pan Ack No
                    workSheet.Column(8).Width = 25;  //Aadhar No
                    workSheet.Column(9).Width = 25; // Identification Type
                    workSheet.Column(10).Width = 27; // identification No
                    workSheet.Column(11).Width = 25; //Flat Door Building
                    workSheet.Column(12).Width = 43; //Name of Prem
                    workSheet.Column(13).Width = 25; //Road Street
                    workSheet.Column(14).Width = 30; //Area Locality
                    workSheet.Column(15).Width = 20; //City Town
                    workSheet.Column(16).Width = 20; //Postal Code
                    workSheet.Column(17).Width = 20; //State Code
                    workSheet.Column(18).Width = 20; //Country Code
                    workSheet.Column(19).Width = 25; //Mobile No
                    workSheet.Column(20).Width = 23; //STD Code
                    workSheet.Column(21).Width = 25; //Telephone No
                    workSheet.Column(22).Width = 38; //Agri Income
                    workSheet.Column(23).Width = 40; //NonAgri Income
                    workSheet.Column(24).Width = 25; //Remarks
                    workSheet.Column(25).Width = 40; //Form60 Ack No
                    workSheet.Column(26).Width = 25; //Transaction Date
                    workSheet.Column(27).Width = 25; //Transaction ID
                    workSheet.Column(28).Width = 25; //Transaction Type
                    workSheet.Column(29).Width = 25; //Transaction Amount
                    workSheet.Column(30).Width = 25; //Transaction Mode


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;


                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    workSheet.Cells[6, 1].Value = "Report Serial Number";
                    workSheet.Cells[6, 2].Value = "Original Report Serial Number";
                    workSheet.Cells[6, 3].Value = "Customer ID";
                    workSheet.Cells[6, 4].Value = "Person Name";
                    workSheet.Cells[6, 5].Value = "Date of birth/ Incorporation";
                    workSheet.Cells[6, 6].Value = "Father's Name(for individuals)";
                    workSheet.Cells[6, 7].Value = "PAN Acknowledgement Number";
                    workSheet.Cells[6, 8].Value = "Aadhaar Number";
                    workSheet.Cells[6, 9].Value = "Identification Type";
                    workSheet.Cells[6, 10].Value = "Identification Number";
                    workSheet.Cells[6, 11].Value = "Flat / Door / Building";
                    workSheet.Cells[6, 12].Value = "Name Of Premises / Building / Village";
                    workSheet.Cells[6, 13].Value = "Road / Street";
                    workSheet.Cells[6, 14].Value = "Area / Locality";
                    workSheet.Cells[6, 15].Value = "City / Town";
                    workSheet.Cells[6, 16].Value = "Postal Code";
                    workSheet.Cells[6, 17].Value = "State Code";
                    workSheet.Cells[6, 18].Value = "Country Code";
                    workSheet.Cells[6, 19].Value = "Mobile Number";
                    workSheet.Cells[6, 20].Value = "STD Code";
                    workSheet.Cells[6, 21].Value = "Telephone Number";
                    workSheet.Cells[6, 22].Value = "Estimated agricultural income";
                    workSheet.Cells[6, 23].Value = "Estimated non-agricultural income";
                    workSheet.Cells[6, 24].Value = "Remarks";
                    workSheet.Cells[6, 25].Value = "Form 60 Acknowledgement Number";
                    workSheet.Cells[6, 26].Value = "Transaction Date";
                    workSheet.Cells[6, 27].Value = "Transaction ID";
                    workSheet.Cells[6, 28].Value = "Transaction Type";
                    workSheet.Cells[6, 29].Value = "Transaction Amount";
                    workSheet.Cells[6, 30].Value = "Transaction Mode";


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";


                    foreach (var items in incomeTaxReportDetailsList)
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
                        workSheet.Cells[rowIndex, 28].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 29].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 30].Style.Font.Name = "KNB-TTUmaEN";


                        workSheet.Cells[rowIndex, 22].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 23].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 29].Style.Numberformat.Format = "0.00";



                        workSheet.Cells[rowIndex, 1].Value = items.ReportSrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.OriginalReportSrNo;
                        workSheet.Cells[rowIndex, 3].Value = items.CustomerId;
                        workSheet.Cells[rowIndex, 4].Value = items.PersonName;
                        workSheet.Cells[rowIndex, 5].Value = items.DateOfBirth;
                        workSheet.Cells[rowIndex, 6].Value = items.FathersName;
                        workSheet.Cells[rowIndex, 7].Value = items.PanAckNo;
                        workSheet.Cells[rowIndex, 8].Value = items.AadharNo;
                        workSheet.Cells[rowIndex, 9].Value = items.IdentificationType;
                        workSheet.Cells[rowIndex, 10].Value = items.IdentificationNumber;
                        workSheet.Cells[rowIndex, 11].Value = items.FlatDoorBuilding;
                        workSheet.Cells[rowIndex, 12].Value = items.NameOfPremises;
                        workSheet.Cells[rowIndex, 13].Value = items.RoadStreet;
                        workSheet.Cells[rowIndex, 14].Value = items.AreaLocality;
                        workSheet.Cells[rowIndex, 15].Value = items.CityTown;
                        workSheet.Cells[rowIndex, 16].Value = items.PostalCode;
                        workSheet.Cells[rowIndex, 17].Value = items.StateCode;
                        workSheet.Cells[rowIndex, 18].Value = items.CountryCode;
                        workSheet.Cells[rowIndex, 19].Value = items.MobileNo;
                        workSheet.Cells[rowIndex, 20].Value = items.StdCode;
                        workSheet.Cells[rowIndex, 21].Value = items.TelephoneNo;
                        workSheet.Cells[rowIndex, 22].Value = items.EstimatedAgriIncome.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        workSheet.Cells[rowIndex, 23].Value = items.EstimatedNonAgriIncome.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        workSheet.Cells[rowIndex, 24].Value = items.Remarks;
                        workSheet.Cells[rowIndex, 25].Value = items.Form60AckNo;
                        workSheet.Cells[rowIndex, 26].Value = items.TransactionDate;
                        workSheet.Cells[rowIndex, 27].Value = items.TransactionID;
                        workSheet.Cells[rowIndex, 28].Value = items.TransactionType;
                        workSheet.Cells[rowIndex, 29].Value = items.TransactionAmount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"));
                        workSheet.Cells[rowIndex, 30].Value = items.TransactionMode;




                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                        workSheet.Cells[rowIndex, 22].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 22].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 23].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 23].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        workSheet.Cells[rowIndex, 29].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 29].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;


                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    //workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 30])
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