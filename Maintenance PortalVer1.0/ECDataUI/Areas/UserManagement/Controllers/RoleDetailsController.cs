#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   RoleDetailsController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for User Management module.
*/
#endregion

#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   RoleDetailsController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   4-10-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller to Consume API controller Methods
*/
#endregion


#region References
using CustomModels.Models.UserManagement;
using ECDataUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using ECDataUI.Filters;
using ECDataUI.Session;
using CustomModels.Security;
#endregion

namespace ECDataUI.Areas.UserManagement.Controllers
{
    [KaveriAuthorizationAttribute]
    public class RoleDetailsController : Controller
    {
        #region Properties
        private String errorMessage = String.Empty;
        CommonFunctions common = new CommonFunctions();
        #endregion

        #region Methods

        //// GET: UserManagement/RoleDetails
        //public ActionResult Index()
        //{
        //    return View();
        //}

        /// <summary>
        ///  AddRoleDetails
        /// </summary>
        /// <param name=""></param>
        /// <returns>Return AddEditRoleDetails page</returns>
        public ActionResult AddRoleDetails()
        {
            try
            {
                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.LevelList = caller.GetCall<List<SelectListItem>>("GetLevelList");
                roleDetailsModel.LevelID = 0;
                roleDetailsModel.IsForUpdate = false;
                roleDetailsModel.IsActive = true;
                return View("AddEditRoleDetails", roleDetailsModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Add Role Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        ///  AddRoleDetails
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns>Return Status of RoleDetails Added or not.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Add Role Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult AddRoleDetails(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                ModelState.Remove("SecondChildMenuDetailsId");
                ModelState.Remove("FirstChildMenuDetailsId");
                ModelState.Remove("ParentMenuDetailsId");
                if (ModelState.IsValid)
                {
                    #region 10-04-2019 For Level drop down validation by shubham bhagat  
                    if (roleDetailsModel.LevelID <= 0)
                    {
                        return Json(new { Status = false, Message = "Level is required " });
                    }
                    #endregion

                    ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                    roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                    RoleDetailsResponseModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsResponseModel>("AddRoleDetails", roleDetailsModel, out errorMessage);
                    if (roleDetailsResponseModel == null)
                    {
                        return Json(new { Status = false, Message = "Error occured while adding Role" });
                    }
                    // return Json(new {  roleDetailsResponseModel }, JsonRequestBehavior.AllowGet);
                    return Json(new { Status = roleDetailsResponseModel.Status, Message = roleDetailsResponseModel.Message });
                }
                else
                {
                    String errorMessage = String.Empty;
                    errorMessage = ModelState.FormatErrorMessageInString();
                    return Json(new { Status = false, Message = errorMessage });

                    //RoleDetailsResponseModel roleDetailsResponseModel = new RoleDetailsResponseModel();
                    //roleDetailsResponseModel.Status = false;
                    //roleDetailsResponseModel.Message = "Role Details not Added.";
                    //return Json(new {  roleDetailsResponseModel }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { Status = false, Message = "Error occured while adding Role" });
            }
        }

        /// <summary>
        ///  RoleDetailsList
        /// </summary>
        /// <param name=""></param>
        /// <returns>Return RoleDetailsList page</returns>
        public ActionResult RoleDetailsList()
        {
            try
            {
                // Added by shubham Bhagat on 20-04-2019 for landing page of department admin flag
                if ((short)CommonEnum.RoleDetails.DepartmentAdmin == KaveriSession.Current.RoleID)
                {
                    if (KaveriSession.Current.IsLandingPageChanged)
                    {
                        KaveriSession.Current.IsLandingPageChanged = false;
                        return RedirectToAction("ViewOfficeUserDetails", "OfficeUserDetails");
                    }
                    //else
                    //    {
                    //    KaveriSession.Current.IsLandingPageChanged = false;

                    //}
                }
                // Added BY SB on 8-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.RoleMenuMapping;
                return View();
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Role Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        ///  LoadRoleDetailsList
        /// </summary>
        /// <param name=""></param>
        /// <returns>Return RoleDetails List to display in datatable</returns>
        // Ajax Call to Load Role List in Datatable
        public ActionResult LoadRoleDetailsList()
        {
            int isTechAdmin = (KaveriSession.Current.RoleID == Convert.ToInt16(CommonEnum.RoleDetails.TechnicalAdmin)) ? 1 : 0;
            try
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();// FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();//
                var length = Request.Form.GetValues("length").FirstOrDefault();//
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

                //Paging Size (10,20,50,100)    
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;

                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                short userLoggedInRole = KaveriSession.Current.RoleID;
                IEnumerable<RoleDetailsModel> roleDetailsList = caller.GetCall<List<RoleDetailsModel>>("RoleDetailsList", new { userLoggedInRole });
                if (roleDetailsList == null)
                {
                    return Json(new { isTechAdmin = isTechAdmin, success = false, errorMessage = "Error occured while getting role details." });
                }

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    roleDetailsList = roleDetailsList.OrderBy(sortColumn + " " + sortColumnDir);
                }

                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    roleDetailsList = roleDetailsList.Where(m =>

                    m.RoleName.ToLower().Contains(searchValue.ToLower())
                    || m.MapMenuButton.ToLower().Contains(searchValue.ToLower())


                    );
                }

                //total number of rows count     
                recordsTotal = roleDetailsList.Count();
                //Paging     
                var data = roleDetailsList.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data   
                //return Json(data: data); 
                return Json(new { isTechAdmin = isTechAdmin, draw, recordsFiltered = recordsTotal, recordsTotal, data });

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { isTechAdmin = isTechAdmin, success = false, errorMessage = "Error occured while getting role details." });
            }
        }


        /// <summary>
        ///  EditRoleDetails
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns>Return AddEditRoleDetails page with roleDetailsModel to edit.</returns>
        [HttpGet]
        public ActionResult EditRoleDetails(String EncryptedID)
        {
            try
            {
                if (EncryptedID == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }
                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                RoleDetailsModel roleDetailsModel = caller.GetCall<RoleDetailsModel>("EditRoleDetails", new { EncryptedID }, out errorMessage);
                roleDetailsModel.EncryptedID = EncryptedID;
                roleDetailsModel.IsForUpdate = true;

                #region 10-04-2019 For Level drop down by shubham bhagat
                KaveriSession.Current.LevelID = roleDetailsModel.LevelID;
                #endregion

                if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }
                return View("AddEditRoleDetails", roleDetailsModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Edit Role Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        ///  UpdateRoleDetails
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns>Return Status of updation of roleDetailsModel is done or not.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Update Role Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateRoleDetails(RoleDetailsModel roleDetailsModel)
        {

            try
            {
                ModelState.Remove("ParentMenuDetailsId");
                ModelState.Remove("FirstChildMenuDetailsId");
                ModelState.Remove("SecondChildMenuDetailsId");

                if (ModelState.IsValid)
                {
                    #region 10-04-2019 For Level drop down validation by shubham bhagat  
                    if (roleDetailsModel.LevelID <= 0)
                    {
                        return Json(new { Status = false, Message = "Level is required " });
                    }
                    #endregion

                    roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                    #region 3-4-2019 For Table LOG by SB
                    roleDetailsModel.UserIPAddress = common.GetIPAddress();
                    #endregion

                    #region 10-04-2019 For Level drop down by shubham bhagat
                    roleDetailsModel.OldLevelID = KaveriSession.Current.LevelID;
                    #endregion

                    ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                    RoleDetailsResponseModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsResponseModel>("UpdateRoleDetails", roleDetailsModel, out errorMessage);
                    if (roleDetailsResponseModel == null)
                    {
                        return Json(new { Status = false, Message = "Error occured while updating Role" });
                    }
                    return Json(new { roleDetailsResponseModel.Status, roleDetailsResponseModel.Message });
                }
                else
                {
                    string errorMsg = ModelState.FormatErrorMessageInString();
                    return Json(new { Status = false, Message = errorMsg });
                }

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { Status = false, Message = "Error occured while updating Role" });
            }
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        ///  DeleteRoleDetails
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns>Return Status of deletion of roleDetailsModel is done or not.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Delete Role Details")]
        public ActionResult DeleteRoleDetails(String EncryptedID)
        {
            ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
            long UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
            RoleDetailsResponseModel roleDetailsResponseModel = caller.GetCall<RoleDetailsResponseModel>("DeleteRoleDetails", new { EncryptedID, UserIdForActivityLogFromSession }, out errorMessage);
            return Json(new { roleDetailsResponseModel });
        }
        #endregion

        /// <summary>
        ///  RoleMenuMapping
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns>Return RoleMenuMapping with parentMenuDetailsList.</returns>
        [HttpGet]
        public ActionResult RoleMenuMapping(String EncryptedID)
        {
            try
            {
                //if (EncryptedID == null)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                // Added by Shubham Bhagat on 22-04-2019 
                short RoleIDFromSession = KaveriSession.Current.RoleID;
                RoleDetailsModel roleDetailsModel = caller.GetCall<RoleDetailsModel>("RoleMenuMapping", new { EncryptedID = EncryptedID, RoleIDFromSession = RoleIDFromSession }, out errorMessage);
                //roleDetailsModel.EncryptedID = EncryptedID;
                //roleDetailsModel.IsForUpdate = true;
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return View("RoleMenuMapping", roleDetailsModel);
                //return Json(new { roleDetails = roleDetails }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Role Menu Mapping View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        ///  FirstChildMenuList
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="parentMenuDetailId"></param>
        /// <returns>Return First Child Menu List.</returns>
        [HttpPost]
        public ActionResult FirstChildMenuList(String EncryptedID, string parentMenuDetailId)
        {
            try
            {
                if (EncryptedID == null|| parentMenuDetailId == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }

                var encryptedParameters = parentMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.ParentMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());


                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                roleDetailsModel.EncryptedID = EncryptedID; 
                // Added by Shubham Bhagat on 22-04-2019 
                roleDetailsModel.RoleIDFromSession = KaveriSession.Current.RoleID;
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("FirstChildMenuList", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { firstChildMenuDetailsList = roleDetailsResponseModel.FirstChildMenuDetailsList, roleDetailsResponseModel.MapUnmapButtonForParent, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while retreiving First child menu list" });
            }
        }

        /// <summary>
        ///  SecondChildMenuList
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="firstChildMenuDetailId"></param>
        /// <returns>Return Second Child Menu List.</returns>
        [HttpPost]
        public ActionResult SecondChildMenuList(String EncryptedID, string firstChildMenuDetailId)
        {
            try
            {
                if (EncryptedID == null|| firstChildMenuDetailId == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }

                var encryptedParameters = firstChildMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.FirstChildMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());


                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                roleDetailsModel.EncryptedID = EncryptedID;
                 
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("SecondChildMenuList", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { secondChildMenuDetailsList = roleDetailsResponseModel.SecondChildMenuDetailsList, roleDetailsResponseModel.MapUnmapButtonForFirstChild, roleDetailsResponseModel.IsParentMenuMapped, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while retreiving Second child menu list" });
            }
        }

        /// <summary>
        ///  GetMapUnmapButtonForSecondChildMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="secondChildMenuDetailId"></param>
        /// <returns>Return Get MapUnmapButton For Second Child Menu.</returns>
        [HttpPost]
        public ActionResult GetMapUnmapButtonForSecondChildMenu(String EncryptedID, int secondChildMenuDetailId)
        {
            try
            {
                if (EncryptedID == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }
                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.EncryptedID = EncryptedID;
                roleDetailsModel.SecondChildMenuDetailsId = secondChildMenuDetailId;
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("GetMapUnmapButtonForSecondChildMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForSecondChild, roleDetailsResponseModel.IsFirstChildMenuMapped, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while retreiving Map/Unmap button" });
            }
        }

        /// <summary>
        ///  MapParentMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="parentMenuDetailId"></param>
        /// <returns>Return MapUnmapButton For Parent Menu.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Map Parent Menu")]
        public ActionResult MapParentMenu(String EncryptedID, string parentMenuDetailId)
        {
            try
            {
                if (EncryptedID == null || parentMenuDetailId==null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }

                var encryptedParameters = parentMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
           

                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.ParentMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());


                roleDetailsModel.EncryptedID = EncryptedID;

                roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                #region 3-4-2019 For Table LOG by SB
                roleDetailsModel.UserIPAddress = common.GetIPAddress();
                #endregion
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("MapParentMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForParent, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while mapping parent menu." });
            }
        }

        /// <summary>
        ///  FirstChildList_SecondChildList_BeforeParentUnmap
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="parentMenuDetailId"></param>
        /// <returns>Return first child menu and second child menu list if they exists.</returns>
        [HttpPost]
        //[EventAuditLogFilter(Description = "Unmap Parent Menu")]
        public ActionResult FirstChildList_SecondChildList_BeforeParentUnmap(String EncryptedID, string parentMenuDetailId)
        {

            try
            {
                if (EncryptedID == null|| parentMenuDetailId == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }

                var encryptedParameters = parentMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();

                roleDetailsModel.ParentMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());

                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                roleDetailsModel.EncryptedID = EncryptedID; 
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("FirstChildList_SecondChildList_BeforeParentUnmap", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}

                //if (roleDetailsResponseModel.FirstChildMenuDetailsList == null && roleDetailsResponseModel.SecondChildMenuDetailsList == null)
                //{
                //    return Json(new { message = "Delete parent menu", IsFirstAndSecondChildListEmpty = true });
                //}
                if (roleDetailsResponseModel.FirstChildListString.Count() > 0 && roleDetailsResponseModel.SecondChildListString.Count() <= 0)
                {
                    return Json(new { roleDetailsResponseModel.FirstChildListString, IsFirstAndSecondChildListEmpty = false, IsSecondChildListEmpty = true, roleDetailsResponseModel.ParentMenuName, roleDetailsResponseModel.RoleName, serverError = false });
                }
                else if (roleDetailsResponseModel.FirstChildListString.Count() > 0 && roleDetailsResponseModel.SecondChildListString.Count() > 0)
                {
                    return Json(new { roleDetailsResponseModel.FirstChildListString, roleDetailsResponseModel.SecondChildListString, IsFirstAndSecondChildListEmpty = false, IsSecondChildListEmpty = false, roleDetailsResponseModel.ParentMenuName, roleDetailsResponseModel.RoleName, serverError = false });
                }
                else
                    return Json(new { IsFirstAndSecondChildListEmpty = true, IsSecondChildListEmpty = true, roleDetailsResponseModel.ParentMenuName, roleDetailsResponseModel.RoleName, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while getting first and second child menu list." });
            }
        }


        /// <summary>
        ///  UnmapParentMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="parentMenuDetailId"></param>
        /// <returns>Return MapUnmapButton For Parent.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Unmap Parent Menu")]
        public ActionResult UnmapParentMenu(String EncryptedID, string parentMenuDetailId)
        {
            try
            {
                if (EncryptedID == null|| parentMenuDetailId==null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }


                var encryptedParameters = parentMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.ParentMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());


                roleDetailsModel.EncryptedID = EncryptedID; 
                roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                #region 3-4-2019 For Table LOG by SB
                roleDetailsModel.UserIPAddress = common.GetIPAddress();
                #endregion

                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("UnmapParentMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForParent, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while unmapping parent menu." });
            }
        }

        /// <summary>
        ///  UnmapParentMenuAndSubMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="parentMenuDetailId"></param>
        /// <returns>Return MapUnmapButton For Parent.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Unmap Parent Menu And Sub Menu")]
        public ActionResult UnmapParentMenuAndSubMenu(String EncryptedID, string parentMenuDetailId)
        {
            try
            {
                if (EncryptedID == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }



                var encryptedParameters = parentMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });



                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.EncryptedID = EncryptedID;
                roleDetailsModel.ParentMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());

                roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                #region 3-4-2019 For Table LOG by SB
                roleDetailsModel.UserIPAddress = common.GetIPAddress();
                #endregion
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("UnmapParentMenuAndSubMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForParent, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while unmapping parent menu and sub menu." });
            }
        }

