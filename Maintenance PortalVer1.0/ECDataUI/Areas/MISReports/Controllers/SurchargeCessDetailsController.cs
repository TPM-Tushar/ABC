#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   SurchargeCessDetailsController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.SurchargeCessDetails;
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
    public class SurchargeCessDetailsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// Surcharge Cess Details View
        /// </summary>
        /// <returns>returns view</returns>
        [EventAuditLogFilter(Description = "Surcharge Cess Details View")]
        public ActionResult SurchargeCessDetailsView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.SurchargeandcessDetails;
                caller = new ServiceCaller("SurchargeCessDetailsAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                SurchargeCessDetailsModel reqModel = caller.GetCall<SurchargeCessDetailsModel>("SurchargeCessDetailsView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Index II Report View", URLToRedirect = "/Home/HomePage" });
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

        /// <summary>
        /// Get Surcharge Cess Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Surcharge Cess Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult SurchargeCessDetails(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string NatureOfDocumentListID = formCollection["NatureOfDocumentListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string DROfficeID = formCollection["DROfficeID"];
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = "0",
                    errorMessage = ""
                });

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion
                #region Server Side Validation           
                ////caller = new ServiceCaller("CommonsApiController");
                ////short OfficeID = KaveriSession.Current.OfficeID;
                ////short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);
                //////Validation For DR Login
                ////if (LevelID == Convert.ToInt16(CommonEnum.LevelDetails.DR))
                ////{
                ////    //Validation for DR when user do not select any sro which is by default "Select"
                ////    if ((SroId == 0))
                ////    {
                ////        var emptyData = Json(new
                ////        {
                ////            draw = formCollection["draw"],
                ////            recordsTotal = 0,
                ////            recordsFiltered = 0,
                ////            data = "",
                ////            status = false,
                ////            errorMessage = "Please select any SRO"
                ////        });
                ////        emptyData.MaxJsonLength = Int32.MaxValue;
                ////        return emptyData;
                ////    }
                ////}
                ////else
                ////{//Validations of Logins other than SR and DR
                ////    if ((SroId == 0 && DroId == 0))//when user do not select any DR and SR which are by default "Select"
                ////    {
                ////        var emptyData = Json(new
                ////        {
                ////            draw = formCollection["draw"],
                ////            recordsTotal = 0,
                ////            recordsFiltered = 0,
                ////            data = "",
                ////            status = false,
                ////            errorMessage = "Please select any District"
                ////        });
                ////        emptyData.MaxJsonLength = Int32.MaxValue;
                ////        return emptyData;
                ////    }
                ////    else if (SroId == 0 && DroId != 0)//when User selects DR but not SR which is by default "Select"
                ////    {
                ////        var emptyData = Json(new
                ////        {
                ////            draw = formCollection["draw"],
                ////            recordsTotal = 0,
                ////            recordsFiltered = 0,
                ////            data = "",
                ////            status = false,
                ////            errorMessage = "Please select any SRO"
                ////        });
                ////        emptyData.MaxJsonLength = Int32.MaxValue;
                ////        return emptyData;
                ////    }
                ////}                

                if (String.IsNullOrEmpty(DROfficeID))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select any District."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (String.IsNullOrEmpty(SROOfficeListID))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select any SRO."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                int SroId = Convert.ToInt32(SROOfficeListID);
                int DroId = Convert.ToInt32(DROfficeID);
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
                        errorMessage = "From Date can not be larger than To Date."
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
                else if (((toDate - frmDate).TotalDays > 365))
                {
                    //Added on 18-11-2019 By RamanK
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Data of One year can be seen at a time. Please enter valid Date Range.."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;



                    //Commented on 18-11-2019 By RamanK
                    //#region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
                    //DateTime CurrentDate = DateTime.Now;
                    //int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    //int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                    //int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                    //int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                    //if (CMonth > 3)
                    //{
                    //    if (FromDateyear < CYear)
                    //    {
                    //        var emptyData = Json(new
                    //        {
                    //            draw = formCollection["draw"],
                    //            recordsTotal = 0,
                    //            recordsFiltered = 0,
                    //            data = "",
                    //            status = false,
                    //            errorMessage = "Records of current financial year only can be seen at a time"
                    //        });
                    //        emptyData.MaxJsonLength = Int32.MaxValue;
                    //        return emptyData;
                    //    }
                    //    else
                    //    {
                    //        if (FromDateyear > CYear)
                    //        {
                    //            if (FromDateyear == CYear + 1)
                    //            {
                    //                if (FromDateMonth > 3)
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
                    //        else if (FromDateyear == CYear)
                    //        {
                    //            if (FromDateMonth < 4)
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
                    //}
                    //else if (CMonth <= 3)
                    //{
                    //    if (FromDateyear < CYear)
                    //    {
                    //        if (FromDateyear == CYear - 1)
                    //        {
                    //            if (FromDateMonth < 4)
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
                    //        else
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
                    //    }
                    //}
                    //#endregion
                }
                //  ADDED BY SHUBHAM BHAGAT ON 15-07-2019 
                //  3 YEARS VALIDATION BETWEEN FROM DATE AND TO DATE
                //if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(ToDate))
                //{
                //    if ((toDate - frmDate).TotalDays > 1095)
                //    {
                //        var emptyData = Json(new
                //        {
                //            draw = formCollection["draw"],
                //            recordsTotal = 0,
                //            recordsFiltered = 0,
                //            data = "",
                //            status = "0",
                //            errorMessage = "Records of three years can be searched at a time."
                //        });
                //        emptyData.MaxJsonLength = Int32.MaxValue;
                //        return emptyData;
                //    }
                //}
                #endregion
                //if (DroId == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any District."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                //if (SroId == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "Please select any SRO."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}



                ////  ADDED BY SHUBHAM BHAGAT ON 15-07-2019 
                ////  3 YEARS VALIDATION BETWEEN FROM DATE AND TO DATE
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
                #endregion
                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum;
                int skip = startLen;
                SurchargeCessDetailsModel reqModel = new SurchargeCessDetailsModel();
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.NatureOfDocumentID = Convert.ToInt32(NatureOfDocumentListID);
                reqModel.SROfficeID = SroId;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DROfficeID = DroId;
                caller = new ServiceCaller("SurchargeCessDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                reqModel.SearchValue = searchValue;


                //To get records of Surcharge Cess report table 
                SurchargeCessDetailWrapper result = caller.PostCall<SurchargeCessDetailsModel, SurchargeCessDetailWrapper>("SurchargeCessDetails", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting surcharge and cess details." });
                }
                if (result.SurchargeCessDetailList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting surcharge and cess details." });
                }

                int totalCount = result.SurchargeCessDetailList.Count;

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
                        result.SurchargeCessDetailList = result.SurchargeCessDetailList.Where(
                            m =>
                            m.ArticleNameE.ToLower().Contains(searchValue.ToLower()) ||
                            m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
                        m.VillageNameE.ToLower().Contains(searchValue.ToLower()) ||
                        m.Executant.ToLower().Contains(searchValue.ToLower()) ||
                        m.Claimant.ToLower().Contains(searchValue.ToLower()) ||
                        m.PropertyValue.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.GovtDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        //m.AdditionalDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.CessDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.TotalStumpDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.PaidStumpDuty.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.RegisteredDatetime.ToLower().Contains(searchValue.ToLower())).ToList();
                        totalCount = result.SurchargeCessDetailList.Count();
                    }
                }

                var gridData = result.SurchargeCessDetailList.Select(SurchargeCessDetail => new
                {
                    serialNo = SurchargeCessDetail.SerialNo,
                    SroName = SurchargeCessDetail.SroName,
                    FinalRegistrationNumber = SurchargeCessDetail.FinalRegistrationNumber,
                    PropertyDetails = SurchargeCessDetail.PropertyDetails,
                    VillageNameE = SurchargeCessDetail.VillageNameE,
                    Executant = SurchargeCessDetail.Executant,
                    Claimant = SurchargeCessDetail.Claimant,
                    PropertyValue = SurchargeCessDetail.PropertyValue.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    GovtDuty = SurchargeCessDetail.GovtDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    //AdditionalDuty = SurchargeCessDetail.AdditionalDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    //Removed AdditionalDuty and added TWOPERCENT_GOVTDUTY and THREEPERCENT_GOVTDUTY by RamanK on 09-12-2019
                    TWOPERCENT_GOVTDUTY = SurchargeCessDetail.TWOPERCENT_GOVTDUTY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    THREEPERCENT_GOVTDUTY = SurchargeCessDetail.THREEPERCENT_GOVTDUTY.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    CessDuty = SurchargeCessDetail.CessDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    TotalStumpDuty = SurchargeCessDetail.TotalStumpDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    PaidStumpDuty = SurchargeCessDetail.PaidStumpDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegisteredDatetime = SurchargeCessDetail.RegisteredDatetime,
                    ArticleNameE = SurchargeCessDetail.ArticleNameE
                });

                String PDFDownloadBtn = result.SurchargeCessDetailList.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "','" + NatureOfDocumentListID + "','" + DROfficeID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.SurchargeCessDetailList.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "','" + NatureOfDocumentListID + "','" + DROfficeID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = result.TotalRecords,
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
                        recordsTotal = result.TotalRecords,
                        status = "1",
                        recordsFiltered = result.TotalRecords,
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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Surcharge Cess Details." });
            }
        }
        #region PDF

        /// <summary>
        /// Export Surcharge Cess Details To PDF
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="NatureOfDocumentListID"></param>
        /// <param name="DROOfficeListID"></param>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Surcharge Cess Details Report To PDF")]
        public ActionResult ExportSurchargeCessDetailsToPDF(string FromDate, string ToDate, string SROOfficeListID, string NatureOfDocumentListID, string DROOfficeListID, string SROSelected, string DistrictSelected)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                SurchargeCessDetailsModel model = new SurchargeCessDetailsModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    NatureOfDocumentID = Convert.ToInt32(NatureOfDocumentListID),
                    DROfficeID = Convert.ToInt32(DROOfficeListID),
                    startLen = 0,
                    totalNum = 10,
                };

                caller = new ServiceCaller("SurchargeCessDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in Surcharge Cess report datatable
                int totalCount = caller.PostCall<SurchargeCessDetailsModel, int>("SurchargeCessDetailsTotalCount", model);
                model.totalNum = totalCount;

                // To get total records of Surcharge Cess  report table
                SurchargeCessDetailWrapper objListItemsToBeExported = caller.PostCall<SurchargeCessDetailsModel, SurchargeCessDetailWrapper>("SurchargeCessDetails", model, out errorMessage);

                if (objListItemsToBeExported == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
                }

                if (objListItemsToBeExported.SurchargeCessDetailList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
                }

                string fileName = string.Format("SurcharCessDetails.pdf");
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("Surcharge and Cess Details (Between {0} and {1})", FromDate, ToDate);

                //To get SRONAME
                string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                //Create Temp PDF File
                byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported.SurchargeCessDetailList, fileName, pdfHeader, SROName, SROSelected, DistrictSelected);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "SurcharCessDetailsPDF_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
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
        /// <returns></returns>
        private byte[] CreatePDFFile(List<SurchargeCessDetail> objListItemsToBeExported, string fileName, string pdfHeader, string SROName, string SROSelected, string DistrictSelected)
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
                            var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
                            string count = (objListItemsToBeExported.Count()).ToString();
                            var countChunk = new Chunk(count, redListTextFont);
                            var SelectedSROChunk = new Chunk("SRO : ", blackListTextFont);
                            var SelectedDistrictChunk = new Chunk("District  : ", blackListTextFont);
                            var SRO = new Chunk(SROSelected + "       ", redListTextFont);
                            var DistrictName = new Chunk(DistrictSelected + "       ", redListTextFont);


                            var SROPhrase = new Phrase(SelectedSROChunk)
                        {
                            SRO
                        };

                            var DROPhrase = new Phrase(SelectedDistrictChunk)
                        {
                            DistrictName
                        };

                            var titlePhrase = new Phrase(titleChunk)
                        {
                            descriptionChunk
                        };
                            var totalPhrase = new Phrase(totalChunk)
                        {
                            countChunk
                        };

                            doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(SROPhrase);
                            doc.Add(DROPhrase);

                            doc.Add(titlePhrase);
                            //doc.Add(SroNamePhrase);
                            doc.Add(totalPhrase);
                            doc.Add(addSpace);

                            doc.Add(SurchangeCessDetailTable(objListItemsToBeExported));
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
        /// Surchange Cess Detail Table
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns></returns>
        private PdfPTable SurchangeCessDetailTable(List<SurchargeCessDetail> objListItemsToBeExported)
        {
            string serialNumber = "Serial Number";
            string SroName = "Sro Name";
            string ArticleName = "Article Name";
            string FinalRegistrationNumber = "Final Registration Number";
            string PropertyDetails = "Property Details";
            string VillageName = "Village Name";
            string Executant = "Executant";
            string Claimant = "Claimant";
            string PropertyValue = "Property Value";
            string GovtDuty = "Govt Duty";
            string AdditionalDuty = "Additional Duty";
            string CessDuty = "Cess Duty";
            string TotalStumpDuty = "Total Stump Duty";
            string PaidStumpDuty = "Paid Stump Duty";
            string RegisteredDatetime = "Registered Date Time";
            try
            {
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;
                PdfPCell cell7 = null;
                PdfPCell cell8 = null;
                PdfPCell cell9 = null;
                PdfPCell cell10 = null;
                PdfPCell cell11 = null;
                PdfPCell cell12 = null;
                PdfPCell cell13 = null;
                PdfPCell cell14 = null;
                PdfPCell cell15 = null;


                string[] col = { serialNumber, SroName ,ArticleName,FinalRegistrationNumber, PropertyDetails, VillageName, Executant,
                    Claimant, PropertyValue, GovtDuty, AdditionalDuty, CessDuty, TotalStumpDuty, PaidStumpDuty,
                    RegisteredDatetime };
                PdfPTable table = new PdfPTable(15)
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
                table.SetWidths(new Single[] { 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4 });
                /*
                * by default tables 'collapse' on surrounding elements,
                * so you need to explicitly add spacing
                */
                //table.SpacingBefore = 10;
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
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;

                    cell2 = new PdfPCell(new Phrase(items.ArticleNameE, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.BackgroundColor = BaseColor.WHITE;
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;

                    cell3 = new PdfPCell(new Phrase(items.FinalRegistrationNumber, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.PropertyDetails, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.BackgroundColor = BaseColor.WHITE;
                    cell4.HorizontalAlignment = Element.ALIGN_LEFT;

                    cell5 = new PdfPCell(new Phrase(items.VillageNameE, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.BackgroundColor = BaseColor.WHITE;
                    cell5.HorizontalAlignment = Element.ALIGN_LEFT;

                    cell6 = new PdfPCell(new Phrase(items.Executant, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell6.BackgroundColor = BaseColor.WHITE;
                    cell6.HorizontalAlignment = Element.ALIGN_LEFT;

                    cell7 = new PdfPCell(new Phrase(items.Claimant, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell7.BackgroundColor = BaseColor.WHITE;
                    cell7.HorizontalAlignment = Element.ALIGN_LEFT;

                    cell8 = new PdfPCell(new Phrase(items.PropertyValue.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell8.BackgroundColor = BaseColor.WHITE;
                    cell8.HorizontalAlignment = Element.ALIGN_RIGHT;

                    cell9 = new PdfPCell(new Phrase(items.GovtDuty.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell9.BackgroundColor = BaseColor.WHITE;
                    cell9.HorizontalAlignment = Element.ALIGN_RIGHT;

                    //cell10 = new PdfPCell(new Phrase(items.AdditionalDuty.ToString(), tableContentFont))
                    //{
                    //    BackgroundColor = new BaseColor(204, 255, 255)
                    //};
                    cell10.BackgroundColor = BaseColor.WHITE;
                    cell10.HorizontalAlignment = Element.ALIGN_RIGHT;

                    cell11 = new PdfPCell(new Phrase(items.CessDuty.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell11.BackgroundColor = BaseColor.WHITE;
                    cell11.HorizontalAlignment = Element.ALIGN_RIGHT;

                    cell12 = new PdfPCell(new Phrase(items.TotalStumpDuty.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell12.BackgroundColor = BaseColor.WHITE;
                    cell12.HorizontalAlignment = Element.ALIGN_RIGHT;

                    cell13 = new PdfPCell(new Phrase(items.PaidStumpDuty.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell13.BackgroundColor = BaseColor.WHITE;
                    cell13.HorizontalAlignment = Element.ALIGN_RIGHT;

                    cell14 = new PdfPCell(new Phrase(items.RegisteredDatetime.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell14.BackgroundColor = BaseColor.WHITE;


                    cell15 = new PdfPCell(new Phrase(items.SroName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell15.BackgroundColor = BaseColor.WHITE;
                    cell15.HorizontalAlignment = Element.ALIGN_LEFT;

                    table.AddCell(cell1);
                    table.AddCell(cell15);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                    table.AddCell(cell6);
                    table.AddCell(cell7);
                    table.AddCell(cell8);
                    table.AddCell(cell9);
                    table.AddCell(cell10);
                    table.AddCell(cell11);
                    table.AddCell(cell12);
                    table.AddCell(cell13);
                    table.AddCell(cell14);

                    //cell1 = new PdfPCell(new Phrase(items.SerialNo.ToString("F"), tableContentFont))
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
        /// Export Surcharge Cess Details To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="NatureOfDocumentListID"></param>
        /// <param name="DROOfficeListID"></param>
        /// <returns>returns excel file</returns>
        [EventAuditLogFilter(Description = "Export Surcharge Cess Details Report To Excel")]
        public ActionResult ExportSurchargeCessDetailsToExcel(string FromDate, string ToDate, string SROOfficeListID, string NatureOfDocumentListID, string DROOfficeListID, string SROSelected, string DistrictSelected)
        {
            try
            {
                caller = new ServiceCaller("SurchargeCessDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "SurchargeCessDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;

                if (string.IsNullOrEmpty(FromDate))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date required", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "To Date required", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                }

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                if (String.IsNullOrEmpty(DROOfficeListID))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any District.", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                }
                if (String.IsNullOrEmpty(SROOfficeListID))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any SRO.", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                }
                int SroId = Convert.ToInt32(SROOfficeListID);
                int DroId = Convert.ToInt32(DROOfficeListID);
                #region Validate date Inputs
                if (!boolFrmDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid From Date", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                }
                if (!boolToDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid To Date", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                }
                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                if (frmDate > toDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date can not be larger than To Date.", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                }
                //Added on 18-11-2019 To allow Date Range for one year By RamanK 
                if (((toDate - frmDate).TotalDays > 365))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Data of One year can be seen at a time. Please enter valid Date Range..", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                }

                //Commented on 18-11-2019 By RamanK
                //#region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
                //DateTime CurrentDate = DateTime.Now;
                //int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                //int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                //int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                //int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                //if (CMonth > 3)
                //{
                //    if (FromDateyear < CYear)
                //    {
                //        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                //    }
                //    else
                //    {
                //        if (FromDateyear > CYear)
                //        {
                //            if (FromDateyear == CYear + 1)
                //            {
                //                if (FromDateMonth > 3)
                //                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                //            }
                //            else
                //            {
                //                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                //            }
                //        }
                //        else if (FromDateyear == CYear)
                //        {
                //            if (FromDateMonth < 4)
                //                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                //        }
                //    }
                //}
                //else if (CMonth <= 3)
                //{
                //    if (FromDateyear < CYear)
                //    {
                //        if (FromDateyear == CYear - 1)
                //        {
                //            if (FromDateMonth < 4)
                //                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                //        }
                //        else
                //        {
                //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/SurchargeCessDetails/SurchargeCessDetailsView" });
                //        }
                //    }
                //}
                //#endregion
                #endregion

                SurchargeCessDetailsModel model = new SurchargeCessDetailsModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    NatureOfDocumentID = Convert.ToInt32(NatureOfDocumentListID),
                    DROfficeID = Convert.ToInt32(DROOfficeListID),
                    startLen = 0,
                    totalNum = 10,
                };

                caller = new ServiceCaller("SurchargeCessDetailsAPIController");
                caller.HttpClient.Timeout = objTimeSpan;
                model.IsExcel = true;

                SurchargeCessDetailWrapper objListItemsToBeExported = caller.PostCall<SurchargeCessDetailsModel, SurchargeCessDetailWrapper>("SurchargeCessDetails", model, out errorMessage);
                if (objListItemsToBeExported == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                }
                if (objListItemsToBeExported.SurchargeCessDetailList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string excelHeader = string.Format("Surcharge and Cess Detail Report Between ({0} and {1})", FromDate, ToDate);
                string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROSelected, DistrictSelected);
                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();
                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
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
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SROName"></param>
        /// <returns></returns>
        private string CreateExcel(SurchargeCessDetailWrapper objListItemsToBeExported, string fileName, string excelHeader, string SROSelected, string DistrictSelected)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Surcharge and Cess Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + DistrictSelected;
                    workSheet.Cells[3, 1].Value = "SRO : " + SROSelected;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (objListItemsToBeExported.SurchargeCessDetailList.Count());
                    workSheet.Cells[1, 1, 1, 14].Merge = true;
                    workSheet.Cells[2, 1, 2, 14].Merge = true;
                    workSheet.Cells[3, 1, 3, 14].Merge = true;
                    workSheet.Cells[4, 1, 4, 14].Merge = true;
                    workSheet.Cells[5, 1, 5, 14].Merge = true;

                    workSheet.Column(5).Style.WrapText = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Column(8).Style.WrapText = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Row(7).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 40;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Cells[7, 1].Value = "Serial No";
                    workSheet.Cells[7, 1].Value = "SRO Name";
                    workSheet.Cells[7, 2].Value = "Article Name";
                    //workSheet.Cells[7, 4].Value = "Final Registration Number";
                    //workSheet.Cells[7, 5].Value = "Property Details";
                    //workSheet.Cells[7, 6].Value = "Village Name";
                    //workSheet.Cells[7, 7].Value = "Executant";
                    //workSheet.Cells[7, 8].Value = "Claimant";
                    workSheet.Cells[7, 3].Value = "Property Value";
                    workSheet.Cells[7, 4].Value = "Govt Duty";
                    workSheet.Cells[7, 5].Value = "Additional Duty ( CORPORATION/TMC/CMC/T.P (2%) )";
                    workSheet.Cells[7, 6].Value = "Additional Duty ( TALUK PANCHAYAT (3%) )";
                    workSheet.Cells[7, 7].Value = "Cess Duty";
                    workSheet.Cells[7, 8].Value = "Total Stamp Duty";
                    //workSheet.Cells[7, 14].Value = "Paid Stamp Duty";
                    //workSheet.Cells[7, 15].Value = "Total";
                    //workSheet.Cells[7, 16].Value = "Registered Date Time";


                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Cells[7, 4].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;


                    foreach (var items in objListItemsToBeExported.SurchargeCessDetailList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 8].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 6].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 5].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 8].Style.WrapText = true;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;

                        workSheet.Cells[rowIndex, 3].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 4].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SroName;
                        workSheet.Cells[rowIndex, 2].Value = items.ArticleNameE;
                        //workSheet.Cells[rowIndex, 4].Value = items.FinalRegistrationNumber;
                        //workSheet.Cells[rowIndex, 5].Value = items.PropertyDetails;
                        //workSheet.Cells[rowIndex, 6].Value = items.VillageNameE;
                        //workSheet.Cells[rowIndex, 7].Value = items.Executant;
                        //workSheet.Cells[rowIndex, 8].Value = items.Claimant;
                        workSheet.Cells[rowIndex, 3].Value = items.PropertyValue;
                        workSheet.Cells[rowIndex, 4].Value = items.GovtDuty;
                        workSheet.Cells[rowIndex, 5].Value = items.TWOPERCENT_GOVTDUTY;
                        workSheet.Cells[rowIndex, 6].Value = items.THREEPERCENT_GOVTDUTY;
                        workSheet.Cells[rowIndex, 7].Value = items.CessDuty;
                        workSheet.Cells[rowIndex, 8].Value = items.TotalStumpDuty;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;


                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    workSheet.Cells[rowIndex, 1].Value = "";
                    workSheet.Cells[rowIndex, 2].Value = "Total";
                    workSheet.Cells[rowIndex, 3].Value = objListItemsToBeExported.Total_PropertyValue;
                    workSheet.Cells[rowIndex, 4].Value = objListItemsToBeExported.Total_GovtDuty;
                    workSheet.Cells[rowIndex, 5].Value = objListItemsToBeExported.Total_TWOPERCENT_GOVTDUTY;
                    workSheet.Cells[rowIndex, 6].Value = objListItemsToBeExported.Total_THREEPERCENT_GOVTDUTY;
                    workSheet.Cells[rowIndex, 7].Value = objListItemsToBeExported.Total_CessDuty;
                    workSheet.Cells[rowIndex, 8].Value = objListItemsToBeExported.Total_TotalStumpDuty;

                    //using (ExcelRange Rng = workSheet.Cells[rowIndex, 1, rowIndex, 16])
                    //{
                    //    Rng.Style.Font.Name = "KNB-TTUmaEN";
                    //    Rng.Style.Numberformat.Format = "0.00";
                    //    Rng.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    //}

                    workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Row(rowIndex).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(rowIndex).Style.Numberformat.Format = "0.00";

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex), 8])
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



        public ActionResult ValidateSearchParameters(string FromDate, string ToDate, string SROOfficeListID, string NatureOfDocumentListID, string DROOfficeListID)
        {
            try
            {
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                System.Text.RegularExpressions.Regex regx = new Regex("^[0-9]*$");
                Match mtchSRO = regx.Match(SROOfficeListID);
                Match mtchDistrict = regx.Match(DROOfficeListID);
                Match mtchNatureOfdDoc = regx.Match(NatureOfDocumentListID);

                if (string.IsNullOrEmpty(DROOfficeListID))
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid District" }, JsonRequestBehavior.AllowGet);

                }
                else if (!mtchDistrict.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid District" }, JsonRequestBehavior.AllowGet);

                }

                if (string.IsNullOrEmpty(SROOfficeListID))
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid SRO Office" }, JsonRequestBehavior.AllowGet);

                }
                else if (!mtchSRO.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid SRO Office" }, JsonRequestBehavior.AllowGet);

                }
                if (string.IsNullOrEmpty(NatureOfDocumentListID))
                {
                    return Json(new { success = false, errorMessage = "Please select any Nature Of Document." }, JsonRequestBehavior.AllowGet);

                }
                else if (!mtchNatureOfdDoc.Success)
                {
                    return Json(new { success = false, errorMessage = "Please select any Nature Of Document." }, JsonRequestBehavior.AllowGet);

                }

                if (string.IsNullOrEmpty(FromDate))
                {
                    return Json(new { success = false, errorMessage = "From Date required" }, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    return Json(new { success = false, errorMessage = "To Date required" }, JsonRequestBehavior.AllowGet);
                }

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);



                int SroId = Convert.ToInt32(SROOfficeListID);
                int DroId = Convert.ToInt32(DROOfficeListID);
                #region Validate date Inputs
                if (!boolFrmDate)
                {
                    return Json(new { success = false, errorMessage = "Invalid From Date" }, JsonRequestBehavior.AllowGet);
                }
                if (!boolToDate)
                {
                    return Json(new { success = false, errorMessage = "Invalid To Date" }, JsonRequestBehavior.AllowGet);
                }
                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                if (frmDate > toDate)
                {
                    return Json(new { success = false, errorMessage = "From Date can not be larger than To Date" }, JsonRequestBehavior.AllowGet);
                }
                //Added on 18-11-2019 To allow Date Range for one year By RamanK 
                if (((toDate - frmDate).TotalDays > 365))
                {
                    return Json(new { success = false, errorMessage = "Data of One year can be seen at a time. Please enter valid Date Range.." }, JsonRequestBehavior.AllowGet);
                }

                //Commented on 18-11-2019 By RamanK
                //#region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
                //DateTime CurrentDate = DateTime.Now;
                //int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                //int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                //int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                //int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                //if (CMonth > 3)
                //{
                //    if (FromDateyear < CYear)
                //    {
                //        return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);
                //    }
                //    else
                //    {
                //        if (FromDateyear > CYear)
                //        {
                //            if (FromDateyear == CYear + 1)
                //            {
                //                if (FromDateMonth > 3)
                //                    return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);
                //            }
                //            else
                //            {
                //                return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);
                //            }
                //        }
                //        else if (FromDateyear == CYear)
                //        {
                //            if (FromDateMonth < 4)
                //                return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);
                //        }
                //    }
                //}
                //else if (CMonth <= 3)
                //{
                //    if (FromDateyear < CYear)
                //    {
                //        if (FromDateyear == CYear - 1)
                //        {
                //            if (FromDateMonth < 4)
                //                return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);
                //        }
                //        else
                //        {
                //            return Json(new { success = false, errorMessage = "Records of current financial year only can be seen at a time" }, JsonRequestBehavior.AllowGet);

                //        }
                //    }
                //}
                //#endregion
                #endregion
                if ((toDate - frmDate).TotalDays > 365)//six months validation by RamanK on 20-09-2019
                {
                    return Json(new { success = false, errorMessage = "Data of only one year can be seen at a time" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, errorMessage = "" }, JsonRequestBehavior.AllowGet);


            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        #endregion
    }
}