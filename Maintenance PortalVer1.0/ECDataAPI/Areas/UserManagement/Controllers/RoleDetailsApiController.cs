#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   RoleDetailsApiController.cs
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
    public class RoleDetailsApiController : ApiController
    {
        IRoleDetails roleDetails = null;
        /// <summary>
        /// Returns RoleDetailsList
        /// </summary>
        /// <param name="userLoggedInRole"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RoleDetailsApiController/RoleDetailsList")]
        public IHttpActionResult RoleDetailsList(short userLoggedInRole)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                IEnumerable<RoleDetailsModel> roleDetailsList = roleDetails.RoleDetailsList( userLoggedInRole);
                return Ok(roleDetailsList);

            }
            catch (Exception ) { throw ; }
        }

        /// <summary>
        /// Add Role Details
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/AddRoleDetails")]
        [EventApiAuditLogFilter(Description = "Add Role Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult AddRoleDetails(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsResponseModel roleDetailsResponseModel = roleDetails.AddRoleDetails(roleDetailsModel);
                return Ok(roleDetailsResponseModel);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Edits RoleDetails
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RoleDetailsApiController/EditRoleDetails")]
        public IHttpActionResult EditRoleDetails(String EncryptedID)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsModel = roleDetails.EditRoleDetails(EncryptedID);
                return Ok(roleDetailsModel);
            }
            catch (Exception ) { throw ; }
        }


        /// <summary>
        /// Update Role Details
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/UpdateRoleDetails")]
        [EventApiAuditLogFilter(Description = "Update Role Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UpdateRoleDetails(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsResponseModel roleDetailsResponseModel = roleDetails.UpdateRoleDetails(roleDetailsModel);
                return Ok(roleDetailsResponseModel);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Delete Role Details
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="UserIdForActivityLogFromSession"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RoleDetailsApiController/DeleteRoleDetails")]
        [EventApiAuditLogFilter(Description = "Delete Role Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DeleteRoleDetails(String EncryptedID, long UserIdForActivityLogFromSession)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsResponseModel roleDetailsResponseModel = roleDetails.DeleteRoleDetails(EncryptedID, UserIdForActivityLogFromSession);
                return Ok(roleDetailsResponseModel);
            }
            catch (Exception) { throw; }
        }
        #endregion

        /// <summary>
        /// Maps a role to a menu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="RoleIDFromSession"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RoleDetailsApiController/RoleMenuMapping")]
        public IHttpActionResult RoleMenuMapping(String EncryptedID,short RoleIDFromSession)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsModel = roleDetails.RoleMenuMapping(EncryptedID, RoleIDFromSession);
                return Ok(roleDetailsModel);
            }
            catch (Exception ) { throw ; }
        }

        /// <summary>
        /// returns FirstChildMenuList
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/FirstChildMenuList")]
        public IHttpActionResult FirstChildMenuList(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj= roleDetails.FirstChildMenuList(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// returns SecondChildMenuList
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/SecondChildMenuList")]
        public IHttpActionResult SecondChildMenuList(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.SecondChildMenuList(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Gets MapUnmapButtonForSecondChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/GetMapUnmapButtonForSecondChildMenu")]
        public IHttpActionResult GetMapUnmapButtonForSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.GetMapUnmapButtonForSecondChildMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Maps ParentMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/MapParentMenu")]
        [EventApiAuditLogFilter(Description = "Map Parent Menu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult MapParentMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.MapParentMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// FirstChildList SecondChildList Before Parent Unmap
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/FirstChildList_SecondChildList_BeforeParentUnmap")]
        //[EventApiAuditLogFilter(Description = "Unmap Parent Menu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult FirstChildList_SecondChildList_BeforeParentUnmap(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.FirstChildList_SecondChildList_BeforeParentUnmap(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/UnmapParentMenu")]
        [EventApiAuditLogFilter(Description = "Unmap Parent Menu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UnmapParentMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.UnmapParentMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Unmap Parent Menu And SubMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/UnmapParentMenuAndSubMenu")]
        [EventApiAuditLogFilter(Description = "Unmap Parent Menu And SubMenu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UnmapParentMenuAndSubMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.UnmapParentMenuAndSubMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }


        /// <summary>
        /// Map First Child Menu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/MapFirstChildMenu")]
        [EventApiAuditLogFilter(Description = "Map First Child Menu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult MapFirstChildMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.MapFirstChildMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// SecondChildList BeforeFirstChildUnmap
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/SecondChildList_BeforeFirstChildUnmap")]
       // [EventApiAuditLogFilter(Description = "Unmap First Child Menu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SecondChildList_BeforeFirstChildUnmap(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.SecondChildList_BeforeFirstChildUnmap(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Unmaps FirstChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/UnmapFirstChildMenu")]
        [EventApiAuditLogFilter(Description = "Unmap First Child Menu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UnmapFirstChildMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.UnmapFirstChildMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Unmaps FirstChildMenuAndSubMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/UnmapFirstChildMenuAndSubMenu")]
        [EventApiAuditLogFilter(Description = "Unmap First Child Menu And SubMenu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UnmapFirstChildMenuAndSubMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.UnmapFirstChildMenuAndSubMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Maps SecondChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/MapSecondChildMenu")]
        [EventApiAuditLogFilter(Description = "Map Second Child Menu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult MapSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.MapSecondChildMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Unmaps SecondChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RoleDetailsApiController/UnmapSecondChildMenu")]
        [EventApiAuditLogFilter(Description = "Unmap Second Child Menu", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UnmapSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                roleDetails = new RoleDetailsBAL();
                RoleDetailsModel roleDetailsObj = roleDetails.UnmapSecondChildMenu(roleDetailsModel);
                return Ok(roleDetailsObj);
            }
            catch (Exception )
            {
                throw ;
            }
        }

        /// <summary>
        /// Gets LevelList
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RoleDetailsApiController/GetLevelList")]
        public IHttpActionResult GetLevelList()
        {
            roleDetails = new RoleDetailsBAL();
            List<System.Web.Mvc.SelectListItem> levelList = roleDetails.GetLevelList();
            return Ok(levelList);
        }
    }
}
