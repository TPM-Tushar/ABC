#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ServicePackStatusController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.ServicePackStatus;
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
    // COMMENTED AUTHORIZATION ATTRIBUTE BY SHUBHAM ON 09-09-2020 AT 10:32 AM
    [KaveriAuthorizationAttribute]
    public class ServicePackStatusController : Controller
    {
        ServiceCaller caller = null;

        /// <summary>
        /// ServicePackStatusView
        /// </summary>
        /// <returns>returns Service Pack Status View</returns>
        [EventAuditLogFilter(Description = "Service Pack Status View")]
        [MenuHighlight]
        public ActionResult ServicePackStatusView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ServicePackStatus;
                caller = new ServiceCaller("ServicePackStatusAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                ServicePackStatusModel reqModel = caller.GetCall<ServicePackStatusModel>("ServicePackStatusView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Service Pack Status View", URLToRedirect = "/Home/HomePage" });
            }
        }


        //[HttpGet]
        //public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        //{
        //    try
        //    {
        //        string errormessage = string.Empty;
        //        List<SelectListItem> sroOfficeList = new List<SelectListItem>();
        //        ServiceCaller caller = new ServiceCaller("CommonsApiController");
        //        sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictID", new { DistrictID = DistrictID }, out errormessage);
        //        return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
        //    }
        //}


        /// <summary>
        /// Service Pack Status Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns Service Pack Status Details list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Service Pack Status Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult ServicePackStatusDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("ServicePackStatusAPIController");
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;

                #region User Variables and Objects      


                string IsSRDRFlag = formCollection["IsSRDRFlag"];
                string DROOfficeListID = formCollection["DROOfficeListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string SoftwareReleaseTypeListID = formCollection["SoftwareReleaseTypeListID"];
                string ServicePackChangeTypeListID = formCollection["ServicePackChangeTypeListID"];
                string ReleasedStatusListID = formCollection["ReleasedStatusListID"];

                #region Server Side Validation
                if (string.IsNullOrEmpty(DROOfficeListID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Please select District."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (string.IsNullOrEmpty(SROOfficeListID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Please select SRO Name."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (string.IsNullOrEmpty(SoftwareReleaseTypeListID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Please select Release Type."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (string.IsNullOrEmpty(ServicePackChangeTypeListID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Please select Change Type."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (string.IsNullOrEmpty(ReleasedStatusListID))
                {
                    var emptyData = Json(new
                    {
                        errorMessage = "Please select Released Status."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                int DROOfficeListID_INT = Convert.ToInt32(DROOfficeListID);
                int SROOfficeListID_INT = Convert.ToInt32(SROOfficeListID);
                int SoftwareReleaseTypeListID_INT = Convert.ToInt32(SoftwareReleaseTypeListID);
                int ServicePackChangeTypeListID_INT = Convert.ToInt32(ServicePackChangeTypeListID);
                int ReleasedStatusListID_INT = Convert.ToInt32(ReleasedStatusListID);


                //if (SroId <= 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select SRO Office."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                //if (StatusListIDINT <= 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select District."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                //if (ServicePackID <= 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select Service Pack."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                #endregion
                #endregion

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                ServicePackStatusModel reqModel = new ServicePackStatusModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.DROfficeID = DROOfficeListID_INT;
                reqModel.SROfficeID = SROOfficeListID_INT;
                reqModel.ServicePackChangeTypeID = ServicePackChangeTypeListID_INT;
                reqModel.SoftwareReleaseTypeID = SoftwareReleaseTypeListID_INT;
                reqModel.ReleasedStatusID = ReleasedStatusListID_INT;
                reqModel.IsSRDRFlag = IsSRDRFlag;

                //reqModel.StatusID = StatusListIDINT;
                //reqModel.ServicePackID = ServicePackID;

                int totalCount = caller.PostCall<ServicePackStatusModel, int>("ServicePackStatusTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                IEnumerable<ServicePackStatusDetails> result = caller.PostCall<ServicePackStatusModel, List<ServicePackStatusDetails>>("ServicePackStatusDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Service Pack Status details." });
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
                        result = result.Where(m =>
                        m.DistrictName.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                        m.SoftwareReleaseType.ToLower().Contains(searchValue.ToLower()) ||
                        m.ReleaseMode.ToLower().Contains(searchValue.ToLower()) ||
                        m.Description.ToLower().Contains(searchValue.ToLower()) ||
                        m.InstallationProcedure.ToLower().Contains(searchValue.ToLower()) ||
                        m.ChangeType.ToLower().Contains(searchValue.ToLower()) ||
                        m.Status.ToLower().Contains(searchValue.ToLower()) ||
                        m.ReleaseInstruction.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.ReleaseDate.ToString().Contains(searchValue.ToLower()) ||
                        m.AddedDate.ToString().Contains(searchValue.ToLower())
                        );
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(ServicePackStatusDetails => new
                {
                    SerialNo = ServicePackStatusDetails.SerialNo,
                    DistrictName = ServicePackStatusDetails.DistrictName,
                    SROName = ServicePackStatusDetails.SROName,
                    SoftwareReleaseType = ServicePackStatusDetails.SoftwareReleaseType,
                    ReleaseMode = ServicePackStatusDetails.ReleaseMode,
                    Major = ServicePackStatusDetails.Major,
                    Minor = ServicePackStatusDetails.Minor,
                    Description = ServicePackStatusDetails.Description,
                    InstallationProcedure = ServicePackStatusDetails.InstallationProcedure,
                    ChangeType = ServicePackStatusDetails.ChangeType,
                    Status = ServicePackStatusDetails.Status,
                    ReleaseInstruction = ServicePackStatusDetails.ReleaseInstruction,
                    ReleaseDate = ServicePackStatusDetails.ReleaseDate,
                    AddedDate = ServicePackStatusDetails.AddedDate,

                });

                //String PDFDownloadBtn = "<button type ='button' class='btn btn-group-md btn-warning' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROOfficeListID + "','" + FinancialID + "')>PDF</button>";
                //String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + ServicePackListID + "','" + StatusListID + "','" + SROOfficeListID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DROOfficeListID_INT + "','" + SROOfficeListID_INT + "','" + SoftwareReleaseTypeListID + "','" + ServicePackChangeTypeListID + "','" + ReleasedStatusListID + "','" + reqModel.IsSRDRFlag + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";
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
                        //PDFDownloadBtn = PDFDownloadBtn,
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
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Service Pack Status details." }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Download report in PDF
        ///// <summary>
        ///// Export Report To PDF
        ///// </summary>
        ///// <param name="ServicePackListID"></param>
        ///// <param name="StatusListID"></param>
        ///// <param name="SROOfficeListID"></param>
        ///// <returns>returns pdf file</returns>
        //[EventAuditLogFilter(Description = "Service Pack Status Export Report To PDF")]
        //public ActionResult ExportReportToPDF(string ServicePackListID, string StatusListID, string SROOfficeListID)
        //{
        //    try
        //    {
        //        CommonFunctions objCommon = new CommonFunctions();
        //        string errorMessage = string.Empty;

        //        ServicePackStatusModel model = new ServicePackStatusModel
        //        {
        //            SROfficeID = Convert.ToInt32(SROOfficeListID),
        //            //StatusID = Convert.ToInt32(StatusListID),
        //            //ServicePackID = Convert.ToInt32(ServicePackListID),
        //            startLen = 0,
        //            totalNum = 10
        //        };
        //        //model.Amount = Convert.ToInt32(Amount);

        //        List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();

        //        caller = new ServiceCaller("ServicePackStatusAPIController");

        //        //To get total count of records in indexII report datatable
        //        int totalCount = caller.PostCall<ServicePackStatusModel, int>("ServicePackStatusTotalCount", model);
        //        model.totalNum = totalCount;

        //        // To get total records of indexII report table
        //        objListItemsToBeExported = caller.PostCall<ServicePackStatusModel, List<ServicePackStatusDetails>>("ServicePackStatusDetails", model, out errorMessage);

        //        if (objListItemsToBeExported == null)
        //        {
        //            return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
        //        }

        //        //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
        //        string fileName = string.Format("ServicePackStatus.pdf");
        //        string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //        string pdfHeader = string.Format("Service Pack Status");

        //        //To get SRONAME
        //        // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

        //        //Create Temp PDF File
        //        // byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader, SROName);
        //        byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader);

        //        return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ServicePackStatus_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");

        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
        //    }
        //}

        ///// <summary>
        ///// Create PDF File
        ///// </summary>
        ///// <param name="objListItemsToBeExported"></param>
        ///// <param name="fileName"></param>
        ///// <param name="pdfHeader"></param>
        ///// <returns>returns pdf byte array</returns>
        //private byte[] CreatePDFFile(List<ServicePackStatusDetails> objListItemsToBeExported, string fileName, string pdfHeader)
        //{
        //    string folderPath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/"));

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

        //                    //  string Info = string.Format("Print Date Time : {0}   Total Records : {1}  SRO Name : {2}", DateTime.Now.ToString(), SROName);
        //                    doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
        //                    var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
        //                    doc.Open();
        //                    Paragraph addHeading = new Paragraph(pdfHeader, headerTextFont)
        //                    {
        //                        Alignment = 1,
        //                    };
        //                    //Paragraph Info = new Paragraph(Info, redListTextFont)
        //                    //{
        //                    //    Alignment = 1,
        //                    //};
        //                    Paragraph addSpace = new Paragraph(" ")
        //                    {
        //                        Alignment = 1
        //                    };
        //                    var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
        //                    //var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(128,191,255));
        //                    var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));


        //                    var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
        //                    var totalChunk = new Chunk("Total Records: ", blackListTextFont);
        //                    var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);
        //                    // var SroName = new Chunk(SROName + "       ", redListTextFont);
        //                    var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
        //                    string count = objListItemsToBeExported.Count().ToString();
        //                    var countChunk = new Chunk(count, redListTextFont);

        //                    var titlePhrase = new Phrase(titleChunk)
        //                {
        //                    descriptionChunk
        //                };
        //                    var totalPhrase = new Phrase(totalChunk)
        //                {
        //                    countChunk
        //                };
        //                    //    var SroNamePhrase = new Phrase(SroNameChunk)
        //                    //{
        //                    //    SroName
        //                    //};
        //                    doc.Add(addHeading);
        //                    doc.Add(addSpace);
        //                    doc.Add(titlePhrase);
        //                    //doc.Add(SroNamePhrase);
        //                    doc.Add(totalPhrase);
        //                    doc.Add(addSpace);

        //                    doc.Add(ReportTable(objListItemsToBeExported));
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

        ///// <summary>
        ///// Report Table
        ///// </summary>
        ///// <param name="objListItemsToBeExported"></param>
        ///// <returns>returns pdf table</returns>
        //private PdfPTable ReportTable(List<ServicePackStatusDetails> objListItemsToBeExported)
        //{

        //    string SerialNumber = "Serial Number";
        //    string ServicePack = "Service Pack";
        //    string SROName = "SRO Name";
        //    string Status = "Status";
        //    string Description = "Description";
        //    string ServicePackDate = "Service Pack Date";
        //    string ReleaseDate = "Release Date";

        //    try
        //    {
        //        string[] col = { SerialNumber, ServicePack, SROName, Status, Description, ServicePackDate, ReleaseDate };
        //        PdfPTable table = new PdfPTable(7)
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
        //        table.SetWidths(new Single[] { 4, 5, 4, 4, 7, 5, 4 });
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

        //        for (int i = 0; i < col.Length; ++i)
        //        {
        //            PdfPCell cell = new PdfPCell(new Phrase(col[i]))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell.HorizontalAlignment = Element.ALIGN_CENTER;

        //            table.AddCell(cell);
        //        }

        //        foreach (var items in objListItemsToBeExported)
        //        {
        //            cell1 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell1.BackgroundColor = BaseColor.WHITE;

        //            cell2 = new PdfPCell(new Phrase(items.ServicePackName, tableContentFont))
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

        //            cell4 = new PdfPCell(new Phrase(items.Status, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell4.BackgroundColor = BaseColor.WHITE;

        //            cell4.HorizontalAlignment = Element.ALIGN_LEFT;

        //            cell5 = new PdfPCell(new Phrase(items.Description, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell5.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell5.BackgroundColor = BaseColor.WHITE;

        //            cell6 = new PdfPCell(new Phrase(items.ServicePackDate, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell6.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell6.BackgroundColor = BaseColor.WHITE;

        //            cell7 = new PdfPCell(new Phrase(items.ReleaseDate, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell7.BackgroundColor = BaseColor.WHITE;
        //            cell7.HorizontalAlignment = Element.ALIGN_CENTER;

        //            table.AddCell(cell1);
        //            table.AddCell(cell2);
        //            table.AddCell(cell3);
        //            table.AddCell(cell4);
        //            table.AddCell(cell5);
        //            table.AddCell(cell6);
        //            table.AddCell(cell7);
        //        }
        //        return table;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        //Remove this code
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
        /// ExportToExcel
        /// </summary>
        /// <param name="ServicePackListID"></param>
        /// <param name="StatusListID"></param>
        /// <param name="SROOfficeListID"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Service Pack Status Export To Excel")]
        public ActionResult ExportToExcel(string DROOfficeListID, string SROOfficeListID, string SoftwareReleaseTypeListID, string ServicePackChangeTypeListID, string ReleasedStatusListID, string IsSRDRFlag, string DROOfficeSelText, string SROOfficeSelText, string SoftwareReleaseTypeSelText, string ServicePackChangeTypeSelText, string ReleasedStatusSelText)
        {
            try
            {
                caller = new ServiceCaller("ServicePackStatusAPIController");
                string fileName = string.Format("ServicePackStatus.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                ServicePackStatusModel model = new ServicePackStatusModel
                {
                    DROfficeID = Convert.ToInt32(DROOfficeListID),
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    SoftwareReleaseTypeID = Convert.ToInt32(SoftwareReleaseTypeListID),
                    ServicePackChangeTypeID = Convert.ToInt32(ServicePackChangeTypeListID),
                    ReleasedStatusID = Convert.ToInt32(ReleasedStatusListID),
                    IsSRDRFlag = IsSRDRFlag,
                    //StatusID = Convert.ToInt32(StatusListID),
                    //ServicePackID = Convert.ToInt32(ServicePackListID),
                    startLen = 0,
                    totalNum = 10
                };

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                List<ServicePackStatusDetails> objListItemsToBeExported = new List<ServicePackStatusDetails>();

                caller = new ServiceCaller("ServicePackStatusAPIController");
                int totalCount = caller.PostCall<ServicePackStatusModel, int>("ServicePackStatusTotalCount", model);
                model.totalNum = totalCount;
                objListItemsToBeExported = caller.PostCall<ServicePackStatusModel, List<ServicePackStatusDetails>>("ServicePackStatusDetails", model, out errorMessage);
                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string excelHeader = string.Format("Service Pack Status");
                //  string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, DROOfficeSelText, SROOfficeSelText, SoftwareReleaseTypeSelText, ServicePackChangeTypeSelText, ReleasedStatusSelText, IsSRDRFlag);

                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();
                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ServicePackStatus_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        private string CreateExcel(List<ServicePackStatusDetails> objListItemsToBeExported, string fileName, string excelHeader, string DROOfficeSelText, string SROOfficeSelText, string SoftwareReleaseTypeSelText, string ServicePackChangeTypeSelText, string ReleasedStatusSelText, string IsSRDRFlag)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Service Pack Status");
                    workSheet.Cells.Style.Font.Size = 14;

                    if (IsSRDRFlag == "S")
                    {
                        workSheet.Cells[1, 1].Value = excelHeader;
                        workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                        workSheet.Cells[3, 1].Value = "District : " + DROOfficeSelText;
                        workSheet.Cells[4, 1].Value = "SRO Name : " + SROOfficeSelText;
                        workSheet.Cells[5, 1].Value = "Release Type : " + SoftwareReleaseTypeSelText;
                        workSheet.Cells[6, 1].Value = "Change Type : " + ServicePackChangeTypeSelText;
                        workSheet.Cells[7, 1].Value = "Released Status : " + ReleasedStatusSelText;
                        workSheet.Cells[8, 1].Value = "Total Records : " + objListItemsToBeExported.Count();
                        workSheet.Cells[1, 1, 1, 14].Merge = true;
                        workSheet.Cells[2, 1, 2, 14].Merge = true;
                        workSheet.Cells[3, 1, 3, 14].Merge = true;
                        workSheet.Cells[4, 1, 4, 14].Merge = true;
                        workSheet.Cells[5, 1, 5, 14].Merge = true;
                        workSheet.Cells[6, 1, 6, 14].Merge = true;
                        workSheet.Cells[7, 1, 7, 14].Merge = true;
                        workSheet.Cells[8, 1, 8, 14].Merge = true;
                        workSheet.Column(6).Style.WrapText = true;
                        workSheet.Column(7).Style.WrapText = true;
                        // ADDED BY SHUBHAM BHAGAT ON 23-10-2020 AT 5:15 PM
                        workSheet.Column(10).Style.WrapText = true;
                        workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Column(1).Width = 20;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 30;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 30;
                        workSheet.Column(8).Width = 40;
                        workSheet.Column(9).Width = 40;
                        workSheet.Column(10).Width = 50;
                        workSheet.Column(11).Width = 30;
                        workSheet.Column(12).Width = 30;
                        workSheet.Column(13).Width = 30;
                        workSheet.Column(14).Width = 30;

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Row(2).Style.Font.Bold = true;
                        workSheet.Row(3).Style.Font.Bold = true;
                        workSheet.Row(4).Style.Font.Bold = true;
                        workSheet.Row(5).Style.Font.Bold = true;
                        workSheet.Row(6).Style.Font.Bold = true;
                        workSheet.Row(7).Style.Font.Bold = true;
                        workSheet.Row(8).Style.Font.Bold = true;
                        workSheet.Row(10).Style.Font.Bold = true;

                        int rowIndex = 11;
                        workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[10, 1].Value = "Serial Number";
                        workSheet.Cells[10, 2].Value = "District";
                        workSheet.Cells[10, 3].Value = "Sub Registrar Office";
                        workSheet.Cells[10, 4].Value = "Software Release Type";
                        workSheet.Cells[10, 5].Value = "Release Mode";
                        workSheet.Cells[10, 6].Value = "Major";
                        workSheet.Cells[10, 7].Value = "Minor";
                        workSheet.Cells[10, 8].Value = "Description";
                        workSheet.Cells[10, 9].Value = "Installation Procedure";
                        workSheet.Cells[10, 10].Value = "Changes / Enhancement Description";
                        workSheet.Cells[10, 11].Value = "Status";
                        workSheet.Cells[10, 12].Value = "Release Instructions";
                        workSheet.Cells[10, 13].Value = "Service Pack Added Date";
                        workSheet.Cells[10, 14].Value = "Service Pack Release Date";

                        workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(10).Style.Font.Name = "KNB-TTUmaEN";                        

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
                            workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 14].Style.Font.Name = "KNB-TTUmaEN";

                            workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                            workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                            workSheet.Cells[rowIndex, 3].Value = items.SROName;
                            workSheet.Cells[rowIndex, 4].Value = items.SoftwareReleaseType;
                            workSheet.Cells[rowIndex, 5].Value = items.ReleaseMode;
                            workSheet.Cells[rowIndex, 6].Value = items.Major;
                            workSheet.Cells[rowIndex, 7].Value = items.Minor;
                            workSheet.Cells[rowIndex, 8].Value = items.Description;
                            workSheet.Cells[rowIndex, 9].Value = items.InstallationProcedure;
                            workSheet.Cells[rowIndex, 10].Value = items.ChangeType;
                            workSheet.Cells[rowIndex, 11].Value = items.Status;
                            workSheet.Cells[rowIndex, 12].Value = items.ReleaseInstruction;
                            workSheet.Cells[rowIndex, 13].Value = items.AddedDate;
                            workSheet.Cells[rowIndex, 14].Value = items.ReleaseDate;

                            workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                            workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            rowIndex++;
                            //Function that passes the current row and adds the column details 
                            //AddSubRowsForCurrentRow(out row,out workSheet);
                        }

                        using (ExcelRange Rng = workSheet.Cells[10, 1, (rowIndex - 1), 14])
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                    }

                    else
                    {
                        workSheet.Cells[1, 1].Value = excelHeader;
                        workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                        workSheet.Cells[3, 1].Value = "District : " + DROOfficeSelText;
                        workSheet.Cells[4, 1].Value = "Release Type : " + SoftwareReleaseTypeSelText;
                        workSheet.Cells[5, 1].Value = "Change Type : " + ServicePackChangeTypeSelText;
                        workSheet.Cells[6, 1].Value = "Released Status : " + ReleasedStatusSelText;
                        workSheet.Cells[7, 1].Value = "Total Records : " + objListItemsToBeExported.Count();
                        workSheet.Cells[1, 1, 1, 13].Merge = true;
                        workSheet.Cells[2, 1, 2, 13].Merge = true;
                        workSheet.Cells[3, 1, 3, 13].Merge = true;
                        workSheet.Cells[4, 1, 4, 13].Merge = true;
                        workSheet.Cells[5, 1, 5, 13].Merge = true;
                        workSheet.Cells[6, 1, 6, 13].Merge = true;
                        workSheet.Cells[7, 1, 7, 13].Merge = true;
                        //workSheet.Cells[8, 1, 8, 14].Merge = true;

                        //workSheet.Cells[4, 1].Value = "Total Records : " + objListItemsToBeExported.Count();
                        workSheet.Cells[1, 1, 1, 13].Merge = true;
                        workSheet.Cells[2, 1, 2, 13].Merge = true;
                        workSheet.Cells[3, 1, 3, 13].Merge = true;
                        workSheet.Cells[4, 1, 4, 13].Merge = true;
                        workSheet.Cells[5, 1, 5, 13].Merge = true;
                        workSheet.Cells[6, 1, 6, 13].Merge = true;
                        workSheet.Cells[7, 1, 7, 13].Merge = true;
                        
                        workSheet.Column(6).Style.WrapText = true;
                        workSheet.Column(7).Style.WrapText = true;
                        // ADDED BY SHUBHAM BHAGAT ON 23-10-2020 AT 5:15 PM
                        workSheet.Column(9).Style.WrapText = true;
                        workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Column(1).Width = 20;
                        workSheet.Column(2).Width = 30;
                        //workSheet.Column(3).Width = 30;
                        workSheet.Column(3).Width = 30;
                        workSheet.Column(4).Width = 30;
                        workSheet.Column(5).Width = 30;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 40;
                        workSheet.Column(8).Width = 40;
                        workSheet.Column(9).Width = 50;
                        workSheet.Column(10).Width = 30;
                        workSheet.Column(11).Width = 30;
                        workSheet.Column(12).Width = 30;
                        workSheet.Column(13).Width = 30;

                        workSheet.Row(1).Style.Font.Bold = true;
                        workSheet.Row(2).Style.Font.Bold = true;
                        workSheet.Row(3).Style.Font.Bold = true;
                        workSheet.Row(4).Style.Font.Bold = true;
                        workSheet.Row(5).Style.Font.Bold = true;
                        workSheet.Row(6).Style.Font.Bold = true;
                        workSheet.Row(7).Style.Font.Bold = true;
                        workSheet.Row(9).Style.Font.Bold = true;

                        int rowIndex = 10;
                        workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                        workSheet.Cells[9, 1].Value = "Serial Number";
                        workSheet.Cells[9, 2].Value = "District";
                        //workSheet.Cells[9, 3].Value = "Sub Registrar Office";
                        workSheet.Cells[9, 3].Value = "Software Release Type";
                        workSheet.Cells[9, 4].Value = "Release Mode";
                        workSheet.Cells[9, 5].Value = "Major";
                        workSheet.Cells[9, 6].Value = "Minor";
                        workSheet.Cells[9, 7].Value = "Description";
                        workSheet.Cells[9, 8].Value = "Installation Procedure";
                        workSheet.Cells[9, 9].Value = "Changes / Enhancement Description";
                        workSheet.Cells[9, 10].Value = "Status";
                        workSheet.Cells[9, 11].Value = "Release Instructions";
                        workSheet.Cells[9, 12].Value = "Service Pack Added Date";
                        workSheet.Cells[9, 13].Value = "Service Pack Release Date";

                        workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";

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
                            workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";
                            workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";
                            //workSheet.Cells[rowIndex, 14].Style.Font.Name = "KNB-TTUmaEN";

                            workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                            workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                            //workSheet.Cells[rowIndex, 3].Value = items.SROName;
                            workSheet.Cells[rowIndex, 3].Value = items.SoftwareReleaseType;
                            workSheet.Cells[rowIndex, 4].Value = items.ReleaseMode;
                            workSheet.Cells[rowIndex, 5].Value = items.Major;
                            workSheet.Cells[rowIndex, 6].Value = items.Minor;
                            workSheet.Cells[rowIndex, 7].Value = items.Description;
                            workSheet.Cells[rowIndex, 8].Value = items.InstallationProcedure;
                            workSheet.Cells[rowIndex, 9].Value = items.ChangeType;
                            workSheet.Cells[rowIndex, 10].Value = items.Status;
                            workSheet.Cells[rowIndex, 11].Value = items.ReleaseInstruction;
                            workSheet.Cells[rowIndex, 12].Value = items.AddedDate;
                            workSheet.Cells[rowIndex, 13].Value = items.ReleaseDate;

                            workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                            workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                            workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            workSheet.Cells[rowIndex, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                            //workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                            // ADDED BY SHUBHAM BHAGAT ON 23-10-2020 AT 5:15 PM
                            //workSheet.Cells[rowIndex, 9].Style.WrapText = true;
                            rowIndex++;
                            //Function that passes the current row and adds the column details 
                            //AddSubRowsForCurrentRow(out row,out workSheet);
                        }

                        using (ExcelRange Rng = workSheet.Cells[9, 1, (rowIndex - 1), 13])
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
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

        // ADDED BY SHUBHAM BHAGAT ON 09-09-2020
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" });
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}