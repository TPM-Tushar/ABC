#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ActionDetailsController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for User Management module.
*/
#endregion

using CustomModels.Models.ControllerAction;
using ECDataUI.Common;
using static ECDataUI.Common.CommonFunctions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ECDataUI.Filters;
using System.Web.Routing;
using System.Reflection;
using ECDataUI.Session;
using CustomModels.Models.UserManagement;

namespace ECDataUI.Areas.UserManagement.Controllers
{
      [KaveriAuthorizationAttribute]
    public class ActionDetailsController : Controller
    {

        string errorMessage = String.Empty;
        ServiceCaller caller = null;

        /// <summary>
        /// Show Controller Action Data
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowControllerActionData()
        {
            try
            {
                // Added BY SB on 8-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.ControllerActionDetails;

                // added by m rafe on 28-11-19

                //call web api here
                //ControllerActionWrapperModel

                caller = new ServiceCaller("ActionDetailsApiController");
                ControllerActionViewModel responseModel = new ControllerActionViewModel();
                ControllerActionViewModel reqModel = new ControllerActionViewModel();
                reqModel.CurrentRoleID = KaveriSession.Current.RoleID;
                responseModel = caller.PostCall<ControllerActionViewModel, ControllerActionViewModel>("ShowControllerActionData", reqModel, out errorMessage);


                return View(responseModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Controller Action Details View.", URLToRedirect = "/Home/HomePage" });
            }
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Insert Controller Action Data
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertControllerActionData()
        {
            ControllerActionDataModel DataModel = new ControllerActionDataModel();
            DataModel.AreaList = GetAllAreas().ToList();
            DataModel.EncryptedId = string.Empty;
            ControllerActionModel modelViewForCreate = new ControllerActionModel();
            caller = new ServiceCaller("ActionDetailsApiController");

            modelViewForCreate = caller.PostCall<ControllerActionDataModel, ControllerActionModel>("GetControllerActionModel", DataModel, out errorMessage);
            modelViewForCreate.IsForUpdate = false;
            return View("InsertControllerActionData", modelViewForCreate);
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Insert Controller Action Data
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "insert new Controller ActionData")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult InsertControllerActionData(ControllerActionModel viewModel)
        {
            //if (viewModel.RoleId != null)
            //{
            if (ModelState.IsValid)
            {

                caller = new ServiceCaller("ActionDetailsApiController");
                // For Activity Log by Shubham Bhagat
                viewModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;

                ControllerActionModel response = caller.PostCall<ControllerActionModel, ControllerActionModel>("InsertControllerActionData", viewModel, out errorMessage);



                if (response != null)
                {
                    return Json(new { success = true, message = "Successfully Inserted" });

                }
                else
                {

                    return Json(new { success = false, message = "Insertion Failed" });

                }

            }
            else
            {

                    string errorMsg = ModelState.FormatErrorMessageInString();


                return Json(new { success = false, message = errorMsg });


            }

            //}
            //else
            //{

            //    return Json(new { success = false, message = "Please Select Role", role = true });

            //}
        }
        #endregion

        /// <summary>
        /// Update Controller Action Data
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public ActionResult UpdateControllerActionData(String EncryptedID)
        {
            try
            {
                ControllerActionDataModel DataModel = new ControllerActionDataModel();
                DataModel.AreaList = GetAllAreas().Where(x => x.Name != "NoArea").ToList();
                DataModel.EncryptedId = EncryptedID;
                ControllerActionModel updationModel = new ControllerActionModel();
                caller = new ServiceCaller("ActionDetailsApiController");
                errorMessage = String.Empty;
                ControllerActionModel ControlViewModel = caller.PostCall<ControllerActionDataModel, ControllerActionModel>("GetControllerActionModel", DataModel, out errorMessage);
                ControlViewModel.IsForUpdate = true;
                return PartialView("InsertControllerActionData", ControlViewModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Controller Action Details Update View.", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// Update Controller Action Data
        /// </summary>
        /// <param name="updationModel"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Update Controller Action Data")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateControllerActionData(ControllerActionModel updationModel)
        {
            try
            {
                // Added By Shubham Bhagat on 28-12-2018
                ModelState.Remove("ActionNameId");
                ModelState.Remove("ControllerNameId");
                if (ModelState.IsValid)
                {

                    caller = new ServiceCaller("ActionDetailsApiController");
                    errorMessage = String.Empty;
                    // For Activity Log by Shubham Bhagat
                    updationModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                    CommonFunctions commonOBJ = new CommonFunctions();
                    updationModel.UserIPAddress = commonOBJ.GetIPAddress();
                    // Added By Shubham Bhagat on 18-12-2018
                    updationModel.ControllerNameId = updationModel.ControllerNameId_Hidden;
                    updationModel.ActionNameId = updationModel.ActionNameId_Hidden;

                    ControllerActionModel response = caller.PostCall<ControllerActionModel, ControllerActionModel>("UpdateControllerActionData", updationModel, out errorMessage);
                    if (response != null)
                    {
                        return Json(new { success = true, message = "Controller action details updated successfully" });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Controller action details not updated" });
                    }
                }
                else
                {
                    string errorMsg = ModelState.FormatErrorMessageInString();
                    return Json(new { success = false, message = errorMsg });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = false, message = "Error occured while updating controller action details." });
            }

        }

        /// <summary>
        /// Load Controller Action Data
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadControllerActionData()
        {
            int isTechAdmin = (KaveriSession.Current.RoleID == Convert.ToInt16(CommonEnum.RoleDetails.TechnicalAdmin)) ? 1 : 0;
            try
            {


                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();
                int filterMenuId = Convert.ToInt32(Request.Form.GetValues("filterMenuId").FirstOrDefault());


                if (KaveriSession.Current.RoleID != Convert.ToInt16(CommonEnum.RoleDetails.TechnicalAdmin))
                {
                    if (filterMenuId == 0)
                        return Json(new { isTechAdmin = isTechAdmin, serverError = false, errorMessage = "Please select menu." });

                }

                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 5;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                // Getting all Customer data    
                //  string EncryptedID = "ABC";
                IEnumerable<ControllerActionModel> CAIDList = new List<ControllerActionModel>();
                caller = new ServiceCaller("ActionDetailsApiController");
                //CAIDList = caller.GetCall<List<ControllerActionModel>>("GetControllerActionDetails");

                // changed by m rafe on 28--11-19
                ControllerActionViewModel viewModel = new ControllerActionViewModel();
                viewModel.filterMenuDetailsId = filterMenuId;

                viewModel.CurrentRoleID = KaveriSession.Current.RoleID;

                CAIDList = caller.PostCall<ControllerActionViewModel, List<ControllerActionModel>>("GetControllerActionDetails", viewModel);
                if (CAIDList == null)
                {
                    return Json(new { isTechAdmin = isTechAdmin, serverError = false, errorMessage = "Error occured while getting controller action details." });
                }
                CAIDList = CAIDList.Where(x => x.AreaNameId != "").ToList();
                CAIDList = CAIDList.Where(x => x.AreaNameId != "NoArea").ToList();
                //Sorting    
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    CAIDList = CAIDList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    CAIDList = CAIDList.Where(m => m.ActionNameId.ToLower().Contains(searchValue.ToLower())
                    || m.ControllerNameId.ToLower().Contains(searchValue.ToLower())
                    || m.AreaNameId.ToLower().Contains(searchValue.ToLower())
                    || m.Description.ToLower().Contains(searchValue.ToLower())


                    );
                }

                //total number of rows count  
                recordsTotal = 0;
                var data = new List<ControllerActionModel>();
                if (CAIDList != null)
                {
                    recordsTotal = CAIDList.Count();
                    //Paging     
                    data = CAIDList.Skip(skip).Take(pageSize).ToList();
                }
                //Returning Json Data    

                return Json(new { isTechAdmin = isTechAdmin, draw, recordsFiltered = recordsTotal, recordsTotal, data });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { isTechAdmin = isTechAdmin, serverError = false, errorMessage = "Error occured while getting controller action details." });
            }

        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Delete Controller Action Data
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Delete Controller Action ")]
        public ActionResult DeleteControllerActionData(String EncryptedID)
        {
            caller = new ServiceCaller("ActionDetailsApiController");
            errorMessage = String.Empty;
            ControllerActionModel modelForDelete = new ControllerActionModel();
            modelForDelete.EncryptedID = EncryptedID;
            Boolean response = caller.PostCall<ControllerActionModel, Boolean>("DeleteControllerActionData", modelForDelete, out errorMessage);
            //Boolean response = caller.GetCall<Boolean>("DeleteControlData", new { CAID=caid}, out errorMessage);
            if (response)
            {
                return Json(new { success = true, message = "Successfully Deleted" });

            }
            else
            {

                return Json(new { success = false, message = "Deletion Failed" });

            }
        }
        #endregion

        /// <summary>
        /// Get Controller List
        /// </summary>
        /// <param name="AreaName"></param>
        /// <returns></returns>
        public ActionResult GetControllerList(string AreaName)
        {
            try
            {
                ControllerActionDataModel DataModel = new ControllerActionDataModel();
                DataModel.AreaList = GetAllAreas().ToList();
                DataModel.AreaName = AreaName;
                ControllerActionModel modelViewForCreate = new ControllerActionModel();
                caller = new ServiceCaller("ActionDetailsApiController");

                modelViewForCreate = caller.PostCall<ControllerActionDataModel, ControllerActionModel>("GetControllerActionLists", DataModel, out errorMessage);
                if (modelViewForCreate.ControllerList != null)
                {
                    return Json(new { success = true, modelViewForCreate.ControllerList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, response = "Invalid Area." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = false, response = "Error occured while getting Controller List." }, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Get Action List
        /// </summary>
        /// <param name="AreaName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public ActionResult GetActionList(string AreaName, string controllerName)
        {
            try
            {
                ControllerActionDataModel DataModel = new ControllerActionDataModel();
                DataModel.AreaList = GetAllAreas().ToList();
                DataModel.AreaName = AreaName;
                DataModel.ControllerName = controllerName;
                ControllerActionModel modelViewForCreate = new ControllerActionModel();
                caller = new ServiceCaller("ActionDetailsApiController");

                modelViewForCreate = caller.PostCall<ControllerActionDataModel, ControllerActionModel>("GetControllerActionLists", DataModel, out errorMessage);
                if (modelViewForCreate.ActionList != null)
                {
                    return Json(new { success = true, modelViewForCreate.ActionList }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, response = "Invalid Controller." }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = false, response = "Error occured while getting Action List." }, JsonRequestBehavior.AllowGet);
            }
        }


        public ActionResult GetRoleAuthView(String EncryptedID)

        {
            ControllerActionModel modelViewForCreate = new ControllerActionModel();
            caller = new ServiceCaller("ActionDetailsApiController");

            modelViewForCreate = caller.GetCall<ControllerActionModel>("GetRoleAuthView", new { EncryptedID = EncryptedID }, out errorMessage);

            return View("RoleAuthView", modelViewForCreate);
        }

        public ActionResult UpdateRoleActionAuth(ControllerActionModel model)

        {
            try
            {
                // add val here

                caller = new ServiceCaller("ActionDetailsApiController");
                errorMessage = String.Empty;
                // For Activity Log by Shubham Bhagat
                model.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;

                CommonFunctions commonOBJ = new CommonFunctions();
                model.UserIPAddress = commonOBJ.GetIPAddress();
                ControllerActionModel response = caller.PostCall<ControllerActionModel, ControllerActionModel>("UpdateRoleActionAuth", model, out errorMessage);
                if (response != null)
                {
                    return Json(new { success = true, message = "Controller action details updated successfully" });
                }
                else
                {
                    return Json(new { success = false, message = "Controller action details not updated" });
                }

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = false, message = "Error occured while updating controller action details." });
            }
        }



    }
}
