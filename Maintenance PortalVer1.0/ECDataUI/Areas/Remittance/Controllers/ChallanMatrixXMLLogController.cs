#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ChallanMatrixXMLLogController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.ChallanMatrixXMLLog;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class ChallanMatrixXMLLogController : Controller
    {
        #region PROPERTIES
        private ServiceCaller caller = new ServiceCaller("ChallanMatrixXMLLogApiController");
        #endregion

        #region METHOD
        /// <summary>
        /// Challan Matrix XML Log View
        /// </summary>
        /// <returns>returns view</returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Challan Matrix XML Log View")]
        public ActionResult ChallanMatrixXMLLogView()
        {
            try
            {
                // Added BY Shubham Bhagat on 16-05-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ChallanMatrixXMLLog;

                ChallanMatrixWrapperModel model = new ChallanMatrixWrapperModel();
                model.OfficeTypeList = new List<SelectListItem>();
                model.OfficeTypeID = 0;
                SelectListItem item = new SelectListItem();
                item.Text = "Select";
                item.Value = "0";
                model.OfficeTypeList.Add(item);
                return View(model);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while processing your request.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Get Office List
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns>returns Office List</returns>
        [HttpGet]
        public ActionResult GetOfficeList(String OfficeType)
        {
            ChallanMatrixWrapperModel model = new ChallanMatrixWrapperModel();

            try
            {
                #region Server Side Validation
                // ADDED BY SHUBHAM BHAGAT ON 21-05-2019 AT 6:36 PM
                if (String.IsNullOrEmpty(OfficeType))
                {
                    return Json(new { errorMessage = "Please select office type." }, JsonRequestBehavior.AllowGet);
                }
                #endregion

                model = caller.GetCall<ChallanMatrixWrapperModel>("GetOfficeList", new { OfficeType });
                //By Shubham bhagat on 26-04-2019
                //JSON with JsonRequestBehavior.AllowGet is working fine when KaveriAuthorizationAttribute is not added. 
                return Json(new { OfficeList = model.OfficeTypeList }, JsonRequestBehavior.AllowGet);
                //return Json(new { OfficeList = model.OfficeTypeList });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { errorMessage = "Error occured while processing your request." }, JsonRequestBehavior.AllowGet);

            }
        }

        /// <summary>
        /// Challan Matrix Details
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <param name="OfficeTypeID"></param>
        /// <param name="fromDate"></param>
        /// <param name="ToDate"></param>
        /// <param name="RequestTxt"></param>
        /// <param name="ResponseTxt"></param>
        /// <returns>returns Challan Matrix Details list</returns>
        [EventAuditLogFilter(Description = "Challan Matrix Details")]
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]

        public ActionResult ChallanMatrixDetails(String OfficeType, String OfficeTypeID, String fromDate, String ToDate, String RequestTxt, String ResponseTxt)
        {
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();// FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();//
                var length = Request.Form.GetValues("length").FirstOrDefault();//
                //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                #region Server Side Validation
                if (String.IsNullOrEmpty(OfficeType))
                {
                    var emptyData = Json(new
                    {
                        draw = draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please select office type."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                if (String.IsNullOrEmpty(OfficeTypeID))
                {
                    var emptyData = Json(new
                    {
                        draw = draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please select office."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }
                int OfficeTypeIDINT = Convert.ToInt32(OfficeTypeID);
                if (OfficeTypeIDINT == 0 || OfficeTypeIDINT < 0)
                {
                    var emptyData = Json(new
                    {
                        draw = draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "Please select office."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                if (string.IsNullOrEmpty(fromDate))
                {
                    var emptyData = Json(new
                    {
                        draw = draw,
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
                        draw = draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = "0",
                        errorMessage = "To Date required"
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                Regex regx = new Regex("[#$<>]");
                Match requestMatch = regx.Match((string)RequestTxt);
                Match responseMatch = regx.Match((string)ResponseTxt);

                if (!string.IsNullOrEmpty(RequestTxt))
                {
                    if (requestMatch.Success)
                    {
                        var emptyData = Json(new
                        {
                            draw = draw,
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please enter valid Request text."
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }

                if (!string.IsNullOrEmpty(ResponseTxt))
                {
                    if (responseMatch.Success)
                    {
                        var emptyData = Json(new
                        {
                            draw = draw,
                            recordsTotal = 0,
                            recordsFiltered = 0,
                            data = "",
                            status = false,
                            errorMessage = "Please enter valid Response text."
                        });
                        emptyData.MaxJsonLength = Int32.MaxValue;
                        return emptyData;
                    }
                }
                #endregion

                // ADDED BY SHUBHAM BHAGAT ON 17-05-2019            
                DateTime frmDate, toDate;
                bool boolFrmDate = DateTime.TryParse(DateTime.ParseExact(fromDate, "dd/MM/yyyy", null).ToString(), out frmDate);
                bool boolToDate = DateTime.TryParse(DateTime.ParseExact(ToDate, "dd/MM/yyyy", null).ToString(), out toDate);


                ChallanMatrixLogRequestModel reqModel = new ChallanMatrixLogRequestModel();
                reqModel.OfficeType = OfficeType;
                reqModel.OfficeTypeID = OfficeTypeID;
                reqModel.SROCode = String.IsNullOrEmpty(OfficeTypeID) ? 0 : Convert.ToInt32(OfficeTypeID);
                reqModel.Datetime_FromDate = frmDate;
                reqModel.Datetime_ToDate = toDate;
                reqModel.Request = String.IsNullOrEmpty(RequestTxt) ? "0" : RequestTxt;
                reqModel.Response = String.IsNullOrEmpty(ResponseTxt) ? "0" : ResponseTxt;
                if (OfficeType.ToLower().Equals("dro"))
                {
                    reqModel.IsDRO = true;
                }
                ChallanMatrixWrapperModel responseModel = caller.PostCall<ChallanMatrixLogRequestModel, ChallanMatrixWrapperModel>("ChallanMatrixDetails", reqModel);

                // ADDED BY SHUBHAM BHAGAT ON 15-05-2019              
                if (responseModel == null)
                {
                    var emptyData = Json(new
                    {
                        draw = draw,
                        recordsTotal = 0,
                        recordsFiltered = 0,
                        data = "",
                        status = false,
                        errorMessage = "No data found."
                    });
                    emptyData.MaxJsonLength = Int32.MaxValue;
                    return emptyData;
                }

                //Sorting working but commented
                //if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                //{
                //    ExceptionDetailsList = ExceptionDetailsList.OrderBy(sortColumn + " " + sortColumnDir);
                //}

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    responseModel.ChallanMatrixXMLLogDetailList = responseModel.ChallanMatrixXMLLogDetailList.Where(m =>
                        m.RequestXMLID.ToString().Contains(searchValue)
                        || m.SROCode.ToString().Contains(searchValue)
                        || m.RequestDateTime.ToString().Contains(searchValue)
                        || m.ResponseDateTime.ToString().Contains(searchValue)
                        || m.RequestExceptionDetails.ToLower().Contains(searchValue.ToLower())
                        || m.ResponseExceptionDetails.ToLower().Contains(searchValue.ToLower())
                        //|| m.DROCode.ToLower().Contains(searchValue.ToLower())
                        ).ToList();
                }

                //total number of rows count     
                recordsTotal = responseModel.ChallanMatrixXMLLogDetailList.Count();
                //Paging     
                var data = responseModel.ChallanMatrixXMLLogDetailList.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data                  
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });
                //return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data, errorMessage = "No records found." });
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
        /// Download Challan Matrix XML
        /// </summary>
        /// <param name="RequestXMLID"></param>
        /// <returns>returns Challan Matrix XML zip file </returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Download Challan Matrix XML")]
        public ActionResult DownloadChallanMatrixXML(String RequestXMLID)
        {
            try
            {

                FileDownloadModel model = new FileDownloadModel();
                ChallanMatrixLogRequestModel reqModel = new ChallanMatrixLogRequestModel();
                reqModel.RequestXMLID = RequestXMLID;//String.IsNullOrEmpty(RequestXMLID)
                model = caller.PostCall<ChallanMatrixLogRequestModel, FileDownloadModel>("DownloadChallanMatrixXML", reqModel);

                HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + RequestXMLID + "_ChallanMatXML_" + DateTime.Now + ".zip");
                return File(model.FileContentField, "application/zip");
            }
            catch (Exception e)
            {

                ExceptionLogs.LogException(e);

                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while downloading file.", URLToRedirect = "/Home/HomePage" });
            }
        }

        #endregion
    }

}