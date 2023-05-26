using CustomModels.Models.Remittance.ScheduleAllocationAnalysis;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using iTextSharp.text;
using iTextSharp.text.pdf;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.IO;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorization]
    public class ScheduleAllocationAnalysisController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        [MenuHighlight]
        [HttpGet]
        // GET: Remittance/ScheduleAllocationAnalysis
        public ActionResult ScheduleAllocationAnalysisView()
        {
            int OfficeID = KaveriSession.Current.OfficeID;
            caller = new ServiceCaller("ScheduleAllocationAnalysisAPIController");
            ScheduleAllocationAnalysisResponseModel reqModel = caller.GetCall<ScheduleAllocationAnalysisResponseModel>("ScheduleAllocationAnalysisView", new { officeID = OfficeID });
            reqModel.DROName = " ";
            reqModel.SROName = " ";
            reqModel.Year = " ";
            //reqModel.RegArticleId = null;
            return View(reqModel);
        }


        [HttpGet]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            try
            {
                List<SelectListItem> sroOfficeList = new List<SelectListItem>();
                ServiceCaller caller = new ServiceCaller("CommonsApiController");
                //Commented By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                //sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.

                //Added By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictID", new { DistrictID = DistrictID }, out errormessage);
                //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }



        [HttpPost]
        [EventAuditLogFilter(Description = "Get Schedule Allocation Analysis Details")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetScheduleAllocationAnalysisDetails(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects

                string SROfficeID = formCollection["SROfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                string YearID = formCollection["YearID"];
                string RegArticleID = formCollection["RegArticleID"];
                string IsPartyIdCheckBoxSelectedID = formCollection["IsPartyIdCheckBoxSelected"];

                //Added By ShivamB on 30-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page
                string IsThroughVerifyCheckBoxSelectedID = formCollection["IsThroughVerifyCheckBoxSelected"];
                string IsSelectAllYearCheckBoxSelectedID = formCollection["IsSelectAllYearCheckBoxSelected"];
                //Added By ShivamB on 30-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page

                int SroId = Convert.ToInt32(SROfficeID);
                int DroId = Convert.ToInt32(DROfficeID);


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

                //Commented By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
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
                // else
                //{//Validations of Logins other than SR and DR


                //if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
                //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.

                //Commented By ShivamB on 30-09-2022 for adding All options in District DropDown parameter.
                //if (DroId == 0)//when user do not select any DR and SR which are by default "Select"
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
                //Commented By ShivamB on 30-09-2022 for adding All options in District DropDown parameter.


                //Commented By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                //else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
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
                //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.

                //Validations of Registration Article
                string RegistrationArticleID = RegArticleID.Trim();
                if (RegistrationArticleID == null || RegistrationArticleID == " " || RegArticleID == string.Empty)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select any Registration Article."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //Validations of Year
                if (Convert.ToBoolean(IsSelectAllYearCheckBoxSelectedID) == false)
                {
                    string yearId = YearID.Trim();
                    if (yearId == null || yearId == " " || yearId == string.Empty)
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please select any Year"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }

                //int Year_ID = Convert.ToInt32(YearID);

                bool IsPartyIdCheckBoxSelected = Convert.ToBoolean(IsPartyIdCheckBoxSelectedID);

                //Added By ShivamB on 30-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page
                bool IsThroughVerifyCheckBoxSelected = Convert.ToBoolean(IsThroughVerifyCheckBoxSelectedID);
                bool IsSelectAllYearCheckBoxSelected = Convert.ToBoolean(IsSelectAllYearCheckBoxSelectedID);
                //Ended By ShivamB on 30-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page

                #endregion


                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);


                ScheduleAllocationAnalysisResponseModel reqModel = new ScheduleAllocationAnalysisResponseModel();
                ScheduleAllocationAnalysisResultModel resultModel = new ScheduleAllocationAnalysisResultModel();

                reqModel.SROfficeListID = Convert.ToInt32(SROfficeID);
                reqModel.DROfficeListID = Convert.ToInt32(DROfficeID);
                reqModel.Year = YearID;


                int[] RegArticleIdList = Array.ConvertAll(RegistrationArticleID.Split(','), int.Parse);
                reqModel.RegArticleId = RegArticleIdList;


                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.IsPartyIdCheckBoxSelected = IsPartyIdCheckBoxSelected;

                //Added By ShivamB on 30-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page
                reqModel.IsThroughVerifyCheckBoxSelected = IsThroughVerifyCheckBoxSelected;
                reqModel.IsSelectAllYearSelected = IsSelectAllYearCheckBoxSelected;
                //Ended By ShivamB on 30-09-2022 for adding IsThroughVerifyCheckBox in the ScheduleAllocationAnalysis view page

                caller = new ServiceCaller("ScheduleAllocationAnalysisAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                //To get total count of records in Schedule Allocation Analysis report datatable
                int totalCount = caller.PostCall<ScheduleAllocationAnalysisResponseModel, int>("GetScheduleAllocationAnalysisDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //To get records of Schedule Allocation Analysis table 
                resultModel = caller.PostCall<ScheduleAllocationAnalysisResponseModel, ScheduleAllocationAnalysisResultModel>("GetScheduleAllocationAnalysisDetails", reqModel, out errorMessage);

                IEnumerable<ScheduleAllocationAnalysisDetailsModel> result = resultModel.scheduleAllocationDetailsList;

                if (resultModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Schedule Allocation Analysis Details" });
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
                        result = result.Where(m => m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.Stamp5DateTime.ToString().ToLower().Contains(searchValue.ToLower()));
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(ScheduleAllocationAnalysisDetailsModel => new
                {
                    FinalRegistrationNumber = ScheduleAllocationAnalysisDetailsModel.FinalRegistrationNumber,
                    Stamp5DateTime = ScheduleAllocationAnalysisDetailsModel.Stamp5DateTime,
                    SrNo = ScheduleAllocationAnalysisDetailsModel.SrNo,

                });

                if (String.IsNullOrEmpty(resultModel.Year))
                    resultModel.Year = "All";

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + DROfficeID + "','" + SROfficeID + "','" + RegArticleID + "','" + resultModel.Year + "','" + IsPartyIdCheckBoxSelectedID + "','" + IsThroughVerifyCheckBoxSelectedID + "','" + IsSelectAllYearCheckBoxSelectedID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' id='PDFDownloadBtn' onclick=PDFDownloadFun('" + DROfficeID + "','" + SROfficeID + "','" + RegArticleID + "','" + resultModel.Year + "','" + IsPartyIdCheckBoxSelectedID + "','" + IsThroughVerifyCheckBoxSelectedID + "','" + IsSelectAllYearCheckBoxSelectedID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";

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
                        DroName = resultModel.DROName,
                        SroName = resultModel.SROName,
                        Year = resultModel.Year,

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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Schedule Allocation Analysis Details" });
            }
        }


        [EventAuditLogFilter(Description = "Export Schedule Allocation Analysis To PDF")]
        public ActionResult ExportScheduleAllocationAnalysisToPDF(string DROfficeID, string SROfficeID, string RegArticleID, string YearID, string IsPartyIdCheckBoxSelectedID, string IsThroughVerifyCheckBoxSelectedID, string IsSelectAllYearCheckBoxSelectedID)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                int[] RegArticleIdList = Array.ConvertAll(RegArticleID.Split(','), int.Parse);


                ScheduleAllocationAnalysisResponseModel model = new ScheduleAllocationAnalysisResponseModel
                {
                    SROfficeListID = Convert.ToInt32(SROfficeID),
                    DROfficeListID = Convert.ToInt32(DROfficeID),
                    Year = YearID,
                    RegArticleId = RegArticleIdList,
                    IsPartyIdCheckBoxSelected = Convert.ToBoolean(IsPartyIdCheckBoxSelectedID),
                    IsThroughVerifyCheckBoxSelected = Convert.ToBoolean(IsThroughVerifyCheckBoxSelectedID),
                    IsSelectAllYearSelected = Convert.ToBoolean(IsSelectAllYearCheckBoxSelectedID),
                    startLen = 0,
                    totalNum = 10,
                };

                ScheduleAllocationAnalysisResultModel scheduleAllocationAnalysisResultModel = new ScheduleAllocationAnalysisResultModel();
                model.IsPdf = true;
                caller = new ServiceCaller("ScheduleAllocationAnalysisAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in Schedule Allocation Analysis datatable
                int totalCount = caller.PostCall<ScheduleAllocationAnalysisResponseModel, int>("GetScheduleAllocationAnalysisDetailsTotalCount", model);
                model.totalNum = totalCount;

                // To get total records of Schedule Allocation Analysis table
                scheduleAllocationAnalysisResultModel = caller.PostCall<ScheduleAllocationAnalysisResponseModel, ScheduleAllocationAnalysisResultModel>("GetScheduleAllocationAnalysisDetails", model, out errorMessage);

                if (scheduleAllocationAnalysisResultModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }

                if (Convert.ToBoolean(IsSelectAllYearCheckBoxSelectedID) == true)
                    YearID = "Year Selected all";

                //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
                string fileName = string.Format("ScheduleAllocationAnalysisPDF.pdf");
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("Schedule Allocation Analysis ({0})", YearID);

                //Added By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                //To get SRONAME
                string SROName = null;
                if (SROfficeID == "0")
                    SROName = "All";
                else
                    SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROfficeID });
                //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.


                //Create Temp PDF File
                byte[] pdfBytes = CreatePDFFile(scheduleAllocationAnalysisResultModel.scheduleAllocationDetailsList, fileName, pdfHeader, SROName);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ScheduleAllocationAnalysisPDF_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
                //return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "IncomeTaxReportPDF_" + DateTime.Now.ToString("dd-MM-yyyy-hh_mm_ss") + ".pdf");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
            }
        }


        private byte[] CreatePDFFile(List<ScheduleAllocationAnalysisDetailsModel> scheduleAllocationAnalysisDetailsList, string fileName, string pdfHeader, string SROName)
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
                            string count = (scheduleAllocationAnalysisDetailsList.Count()).ToString();
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

                            doc.Add(ScheduleAllocationAnalysisTable(scheduleAllocationAnalysisDetailsList));
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



        private PdfPTable ScheduleAllocationAnalysisTable(List<ScheduleAllocationAnalysisDetailsModel> scheduleAllocationAnalysisDetailsList)
        {

            string SrNo = "SrNo.";
            string FinalRegisrationNumber = "Final Registration Number";
            string Stamp5DateTime = "Stamp 5 Date Time";


            try
            {
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                string[] col = { SrNo, FinalRegisrationNumber, Stamp5DateTime };
                PdfPTable table = new PdfPTable(3)
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
                table.SetWidths(new Single[] { 10, 30, 30 });
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
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell.Rotation = 90;
                    table.AddCell(cell);

                }

                //int i = 1;

                foreach (var items in scheduleAllocationAnalysisDetailsList)
                {

                    cell = new PdfPCell(new Phrase(items.SrNo.ToString(), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell.Rotation = 90;
                    table.AddCell(cell);


                    cell = new PdfPCell(new Phrase(items.FinalRegistrationNumber.ToString(), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell.Rotation = 90;
                    table.AddCell(cell);

                    cell = new PdfPCell(new Phrase(items.Stamp5DateTime.ToString(), tableContentFont));
                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //cell.Rotation = 90;
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


        [EventAuditLogFilter(Description = "Export Schedule Allocation Analysis To Excel")]
        public ActionResult ExportScheduleAllocationAnalysisToExcel(string DROfficeID, string SROfficeID, string RegArticleID, string YearID, string IsPartyIdCheckBoxSelectedID, string IsThroughVerifyCheckBoxSelectedID, string IsSelectAllYearCheckBoxSelectedID)
        {
            try
            {
                caller = new ServiceCaller("ScheduleAllocationAnalysisAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("ScheduleAllocationAnalysis.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                int[] RegArticleIdList = Array.ConvertAll(RegArticleID.Split(','), int.Parse);

                ScheduleAllocationAnalysisResponseModel model = new ScheduleAllocationAnalysisResponseModel
                {
                    SROfficeListID = Convert.ToInt32(SROfficeID),
                    DROfficeListID = Convert.ToInt32(DROfficeID),
                    Year = YearID,
                    RegArticleId = RegArticleIdList,
                    IsPartyIdCheckBoxSelected = Convert.ToBoolean(IsPartyIdCheckBoxSelectedID),
                    IsThroughVerifyCheckBoxSelected = Convert.ToBoolean(IsThroughVerifyCheckBoxSelectedID),
                    IsSelectAllYearSelected = Convert.ToBoolean(IsSelectAllYearCheckBoxSelectedID),
                    startLen = 0,
                    totalNum = 10,
                };
                model.IsExcel = true;

                //Added By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.
                string SROName = null;
                if (SROfficeID == "0")
                {
                    SROName = "All";
                }
                else
                {
                    SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROfficeID }, out errorMessage);
                    if (SROName == null)
                    {
                        return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                    }
                }
                //End By ShivamB on 30-09-2022 for adding All options in SRODropDown parameter on selection of District.    



                ScheduleAllocationAnalysisResultModel scheduleAllocationAnalysisResultModel = new ScheduleAllocationAnalysisResultModel();

                caller = new ServiceCaller("ScheduleAllocationAnalysisAPIController");
                TimeSpan objTimeSpan2 = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan2;
                int totalCount = caller.PostCall<ScheduleAllocationAnalysisResponseModel, int>("GetScheduleAllocationAnalysisDetailsTotalCount", model);
                model.totalNum = totalCount;
                scheduleAllocationAnalysisResultModel = caller.PostCall<ScheduleAllocationAnalysisResponseModel, ScheduleAllocationAnalysisResultModel>("GetScheduleAllocationAnalysisDetails", model, out errorMessage);

                if (scheduleAllocationAnalysisResultModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }

                if (Convert.ToBoolean(IsSelectAllYearCheckBoxSelectedID) == true)
                    YearID = "Year Selected all";

                string excelHeader = string.Format("Schedule Allocation Analysis ({0})", YearID);
                string createdExcelPath = CreateExcel(scheduleAllocationAnalysisResultModel.scheduleAllocationDetailsList, fileName, excelHeader, SROName);

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);

                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ScheduleAllocationAnalysisExcel" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }


        private string CreateExcel(List<ScheduleAllocationAnalysisDetailsModel> incomeTaxReportDetailsList, string fileName, string excelHeader, string SROName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("schedule Allocation Analysis Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (incomeTaxReportDetailsList.Count());
                    workSheet.Cells[1, 1, 1, 3].Merge = true;
                    workSheet.Cells[2, 1, 2, 3].Merge = true;
                    workSheet.Cells[3, 1, 3, 3].Merge = true;
                    workSheet.Cells[4, 1, 4, 3].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    workSheet.Column(1).Width = 20;  // SR No
                    workSheet.Column(2).Width = 50;  // Final Registration No
                    workSheet.Column(3).Width = 50;  // Stamp 5 Date Time


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;


                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;


                    workSheet.Cells[6, 1].Value = "Sr No.";
                    workSheet.Cells[6, 2].Value = "Final Registration Number";
                    workSheet.Cells[6, 3].Value = "Stamp 5 Date Time";



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


                        //workSheet.Cells[rowIndex, 22].Style.Numberformat.Format = "0.00";
                        //workSheet.Cells[rowIndex, 23].Style.Numberformat.Format = "0.00";
                        //workSheet.Cells[rowIndex, 29].Style.Numberformat.Format = "0.00";


                        workSheet.Cells[rowIndex, 1].Value = items.SrNo;
                        workSheet.Cells[rowIndex, 2].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 3].Value = items.Stamp5DateTime;





                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                        //workSheet.Cells[rowIndex, 22].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        //workSheet.Cells[rowIndex, 22].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        //workSheet.Cells[rowIndex, 23].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        //workSheet.Cells[rowIndex, 23].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        //workSheet.Cells[rowIndex, 29].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        //workSheet.Cells[rowIndex, 29].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;


                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    //workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 3])
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