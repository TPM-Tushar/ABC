#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   MenuDetailsApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for User Management module.
*/
#endregion

using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.BAL;
using ECDataAPI.Areas.UserManagement.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;


namespace ECDataAPI.Areas.UserManagement.Controllers
{
    public class MenuDetailsApiController : ApiController
    {
        IMenuDetails menuDetails = null;

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Adds Menu
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MenuDetailsApiController/AddMenu")]
        [EventApiAuditLogFilter(Description = "Add Menu Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult AddMenu(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsResponseModel menuDetailsResponseModel = menuDetails.AddMenu(menuDetailsModel);
                return Ok(menuDetailsResponseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        /// <summary>
        /// Retrives Menu List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MenuDetailsApiController/RetriveMenu")]
        public IHttpActionResult RetriveMenu()
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                IEnumerable<MenuDetailsModel> result = menuDetails.RetriveMenu();
                return Ok(result);
            }
            catch (Exception) { throw; }
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Edits Menu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MenuDetailsApiController/EditMenu")]
        public IHttpActionResult EditMenu(String EncryptedID)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                var menuDetailsView = menuDetails.EditMenu(EncryptedID);
                return Ok(menuDetailsView);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// For Updating Menu
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MenuDetailsApiController/UpdateMenu")]
        [EventApiAuditLogFilter(Description = "Update Menu Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UpdateMenu(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsResponseModel menuDetailsResponseModel = menuDetails.UpdateMenu(menuDetailsModel);
                return Ok(menuDetailsResponseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// For Deleting Menu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="UserIdForActivityLogFromSession"></param>
        /// <returns></returns>
        [HttpGet]
        //[HttpPost]
        [Route("api/MenuDetailsApiController/DeleteMenu")]
        [EventApiAuditLogFilter(Description = "Delete Menu Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DeleteMenu(String EncryptedID, long UserIdForActivityLogFromSession, string IPAddress)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsResponseModel menuDetailsResponseModel = menuDetails.DeleteMenu(EncryptedID, UserIdForActivityLogFromSession, IPAddress);
                return Ok(menuDetailsResponseModel);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region RoleMenuMapping
       // [HttpGet]
        //[Route("api/MenuDetailsApiController/GetRoleList")]
        //public IHttpActionResult GetRoleList()
        //{
        //    try
        //    {
        //        menuDetails = new MenuDetailsBAL();
        //        var roleList = menuDetails.GetRoleList();
        //        return Ok(roleList);

        //    }
        //    catch (Exception exception) { throw exception; }
        //}
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Gets Menu List
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MenuDetailsApiController/GetMenuList")]
        public IHttpActionResult GetMenuList()
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                var menuList = menuDetails.GetMenuList();
                return Ok(menuList);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Adds Menu
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MenuDetailsApiController/AddMenu")]
        public IHttpActionResult AddMenu()
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsModel menuDetailsModel = menuDetails.AddMenu();
                return Ok(menuDetailsModel);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Gets FirstChildMenuDetailsList
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MenuDetailsApiController/GetFirstChildMenuDetailsList")]
        public IHttpActionResult GetFirstChildMenuDetailsList(int parentId)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                List<System.Web.Mvc.SelectListItem> firstChildMenuDetailsList = menuDetails.GetFirstChildMenuDetailsList(parentId);
                return Ok(firstChildMenuDetailsList);
            }
            catch (Exception) { throw; }
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Get SecondChildMenuDetailsList
        /// </summary>
        /// <param name="firstChildMenuDetailsId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MenuDetailsApiController/GetSecondChildMenuDetailsList")]
        public IHttpActionResult GetSecondChildMenuDetailsList(int firstChildMenuDetailsId)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                List<System.Web.Mvc.SelectListItem> secondChildMenuDetailsList = menuDetails.GetSecondChildMenuDetailsList(firstChildMenuDetailsId);
                return Ok(secondChildMenuDetailsList);
            }
            catch (Exception) { throw; }
        }
        #endregion

        /// <summary>
        /// Maps Action to a menu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/MenuDetailsApiController/MenuActionMapping")]
        public IHttpActionResult MenuActionMapping(String EncryptedID)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsModel menuDetailsModel = menuDetails.MenuActionMapping(EncryptedID);
                return Ok(menuDetailsModel);
            }
            catch (Exception) { throw; }
        }

        /// <summary>
        /// returns Controller List
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MenuDetailsApiController/ControllerList")]
        public IHttpActionResult ControllerList(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsModel menuDetailsObj = menuDetails.ControllerList(menuDetailsModel);
                return Ok(menuDetailsObj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns ActionList
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MenuDetailsApiController/ActionList")]
        public IHttpActionResult ActionList(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsModel menuDetailsObj = menuDetails.ActionList(menuDetailsModel);
                return Ok(menuDetailsObj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Maps Menu To an Action
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MenuDetailsApiController/MapMenuToAction")]
        [EventApiAuditLogFilter(Description = "Menu Mapped to Action", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult MapMenuToAction(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsModel menuDetailsObj = menuDetails.MapMenuToAction(menuDetailsModel);
                return Ok(menuDetailsObj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Unmaps MenuToAction
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MenuDetailsApiController/UnmapMenuToAction")]
        [EventApiAuditLogFilter(Description = "Menu Unmapped to Action", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UnmapMenuToAction(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsModel menuDetailsObj = menuDetails.UnmapMenuToAction(menuDetailsModel);
                return Ok(menuDetailsObj);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Maps Unmaps MenuActionButton
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/MenuDetailsApiController/MapUnmapMenuActionButton")]
        public IHttpActionResult MapUnmapMenuActionButton(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                menuDetails = new MenuDetailsBAL();
                MenuDetailsModel menuDetailsObj = menuDetails.MapUnmapMenuActionButton(menuDetailsModel);
                return Ok(menuDetailsObj);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
