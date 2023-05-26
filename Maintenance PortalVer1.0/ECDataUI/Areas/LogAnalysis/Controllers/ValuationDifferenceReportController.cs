using CustomModels.Models.Common;
using CustomModels.Models.LogAnalysis.ValuationDifferenceReport;
using CustomModels.Security;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.LogAnalysis.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ValuationDifferenceReportController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        // GET: LogAnalysis/ValuationDifferenceReport

        [MenuHighlightAttribute]
        public ActionResult ValuationDifferenceReportView()
        {

            int OfficeID = KaveriSession.Current.OfficeID;
            caller = new ServiceCaller("ValuationDifferenceReportAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            ValuationDiffReportViewModel reqModel = caller.GetCall<ValuationDiffReportViewModel>("ValuationDifferenceReportView", new { OfficeID = OfficeID });
            if (reqModel == null)
            {
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Valuation Difference View", URLToRedirect = "/Home/HomePage" });
            }
            return View(reqModel);
        }

        /// <summary>
        /// GetValuationDiffRptData
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>GetValuation Wise Detail list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Valuation Difference")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetValuationDiffRptData(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                //string fromDate = formCollection["FromDate"];
                //string ToDate = formCollection["ToDate"];
                //string SROOfficeListID = formCollection["SROOfficeListID"];
                //string DROfficeID = formCollection["DROfficeID"];
                //DateTime frmDate, toDate;
                //bool boolFrmDate = false;
                //bool boolToDate = false;
                int PropertyTypeListID = Convert.ToInt32(formCollection["PropertyTypeListID"]);
                string RegArticleIdArr = Convert.ToString(formCollection["RegArticleIdArr"]);

                String searchValue = Request.Form.GetValues("search[value]").FirstOrDefault().TrimEnd();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                System.Text.RegularExpressions.Regex regxForpropertyTypeID = new Regex("^[0-9]*$");
                Match mtchPropertyTypeID = regxForpropertyTypeID.Match(Convert.ToString(PropertyTypeListID));
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                //boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = ""
                });
                if (PropertyTypeListID <= 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Property Type"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (!mtchPropertyTypeID.Success)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select valid Property Type"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                #region Server Side Validation           

                #endregion

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                if (totalNum < 0)
                    totalNum = 350;
                int pageSize = totalNum;
                int skip = startLen;

                ValuationDiffReportViewModel reqModel = new ValuationDiffReportViewModel();
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;
                reqModel.strRegArtId = RegArticleIdArr;

                //reqModel.FromDate = fromDate;
                //reqModel.ToDate = ToDate;
                //reqModel.SROfficeID = SroId;
                //reqModel.DateTime_ToDate = toDate;
                //reqModel.DateTime_FromDate = frmDate;
                //reqModel.DROfficeID = DroId;

                caller = new ServiceCaller("ValuationDifferenceReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in Surcharge Cess report datatable
                reqModel.SearchValue = searchValue;
                reqModel.PropertyID = PropertyTypeListID;

                //To get records of Surcharge Cess report table 
                ValuationDiffReportDataModel result = caller.PostCall<ValuationDiffReportViewModel, ValuationDiffReportDataModel>("GetValuationDiffRptData", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Valuation Analysis RPT." });
                }
                if (result.ValuationDiffReportRecList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Valuation Analysis RPT." });
                }


                //   int totalCount = result.ValuationDiffReportRecList.Count;
                int totalCount = result.TotalRecords;
                string sParams = "'" + RegArticleIdArr + "','" + PropertyTypeListID + "'";
                String ExcelDownloadBtn = totalCount == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=\"DownloadEXCELSummary(" + sParams + ")\"><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";


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
                        result.ValuationDiffReportRecList = result.ValuationDiffReportRecList.Where(
                            m =>
                            m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                            m.SerialNo.ToString().Contains(searchValue.ToLower()) ||
                            m.StampDutyRecovery.ToLower().Contains(searchValue.ToLower()) ||
                            m.Registration_Fees_Recovery__Probable_.ToLower().Contains(searchValue.ToLower()) ||
                            m.Total.ToLower().Contains(searchValue.ToLower()) ||
                        m.TansactionsDone.ToLower().Contains(searchValue.ToLower())).ToList();
                        totalCount = result.ValuationDiffReportRecList.Count();
                    }
                }

                //var gridData = result.ValuationDiffReportRecList.Select(JurisdictionalWiseDetail => new
                //{
                //    SerialNo = JurisdictionalWiseDetail.,
                //    // CHANGES DONE BY SHUBHAM BHAGAT ON 03-10-2019 FOR DATA ISSUES
                //    //JurisdictionalOffice = JurisdictionalWiseDetail.SROName,
                //    JurisdictionalOffice = JurisdictionalWiseDetail.JurisdictionalOffice,
                //    SROName = JurisdictionalWiseDetail.SROName,
                //    FinalRegistrationNumber = JurisdictionalWiseDetail.FinalRegistrationNumber,
                //    StumpDuty = JurisdictionalWiseDetail.StumpDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                //    RegistrationFees = JurisdictionalWiseDetail.RegistrationFees.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                //    Total = JurisdictionalWiseDetail.Total.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                //});


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = result.ValuationDiffReportRecList.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = result.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn

                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = result.ValuationDiffReportRecList.ToArray(),
                        recordsTotal = totalCount,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn


                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Valuation Analysis." });
            }
        }

        /// <summary>
        /// GetValuationDiffRptData
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>GetValuation Wise Detail list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Valuation Difference")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetValuationDiffDetailedData(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                //string fromDate = formCollection["FromDate"];
                //string ToDate = formCollection["ToDate"];
                //string SROOfficeListID = formCollection["SROOfficeListID"];
                //string DROfficeID = formCollection["DROfficeID"];
                //DateTime frmDate, toDate;
                //bool boolFrmDate = false;
                //bool boolToDate = false;
                int SROCode = Convert.ToInt32(formCollection["SROCode"]);
                int PropertyTypeListID = Convert.ToInt32(formCollection["PropertyTypeListID"]);
                string PropertyType = Convert.ToString(formCollection["PropertyTypeName"]);
                string SROName = Convert.ToString(formCollection["SROName"]);
                string RegArticleIdArr = Convert.ToString(formCollection["RegArticleIdArr"]);

                String searchValue = Request.Form.GetValues("search[value]").FirstOrDefault().TrimEnd();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                //boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = ""
                });

                String errorMessage = String.Empty;
                #endregion

                #region Server Side Validation           

                #endregion

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);

                int pageSize = totalNum;
                int skip = startLen;

                ValuationDiffReportViewModel reqModel = new ValuationDiffReportViewModel();
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;
                //reqModel.FromDate = fromDate;
                //reqModel.ToDate = ToDate;
                //reqModel.SROfficeID = SroId;
                //reqModel.DateTime_ToDate = toDate;
                //reqModel.DateTime_FromDate = frmDate;
                //reqModel.DROfficeID = DroId;

                caller = new ServiceCaller("ValuationDifferenceReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in Surcharge Cess report datatable
                reqModel.SearchValue = searchValue;
                reqModel.PropertyID = PropertyTypeListID;
                reqModel.SROCode = SROCode;
                reqModel.strRegArtId = RegArticleIdArr;

                //To get records of Surcharge Cess report table 
                ValuationDiffReportDataModel result = caller.PostCall<ValuationDiffReportViewModel, ValuationDiffReportDataModel>("GetValuationDiffDetailedData", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Valuation Analysis RPT." });
                }
                if (result.ValuationDiffReportDetailedRecList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Valuation Analysis RPT." });
                }


                int totalCount = result.ValuationDiffReportDetailedRecList.Count;


                //Sorting
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                //}

                string sParams = "'" + SROCode + "','" + PropertyTypeListID + "','" + PropertyType + "','" + SROName + "','" + RegArticleIdArr + "'";
                String ExcelDownloadBtn = totalCount == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=\"EXCELDownloadFun(" + sParams + ")\"><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

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
                        result.ValuationDiffReportDetailedRecList = result.ValuationDiffReportDetailedRecList.Where(
                        m =>
                        m.RegistrationDate.ToLower().Contains(searchValue.ToLower()) ||
                        m.AreaName.ToLower().Contains(searchValue.ToLower()) ||
                        m.GuidancePerSquareFeetRate.ToString().Contains(searchValue.ToLower()) ||
                        m.Measurement.ToString().Contains(searchValue.ToLower()) ||
                        m.Consideration.ToString().Contains(searchValue.ToLower()) ||
                        m.RegisteredGuidanceValue.ToString().Contains(searchValue.ToLower()) ||
                        m.PaidStampDuty.ToString().Contains(searchValue.ToLower()) ||
                        m.PayableStampDuty.ToString().Contains(searchValue.ToLower()) ||
                        m.StampDutyDifference.ToString().Contains(searchValue.ToLower()) ||
                        m.Result.ToLower().Contains(searchValue.ToLower()) ||
                        // ADDED BY SHUBHAM BHAGAT ON 6-3-2020
                        m.Registered_Per_Square_Feet_Rate.ToString().Contains(searchValue.ToLower()) ||
                        m.Measurement__Square_Feet_.ToString().Contains(searchValue.ToLower()) ||

                    m.RegisteredPerSquareFeetRate.ToString().Contains(searchValue.ToLower())).ToList();
                        totalCount = result.ValuationDiffReportDetailedRecList.Count();
                    }
                }

                var gridData = result.ValuationDiffReportDetailedRecList.Select(ValuationDiffRptDetailedRecordModel => new
                {

                    RegistrationDate = ValuationDiffRptDetailedRecordModel.RegistrationDate,
                    FinalRegistrationNumber = ValuationDiffRptDetailedRecordModel.FinalRegistrationNumber,
                    AreaName = ValuationDiffRptDetailedRecordModel.AreaName,
                    NatureOfDocument = ValuationDiffRptDetailedRecordModel.NatureOfDocument,
                    GuidancePerSquareFeetRate = ValuationDiffRptDetailedRecordModel.GuidancePerSquareFeetRate.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Registration_dump = ValuationDiffRptDetailedRecordModel.Registration_dump,
                    Consideration = ValuationDiffRptDetailedRecordModel.Consideration.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    PaidStampDuty = ValuationDiffRptDetailedRecordModel.PaidStampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegFeePaid = ValuationDiffRptDetailedRecordModel.RegFeePaid.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegisteredGuidanceValue = ValuationDiffRptDetailedRecordModel.RegisteredGuidanceValue.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    PayableStampDuty = ValuationDiffRptDetailedRecordModel.PayableStampDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    StampDutyDifference = ValuationDiffRptDetailedRecordModel.StampDutyDifference.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    payableRegFee = ValuationDiffRptDetailedRecordModel.payableRegFee.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegFeeDifference = ValuationDiffRptDetailedRecordModel.RegFeeDifference.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    TotalDifference = ValuationDiffRptDetailedRecordModel.TotalDifference.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),


                    Measurement__Guntas = ValuationDiffRptDetailedRecordModel.Measurement__Guntas,
                    Registered_Per_Gunta_Rate = ValuationDiffRptDetailedRecordModel.Registered_Per_Gunta_Rate,
                    Apartment_Name = ValuationDiffRptDetailedRecordModel.Apartment_Name,
                    Super_Builtup_Area_shown_in_Document = ValuationDiffRptDetailedRecordModel.Super_Builtup_Area_shown_in_Document,
                    Rate_as_per_G_V_notification_01_01_2019 = ValuationDiffRptDetailedRecordModel.Rate_as_per_G_V_notification_01_01_2019,
                    Total_Value_on_Super_Builtup_Area = ValuationDiffRptDetailedRecordModel.Total_Value_on_Super_Builtup_Area,

                    TotalPayable = ValuationDiffRptDetailedRecordModel.TotalPayable,
                    Market_Value_calculated_as_per_document_at_the_time_of_Registration = ValuationDiffRptDetailedRecordModel.Market_Value_calculated_as_per_document_at_the_time_of_Registration,
                    TotalPaid = ValuationDiffRptDetailedRecordModel.TotalPaid,
                    Difference_between_the_Two = ValuationDiffRptDetailedRecordModel.Difference_between_the_Two,

                    // ADDED BY SHUBHAM BHAGAT ON 6-3-2020
                    Registered_Per_Square_Feet_Rate = ValuationDiffRptDetailedRecordModel.Registered_Per_Square_Feet_Rate.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Measurement__Square_Feet_ = ValuationDiffRptDetailedRecordModel.Measurement__Square_Feet_.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),

                    ClickToViewDocument = ValuationDiffRptDetailedRecordModel.ClickToViewDocument
                    //Measurement = ValuationDiffRptDetailedRecordModel.Measurement.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),

                    //RegisteredPerSquareFeetRate = ValuationDiffRptDetailedRecordModel.RegisteredPerSquareFeetRate.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    //Result = ValuationDiffRptDetailedRecordModel.Result,
                });


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = result.TotalRecords,
                        status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                }
                else
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray(),
                        recordsTotal = result.TotalRecords,
                        status = "1",
                        recordsFiltered = result.TotalRecords,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                }
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Valuation Difference." });
            }
        }

        #region Excel
        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SroID"></param>
        /// <param name="SelectedSRO"></param>
        /// <param name="SelectedDistrict"></param>
        /// <returns>returns excel file</returns>
        //[EventAuditLogFilter(Description = "Sro DD PO Collection Report Export To Excel")]
        //public ActionResult ExportToExcel(string FromDate, string ToDate, string SroID, string SelectedSRO)
        [EventAuditLogFilter(Description = "Export Valuation Difference to EXCEL")]
        public ActionResult ExportValuationDiffRptToExcel(string SROCode, string PropertyTypeID, string PropertyType, string SROName, string RegArticleIdArr)
        {
            try
            {
                caller = new ServiceCaller("ValuationDifferenceReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName;
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                ValuationDiffReportViewModel model = new ValuationDiffReportViewModel
                {

                    SROCode = Convert.ToInt32(SROCode),
                    PropertyID = Convert.ToInt32(PropertyTypeID),
                    IsExcel = true,
                    strRegArtId = RegArticleIdArr
                };

                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                //ValuationDiffReportDataModel ResModel = new ValuationDiffReportDataModel();
                //caller = new ServiceCaller("ECDailyReceiptReportAPIController");
                //caller.HttpClient.Timeout = objTimeSpan;
                //int totalCount = caller.PostCall<ECDailyReceiptRptView, int>("GetECDailyReceiptsTotalCount", model);
                //model.totalNum = totalCount;
                model.IsExcel = true;
                ValuationDiffReportDataModel ResModel = caller.PostCall<ValuationDiffReportViewModel, ValuationDiffReportDataModel>("GetValuationDiffDetailedData", model, out errorMessage);
                if (ResModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Valuation Analysis RPT." });
                }
                if (ResModel.ValuationDiffReportDetailedRecList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Valuation Analysis RPT." });
                }
                //ResModel = caller.PostCall<ValuationDiffReportViewModel, ValuationDiffReportDataModel>("GetValuationDiffDetailedData", model, out errorMessage);
                //if (ResModel.ValuationDiffReportDetailedRecList == null)
                //{

                //    return Json(new { success = false, errorMessage = "Error Occured While Getting Valuation Analysis Report..." }, JsonRequestBehavior.AllowGet);

                //}


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Valuation Analysis Report");
                string createdExcelPath;
                int ch = Convert.ToInt32(PropertyTypeID);
                switch (ch)
                {
                    case 1://OfficeOpenXml Built Rate
                        fileName = string.Format("ValuationAnalysis" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                        createdExcelPath = CreateExcelForOpenBuiltRate(ResModel, fileName, excelHeader, PropertyType, SROName);
                        break;

                    case 2://Agriculture
                        fileName = string.Format("ValuationAnalysis" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                        createdExcelPath = CreateExcelForAgriculture(ResModel, fileName, excelHeader, PropertyType, SROName);
                        break;

                    case 3://Apartment -A (Urban)
                        fileName = string.Format("ValuationAnalysis" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                        createdExcelPath = CreateExcelForApartment(ResModel, fileName, excelHeader, PropertyType, SROName);
                        break;

                    case 4://Apartment -A (Rural)
                        fileName = string.Format("ValuationAnalysis" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                        createdExcelPath = CreateExcelForApartment(ResModel, fileName, excelHeader, PropertyType, SROName);
                        break;

                    default://Default
                        fileName = string.Format("ValuationAnalysis" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                        createdExcelPath = CreateExcelForOpenBuiltRate(ResModel, fileName, excelHeader, PropertyType, SROName);
                        break;

                }

                //string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, PropertyType, SROName);
                // string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);


                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, SROName + "_Valuation Data Analysis" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SelectedDistrict"></param>
        /// <param name="SelectedSRO"></param>
        /// <returns>returns excel file path</returns>

        private string CreateExcelForOpenBuiltRate(ValuationDiffReportDataModel ResModel, string fileName, string excelHeader, string PropertyType, string SROName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Valuation Analysis Report");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Property Type : " + PropertyType;
                    workSheet.Cells[3, 1].Value = "SRO : " + SROName;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.ValuationDiffReportDetailedRecList.Count());
                    workSheet.Cells[1, 1, 1, 17].Merge = true;
                    workSheet.Cells[2, 1, 2, 17].Merge = true;
                    workSheet.Cells[3, 1, 3, 17].Merge = true;
                    workSheet.Cells[4, 1, 4, 17].Merge = true;
                    workSheet.Cells[5, 1, 5, 17].Merge = true;
                    workSheet.Cells[6, 1, 6, 17].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 45;
                    workSheet.Column(5).Width = 40;
                    //workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 40;
                    workSheet.Column(8).Width = 55;
                    workSheet.Column(9).Width = 30;
                    workSheet.Column(10).Width = 35;
                    workSheet.Column(11).Width = 53;
                    workSheet.Column(12).Width = 30;
                    workSheet.Column(13).Width = 40;
                    workSheet.Column(14).Width = 40;
                    workSheet.Column(15).Width = 40;
                    workSheet.Column(16).Width = 40;
                    workSheet.Column(17).Width = 40;
                    //workSheet.Column(16).Width = 40;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                    workSheet.Row(8).Style.Font.Bold = true;
                    workSheet.Row(9).Style.Font.Bold = true;
                    workSheet.Row(10).Style.Font.Bold = true;
                    workSheet.Row(11).Style.Font.Bold = true;

                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(11).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(9).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(10).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(11).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "MARKET VALUE DIFFERENCE AND LOSS OF REVENUE CALCULATION FROM 01.01.2019 TO TILL DATE";
                    workSheet.Cells[8, 1].Value = "Property Type : " + PropertyType;
                    workSheet.Cells[9, 1].Value = "Registration Details";
                    workSheet.Cells[9, 11].Value = "Stamp Duty Calculation Details On Guidance Value (G1)";
                    workSheet.Cells[9, 12].Value = "Registration Fee Calculation Details On Guidance Value";
                    workSheet.Cells[9, 15].Value = "Difference";


                    //workSheet.Cells[7, 1].Value = "";
                    //workSheet.Cells[7, 2].Value = "";
                    //workSheet.Cells[7, 3].Value = "";
                    //workSheet.Cells[7, 4].Value = "";
                    //workSheet.Cells[7, 5].Value = "";
                    //workSheet.Cells[7, 6].Value = "";
                    //workSheet.Cells[7, 7].Value = "";
                    //workSheet.Cells[7, 8].Value = "";
                    //workSheet.Cells[7, 9].Value = "";
                    //workSheet.Cells[7, 10].Value = "";
                    //workSheet.Cells[7, 11].Value = "";
                    //workSheet.Cells[7, 12].Value = "";
                    //workSheet.Cells[7, 13].Value = "";
                    //workSheet.Cells[7, 14].Value = "";
                    //workSheet.Cells[7, 15].Value = "";
                    //workSheet.Cells[7, 16].Value = "";
                    workSheet.Cells[7, 1, 7, 17].Merge = true;
                    workSheet.Cells[8, 1, 8, 17].Merge = true;
                    workSheet.Cells[9, 1, 9, 9].Merge = true;
                    workSheet.Cells[9, 15, 9, 16].Merge = true;
                    //workSheet.Cells[9, 14, 9, 15].Merge = true;

                    workSheet.Cells[11, 1].Value = "";
                    workSheet.Cells[11, 2].Value = "";
                    workSheet.Cells[11, 3].Value = "";
                    workSheet.Cells[11, 4].Value = "";
                    workSheet.Cells[11, 5].Value = "R1";
                    workSheet.Cells[11, 6].Value = "R2";
                    workSheet.Cells[11, 7].Value = "R3=R1/R2";
                    workSheet.Cells[11, 8].Value = "";
                    workSheet.Cells[11, 9].Value = "R4";
                    workSheet.Cells[11, 10].Value = "G1";
                    workSheet.Cells[11, 11].Value = "G2=5.65 % of (G1*R2)";
                    workSheet.Cells[11, 12].Value = "G4=1 % of (G1*R2)";
                    workSheet.Cells[11, 13].Value = "R5";
                    workSheet.Cells[11, 14].Value = "G5";
                    workSheet.Cells[11, 15].Value = "G3=G2-R5";
                    workSheet.Cells[11, 16].Value = "G6=G4-G5";
                    workSheet.Cells[11, 17].Value = "G7=G3+G6";


                    workSheet.Cells[10, 1].Value = "Registration Date";
                    workSheet.Cells[10, 2].Value = "Final Registration Number";
                    workSheet.Cells[10, 3].Value = "Nature Of Document";
                    workSheet.Cells[10, 4].Value = "Area Name";
                    workSheet.Cells[10, 5].Value = "Guidance Value adopted at the time of RGN (₹)";
                    workSheet.Cells[10, 6].Value = "Measurement (Square feet)";
                    workSheet.Cells[10, 7].Value = "Per Square Feet Rate (₹)";
                    workSheet.Cells[10, 8].Value = "Building Measurement in Sq.Ft Building Rate adopted at the time of RGN";
                    workSheet.Cells[10, 9].Value = "Consideration Amount (₹)";
                    workSheet.Cells[10, 10].Value = "Guidance Value (Latest Rates after Jan 2019) (₹)";
                    workSheet.Cells[10, 11].Value = "Payable Stamp Duty (₹)";
                    workSheet.Cells[10, 12].Value = "Payable Reg Fee (₹)";
                    workSheet.Cells[10, 13].Value = "Stamp duty paid on Document at the time of Registration (₹)";
                    workSheet.Cells[10, 14].Value = "Registration Fee paid on Document at the time of Registration (₹)";
                    workSheet.Cells[10, 15].Value = "Stamp Duty Difference (₹)";
                    workSheet.Cells[10, 16].Value = "Reg Fee Difference (₹)";
                    workSheet.Cells[10, 17].Value = "Total Difference (₹)";
                    //workSheet.Cells[10, 16].Value = "Total Difference in Duty (13 + 15 = 16)";

                    //workSheet.Cells[7, 12].Value = "Click to view document";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(10).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(11).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.WrapText = true;
                    workSheet.Row(8).Style.WrapText = true;
                    workSheet.Row(9).Style.WrapText = true;
                    workSheet.Row(10).Style.WrapText = true;
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    int rowIndex = 12;
                    foreach (var items in ResModel.ValuationDiffReportDetailedRecList)
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

                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        //workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 10].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 11].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 12].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 13].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 14].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 15].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 16].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 17].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.RegistrationDate;
                        workSheet.Cells[rowIndex, 2].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 3].Value = items.NatureOfDocument;
                        workSheet.Cells[rowIndex, 4].Value = items.AreaName;
                        workSheet.Cells[rowIndex, 5].Value = items.GuidancePerSquareFeetRate;
                        workSheet.Cells[rowIndex, 6].Value = items.Measurement__Square_Feet_;
                        workSheet.Cells[rowIndex, 7].Value = items.Registered_Per_Square_Feet_Rate;
                        workSheet.Cells[rowIndex, 8 ].Value = items.Registration_dump;
                        workSheet.Cells[rowIndex, 9 ].Value = items.Consideration;
                        workSheet.Cells[rowIndex, 10].Value = items.RegisteredGuidanceValue ;
                        workSheet.Cells[rowIndex, 11].Value = items.PayableStampDuty;
                        workSheet.Cells[rowIndex, 12].Value = items.payableRegFee;
                        workSheet.Cells[rowIndex, 13].Value = items.PaidStampDuty;
                        workSheet.Cells[rowIndex, 14].Value = items.RegFeePaid;
                        workSheet.Cells[rowIndex, 15].Value = items.StampDutyDifference;
                        workSheet.Cells[rowIndex, 16].Value = items.RegFeeDifference;
                        workSheet.Cells[rowIndex, 17].Value = items.TotalDifference;

                        workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;                        
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 14].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 15].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 16].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 17].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        rowIndex++;
                    }
                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 17])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 1])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 17])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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
        private string CreateExcelForAgriculture(ValuationDiffReportDataModel ResModel, string fileName, string excelHeader, string PropertyType, string SROName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Valuation Analysis Report");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Property Type : " + PropertyType;
                    //workSheet.Cells[3, 1].Value = "SRO : " + SROName;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (ResModel.ValuationDiffReportDetailedRecList.Count());
                    workSheet.Cells[1, 1, 1, 16].Merge = true;
                    workSheet.Cells[2, 1, 2, 16].Merge = true;
                    workSheet.Cells[3, 1, 3, 16].Merge = true;
                    workSheet.Cells[4, 1, 4, 16].Merge = true;
                    workSheet.Cells[5, 1, 5, 16].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 40;
                    //workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 35;
                    workSheet.Column(9).Width = 53;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 40;
                    workSheet.Column(12).Width = 40;
                    workSheet.Column(13).Width = 40;
                    workSheet.Column(14).Width = 40;
                    workSheet.Column(15).Width = 40;
                    workSheet.Column(16).Width = 40;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                    workSheet.Row(8).Style.Font.Bold = true;
                    workSheet.Row(9).Style.Font.Bold = true;
                    workSheet.Row(10).Style.Font.Bold = true;

                    int rowIndex = 11;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Row(6).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(8).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(9).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    workSheet.Cells[6, 1].Value = "MARKET VALUE DIFFERENCE AND LOSS OF REVENUE CALCULATION FROM 01.01.2019 TO TILL DATE";
                    workSheet.Cells[7, 1].Value = "Property Type : " + PropertyType;
                    workSheet.Cells[8, 1].Value = "Registration Details";
                    workSheet.Cells[8, 11].Value = "Guidance Value (Latest Rates after 01.01.2019)";
                    workSheet.Cells[8, 12].Value = "Stamp Duty Calculation Details On Guidance Value (G1)";
                    workSheet.Cells[8, 14].Value = "Registration Fee Calculation Details On Guidance Value (G1)";
                    workSheet.Cells[8, 16].Value = "Total Difference in Duty (₹)";


                    //workSheet.Cells[7, 1].Value = "";
                    //workSheet.Cells[7, 2].Value = "";
                    //workSheet.Cells[7, 3].Value = "";
                    //workSheet.Cells[7, 4].Value = "";
                    //workSheet.Cells[7, 5].Value = "";
                    //workSheet.Cells[7, 6].Value = "";
                    //workSheet.Cells[7, 7].Value = "";
                    //workSheet.Cells[7, 8].Value = "";
                    //workSheet.Cells[7, 9].Value = "";
                    //workSheet.Cells[7, 10].Value = "";
                    //workSheet.Cells[7, 11].Value = "";
                    //workSheet.Cells[7, 12].Value = "";
                    //workSheet.Cells[7, 13].Value = "";
                    //workSheet.Cells[7, 14].Value = "";
                    //workSheet.Cells[7, 15].Value = "";
                    //workSheet.Cells[7, 16].Value = "";
                    workSheet.Cells[6, 1, 6, 16].Merge = true;
                    workSheet.Cells[7, 1, 7, 16].Merge = true;
                    workSheet.Cells[8, 1, 8, 10].Merge = true;
                    workSheet.Cells[8, 12, 8, 13].Merge = true;
                    workSheet.Cells[8, 14, 8, 15].Merge = true;

                    workSheet.Cells[10, 1].Value = "";
                    workSheet.Cells[10, 2].Value = "";
                    workSheet.Cells[10, 3].Value = "";
                    workSheet.Cells[10, 4].Value = "";
                    workSheet.Cells[10, 5].Value = "R1";
                    workSheet.Cells[10, 6].Value = "R2";
                    workSheet.Cells[10, 7].Value = "R3=R1/R2";
                    workSheet.Cells[10, 8].Value = "R4";
                    workSheet.Cells[10, 9].Value = "R5";
                    workSheet.Cells[10, 10].Value = "R6";
                    workSheet.Cells[10, 11].Value = "G1";
                    workSheet.Cells[10, 12].Value = "G2=5.65 % of (G1*R2)";
                    workSheet.Cells[10, 13].Value = "G3=G2-R5";
                    workSheet.Cells[10, 14].Value = "G4=1 % of (G1*R2)";
                    workSheet.Cells[10, 15].Value = "G5=G4-R6";
                    workSheet.Cells[10, 16].Value = "G6=G3+G5";

                    workSheet.Cells[9, 1].Value = "Registration Date";
                    workSheet.Cells[9, 2].Value = "Final Registration Number";
                    workSheet.Cells[9, 3].Value = "Nature Of Document";
                    workSheet.Cells[9, 4].Value = "Area Name";
                    workSheet.Cells[9, 5].Value = "Guidance Value adopted at the time of RGN (₹)";
                    workSheet.Cells[9, 6].Value = "Measurement (Guntas)";
                    workSheet.Cells[9, 7].Value = "Per Gunta Rate";
                    workSheet.Cells[9, 8].Value = "Consideration Amount (₹)";
                    workSheet.Cells[9, 9].Value = "Stamp Duty Paid (₹)";
                    workSheet.Cells[9, 10].Value = "Registration Fee Paid (₹)";
                    workSheet.Cells[9, 11].Value = "";
                    workSheet.Cells[9, 12].Value = "Payable Stamp Duty (₹)";
                    workSheet.Cells[9, 13].Value = "Stamp Duty Difference (₹)";
                    workSheet.Cells[9, 14].Value = "Payable Reg Fee (₹)";
                    workSheet.Cells[9, 15].Value = "Reg Fee Difference (₹)";
                    workSheet.Cells[9, 16].Value = "";

                    //workSheet.Cells[7, 12].Value = "Click to view document";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(10).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(11).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.WrapText = true;
                    workSheet.Row(7).Style.WrapText = true;
                    workSheet.Row(8).Style.WrapText = true;
                    workSheet.Row(9).Style.WrapText = true;
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.ValuationDiffReportDetailedRecList)
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

                        //workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 11].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 10].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.RegistrationDate;
                        workSheet.Cells[rowIndex, 2].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 3].Value = items.NatureOfDocument;
                        workSheet.Cells[rowIndex, 4].Value = items.AreaName;
                        workSheet.Cells[rowIndex, 5].Value = items.GuidancePerSquareFeetRate;
                        workSheet.Cells[rowIndex, 6].Value = items.Measurement__Guntas;
                        workSheet.Cells[rowIndex, 7].Value = items.Registered_Per_Gunta_Rate;
                        workSheet.Cells[rowIndex, 8].Value = items.Consideration;
                        workSheet.Cells[rowIndex, 9].Value = items.PaidStampDuty;
                        workSheet.Cells[rowIndex, 10].Value = items.RegFeePaid;
                        workSheet.Cells[rowIndex, 11].Value = items.RegisteredGuidanceValue;
                        workSheet.Cells[rowIndex, 12].Value = items.PayableStampDuty;
                        workSheet.Cells[rowIndex, 13].Value = items.StampDutyDifference;
                        workSheet.Cells[rowIndex, 14].Value = items.payableRegFee;
                        workSheet.Cells[rowIndex, 15].Value = items.RegFeeDifference;
                        workSheet.Cells[rowIndex, 16].Value = items.TotalDifference;

                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 14].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 15].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 16].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;
                    }
                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 16])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 1])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[6, 1, 7, 16])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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
        private string CreateExcelForApartment(ValuationDiffReportDataModel ResModel, string fileName, string excelHeader, string PropertyType, string SROName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Valuation Analysis Report");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Property Type : " + PropertyType;
                    workSheet.Cells[3, 1].Value = "SRO : " + SROName;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (ResModel.ValuationDiffReportDetailedRecList.Count());
                    workSheet.Cells[1, 1, 1, 17].Merge = true;
                    workSheet.Cells[2, 1, 2, 17].Merge = true;
                    workSheet.Cells[3, 1, 3, 17].Merge = true;
                    workSheet.Cells[4, 1, 4, 17].Merge = true;
                    workSheet.Cells[5, 1, 5, 17].Merge = true;
                    workSheet.Cells[6, 1, 6, 17].Merge = true;
                    workSheet.Cells[7, 1, 7, 17].Merge = true;
                    workSheet.Cells[8, 1, 8, 17].Merge = true;
                    workSheet.Cells[9, 9, 9, 12].Merge = true;
                    workSheet.Cells[9, 14, 9, 16].Merge = true;


                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 40;
                    //workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 35;
                    workSheet.Column(9).Width = 53;
                    workSheet.Column(10).Width = 30;
                    workSheet.Column(11).Width = 40;
                    workSheet.Column(12).Width = 40;
                    workSheet.Column(13).Width = 40;
                    workSheet.Column(14).Width = 40;
                    workSheet.Column(15).Width = 40;
                    workSheet.Column(16).Width = 40;
                    workSheet.Column(17).Width = 40;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                    workSheet.Row(8).Style.Font.Bold = true;
                    workSheet.Row(9).Style.Font.Bold = true;
                    workSheet.Row(10).Style.Font.Bold = true;

                    int rowIndex = 11;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(8).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(9).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(10).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "MARKET VALUE DIFFERENCE AND LOSS OF REVENUE CALCULATION FROM 01.01.2019 TO TILL DATE";
                    workSheet.Cells[8, 1].Value = "Property Type :" + PropertyType;
                    String DutyPayableHead = String.Empty;
                    String DutyPayableHead2 = String.Empty;

                    if (PropertyType == "Apartment -A (Urban)")
                    {
                        DutyPayableHead = "Duty Payable @ 5.6% as conveyance";
                        DutyPayableHead2 = "Stamp Duty Payable @ 5.6%";
                    }
                    else
                    {
                        DutyPayableHead = "Duty Payable @ 5.65 % as conveyance";
                        DutyPayableHead2 = "Stamp Duty Payable @ 5.65%";
                    }

                    workSheet.Cells[9, 9].Value = DutyPayableHead;
                    workSheet.Cells[9, 14].Value = "Duty Paid";
                    //workSheet.Cells[9, 12].Value = "Stamp Duty Calculation Details On Guidance Value (G1)";
                    //workSheet.Cells[9, 14].Value = "Registration Fee Calculation Details On Guidance Value (G1)";
                    //workSheet.Cells[9, 16].Value = "Total Difference in Duty (13 + 15 = 16)";


                    //workSheet.Cells[7, 1].Value = "";
                    //workSheet.Cells[7, 2].Value = "";
                    //workSheet.Cells[7, 3].Value = "";
                    //workSheet.Cells[7, 4].Value = "";
                    //workSheet.Cells[7, 5].Value = "";
                    //workSheet.Cells[7, 6].Value = "";
                    //workSheet.Cells[7, 7].Value = "";
                    //workSheet.Cells[7, 8].Value = "";
                    //workSheet.Cells[7, 9].Value = "";
                    //workSheet.Cells[7, 10].Value = "";
                    //workSheet.Cells[7, 11].Value = "";
                    //workSheet.Cells[7, 12].Value = "";
                    //workSheet.Cells[7, 13].Value = "";
                    //workSheet.Cells[7, 14].Value = "";
                    //workSheet.Cells[7, 15].Value = "";
                    //workSheet.Cells[7, 16].Value = "";
                    //workSheet.Cells[7, 1, 7, 18].Merge = true;
                    //workSheet.Cells[8, 1, 8, 18].Merge = true;
                    //workSheet.Cells[9, 1, 9, 10].Merge = true;
                    //workSheet.Cells[9, 12, 9, 13].Merge = true;
                    //workSheet.Cells[9, 14, 9, 15].Merge = true;

                    //workSheet.Cells[11, 1].Value = "";
                    //workSheet.Cells[11, 2].Value = "";
                    //workSheet.Cells[11, 3].Value = "";
                    //workSheet.Cells[11, 4].Value = "R1";
                    //workSheet.Cells[11, 5].Value = "R2";
                    //workSheet.Cells[11, 6].Value = "R3=R1/R2";
                    //workSheet.Cells[11, 7].Value = "R4";
                    //workSheet.Cells[11, 8].Value = "R5";
                    //workSheet.Cells[11, 9].Value = "G1";
                    //workSheet.Cells[11, 10].Value = "G2=5.65 % of (G1*R2)";
                    //workSheet.Cells[11, 11].Value = "G3=G2-R5";

                    workSheet.Cells[10, 1].Value = "Registration Date";
                    workSheet.Cells[10, 2].Value = "Final Registration Number";
                    workSheet.Cells[10, 3].Value = "Nature Of Document";
                    workSheet.Cells[10, 4].Value = "Consideration Amount (₹)";
                    workSheet.Cells[10, 5].Value = "Area Name";
                    workSheet.Cells[10, 6].Value = "Apartment Name";
                    workSheet.Cells[10, 7].Value = "Super Builtup Area shown in document";
                    workSheet.Cells[10, 8].Value = "Rate as per G.V notification 01.01.2019";
                    workSheet.Cells[10, 9].Value = "Total Value on Super Builtup Area";
                    workSheet.Cells[10, 10].Value = "Stamp Duty";
                    workSheet.Cells[10, 11].Value = "Fees";
                    workSheet.Cells[10, 12].Value = "Total";                    
                    workSheet.Cells[10, 13].Value = "Market Value calculated as per document at the time of Registration";
                    workSheet.Cells[10, 14].Value = DutyPayableHead2;
                    workSheet.Cells[10, 15].Value = "Fees";
                    workSheet.Cells[10, 16].Value = "Total";
                    workSheet.Cells[10, 17].Value = "Difference between the Two";


                    //workSheet.Cells[7, 12].Value = "Click to view document";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(10).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(11).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.WrapText = true;
                    workSheet.Row(8).Style.WrapText = true;
                    workSheet.Row(9).Style.WrapText = true;
                    workSheet.Row(10).Style.WrapText = true;
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    foreach (var items in ResModel.ValuationDiffReportDetailedRecList)
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

                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 10].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 11].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 12].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 13].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 14].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 15].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 16].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 17].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.RegistrationDate;
                        workSheet.Cells[rowIndex, 2].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 3].Value = items.NatureOfDocument;
                        workSheet.Cells[rowIndex, 4].Value = items.Consideration;
                        workSheet.Cells[rowIndex, 5].Value = items.AreaName;
                        workSheet.Cells[rowIndex, 6].Value = items.Apartment_Name;
                        workSheet.Cells[rowIndex, 7].Value = items.Super_Builtup_Area_shown_in_Document;//"HARD CODED BECAUSE NOT AVAILABLE";
                        workSheet.Cells[rowIndex, 8].Value = items.Rate_as_per_G_V_notification_01_01_2019;
                        workSheet.Cells[rowIndex, 9].Value = items.Total_Value_on_Super_Builtup_Area;
                        workSheet.Cells[rowIndex, 10].Value = items.PayableStampDuty;//"HARD CODED BECAUSE NOT AVAILABLE";
                        workSheet.Cells[rowIndex, 11].Value = items.payableRegFee;
                        workSheet.Cells[rowIndex, 12].Value = items.TotalPayable;
                        workSheet.Cells[rowIndex, 13].Value = items.Market_Value_calculated_as_per_document_at_the_time_of_Registration;
                        workSheet.Cells[rowIndex, 14].Value = items.PaidStampDuty;
                        workSheet.Cells[rowIndex, 15].Value = items.RegFeePaid;
                        workSheet.Cells[rowIndex, 16].Value = items.TotalPaid;
                        workSheet.Cells[rowIndex, 17].Value = items.Difference_between_the_Two;

                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 14].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 15].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 16].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 17].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        rowIndex++;
                    }
                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 17])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 1])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 17])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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
        [HttpGet]
        public ActionResult GetValuationDocumentPopup(string EncryptedId)
        {
            try
            {
                if (string.IsNullOrEmpty(EncryptedId))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Parameters are required", URLToRedirect = "/Home/HomePage" });

                }

                Dictionary<String, String> decryptedParameters = null;
                String[] encryptedParameters = null;
                encryptedParameters = EncryptedId.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");


                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                string FinalRegistrationNumber = decryptedParameters["FRN"];
                int SROCode = Convert.ToInt32(decryptedParameters["SROCODE"]);

                ValuationDiffFileModel model = new ValuationDiffFileModel();
                model.encryptedId = EncryptedId;
                model.Heading = FinalRegistrationNumber;
                return View("GetValuationDocument", model);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpGet]
        public ActionResult GetValuationDocument(string EncryptedId)
        {

            try
            {
                caller = new ServiceCaller("ValuationDifferenceReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;


                ValuationDiffFileModel reqModel = new ValuationDiffFileModel();
                reqModel.encryptedId = EncryptedId;

                string errorMessage = string.Empty;
                FileDisplayModel result = caller.PostCall<ValuationDiffFileModel, FileDisplayModel>("GetValuationDocument", reqModel, out errorMessage);

                if (result == null)
                    return RedirectToAction("ShowErrorMessageWithoutBack", "Error", new { area = "", message = "Error occured while getting valuation document detail, please contact admin", URLToRedirect = "" });


                if (!result.isFileExist)
                    return RedirectToAction("ShowErrorMessageWithoutBack", "Error", new { area = "", message = "File not found , please contact admin", URLToRedirect = "" });
                else
                    return File(result.fileBytes, "application/pdf");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[HttpPost]

        public ActionResult ExportValuationDiffSummaryToExcel(string RegArticleIdArr, string PropertyTypeListID,string PropertyTypeName)
        {
            try
            {
                caller = new ServiceCaller("ValuationDifferenceReportAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName;
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                ValuationDiffReportViewModel reqModel = new ValuationDiffReportViewModel();
    
                reqModel.strRegArtId = RegArticleIdArr;
                reqModel.PropertyID = Convert.ToInt32(PropertyTypeListID);
                reqModel.StartLen = 0;
                reqModel.TotalNum = 350;
                // string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}

                //ValuationDiffReportDataModel ResModel = new ValuationDiffReportDataModel();
                //caller = new ServiceCaller("ECDailyReceiptReportAPIController");
                //caller.HttpClient.Timeout = objTimeSpan;
                //int totalCount = caller.PostCall<ECDailyReceiptRptView, int>("GetECDailyReceiptsTotalCount", model);
                //model.totalNum = totalCount;
                reqModel.IsExcel = true;
                ValuationDiffReportDataModel ResModel = caller.PostCall<ValuationDiffReportViewModel, ValuationDiffReportDataModel>("GetValuationDiffRptData", reqModel, out errorMessage);

                if (ResModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Valuation Analysis Summary RPT." });
                }
                if (ResModel.ValuationDiffReportRecList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Valuation Analysis Summary RPT." });
                }
                //ResModel = caller.PostCall<ValuationDiffReportViewModel, ValuationDiffReportDataModel>("GetValuationDiffDetailedData", model, out errorMessage);
                //if (ResModel.ValuationDiffReportDetailedRecList == null)
                //{

                //    return Json(new { success = false, errorMessage = "Error Occured While Getting Valuation Analysis Report..." }, JsonRequestBehavior.AllowGet);

                //}


                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();

                //}
                string excelHeader = string.Format("Valuation Data Analysis Summary");
                string createdExcelPath;

                fileName = string.Format("ValuationDataAnalysisSummary" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
                createdExcelPath = CreateExcelForSummaryTable(ResModel, fileName, excelHeader, PropertyTypeName);
                //string createdExcelPath = CreateExcel(ResModel, fileName, excelHeader, PropertyType, SROName);
                // string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader);


                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();

                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet,  "_Valuation Data Analysis_Summary" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        
       private string CreateExcelForSummaryTable(ValuationDiffReportDataModel ResModel, string fileName, string excelHeader, string PropertyTypeName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Valuation Analysis Report");
                    workSheet.Cells.Style.Font.Size = 14;
                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Property Type : " + PropertyTypeName;
                    workSheet.Cells[3, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (ResModel.ValuationDiffReportRecList.Count());
                    //workSheet.Cells[1, 1, 1, 17].Merge = true;
                    //workSheet.Cells[2, 1, 2, 17].Merge = true;
                    //workSheet.Cells[3, 1, 3, 17].Merge = true;
                    //workSheet.Cells[4, 1, 4, 17].Merge = true;
                    //workSheet.Cells[5, 1, 5, 17].Merge = true;
                    //workSheet.Cells[6, 1, 6, 17].Merge = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 45;
                    workSheet.Column(5).Width = 40;
                    //workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 40;
                   
                    //workSheet.Column(16).Width = 40;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                    //workSheet.Row(8).Style.Font.Bold = true;
                    //workSheet.Row(9).Style.Font.Bold = true;
                    //workSheet.Row(10).Style.Font.Bold = true;
                    //workSheet.Row(11).Style.Font.Bold = true;

                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(8).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                    workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(10).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(11).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(9).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(10).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                    workSheet.Row(11).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;




                    //workSheet.Cells[7, 1].Value = "";
                    //workSheet.Cells[7, 2].Value = "";
                    //workSheet.Cells[7, 3].Value = "";
                    //workSheet.Cells[7, 4].Value = "";
                    //workSheet.Cells[7, 5].Value = "";
                    //workSheet.Cells[7, 6].Value = "";
                    //workSheet.Cells[7, 7].Value = "";
                    //workSheet.Cells[7, 8].Value = "";
                    //workSheet.Cells[7, 9].Value = "";
                    //workSheet.Cells[7, 10].Value = "";
                    //workSheet.Cells[7, 11].Value = "";
                    //workSheet.Cells[7, 12].Value = "";
                    //workSheet.Cells[7, 13].Value = "";
                    //workSheet.Cells[7, 14].Value = "";
                    //workSheet.Cells[7, 15].Value = "";
                    //workSheet.Cells[7, 16].Value = "";
                    workSheet.Cells[1, 1, 1, 6].Merge = true;
                    workSheet.Cells[2, 1, 2, 6].Merge = true;
                    workSheet.Cells[3, 1, 3, 6].Merge = true;
                    workSheet.Cells[4, 1, 4, 6].Merge = true;
                    workSheet.Cells[5, 1, 5, 6].Merge = true;




                    workSheet.Cells[7, 1].Value = "Serial Number";
                    workSheet.Cells[7, 2].Value = "SR Office";
                    workSheet.Cells[7, 3].Value = "Total Occurrences";
                    workSheet.Cells[7, 4].Value = "Difference in Stamp Duty (Probable) (₹)";
                    workSheet.Cells[7, 5].Value = "Difference in Registration Fees (Probable) (₹)";
                    workSheet.Cells[7, 6].Value = "Total Difference (₹)";
                   

                    //workSheet.Cells[7, 12].Value = "Click to view document";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(10).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(11).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.WrapText = true;
                    workSheet.Row(8).Style.WrapText = true;
                    workSheet.Row(9).Style.WrapText = true;
                    workSheet.Row(10).Style.WrapText = true;
                    workSheet.Cells[7, 8].Style.WrapText = true;

                    int rowIndex = 8;
                    foreach (var items in ResModel.ValuationDiffReportRecList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                       
                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";


                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SROName;
                        workSheet.Cells[rowIndex, 3].Value = items.TansactionsDoneForExcel;
                        workSheet.Cells[rowIndex, 4].Value = items.StampDutyRecoveryForExcel;
                        workSheet.Cells[rowIndex, 5].Value = items.Registration_Fees_Recovery__Probable_ForExcel;
                        workSheet.Cells[rowIndex, 6].Value = items.TotalForExcel;
                      

                        //workSheet.Cells[rowIndex, 3].Style.WrapText = true;
                        //workSheet.Cells[rowIndex, 4].Style.WrapText = true;
                        //workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        //workSheet.Cells[rowIndex, 7].Style.WrapText = true;
                        //workSheet.Cells[rowIndex, 8].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 11].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 12].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 13].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 14].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 15].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 16].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 17].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        rowIndex++;
                    }
                    workSheet.Row(rowIndex-1).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 6])
                    {

                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    using (ExcelRange Rng = workSheet.Cells[1, 1, 1, 1])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    }
                    using (ExcelRange Rng = workSheet.Cells[7, 1, 7, 6])
                    {
                        Rng.Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
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


    }
}