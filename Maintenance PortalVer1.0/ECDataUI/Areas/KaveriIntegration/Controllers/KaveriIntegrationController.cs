/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: KaveriIntegrationController.cs
 * Author : Shubham Bhagat
 * Creation Date :14 Oct 2019
 * Desc : Provides methods for view and model interaction
 * ECR No : 
*/

using CustomModels.Models.KaveriIntegration;
using CustomModels.Models.MISReports.AnywhereECLog;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.KaveriIntegration.Controllers
{
    [KaveriAuthorizationAttribute]
    public class KaveriIntegrationController : Controller
    {
        ServiceCaller caller = null;

        /// <summary>
        /// Kaveri Integration View
        /// </summary>
        /// <returns>returns view containing Kaveri Integration Model.</returns>
        public ActionResult KaveriIntegrationView()
        {
            try
            {
                KaveriIntegrationModel model = new KaveriIntegrationModel();

                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.KaveriIntegrationReport;
                int OfficeID = KaveriSession.Current.OfficeID;

                caller = new ServiceCaller("KaveriIntegrationAPIController");

                model = caller.GetCall<KaveriIntegrationModel>("KaveriIntegrationView", new { OfficeID = OfficeID });

                DateTime now = DateTime.Now;
                var startDate = new DateTime(now.Year, now.Month, 1);
                model.FromDate = startDate.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                model.ToDate = DateTime.Now.ToString("dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture);
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Kaveri Integration View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Load Kaveri Integration Table
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns json data containing Kaveri Integration report numbers.</returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult LoadKaveriIntegrationTable(FormCollection formCollection)
        {
            caller = new ServiceCaller("KaveriIntegrationAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects       
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string DistrictID = formCollection["DistrictID"];
                string SroID = formCollection["SroID"];

                int DroID = 0;
                int SroIDINT = 0;
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;

                #region Server Side Validation
                if (string.IsNullOrEmpty(DistrictID))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Any District."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                Regex regx1 = new Regex("^[0-9]*$");
                Match mtchDistrict = regx1.Match(DistrictID);
                if (!mtchDistrict.Success)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Any District."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (string.IsNullOrEmpty(SroID))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Any SRO."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                Match mtchSRO = regx1.Match(SroID);
                if (!mtchSRO.Success)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select Any SRO."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                DroID = Convert.ToInt32(DistrictID);
                SroIDINT = Convert.ToInt32(SroID); 

                //if (DroID < 0)
                //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select Any District.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

                if (string.IsNullOrEmpty(FromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date required."
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
                        status = false,
                        errorMessage = "To Date required."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);

                if (!boolFrmDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Invalid From Date."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (!boolToDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Invalid To Date."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                else if (frmDate > toDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date can not be larger than To Date."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                // 180 days validation or 6 months vaidation
                else if ((toDate - frmDate).TotalDays > 365)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Data of one year can be seen at a time"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                KaveriIntegrationModel reqModel = new KaveriIntegrationModel();
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DistrictID = DroID;
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;
                reqModel.SROfficeID= SroIDINT;


                // total Count is fetched in same call 
                int totalCount = 0;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.IsForSearch = true;
                }

                KaveriIntegrationWrapperModel responseModel = caller.PostCall<KaveriIntegrationModel, KaveriIntegrationWrapperModel>("LoadKaveriIntegrationTable", reqModel, out errorMessage);
                totalCount = responseModel.TotalCount;
                IEnumerable<KaveriIntegrationDetail> result = responseModel.KaveriIntegrationDetailList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Kaveri Integration details ( LoadKaveriIntegrationTable )." });
                    //return Json(new { serverError = true, errorMessage = "Error occured while getting Kaveri Integration details." }, JsonRequestBehavior.AllowGet);

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
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }
                    else
                    {
                        result = result.Where(m =>
                        m.SROName.ToLower().Contains(searchValue.ToLower()) ||
                        m.TotalPropertiesRegistered.Contains(searchValue) ||
                        m.Bhoomi.Contains(searchValue) ||
                        m.E_Swathu.Contains(searchValue) ||
                        m.UPOR.Contains(searchValue) ||
                        //Added by shubham bhagat on 7-11-2019 for Mojani 
                        m.Mojani.Contains(searchValue) ||
                        m.EAasthi.Contains(searchValue) ||
                        m.Total_Properties_Registered_Without_Importing.Contains(searchValue) ||
                        m.DistrictName.ToLower().Contains(searchValue.ToLower()));

                        totalCount = result.Count();
                    }
                }

                //var gridData = new List<String>();
                var gridData = result.Select(KaveriIntegrationDetail => new
                {
                    SerialNo = KaveriIntegrationDetail.SerialNo,
                    DistrictName = KaveriIntegrationDetail.DistrictName,
                    SROName = KaveriIntegrationDetail.SROName,
                    TotalPropertiesRegistered = KaveriIntegrationDetail.TotalPropertiesRegistered,
                    Bhoomi = KaveriIntegrationDetail.Bhoomi,
                    E_Swathu = KaveriIntegrationDetail.E_Swathu,
                    UPOR = KaveriIntegrationDetail.UPOR,
                    //Added by shubham bhagat on 7-11-2019 for Mojani 
                    Mojani = KaveriIntegrationDetail.Mojani,
                    EAasthi = KaveriIntegrationDetail.EAasthi,
                    Total_Without_Importing = KaveriIntegrationDetail.Total_Properties_Registered_Without_Importing,
                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + FromDate + "','" + ToDate + "','" + DistrictID + "','" + SroID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = totalCount,
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
                        //status = "1",
                        recordsFiltered = totalCount,
                        ExcelDownloadBtn = ExcelDownloadBtn
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = formCollection["draw"],
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while getting Kaveri Integration details."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }

        //[HttpPost]
        //public ActionResult LoadKaveriIntegrationTable(FormCollection formCollection)
        //{
        //    caller = new ServiceCaller("KaveriIntegrationAPIController");
        //    TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
        //    caller.HttpClient.Timeout = objTimeSpan;
        //    try
        //    {
        //        #region User Variables and Objects       
        //        string FromDate = formCollection["FromDate"];
        //        string ToDate = formCollection["ToDate"];
        //        string DistrictID = formCollection["DistrictID"];
        //        int DroID = 0;
        //        DateTime frmDate, toDate;
        //        bool boolFrmDate = false;
        //        bool boolToDate = false;
        //        CommonFunctions objCommon = new CommonFunctions();
        //        String errorMessage = String.Empty;
        //        #endregion

        //        #region Server Side Validation
        //        if (string.IsNullOrEmpty(DistrictID))
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select Any District.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

        //        Regex regx = new Regex("^[0-9]*$");
        //        Match mtchDistrict = regx.Match(DistrictID);
        //        if (!mtchDistrict.Success)
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select Any District.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

        //        DroID = Convert.ToInt32(DistrictID);

        //        //if (DroID < 0)
        //        //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Please select Any District.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

        //        if (string.IsNullOrEmpty(FromDate))
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date required.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

        //        else if (string.IsNullOrEmpty(ToDate))
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "To Date required.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

        //        boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
        //        boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

        //        bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);

        //        if (!boolFrmDate)
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid From Date.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

        //        else if (!boolToDate)
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Invalid To Date.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

        //        else if (frmDate > toDate)
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date can not be larger than To Date.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });
        //        // 180 days validation or 6 months vaidation
        //        //else if ((toDate - frmDate).TotalDays > 180)
        //        //    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "You can only see records of six months.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });

        //        #endregion

        //        KaveriIntegrationModel reqModel = new KaveriIntegrationModel();
        //        reqModel.DateTime_FromDate = frmDate;
        //        reqModel.DateTime_ToDate = toDate;
        //        reqModel.DistrictID = DroID;

        //        KaveriIntegrationWrapperModel responseModel = caller.PostCall<KaveriIntegrationModel, KaveriIntegrationWrapperModel>("LoadKaveriIntegrationTable", reqModel, out errorMessage);
        //        if (responseModel != null)
        //            return View("KaveriIntegrationTableView", responseModel);
        //        else
        //            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting Kaveri Integration details.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });
        //    }
        //    catch (Exception e)
        //    {
        //        ExceptionLogs.LogException(e);
        //        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while getting Kaveri Integration details.", URLToRedirect = "/KaveriIntegration/KaveriIntegration/KaveriIntegrationView" });
        //    }
        //}

        //public ActionResult OtherTableDetails(String columnName)
        //{
        //    return View();
        //}

        /// <summary>
        /// Other Table Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns>returns json data containing Kaveri Integration report details.</returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult OtherTableDetails(FormCollection formCollection)
        {
            caller = new ServiceCaller("KaveriIntegrationAPIController");
            TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
            caller.HttpClient.Timeout = objTimeSpan;
            try
            {
                #region User Variables and Objects    
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                string FromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string DistrictID = formCollection["DistrictID"];
                string columnName = formCollection["columnName"];
                int DistrictId = Convert.ToInt32(DistrictID);
                string SROCode = formCollection["SROCode"];

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion                

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;
                short OfficeID = KaveriSession.Current.OfficeID;

                //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID });

                //Validation For DR Login
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
                //    if ((SroId == 0 && DistrictId == 0))//when user do not select any DR and SR which are by default "Select"
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
                //    else if (SroId == 0 && DistrictId != 0)//when User selects DR but not SR which is by default "Select"
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

                if (string.IsNullOrEmpty(FromDate))
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

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);

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
                else if (!boolToDate)
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
                else if (frmDate > toDate)
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

                // Request Model
                KaveriIntegrationModel reqModel = new KaveriIntegrationModel();
                reqModel.StartLen = startLen;
                reqModel.TotalNum = totalNum;
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DistrictID = DistrictId;
                reqModel.ColumnName = columnName;
                reqModel.SROCode = Convert.ToInt32(SROCode);

                //int totalCount = caller.PostCall<KaveriIntegrationModel, int>("GetAnywhereECLogTotalCount", reqModel, out errorMessage);

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}

                // total Count is fetched in same call 
                int totalCount = 0;

                if (searchValue != null && searchValue != "")
                {
                    reqModel.IsForSearch = true;
                }

                KIDetailsWrapperModel ResModel = caller.PostCall<KaveriIntegrationModel, KIDetailsWrapperModel>("OtherTableDetails", reqModel, out errorMessage);
                totalCount = ResModel.TotalCount;
                IEnumerable<KIDetailsModel> result = ResModel.KIDetailsModelList;
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Kaveri Integration details." });
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
                                errorMessage = "Please enter valid Search String."
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;
                        }
                    }
                    else
                    {
                        result = result.Where(m =>
                        m.DocumentNumber.ToString().Contains(searchValue.ToLower()) ||
                        m.PropertyDetails.ToLower().Contains(searchValue.ToLower()) ||
                        m.Executant.ToLower().Contains(searchValue.ToLower()) ||
                        m.Claimant.ToLower().Contains(searchValue.ToLower()) ||
                        m.Reference_AcknowledgementNumber.ToString().Contains(searchValue.ToLower()) ||
                        m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.IntegrationDepartmentName.ToLower().Contains(searchValue.ToLower()) ||
                        m.UploadDate.ToLower().Contains(searchValue.ToLower()) ||
                        m.NatureOfDocument.ToLower().Contains(searchValue.ToLower())
                        );

                        totalCount = result.Count();
                    }
                }

                var gridData = result.Select(KIDetailsModel => new
                {
                    SerialNo = KIDetailsModel.SerialNo,
                    DocumentID = KIDetailsModel.DocumentNumber,
                    PropertyDetails = KIDetailsModel.PropertyDetails,
                    Executant = KIDetailsModel.Executant,
                    Claimant = KIDetailsModel.Claimant,
                    Reference_AcknowledgementNumber = KIDetailsModel.Reference_AcknowledgementNumber,
                    FinalRegistrationNumber = KIDetailsModel.FinalRegistrationNumber.ToString(),
                    IntegrationDepartmentName = KIDetailsModel.IntegrationDepartmentName.ToString(),
                    UploadDate = KIDetailsModel.UploadDate.ToString(),
                    NatureOfDocument = KIDetailsModel.NatureOfDocument.ToString(),
                    VillageName = KIDetailsModel.VillageName.ToString()
                });

                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=ExportToExcelDetails('" + FromDate + "','" + ToDate + "','" + SROCode + "','" + columnName + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = totalCount,
                        SROName = ResModel.SROName,
                        //PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        columnName = columnName
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
                        //status = "1",
                        recordsFiltered = totalCount,
                        SROName = ResModel.SROName,
                        // PDFDownloadBtn = PDFDownloadBtn,
                        ExcelDownloadBtn = ExcelDownloadBtn,
                        columnName = columnName
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }
            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Kaveri Integration details." }, JsonRequestBehavior.AllowGet);
            }
        }


        #region Excel
        /// <summary>
        /// Export To Excel
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="DistrictID"></param>
        /// <returns> returns Excel of Kaveri Integration report numbers.</returns>
        [EventAuditLogFilter(Description = "Kaveri Integration Summary Export To Excel")]
        public ActionResult ExportToExcel(string FromDate, string ToDate, string DistrictID,String SroID)
        {
            try
            {
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                caller = new ServiceCaller("KaveriIntegrationAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("KaveriIntegration.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                KaveriIntegrationModel reqModel = new KaveriIntegrationModel();
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DistrictID = Convert.ToInt32(DistrictID);
                reqModel.IsForExcelDownload = true;
                reqModel.SROfficeID = Convert.ToInt32(SroID);                
                List<KaveriIntegrationDetail> objListItemsToBeExported = new List<KaveriIntegrationDetail>();

                caller = new ServiceCaller("KaveriIntegrationAPIController");
                caller.HttpClient.Timeout = objTimeSpan;
                KaveriIntegrationWrapperModel kaveriIntegrationWrapperModel = new KaveriIntegrationWrapperModel();
                kaveriIntegrationWrapperModel = caller.PostCall<KaveriIntegrationModel, KaveriIntegrationWrapperModel>("LoadKaveriIntegrationTable", reqModel, out errorMessage);
                objListItemsToBeExported = kaveriIntegrationWrapperModel.KaveriIntegrationDetailList;

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string excelHeader = string.Format("Kaveri Integration Summary Report Between (" + FromDate + " and " + ToDate + ")");

                string createdExcelPath = string.Empty;

                createdExcelPath = CreateExcel(kaveriIntegrationWrapperModel, fileName, excelHeader);

                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();
                //}

                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);
                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, "Kaveri_Integration_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading Excel", URLToRedirect = "/Home/HomePage" });
            }
        }

        ///// <summary>
        ///// Create Excel
        ///// </summary>
        ///// <param name="kaveriIntegrationWrapperModel"></param>
        ///// <param name="fileName"></param>
        ///// <param name="excelHeader"></param>
        ///// <returns>returns excel file path</returns>     
        private string CreateExcel(KaveriIntegrationWrapperModel kaveriIntegrationWrapperModel, string fileName, string excelHeader)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Kaveri Integration Summary Report");
                    workSheet.Cells.Style.Font.Size = 15;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "Total Records : " + kaveriIntegrationWrapperModel.KaveriIntegrationDetailList.Count();
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;

                    //workSheet.Column(6).Style.WrapText = true;
                    // workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 25;
                    workSheet.Column(2).Width = 25;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 40;
                    workSheet.Column(5).Width = 20;
                    workSheet.Column(6).Width = 20;
                    workSheet.Column(7).Width = 20;
                    workSheet.Column(8).Width = 20;
                    workSheet.Column(9).Width = 20;
                    workSheet.Column(10).Width = 70;


                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;

                    int rowIndex = 6;

                    workSheet.Cells[5, 1].Value = "Serial Number";
                    workSheet.Cells[5, 2].Value = "District Office";
                    workSheet.Cells[5, 3].Value = "Sub-Registrar Office (A)";
                    workSheet.Cells[5, 4].Value = "Total Properties Registered (B)";
                    workSheet.Cells[5, 5].Value = "Bhoomi (C)";
                    workSheet.Cells[5, 6].Value = "E-Swathu (D)";
                    workSheet.Cells[5, 7].Value = "UPOR (E)";
                    workSheet.Cells[5, 8].Value = "Mojini (F)";
                    workSheet.Cells[5, 9].Value = "E-Asthi (G)";
                    workSheet.Cells[5, 10].Value = "Total properties registered without importing (H=B-(C+D+E+G))";

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    foreach (var items in kaveriIntegrationWrapperModel.KaveriIntegrationDetailList)
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


                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.DistrictName;
                        workSheet.Cells[rowIndex, 3].Value = items.SROName;
                        workSheet.Cells[rowIndex, 4].Value = items.TotalPropertiesRegistered;
                        workSheet.Cells[rowIndex, 5].Value = items.Bhoomi;
                        workSheet.Cells[rowIndex, 6].Value = items.E_Swathu;
                        workSheet.Cells[rowIndex, 7].Value = items.UPOR;
                        workSheet.Cells[rowIndex, 8].Value = items.Mojani;
                        workSheet.Cells[rowIndex, 9].Value = items.EAasthi;
                        workSheet.Cells[rowIndex, 10].Value = items.Total_Properties_Registered_Without_Importing;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                    }

                    using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 10])
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

        /// <summary>
        /// Export To Excel Details
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="SROCode"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        [EventAuditLogFilter(Description = "Kaveri Integration Summary Export To Excel")]
        public ActionResult ExportToExcelDetails(string FromDate, string ToDate, string SROCode, String columnName)
        {
            try
            {
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                caller = new ServiceCaller("KaveriIntegrationAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                string fileName = string.Format("KaveriIntegrationDetails.xlsx");
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;

                KaveriIntegrationModel reqModel = new KaveriIntegrationModel();
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DateTime_ToDate = toDate;
                reqModel.SROCode = Convert.ToInt32(SROCode);
                reqModel.IsForExcelDownload = true;
                reqModel.ColumnName = columnName;

                List<KIDetailsModel> objListItemsToBeExported = new List<KIDetailsModel>();

                caller = new ServiceCaller("KaveriIntegrationAPIController");
                caller.HttpClient.Timeout = objTimeSpan;

                KIDetailsWrapperModel kIDetailsWrapperModel = new KIDetailsWrapperModel();
                kIDetailsWrapperModel = caller.PostCall<KaveriIntegrationModel, KIDetailsWrapperModel>("OtherTableDetails", reqModel, out errorMessage);
                objListItemsToBeExported = kIDetailsWrapperModel.KIDetailsModelList;

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing.." }, JsonRequestBehavior.AllowGet);
                }

                //string clientDownloadableExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                //if (string.IsNullOrEmpty(clientDownloadableExcelFilePath))
                //{
                //    throw new Exception();
                //}
                string excelHeader = string.Format("Kaveri Integration Details Report Between (" + FromDate + " and " + ToDate + ")");
                string createdExcelPath = string.Empty;

                createdExcelPath = CreateExcelDetails(kIDetailsWrapperModel, fileName, excelHeader, columnName);

                //if (string.IsNullOrEmpty(createdExcelPath))
                //{
                //    throw new Exception();
                //}
                byte[] pdfBinary = System.IO.File.ReadAllBytes(createdExcelPath);
                objCommon.DeleteFileFromTemporaryFolder(createdExcelPath);

                String FileName = String.Empty;
                switch (columnName)
                {
                    case "B":
                        FileName = "Total_Properties_Registered_";
                        break;

                    case "C":
                        FileName = "Bhoomi_";
                        break;

                    case "D":
                        FileName = "E_Swathu_";
                        break;

                    case "E":
                        FileName = "UPOR_";
                        break;

                    case "F":
                        FileName = "Mojini_";
                        break;

                    case "G":
                        FileName = "Total_properties_registered_without_importing_";
                        break;
                }

                return File(pdfBinary, System.Net.Mime.MediaTypeNames.Application.Octet, FileName + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx");
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
        /// <param name="kaveriIntegrationWrapperModel"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <returns>returns excel file path</returns>
        private string CreateExcelDetails(KIDetailsWrapperModel kIDetailsWrapperModel, string fileName, string excelHeader, string columnName)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Kaveri Integration Details Report");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
                    workSheet.Cells[3, 1].Value = "Total Records : " + kIDetailsWrapperModel.KIDetailsModelList.Count();

                    if (columnName == "B" || columnName == "G")
                    {
                        workSheet.Cells[1, 1, 1, 9].Merge = true;
                        workSheet.Cells[2, 1, 2, 9].Merge = true;
                        workSheet.Cells[3, 1, 3, 9].Merge = true;
                    }
                    else
                    {
                        workSheet.Cells[1, 1, 1, 11].Merge = true;
                        workSheet.Cells[2, 1, 2, 11].Merge = true;
                        workSheet.Cells[3, 1, 3, 11].Merge = true;
                    }

                    workSheet.Column(3).Style.WrapText = true;
                    workSheet.Column(4).Style.WrapText = true;
                    workSheet.Column(5).Style.WrapText = true;
                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Column(8).Style.WrapText = true;
                    workSheet.Column(9).Style.WrapText = true;
                    workSheet.Column(10).Style.WrapText = true;
                    workSheet.Column(11).Style.WrapText = true;

                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    if (columnName == "B" || columnName == "G")
                    {
                        workSheet.Column(1).Width = 25;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 50;
                        workSheet.Column(4).Width = 50;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 30;
                        workSheet.Column(7).Width = 50;
                        workSheet.Column(8).Width = 50;
                        workSheet.Column(9).Width = 50;

                    }
                    else
                    {
                        workSheet.Column(1).Width = 25;
                        workSheet.Column(2).Width = 30;
                        workSheet.Column(3).Width = 50;
                        workSheet.Column(4).Width = 50;
                        workSheet.Column(5).Width = 50;
                        workSheet.Column(6).Width = 50;
                        workSheet.Column(7).Width = 30;
                        workSheet.Column(8).Width = 50;
                        workSheet.Column(9).Width = 50;
                        workSheet.Column(10).Width = 50;
                        workSheet.Column(11).Width = 50;
                    }

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;

                    int rowIndex = 6;

                    if (columnName == "B" || columnName == "G")
                    {
                        workSheet.Cells[5, 1].Value = "Serial Number";
                        workSheet.Cells[5, 2].Value = "Document Number";
                        workSheet.Cells[5, 3].Value = "Final Registration Number";
                        workSheet.Cells[5, 4].Value = "Nature of Document";
                        workSheet.Cells[5, 5].Value = "Village Name";
                        workSheet.Cells[5, 6].Value = "Property Details";
                        workSheet.Cells[5, 7].Value = "Executant";
                        workSheet.Cells[5, 8].Value = "Claimant";
                        workSheet.Cells[5, 9].Value = "Integration Department Name";
                    }
                    else
                    {
                        workSheet.Cells[5, 1].Value = "Serial Number";
                        workSheet.Cells[5, 2].Value = "Document Number";
                        workSheet.Cells[5, 3].Value = "Final Registration Number";
                        workSheet.Cells[5, 4].Value = "Nature of Document";
                        workSheet.Cells[5, 5].Value = "Village Name";
                        workSheet.Cells[5, 6].Value = "Property Details";
                        workSheet.Cells[5, 7].Value = "Executant";
                        workSheet.Cells[5, 8].Value = "Claimant";
                        workSheet.Cells[5, 9].Value = "Reference / Acknowledgement Number";
                        workSheet.Cells[5, 10].Value = "Integration Department Name";
                        workSheet.Cells[5, 11].Value = "Upload Date";
                    }

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    foreach (var items in kIDetailsWrapperModel.KIDetailsModelList)
                    {
                        if (columnName == "B" || columnName == "G")
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
                        }
                        else
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
                        }

                        if (columnName == "B" || columnName == "G")
                        {
                            workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                            workSheet.Cells[rowIndex, 2].Value = items.DocumentNumber;
                            workSheet.Cells[rowIndex, 3].Value = items.FinalRegistrationNumber;
                            workSheet.Cells[rowIndex, 4].Value = items.NatureOfDocument;
                            workSheet.Cells[rowIndex, 5].Value = items.VillageName;
                            workSheet.Cells[rowIndex, 6].Value = items.PropertyDetails;
                            workSheet.Cells[rowIndex, 7].Value = items.Executant;
                            workSheet.Cells[rowIndex, 8].Value = items.Claimant;
                            workSheet.Cells[rowIndex, 9].Value = items.IntegrationDepartmentName;
                        }
                        else
                        {
                            workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                            workSheet.Cells[rowIndex, 2].Value = items.DocumentNumber;
                            workSheet.Cells[rowIndex, 3].Value = items.FinalRegistrationNumber;
                            workSheet.Cells[rowIndex, 4].Value = items.NatureOfDocument;
                            workSheet.Cells[rowIndex, 5].Value = items.VillageName;
                            workSheet.Cells[rowIndex, 6].Value = items.PropertyDetails;
                            workSheet.Cells[rowIndex, 7].Value = items.Executant;
                            workSheet.Cells[rowIndex, 8].Value = items.Claimant;
                            workSheet.Cells[rowIndex, 9].Value = items.Reference_AcknowledgementNumber;
                            workSheet.Cells[rowIndex, 10].Value = items.IntegrationDepartmentName;
                            workSheet.Cells[rowIndex, 11].Value = items.UploadDate;

                        }

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

                        workSheet.Cells[rowIndex, 3].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 4].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 7].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        //workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;

                        rowIndex++;
                    }

                    if (columnName == "B" || columnName == "G")
                    {
                        using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 9])
                        {
                            Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        }
                    }
                    else
                    {
                        using (ExcelRange Rng = workSheet.Cells[5, 1, (rowIndex - 1), 11])
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

        #endregion

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