#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ExemptionDocumentController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.ExemptionDocument;
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
    public class ExemptionDocumentController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// Exemption Document View
        /// </summary>
        /// <returns>Exemption Document View</returns>
        [EventAuditLogFilter(Description = "Exempted Document View")]
        public ActionResult ExemptionDocumentView()
        {
            try
            {
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ExemptedDocument;
                caller = new ServiceCaller("ExemptionDocumentAPIController");
                int OfficeID = KaveriSession.Current.OfficeID;
                ExemptionDocumentModel reqModel = caller.GetCall<ExemptionDocumentModel>("ExemptionDocumentView", new { OfficeID = OfficeID });
                return View(reqModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Exempted Document View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Exemption Document Detail
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>Exemption Document Detail model list</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Exempted Document Detail")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult ExemptionDocumentDetail(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeListID = formCollection["SROOfficeListID"];

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

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
                else if (Convert.ToInt32(SROOfficeListID)==0)
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

                if (SroId < 0)
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
                        errorMessage = "From Date cannot be larger than To Date."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //  ADDED BY SHUBHAM BHAGAT ON 18-07-2019 
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

                #endregion

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);

                int pageSize = totalNum;
                int skip = startLen;

                ExemptionDocumentModel reqModel = new ExemptionDocumentModel();
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.SROfficeID = SroId;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;

                caller = new ServiceCaller("ExemptionDocumentAPIController");

                //To get total count of records in Surcharge Cess report datatable
                int totalCount = caller.PostCall<ExemptionDocumentModel, int>("ExemptionDocumentTotalCount", reqModel, out errorMessage);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.StartLen = 0;
                    reqModel.TotalNum = totalCount;
                }

                //To get records of Surcharge Cess report table 
                ExemptionDocumentDetailWrapper result = caller.PostCall<ExemptionDocumentModel, ExemptionDocumentDetailWrapper>("ExemptionDocumentDetail", reqModel, out errorMessage);
                if (result == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Exempted Document Details." });
                }
                if (result.ExemptionDocumentDetailList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Exempted Document Details." });
                }

                if (searchValue != null && searchValue != "")
                {
                    reqModel.StartLen = 0;
                    reqModel.TotalNum = totalCount;
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
                        result.ExemptionDocumentDetailList = result.ExemptionDocumentDetailList.Where(
                            m =>
                            m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                            m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                        m.STAMPDUTY_BEFORE_EXEMPTION.ToString().Contains(searchValue.ToLower()) ||
                        m.EXEMPTION_GIVEN.ToString().Contains(searchValue.ToLower()) ||
                        m.STAMPDUTY_AFTER_EXEMPTION.ToString().Contains(searchValue.ToLower()) ||
                        m.RegistrationFees.ToString().Contains(searchValue.ToLower()) ||
                        m.Total.ToString().Contains(searchValue.ToLower())).ToList();
                        totalCount = result.ExemptionDocumentDetailList.Count();
                    }
                }

                var gridData = result.ExemptionDocumentDetailList.Select(ExemptionDocumentDetail => new
                {
                    SerialNo = ExemptionDocumentDetail.SerialNo,
                    //JurisdictionalOffice = ExemptionDocumentDetail.SROName,
                    SROName = ExemptionDocumentDetail.SROName,
                    FinalRegistrationNumber = ExemptionDocumentDetail.FinalRegistrationNumber,
                    STAMPDUTY_BEFORE_EXEMPTION = ExemptionDocumentDetail.STAMPDUTY_BEFORE_EXEMPTION.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    EXEMPTION_GIVEN = ExemptionDocumentDetail.EXEMPTION_GIVEN.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    STAMPDUTY_AFTER_EXEMPTION = ExemptionDocumentDetail.STAMPDUTY_AFTER_EXEMPTION.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    RegistrationFees = ExemptionDocumentDetail.RegistrationFees.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    Total = ExemptionDocumentDetail.Total.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN"))
                });

                String PDFDownloadBtn = result.ExemptionDocumentDetailList.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.ExemptionDocumentDetailList.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeListID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Exempted Document Details." });
            }
        }

        ///// <summary>
        ///// Exemption Document Summary
        ///// </summary>
        ///// <param name="formCollection"></param>
        ///// <returns>Exemption Document Summary model</returns>
        //[HttpPost]
        //[EventAuditLogFilter(Description = "Exempted Document Summary")]
        //[ValidateAntiForgeryTokenOnAllPosts]
        //public ActionResult ExemptionDocumentSummary(FormCollection formCollection)
        //{
        //    try
        //    {
        //        #region User Variables and Objects
        //        string fromDate = formCollection["FromDate"];
        //        string ToDate = formCollection["ToDate"];
        //        string SROOfficeListID = formCollection["SROOfficeListID"];
        //        //string DROfficeID = formCollection["DROfficeID"];

        //        CommonFunctions objCommon = new CommonFunctions();
        //        String errorMessage = String.Empty;
        //        #endregion

        //        #region Server Side Validation       

        //        //if (String.IsNullOrEmpty(DROfficeID))
        //        //{
        //        //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any District.", URLToRedirect = "/Home/HomePage" });
        //        //}

        //        if (String.IsNullOrEmpty(SROOfficeListID))
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any Jurisdictional Office.", URLToRedirect = "/Home/HomePage" });
        //        }

        //        int SroId = Convert.ToInt32(SROOfficeListID);
        //        //int DroId = Convert.ToInt32(DROfficeID);

        //        //if (DroId == 0)
        //        //{
        //        //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any District.", URLToRedirect = "/Home/HomePage" });
        //        //}

        //        if (SroId < 0)
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select any Jurisdictional Office.", URLToRedirect = "/Home/HomePage" });
        //        }

        //        if (string.IsNullOrEmpty(fromDate))
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date required.", URLToRedirect = "/Home/HomePage" });
        //        }

        //        if (string.IsNullOrEmpty(ToDate))
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "To Date required.", URLToRedirect = "/Home/HomePage" });
        //        }
        //        #endregion

        //        DateTime frmDate, toDate;
        //        bool boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
        //        bool boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

        //        #region Validate date Inputs
        //        if (!boolFrmDate)
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid From Date.", URLToRedirect = "/Home/HomePage" });
        //        }
        //        if (!boolToDate)
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid To Date.", URLToRedirect = "/Home/HomePage" });
        //        }
        //        bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
        //        if (frmDate > toDate)
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date cannot be larger than To Date.", URLToRedirect = "/Home/HomePage" });
        //        }

        //        //  ADDED BY SHUBHAM BHAGAT ON 15-07-2019 
        //        //  3 YEARS VALIDATION BETWEEN FROM DATE AND TO DATE
        //        //if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(ToDate))
        //        //{
        //        //    if ((toDate - frmDate).TotalDays > 1095)
        //        //    {
        //        //        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of three years can be searched at a time.", URLToRedirect = "/Home/HomePage" });
        //        //    }
        //        //}

        //        #region Validation For Allowing Date range between only Current Financial year(Validation for From Date)
        //        DateTime CurrentDate = DateTime.Now;
        //        int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
        //        int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
        //        int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
        //        int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
        //        if (CMonth > 3)
        //        {
        //            if (FromDateyear < CYear)
        //            {                      
        //                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time.", URLToRedirect = "/Home/HomePage" });
        //            }
        //            else
        //            {
        //                if (FromDateyear > CYear)
        //                {
        //                    if (FromDateyear == CYear + 1)
        //                    {
        //                        if (FromDateMonth > 3)
        //                        {
        //                            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time.", URLToRedirect = "/Home/HomePage" });
        //                        }
        //                    }
        //                    else
        //                    {
        //                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time.", URLToRedirect = "/Home/HomePage" });
        //                    }
        //                }
        //                else if (FromDateyear == CYear)
        //                {
        //                    if (FromDateMonth < 4)
        //                    {
        //                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time.", URLToRedirect = "/Home/HomePage" });
        //                    }
        //                }
        //            }
        //        }
        //        else if (CMonth <= 3)
        //        {
        //            if (FromDateyear < CYear)
        //            {
        //                if (FromDateyear == CYear - 1)
        //                {
        //                    if (FromDateMonth < 4)
        //                    {
        //                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time.", URLToRedirect = "/Home/HomePage" });
        //                    }
        //                }
        //                else
        //                {
        //                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time.", URLToRedirect = "/Home/HomePage" });
        //                }
        //            }
        //        }
        //        #endregion
        //        #endregion

        //        ExemptionDocumentModel reqModel = new ExemptionDocumentModel();
        //        reqModel.SROfficeID = SroId;
        //        reqModel.DateTime_ToDate = toDate;
        //        reqModel.DateTime_FromDate = frmDate;

        //        caller = new ServiceCaller("ExemptionDocumentAPIController");

        //        ExemptionDocumentSummary exemptionDocumentSummary = caller.PostCall<ExemptionDocumentModel, ExemptionDocumentSummary>("ExemptionDocumentSummary", reqModel, out errorMessage);
        //        if (exemptionDocumentSummary == null)
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Exempted Document Summary details", URLToRedirect = "/Home/HomePage" });
        //        }

        //        return View(exemptionDocumentSummary);
        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
        //    }
        //}

        #region PDF    
        /// <summary>
        /// Exemption Document To PDF
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="MaxDate"></param>
        /// <returns></returns>
        //[EventAuditLogFilter(Description = "Exempted Document Report To PDF")]
        //public ActionResult ExemptionDocumentToPDF(string FromDate, string ToDate, string SROOfficeListID, string MaxDate)
        //{
        //    try
        //    {
        //        int SROINT = Convert.ToInt32(SROOfficeListID);
        //        CommonFunctions objCommon = new CommonFunctions();
        //        string errorMessage = string.Empty;
        //        DateTime frmDate, toDate;
        //        DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
        //        DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

        //        ExemptionDocumentModel model = new ExemptionDocumentModel
        //        {
        //            DateTime_FromDate = frmDate,
        //            DateTime_ToDate = toDate,
        //            SROfficeID = Convert.ToInt32(SROOfficeListID),
        //            StartLen = 0,
        //            TotalNum = 10,
        //        };

        //        caller = new ServiceCaller("ExemptionDocumentAPIController");

        //        //To get total count of records in Exemption Document Report datatable
        //        int totalCount = caller.PostCall<ExemptionDocumentModel, int>("ExemptionDocumentTotalCount", model);
        //        model.TotalNum = totalCount;

        //        // To get total records of Exemption Document report table
        //        ExemptionDocumentDetailWrapper objListItemsToBeExported = caller.PostCall<ExemptionDocumentModel, ExemptionDocumentDetailWrapper>("ExemptionDocumentDetail", model, out errorMessage);

        //        if (objListItemsToBeExported == null)
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
        //        }

        //        if (objListItemsToBeExported.ExemptionDocumentDetailList == null)
        //        {
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
        //        }

        //        // For Summary 
        //        //ExemptionDocumentSummary exemptionDocumentSummary = caller.PostCall<ExemptionDocumentModel, ExemptionDocumentSummary>("ExemptionDocumentSummary", model, out errorMessage);
        //        //if (exemptionDocumentSummary == null)
        //        //{
        //        //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading PDF", URLToRedirect = "/Home/HomePage" });
        //        //}

        //        string fileName = string.Format("ExemptedDocument.pdf");
        //        string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //        string pdfHeader = string.Format("Exempted Document Report Details (Between {0} and {1})", FromDate, ToDate);

        //        //To get SRONAME
        //        string SROName = String.Empty;

        //        if (SROINT == 0)
        //            SROName = "All";
        //        else
        //        {
        //            SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });
        //            if (SROName == null)
        //            {
        //                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
        //            }
        //        }

        //        //Create Temp PDF File
        //        byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported.ExemptionDocumentDetailList, fileName, pdfHeader, SROName, exemptionDocumentSummary, MaxDate);

        //        return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "ExemptedDocument_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");
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
        ///// <param name="SROName"></param>
        ///// <param name="exemptionDocumentSummary"></param>
        ///// <param name="MaxDate"></param>
        ///// <returns>returns PDF byte array</returns>
        //private byte[] CreatePDFFile(List<ExemptionDocumentDetail> objListItemsToBeExported, string fileName, string pdfHeader, string SROName, ExemptionDocumentSummary exemptionDocumentSummary, string MaxDate)
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
        //                    doc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
        //                    var headerTextFont = FontFactory.GetFont("Arial", 15, new BaseColor(0, 128, 255));
        //                    doc.Open();
        //                    Paragraph addHeading = new Paragraph(pdfHeader, headerTextFont)
        //                    {
        //                        Alignment = 1,
        //                    };
        //                    Paragraph addSpace = new Paragraph(" ")
        //                    {
        //                        Alignment = 1
        //                    };
        //                    var blackListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(0, 0, 0));
        //                    var redListTextFont = FontFactory.GetFont("Arial", 12, new BaseColor(94, 154, 214));

        //                    var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
        //                    var totalChunk = new Chunk("Total Records: ", blackListTextFont);
        //                    var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);
        //                    var SroName = new Chunk(SROName + "       ", redListTextFont);
        //                    var descriptionChunk = new Chunk(DateTime.Now.ToString() + "       ", redListTextFont);
        //                    string count = (objListItemsToBeExported.Count()).ToString();
        //                    var countChunk = new Chunk(count, redListTextFont);

        //                    var titlePhrase = new Phrase(titleChunk)
        //                {
        //                    descriptionChunk
        //                };
        //                    var totalPhrase = new Phrase(totalChunk)
        //                {
        //                    countChunk
        //                };
        //                    var SroNamePhrase = new Phrase(SroNameChunk)
        //                {
        //                    SroName
        //                };

        //                    var FontItalic = FontFactory.GetFont("Arial", 10, 2, new BaseColor(94, 94, 94));

        //                    Paragraph NotePara = new Paragraph("Note : This report is based on pre processed data considered upto : " + MaxDate, FontItalic);
        //                    NotePara.Alignment = Element.ALIGN_RIGHT;

        //                    // For Adding Jurisdiction Summary Table

        //                    string fontpath = System.Configuration.ConfigurationManager.AppSettings["FontPath"];
        //                    string ARIALUNI_TFF = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), "KNBUM3NT.ttf");
        //                    BaseFont customKannadafont = BaseFont.CreateFont(ARIALUNI_TFF, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        //                    iTextSharp.text.Font tableContentFont = new iTextSharp.text.Font(customKannadafont, 14);

        //                    string SerialNo = "Serial No";
        //                    string JurisdictionalSRO = "Jurisdictional Office";
        //                    string SRONameST = "SRO Name";
        //                    string Documents = "Documents";
        //                    string TotalStampBeforeExcemption = "Total Stamp Before Excemption";
        //                    string TotalExcemptionGiven = "Total Excemption Given";
        //                    string TotalStampAfterExcemption = "Total Stamp After Excemption";
        //                    string RegistrationFees = "Total Registration Fees";
        //                    string Total = "Total";

        //                    PdfPCell cell = null;

        //                    cell = new PdfPCell(new Phrase())
        //                    {
        //                        BackgroundColor = new BaseColor(226, 226, 226)
        //                    };

        //                    string[] colSummary = { SerialNo, JurisdictionalSRO, SRONameST, Documents, TotalStampBeforeExcemption,TotalExcemptionGiven,TotalStampAfterExcemption, RegistrationFees, Total };
        //                    PdfPTable tableSummary = new PdfPTable(9)
        //                    {
        //                        WidthPercentage = 90
        //                    };
        //                    tableSummary.HeaderRows = 1;


        //                    tableSummary.SetWidths(new Single[] { 8, 8, 8, 8, 8, 8, 8, 8, 8 });

        //                    for (int i = 0; i < colSummary.Length; ++i)
        //                    {
        //                        cell = new PdfPCell(new Phrase(colSummary[i]))
        //                        {
        //                            BackgroundColor = new BaseColor(204, 255, 255)
        //                        };
        //                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //                        tableSummary.AddCell(cell);
        //                    }

        //                    PdfPCell cell1 = new PdfPCell(new Phrase(exemptionDocumentSummary.SerialNo.ToString(), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell1.BackgroundColor = BaseColor.WHITE;

        //                    PdfPCell cell2 = new PdfPCell(new Phrase(exemptionDocumentSummary.JurisdictionalOffice, tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell2.BackgroundColor = BaseColor.WHITE;

        //                    PdfPCell cell3 = new PdfPCell(new Phrase(exemptionDocumentSummary.SROName, tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell3.HorizontalAlignment = Element.ALIGN_LEFT;
        //                    cell3.BackgroundColor = BaseColor.WHITE;

        //                    PdfPCell cell4 = new PdfPCell(new Phrase(exemptionDocumentSummary.Documents.ToString(), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;
        //                    cell4.BackgroundColor = BaseColor.WHITE;

        //                    PdfPCell cell5 = new PdfPCell(new Phrase(exemptionDocumentSummary.STAMPDUTY_BEFORE_EXEMPTION.ToString("F"), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                    cell5.BackgroundColor = BaseColor.WHITE;

        //                    PdfPCell cell6 = new PdfPCell(new Phrase(exemptionDocumentSummary.EXEMPTION_GIVEN.ToString("F"), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell6.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                    cell6.BackgroundColor = BaseColor.WHITE;

        //                    PdfPCell cell7 = new PdfPCell(new Phrase(exemptionDocumentSummary.STAMPDUTY_AFTER_EXEMPTION.ToString("F"), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell7.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                    cell7.BackgroundColor = BaseColor.WHITE;

        //                    PdfPCell cell8 = new PdfPCell(new Phrase(exemptionDocumentSummary.RegistrationFees.ToString("F"), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell8.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                    cell8.BackgroundColor = BaseColor.WHITE;

        //                    PdfPCell cell9= new PdfPCell(new Phrase(exemptionDocumentSummary.Total.ToString("F"), tableContentFont))
        //                    {
        //                        BackgroundColor = new BaseColor(204, 255, 255)
        //                    };
        //                    cell9.HorizontalAlignment = Element.ALIGN_RIGHT;
        //                    cell9.BackgroundColor = BaseColor.WHITE;


        //                    tableSummary.AddCell(cell1);
        //                    tableSummary.AddCell(cell2);
        //                    tableSummary.AddCell(cell3);
        //                    tableSummary.AddCell(cell4);
        //                    tableSummary.AddCell(cell5);
        //                    tableSummary.AddCell(cell6);
        //                    tableSummary.AddCell(cell7);
        //                    tableSummary.AddCell(cell8);
        //                    tableSummary.AddCell(cell9);

        //                    doc.Add(addHeading);
        //                    doc.Add(addSpace);
        //                    doc.Add(titlePhrase);
        //                    doc.Add(SroNamePhrase);
        //                    doc.Add(totalPhrase);
        //                    doc.Add(addSpace);
        //                    // Added by Shubham bhagat to add space between Note and summary table on 06-08-2019 At 12:38 PM
        //                    doc.Add(NotePara);
        //                    doc.Add(addSpace);
        //                    // Added Summary table
        //                    doc.Add(tableSummary);
        //                    // Added Details table
        //                    doc.Add(ExemptionDocumentTable(objListItemsToBeExported));
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
        ///// Exemption Document Table
        ///// </summary>
        ///// <param name="objListItemsToBeExported"></param>
        ///// <returns>returns PDF table</returns>
        //private PdfPTable ExemptionDocumentTable(List<ExemptionDocumentDetail> objListItemsToBeExported)
        //{
        //    string serialNumber = "Serial No";
        //    string JurisdictionalOffice = "Jurisdictional Office";
        //    string SROName = "SRO Name";
        //    string FinalRegistrationNumber = "Final Registration Number";
        //    string StampBeforeExcemption = "Stamp Before Excemption";
        //    string ExcemptionGiven = "Excemption Given";
        //    string StampAfterExcemption = "Stamp After Excemption";
        //    string RegistrationFees = "Registration Fees";
        //    string Total = "Total";
        //    try
        //    {
        //        PdfPCell cell1 = null;
        //        PdfPCell cell2 = null;
        //        PdfPCell cell3 = null;
        //        PdfPCell cell4 = null;
        //        PdfPCell cell5 = null;
        //        PdfPCell cell6 = null;
        //        PdfPCell cell7 = null;
        //        PdfPCell cell8 = null;
        //        PdfPCell cell9 = null;

        //        string[] col = { serialNumber, JurisdictionalOffice, SROName, FinalRegistrationNumber, StampBeforeExcemption,ExcemptionGiven,StampAfterExcemption, RegistrationFees, Total };
        //        PdfPTable table = new PdfPTable(9)
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
        //        table.SetWidths(new Single[] { 4, 4, 4, 4, 4, 4, 4, 4, 4 });
        //        /*
        //        * by default tables 'collapse' on surrounding elements,
        //        * so you need to explicitly add spacing
        //        */
        //        table.SpacingBefore = 15f;
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
        //            cell1.BackgroundColor = BaseColor.WHITE;
        //            cell1.HorizontalAlignment = Element.ALIGN_CENTER;

        //            cell2 = new PdfPCell(new Phrase(items.SROName, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell2.BackgroundColor = BaseColor.WHITE;
        //            cell2.HorizontalAlignment = Element.ALIGN_LEFT;

        //            cell3 = new PdfPCell(new Phrase(items.SROName, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell3.BackgroundColor = BaseColor.WHITE;
        //            cell3.HorizontalAlignment = Element.ALIGN_LEFT;

        //            cell4 = new PdfPCell(new Phrase(items.FinalRegistrationNumber, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell4.BackgroundColor = BaseColor.WHITE;
        //            cell4.HorizontalAlignment = Element.ALIGN_CENTER;

        //            cell5 = new PdfPCell(new Phrase(items.STAMPDUTY_AFTER_EXEMPTION.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell5.BackgroundColor = BaseColor.WHITE;
        //            cell5.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            cell6 = new PdfPCell(new Phrase(items.EXEMPTION_GIVEN.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell6.BackgroundColor = BaseColor.WHITE;
        //            cell6.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            cell7 = new PdfPCell(new Phrase(items.STAMPDUTY_AFTER_EXEMPTION.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell7.BackgroundColor = BaseColor.WHITE;
        //            cell7.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            cell8 = new PdfPCell(new Phrase(items.RegistrationFees.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell8.BackgroundColor = BaseColor.WHITE;
        //            cell8.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            cell9 = new PdfPCell(new Phrase(items.Total.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell9.BackgroundColor = BaseColor.WHITE;
        //            cell9.HorizontalAlignment = Element.ALIGN_RIGHT;

        //            table.AddCell(cell1);
        //            table.AddCell(cell2);
        //            table.AddCell(cell3);
        //            table.AddCell(cell4);
        //            table.AddCell(cell5);
        //            table.AddCell(cell6);
        //            table.AddCell(cell7);
        //            table.AddCell(cell8);
        //            table.AddCell(cell9);

        //            //cell1 = new PdfPCell(new Phrase(items.SerialNo.ToString("F"), tableContentFont))
        //            //{
        //            //    BackgroundColor = new BaseColor(204, 255, 255)
        //            //};
        //            //cell1.BackgroundColor = BaseColor.WHITE;

        //            //cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            //cell2 = new PdfPCell(new Phrase(items.consideration.ToString("F"), tableContentFont))
        //            //{
        //            //    BackgroundColor = new BaseColor(204, 255, 255)
        //            //};
        //            //cell2.BackgroundColor = BaseColor.WHITE;

        //            //cell2.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            //table.AddCell(cell1);
        //            //table.AddCell(cell2);

        //        }
        //        return table;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

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
        /// Export Exemption Document To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROOfficeListID"></param>
        /// <param name="MaxDate"></param>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Export Exempted Document Report To Excel")]
        public ActionResult ExportExemptionDocumentToExcel(string FromDate, string ToDate, string SROOfficeListID, string MaxDate)
        {
            try
            {
                int SROINT = Convert.ToInt32(SROOfficeListID);
                caller = new ServiceCaller("ExemptionDocumentAPIController");
                string fileName = string.Format("ExemptedDocument.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                ExemptionDocumentModel model = new ExemptionDocumentModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    StartLen = 0,
                    TotalNum = 10,
                };

                string SROName = String.Empty;
                if (SROINT == 0)
                    SROName = "All";
                else
                {
                    SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                    if (SROName == null)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                    }
                }

                caller = new ServiceCaller("ExemptionDocumentAPIController");
                int totalCount = caller.PostCall<ExemptionDocumentModel, int>("ExemptionDocumentTotalCount", model);
                model.TotalNum = totalCount;

                // // For Detail list 
                ExemptionDocumentDetailWrapper objListItemsToBeExported = caller.PostCall<ExemptionDocumentModel, ExemptionDocumentDetailWrapper>("ExemptionDocumentDetail", model, out errorMessage);
                if (objListItemsToBeExported == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                }
                if (objListItemsToBeExported.ExemptionDocumentDetailList == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                }

                //Commented by RamanK on 31-12-2019
                // For Summary 
                //ExemptionDocumentSummary exemptionDocumentSummary = caller.PostCall<ExemptionDocumentModel, ExemptionDocumentSummary>("ExemptionDocumentSummary", model, out errorMessage);
                //if (exemptionDocumentSummary == null)
                //{
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
                //}

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string excelHeader = string.Format("Exempted Document Report Between ({0} and {1})", FromDate, ToDate);
                //string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName, exemptionDocumentSummary, MaxDate);

                string createdExcelPath = CreateExcel(objListItemsToBeExported, fileName, excelHeader, SROName,  MaxDate);

                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();
                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "ExemptedDocument_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="exemptionDocumentSummary"></param>
        /// <param name="MaxDate"></param>
        /// <returns></returns>
        private string CreateExcel(ExemptionDocumentDetailWrapper objListItemsToBeExported, string fileName, string excelHeader, string SROName, string MaxDate)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Exempted Document Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[4, 1].Value = "Total Records : " + (objListItemsToBeExported.ExemptionDocumentDetailList.Count()) + "                                                                                                                                                                                             Note : This report is based on pre processed data considered upto :" + MaxDate;
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
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
                    workSheet.Column(8).Width = 30;
                    workSheet.Column(9).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(6).Style.Font.Bold = true;
                    //workSheet.Row(9).Style.Font.Bold = true;

                    workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    //workSheet.Cells[6, 1].Value = "Serial No";
                    //workSheet.Cells[6, 2].Value = "Jurisdictional Office";
                    //workSheet.Cells[6, 3].Value = "SRO Name";
                    //workSheet.Cells[6, 4].Value = "Documents";
                    //workSheet.Cells[6, 5].Value = "Total Stamp Before Excemption";
                    //workSheet.Cells[6, 6].Value = "Total Excemption Given";
                    //workSheet.Cells[6, 7].Value = "Total Stamp After Excemption";
                    //workSheet.Cells[6, 8].Value = "Total Registration Fees";
                    //workSheet.Cells[6, 9].Value = "Total";


                    //workSheet.Cells[7, 1].Value = exemptionDocumentSummary.SerialNo;
                    //workSheet.Cells[7, 2].Value = exemptionDocumentSummary.JurisdictionalOffice;
                    //workSheet.Cells[7, 3].Value = exemptionDocumentSummary.SROName;
                    //workSheet.Cells[7, 4].Value = exemptionDocumentSummary.Documents;
                    //workSheet.Cells[7, 5].Value = exemptionDocumentSummary.STAMPDUTY_BEFORE_EXEMPTION;
                    //workSheet.Cells[7, 6].Value = exemptionDocumentSummary.EXEMPTION_GIVEN;
                    //workSheet.Cells[7, 7].Value = exemptionDocumentSummary.STAMPDUTY_AFTER_EXEMPTION;
                    //workSheet.Cells[7, 8].Value = exemptionDocumentSummary.RegistrationFees;
                    //workSheet.Cells[7, 9].Value = exemptionDocumentSummary.Total;

                    //workSheet.Cells[7, 5].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[7, 6].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[7, 7].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[7, 8].Style.Numberformat.Format = "0.00";
                    //workSheet.Cells[7, 9].Style.Numberformat.Format = "0.00";

                    //workSheet.Cells[7, 1].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Cells[7, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    //workSheet.Cells[7, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //workSheet.Cells[7, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //workSheet.Cells[7, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //workSheet.Cells[7, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                    //workSheet.Cells[7, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                    int rowIndex = 7;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[6, 1].Value = "Serial No";
                    workSheet.Cells[6, 2].Value = "Jurisdictional Office";
                    workSheet.Cells[6, 3].Value = "SRO Name";
                    workSheet.Cells[6, 4].Value = "Final Registration Number";
                    workSheet.Cells[6, 5].Value = "Stamp Before Excemption";
                    workSheet.Cells[6, 6].Value = "Excemption Given";
                    workSheet.Cells[6, 7].Value = "Stamp After Excemption";
                    workSheet.Cells[6, 8].Value = "Registration Fees";
                    workSheet.Cells[6, 9].Value = "Total";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";

                    foreach (var items in objListItemsToBeExported.ExemptionDocumentDetailList)
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

                        workSheet.Cells[rowIndex, 5].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 6].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 7].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SROName;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.FinalRegistrationNumber;
                        workSheet.Cells[rowIndex, 5].Value = items.STAMPDUTY_BEFORE_EXEMPTION;
                        workSheet.Cells[rowIndex, 6].Value = items.EXEMPTION_GIVEN;
                        workSheet.Cells[rowIndex, 7].Value = items.STAMPDUTY_AFTER_EXEMPTION;
                        workSheet.Cells[rowIndex, 8].Value = items.RegistrationFees;
                        workSheet.Cells[rowIndex, 9].Value = items.Total;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;

                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    // commented by shubham bhagat on 05-07-2019
                    // to stop making bold last line in excel 
                    //workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

                    using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 9])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    //// For Summary
                    //using (ExcelRange Rng = workSheet.Cells[6, 1, 7, 9])
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