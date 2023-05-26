#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RoleDetailsBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for User Management module.
*/
#endregion

using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.DAL;
using ECDataAPI.Areas.UserManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.BAL
{
    public class RoleDetailsBAL : IRoleDetails
    {
        IRoleDetails roleDetails = new RoleDetailsDAL();

        /// <summary>
        /// returns RoleDetailsList
        /// </summary>
        /// <param name="userLoggedInRole"></param>
        /// <returns></returns>
        public IEnumerable<RoleDetailsModel> RoleDetailsList(short userLoggedInRole)
        {
            return roleDetails.RoleDetailsList(userLoggedInRole);
        }

        /// <summary>
        /// Adds RoleDetails
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsResponseModel AddRoleDetails(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.AddRoleDetails(roleDetailsModel);
        }

        /// <summary>
        /// Edits RoleDetails
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public RoleDetailsModel EditRoleDetails(String EncryptedID)
        {
            return roleDetails.EditRoleDetails(EncryptedID);
        }

        /// <summary>
        /// Updates RoleDetails
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsResponseModel UpdateRoleDetails(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.UpdateRoleDetails(roleDetailsModel);
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Deletes RoleDetails
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="UserIdForActivityLogFromSession"></param>
        /// <returns></returns>
        public RoleDetailsResponseModel DeleteRoleDetails(String EncryptedID, long UserIdForActivityLogFromSession)
        {
            return roleDetails.DeleteRoleDetails(EncryptedID, UserIdForActivityLogFromSession);
        }
        #endregion

        /// <summary>
        /// RoleMenuMapping
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="RoleIDFromSession"></param>
        /// <returns></returns>
        public RoleDetailsModel RoleMenuMapping(String EncryptedID, short RoleIDFromSession)
        {
            return roleDetails.RoleMenuMapping(EncryptedID, RoleIDFromSession);
        }


        /// <summary>
        /// returns FirstChildMenuList
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel FirstChildMenuList(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.FirstChildMenuList(roleDetailsModel);
        }

        /// <summary>
        /// Returns SecondChildMenuList
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel SecondChildMenuList(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.SecondChildMenuList(roleDetailsModel);
        }

        /// <summary>
        /// Gets MapUnmapButtonForSecondChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel GetMapUnmapButtonForSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.GetMapUnmapButtonForSecondChildMenu(roleDetailsModel);
        }

        /// <summary>
        /// Maps ParentMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel MapParentMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.MapParentMenu(roleDetailsModel);
        }

        /// <summary>
        /// FirstChildList SecondChildList BeforeParentUnmap
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel FirstChildList_SecondChildList_BeforeParentUnmap(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.FirstChildList_SecondChildList_BeforeParentUnmap(roleDetailsModel);
        }

        /// <summary>
        /// UnmapParentMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel UnmapParentMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.UnmapParentMenu(roleDetailsModel);
        }

        /// <summary>
        /// To Unmap ParentMenu And SubMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel UnmapParentMenuAndSubMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.UnmapParentMenuAndSubMenu(roleDetailsModel);
        }

        /// <summary>
        /// Map First ChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel MapFirstChildMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.MapFirstChildMenu(roleDetailsModel);
        }

        /// <summary>
        /// SecondChildList_BeforeFirstChildUnmap
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel SecondChildList_BeforeFirstChildUnmap(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.SecondChildList_BeforeFirstChildUnmap(roleDetailsModel);
        }


        /// <summary>
        /// Unmap FirstChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel UnmapFirstChildMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.UnmapFirstChildMenu(roleDetailsModel);
        }

        /// <summary>
        /// UnmapFirstChildMenuAndSubMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel UnmapFirstChildMenuAndSubMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.UnmapFirstChildMenuAndSubMenu(roleDetailsModel);
        }
        /// <summary>
        /// MapSecondChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>

        public RoleDetailsModel MapSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.MapSecondChildMenu(roleDetailsModel);
        }

        /// <summary>
        /// UnmapSecondChildMenu
        /// </summary>
        /// <param name="roleDetailsModel"></param>
        /// <returns></returns>
        public RoleDetailsModel UnmapSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            return roleDetails.UnmapSecondChildMenu(roleDetailsModel);
        }

        /// <summary>
        /// GetLevelList
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetLevelList()
        {
            return roleDetails.GetLevelList();
        }
    }
}
