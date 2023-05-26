#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DailyReceiptDetailsController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.DailyReceiptDetails;
using CustomModels.Models.MISReports.IndexIIReports;
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
    public class DailyReceiptDetailsController : Controller
    {
        ServiceCaller caller = null;
        string errormessage = string.Empty;

        /// <summary>
        /// Daily Receipt Details View
        /// </summary>
        /// <returns>returns DailyReceiptDetailsview</returns>
        [EventAuditLogFilter(Description = "Load Daily Receipt Details Data Table")]
        public ActionResult DailyReceiptDetails()
        {
            try
            {

                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DailyReceiptsDetails;
                caller = new ServiceCaller("DailyReceiptDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                int OfficeID = KaveriSession.Current.OfficeID;
                DailyReceiptDetailsViewModel reqModel = caller.GetCall<DailyReceiptDetailsViewModel>("DailyReceiptDetails", new { OfficeID = OfficeID });
                if (reqModel == null)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Daily Receipt Details View", URLToRedirect = "/Home/HomePage" });

                }
                return View(reqModel);

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Daily Receipt Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Loads Daily Receipt Details DataTable
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns> Data Table</returns>
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        [EventAuditLogFilter(Description = "Load Daily Receipt Details Data Table")]
        public ActionResult LoadDailyReceiptDetailsTable(FormCollection formCollection)
        {
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["FromDate"];
                string ToDate = formCollection["ToDate"];
                string SROOfficeID = formCollection["SROOfficeID"];
                string DROfficeID = formCollection["DROfficeID"];
                string ModuleID = formCollection["ModuleID"];
                string FeeTypeID = formCollection["FeeTypeID"];
                int SroId = Convert.ToInt32(SROOfficeID);
                int DroId = Convert.ToInt32(DROfficeID);
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                System.Text.RegularExpressions.Regex regx = new Regex("/^[^<>] +$/");
                Match mtch = regx.Match((string)searchValue);
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
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
                    #region Validation For Allowing Date range between only Current Financial year
                    DateTime CurrentDate = DateTime.Now;
                    int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                    int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                    int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                    int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                    if (CMonth > 3)
                    {
                        if (FromDateyear < CYear)
                        {
                            var emptyData = Json(new
                            {
                                draw = formCollection["draw"],
                                recordsTotal = 0,
                                recordsFiltered = 0,
                                data = "",
                                status = false,
                                errorMessage = "Records of current financial year only can be seen at a time"
                            });
                            emptyData.MaxJsonLength = Int32.MaxValue;
                            return emptyData;

                        }
                        else
                        {
                            if (FromDateyear > CYear)
                            {
                                if (FromDateyear == CYear + 1)
                                {
                                    if (FromDateMonth > 3)
                                    {
                                        var emptyData = Json(new
                                        {
                                            draw = formCollection["draw"],
                                            recordsTotal = 0,
                                            recordsFiltered = 0,
                                            data = "",
                                            status = false,
                                            errorMessage = "Records of current financial year only can be seen at a time"
                                        });
                                        emptyData.MaxJsonLength = Int32.MaxValue;
                                        return emptyData;
                                    }
                                }
                                else
                                {
                                    var emptyData = Json(new
                                    {
                                        draw = formCollection["draw"],
                                        recordsTotal = 0,
                                        recordsFiltered = 0,
                                        data = "",
                                        status = false,
                                        errorMessage = "Records of current financial year only can be seen at a time"
                                    });
                                    emptyData.MaxJsonLength = Int32.MaxValue;
                                    return emptyData;
                                }
                            }
                            else if (FromDateyear == CYear)
                            {
                                if (FromDateMonth < 4)
                                {
                                    var emptyData = Json(new
                                    {
                                        draw = formCollection["draw"],
                                        recordsTotal = 0,
                                        recordsFiltered = 0,
                                        data = "",
                                        status = false,
                                        errorMessage = "Records of current financial year only can be seen at a time"
                                    });
                                    emptyData.MaxJsonLength = Int32.MaxValue;
                                    return emptyData;
                                }
                            }
                        }
                    }
                    else if (CMonth <= 3)
                    {
                        if (FromDateyear < CYear)
                        {
                            if (FromDateyear == CYear - 1)
                            {
                                if (FromDateMonth < 4)
                                {
                                    var emptyData = Json(new
                                    {
                                        draw = formCollection["draw"],
                                        recordsTotal = 0,
                                        recordsFiltered = 0,
                                        data = "",
                                        status = false,
                                        errorMessage = "Records of current financial year only can be seen at a time"
                                    });
                                    emptyData.MaxJsonLength = Int32.MaxValue;
                                    return emptyData;

                                }

                            }
                            else
                            {
                                var emptyData = Json(new
                                {
                                    draw = formCollection["draw"],
                                    recordsTotal = 0,
                                    recordsFiltered = 0,
                                    data = "",
                                    status = false,
                                    errorMessage = "Records of current financial year only can be seen at a time"
                                });
                                emptyData.MaxJsonLength = Int32.MaxValue;
                                return emptyData;
                            }
                        }
                    }
                    #endregion
                }

                caller = new ServiceCaller("CommonsApiController");
                //short OfficeID = KaveriSession.Current.OfficeID;
                //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

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
                //}







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
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                DailyReceiptDetailsViewModel reqModel = new DailyReceiptDetailsViewModel();
                reqModel.FeeTypeID = Convert.ToInt32(FeeTypeID);
                reqModel.startLen = startLen;
                reqModel.totalNum = totalNum;
                reqModel.FromDate = fromDate;
                reqModel.ToDate = ToDate;
                reqModel.ModuleID = Convert.ToInt32(ModuleID);
                reqModel.SROfficeID = SroId;
                reqModel.DateTime_ToDate = toDate;
                reqModel.DateTime_FromDate = frmDate;
                reqModel.DROfficeID = Convert.ToInt32(DROfficeID);

                caller = new ServiceCaller("DailyReceiptDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in Daily Receipts Details datatable
                //int totalCount = caller.PostCall<DailyReceiptDetailsViewModel, int>("GetDailyReceiptsTotalCount", reqModel, out errorMessage);

                //if (searchValue != null && searchValue != "")
                //{
                //    reqModel.startLen = 0;
                //    reqModel.totalNum = totalCount;
                //}
                reqModel.SearchValue = searchValue;
                //To get records of indexII report table 
                DailyReceiptDetailsResModel resModel = caller.PostCall<DailyReceiptDetailsViewModel, DailyReceiptDetailsResModel>("GetDailyReceiptTableData", reqModel, out errorMessage);


                int totalCount = resModel.DailyReceiptsDetailsList.Count;
                if (resModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting Daily Receipt Details" });
                }
                IEnumerable<DailyReceiptDetailsModel> result = resModel.DailyReceiptsDetailsList;

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
                        m.ArticleName.ToLower().Contains(searchValue.ToLower()) ||
                        m.SerialNo.ToString().Contains(searchValue.ToLower()) ||
                        m.FRN.ToString().Contains(searchValue) ||
                        m.ReceiptNo.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.DateOfReceipt.ToLower().Contains(searchValue.ToLower()) ||
                        m.DescriptionEnglish.ToLower().Contains(searchValue.ToLower()) ||
                        m.Description.ToLower().Contains(searchValue.ToLower()) ||
                        m.Amount.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.AmountPaid.ToString().ToLower().Contains(searchValue.ToLower()) ||
                        m.ChallanNo.ToString().ToLower().Contains(searchValue.ToLower()) ||


                        m.Description.ToString().Contains(searchValue));
                        totalCount = result.Count();

                    }
                }

                var gridData = result.Select(DailyReceiptDetailsModel => new
                {
                    SrNo = DailyReceiptDetailsModel.SerialNo,
                    SroName = DailyReceiptDetailsModel.SroName,
                    PresentDateTime = DailyReceiptDetailsModel.PresentDateTime,
                    DocumentNumber = DailyReceiptDetailsModel.DocumentNo,
                    ArticleName = DailyReceiptDetailsModel.ArticleName,
                    FinalRegistrationNumber = DailyReceiptDetailsModel.FRN,
                    MarriageCaseNumber = DailyReceiptDetailsModel.MarriageCaseNumber,
                    NoticeNumber = DailyReceiptDetailsModel.NoticeNumber,
                    ReceiptNumber = DailyReceiptDetailsModel.ReceiptNo,
                    DescriptionEnglish = DailyReceiptDetailsModel.DescriptionEnglish,
                    DateOfReceipt = DailyReceiptDetailsModel.DateOfReceipt,
                    Description = DailyReceiptDetailsModel.Description,
                    StampType = DailyReceiptDetailsModel.StampType,
                    Amount = DailyReceiptDetailsModel.Amount.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    AmountPaid = DailyReceiptDetailsModel.AmountPaid.ToString("N2", System.Globalization.CultureInfo.CreateSpecificCulture("hi-IN")),
                    DDChallanNo = DailyReceiptDetailsModel.ChallanNo

                });

                String PDFDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style='width:75%;' class='btn btn-group-md btn-success' onclick=PDFDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeID + "','" + DROfficeID + "','" + ModuleID + "','" + FeeTypeID + "')><i style='padding-right:3%;' class='fa fa-file-pdf-o'></i>Download as PDF</button>";
                String ExcelDownloadBtn = result.Count() == 0 ? "" : "<button type ='button' style = 'width:75%;' class='btn btn-group-md btn-success' onclick=EXCELDownloadFun('" + fromDate + "','" + ToDate + "','" + SROOfficeID + "','" + DROfficeID + "','" + ModuleID + "','" + FeeTypeID + "')><i style = 'padding-right:3%;' class='fa fa-file-excel-o'></i>Download as Excel</button>";


                if (searchValue != null && searchValue != "")
                {
                    JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = resModel.TotalRecords,
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
                        recordsTotal = resModel.TotalRecords,
                        status = "1",
                        recordsFiltered = resModel.TotalRecords,
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
                return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting Daily Receipts Details" });
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

        [EventAuditLogFilter(Description = "Export Daily Receipt Details To PDF")]
        public ActionResult ExportDailyReceiptDetailsToPDF(string FromDate, string ToDate, string SROOfficeListID, string DROfficeID, string ModuleID, string FeeTypeID, string SelectedDistrict, string SelectedSRO)
        {
            try
            {
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                bool boolFrmDate = false;
                bool boolToDate = false;

                if (string.IsNullOrEmpty(FromDate))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date required", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });
                }
                if (string.IsNullOrEmpty(ToDate))
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "To Date required", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });
                }

                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                #region Server Side Validation   
                #region Validate date Inputs
                if (!boolFrmDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });

                }
                if (!boolToDate)
                {
                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });

                }
                bool isTodateGreater = CommonFunctions.IsDateGreaterThanCurrentDate(toDate);
                if (frmDate > toDate)
                {

                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "From Date can not be larger than To Date", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });

                }

                #endregion
                #region Validation For Allowing Date range between only Current Financial year
                DateTime CurrentDate = DateTime.Now;
                int CMonth = Convert.ToInt32(DateTime.Now.ToString("MM"));
                int CYear = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int FromDateyear = Convert.ToInt32(frmDate.ToString("yyyy"));
                int FromDateMonth = Convert.ToInt32(frmDate.ToString("MM"));
                if (CMonth > 3)
                {
                    if (FromDateyear < CYear)
                    {
                        return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });
                    }
                    else
                    {
                        if (FromDateyear > CYear)
                        {
                            if (FromDateyear == CYear + 1)
                            {
                                if (FromDateMonth > 3)
                                {
                                    return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });

                                }
                            }
                            else
                            {
                                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });
                            }
                        }
                        else if (FromDateyear == CYear)
                        {
                            if (FromDateMonth < 4)
                            {
                                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });
                            }
                        }
                    }
                }
                else if (CMonth <= 3)
                {
                    if (FromDateyear < CYear)
                    {
                        if (FromDateyear == CYear - 1)
                        {
                            if (FromDateMonth < 4)
                                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });
                        }
                        else
                        {
                            return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Records of current financial year only can be seen at a time", URLToRedirect = "/MISReports/DailyReceiptDetails/DailyReceiptDetails" });

                        }
                    }
                }
                #endregion


                caller = new ServiceCaller("CommonsApiController");
                //short OfficeID = KaveriSession.Current.OfficeID;
                //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

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
                //}







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




                DailyReceiptDetailsViewModel model = new DailyReceiptDetailsViewModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    ModuleID = Convert.ToInt32(ModuleID),
                    DROfficeID = Convert.ToInt32(DROfficeID),
                    FeeTypeID = Convert.ToInt32(FeeTypeID),
                    startLen = 0,
                    totalNum = 10,
                };

                List<DailyReceiptDetailsModel> objListItemsToBeExported = new List<DailyReceiptDetailsModel>();
                model.IsPdf = true;
                caller = new ServiceCaller("DailyReceiptDetailsAPIController");
                TimeSpan objTimeSpan = new TimeSpan(0, 30, 0);
                caller.HttpClient.Timeout = objTimeSpan;
                //To get total count of records in indexII report datatable
                int totalCount = caller.PostCall<DailyReceiptDetailsViewModel, int>("GetDailyReceiptsTotalCount", model);
                model.totalNum = totalCount;

                // To get total records of indexII report table

                DailyReceiptDetailsResModel resModel = caller.PostCall<DailyReceiptDetailsViewModel, DailyReceiptDetailsResModel>("GetDailyReceiptTableData", model, out errorMessage);
                objListItemsToBeExported = resModel.DailyReceiptsDetailsList;

                if (objListItemsToBeExported == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                }

                //string fileName = string.Format("ECDataAudit{0}{1}_{2}_{3}.pdf",  DateTime.Now.ToString().Replace(" ", "").Replace("-", "").Replace(":", ""), FromDate.Replace("/", ""), ToDate.Replace("/", ""));
                string fileName = string.Format("DailyReceiptDetails" + ".pdf");
                string filepath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
                string pdfHeader = string.Format("Daily Receipt Details (Between {0} and {1} )", FromDate, ToDate);

                //caller = new ServiceCaller("CommonsApiController");
                ////To get SRONAME
                //string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID });

                //if (SROName == null)
                //{
                //    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                //}
                //Create Temp PDF File
                byte[] pdfBytes = CreatePDFFile(objListItemsToBeExported, fileName, pdfHeader, ModuleID, SelectedDistrict, SelectedSRO);

                return File(pdfBytes, System.Net.Mime.MediaTypeNames.Application.Octet, "DaillyReceiptDetails_" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".pdf");

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
        private byte[] CreatePDFFile(List<DailyReceiptDetailsModel> objListItemsToBeExported, string fileName, string pdfHeader, string ModuleId, string SelectedDistrict, string SelectedSRO)
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
                            var DistrictChunk = new Chunk("District : ", blackListTextFont);

                            var titleChunk = new Chunk("Print Date Time : ", blackListTextFont);
                            var totalChunk = new Chunk("Total Records: ", blackListTextFont);
                            var SroNameChunk = new Chunk("SRO Name: ", blackListTextFont);
                            var SroName = new Chunk(SelectedSRO + "     ", redListTextFont);
                            var District = new Chunk(SelectedDistrict + "     ", redListTextFont);

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
                            District
                        };
                            var DistrictPhrase = new Phrase(DistrictChunk)
                        {
                            SroName
                        };

                            doc.Add(addHeading);
                            doc.Add(addSpace);
                            doc.Add(SroNamePhrase);
                            doc.Add(DistrictPhrase);
                            doc.Add(titlePhrase);
                            //doc.Add(SroNamePhrase);
                            doc.Add(totalPhrase);

                            doc.Add(addSpace);

                            if (ModuleID == (int)CommonEnum.ModuleNames.All)
                            {
                                doc.Add(DailyReceiptAllTbl(objListItemsToBeExported));

                            }
                            else if (ModuleID == (int)CommonEnum.ModuleNames.DocumentReg)
                            {
                                doc.Add(DailyReceiptDocRegTbl(objListItemsToBeExported));

                            }
                            else if (ModuleID == (int)CommonEnum.ModuleNames.StampDuty)
                            {
                                doc.Add(DailyReceiptStampDutyTbl(objListItemsToBeExported));

                            }
                            else if (ModuleID == (int)CommonEnum.ModuleNames.Others)
                            {
                                doc.Add(DailyReceiptOthersTbl(objListItemsToBeExported));

                            }
                            //else if (ModuleID == (int)CommonEnum.ModuleNames.MarrriageReg)
                            //{
                            //    doc.Add(DailyReceiptMarRegTbl(objListItemsToBeExported));

                            //}
                            //else if (ModuleID == (int)CommonEnum.ModuleNames.MarriageNotice)
                            //{
                            //    doc.Add(DailyReceiptMarNoticeTbl(objListItemsToBeExported));

                            //}
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
        /// Returns DailyReceipt PDFTable when "All" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns></returns>
        private PdfPTable DailyReceiptAllTbl(List<DailyReceiptDetailsModel> objListItemsToBeExported)
        {
            string SrNo = "Sr No.";
            string SroName = "Sro Name";
            string ReceiptNo = "Receipt Number";
            string DescEng = "Description English";
            string DateOfReceipt = "Date Of Receipt";
            string Desc = "Description";
            string AmtPaid = "Amount Paid";

            try
            {
                PdfPCell cell0 = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;

                string[] col = { SrNo, SroName, ReceiptNo, DescEng, DateOfReceipt, Desc, AmtPaid };
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
                table.SetWidths(new Single[] { 3, 4, 5, 5, 5, 5, 5 });
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

                    cell0 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell0.BackgroundColor = BaseColor.WHITE;

                    cell1 = new PdfPCell(new Phrase(items.ReceiptNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell2 = new PdfPCell(new Phrase(items.DescriptionEnglish, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(items.DateOfReceipt, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.Description, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell4.BackgroundColor = BaseColor.WHITE;

                    cell5 = new PdfPCell(new Phrase(items.AmountPaid.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell5.BackgroundColor = BaseColor.WHITE;

                    cell6 = new PdfPCell(new Phrase(items.SroName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell6.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell6.BackgroundColor = BaseColor.WHITE;

                    table.AddCell(cell0);
                    table.AddCell(cell6);
                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                }
                return table;
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
        private PdfPTable DailyReceiptDocRegTbl(List<DailyReceiptDetailsModel> objListItemsToBeExported)
        {
            string SrNo = "Sr No";
            string SroName = "SroName";
            string DocNo = "Document Number ";
            string ArticleName = "Article Name ";
            string FRN = "Final Registration Number";
            string ReceiptNo = " Receipt Number";
            string DateOfReceipt = "Date of Receipt";
            string Description = "Description";
            string Amount = "Amount";

            try
            {
                PdfPCell cell0 = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;
                PdfPCell cell7 = null;
                PdfPCell cell8 = null;

                string[] col = { SrNo, SroName, DocNo, ArticleName, FRN, ReceiptNo, DateOfReceipt, Description, Amount };
                PdfPTable table = new PdfPTable(9)
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
                table.SetWidths(new Single[] { 3, 4, 5, 5, 5, 5, 5, 5, 5 });
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
                    cell0 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
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
                    cell2 = new PdfPCell(new Phrase(items.ArticleName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(items.FRN, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.ReceiptNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell4.BackgroundColor = BaseColor.WHITE;

                    cell5 = new PdfPCell(new Phrase(items.DateOfReceipt, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell5.BackgroundColor = BaseColor.WHITE;

                    cell6 = new PdfPCell(new Phrase(items.Description, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell6.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell6.BackgroundColor = BaseColor.WHITE;

                    cell7 = new PdfPCell(new Phrase(items.AmountPaid.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell7.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell7.BackgroundColor = BaseColor.WHITE;

                    cell8 = new PdfPCell(new Phrase(items.SroName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell8.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell8.BackgroundColor = BaseColor.WHITE;

                    table.AddCell(cell0);
                    table.AddCell(cell8);
                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);
                    table.AddCell(cell6);
                    table.AddCell(cell7);

                }
                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }
        //private PdfPTable DailyReceiptMarRegTbl(List<DailyReceiptDetailsModel> objListItemsToBeExported)
        //{

        //    string MarCaseNo = "Marriage Case Number";
        //    string ReceiptNo = "Receipt Number";
        //    string DateOfReceipt = "Date Of Receipt";
        //    string Desc = "Description";
        //    string AmtPaid = "Amount Paid";


        //    try
        //    {
        //        PdfPCell cell1 = null;
        //        PdfPCell cell2 = null;
        //        PdfPCell cell3 = null;
        //        PdfPCell cell4 = null;
        //        PdfPCell cell5 = null;
        //        string[] col = { MarCaseNo, ReceiptNo, DateOfReceipt, Desc, AmtPaid };
        //        PdfPTable table = new PdfPTable(5)
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
        //        table.SetWidths(new Single[] { 5, 5, 5, 5, 5 });
        //        /*
        //        * by default tables 'collapse' on surrounding elements,
        //        * so you need to explicitly add spacing
        //        */
        //        //table.SpacingBefore = 10;
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
        //            cell1 = new PdfPCell(new Phrase(items.MarriageCaseNumber, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell1.BackgroundColor = BaseColor.WHITE;
        //            cell2 = new PdfPCell(new Phrase(items.ReceiptNo.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell2.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell2.BackgroundColor = BaseColor.WHITE;

        //            cell3 = new PdfPCell(new Phrase(items.DateOfReceipt, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell3.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell3.BackgroundColor = BaseColor.WHITE;

        //            cell4 = new PdfPCell(new Phrase(items.Description, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell4.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell4.BackgroundColor = BaseColor.WHITE;

        //            cell5 = new PdfPCell(new Phrase(items.Amount.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell5.BackgroundColor = BaseColor.WHITE;
        //            table.AddCell(cell1);
        //            table.AddCell(cell2);
        //            table.AddCell(cell3);
        //            table.AddCell(cell4);
        //            table.AddCell(cell5);
        //        }
        //        return table;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        //private PdfPTable DailyReceiptMarNoticeTbl(List<DailyReceiptDetailsModel> objListItemsToBeExported)
        //{

        //    string MarNoticeNo = "Notice Number";
        //    string ReceiptNo = "Receipt Number";
        //    string DateOfReceipt = "Date Of Receipt";
        //    string Desc = "Description";
        //    string AmtPaid = "Amount Paid";
        //    try
        //    {
        //        PdfPCell cell1 = null;
        //        PdfPCell cell2 = null;
        //        PdfPCell cell3 = null;
        //        PdfPCell cell4 = null;
        //        PdfPCell cell5 = null;

        //        string[] col = { MarNoticeNo, ReceiptNo, DateOfReceipt, Desc, AmtPaid };
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
        //        table.SetWidths(new Single[] { 5, 5, 5, 5, 5, 5, 5 });
        //        /*
        //        * by default tables 'collapse' on surrounding elements,
        //        * so you need to explicitly add spacing
        //        */
        //        //table.SpacingBefore = 10;
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
        //            cell1 = new PdfPCell(new Phrase(items.NoticeNumber.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell1.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell1.BackgroundColor = BaseColor.WHITE;
        //            cell2 = new PdfPCell(new Phrase(items.ReceiptNo.ToString(), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell2.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell2.BackgroundColor = BaseColor.WHITE;

        //            cell3 = new PdfPCell(new Phrase(items.DateOfReceipt, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell3.HorizontalAlignment = Element.ALIGN_CENTER;
        //            cell3.BackgroundColor = BaseColor.WHITE;

        //            cell4 = new PdfPCell(new Phrase(items.Description, tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell4.HorizontalAlignment = Element.ALIGN_LEFT;
        //            cell4.BackgroundColor = BaseColor.WHITE;

        //            cell5 = new PdfPCell(new Phrase(items.Amount.ToString("F"), tableContentFont))
        //            {
        //                BackgroundColor = new BaseColor(204, 255, 255)
        //            };
        //            cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
        //            cell5.BackgroundColor = BaseColor.WHITE;
        //            table.AddCell(cell1);
        //            table.AddCell(cell2);
        //            table.AddCell(cell3);
        //            table.AddCell(cell4);
        //            table.AddCell(cell5);

        //        }
        //        return table;
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Returns DailyReceipt PDFTable when "Stamp Duty" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns></returns>
        private PdfPTable DailyReceiptStampDutyTbl(List<DailyReceiptDetailsModel> objListItemsToBeExported)
        {
            string SrNo = "Sr No";
            string SroName = "Sro Name";
            string PresentDateTime = "Present Date Time";
            string DocumentNumber = "Document Number";
            string FRN = "Final Registration Number";
            string StampType = "Stamp Type";
            string Amount = "Amount";
            string DDChallanNumber = "DD Challan Number";

            try
            {
                PdfPCell cell0 = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;
                PdfPCell cell7 = null;


                string[] col = { SrNo, SroName, PresentDateTime, DocumentNumber, FRN, StampType, DDChallanNumber, Amount };
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
                table.SetWidths(new Single[] { 3, 4, 5, 5, 5, 5, 5, 5 });
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
                    cell0 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell0.BackgroundColor = BaseColor.WHITE;
                    cell1 = new PdfPCell(new Phrase(items.PresentDateTime, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell2 = new PdfPCell(new Phrase(items.DocumentNo, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(items.FRN, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell3.BackgroundColor = BaseColor.WHITE;


                    cell4 = new PdfPCell(new Phrase(items.StampType.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell4.BackgroundColor = BaseColor.WHITE;



                    cell5 = new PdfPCell(new Phrase(items.Amount.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell5.BackgroundColor = BaseColor.WHITE;

                    cell6 = new PdfPCell(new Phrase(items.ChallanNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell6.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell6.BackgroundColor = BaseColor.WHITE;

                    cell7 = new PdfPCell(new Phrase(items.SroName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell7.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell7.BackgroundColor = BaseColor.WHITE;

                    table.AddCell(cell0);
                    table.AddCell(cell7);
                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell6);
                    table.AddCell(cell5);


                }
                return table;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns DailyReceipt PDFTable when "Others" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <returns></returns>
        private PdfPTable DailyReceiptOthersTbl(List<DailyReceiptDetailsModel> objListItemsToBeExported)
        {
            string SrNo = "Sr No";
            string SroName = "Sro Name";
            string ReceiptNo = "ReceiptNumber";
            string DescEng = "DescriptionEnglish";
            string DateOfReceipt = "DateOfReceipt";
            string Desc = "Description";
            string Amt = "Amount";


            try
            {
                PdfPCell cell0 = null;
                PdfPCell cell1 = null;
                PdfPCell cell2 = null;
                PdfPCell cell3 = null;
                PdfPCell cell4 = null;
                PdfPCell cell5 = null;
                PdfPCell cell6 = null;


                string[] col = { SrNo, SroName, ReceiptNo, DescEng, DateOfReceipt, Desc, Amt };
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
                table.SetWidths(new Single[] { 5, 5, 5, 5, 5, 5, 5 });
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
                    cell0 = new PdfPCell(new Phrase(items.SerialNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell0.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell0.BackgroundColor = BaseColor.WHITE;
                    cell1 = new PdfPCell(new Phrase(items.ReceiptNo.ToString(), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell1.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell1.BackgroundColor = BaseColor.WHITE;
                    cell2 = new PdfPCell(new Phrase(items.DescriptionEnglish, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell2.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell2.BackgroundColor = BaseColor.WHITE;

                    cell3 = new PdfPCell(new Phrase(items.DateOfReceipt, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell3.HorizontalAlignment = Element.ALIGN_CENTER;
                    cell3.BackgroundColor = BaseColor.WHITE;

                    cell4 = new PdfPCell(new Phrase(items.Description, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell4.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell4.BackgroundColor = BaseColor.WHITE;

                    cell5 = new PdfPCell(new Phrase(items.AmountPaid.ToString("F"), tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell5.HorizontalAlignment = Element.ALIGN_RIGHT;
                    cell5.BackgroundColor = BaseColor.WHITE;

                    cell6 = new PdfPCell(new Phrase(items.SroName, tableContentFont))
                    {
                        BackgroundColor = new BaseColor(204, 255, 255)
                    };
                    cell6.HorizontalAlignment = Element.ALIGN_LEFT;
                    cell6.BackgroundColor = BaseColor.WHITE;
                    table.AddCell(cell0);
                    table.AddCell(cell6);
                    table.AddCell(cell1);
                    table.AddCell(cell2);
                    table.AddCell(cell3);
                    table.AddCell(cell4);
                    table.AddCell(cell5);

                }
                return table;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        //To add paging to Daily Receipts Details report table in PDF
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
        [EventAuditLogFilter(Description = "Export Daily Receipt Details table To Excel")]

        public ActionResult ExportDailyReceiptDetailsToExcel(string FromDate, string ToDate, string SROOfficeListID, string DROfficeID, string ModuleID, string FeeTypeID, string SelectedDistrict, string SelectedSRO)
        {
            try
            {
                caller = new ServiceCaller("CommonsApiController");
                string fileName = "DailyReceiptDetailsExcel" + DateTime.Now.ToString("dd-MM-yyyy-HH_MM_ss") + ".xlsx";
                CommonFunctions objCommon = new CommonFunctions();
                string errorMessage = string.Empty;
                DateTime frmDate, toDate;
                DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);

                DailyReceiptDetailsViewModel model = new DailyReceiptDetailsViewModel
                {
                    DateTime_FromDate = frmDate,
                    DateTime_ToDate = toDate,
                    SROfficeID = Convert.ToInt32(SROOfficeListID),
                    DROfficeID = Convert.ToInt32(DROfficeID),
                    ModuleID = Convert.ToInt32(ModuleID),
                    FeeTypeID = Convert.ToInt32(FeeTypeID),
                    startLen = 0,
                    totalNum = 10,
                };
                model.IsExcel = true;

                if (SROOfficeListID != "0")
                {
                    string SROName = caller.GetCall<string>("GetSroName", new { SROfficeID = SROOfficeListID }, out errorMessage);
                    if (SROName == null)
                    {
                        return Json(new { success = false, errorMessage = "Error Occured While Processing..." });

                    }
                }
                caller = new ServiceCaller("DailyReceiptDetailsAPIController");
                //int totalCount = caller.PostCall<DailyReceiptDetailsViewModel, int>("GetDailyReceiptsTotalCount", model);
                //model.totalNum = totalCount;
                List<DailyReceiptDetailsModel> objListItemsToBeExported = new List<DailyReceiptDetailsModel>();
                DailyReceiptDetailsResModel ResModel = caller.PostCall<DailyReceiptDetailsViewModel, DailyReceiptDetailsResModel>("GetDailyReceiptTableData", model, out errorMessage);
                if (ResModel == null)
                {
                    return Json(new { success = false, errorMessage = "Error Occured While Processing..." });
                }

                objListItemsToBeExported = ResModel.DailyReceiptsDetailsList;

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
                string excelHeader = string.Format("Daily Receipt Details Between ({0} and {1})", FromDate, ToDate);


                if (Convert.ToInt32(ModuleID) == (int)CommonEnum.ModuleNames.All)
                {
                    createdExcelPath = CreateExcelForALL(objListItemsToBeExported, fileName, excelHeader, SelectedDistrict, SelectedSRO);
                }
                else if (Convert.ToInt32(ModuleID) == (int)CommonEnum.ModuleNames.DocumentReg)
                {
                    createdExcelPath = CreateExcelForDocReg(objListItemsToBeExported, fileName, excelHeader, SelectedDistrict, SelectedSRO);
                }
                else if (Convert.ToInt32(ModuleID) == (int)CommonEnum.ModuleNames.StampDuty)
                {
                    createdExcelPath = CreateExcelForStampDuty(objListItemsToBeExported, fileName, excelHeader, SelectedDistrict, SelectedSRO);
                }
                else if (Convert.ToInt32(ModuleID) == (int)CommonEnum.ModuleNames.Others)
                {
                    createdExcelPath = CreateExcelForOther(objListItemsToBeExported, fileName, excelHeader, SelectedDistrict, SelectedSRO);
                }
                //else if (Convert.ToInt32(ModuleID) == (int)CommonEnum.ModuleNames.MarrriageReg)
                //{
                //     createdExcelPath = CreateExcelForMarrReg(objListItemsToBeExported, fileName, excelHeader, SROName);
                //}
                //else if (Convert.ToInt32(ModuleID) == (int)CommonEnum.ModuleNames.MarriageNotice)
                //{
                //     createdExcelPath = CreateExcelForMarrNotice(objListItemsToBeExported, fileName, excelHeader, SROName);
                //}


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
        /// Create Excel of Daily Receipt Details when "Document Registration" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SROName"></param>
        /// <returns>returns ExcelFilePath</returns>
        private string CreateExcelForDocReg(List<DailyReceiptDetailsModel> objListItemsToBeExported, string fileName, string excelHeader, string SelectedDistrict, string SelectedSRO)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Daily Receipt Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (objListItemsToBeExported.Count() -1 ); //-1 is Added by ShivamB to show only Total records coming from Stored procedure and not the extra one which is added manually in list coming from DAL
                    workSheet.Cells[1, 1, 1, 10].Merge = true;
                    workSheet.Cells[2, 1, 2, 10].Merge = true;
                    workSheet.Cells[3, 1, 3, 10].Merge = true;
                    workSheet.Cells[4, 1, 4, 10].Merge = true;
                    workSheet.Cells[5, 1, 5, 10].Merge = true;


                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 23;
                    workSheet.Column(4).Width = 45;
                    workSheet.Column(5).Width = 45;
                    workSheet.Column(6).Width = 35;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 35;
                    workSheet.Column(9).Width = 45;
                    workSheet.Column(10).Width = 30;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";

                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";



                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "Sro Name";
                    workSheet.Cells[7, 3].Value = "Document Number";
                    //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022
                    workSheet.Cells[7, 4].Value = "Challan Number";
                    //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022

                    workSheet.Cells[7, 5].Value = "Article Name";
                    workSheet.Cells[7, 6].Value = "Final Registration Number";
                    workSheet.Cells[7, 7].Value = "Receipt Number";
                    workSheet.Cells[7, 8].Value = "Date of Receipt";
                    workSheet.Cells[7, 9].Value = "Description";
                    workSheet.Cells[7, 10].Value = "Amount";

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


                        workSheet.Cells[rowIndex, 10].Style.Numberformat.Format = "0.00";

                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SroName;
                        workSheet.Cells[rowIndex, 3].Value = items.DocumentNo;
                        //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022
                        workSheet.Cells[rowIndex, 4].Value = items.ChallanNo;
                        //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022
                        workSheet.Cells[rowIndex, 5].Value = items.ArticleName;
                        workSheet.Cells[rowIndex, 6].Value = items.FRN;
                        workSheet.Cells[rowIndex, 7].Value = items.ReceiptNo;
                        workSheet.Cells[rowIndex, 8].Value = items.DateOfReceipt;
                        workSheet.Cells[rowIndex, 9].Value = items.Description;
                        workSheet.Cells[rowIndex, 10].Value = items.AmountPaid;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 10].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        workSheet.Cells[rowIndex, 5].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    //workSheet.Row(rowIndex - 1).Style.Font.Bold = true;
                    workSheet.Cells[(rowIndex - 1), 1].Value = "";
                    workSheet.Cells[(rowIndex - 1), 6].Value = "";

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 10])
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

        //private string CreateExcelForMarrReg(List<DailyReceiptDetailsModel> objListItemsToBeExported, string fileName, string excelHeader, string SROName)
        //{
        //    string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //    FileInfo templateFile = GetFileInfo(ExcelFilePath);
        //    try
        //    {
        //        //create a new ExcelPackage
        //        using (ExcelPackage package = new ExcelPackage())
        //        {
        //            var workbook = package.Workbook;
        //            var workSheet = package.Workbook.Worksheets.Add("Daily Receipt Details");
        //            workSheet.Cells.Style.Font.Size = 14;

        //            workSheet.Cells[1, 1].Value = excelHeader;
        //            workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
        //            workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
        //            workSheet.Cells[4, 1].Value = "Total Records : " + (objListItemsToBeExported.Count());
        //            workSheet.Cells[1, 1, 1, 12].Merge = true;
        //            workSheet.Cells[2, 1, 2, 12].Merge = true;
        //            workSheet.Cells[3, 1, 3, 12].Merge = true;
        //            workSheet.Cells[4, 1, 4, 12].Merge = true;
        //            workSheet.Column(6).Style.WrapText = true;
        //            workSheet.Column(7).Style.WrapText = true;
        //            workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //            workSheet.Column(1).Width = 40;
        //            workSheet.Column(2).Width = 30;
        //            workSheet.Column(3).Width = 30;
        //            workSheet.Column(4).Width = 45;

        //            workSheet.Row(1).Style.Font.Bold = true;
        //            workSheet.Row(2).Style.Font.Bold = true;
        //            workSheet.Row(3).Style.Font.Bold = true;
        //            workSheet.Row(4).Style.Font.Bold = true;
        //            int rowIndex = 7;
        //            workSheet.Cells[6, 1].Value = "Marriage Case Number";
        //            workSheet.Cells[6, 2].Value = "Receipt Number";
        //            workSheet.Cells[6, 3].Value = "Date of Receipt";
        //            workSheet.Cells[6, 4].Value = "Description";

        //            foreach (var items in objListItemsToBeExported)
        //            {
        //                workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";

        //                workSheet.Cells[rowIndex, 1].Value = items.MarriageCaseNumber;
        //                workSheet.Cells[rowIndex, 2].Value = items.ReceiptNo;
        //                workSheet.Cells[rowIndex, 3].Value = items.DateOfReceipt;
        //                workSheet.Cells[rowIndex, 4].Value = items.Description;
        //                workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //                rowIndex++;
        //                //Function that passes the current row and adds the column details 
        //            }
        //            //workSheet.Row(rowIndex).Style.Font.Bold = true;

        //            using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex-1), 4])
        //            {
        //                Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //            }
        //            package.SaveAs(templateFile);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return ExcelFilePath;
        //}


        /// <summary>
        /// Create Excel of Daily Receipt Details when "All" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SROName"></param>
        /// <returns>returns ExcelFilePath</returns>
        private string CreateExcelForALL(List<DailyReceiptDetailsModel> objListItemsToBeExported, string fileName, string excelHeader, string SelectedDistrict, string SelectedSRO)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Daily Receipt Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;

                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (objListItemsToBeExported.Count() -1); //-1 is Added by ShivamB to show only Total records coming from Stored procedure and not the extra one which is added manually in list coming from DAL   
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    workSheet.Cells[5, 1, 5, 9].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 30;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 30;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 35;
                    workSheet.Column(9).Width = 35;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(8).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(9).Style.Font.Name = "KNB-TTUmaEN";
                    int rowIndex = 8;
                    workSheet.Row(9).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "SRO Name";
                    workSheet.Cells[7, 3].Value = "Receipt Number";

                    //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022
                    workSheet.Cells[7, 4].Value = "Document Number";
                    workSheet.Cells[7, 5].Value = "Challan Number";
                    //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022

                    workSheet.Cells[7, 6].Value = "Description English";
                    workSheet.Cells[7, 7].Value = "Date Of Receipt";
                    workSheet.Cells[7, 8].Value = "Description";
                    workSheet.Cells[7, 9].Value = "Amount Paid";

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


                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SroName;
                        workSheet.Cells[rowIndex, 3].Value = items.ReceiptNo;

                        //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022
                        workSheet.Cells[rowIndex, 4].Value = items.DocumentNo;
                        workSheet.Cells[rowIndex, 5].Value = items.ChallanNo;
                        //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022

                        workSheet.Cells[rowIndex, 6].Value = items.DescriptionEnglish;
                        workSheet.Cells[rowIndex, 7].Value = items.DateOfReceipt;
                        workSheet.Cells[rowIndex, 8].Value = items.Description;
                        workSheet.Cells[rowIndex, 9].Value = items.AmountPaid;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 2].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 9].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    // workSheet.Row(rowIndex - 1).Style.Font.Bold = true;
                    workSheet.Cells[(rowIndex - 1), 1].Value = "";
                    workSheet.Cells[(rowIndex - 1), 3].Value = "";

                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 9])
                    {
                        Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    package.SaveAs(templateFile);
                }
            }
            catch (Exception ex)
            {
                throw;
            }

            return ExcelFilePath;
        }

        /// <summary>
        /// Create Excel of Daily Receipt Details when "Others" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SROName"></param>
        /// <returns>returns ExcelFilePath</returns>
        private string CreateExcelForOther(List<DailyReceiptDetailsModel> objListItemsToBeExported, string fileName, string excelHeader, string SelectedDistrict, string SelectedSRO)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Daily Receipt Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;

                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;

                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (objListItemsToBeExported.Count() -1 ); //-1 is Added by ShivamB to show only Total records coming from Stored procedure and not the extra one which is added manually in list coming from DAL
                    workSheet.Cells[1, 1, 1, 9].Merge = true;
                    workSheet.Cells[2, 1, 2, 9].Merge = true;
                    workSheet.Cells[3, 1, 3, 9].Merge = true;
                    workSheet.Cells[4, 1, 4, 9].Merge = true;
                    workSheet.Cells[5, 1, 5, 9].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Column(1).Width = 30;
                    workSheet.Column(2).Width = 40;
                    workSheet.Column(3).Width = 35;
                    workSheet.Column(4).Width = 35;
                    workSheet.Column(5).Width = 35;
                    workSheet.Column(6).Width = 40;
                    workSheet.Column(7).Width = 35;
                    workSheet.Column(8).Width = 40;
                    workSheet.Column(9).Width = 35;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

                    workSheet.Cells[7, 1].Value = "Sr No.";
                    workSheet.Cells[7, 2].Value = "SRO Name";
                    workSheet.Cells[7, 3].Value = "ReceiptNumber";

                    //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022
                    workSheet.Cells[7, 4].Value = "Document Number";
                    workSheet.Cells[7, 5].Value = "Challan Number";
                    //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022

                    workSheet.Cells[7, 6].Value = "DescriptionEnglish";
                    workSheet.Cells[7, 7].Value = "DateOfReceipt";
                    workSheet.Cells[7, 8].Value = "Description";
                    workSheet.Cells[7, 9].Value = "Amount";
                    int Counter = 1;
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

                        workSheet.Cells[rowIndex, 1].Value = Counter++;
                        workSheet.Cells[rowIndex, 2].Value = items.SroName;
                        workSheet.Cells[rowIndex, 3].Value = items.ReceiptNo;

                        //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022
                        workSheet.Cells[rowIndex, 4].Value = items.DocumentNo;
                        workSheet.Cells[rowIndex, 5].Value = items.ChallanNo;
                        //Added By ShivamB to view this columns in the DailyReceiptDetails Grid Table on 07-09-2022

                        workSheet.Cells[rowIndex, 6].Value = items.DescriptionEnglish;
                        workSheet.Cells[rowIndex, 7].Value = items.DateOfReceipt;
                        workSheet.Cells[rowIndex, 8].Value = items.Description;
                        workSheet.Cells[rowIndex, 9].Value = items.AmountPaid;
                        workSheet.Cells[rowIndex, 9].Style.Numberformat.Format = "0.00";

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;
                        rowIndex++;
                        //Function that passes the current row and adds the column details 
                        //AddSubRowsForCurrentRow(out row,out workSheet);
                    }
                    workSheet.Cells[(rowIndex - 1), 1].Value = "";
                    workSheet.Cells[(rowIndex - 1), 3].Value = "";
                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 9])
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

        //private string CreateExcelForMarrNotice(List<DailyReceiptDetailsModel> objListItemsToBeExported, string fileName, string excelHeader, string SROName)
        //{
        //    string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
        //    FileInfo templateFile = GetFileInfo(ExcelFilePath);
        //    try
        //    {
        //        //create a new ExcelPackage
        //        using (ExcelPackage package = new ExcelPackage())
        //        {
        //            var workbook = package.Workbook;
        //            var workSheet = package.Workbook.Worksheets.Add("Daily Receipt Details");
        //            workSheet.Cells.Style.Font.Size = 14;

        //            workSheet.Cells[1, 1].Value = excelHeader;
        //            workSheet.Cells[2, 1].Value = "Print Date Time : " + DateTime.Now;
        //            workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
        //            workSheet.Cells[4, 1].Value = "Total Records : " + (objListItemsToBeExported.Count());
        //            workSheet.Cells[1, 1, 1, 12].Merge = true;
        //            workSheet.Cells[2, 1, 2, 12].Merge = true;
        //            workSheet.Cells[3, 1, 3, 12].Merge = true;
        //            workSheet.Cells[4, 1, 4, 12].Merge = true;
        //            workSheet.Column(6).Style.WrapText = true;
        //            workSheet.Column(7).Style.WrapText = true;
        //            workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //            workSheet.Column(1).Width = 30;
        //            workSheet.Column(2).Width = 25;
        //            workSheet.Column(3).Width = 45;
        //            workSheet.Column(4).Width = 25;
        //            workSheet.Column(5).Width = 25;

        //            workSheet.Row(1).Style.Font.Bold = true;
        //            workSheet.Row(2).Style.Font.Bold = true;
        //            workSheet.Row(3).Style.Font.Bold = true;
        //            workSheet.Row(4).Style.Font.Bold = true;
        //            workSheet.Row(6).Style.Font.Bold = true;

        //            int rowIndex = 7;
        //            workSheet.Row(6).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;

        //            workSheet.Cells[6, 1].Value = "NoticeNumber	 ";
        //            workSheet.Cells[6, 2].Value = "Receipt Number";
        //            workSheet.Cells[6, 3].Value = "Date Of Receipt";
        //            workSheet.Cells[6, 4].Value = "Description";
        //            workSheet.Cells[6, 5].Value = "Amount Paid";

        //            foreach (var items in objListItemsToBeExported)
        //            {
        //                workSheet.Cells[rowIndex, 1].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 2].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 3].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 4].Style.Font.Name = "KNB-TTUmaEN";
        //                workSheet.Cells[rowIndex, 5].Style.Font.Name = "KNB-TTUmaEN";

        //                workSheet.Cells[rowIndex, 1].Value = items.NoticeNumber;
        //                workSheet.Cells[rowIndex, 2].Value = items.ReceiptNo;
        //                workSheet.Cells[rowIndex, 3].Value = items.DateOfReceipt;
        //                workSheet.Cells[rowIndex, 4].Value = items.Description;
        //                workSheet.Cells[rowIndex, 5].Value = items.Amount;

        //                workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
        //                workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;

        //                rowIndex++;
        //                //Function that passes the current row and adds the column details 
        //                //AddSubRowsForCurrentRow(out row,out workSheet);
        //            }
        //          //  workSheet.Row(rowIndex - 1).Style.Font.Bold = true;

        //            using (ExcelRange Rng = workSheet.Cells[6, 1, (rowIndex - 1), 5])
        //            {
        //                Rng.Style.Border.Top.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Left.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Right.Style = ExcelBorderStyle.Thin;
        //                Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
        //            }
        //            package.SaveAs(templateFile);
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }

        //    return ExcelFilePath;
        //}

        /// <summary>
        /// Create Excel of Daily Receipt Details when "Stamp Duty" Module is selected
        /// </summary>
        /// <param name="objListItemsToBeExported"></param>
        /// <param name="fileName"></param>
        /// <param name="excelHeader"></param>
        /// <param name="SROName"></param>
        /// <returns>returns ExcelFilePath</returns>
        private string CreateExcelForStampDuty(List<DailyReceiptDetailsModel> objListItemsToBeExported, string fileName, string excelHeader, string SelectedDistrict, string SelectedSRO)
        {
            string ExcelFilePath = HttpContext.Server.MapPath(Path.Combine("~/PDFDocuments/", fileName));
            FileInfo templateFile = GetFileInfo(ExcelFilePath);
            try
            {
                //create a new ExcelPackage
                using (ExcelPackage package = new ExcelPackage())
                {
                    var workbook = package.Workbook;
                    var workSheet = package.Workbook.Worksheets.Add("Daily Receipt Details");
                    workSheet.Cells.Style.Font.Size = 14;

                    workSheet.Cells[1, 1].Value = excelHeader;
                    workSheet.Cells[2, 1].Value = "District : " + SelectedDistrict;
                    workSheet.Cells[3, 1].Value = "SRO : " + SelectedSRO;
                    workSheet.Cells[4, 1].Value = "Print Date Time : " + DateTime.Now;
                    //workSheet.Cells[3, 1].Value = "SRO Name : " + SROName;
                    workSheet.Cells[5, 1].Value = "Total Records : " + (objListItemsToBeExported.Count() -1 ); //-1 is Added by ShivamB to show only Total records coming from Stored procedure and not the extra one which is added manually in list coming from DAL
                    workSheet.Cells[1, 1, 1, 12].Merge = true;
                    workSheet.Cells[2, 1, 2, 12].Merge = true;
                    workSheet.Cells[3, 1, 3, 12].Merge = true;
                    workSheet.Cells[4, 1, 4, 12].Merge = true;
                    workSheet.Cells[5, 1, 5, 12].Merge = true;

                    workSheet.Column(6).Style.WrapText = true;
                    workSheet.Column(7).Style.WrapText = true;
                    workSheet.Row(1).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Column(1).Width = 20;
                    workSheet.Column(2).Width = 30;
                    workSheet.Column(3).Width = 25;
                    workSheet.Column(4).Width = 30;
                    workSheet.Column(5).Width = 35;
                    workSheet.Column(6).Width = 30;
                    workSheet.Column(7).Width = 30;
                    workSheet.Column(8).Width = 50;

                    workSheet.Row(1).Style.Font.Bold = true;
                    workSheet.Row(2).Style.Font.Bold = true;
                    workSheet.Row(3).Style.Font.Bold = true;
                    workSheet.Row(4).Style.Font.Bold = true;
                    workSheet.Row(5).Style.Font.Bold = true;
                    workSheet.Row(7).Style.Font.Bold = true;

                    workSheet.Row(1).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(2).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(3).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(4).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(5).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(6).Style.Font.Name = "KNB-TTUmaEN";
                    workSheet.Row(7).Style.Font.Name = "KNB-TTUmaEN";

                    int rowIndex = 8;
                    workSheet.Row(7).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                    workSheet.Cells[7, 1].Value = "Sr.No";
                    workSheet.Cells[7, 2].Value = "SRO Name";
                    workSheet.Cells[7, 3].Value = "Present Date Time";
                    workSheet.Cells[7, 4].Value = "Document Number";
                    workSheet.Cells[7, 5].Value = "Final Registration Number";
                    workSheet.Cells[7, 6].Value = "Stamp Type";
                    workSheet.Cells[7, 8].Value = "Amount Paid";
                    workSheet.Cells[7, 7].Value = "DD Challan Number";

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


                        workSheet.Cells[rowIndex, 8].Style.Numberformat.Format = "0.00";
                        workSheet.Cells[rowIndex, 1].Value = items.SerialNo;
                        workSheet.Cells[rowIndex, 2].Value = items.SroName;
                        workSheet.Cells[rowIndex, 3].Value = items.PresentDateTime;
                        workSheet.Cells[rowIndex, 4].Value = items.DocumentNo;
                        workSheet.Cells[rowIndex, 5].Value = items.FRN;
                        workSheet.Cells[rowIndex, 6].Value = items.StampType;
                        workSheet.Cells[rowIndex, 8].Value = items.Amount;
                        workSheet.Cells[rowIndex, 7].Value = items.ChallanNo;
                        workSheet.Cells[rowIndex, 7].Style.WrapText = true;

                        workSheet.Row(rowIndex).Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                        workSheet.Row(rowIndex).Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Center;
                        workSheet.Cells[rowIndex, 6].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Left;
                        workSheet.Cells[rowIndex, 8].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Right;


                        rowIndex++;
                        //Function that passes the current row and adds the column details 

                    }
                    //workSheet.Row(rowIndex).Style.Font.Bold = true;
                    workSheet.Cells[(rowIndex - 1), 1].Value = "";
                    //workSheet.Cells[(rowIndex - 1), 3].Value = "";
                    using (ExcelRange Rng = workSheet.Cells[7, 1, (rowIndex - 1), 8])
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
                sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictIDWithFirstRecord", new { DistrictID = DistrictID, FirstRecord = "All" }, out errormessage);
                if (sroOfficeList == null)
                {
                    return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);

                }

                return Json(new { SROOfficeList = sroOfficeList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error in getting sro list." }, JsonRequestBehavior.AllowGet);
            }
        }

        //[HttpGet]
        public ActionResult ValidateSearchParameters(string FromDate, string ToDate, string SROOfficeListID, string DROfficeID, string ModuleID, string FeeTypeID)
        {
            try
            {
                #region Server Side Validation   
                bool boolFrmDate = false;
                bool boolToDate = false;
                DateTime frmDate, toDate;
                System.Text.RegularExpressions.Regex regx = new Regex("^[0-9]*$");
                Match mtchSRO = regx.Match(SROOfficeListID);
                Match mtchDistrict = regx.Match(DROfficeID);
                Match mtchModuleID = regx.Match(ModuleID);
                Match mtchFeeTypeID = regx.Match(FeeTypeID);

                if (string.IsNullOrEmpty(DROfficeID))
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

                if (string.IsNullOrEmpty(ModuleID))
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid Module" }, JsonRequestBehavior.AllowGet);
                }
                else if (!mtchModuleID.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid Module" }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(FeeTypeID))
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid Fee Type" }, JsonRequestBehavior.AllowGet);
                }
                else if (!mtchFeeTypeID.Success)
                {
                    return Json(new { success = false, errorMessage = "Please Enter Valid Fee Type" }, JsonRequestBehavior.AllowGet);
                }

                if (string.IsNullOrEmpty(FromDate))
                    return Json(new { success = false, errorMessage = "From Date required" }, JsonRequestBehavior.AllowGet);

                if (string.IsNullOrEmpty(ToDate))
                    return Json(new { success = false, errorMessage = "To Date required" }, JsonRequestBehavior.AllowGet);
               
                boolFrmDate = DateTime.TryParse(DateTime.ParseExact(FromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);
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
                #endregion
                #region Validation For Allowing Date range between only Current Financial year
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
                #endregion


                //short OfficeID = KaveriSession.Current.OfficeID;
                //short LevelID = caller.GetCall<short>("GetLevelIdByOfficeId", new { OfficeID = OfficeID }, out errormessage);

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
                //}

                //if (!string.IsNullOrEmpty(fromDate) && !string.IsNullOrEmpty(ToDate))
                //{
                //DateTime DateTime_FromDate = Convert.ToDateTime(fromDate);
                //DateTime DateTime_Todate = Convert.ToDateTime(ToDate);

                //if ((DateTime_Todate - DateTime_FromDate).TotalDays > 180)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = "0",
                //        errorMessage = "You can only see records of six months..."
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}
                //     }
                if ((toDate - frmDate).TotalDays > 365)//six months validation by RamanK on 20-09-2019
                {
                    return Json(new { success = false, errorMessage = "Data of only one year can be seen at a time" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true, errorMessage = "" }, JsonRequestBehavior.AllowGet);

                #endregion
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while Validating SearchParameters of Daily Receipts Details", URLToRedirect = "/Home/HomePage" });
            }
        }

    }
}