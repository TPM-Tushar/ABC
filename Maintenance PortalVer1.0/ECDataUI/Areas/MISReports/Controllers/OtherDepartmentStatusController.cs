using CustomModels.Models.MISReports.OtherDepartmentStatus;
using ECDataUI.Common;
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
    public class OtherDepartmentStatusController : Controller
    {
        ServiceCaller caller = null;
                
        public ActionResult OtherDepartmentStatusView()
        {
            KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.OtherDepartmentImport;
            caller = new ServiceCaller("OtherDepartmentStatusAPIController");
            int OfficeID = KaveriSession.Current.OfficeID;
            OtherDepartmentStatusModel reqModel = caller.GetCall<OtherDepartmentStatusModel>("OtherDepartmentStatusView", new { OfficeID = OfficeID });
            return View(reqModel);
        }

        [HttpPost]
        //[EventAuditLogFilter(Description = "Get Sale Deed Rev Collection Details")]
        //[ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult OtherDepartmentStatusDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("OtherDepartmentStatusAPIController");
            try
            {

                #region User Variables and Objects                               
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string IntegrationtypeID = formCollection["IntegrationtypeID"];
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];

                #region Server Side Validation           
                if (string.IsNullOrEmpty(SROOfficeListID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Select Any SRO."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(IntegrationtypeID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Select Any Integration type."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                int SROIDINT = Convert.ToInt32(SROOfficeListID);
                int IntegrationtypeIDINT = Convert.ToInt32(IntegrationtypeID);

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                

                if (SROIDINT < 0)
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

                if (IntegrationtypeIDINT < 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Select Any Integration type."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }


                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);

                int pageSize = totalNum;
                int skip = startLen;

                OtherDepartmentStatusModel reqModel = new OtherDepartmentStatusModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.SROfficeID = SROIDINT;
                reqModel.IntegrationtypeID = IntegrationtypeIDINT;


                int totalCount = caller.PostCall<OtherDepartmentStatusModel, int>("OtherDepartmentStatusDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                IEnumerable<OtherDepartmentStatusDetailsModel> result = caller.PostCall<OtherDepartmentStatusModel, List<OtherDepartmentStatusDetailsModel>>("OtherDepartmentStatusDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Other Department Status details." });
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
                        result = result.Where(
                            m =>    m.Column1.ToLower().Contains(searchValue.ToLower()) ||
                                    m.Column2.ToLower().Contains(searchValue.ToLower()) ||
                                    m.Column3.ToLower().Contains(searchValue.ToLower()) ||
                                    m.Column4.ToLower().Contains(searchValue.ToLower()) ||
                                    m.Column5.ToLower().Contains(searchValue.ToLower()) ||
                                    m.Column6.ToLower().Contains(searchValue.ToLower()) ||
                                    m.Column7.ToLower().Contains(searchValue.ToLower())
                        );
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(OtherDepartmentStatusDetailsModel => new
                {
                    SerialNo = OtherDepartmentStatusDetailsModel.SerialNo,
                    Column1 = OtherDepartmentStatusDetailsModel.Column1,
                    Column2 = OtherDepartmentStatusDetailsModel.Column2,
                    Column3= OtherDepartmentStatusDetailsModel.Column3,
                    Column4= OtherDepartmentStatusDetailsModel.Column4,
                    Column5= OtherDepartmentStatusDetailsModel.Column5,
                    Column6= OtherDepartmentStatusDetailsModel.Column6,
                    Column7 = OtherDepartmentStatusDetailsModel.Column7
                });

                
                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + SROOfficeListID + "','" + IntegrationtypeID + "','" + fromDate + "','" + ToDate + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + SROOfficeListID + "','" + IntegrationtypeID + "','" + fromDate + "','" + ToDate + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
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
                return Json(new { serverError = true, errorMessage = "Error occured while getting Other Department Status details." }, JsonRequestBehavior.AllowGet);
            }
        }

        #region Download report in PDF
      
        //[EventAuditLogFilter(Description = "Sale Deed Rev Export Report To PDF")]
        public ActionResult ExportReportToPDF(string SROOfficeListID,string IntegrationtypeID, string FromDate, string ToDate)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                OtherDepartmentStatusModel model = new OtherDepartmentStatusModel
                {
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    IntegrationtypeID = Convert.ToInt32(IntegrationtypeID),
                    FromDate = frmDate,
                    ToDate = toDate,
                    startLen = 0,
                    totalNum = 10
                };
                //model.Amount = Convert.ToInt32(Amount);

                List<OtherDepartmentStatusDetailsModel> objListItemsToBeExported = new List<OtherDepartmentStatusDetailsModel>();

                caller = new ServiceCaller("OtherDepartmentStatusAPIController");

                //To get total count of records in indexII report datatable
                int totalCount = caller.PostCall<OtherDepartmentStatusModel, int>("OtherDepartmentStatusDetailsTotalCount", model);
                model.totalNum = totalCount;

                // To get total records of indexII report table
                objListItemsToBeExported = caller.PostCall<OtherDepartmentStatusModel, List<OtherDepartmentStatusDetailsModel>>("OtherDepartmentStatusDetails", model, out errorMessage);

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }

                //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
                string fileName = string.Format("OtherDepartmentStatus.pdf");
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("Other Department Status");

                //To get SRONAME
                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                //Create Temp PDF File
                // byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader, SROName);
                byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "OtherDepartmentStatus_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");

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
        private byte[] CreatePDFFile(List<OtherDepartmentStatusDetailsModel> objListItemsToBeExported, string fileName, string pdfHeader)
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
                            // var SroName = new Chunk(SROName + "       ", redListTextFont);
                            var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
                            string count = objListItemsToBeExported.Count().ToString();
                            var countChunk = new Chunk(count, redListTextFont);

                            var titlePhrase = new Phrase(titleChunk)
                        {
                            descriptionChunk
                        };
                            var totalPhrase = new Phrase(totalChunk)
                        {
                            countChunk
                        };
                            //    var SroNamePhrase = new Phrase(SroNameChunk)
                            //{
                            //    SroName
                            //};
                            doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(titlePhrase);
                            //doc.Add(SroNamePhrase);
                            doc.Add(totalPhrase);
                            doc.Add(addSpace);

                            doc.Add(ReportTable(objListItemsToBeExported));
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
        private PdfPTable ReportTable(List<OtherDepartmentStatusDetailsModel> objListItemsToBeExported)
        {

            string SerialNumber = "Serial Number";
            string Column1 = "Column1";
            string Column2 = "Column2";
            string Column3 = "Column3";
            string Column4 = "Column4";
            string Column5 = "Column5";
            string Column6 = "Column6";
            string Column7 = "Column7";

            try
            {
                string[] col = { SerialNumber, Column1, Column2, Column3, Column4, Column5, Column6,Column7 };
                PdfPTable table = new PdfPTable(8)
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
                table.SetWidths(new Single[] { 4, 8, 5, 4, 6, 7, 5 ,5});
                /*
                * by default tables 'collapse' on surrounding elements,
                * so you need to explicitly add spacing
                */
                //table.SpacingBefore = 10;

                // PdfPCell cell = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;
                PdfPCell cell7 = null;
                PdfPCell cell8 = null;

                for (int i = 0; i < col.Length; ++i)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    table.AddCell(cell);

                }
             
                foreach (var items in objListItemsToBeExported)
                {


                    cell1 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;

                    cell2 = new PdfPCell(new Phrase(items.Column1, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(items.Column2, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.Column3, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell4.BackgroundColor = BaseColor.WHITE;
                                       
                    cell5 = new PdfPCell(new Phrase(items.Column4, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell5.BackgroundColor = BaseColor.WHITE;

                    cell6 = new PdfPCell(new Phrase(items.Column5, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell6.BackgroundColor = BaseColor.WHITE;
                    cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
                    
                    cell7 = new PdfPCell(new Phrase(items.Column6, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell7.BackgroundColor = BaseColor.WHITE;
                    cell7.HorizontalAlignment = Element.ALIGN_RIGHT;

                    cell8 = new PdfPCell(new Phrase(items.Column7, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell8.BackgroundColor = BaseColor.WHITE;
                    cell8.HorizontalAlignment = Element.ALIGN_RIGHT;

                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                    table.AddCell(cell6);
                    table.AddCell(cell7);
                    table.AddCell(cell8);

                }

                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

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
       
        //[EventAuditLogFilter(Description = "Sale Deed Rev Export To Excel")]
        public ActionResult ExportToExcel(string SROOfficeListID, string IntegrationtypeID, string FromDate, string ToDate)
        {
            try
            {
                caller = new ServiceCaller("OtherDepartmentStatusAPIController");
                string fileName = string.Format("SaleDeedRegisteredandRevenueCollected.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                OtherDepartmentStatusModel model = new OtherDepartmentStatusModel
                {
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    IntegrationtypeID = Convert.ToInt32(IntegrationtypeID),
                    FromDate = frmDate,
                    ToDate = toDate,
                    startLen = 0,
                    totalNum = 10
                };



                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                List<OtherDepartmentStatusDetailsModel> objListItemsToBeExported = new List<OtherDepartmentStatusDetailsModel>();

                caller = new ServiceCaller("OtherDepartmentStatusAPIController");
                int totalCount = caller.PostCall<OtherDepartmentStatusModel, int>("OtherDepartmentStatusDetailsTotalCount", model);
                model.totalNum = totalCount;
                objListItemsToBeExported = caller.PostCall<OtherDepartmentStatusModel, List<OtherDepartmentStatusDetailsModel>>("OtherDepartmentStatusDetails", model, out errorMessage);
                if (objListItemsToBeExported == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);

                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Other Department Status");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);


                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "OtherDepartmentStatus_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        private string CreateExcel(List<OtherDepartmentStatusDetailsModel> objListItemsToBeExported, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Other Department Status");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    //  workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + objListItemsToBeExported.Count();
                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[2, 1, 2, 7].Merge = true;
                    workSheet.Cells[3, 1, 3, 7].Merge = true;
                    workSheet.Cells[4, 1, 4, 7].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 50;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;


                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[6, 1].Value = "Serial Number";
                    workSheet.Cells[6, 2].Value = "Column 1";
                    workSheet.Cells[6, 3].Value = "Column 2";
                    workSheet.Cells[6, 4].Value = "Column 3";
                    workSheet.Cells[6, 5].Value = "Column 4";
                    workSheet.Cells[6, 6].Value = "Column 5";
                    workSheet.Cells[6, 7].Value = "Column 6";
                    workSheet.Cells[6, 8].Value = "Column 7";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in objListItemsToBeExported)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.Column1;
                        workSheet.Cells[rowIndex, 3].Value = items.Column2;
                        workSheet.Cells[rowIndex, 4].Value = items.Column3;
                        workSheet.Cells[rowIndex, 5].Value = items.Column4;
                        workSheet.Cells[rowIndex, 6].Value = items.Column5;
                        workSheet.Cells[rowIndex, 7].Value = items.Column6;
                        workSheet.Cells[rowIndex, 8].Value = items.Column7;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 7])
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