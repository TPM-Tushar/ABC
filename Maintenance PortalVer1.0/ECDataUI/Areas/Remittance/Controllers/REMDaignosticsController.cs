#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   REMDaignosticsController.cs
    * Author Name       :   Raman Kalegaonkar	
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Remittance module.
*/
#endregion


using CustomModels.Models.Remittance.REMDashboard;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class REMDaignosticsController : Controller
    {
        ServiceCaller caller = null;

        /// <summary>
        /// Remittance Diagnostics Details View
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Remittance Diagnostics Details View")]
        public ActionResult RemittanceDiagnosticsDetailsView(string EncryptedID = "0")//Default EncryptedID is 0 to directly show Details page
        {
            try
            {
                RemitanceDiagnosticsDetailsReqModel reqModel = new RemitanceDiagnosticsDetailsReqModel();
                caller = new ServiceCaller("REMDaignosticsApiController");
                reqModel = caller.GetCall<RemitanceDiagnosticsDetailsReqModel>("RemittanceDiagnosticsDetailsView", new { EncryptedID = EncryptedID });
                // Added BY SB on 2-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.DiagnosticDetails;
                return View(reqModel);
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        [HttpPost]
        [EventAuditLogFilter(Description = "Get Bank Transaction Details List")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetBankTransactionDetailsList(FormCollection formCollection)
        {
            caller = new ServiceCaller("REMDaignosticsApiController");
            try
            {
                #region User Variables and Objects
                string fromDate = formCollection["fromDate"];
                string ToDate = formCollection["ToDate"];
                string DROOfficeListID = formCollection["DROOfficeListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string IsActiveId = formCollection["IsActiveId"];
                bool isActiveId = Convert.ToBoolean(IsActiveId);
                string IsDro = formCollection["IsDro"];
                int isDro = Convert.ToInt32(IsDro);

                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                Regex regx = new Regex("[#$<>]");
                Match mtch = regx.Match((string)searchValue);


                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                #endregion

                #region Server Side Validation
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
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                RemitanceDiagnosticsDetailsReqModel model = new RemitanceDiagnosticsDetailsReqModel
                {
                    Datetime_FromDate = frmDate,
                    Datetime_ToDate = toDate,
                    IsActive = isActiveId,
                    DROOfficeID = Convert.ToInt32(DROOfficeListID),
                    SROOfficeID = Convert.ToInt32(SROOfficeListID),
                };


                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int TransactionStatus = Convert.ToInt32(formCollection["TransactionStatus"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                RemittanceDiagnosticsDetialsWrapperModel WrapperModel = new RemittanceDiagnosticsDetialsWrapperModel();

                WrapperModel.StartLength = startLen;
                WrapperModel.TotalNum = totalNum;
                WrapperModel.Datetime_FromDate = model.Datetime_FromDate;
                WrapperModel.Datetime_ToDate = model.Datetime_ToDate;
                WrapperModel.IsActive = model.IsActive;
                WrapperModel.DROOfficeID = model.DROOfficeID;
                WrapperModel.SROOfficeID = model.SROOfficeID;
                WrapperModel.TransactionStatus = TransactionStatus;
                WrapperModel.IsDro = isDro;

                IEnumerable <BankTransactionDetailsResponseModel> result = caller.PostCall<RemittanceDiagnosticsDetialsWrapperModel, List<BankTransactionDetailsResponseModel>>("GetBankTransactionDetailsList", WrapperModel);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

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
                        result = result.Where(m => m.TransactionID.ToLower().Contains(searchValue.ToLower()) ||
                        m.DateOfUpdate.ToLower().Contains(searchValue.ToLower()) ||
                        m.DocumentID.ToLower().Contains(searchValue.ToLower()) ||
                        m.DROCode.ToLower().Contains(searchValue.ToLower()) ||
                        m.InsertDateTime.ToLower().Contains(searchValue.ToLower()) ||
                        m.InstrumentBankIFSCCode.ToLower().Contains(searchValue.ToLower()) ||
                        m.InstrumentBankMICRCode.ToLower().Contains(searchValue.ToLower()) ||
                        m.InstrumentBankName.ToLower().Contains(searchValue.ToLower()) ||
                        m.InstrumentDate.ToLower().Contains(searchValue.ToLower()) ||
                        m.InstrumentNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.IsDRO.ToLower().Contains(searchValue.ToLower()) ||
                        m.IsReceipt.ToLower().Contains(searchValue.ToLower()) ||
                        m.ReceiptID.ToLower().Contains(searchValue.ToLower()) ||
                        m.ReceiptNumber.ToLower().Contains(searchValue.ToLower()) ||
                        m.ReceiptPaymentMode.ToLower().Contains(searchValue.ToLower()) ||
                        m.Receipt_StampDate.ToLower().Contains(searchValue.ToLower()) ||
                        m.SourceOfReceipt.ToLower().Contains(searchValue.ToLower()) ||
                        m.SROCode.ToLower().Contains(searchValue.ToLower()) ||
                        m.StampDetailsID.ToLower().Contains(searchValue.ToLower()) ||
                        m.StampTypeID.ToLower().Contains(searchValue.ToLower()) ||
                        m.TotalAmount.ToLower().Contains(searchValue.ToLower()) ||
                        m.TransactionID.ToLower().Contains(searchValue.ToLower()));

                    }
                }

                //if (result == null)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


                //if (result.Count() == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = result.Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = result.Count(),
                    status = "1",
                    recordsFiltered = result.Count(),
                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception  e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }
        }

        /// <summary>
        /// Get SRO Office List By District ID
        /// </summary>
        /// <param name="DistrictID"></param>
        /// <returns></returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Get SRO Office List By DistrictID")]
        public ActionResult GetSROOfficeListByDistrictID(long DistrictID)
        {
            string errormessage = string.Empty;
            List<SelectListItem> sroOfficeList = new List<SelectListItem>();
            ServiceCaller caller = new ServiceCaller("CommonsApiController");
            sroOfficeList = caller.GetCall<List<SelectListItem>>("GetSROOfficeListByDistrictID", new { DistrictID = DistrictID }, out errormessage);
            return Json(new { SROOfficeList = sroOfficeList }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get Remittance Details List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Remittance Details List")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetRemittanceDetailsList(FormCollection formCollection)
        {
            try
            {
                caller = new ServiceCaller("REMDaignosticsApiController");
                #region User Variables and Objects
                string fromDate = formCollection["fromDate"];
                string ToDate = formCollection["ToDate"];
                string DROOfficeListID = formCollection["DROOfficeListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string IsActiveId = formCollection["IsActiveId"];
                bool isActiveId = Convert.ToBoolean(IsActiveId);
                int TransactionID = Convert.ToInt32(formCollection["TransactionID"]);
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                Regex regx = new Regex("[#$<>]");
                Match mtch = regx.Match((string)searchValue);
                #endregion

                #region Server Side Validation
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
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                RemitanceDiagnosticsDetailsReqModel model = new RemitanceDiagnosticsDetailsReqModel
                {
                    Datetime_FromDate = frmDate,
                    Datetime_ToDate = toDate,
                    IsActive = isActiveId,
                    DROOfficeID = Convert.ToInt32(DROOfficeListID),
                    SROOfficeID = Convert.ToInt32(SROOfficeListID),
                };


                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                RemittanceDiagnosticsDetialsWrapperModel WrapperModel = new RemittanceDiagnosticsDetialsWrapperModel();

                WrapperModel.StartLength = startLen;
                WrapperModel.TotalNum = totalNum;
                WrapperModel.Datetime_FromDate = model.Datetime_FromDate;
                WrapperModel.Datetime_ToDate = model.Datetime_ToDate;
                WrapperModel.IsActive = model.IsActive;
                WrapperModel.DROOfficeID = model.DROOfficeID;
                WrapperModel.SROOfficeID = model.SROOfficeID;
                WrapperModel.TransactionID = TransactionID;
                IEnumerable<RemittanceDetailsResponseModel> result = caller.PostCall<RemittanceDiagnosticsDetialsWrapperModel, List<RemittanceDetailsResponseModel>>("GetRemittanceDetailsList", WrapperModel);
                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }
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
                        result = result.Where(m => m.TransactionID.ToLower().Contains(searchValue.ToLower())
                        || m.DDOCode.ToLower().Contains(searchValue.ToLower())
                        || m.DeptReferenceCode.ToLower().Contains(searchValue.ToLower())
                        || m.ID.ToLower().Contains(searchValue.ToLower())
                        || m.InsertDateTime.ToLower().Contains(searchValue.ToLower())
                        || m.IPAdd.ToLower().Contains(searchValue.ToLower())
                        || m.PaymentStatusCode.ToLower().Contains(searchValue.ToLower())
                        || m.RemitterName.ToLower().Contains(searchValue.ToLower())
                        || m.StatusCode.ToLower().Contains(searchValue.ToLower())
                        || m.StatusDesc.ToLower().Contains(searchValue.ToLower())
                        || m.TransactionDateTime.ToLower().Contains(searchValue.ToLower())
                        || m.TransactionID.ToLower().Contains(searchValue.ToLower())
                        || m.TransactionStatus.ToLower().Contains(searchValue.ToLower())
                        || m.UIRNumber.ToLower().Contains(searchValue.ToLower())
                        || m.UserID.ToLower().Contains(searchValue.ToLower())

                );

                    }
                }

                //if (result == null)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


                //if (result.Count() == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    //data = result.ToArray(),
                    data = result.Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = result.Count(),
                    status = "1",
                    recordsFiltered = result.Count(),
                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e) 
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }

        }

        /// <summary>
        /// Get Challan Details List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Challan Details List")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetChallanDetailsList(FormCollection formCollection)
        {


            try
            {
                caller = new ServiceCaller("REMDaignosticsApiController");
                #region User Variables and Objects
                string fromDate = formCollection["fromDate"];
                string ToDate = formCollection["ToDate"];
                string DROOfficeListID = formCollection["DROOfficeListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string IsActiveId = formCollection["IsActiveId"];
                bool isActiveId = Convert.ToBoolean(IsActiveId);
                string DeptRefNo = formCollection["DepartmentReferenceNumber"];
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                Regex regx = new Regex("[#$<>]");
                Match mtch = regx.Match((string)searchValue);
                #endregion

                #region Server Side Validation
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
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                RemitanceDiagnosticsDetailsReqModel model = new RemitanceDiagnosticsDetailsReqModel
                {
                    Datetime_FromDate = frmDate,
                    Datetime_ToDate = toDate,
                    IsActive = isActiveId,
                    DROOfficeID = Convert.ToInt32(DROOfficeListID),
                    SROOfficeID = Convert.ToInt32(SROOfficeListID),
                };


                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                RemittanceDiagnosticsDetialsWrapperModel WrapperModel = new RemittanceDiagnosticsDetialsWrapperModel();
                WrapperModel.StartLength = startLen;
                WrapperModel.TotalNum = totalNum;
                WrapperModel.Datetime_FromDate = model.Datetime_FromDate;
                WrapperModel.Datetime_ToDate = model.Datetime_ToDate;
                WrapperModel.IsActive = model.IsActive;
                WrapperModel.DROOfficeID = model.DROOfficeID;
                WrapperModel.SROOfficeID = model.SROOfficeID;
                WrapperModel.DeptRefNo = DeptRefNo;
                IEnumerable<ChallanDetailsResponseModel> result = caller.PostCall<RemittanceDiagnosticsDetialsWrapperModel, List<ChallanDetailsResponseModel>>("GetChallanDetailsList", WrapperModel);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }
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
                        result = result.Where(m => m.DepartmentRefNumber.ToLower().Contains(searchValue.ToLower())
                                         || m.BatchID.ToLower().Contains(searchValue.ToLower())
                                         || m.CardType.ToLower().Contains(searchValue.ToLower())
                                         || m.CCNumber.ToLower().Contains(searchValue.ToLower())
                                         || m.ChallanAmount.ToLower().Contains(searchValue.ToLower())
                                         || m.ChallanDate.ToLower().Contains(searchValue.ToLower())
                                         || m.ChallanExpiryDate.ToLower().Contains(searchValue.ToLower())
                                         || m.ChallanID.ToLower().Contains(searchValue.ToLower())
                                         || m.ChallanRefNum.ToLower().Contains(searchValue.ToLower())
                                         || m.ChallanRequestID.ToLower().Contains(searchValue.ToLower())
                                         || m.ChallanTotalAmount.ToLower().Contains(searchValue.ToLower())
                                         || m.DepartmentRefNumber.ToLower().Contains(searchValue.ToLower())
                                         || m.InsertDateTime.ToLower().Contains(searchValue.ToLower())
                                         || m.InstrmntDate.ToLower().Contains(searchValue.ToLower())
                                         || m.InstrmntNumber.ToLower().Contains(searchValue.ToLower())
                                         || m.MICRCode.ToLower().Contains(searchValue.ToLower())
                                         || m.PaymentMode.ToLower().Contains(searchValue.ToLower())
                                         || m.RemitterName.ToLower().Contains(searchValue.ToLower())
                                         || m.RmtncAgencyBank.ToLower().Contains(searchValue.ToLower())
                                                                                                             );

                    }
                }


                //if (result == null)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


                //if (result.Count() == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = result.Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = result.Count(),
                    status = "1",
                    recordsFiltered = result.Count(),
                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }

        }

        /// <summary>
        /// Get Double Verification Details List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Double Verification Details List")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetDoubleVerificationDetailsList(FormCollection formCollection)
        {


            try
            {
                caller = new ServiceCaller("REMDaignosticsApiController");
                #region User Variables and Objects
                string fromDate = formCollection["fromDate"];
                string ToDate = formCollection["ToDate"];
                string DROOfficeListID = formCollection["DROOfficeListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string IsActiveId = formCollection["IsActiveId"];
                bool isActiveId = Convert.ToBoolean(IsActiveId);
                string DeptRefNo = formCollection["DepartmentReferenceNumber"];
                string ChallanRefNumber = formCollection["ChallanRefNumber"];
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                Regex regx = new Regex("[#$<>]");
                Match mtch = regx.Match((string)searchValue);
                #endregion

                #region Server Side Validation
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
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                RemitanceDiagnosticsDetailsReqModel model = new RemitanceDiagnosticsDetailsReqModel
                {
                    Datetime_FromDate = frmDate,
                    Datetime_ToDate = toDate,
                    IsActive = isActiveId,
                    DROOfficeID = Convert.ToInt32(DROOfficeListID),
                    SROOfficeID = Convert.ToInt32(SROOfficeListID),
                };



                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                RemittanceDiagnosticsDetialsWrapperModel WrapperModel = new RemittanceDiagnosticsDetialsWrapperModel();
                WrapperModel.StartLength = startLen;
                WrapperModel.TotalNum = totalNum;
                WrapperModel.Datetime_FromDate = model.Datetime_FromDate;
                WrapperModel.Datetime_ToDate = model.Datetime_ToDate;
                WrapperModel.IsActive = model.IsActive;
                WrapperModel.DROOfficeID = model.DROOfficeID;
                WrapperModel.SROOfficeID = model.SROOfficeID;
                WrapperModel.DeptRefNo = DeptRefNo;
                WrapperModel.ChallanRefNumber = ChallanRefNumber;
                IEnumerable<DoubleVerificationDetailsResponseModel> result = caller.PostCall<RemittanceDiagnosticsDetialsWrapperModel, List<DoubleVerificationDetailsResponseModel>>("GetDoubleVerificationDetailsList", WrapperModel);
                //Sorting

                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }



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
                        result = result.Where(m => m.BankName.ToLower().Contains(searchValue.ToLower()) || m.BankName.ToLower().Contains(searchValue.ToLower()) ||
                        m.BankTransactionNumber.ToLower().Contains(searchValue.ToLower()) || m.ChallanRefNumber.ToLower().Contains(searchValue.ToLower())
                        || m.ID.ToLower().Contains(searchValue.ToLower())
                        || m.InsertDateTime.ToLower().Contains(searchValue.ToLower())
                        || m.IPAdd.ToLower().Contains(searchValue.ToLower())
                        || m.PaidAmount.ToLower().Contains(searchValue.ToLower())
                        || m.PaymentMode.ToLower().Contains(searchValue.ToLower())
                        || m.PaymentStatusCode.ToLower().Contains(searchValue.ToLower())
                        || m.SchedulerID.ToLower().Contains(searchValue.ToLower())
                        || m.ServiceStatusCode.ToLower().Contains(searchValue.ToLower())
                        || m.ServiceStatusDesc.ToLower().Contains(searchValue.ToLower())
                        || m.TransactionID.ToLower().Contains(searchValue.ToLower())
                        || m.TransactionTimeStamp.ToLower().Contains(searchValue.ToLower())
                        || m.UserID.ToLower().Contains(searchValue.ToLower())
                        );
                    }
                }

                //if (result == null)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


                //if (result.Count() == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}


                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    data = result.Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = result.Count(),
                    status = "1",
                    recordsFiltered = result.Count(),
                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }

        }

        /// <summary>
        /// Get Bank Transaction Amount Details List
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Bank Transaction Amount Details List")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetBankTransactionAmountDetailsList(FormCollection formCollection)
        {


            try
            {
                caller = new ServiceCaller("REMDaignosticsApiController");
                #region User Variables and Objects
                string fromDate = formCollection["fromDate"];
                string ToDate = formCollection["ToDate"];
                string DROOfficeListID = formCollection["DROOfficeListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string IsActiveId = formCollection["IsActiveId"];
                bool isActiveId = Convert.ToBoolean(IsActiveId);
                int TransactionID = Convert.ToInt32(formCollection["TransactionID"]);
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                Regex regx = new Regex("[#$<>]");
                Match mtch = regx.Match((string)searchValue);
                #endregion

                #region Server Side Validation
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
                //  string EncryptedID = formCollection["EncryptedID"];
                TransactionID = Convert.ToInt32(formCollection["TransactionID"]);
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

                RemitanceDiagnosticsDetailsReqModel model = new RemitanceDiagnosticsDetailsReqModel
                {
                    Datetime_FromDate = frmDate,
                    Datetime_ToDate = toDate,
                    IsActive = isActiveId,
                    DROOfficeID = Convert.ToInt32(DROOfficeListID),
                    SROOfficeID = Convert.ToInt32(SROOfficeListID),

                };

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                //Paging Size (10,20,50,100)    
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                RemittanceDiagnosticsDetialsWrapperModel WrapperModel = new RemittanceDiagnosticsDetialsWrapperModel();
                WrapperModel.StartLength = startLen;
                WrapperModel.TotalNum = totalNum;
                WrapperModel.Datetime_FromDate = model.Datetime_FromDate;
                WrapperModel.Datetime_ToDate = model.Datetime_ToDate;
                WrapperModel.IsActive = model.IsActive;
                WrapperModel.DROOfficeID = model.DROOfficeID;
                WrapperModel.SROOfficeID = model.SROOfficeID;
                WrapperModel.TransactionID = TransactionID;

                //var result = caller.PostCall<MISReportDetialsWrapperModel, List<DoubleVerificationDetailsResponseModel>>("GetDoubleVerificationDetailsList", WrapperModel);
                IEnumerable<BankTransactionAmountDetailsResponseModel> result = caller.PostCall<RemittanceDiagnosticsDetialsWrapperModel, List<BankTransactionAmountDetailsResponseModel>>("GetBankTransactionAmountDetailsList", WrapperModel);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

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
                        result = result.Where(m => m.TransactionID.ToLower().Contains(searchValue.ToLower())
                        || m.Amount.ToLower().Contains(searchValue.ToLower())
                        || m.DeptSubPurpooseID.ToLower().Contains(searchValue.ToLower())
                        || m.FeesRuleCode.ToLower().Contains(searchValue.ToLower())
                        || m.ID.ToLower().Contains(searchValue.ToLower())
                        || m.InsertDateTime.ToLower().Contains(searchValue.ToLower())
                        || m.SROCode.ToLower().Contains(searchValue.ToLower())
                        || m.TransactionID.ToLower().Contains(searchValue.ToLower())

                        );

                    }
                }

                //if (result == null)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                //if (result.Count() == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}



                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    //data = result.ToArray(),
                    data = result.Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = result.Count(),
                    status = "1",
                    recordsFiltered = result.Count(),
                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }

        }

        /// <summary>
        /// Get Challan Matrix Transaction Details
        /// </summary>
        /// <param name="formCollection"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Get Challan Matrix Transaction Details")]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult GetChallanMatrixTransactionDetails(FormCollection formCollection)
        {


            try
            {
                caller = new ServiceCaller("REMDaignosticsApiController");
                #region User Variables and Objects
                string fromDate = formCollection["fromDate"];
                string ToDate = formCollection["ToDate"];
                string DROOfficeListID = formCollection["DROOfficeListID"];
                string SROOfficeListID = formCollection["SROOfficeListID"];
                string IsActiveId = formCollection["IsActiveId"];
                bool isActiveId = Convert.ToBoolean(IsActiveId);
                string DeptRefNo = formCollection["DepartmentReferenceNumber"];
                string IsDro = formCollection["IsDro"];
                int isDro = Convert.ToInt32(IsDro);
                CommonFunctions objCommon = new CommonFunctions();
                String errorMessage = String.Empty;
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

                Regex regx = new Regex("[#$<>]");
                Match mtch = regx.Match((string)searchValue);
                #endregion

                #region Server Side Validation
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
                        errorMessage = "From Date can not be larger than To Date"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                #endregion

                RemitanceDiagnosticsDetailsReqModel model = new RemitanceDiagnosticsDetailsReqModel
                {
                    Datetime_FromDate = frmDate,
                    Datetime_ToDate = toDate,
                    IsActive = isActiveId,
                    DROOfficeID = Convert.ToInt32(DROOfficeListID),
                    SROOfficeID = Convert.ToInt32(SROOfficeListID),
                };

                int startLen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);
                int pageSize = totalNum; //totalNum != null ? Convert.ToInt32(totalNum) : 0;
                int skip = startLen;// startLen != null ? Convert.ToInt32(startLen) : 0;

                RemittanceDiagnosticsDetialsWrapperModel WrapperModel = new RemittanceDiagnosticsDetialsWrapperModel();
                WrapperModel.StartLength = startLen;
                WrapperModel.TotalNum = totalNum;
                WrapperModel.Datetime_FromDate = model.Datetime_FromDate;
                WrapperModel.Datetime_ToDate = model.Datetime_ToDate;
                WrapperModel.IsActive = model.IsActive;
                WrapperModel.DROOfficeID = model.DROOfficeID;
                WrapperModel.SROOfficeID = model.SROOfficeID;
                WrapperModel.DeptRefNo = DeptRefNo;
                WrapperModel.IsDro =isDro;
                IEnumerable<ChallanMatrixTransactionDetailsResponseModel> result = caller.PostCall<RemittanceDiagnosticsDetialsWrapperModel, List<ChallanMatrixTransactionDetailsResponseModel>>("GetChallanMatrixTransactionDetailsList", WrapperModel);

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

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
                        result = result.Where(m => m.ChallanReqID.ToLower().Contains(searchValue.ToLower())
                                 || m.BatchID.ToLower().Contains(searchValue.ToLower())
                                 || m.ChallanReqID.ToLower().Contains(searchValue.ToLower())
                                 || m.DDOCode.ToLower().Contains(searchValue.ToLower())
                                 || m.DroCode.ToLower().Contains(searchValue.ToLower())
                                 || m.FirstPrintDate.ToLower().Contains(searchValue.ToLower())
                                 || m.InsertDateTime.ToLower().Contains(searchValue.ToLower())
                                 || m.IPAddress.ToLower().Contains(searchValue.ToLower())
                                 || m.IsDro.ToLower().Contains(searchValue.ToLower())
                                 || m.ReceiptDate.ToLower().Contains(searchValue.ToLower())
                                 || m.RemittanceBankName.ToLower().Contains(searchValue.ToLower())
                                 || m.ReqPaymentMode.ToLower().Contains(searchValue.ToLower())
                                 || m.SchedulerID.ToLower().Contains(searchValue.ToLower())
                                 || m.SroCode.ToLower().Contains(searchValue.ToLower())
                                 || m.StatusCode.ToLower().Contains(searchValue.ToLower())
                                 || m.StatusDesc.ToLower().Contains(searchValue.ToLower())
                                 || m.TransactionDateTime.ToLower().Contains(searchValue.ToLower())
                                 || m.UIRNumber.ToLower().Contains(searchValue.ToLower())
                                 || m.UserID.ToLower().Contains(searchValue.ToLower())
                        );
                    }
                }

                //if (result == null)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                //if (result.Count() == 0)
                //{
                //    var emptyData = Json(new
                //    {
                //        draw = formCollection["draw"],
                //        recordsTotal = 0,
                //        recordsFiltered = 0,
                //        data = "",
                //        status = false,
                //        errorMessage = "No results Found For the Current Input! Please try again"
                //    });
                //    emptyData.MaxJsonLength = Int32.MaxValue;
                //    return emptyData;
                //}

                var JsonData = Json(new
                {
                    draw = formCollection["draw"],
                    //data = result.ToArray(),
                    data = result.Skip(skip).Take(pageSize).ToList(),
                    recordsTotal = result.Count(),
                    status = "1",
                    recordsFiltered = result.Count(),
                });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                emptyData.MaxJsonLength = Int32.MaxValue;
                return emptyData;
            }

        }

    }
}