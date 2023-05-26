#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IntegrationCallExceptionsController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.IntegrationCallExceptions;
using ECDataUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ECDataUI.Session;
using ECDataUI.Filters;
using System.Text.RegularExpressions;

namespace ECDataUI.Areas.Remittance.Controllers
{
    [KaveriAuthorizationAttribute]
    public class IntegrationCallExceptionsController : Controller
    {
        #region Properties
        private ServiceCaller caller = new ServiceCaller("IntegrationCallExceptionsApiController");
        #endregion


        #region Method
        /// <summary>
        /// Intergartion Call EX View
        /// </summary>
        /// <returns>returns view</returns>
        [HttpGet]
        [EventAuditLogFilter(Description = "Intergartion Call EX View")]
        public ActionResult IntergartionCallEXView()
        {
            try
            {
                // Added BY Shubham Bhagat on 25-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.IntegrationCallExceptions;

                IntegrationCallExceptionsModel model = new IntegrationCallExceptionsModel();
                model.OfficeTypeList = new List<SelectListItem>();
                model.OfficeTypeID = 0;
                SelectListItem item = new SelectListItem();
                item.Text = "All";
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
        /// <returns>returns Office List List</returns>
        [HttpGet]
        public ActionResult GetOfficeList(String OfficeType)
        {
            IntegrationCallExceptionsModel model = new IntegrationCallExceptionsModel();

            try
            {
                model = caller.GetCall<IntegrationCallExceptionsModel>("GetOfficeList", new { OfficeType });
                //By Shubham bhagat on 26-04-2019
                //JSON with JsonRequestBehavior.AllowGet is working fine when KaveriAuthorizationAttribute is not added. 
                return Json(new { OfficeList = model.OfficeTypeList }, JsonRequestBehavior.AllowGet);
                //return Json(new { OfficeList = model.OfficeTypeList });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);

                return Json(new { OfficeList = model.OfficeTypeList }, JsonRequestBehavior.AllowGet);

            }
        }

        //[HttpGet]         GetExceptionsDetails
        /// <summary>
        /// Get Exceptions Details
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <param name="OfficeTypeID"></param>
        /// <returns>returns Exceptions Details List</returns>
        [EventAuditLogFilter(Description = "Get Exceptions Details")]
        [HttpPost]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult GetExceptionsDetails(String OfficeType, String OfficeTypeID)
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

                #region Validation added by shubham bhagat on 13-05-2019
                Regex regx = new Regex("[#$<>]");

                if (!string.IsNullOrEmpty(searchValue))
                {
                    Match mtch = regx.Match((string)searchValue);
                    if (mtch.Success)
                    {
                        var emptyData = Json(new
                        {
                            draw = draw,
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
                #endregion

                IEnumerable<IntegrationCallExceptionsModel> ExceptionDetailsList = caller.GetCall<List<IntegrationCallExceptionsModel>>("GetExceptionsDetails", new { OfficeType = OfficeType, OfficeTypeID = OfficeTypeID });
                
                // ADDED BY SHUBHAM BHAGAT ON 15-05-2019              
                if (ExceptionDetailsList == null)
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
                    ExceptionDetailsList = ExceptionDetailsList.Where(m => m.ExceptionType.ToLower().Contains(searchValue.ToLower()) ||
                    m.ExceptionMethodName.ToLower().Contains(searchValue.ToLower()) ||
                    m.LogDate.ToLower().Contains(searchValue.ToLower())).ToList();
                }

                //total number of rows count     
                recordsTotal = ExceptionDetailsList.Count();
                //Paging     
                var data = ExceptionDetailsList.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data   
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });

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

        #endregion
    }
}