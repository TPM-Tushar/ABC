#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   JurisdictionalWiseController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.JurisdictionalWise;
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
    public class JurisdictionalWiseController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// Jurisdictional Wise View
        /// </summary>
        /// <returns>Jurisdictional Wise View</returns>
        [EventAuditLogFilter(Description = "Jurisdictional Wise View")]
        public ActionResult JurisdictionalWiseView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.JurisdictionalWiseReport;
                caller = new ServiceCaller("JurisdictionalWiseAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                JurisdictionalWiseModel reqModel = caller.GetCall<JurisdictionalWiseModel>("JurisdictionalWiseView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Index II Report View", URLToRedirect = "/Home/HomePage" });
            }
        }

        ///// <summary>
        ///// Get SRO Office List By District ID
        ///// </summary>
        ///// <param name="DistrictID"></param>
        ///// <returns>returns SRO Office list</returns>
        //[HttpGet]
        //public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        //{
        //    try
        //    {
        //        List<SelectListItem> sroOfficeList = new List<SelectListItem>();
        //        ServiceCaller caller = new ServiceCaller("CommonsApiController");
        //        sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "Select" }, out errormessage);
        //        return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
        //    }
        //}

        /// <summary>
        /// JurisdictionalWiseDetail
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Jurisdictional Wise Detail list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Jurisdictional Wise Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult JurisdictionalWiseDetail(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                //string DROfficeID = formCollection["DROfficeID"];
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

                //if (String.IsNullOrEmpty(DROfficeID))
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

                if (String.IsNullOrEmpty(SROOfficeListID))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select any Jurisdictional Office."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                int SroId = Convert.ToInt32(SROOfficeListID);
                //int DroId = Convert.ToInt32(DROfficeID);

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
                //        errorMessage = "Please select any Jurisdictional Office."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
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
                else
                {
                    #region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
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
                    #endregion
                    if ((toDate - frmDate).TotalDays > 365)//six months validation by RamanK on 20-09-2019
                    {
                        var emptyData = Json(new
                        {
                            draw = formCollection["draw"],
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = "0",
                            errorMessage = "Data of one year can be seen at a time"
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;

                    }
                }
                #endregion

                #region Validate date Inputs
               

                ////  ADDED BY SHUBHAM BHAGAT ON 18-07-2019 
                ////  3 YEARS VALIDATION BETWEEN FROM DATE AND TO DATE
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

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);

                int pageSize = totalNum;
                int skip = startLen;

                JurisdictionalWiseModel reqModel = new JurisdictionalWiseModel();
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = SroId;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;
                //reqModel.DROfficeID = DroId;

                caller = new ServiceCaller("JurisdictionalWiseAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in Surcharge Cess report datatable
                reqModel.SearchValue = searchValue;


                //To get records of Surcharge Cess report table 
                JurisdictionalWiseDetailWrapper result = caller.PostCall<JurisdictionalWiseModel, JurisdictionalWiseDetailWrapper>("JurisdictionalWiseDetail", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Jurisdictional Wise details." });
                }
                if (result.JurisdictionalWiseDetailList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Jurisdictional Wise details." });
                }


                int totalCount = result.JurisdictionalWiseDetailList.Count;
 

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
                        result.JurisdictionalWiseDetailList = result.JurisdictionalWiseDetailList.Where(
                            m =>
                            m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                            m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                        m.StumpDuty.ToString().Contains(searchValue.ToLower()) ||
                        m.RegistrationFees.ToString().Contains(searchValue.ToLower()) ||
                        m.Total.ToString().Contains(searchValue.ToLower())).ToList();
                        totalCount = result.JurisdictionalWiseDetailList.Count();
                    }
                }

                var gridData = result.JurisdictionalWiseDetailList.Select(JurisdictionalWiseDetail => new
                {
                    SerialNo = JurisdictionalWiseDetail.SerialNo,
                    // CHANGES DONE BY SHUBHAM BHAGAT ON 03-10-2019 FOR DATA ISSUES
                    //JurisdictionalOffice = JurisdictionalWiseDetail.SROName,
                    JurisdictionalOffice = JurisdictionalWiseDetail.JurisdictionalOffice,
                    SROName = JurisdictionalWiseDetail.SROName,
                    FinalRegistrationNumber = JurisdictionalWiseDetail.FinalRegistrationNumber,
                    StumpDuty = JurisdictionalWiseDetail.StumpDuty.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegistrationFees = JurisdictionalWiseDetail.RegistrationFees.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Total = JurisdictionalWiseDetail.Total.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                });

                String PDFDownloadBtn = result.JurisdictionalWiseDetailList.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID  + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.JurisdictionalWiseDetailList.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID +  "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Jurisdictional Wise Details." });
            }
        }

        /// <summary>
        /// Jurisdictional Wise Summary
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Jurisdictional Wise Summary</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Jurisdictional Wise Summary")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult JurisdictionalWiseSummary(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                //string DROfficeID = formCollection["DROfficeID"];

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                #region Server Side Validation       

                //if (String.IsNullOrEmpty(DROfficeID))
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any District.", URLToRedirect = "/Home/HomePage" });
                //}

                if (String.IsNullOrEmpty(SROOfficeListID))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any Jurisdictional Office.", URLToRedirect = "/Home/HomePage" });
                }

                int SroId = Convert.ToInt32(SROOfficeListID);
                //int DroId = Convert.ToInt32(DROfficeID);

                //if (DroId == 0)
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any District.", URLToRedirect = "/Home/HomePage" });
                //}

                //if (SroId == 0)
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any Jurisdictional Office.", URLToRedirect = "/Home/HomePage" });
                //}

                if (string.IsNullOrEmpty(fromDate))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date required.", URLToRedirect = "/Home/HomePage" });
                }

                if (string.IsNullOrEmpty(ToDate))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "To Date required.", URLToRedirect = "/Home/HomePage" });
                }
                #endregion

                DateTime frmDate, toDate;
                bool boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                bool boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                #region Validate date Inputs
                if (!boolFrmDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid From Date.", URLToRedirect = "/Home/HomePage" });
                }
                if (!boolToDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid To Date.", URLToRedirect = "/Home/HomePage" });
                }
                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                if (frmDate > toDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date can not be larger than To Date.", URLToRedirect = "/Home/HomePage" });
                }

                //  ADDED BY SHUBHAM BHAGAT ON 15-07-2019 
                //  3 YEARS VALIDATION BETWEEN FROM DATE AND TO DATE
                if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(ToDate))
                {
                    if ((toDate - frmDate).TotalDays > 1095)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of three years can be searched at a time.", URLToRedirect = "/Home/HomePage" });
                    }
                }               
                #endregion

                JurisdictionalWiseModel reqModel = new JurisdictionalWiseModel();
                reqModel.SROfficeID = SroId;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;
                //reqModel.DROfficeID = DroId;

                caller = new ServiceCaller("JurisdictionalWiseAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                JurisdictionalWiseSummary jurisdictionalWiseSummary = caller.PostCall<JurisdictionalWiseModel, JurisdictionalWiseSummary>("JurisdictionalWiseSummary", reqModel, out errorMessage);
                if (jurisdictionalWiseSummary == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Jurisdictional Wise Summary details", URLToRedirect = "/Home/HomePage" });
                }

                return View(jurisdictionalWiseSummary);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
            }
        }
        
        #region PDF        
        [EventAuditLogFilter(Description = "Jurisdictional Wise Report To PDF")]
        public ActionResult JurisdictionalWiseToPDF(string FromDate, string ToDate, string SROOfficeListID,string MaxDate)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                JurisdictionalWiseModel model = new JurisdictionalWiseModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    //DROfficeID = Convert.ToInt32(DROOfficeListID),
                    StartLen = 0,
                    TotalNum = 10,
                };

                caller = new ServiceCaller("JurisdictionalWiseAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in Jurisdictional Wise report datatable
                int totalCount = caller.PostCall<JurisdictionalWiseModel, int>("JurisdictionalWiseTotalCount", model);
                model.TotalNum = totalCount;

                // To get total records of Jurisdictional Wise report table
                JurisdictionalWiseDetailWrapper objListItemsToBeExported = caller.PostCall<JurisdictionalWiseModel, JurisdictionalWiseDetailWrapper>("JurisdictionalWiseDetail", model, out errorMessage);

                if (objListItemsToBeExported == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
                }

                if (objListItemsToBeExported.JurisdictionalWiseDetailList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
                }

                // For Summary 
                JurisdictionalWiseSummary jurisdictionalWiseSummary = caller.PostCall<JurisdictionalWiseModel, JurisdictionalWiseSummary>("JurisdictionalWiseSummary", model, out errorMessage);
                if (jurisdictionalWiseSummary == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
                }

                string fileName = string.Format("JurisdictionalWise.pdf");
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("Jurisdictional Wise Report Details (Between {0} and {1})", FromDate, ToDate);

                //To get SRONAME
                string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                //Create Temp PDF File
                byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported.JurisdictionalWiseDetailList, fileName, pdfHeader, SROName, jurisdictionalWiseSummary,MaxDate);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "JurisdictionalWiseReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
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
        private byte[] CreatePDFFile(List<JurisdictionalWiseDetail> objListItemsToBeExported, string fileName, string pdfHeader, string SROName, JurisdictionalWiseSummary jurisdictionalWiseSummary,string MaxDate)
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
                            var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);
                            var SroName = new Chunk(SROName + "       ", redListTextFont);
                            var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
                            string count = (objListItemsToBeExported.Count()).ToString();
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

                            // For Adding Jurisdiction Summary Table
                           
                            string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
                            string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
                            BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                            iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 14);

                            string SerialNo = "Serial No";
                            string JurisdictionalSRO = "Jurisdictional Office";
                            string SRONameST = "SRO Name";
                            string Documents = "Documents";
                            string StumpDuty = "Stamp Duty";
                            string RegistrationFees = " Registration Fees";
                            string Total = "Total";

                            PdfPCell cell = null;

                            cell = new PdfPCell(new Phrase())
                            {
                                BackgroundColor = new BaseColor(226, 226, 226)
                            };

                            string[] colSummary = { SerialNo, JurisdictionalSRO, SRONameST, Documents, StumpDuty, RegistrationFees, Total };
                            PdfPTable tableSummary = new PdfPTable(7)
                            {
                                WidthPercentage = 70
                            };
                            tableSummary.HeaderRows = 1;


                            tableSummary.SetWidths(new Single[] { 8, 8, 8, 8, 8, 8, 8 });

                            for (int i = 0; i < colSummary.Length; ++i)
                            {
                                cell = new PdfPCell(new Phrase(colSummary[i]))
                                {
                                    BackgroundColor = new BaseColor(204, 255, 255)
                                };
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                tableSummary.AddCell(cell);
                            }

                            PdfPCell cell1 = new PdfPCell(new Phrase(jurisdictionalWiseSummary.SerialNo.ToString(), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell1.BackgroundColor = BaseColor.WHITE;

                            PdfPCell cell2 = new PdfPCell(new Phrase(jurisdictionalWiseSummary.JurisdictionalOffice, tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell2.BackgroundColor = BaseColor.WHITE;

                            PdfPCell cell3 = new PdfPCell(new Phrase(jurisdictionalWiseSummary.SROName, tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell3.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell3.BackgroundColor = BaseColor.WHITE;

                            PdfPCell cell4 = new PdfPCell(new Phrase(jurisdictionalWiseSummary.Documents.ToString(), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell4.BackgroundColor = BaseColor.WHITE;

                            PdfPCell cell5 = new PdfPCell(new Phrase(jurisdictionalWiseSummary.StumpDuty.ToString("F"), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell5.BackgroundColor = BaseColor.WHITE;

                            PdfPCell cell6 = new PdfPCell(new Phrase(jurisdictionalWiseSummary.RegistrationFees.ToString("F"), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell6.BackgroundColor = BaseColor.WHITE;

                            PdfPCell cell7 = new PdfPCell(new Phrase(jurisdictionalWiseSummary.Total.ToString("F"), tableContentFont))
                            {
                                BackgroundColor = new BaseColor(204, 255, 255)
                            };
                            cell7.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell7.BackgroundColor = BaseColor.WHITE;

                            tableSummary.AddCell(cell1);
                            tableSummary.AddCell(cell2);
                            tableSummary.AddCell(cell3);
                            tableSummary.AddCell(cell4);
                            tableSummary.AddCell(cell5);
                            tableSummary.AddCell(cell6);
                            tableSummary.AddCell(cell7);
                            var FontItalic = FontFactory.GetFont("Arial", 10, 2, new BaseColor(94, 94, 94));
                            Paragraph NotePara = new Paragraph("*This report is based on pre processed data considered upto : " + MaxDate, FontItalic);
                            NotePara.Alignment = Element.ALIGN_RIGHT;

                            doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(titlePhrase);
                            doc.Add(SroNamePhrase);
                            doc.Add(totalPhrase);
                            doc.Add(addSpace);
                            doc.Add(NotePara);
                            doc.Add(addSpace);

                            // Added Summary table
                            doc.Add(tableSummary);                            
                            // Added Details table
                            doc.Add(JurisdictionalWiseTable(objListItemsToBeExported));
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
        /// Jurisdictional Wise Table
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns>Jurisdictional Wise Table</returns>
        private PdfPTable JurisdictionalWiseTable(List<JurisdictionalWiseDetail> objListItemsToBeExported)
        {
            string serialNumber = "Serial No";
            string JurisdictionalOffice = "Jurisdictional Office";
            string SROName = "SRO Name";
            string FinalRegistrationNumber = "Final Registration Number";
            string StumpDuty = "Stamp Duty";
            string RegistrationFees = "Registration Fees";
            string Total = "Total";
            try
            {
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;
                PdfPCell cell7 = null;

                string[] col = { serialNumber, JurisdictionalOffice, SROName, FinalRegistrationNumber, StumpDuty, RegistrationFees, Total };
                PdfPTable table = new PdfPTable(7)
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
                table.SetWidths(new Single[] { 4, 4, 4, 4, 4, 4, 4 });
                /*
                * by default tables 'collapse' on surrounding elements,
                * so you need to explicitly add spacing
                */
                table.SpacingBefore = 15f;
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

                    // CHANGES DONE BY SHUBHAM BHAGAT ON 03-10-2019 FOR DATA ISSUES
                    //cell2 = new PdfPCell(new Phrase(items.SROName, tableContentFont))
                    cell2 = new PdfPCell(new Phrase(items.JurisdictionalOffice, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.BackgroundColor = BaseColor.WHITE;
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;

                    cell3 = new PdfPCell(new Phrase(items.SROName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.BackgroundColor = BaseColor.WHITE;
                    cell3.HorizontalAlignment = Element.ALIGN_LEFT;

                    cell4 = new PdfPCell(new Phrase(items.FinalRegistrationNumber, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.BackgroundColor = BaseColor.WHITE;
                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;

                    cell5 = new PdfPCell(new Phrase(items.StumpDuty.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.BackgroundColor = BaseColor.WHITE;
                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;

                    cell6 = new PdfPCell(new Phrase(items.RegistrationFees.ToString("F"), tableContentFont))
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
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                    table.AddCell(cell6);
                    table.AddCell(cell7);

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
        //[EventAuditLogFilter(Description = "Export Surcharge Cess Details Report To Excel")]
        public ActionResult ExportJurisdictionalWiseToExcel(string FromDate, string ToDate, string SROOfficeListID, string NatureOfDocumentListID,string MaxDate)
        {
            try
            {
                caller = new ServiceCaller("JurisdictionalWiseAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = "JurisdictionalWiseReport_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                string SROName = string.Empty;
                JurisdictionalWiseModel model = new JurisdictionalWiseModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    StartLen = 0,
                    TotalNum = 10,
                };
                if (SROOfficeListID != "0")
                {
                     SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                    if (SROName == null)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                    }
                }
                else {
                    SROName = "All";

                }

                caller = new ServiceCaller("JurisdictionalWiseAPIController");
                TimeSpan objTimeSpan2 = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan2;
                model.IsExcel = true;


                // // For Detail list 
                JurisdictionalWiseDetailWrapper objListItemsToBeExported = caller.PostCall<JurisdictionalWiseModel, JurisdictionalWiseDetailWrapper>("JurisdictionalWiseDetail", model, out errorMessage);
                if (objListItemsToBeExported == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                }
                if (objListItemsToBeExported.JurisdictionalWiseDetailList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                }


                // For Summary 
                //JurisdictionalWiseSummary jurisdictionalWiseSummary = caller.PostCall<JurisdictionalWiseModel, JurisdictionalWiseSummary>("JurisdictionalWiseSummary", model, out errorMessage);
                //if (jurisdictionalWiseSummary == null)
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                //}

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string excelHeader = string.Format("Jurisdictional Wise Report Between ({0} and {1})", FromDate, ToDate);
                
                string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName, null, MaxDate);
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
        private string CreateExcel(JurisdictionalWiseDetailWrapper objListItemsToBeExported, string fileName, string excelHeader, string SROName, JurisdictionalWiseSummary jurisdictionalWiseSummary,string MaxDate)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Jurisdictional Wise Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (objListItemsToBeExported.JurisdictionalWiseDetailList.Count())+ "                                                                                                                                 *This report is based on pre processed data considered upto : "+MaxDate;
                    workSheet.Cells[1, 1, 1, 7].Merge = true;
                    workSheet.Cells[2, 1, 2, 7].Merge = true;
                    workSheet.Cells[3, 1, 3, 7].Merge = true;
                    workSheet.Cells[4, 1, 4, 7].Merge = true;
                    //workSheet.Column(9).Style.WrapText = true;
                    //workSheet.Column(10).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;

                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //// For Jurisdictional Summary 
                    //workSheet.Cells[6, 1].Value = "Serial No";
                    //workSheet.Cells[6, 2].Value = "Jurisdictional Office";
                    //// CHANGES DONE BY SHUBHAM BHAGAT ON 03-10-2019 FOR DATA ISSUES
                    ////workSheet.Cells[6, 3].Value = "SRO Name";
                    //workSheet.Cells[6, 3].Value = "Documents";
                    //workSheet.Cells[6, 4].Value = "Stamp Duty";
                    //workSheet.Cells[6, 5].Value = "Registration Fees";
                    //workSheet.Cells[6, 6].Value = "Total";
                    
                    //workSheet.Cells[7, 1].Value = jurisdictionalWiseSummary.SerialNo;
                    //// CHANGES DONE BY SHUBHAM BHAGAT ON 03-10-2019 FOR DATA ISSUES
                    ////workSheet.Cells[7, 2].Value = jurisdictionalWiseSummary.SROName;
                    //workSheet.Cells[7, 2].Value = jurisdictionalWiseSummary.JurisdictionalOffice;
                    //// CHANGES DONE BY SHUBHAM BHAGAT ON 03-10-2019 FOR DATA ISSUES
                    ////workSheet.Cells[7, 3].Value = jurisdictionalWiseSummary.SROName;
                    //workSheet.Cells[7, 3].Value = jurisdictionalWiseSummary.Documents;
                    //workSheet.Cells[7, 4].Value = jurisdictionalWiseSummary.StumpDuty;
                    //workSheet.Cells[7, 5].Value = jurisdictionalWiseSummary.RegistrationFees;
                    //workSheet.Cells[7, 6].Value = jurisdictionalWiseSummary.Total;

                    //workSheet.Cells[7, 4].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[7, 5].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[7, 6].Style.Numberformat.Format = "0.00";

                    //workSheet.Cells[7, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Cells[7, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Cells[7, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //workSheet.Cells[7, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //workSheet.Cells[7, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    int rowIndex = 7;
                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[6, 1].Value = "Serial No";
                    workSheet.Cells[6, 2].Value = "Jurisdictional Office";
                    workSheet.Cells[6, 3].Value = "SRO Name";
                    workSheet.Cells[6, 4].Value = "Final Registration Number";
                    workSheet.Cells[6, 5].Value = "Stamp Duty";
                    workSheet.Cells[6, 6].Value = "Registration Fees";
                    workSheet.Cells[6, 7].Value = "Total";
                    
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in objListItemsToBeExported.JurisdictionalWiseDetailList)
                    {
                        workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 6].Style.Font.Name = "KNB-TTUmaEN";
                        workSheet.Cells[rowIndex, 7].Style.Font.Name = "KNB-TTUmaEN";

                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.JurisdictionalOffice;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.StumpDuty;
                        workSheet.Cells[rowIndex, 6].Value = items.RegistrationFees;
                        workSheet.Cells[rowIndex, 7].Value = items.Total;

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
                    // commented by shubham bhagat on 05-07-2019
                    // to stop making bold last line in excel 
                    //workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 7])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    // For Summary
                    //using (ExcelRange Rng = workSheet.Cells[6, 1, 7, 6])
                    //{
                    //    Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    //    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
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
        #endregion
    }
}