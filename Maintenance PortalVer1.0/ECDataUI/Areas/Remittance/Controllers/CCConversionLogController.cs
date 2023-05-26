#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   CCConversionLogController.cs
    * Author Name       :   Madhusoodan Bisen
    * Creation Date     :   15-09-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for CC Converion Log Reports under General Diagnostic module.
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CustomModels.Models.Remittance.CCConversionLog;
using ECDataUI.Common;
using ECDataUI.Filters;
using System.Linq.Dynamic;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class CCConversionLogController : Controller
    {
        
        private ServiceCaller caller = new ServiceCaller("CCConversionLogAPIController");

        /// <summary>
        /// CC Conversion Log View
        /// </summary>
        /// <returns>Returns View</returns>
        [EventAuditLogFilter(Description = "CC Conversion Log View")]
        [HttpGet]
        [MenuHighlight]
        public ActionResult CCConversionLogView()
        {
            try
            {
                CCConversionLogWrapperModel responseModel = caller.GetCall<CCConversionLogWrapperModel>("CCConversionLogView");

                return View(responseModel);
            }
            catch(Exception ex)
            {
                ExceptionLogs.LogException(ex);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get CC Conversion Logs Between Two Dates
        /// </summary>
        /// <param name="FromDate"></param>
        /// <param name="ToDate"></param> 
        /// <returns>returns CC Conversion Logs List</returns>
        [HttpPost]
        public ActionResult CCConversionLogDetails(FormCollection formCollection)
        {
            try
            {
                int startlen = Convert.ToInt32(formCollection["start"]);
                int totalNum = Convert.ToInt32(formCollection["length"]);

                //Paging Size (10,20,50,100)
                int pageSize = totalNum;
                int skip = startlen;

                //For Sorting and Searching
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                #region Server Side Validation

                var temp = formCollection["DocTypeID"];
                
                if (formCollection["FromDate"] == null || formCollection["FromDate"] == "")
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date Required."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (formCollection["ToDate"] == null || formCollection["ToDate"] == "")
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "To Date Required."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (formCollection["DocTypeID"] == null || Convert.ToInt32(formCollection["DocTypeID"]) == 0)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "Please select any of the Registration Types."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                #endregion

                int sRecordsFiltered = 0;
                DateTime fromDate = Convert.ToDateTime(formCollection["FromDate"]);
                DateTime toDate = Convert.ToDateTime(formCollection["ToDate"]);

                int DocTypeId = Convert.ToInt32(formCollection["DocTypeID"]);

                //Added by Madhusoodan on 29-09-2020 to validate From Date should not exceed To Date
                if (fromDate > toDate)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "From Date should not be greater than To Date."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //Added by Madhusoodan on 21-09-2020
                bool distinctLogs = Convert.ToBoolean(formCollection["DistinctLogs"]);

                CCConversionLogReqModel reqModel = new CCConversionLogReqModel();

                reqModel.FromDate = fromDate;
                reqModel.ToDate = toDate;
                reqModel.startLen = startlen;
                reqModel.totalNum = totalNum;
                reqModel.DocumentTypeID = DocTypeId;
                reqModel.DistinctLogs = distinctLogs;

                int totalCount = caller.PostCall<CCConversionLogReqModel, int>("GetCCConversionLogDetailsTotalCount", reqModel);

                if (searchValue != null && searchValue != "")
                {
                    reqModel.startLen = 0;
                    reqModel.totalNum = totalCount;
                }

                CCConversionLogWrapperModel responseModel = caller.PostCall<CCConversionLogReqModel, CCConversionLogWrapperModel>("CCConversionLogDetails", reqModel);

                if (responseModel == null)
                {
                    var emptyData = Json(new
                    {
                        draw = formCollection["draw"],
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No data found."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                IEnumerable<CCConversionLogDetails> result = responseModel.CCConversionLogDetailsList;
                
                if (result == null)
                {
                    return Json(new { serverError = true, success = false, errorMessage = "Error occured while getting CC Conversion Logs." });
                }

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    result = result.OrderBy(sortColumn + " " + sortColumnDir);
                }

                //Searching    
              if (!string.IsNullOrEmpty(searchValue))
                {
                    result = result.Where(m =>
                    m.LogDateTime.ToString().ToLower().Contains(searchValue.ToLower())
                    || m.SROCode.ToString().Contains(searchValue)
                    || m.UserName.ToLower().Contains(searchValue.ToLower())
                    || m.DocumentID.ToString().Contains(searchValue)
                    || m.FinalRegistrationNumber.ToLower().Contains(searchValue.ToLower())
                    || m.UserID.ToString().Contains(searchValue)
                    || m.LogID.ToString().Contains(searchValue)
                    //Added by Madhusoodan on 07/10/2020 to add below column
                    || m.IsConvertedUsingImgMagick.ToLower().Contains(searchValue.ToLower())
                    //Added by Madhusoodan on 12/10/2020 to add below column
                    || m.CCID.ToString().Contains(searchValue)
                    ).ToList();
                    sRecordsFiltered = result.Count();
                }

                var gridData = result.Select(CCConversionLogDetails => new
                {
                    SrNo = CCConversionLogDetails.SrNo,
                    LogID = CCConversionLogDetails.LogID,
                    UserID = CCConversionLogDetails.UserID,
                    UserName = CCConversionLogDetails.UserName,
                    SROCode = CCConversionLogDetails.SROCode,
                    DocumentID = CCConversionLogDetails.DocumentID,
                    FinalRegistrationNumber = CCConversionLogDetails.FinalRegistrationNumber,
                    LogDateTime = CCConversionLogDetails.LogDateTime,
                    //Added by Madhusoodan on 07/10/2020 to add below column
                    IsConvertedUsingImgMagick = CCConversionLogDetails.IsConvertedUsingImgMagick,
                    //Added by Madhusoodan on 12/10/2020 to add below column
                    CCID = CCConversionLogDetails.CCID
                });

                if (searchValue != null && searchValue != "")
                {
                    var JsonData = Json(new
                    {
                        draw = formCollection["draw"],
                        data = gridData.ToArray().Skip(skip).Take(pageSize).ToList(),
                        recordsTotal = totalCount,
                        //status = "1",
                        recordsFiltered = sRecordsFiltered,
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
                    });
                    JsonData.MaxJsonLength = Int32.MaxValue;
                    return JsonData;
                }

            }
            catch (Exception ex)
            {
                ExceptionLogs.LogException(ex);

                var emptyData = Json(new
                {
                    draw = Request.Form.GetValues("draw").FirstOrDefault(),
                    recordsTotal = 0,
                    recordsFiltered = 0,
                    data = "",
                    status = false,
                    errorMessage = "Error occured while processing your request."
                });
                return emptyData;
            }
        }
    }

}