        /// <summary>
        ///  MapFirstChildMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="firstChildMenuDetailId"></param>
        /// <returns>Return MapUnmapButton For First Child Menu.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Map First Child Menu")]
        public ActionResult MapFirstChildMenu(String EncryptedID, string firstChildMenuDetailId)
        {
            try
            {
                if (EncryptedID == null|| firstChildMenuDetailId==null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }



                var encryptedParameters = firstChildMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.FirstChildMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());


                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                roleDetailsModel.EncryptedID = EncryptedID; 
                roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                #region 3-4-2019 For Table LOG by SB
                roleDetailsModel.UserIPAddress = common.GetIPAddress();
                #endregion
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("MapFirstChildMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForFirstChild, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while mapping first child menu." });
            }
        }

        /// <summary>
        ///  SecondChildList_BeforeFirstChildUnmap
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="firstChildMenuDetailId"></param>
        /// <returns>Return MapUnmapButton For Second Child Menu.</returns>       
        [HttpPost]
        //[EventAuditLogFilter(Description = "Unmap First Child Menu")]
        public ActionResult SecondChildList_BeforeFirstChildUnmap(String EncryptedID, string firstChildMenuDetailId)
        {
            try
            {
                if (EncryptedID == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }

                var encryptedParameters = firstChildMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.FirstChildMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());

                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                roleDetailsModel.EncryptedID = EncryptedID; 
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("SecondChildList_BeforeFirstChildUnmap", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                if (roleDetailsResponseModel.SecondChildListString.Count() > 0)
                {
                    return Json(new { roleDetailsResponseModel.SecondChildListString, IsSecondChildListEmpty = false, roleDetailsResponseModel.FirstChildMenuName, roleDetailsResponseModel.RoleName, serverError = false });
                }
                else
                    return Json(new { message = "Delete First Child menu", IsSecondChildListEmpty = true, roleDetailsResponseModel.FirstChildMenuName, roleDetailsResponseModel.RoleName, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while getting second child menu list." });
            }
        }

        /// <summary>
        ///  UnmapFirstChildMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="firstChildMenuDetailId"></param>
        /// <returns>Return MapUnmapButton For Second Child Menu.</returns>       
        [HttpPost]
        [EventAuditLogFilter(Description = "Unmap First Child Menu")]
        public ActionResult UnmapFirstChildMenu(String EncryptedID, string firstChildMenuDetailId)
        {
            try
            {
                if (EncryptedID == null|| firstChildMenuDetailId == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }


                var encryptedParameters = firstChildMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.FirstChildMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());



                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                roleDetailsModel.EncryptedID = EncryptedID; 
                roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                #region 3-4-2019 For Table LOG by SB
                roleDetailsModel.UserIPAddress = common.GetIPAddress();
                #endregion
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("UnmapFirstChildMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForFirstChild, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while unmapping first child menu." });
            }
        }

        /// <summary>
        ///  UnmapFirstChildMenuAndSubMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="firstChildMenuDetailId"></param>
        /// <returns>Return MapUnmapButton For Second Child Menu.</returns>       
        [HttpPost]
        [EventAuditLogFilter(Description = "Unmap First Child Menu")]
        public ActionResult UnmapFirstChildMenuAndSubMenu(String EncryptedID, string firstChildMenuDetailId)
        {
            try
            {
                if (EncryptedID == null|| firstChildMenuDetailId == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }


                var encryptedParameters = firstChildMenuDetailId.Split('/');
                var decryptedParameters = URLEncrypt.DecryptParameters(new string[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.FirstChildMenuDetailsId = Convert.ToInt16(decryptedParameters["ParentMenuID"].ToString().Trim());


                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                roleDetailsModel.EncryptedID = EncryptedID; 
                roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                #region 3-4-2019 For Table LOG by SB
                roleDetailsModel.UserIPAddress = common.GetIPAddress();
                #endregion
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("UnmapFirstChildMenuAndSubMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForFirstChild, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while unmapping first child menu and sub menu." });
            }
        }

        /// <summary>
        ///  MapSecondChildMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="secondChildMenuDetailId"></param>
        /// <returns>Return MapUnmapButton For Second Child Menu.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Map Second Child Menu")]
        public ActionResult MapSecondChildMenu(String EncryptedID, int secondChildMenuDetailId)
        {
            try
            {
                if (EncryptedID == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }
                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.EncryptedID = EncryptedID;
                roleDetailsModel.SecondChildMenuDetailsId = secondChildMenuDetailId;
                roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                #region 3-4-2019 For Table LOG by SB
                roleDetailsModel.UserIPAddress = common.GetIPAddress();
                #endregion
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("MapSecondChildMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForSecondChild, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while mapping second child menu." });
            }
        }

        /// <summary>
        ///  UnmapSecondChildMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="secondChildMenuDetailId"></param>
        /// <returns>Return MapUnmapButton ForFirstChild.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Unmap Second Child Menu")]
        public ActionResult UnmapSecondChildMenu(String EncryptedID, int secondChildMenuDetailId)
        {
            try
            {
                if (EncryptedID == null)
                {
                    return RedirectToAction("RoleDetailsList", "RoleDetails");
                }
                ServiceCaller caller = new ServiceCaller("RoleDetailsApiController");
                RoleDetailsModel roleDetailsModel = new RoleDetailsModel();
                roleDetailsModel.EncryptedID = EncryptedID;
                roleDetailsModel.SecondChildMenuDetailsId = secondChildMenuDetailId;
                roleDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                #region 3-4-2019 For Table LOG by SB
                roleDetailsModel.UserIPAddress = common.GetIPAddress();
                #endregion
                RoleDetailsModel roleDetailsResponseModel = caller.PostCall<RoleDetailsModel, RoleDetailsModel>("UnmapSecondChildMenu", roleDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { roleDetailsResponseModel.MapUnmapButtonForSecondChild, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, Message = "Error occured while unmapping second child menu." });
            }
        }
        #endregion
    }
}
