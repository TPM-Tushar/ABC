using CustomModels.Models.MISReports.ReScanningDetails;
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
    public class ReScanningDetailsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// ReScanning Summary Details View
        /// </summary>
        /// <returns></returns>
        public ActionResult ReScanningDetails()
        {
            try
            {

                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ReScanningDetails;
                caller = new ServiceCaller("ReScanningDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                ReScanningDetailsViewModel reqModel = caller.GetCall<ReScanningDetailsViewModel>("ReScanningDetails", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while rescanning summary Details View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while rescanning summary Details View", URLToRedirect = "/Home/HomePage" });
            }
        }


        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Load Daily ReScanning Details Data Table")]
        public ActionResult LoadReScanningDetailsTable(FormCollection formCollection)
        {
            try
            {

                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeID = formCollection["SROOfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                string ModuleID = formCollection["ModuleID"];

                int SroId = Convert.ToInt32(SROOfficeID);
                int DroId = Convert.ToInt32(DROfficeID);

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                string Amount = formCollection["Amount"];
                if (Amount == "")
                {
                    Amount = "0";
                }
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = "Invalid To Date"
                });
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                #region Server Side Validation              


                caller = new ServiceCaller("CommonsApiController");
                short OfficeID = KaveriSession.Current.OfficeID;
                short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

                ////Validation For DR Login
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

                //    if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please select any District"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //    else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
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
                //    else if (ModuleID == "0" )//when User selects DR but not SR which is by default "Select"
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = false,
                //            errorMessage = "Please Select Module to Procced"
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;

                //    }
                //}
                if (ModuleID == "0")//when User selects DR but not SR which is by default "Select"
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please Select Module to Procced"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }



                    if (string.IsNullOrEmpty(fromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date required"
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
                        errorMessage = "To Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    DateTime DateTime_FromDate = Convert.ToDateTime(fromDate);
                    DateTime DateTime_Todate = Convert.ToDateTime(ToDate);

                    if ((DateTime_Todate - DateTime_FromDate).TotalDays > 180)
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = "0",
                            errorMessage = "You can only see records of six months..."
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                #endregion

                DateTime frmDate, toDate;
                bool boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                bool boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                #region Validate date Inputs
                if (!boolFrmDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid From Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (!boolToDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Invalid To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                if (frmDate > toDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                ReScanningDetailsViewModel reqModel = new ReScanningDetailsViewModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.ModuleID = Convert.ToInt32(ModuleID);
                reqModel.SROfficeID = SroId;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);

                caller = new ServiceCaller("ReScanningDetailsAPIController");
                int totalCount = caller.PostCall<ReScanningDetailsViewModel, int>("GetReScanningTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //To get records of indexII report table 
                ReScanningDetailsResModel resModel = caller.PostCall<ReScanningDetailsViewModel, ReScanningDetailsResModel>("GetReScanningTableData", reqModel, out errorMessage);
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Rescanning Details" });
                }
                IEnumerable<ReScanningDetailsModel> result = resModel.ReScanningDetailsList;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }


                //Sorting
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
                        result = result.Where(m => m.DocumentNo.ToLower().Contains(searchValue.ToLower()) ||
                        m.DocumentNo.ToLower().Contains(searchValue.ToLower()) ||
                        m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROOffice.ToString().Contains(searchValue) ||
                        m.RescanEnableDateTime.ToString().ToLower().Contains(searchValue.ToLower()));
                    }
                }

                var gridData = result.Select(ReScanningDetailsModel => new
                {
                    DocumentNumber = ReScanningDetailsModel.DocumentNo == "0" ? "NA" : ReScanningDetailsModel.DocumentNo,
                    ReScanningDetailsModel.FinalRegistrationNumber,
                    //ReScanningDetailsModel.MarriageCaseNo,
                    ReScanningDetailsModel.SROOffice,
                    ReScanningDetailsModel.RescanEnableDateTime,
                    ReScanningDetailsModel.IsFileUploaded
                });
                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeID + "','" + DROfficeID + "','" + ModuleID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeID + "','" + DROfficeID + "','" + ModuleID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Rescanning Details" });
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
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }



        #region PDF
        /// <summary>
        /// Export Daily Receipt Details Report To PDF
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="DROfficeID"></param>
        /// <param name="ModuleID"></param>
        ///  <param name="FeeTypeID"></param>
        /// <returns>Daily Receipt Details in PDF format</returns>
        [EventAuditLogFilter(Description = "Export Rescanning Details To PDF")]
        public ActionResult ExportReScanningDetailsToPDF(string FromDate, string ToDate, string SROOfficeListID, string DROfficeID, string ModuleID)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                ReScanningDetailsViewModel model = new ReScanningDetailsViewModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    ModuleID = Convert.ToInt32(ModuleID),
                    DROfficeID = Convert.ToInt32(DROfficeID),
                    startLen = 0,
                    totalNum = 10,
                };

                List<ReScanningDetailsModel> objListItemsToBeExported = new List<ReScanningDetailsModel>();
                model.IsPdf = true;
                caller = new ServiceCaller("ReScanningDetailsAPIController");

                //To get total count of records in indexII report datatable
                int totalCount = caller.PostCall<ReScanningDetailsViewModel, int>("GetReScanningTotalCount", model);
                model.totalNum = totalCount;

                // To get total records of indexII report table
                ReScanningDetailsResModel resModel = caller.PostCall<ReScanningDetailsViewModel, ReScanningDetailsResModel>("GetReScanningTableData", model, out errorMessage);
                objListItemsToBeExported = resModel.ReScanningDetailsList;

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }

                string fileName = string.Format("ReScanningDetails" + ".pdf");
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("Rescanning Details (Between {0} and {1} )", FromDate, ToDate);

                caller = new ServiceCaller("CommonsApiController");
                //To get SRONAME
                string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                //Create Temp PDF File
                byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader, SROName, ModuleID);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "RescanningDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");

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
        /// <param name="SROName"></param>
        /// <param name="ModuleId"></param>
        /// <returns>PDF File Byte Array</returns>
        private byte[] CreatePDFFile(List<ReScanningDetailsModel> objListItemsToBeExported, string fileName, string pdfHeader, string SROName, string ModuleId)
        {
            string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));
            int ModuleID = Convert.ToInt32(ModuleId);
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
                            Paragraph addSpace = new Paragraph(" ")
                            {
                                Alignment = 1
                            };
                            var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
                            var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));


                            var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
                            var totalChunk = new Chunk("Total Records: ", blackListTextFont);
                            var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);
                            var SroName = new Chunk(SROName + "     ", redListTextFont);
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

                            if (ModuleID == (int)CommonEnum.ModuleNames.DocumentReg)
                            {
                                doc.Add(DailyReceiptDocRegTbl(objListItemsToBeExported));

                            }
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
        /// Returns DailyReceipt PDFTable when "Document Registration" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns></returns>
        private PdfPTable DailyReceiptDocRegTbl(List<ReScanningDetailsModel> objListItemsToBeExported)
        {
            string SRNO = "SR NO";
            string DocNo = "Document Number ";
            string FRN = "Final Registration Number";
            string OfficeName = "Office Name";
            string DateOfReScanning = "Date Of ReScanning";
            string IsFileUploaded = "File Uploaded";

            try
            {
                PdfPCell cell0 = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;



                string[] col = { SRNO, DocNo, FRN, OfficeName, DateOfReScanning, IsFileUploaded };
                PdfPTable table = new PdfPTable(6)
                {
                    WidthPercentage = 100
                };

                string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 14);

                //to repeat Headers
                table.HeaderRows = 1;
                // then set the column's __relative__ widths
                table.SetWidths(new Single[] { 5, 5, 5, 5, 5, 5 });
                for (int i = 0; i < col.Length; ++i)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(col[i]))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    table.AddCell(cell);

                }
                int cnt = 1;
                foreach (var items in objListItemsToBeExported)
                {
                    cell0 = new PdfPCell(new Phrase(cnt.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell0.BackgroundColor = BaseColor.WHITE;

                    cell1 = new PdfPCell(new Phrase(items.DocumentNo, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell2 = new PdfPCell(new Phrase(items.FinalRegistrationNumber, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(items.SROOffice, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.RescanEnableDateTime, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell4.BackgroundColor = BaseColor.WHITE;

                    cell5 = new PdfPCell(new Phrase(items.IsFileUploaded == true ? "Uploaded" : "Not Uploaded", tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell5.BackgroundColor = BaseColor.WHITE;

                    table.AddCell(cell0);
                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                    cnt++;
                }
                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

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
        /// Export Daily Receipt Details report To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="ModuleID"></param>
        /// <param name="FeeTypeID"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Export Rescanning Details table To Excel")]
        public ActionResult ExportReScanningDetailsToExcel(string FromDate, string ToDate, string SROOfficeListID, string DROfficeID, string ModuleID)
        {
            try
            {
                caller = new ServiceCaller("CommonsApiController");
                string fileName = string.Format("RescanningtDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                ReScanningDetailsViewModel model = new ReScanningDetailsViewModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    DROfficeID = Convert.ToInt32(DROfficeID),
                    ModuleID = Convert.ToInt32(ModuleID),
                    startLen = 0,
                    totalNum = 10,
                };
                model.IsExcel = true;
                string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                if (SROName == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }


                caller = new ServiceCaller("ReScanningDetailsAPIController");
                int totalCount = caller.PostCall<ReScanningDetailsViewModel, int>("GetReScanningTotalCount", model);
                model.totalNum = totalCount;
                List<ReScanningDetailsModel> objListItemsToBeExported = new List<ReScanningDetailsModel>();
                ReScanningDetailsResModel ResModel = caller.PostCall<ReScanningDetailsViewModel, ReScanningDetailsResModel>("GetReScanningTableData", model, out errorMessage);
                objListItemsToBeExported = ResModel.ReScanningDetailsList;

                if (objListItemsToBeExported == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }

                string createdExcelPath = string.Empty;
                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("ReScanning Details Between ({0} and {1})", FromDate, ToDate);

                if (Convert.ToInt32(ModuleID) == (int)CommonEnum.ModuleNames.DocumentReg)
                {
                    createdExcelPath = CreateExcelForDocReg(objListItemsToBeExported, fileName, excelHeader, SROName);
                }

                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ReScanningDetailsExcel" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Create Excel of Daily Receipt Details when "Document Registration" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SROName"></param>
        /// <returns>returns ExcelFilePath</returns>
        private string CreateExcelForDocReg(List<ReScanningDetailsModel> objListItemsToBeExported, string fileName, string excelHeader, string SROName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("ReScanning Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (objListItemsToBeExported.Count());
                    workSheet.Cells[1, 1, 1, 6].Merge = true;
                    workSheet.Cells[2, 1, 2, 6].Merge = true;
                    workSheet.Cells[3, 1, 3, 6].Merge = true;
                    workSheet.Cells[4, 1, 4, 6].Merge = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 45;
                    workSheet.Column(4).Width = 25;
                    workSheet.Column(5).Width = 25;
                    workSheet.Column(6).Width = 25;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;


                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[6, 1].Value = "Sr No";
                    workSheet.Cells[6, 2].Value = "Document Number";
                    workSheet.Cells[6, 3].Value = "Final Registration Number";
                    workSheet.Cells[6, 4].Value = "SRO Office";
                    workSheet.Cells[6, 5].Value = "Date of Rescanning Enabled";
                    workSheet.Cells[6, 6].Value = "Is File Uploaded";


                    int cnt = 1;
                    foreach (var items in objListItemsToBeExported)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 1].Value = cnt;
                        workSheet.Cells[rowIndex, 2].Value = items.DocumentNo;
                        workSheet.Cells[rowIndex, 3].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 4].Value = items.SROOffice;
                        workSheet.Cells[rowIndex, 5].Value = items.RescanEnableDateTime;
                        workSheet.Cells[rowIndex, 6].Value = items.IsFileUploaded == true ? "Uploaded" : "Not Uploaded";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        rowIndex++;
                        //Function that passes the current row and adds the column details
                        cnt++;
                    }
                    workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 6])
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