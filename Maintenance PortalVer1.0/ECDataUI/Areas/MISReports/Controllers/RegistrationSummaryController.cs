using CustomModels.Models.MISReports.RegistrationSummary;
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
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.MISReports.Controllers
{
    public class RegistrationSummaryController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// Index II Reports View
        /// </summary>
        /// <returns>returns view</returns>
        //[EventAuditLogFilter(Description = "Index II Reports View")]
        [MenuHighlight]
        public ActionResult RegistrationSummaryView()
        {
            try
            {

                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.IndexIIReports;
                caller = new ServiceCaller("RegistrationSummaryAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                RegistrationSummaryRESModel reqModel = caller.GetCall<RegistrationSummaryRESModel>("RegistrationSummaryView", new { OfficeID = OfficeID });
                return View(reqModel);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Registration Summary View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get Index II Reports Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        //[EventAuditLogFilter(Description = "Get Index II Reports Details")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetRegistrationSummaryDetails(FormCollection formCollection)
        {
            try
            {

                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string NatureOfDocumentListID = formCollection["NatureOfDocumentListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                //string DROfficeID = formCollection["DROfficeID"];

                int SroId = Convert.ToInt32(SROOfficeListID);
                //int DroId = Convert.ToInt32(DROfficeID);
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //string Amount = formCollection["Amount"];
                //if (Amount == "")
                //{
                //    Amount = "0";
                //}
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

                    //if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
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
                    //else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
                    if (SroId == 0)//when User selects DR but not SR which is by default "Select"
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

                //if (SroId == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Select any SRO"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;

                //}
                if (string.IsNullOrEmpty(fromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please Enter From Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (string.IsNullOrEmpty(ToDate))
                {

                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please Enter To Date"

                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;

                }
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

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
                else if (string.IsNullOrEmpty(ToDate))
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
                //else
                //{
                //    #region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
                //    DateTime CurrentDate = DateTime.Now;
                //    int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                //    int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));

                //    int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                //    int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                //    if (CMonth > 3)
                //    {
                //        if (FromDateyear < CYear)
                //        {
                //            var emptyData = Json(new
                //            {
                //                draw = formCollection["draw"],
                //                recordsTotal = 0,
                //                recordsFiltered = 0,
                //                data = "",
                //                status = false,
                //                errorMessage = "Records of current financial year only can be seen at a time"
                //            });
                //            emptyData.MaxJsonLength = Int32.MaxValue;
                //            return emptyData;

                //        }
                //        else
                //        {
                //            if (FromDateyear > CYear)
                //            {
                //                if (FromDateyear == CYear + 1)
                //                {
                //                    if (FromDateMonth > 3)
                //                    {
                //                        var emptyData = Json(new
                //                        {
                //                            draw = formCollection["draw"],
                //                            recordsTotal = 0,
                //                            recordsFiltered = 0,
                //                            data = "",
                //                            status = false,
                //                            errorMessage = "Records of current financial year only can be seen at a time"
                //                        });
                //                        emptyData.MaxJsonLength = Int32.MaxValue;
                //                        return emptyData;

                //                    }

                //                }
                //                else
                //                {
                //                    var emptyData = Json(new
                //                    {
                //                        draw = formCollection["draw"],
                //                        recordsTotal = 0,
                //                        recordsFiltered = 0,
                //                        data = "",
                //                        status = false,
                //                        errorMessage = "Records of current financial year only can be seen at a time"
                //                    });
                //                    emptyData.MaxJsonLength = Int32.MaxValue;
                //                    return emptyData;
                //                }
                //            }
                //            else if (FromDateyear == CYear)
                //            {
                //                if (FromDateMonth < 4)
                //                {
                //                    var emptyData = Json(new
                //                    {
                //                        draw = formCollection["draw"],
                //                        recordsTotal = 0,
                //                        recordsFiltered = 0,
                //                        data = "",
                //                        status = false,
                //                        errorMessage = "Records of current financial year only can be seen at a time"
                //                    });
                //                    emptyData.MaxJsonLength = Int32.MaxValue;
                //                    return emptyData;

                //                }
                //            }



                //        }

                //    }
                //    else if (CMonth <= 3)
                //    {
                //        if (FromDateyear < CYear)
                //        {
                //            if (FromDateyear == CYear - 1)
                //            {
                //                if (FromDateMonth < 4)
                //                {
                //                    var emptyData = Json(new
                //                    {
                //                        draw = formCollection["draw"],
                //                        recordsTotal = 0,
                //                        recordsFiltered = 0,
                //                        data = "",
                //                        status = false,
                //                        errorMessage = "Records of current financial year only can be seen at a time"
                //                    });
                //                    emptyData.MaxJsonLength = Int32.MaxValue;
                //                    return emptyData;

                //                }

                //            }
                //            else
                //            {
                //                var emptyData = Json(new
                //                {
                //                    draw = formCollection["draw"],
                //                    recordsTotal = 0,
                //                    recordsFiltered = 0,
                //                    data = "",
                //                    status = false,
                //                    errorMessage = "Records of current financial year only can be seen at a time"
                //                });
                //                emptyData.MaxJsonLength = Int32.MaxValue;
                //                return emptyData;


                //            }

                //        }

                //    }
                //    #endregion
                //}

                // COMMENTED BY SHUBHAM BHAGAT ON 22-04-2021
                //if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(ToDate))
                //{
                //    DateTime DateTime_FromDate = Convert.ToDateTime(fromDate);
                //    DateTime DateTime_Todate = Convert.ToDateTime(ToDate);

                //    if ((DateTime_Todate - DateTime_FromDate).TotalDays > 180)
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = "0",
                //            errorMessage = "You can only see records of six months..."
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //}


                // ADDED BY SHUBHAM BHAGAT ON 26-04-2021 FOR CHECKING FROM DATE AND TO DATE IS IN SAME FINANCIAL YEAR
                int FromDateYear = frmDate.Month >= 4 ? frmDate.Year + 1 : frmDate.Year;
                int ToDateYear = toDate.Month >= 4 ? toDate.Year + 1 : toDate.Year;
                if (FromDateYear != ToDateYear)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "From date and To date should be in same financial year."
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

                RegistrationSummaryRESModel reqModel = new RegistrationSummaryRESModel();

                //reqModel.Amount = Convert.ToInt64(Amount);
                //Int64 AmountINT64 = 0;
                //bool IsAmountConverted = Int64.TryParse(Amount, out AmountINT64);
                //if (!IsAmountConverted)
                //{
                //    return Json(new { success = false, errorMessage = "Please enter a valid Amount." });
                //}
                //reqModel.Amount = AmountINT64;
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.NatureOfDocumentID = Convert.ToInt32(NatureOfDocumentListID);
                reqModel.SROfficeID = Convert.ToInt32(SROOfficeListID);
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;
                //reqModel.DROfficeID = Convert.ToInt32(DROfficeID);


                caller = new ServiceCaller("RegistrationSummaryAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;

                //To get total count of records in indexII report datatable
                int totalCount = caller.PostCall<RegistrationSummaryRESModel, int>("GetRegistrationSummaryDetailsTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                //To get records of indexII report table 
                IEnumerable<RegistrationSummaryDetailModel> result = caller.PostCall<RegistrationSummaryRESModel, List<RegistrationSummaryDetailModel>>("GetRegistrationSummaryDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Registration Summary Details" });
                }
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
                        //Modified By Raman On 28-06-2019 for consideration and marketvalue
                        result = result.Where(m => m.ArticleNameE.ToLower().Contains(searchValue.ToLower()) ||
                        m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.Claimant.ToLower().Contains(searchValue.ToLower()) ||
                        m.consideration.ToString().Contains(searchValue) ||
                        m.Executant.ToLower().Contains(searchValue.ToLower()) ||
                        m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.marketvalue.ToString().Contains(searchValue) ||
                        m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
                        m.Stamp5Datetime.ToLower().Contains(searchValue.ToLower()) ||
                        m.TotalArea.ToLower().Contains(searchValue.ToLower()) ||
                        m.Unit.ToLower().Contains(searchValue.ToLower()) ||
                        m.Schedule.ToLower().Contains(searchValue.ToLower()) ||
                        m.VillageNameE.ToLower().Contains(searchValue.ToLower()));
                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(IndexIIReportsDetailsModel => new
                {
                    FinalRegistrationNumber = IndexIIReportsDetailsModel.FinalRegistrationNumber,
                    ArticleNameE = IndexIIReportsDetailsModel.ArticleNameE,
                    Stamp5Datetime = IndexIIReportsDetailsModel.Stamp5Datetime,
                    TotalArea = IndexIIReportsDetailsModel.TotalArea,
                    Unit = IndexIIReportsDetailsModel.Unit,
                    PropertyDetails = IndexIIReportsDetailsModel.PropertyDetails,
                    Schedule = IndexIIReportsDetailsModel.Schedule,
                    Executant = IndexIIReportsDetailsModel.Executant,
                    Claimant = IndexIIReportsDetailsModel.Claimant,
                    VillageNameE = IndexIIReportsDetailsModel.VillageNameE,
                    marketvalue = IndexIIReportsDetailsModel.marketvalue.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    consideration = IndexIIReportsDetailsModel.consideration.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                });

                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "','" + NatureOfDocumentListID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:45%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "','" + NatureOfDocumentListID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Registration Summary Details" });
            }
        }



        #region Excel
        /// <summary>
        /// Export Index II Report To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="NatureOfDocumentListID"></param>
        /// <param name="Amount"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Export Registration Summary To Excel")]
        public ActionResult ExportRegistrationSummaryToExcel(string FromDate, string ToDate, string SROOfficeListID, string NatureOfDocumentListID, string Amount, string DROOfficeListID)
        {
            try
            {
                caller = new ServiceCaller("RegistrationSummaryAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("RegistrationSummaryExcel.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                RegistrationSummaryRESModel model = new RegistrationSummaryRESModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    NatureOfDocumentID = Convert.ToInt32(NatureOfDocumentListID),
                    //DROfficeID = Convert.ToInt32(DROOfficeListID),
                    startLen = 0,
                    totalNum = 10,
                };
                //model.Amount = Convert.ToInt32(Amount);
                model.IsExcel = true;
                string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                if (SROName == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }

                List<RegistrationSummaryDetailModel> objListItemsToBeExported = new List<RegistrationSummaryDetailModel>();

                caller = new ServiceCaller("RegistrationSummaryAPIController");
                TimeSpan objTimeSpan2 = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan2;
                int totalCount = caller.PostCall<RegistrationSummaryRESModel, int>("GetRegistrationSummaryDetailsTotalCount", model);
                model.totalNum = totalCount;
                objListItemsToBeExported = caller.PostCall<RegistrationSummaryRESModel, List<RegistrationSummaryDetailModel>>("GetRegistrationSummaryDetails", model, out errorMessage);
                if (objListItemsToBeExported == null)
                {

                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Registration Summary Between ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName);
                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "RegistrationSummaryExcel" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="SROName"></param>
        /// <returns>returns ExcelFilePath</returns>
        private string CreateExcel(List<RegistrationSummaryDetailModel> objListItemsToBeExported, string fileName, string excelHeader, string SROName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Registration Summary Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (objListItemsToBeExported.Count() - 1);
                    workSheet.Cells[1, 1, 1, 12].Merge = true;
                    workSheet.Cells[2, 1, 2, 12].Merge = true;
                    workSheet.Cells[3, 1, 3, 12].Merge = true;
                    workSheet.Cells[4, 1, 4, 12].Merge = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;



                    workSheet.Column(6).Width = 50;
                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 25;
                    workSheet.Column(5).Width = 25;
                    workSheet.Column(7).Width = 50;
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 30;
                    workSheet.Column(12).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;


                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[6, 1].Value = "Final Registration Number";
                    workSheet.Cells[6, 2].Value = "Article Name";
                    workSheet.Cells[6, 3].Value = "Stamp5 Date Time";
                    workSheet.Cells[6, 4].Value = "TotalArea";
                    workSheet.Cells[6, 5].Value = "Unit";
                    workSheet.Cells[6, 6].Value = "Property Details";
                    workSheet.Cells[6, 7].Value = "Schedule";
                    workSheet.Cells[6, 8].Value = "Executant";
                    workSheet.Cells[6, 9].Value = "Claimant";
                    workSheet.Cells[6, 10].Value = "Village Name";
                    workSheet.Cells[6, 11].Value = "Market Value";
                    workSheet.Cells[6, 12].Value = "Consideration";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
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
                        workSheet.Cells[rowIndex, 9].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 10].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 11].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 12].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 13].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 11].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 12].Style.Numberformat.Format = "0.00";


                        workSheet.Cells[rowIndex, 1].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 2].Value = items.ArticleNameE;
                        workSheet.Cells[rowIndex, 3].Value = items.Stamp5Datetime;
                        workSheet.Cells[rowIndex, 4].Value = items.TotalArea;
                        workSheet.Cells[rowIndex, 5].Value = items.Unit;
                        workSheet.Cells[rowIndex, 6].Value = items.PropertyDetails;
                        workSheet.Cells[rowIndex, 7].Value = items.Schedule;
                        workSheet.Cells[rowIndex, 8].Value = items.Executant;
                        workSheet.Cells[rowIndex, 9].Value = items.Claimant;
                        workSheet.Cells[rowIndex, 10].Value = items.VillageNameE;
                        workSheet.Cells[rowIndex, 11].Value = items.marketvalue;
                        workSheet.Cells[rowIndex, 12].Value = items.consideration;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 12])
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
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult ValidateParameters(string FRN)
        {
            RegistrationSummaryREQModel ResModel = new RegistrationSummaryREQModel();
            try
            {
                if (string.IsNullOrEmpty(FRN))
                {
                    return Json(new { success = false, errorMessage = "Please enter Final Registration Number" }, JsonRequestBehavior.AllowGet);
                }

                long UserId = KaveriSession.Current.UserID;
                string errorMessage = string.Empty;
                caller = new ServiceCaller("RegistrationSummaryAPIController");
                RegistrationSummaryREQModel ReqModel = new RegistrationSummaryREQModel();
                ReqModel.FinalRegistrationNumberFilePath = FRN;
                ResModel = caller.PostCall<RegistrationSummaryREQModel, RegistrationSummaryREQModel>("DisplayScannedFile", ReqModel, out errorMessage);

                if (ResModel == null)
                    return Json(new { serverError = false, success = false, errorMessage = "Due to some techinical problem this document cannot be viewed right now.Please try again later." }, JsonRequestBehavior.AllowGet);

                else if (ResModel.ScannedFileByteArray == null)
                    return Json(new { serverError = false, success = false, errorMessage = "Due to some techinical problem this document cannot be viewed right now.Please try again later." }, JsonRequestBehavior.AllowGet);
                else
                    return Json(new { serverError = false, success = true, IsFileExistAtDownloadPath = true }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = false, errorMessage = "Error Occured while validating parameters" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DisplayScannedFile(string FRN)
        {
            RegistrationSummaryREQModel ResModel = new RegistrationSummaryREQModel();
            try
            {
                long UserId = KaveriSession.Current.UserID;
                string errorMessage = string.Empty;
                caller = new ServiceCaller("RegistrationSummaryAPIController");
                RegistrationSummaryREQModel ReqModel = new RegistrationSummaryREQModel();
                ReqModel.FinalRegistrationNumberFilePath = FRN;
                ResModel = caller.PostCall<RegistrationSummaryREQModel, RegistrationSummaryREQModel>("DisplayScannedFile", ReqModel, out errorMessage);
                
                if (ResModel.ScannedFileByteArray==null)
                {

                    // Input string.
                    const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                    // Invoke GetBytes method.
                    byte[] array = Encoding.ASCII.GetBytes(input);

                    return File(array, "application/pdf");
                }

                //----------------------- SAVE ENC FILE IN TEMPERORY FOLDER-------------------------
                String fileNameWithExten = Path.GetFileName(FRN.Replace("*", "//"));
                String encFilePath = HttpContext.Server.MapPath(Path.Combine("~/TempEncFilesDownload/", fileNameWithExten));
                System.IO.File.WriteAllBytes(encFilePath, ResModel.ScannedFileByteArray); // Requires System.IO

                //using (FileStream stream = new FileStream(encFilePath, FileMode.Create))
                //{
                //    stream.Write(ResModel.ScannedFileByteArray, 0, ResModel.ScannedFileByteArray.Length);
                //    stream.Close();
                //}
                ////-------------------------------------------------------------------------------------------- 


                //----------------------- CONVERT ENC TO TIFF-------------------------
                string filePath = string.Empty;
                string fileName = string.Empty;
                string watermark = string.Empty;
                string finalFilePath = string.Empty;
                string tempPdfFilePath = string.Empty;
                string fileNameToProcess = string.Empty;
                //string errorMessage = string.Empty;
                Decrypt decryptor = new Decrypt();
                // Tiff2Pdf tiffToPdfConverter = null;
                PDFConversion objPDFConversion = null;
                string decryptFilePath = HttpContext.Server.MapPath("~/TempEncFilesDownload/");
                fileNameToProcess = fileNameWithExten.Substring(0, fileNameWithExten.IndexOf("."));// +@"-" + Environment.TickCount.ToString();
                decryptor.encFilePath = encFilePath.ToString();
                //decryptor.decFilePath = @decryptFilePath +@"\" + fileNameToProcess + ".tiff";
                decryptor.decFilePath = @decryptFilePath + fileNameToProcess + ".tiff";
                string TiffFileName = fileNameToProcess + ".tiff";
                string TiffFilePath = @decryptFilePath + @"\" + fileNameToProcess + ".tiff";

                try
                {

                    if (decryptor.DecryptFile("fecdba9876543210", ref errorMessage) == false)
                    {
                        new CommonFunctions().DeleteFileFromTemporaryFolder(decryptor.decFilePath);
                        throw new Exception("File Decryption Failed for " + fileName.ToString() + errorMessage);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(ex);
                    // Input string.
                    const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                    // Invoke GetBytes method.
                    byte[] array = Encoding.ASCII.GetBytes(input);

                    return File(array, "application/pdf");
                }
                ////--------------------------------------------------------------------------------------------
                ///

                //----------------------- CONVERT TIFF TO PDF-------------------------
                try
                {
                    tempPdfFilePath = @decryptFilePath + "Temp_" + fileNameToProcess + ".pdf";
                    objPDFConversion = new PDFConversion(@decryptor.decFilePath, tempPdfFilePath);

                    if (objPDFConversion.convertUsingImageMagick(watermark, ref errorMessage) == false)
                    {
                        throw new Exception("File conversion from Tiff to PDF Failed for " + fileName.ToString(), new Exception(errorMessage));
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(ex);
                    // Input string.
                    const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                    // Invoke GetBytes method.
                    byte[] array = Encoding.ASCII.GetBytes(input);

                    return File(array, "application/pdf");
                }
                //fileName = fileNameToProcess + ".pdf";
                //finalFilePath = @decryptFilePath + fileName;
                //objPDFConversion.srcFile = tempPdfFilePath;
                //objPDFConversion.dstFile = finalFilePath;
                ////-------------------------------------------------------------------------------------------- 

                //-------------------------Read PDF byte array from PDF file----------------------------------

                try
                {
                    byte[] pdfByteArray = System.IO.File.ReadAllBytes(tempPdfFilePath);
                    return File(pdfByteArray, "application/pdf");
                }
                catch (Exception ex)
                {
                    ExceptionLogs.LogException(ex);
                    // Input string.
                    const string input = "Due to some techinical problem this document cannot be viewed right now.Please try again later.";

                    // Invoke GetBytes method.
                    byte[] array = Encoding.ASCII.GetBytes(input);

                    return File(array, "application/pdf");
                }


                if (ResModel.IsError)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = ResModel.ErrorMessage, URLToRedirect = "/Utilities/ScannedFileDownload/ScannedFileDownloadView" });
                }

                if (ResModel != null && ResModel.ScannedFileByteArray != null)
                {
                    //string ScannedFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", FileName));
                    //FileInfo templateFile = GetFileInfo(ScannedFilePath);
                    //LogStatus = caller.PostCall<ScannedFileDownloadView, bool>("SaveScannedFileDownloadDetails", ReqModel);

                    //Response.AppendCookie(new HttpCookie("fileDownload", "true") { Path = "/", HttpOnly = false });
                    return File(ResModel.ScannedFileByteArray, System.Net.Mime.MediaTypeNames.Application.Octet, "FileName" + ".enc");
                }
                else
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "No data found for entered Final Registration Number.", URLToRedirect = "/Utilities/ScannedFileDownload/ScannedFileDownloadView" });
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Scanned File.", URLToRedirect = "/Utilities/ScannedFileDownload/ScannedFileDownloadView" });
            }

        }


    }
}