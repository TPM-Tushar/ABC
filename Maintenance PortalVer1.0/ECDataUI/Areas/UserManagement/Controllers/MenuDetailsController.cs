#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   MenuDetailsController.cs
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
    * File Name         :   MenuDetailsController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   11-09-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller to Consume API controller Methods
*/
#endregion


#region References
using ECDataUI.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Linq.Dynamic;
using CustomModels.Models.UserManagement;
using ECDataUI.Filters;
using ECDataUI.Session;

#endregion


namespace ECDataUI.Areas.UserManagement.Controllers
{
    //[OutputCache(Duration =0)]
    [KaveriAuthorizationAttribute]
    public class MenuDetailsController : Controller
    {

        #region Properties
        private String errorMessage = String.Empty;
        CommonFunctions commonObj = new CommonFunctions();
        #endregion

        #region Methods

        // GET: UserManagement/MenuDetails
        //public ActionResult Index()
        //{
        //    return View();
        //}

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        ///  AddMenu
        /// </summary>
        /// <returns>Return AddEditMenu page</returns>
        [HttpGet]
        public ActionResult AddMenu()
        {
            try
            {
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                MenuDetailsModel menuDetailsModelFromDB = caller.GetCall<MenuDetailsModel>("AddMenu");
                return View("AddEditMenu", menuDetailsModelFromDB);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Menu Details Add form View", URLToRedirect = "/Home/HomePage" });
            }
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        ///  AddMenu
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns>Return Status of MenuDetails Added or not.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Add Menu Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult AddMenu(MenuDetailsModel menuDetailsModel)
        {
            try
            {

                ModelState.Remove("ControllerActionDetails_ControllerListId");
                ModelState.Remove("ControllerActionDetails_ActionListId");
                ModelState.Remove("MAS_Modules_ModuleListId");
                ModelState.Remove("ControllerActionDetails_AreaListId");


                if (ModelState.IsValid)
                {
                    ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                    menuDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                    MenuDetailsResponseModel menuDetailsResponseModel = caller.PostCall<MenuDetailsModel, MenuDetailsResponseModel>("AddMenu", menuDetailsModel, out errorMessage);
                    return Json(new { menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                    menuDetailsResponseModel.Result = false;
                    //  menuDetailsResponseModel.Message = "Menu Details not Added.";
                    // changed by m rafe on 26-11-19
                    menuDetailsResponseModel.Message = ModelState.FormatErrorMessage();
                    return Json(new { menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                menuDetailsResponseModel.Result = false;
                menuDetailsResponseModel.Message = "Menu Details not Added.";
                return Json(new { menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        /// <summary>
        ///  RetriveMenu
        /// </summary>
        /// <param name=""></param>
        /// <returns>Return RetriveMenu page</returns>
        //  Retrive Menu List
        public ActionResult RetriveMenu()
        {
            try
            {
                // Added BY SB on 8-04-2019 to active link clicked
                KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.MenuActionDetails;
                return View();
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Menu Details View", URLToRedirect = "/Home/HomePage" });
            }
        }


        /// <summary>
        ///  LoadData
        /// </summary>
        /// <param name=""></param>
        /// <returns>Return MenuDetails List to display in datatable</returns>
        // Ajax Call to Load Menu List in Datatable
        public ActionResult LoadData()
        {
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

                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                //String EncryptedId = "ABC";
                //IEnumerable<MenuDetailsModel> menuList = caller.GetCall<List<MenuDetailsModel>>("RetriveMenu", new { EncryptedId = EncryptedId });
                IEnumerable<MenuDetailsModel> menuList = caller.GetCall<List<MenuDetailsModel>>("RetriveMenu");
                if (menuList == null)
                {
                    return Json(new { success = false, errorMessage = "Error occured while getting menu details." });
                }

                //Sorting
                if (!(string.IsNullOrEmpty(sortColumn) && string.IsNullOrEmpty(sortColumnDir)))
                {
                    menuList = menuList.OrderBy(sortColumn + " " + sortColumnDir);
                }
                //Search    
                //if (!string.IsNullOrEmpty(searchValue))
                //{
                //    menuList = menuList.Where(m => m.MenuName == searchValue);
                //}
                //Search    
                if (!string.IsNullOrEmpty(searchValue))
                {
                    menuList = menuList.Where(m => m.MenuName.ToLower().Contains(searchValue.ToLower())
                ||
                    m.ActionAssigned.ToLower().Contains(searchValue.ToLower()
                    ));
                }

                //total number of rows count     
                recordsTotal = menuList.Count();
                //Paging     
                var data = menuList.Skip(skip).Take(pageSize).ToList();
                //Returning Json Data   
                //return Json(data: data); 
                return Json(new { draw, recordsFiltered = recordsTotal, recordsTotal, data });

            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { success = false, errorMessage = "Error occured while getting menu details." });
            }
        }

        //  #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Edit Code Commented Changes on 15-12-2018 Final Changes in User Management
        /// <summary>
        ///  EditMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns>Return AddEditMenu page with menuDetailsModel to edit.</returns>
        [HttpGet]
        public ActionResult EditMenu(String EncryptedID)
        {
            try
            {
                if (EncryptedID == null)
                {
                    return RedirectToAction("RetriveMenu", "MenuDetails");
                    //MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                    //menuDetailsResponseModel.Result = false;
                    //menuDetailsResponseModel.Message = "Menu Details cannot be Edited.";
                    //return Json(new { menuDetailsResponseModel = menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
                }

                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                MenuDetailsModel menuDetailsModel = caller.GetCall<MenuDetailsModel>("EditMenu", new { EncryptedID }, out errorMessage);
                menuDetailsModel.EncryptedID = EncryptedID;
                menuDetailsModel.IsUpdatable = true;

                //if (menuDetailsModel.MenuDetailsResponseModel.Result == false)
                //{
                //    MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                //    menuDetailsResponseModel.Result = menuDetailsModel.MenuDetailsResponseModel.Result;
                //    menuDetailsResponseModel.Message = menuDetailsModel.MenuDetailsResponseModel.Message;
                //    return Json(new { menuDetailsResponseModel = menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
                //}
                if (menuDetailsModel.MenuDetailsResponseModel.Result == false)
                {
                    return RedirectToAction("RetriveMenu", "MenuDetails");
                }
                //return PartialView("AddEditMenu1", menuDetailsModel);
                return View("AddEditMenu", menuDetailsModel);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Menu Details Edit form View", URLToRedirect = "/Home/HomePage" });
            }

        }
        #endregion
        // #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        ///  UpdateMenu
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns>Return Status of updation of menuDetailsModel is done or not.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Update Menu Details")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UpdateMenu(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                menuDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                menuDetailsModel.UserIPAddress = commonObj.GetIPAddress();
                MenuDetailsResponseModel menuDetailsResponseModel = caller.PostCall<MenuDetailsModel, MenuDetailsResponseModel>("UpdateMenu", menuDetailsModel, out errorMessage);
                return Json(new { menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                menuDetailsResponseModel.Result = false;
                menuDetailsResponseModel.Message = "Menu Details not Updated.";
                return Json(new { menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Delete Code Commented Changes on 15-12-2018 Final Changes in User Management
        /// <summary>
        ///  DeleteMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns>Return Status of deletion of menuDetailsModel is done or not.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Delete Menu Details")]
        public ActionResult DeleteMenu(String EncryptedID)
        {
            try
            {
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");

                long UserIdForActivityLogFromSession = KaveriSession.Current.UserID;

                MenuDetailsResponseModel menuDetailsResponseModel = caller.GetCall<MenuDetailsResponseModel>("DeleteMenu", new { EncryptedID, UserIdForActivityLogFromSession, IPAddress = commonObj.GetIPAddress() }, out errorMessage);
                return Json(new { menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                menuDetailsResponseModel.Message = "Menu Details Not Deleted";
                menuDetailsResponseModel.Result = false;
                return Json(new { menuDetailsResponseModel }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        ///  GetFirstChildMenuDetailsList
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns>Return list of firstChildMenuDetailsList.</returns>
        [HttpPost]
        public ActionResult GetFirstChildMenuDetailsList(int parentId)
        {
            try
            {
                //int parentIdInt = Convert.ToInt32(parentId);
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                IEnumerable<SelectListItem> firstChildMenuDetailsList = caller.GetCall<List<SelectListItem>>("GetFirstChildMenuDetailsList", new { parentId }, out errorMessage);
                return Json(new { firstChildMenuDetailsList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting First Child menu details list." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        ///  GetSecondChildMenuDetailsList
        /// </summary>
        /// <param name="firstChildMenuDetailsId"></param>
        /// <returns>Return list of secondChildMenuDetailsList.</returns>
        [HttpPost]
        public ActionResult GetSecondChildMenuDetailsList(int firstChildMenuDetailsId)
        {
            try
            {
                //int parentIdInt = Convert.ToInt32(parentId);
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                IEnumerable<SelectListItem> secondChildMenuDetailsList = caller.GetCall<List<SelectListItem>>("GetSecondChildMenuDetailsList", new { firstChildMenuDetailsId }, out errorMessage);
                return Json(new { secondChildMenuDetailsList, serverError = false }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Second Child menu details list." }, JsonRequestBehavior.AllowGet);
            }
        }
        #endregion

        /// <summary>
        ///  MenuActionMapping
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns>Return RoleMenuMapping with ControllerActionDetails_AreaList.</returns>
        [HttpGet]
        public ActionResult MenuActionMapping(String EncryptedID)
        {
            try
            {
                //if (EncryptedID == null)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                MenuDetailsModel menuDetailsModel = caller.GetCall<MenuDetailsModel>("MenuActionMapping", new { EncryptedID }, out errorMessage);
                //roleDetailsModel.EncryptedID = EncryptedID;
                //roleDetailsModel.IsForUpdate = true;
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return View("MenuActionMapping", menuDetailsModel);
                //return Json(new { roleDetails = roleDetails }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Menu Action Mapping View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        ///  ControllerList
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="ControllerActionDetails_AreaListId"></param>
        /// <returns>Return ControllerList.</returns>
        [HttpPost]
        public ActionResult ControllerList(String EncryptedID, String ControllerActionDetails_AreaListId)
        {
            try
            {
                //if (EncryptedID == null)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                MenuDetailsModel menuDetailsModel = new MenuDetailsModel();
                menuDetailsModel.EncryptedID = EncryptedID;
                menuDetailsModel.ControllerActionDetails_AreaListId = ControllerActionDetails_AreaListId;
                MenuDetailsModel menuDetailsResponseModel = caller.PostCall<MenuDetailsModel, MenuDetailsModel>("ControllerList", menuDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { menuDetailsResponseModel.ControllerActionDetails_ControllerList, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting controller list." });
            }
        }


        /// <summary>
        ///  ActionList
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="ControllerActionDetails_ControllerListId"></param>
        /// <returns>Return ActionList.</returns>
        [HttpPost]
        public ActionResult ActionList(String EncryptedID, String ControllerActionDetails_ControllerListId)
        {
            try
            {
                //if (EncryptedID == null)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                MenuDetailsModel menuDetailsModel = new MenuDetailsModel();
                menuDetailsModel.EncryptedID = EncryptedID;
                menuDetailsModel.ControllerActionDetails_ControllerListId = ControllerActionDetails_ControllerListId;
                MenuDetailsModel menuDetailsResponseModel = caller.PostCall<MenuDetailsModel, MenuDetailsModel>("ActionList", menuDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { menuDetailsResponseModel.ControllerActionDetails_ActionList, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting action list." });
            }
        }


        /// <summary>
        ///  MapMenuToAction
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="MAS_Modules_ModuleListId"></param>
        /// <param name="ControllerActionDetails_AreaListId"></param>
        /// <param name="ControllerActionDetails_ControllerListId"></param>
        /// <param name="ControllerActionDetails_ActionListId"></param>
        /// <returns>Return Status of Menu Action mapping and response message.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Menu Mapped To Action")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult MapMenuToAction(String EncryptedID, int MAS_Modules_ModuleListId, String ControllerActionDetails_AreaListId, String ControllerActionDetails_ControllerListId, String ControllerActionDetails_ActionListId)
        {

            try
            {
                if (ModelState.IsValid)
                {
                    //if (EncryptedID == null)
                    //{
                    //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                    //}
                    ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                    MenuDetailsModel menuDetailsModel = new MenuDetailsModel();
                    menuDetailsModel.EncryptedID = EncryptedID;
                    menuDetailsModel.MAS_Modules_ModuleListId = MAS_Modules_ModuleListId;
                    menuDetailsModel.ControllerActionDetails_AreaListId = ControllerActionDetails_AreaListId;
                    menuDetailsModel.ControllerActionDetails_ControllerListId = ControllerActionDetails_ControllerListId;
                    menuDetailsModel.ControllerActionDetails_ActionListId = ControllerActionDetails_ActionListId;
                    menuDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                    menuDetailsModel.UserIPAddress = commonObj.GetIPAddress();
                    MenuDetailsModel menuDetailsResponseModel = caller.PostCall<MenuDetailsModel, MenuDetailsModel>("MapMenuToAction", menuDetailsModel, out errorMessage);
                    //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                    //{
                    //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                    //}
                    return Json(new { status = menuDetailsResponseModel.MenuDetailsResponseModel.Result, message = menuDetailsResponseModel.MenuDetailsResponseModel.Message });

                }
                else
                {
                    string errorMsg = ModelState.FormatErrorMessageInString();
                    return Json(new { status = false, message = errorMsg });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { status = false, message = "Menu not mapped to action." });
            }
        }

        /// <summary>
        ///  UnmapMenuToAction
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="MAS_Modules_ModuleListId"></param>
        /// <param name="ControllerActionDetails_AreaListId"></param>
        /// <param name="ControllerActionDetails_ControllerListId"></param>
        /// <param name="ControllerActionDetails_ActionListId"></param>
        /// <returns>Return Status of Menu Action unmapping and response message.</returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Menu Unmapped To Action")]
        [ValidateAntiForgeryTokenOnAllPosts]
        public ActionResult UnmapMenuToAction(String EncryptedID, int MAS_Modules_ModuleListId, String ControllerActionDetails_AreaListId, String ControllerActionDetails_ControllerListId, String ControllerActionDetails_ActionListId)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    //if (EncryptedID == null)
                    //{
                    //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                    //}
                    ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                    MenuDetailsModel menuDetailsModel = new MenuDetailsModel();
                    menuDetailsModel.EncryptedID = EncryptedID;
                    menuDetailsModel.MAS_Modules_ModuleListId = MAS_Modules_ModuleListId;
                    menuDetailsModel.ControllerActionDetails_AreaListId = ControllerActionDetails_AreaListId;
                    menuDetailsModel.ControllerActionDetails_ControllerListId = ControllerActionDetails_ControllerListId;
                    menuDetailsModel.ControllerActionDetails_ActionListId = ControllerActionDetails_ActionListId;
                    menuDetailsModel.UserIdForActivityLogFromSession = KaveriSession.Current.UserID;
                    menuDetailsModel.UserIPAddress = commonObj.GetIPAddress();
                    MenuDetailsModel menuDetailsResponseModel = caller.PostCall<MenuDetailsModel, MenuDetailsModel>("UnmapMenuToAction", menuDetailsModel, out errorMessage);
                    //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                    //{
                    //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                    //}
                    return Json(new { status = menuDetailsResponseModel.MenuDetailsResponseModel.Result, message = menuDetailsResponseModel.MenuDetailsResponseModel.Message });
                }
                else
                {
                    string errorMsg = ModelState.FormatErrorMessageInString();
                    return Json(new { status = false, message = errorMsg });
                }
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { status = false, message = "Menu not unmapped to action." });
            }

        }


        /// <summary>
        ///  MapUnmapMenuActionButton
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="MAS_Modules_ModuleListId"></param>
        /// <param name="ControllerActionDetails_AreaListId"></param>
        /// <param name="ControllerActionDetails_ControllerListId"></param>
        /// <param name="ControllerActionDetails_ActionListId"></param>
        /// <returns>Return MapUnmap Menu Action Button.</returns>
        [HttpPost]
        public ActionResult MapUnmapMenuActionButton(String EncryptedID, int MAS_Modules_ModuleListId, String ControllerActionDetails_AreaListId, String ControllerActionDetails_ControllerListId, String ControllerActionDetails_ActionListId)
        {
            try
            {
                //if (EncryptedID == null)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                ServiceCaller caller = new ServiceCaller("MenuDetailsApiController");
                MenuDetailsModel menuDetailsModel = new MenuDetailsModel();
                menuDetailsModel.EncryptedID = EncryptedID;
                menuDetailsModel.MAS_Modules_ModuleListId = MAS_Modules_ModuleListId;
                menuDetailsModel.ControllerActionDetails_AreaListId = ControllerActionDetails_AreaListId;
                menuDetailsModel.ControllerActionDetails_ControllerListId = ControllerActionDetails_ControllerListId;
                menuDetailsModel.ControllerActionDetails_ActionListId = ControllerActionDetails_ActionListId;
                MenuDetailsModel menuDetailsResponseModel = caller.PostCall<MenuDetailsModel, MenuDetailsModel>("MapUnmapMenuActionButton", menuDetailsModel, out errorMessage);
                //if (roleDetailsModel.RoleDetailsResponseModel.Status == false)
                //{
                //    return RedirectToAction("RoleDetailsList", "RoleDetails");
                //}
                return Json(new { menuDetailsResponseModel.MapUnmapMenuActionButton, serverError = false });
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return Json(new { serverError = true, errorMessage = "Error occured while getting Map/Unmap button." });
            }
        }

        #endregion
    }
}
