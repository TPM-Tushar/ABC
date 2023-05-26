#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   MenuDetailsBAL.cs
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
    public class MenuDetailsBAL : IMenuDetails
    {
        IMenuDetails menuDetails = new MenuDetailsDAL();

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Adds Menu
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsResponseModel AddMenu(MenuDetailsModel menuDetailsModel)
        {
            return menuDetails.AddMenu(menuDetailsModel);
        }
        #endregion

        /// <summary>
        /// Retrives Menu
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MenuDetailsModel> RetriveMenu()
        {
            return menuDetails.RetriveMenu();
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// EditsMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public MenuDetailsModel EditMenu(String EncryptedID)
        {
            return menuDetails.EditMenu(EncryptedID);
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Updates Menu
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsResponseModel UpdateMenu(MenuDetailsModel menuDetailsModel)
        {
            return menuDetails.UpdateMenu(menuDetailsModel);
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Deletes Menu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="UserIdForActivityLogFromSession"></param>
        /// <returns></returns>
        public MenuDetailsResponseModel DeleteMenu(String EncryptedID, long UserIdForActivityLogFromSession, string IPAddress)
        {
            return menuDetails.DeleteMenu(EncryptedID, UserIdForActivityLogFromSession, IPAddress);
        }
        #endregion

        #region RoleMenuMapping
        //public List<SelectListItem> GetRoleList()
        //{
        //    return menuDetails.GetRoleList();
        //}
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Gets MenuList
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetMenuList(int menuId = 0)
        {
            return menuDetails.GetMenuList();
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Adds Menu
        /// </summary>
        /// <returns></returns>
        public MenuDetailsModel AddMenu()
        {
            return menuDetails.AddMenu();
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Gets FirstChildMenuDetailsList
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetFirstChildMenuDetailsList(int parentId, int menuId = 0)
        {
            return menuDetails.GetFirstChildMenuDetailsList(parentId);
        }
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Gets SecondChildMenuDetailsList
        /// </summary>
        /// <param name="firstChildMenuDetailsId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSecondChildMenuDetailsList(int firstChildMenuDetailsId, int menuId = 0)
        {
            return menuDetails.GetSecondChildMenuDetailsList(firstChildMenuDetailsId);
        }
        /// <summary>
        /// Maps a menu to an action
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        #endregion

        public MenuDetailsModel MenuActionMapping(String EncryptedID)
        {
            return menuDetails.MenuActionMapping(EncryptedID);
        }

        /// <summary>
        /// returns Controller List
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsModel ControllerList(MenuDetailsModel menuDetailsModel)
        {
            return menuDetails.ControllerList(menuDetailsModel);
        }

        /// <summary>
        /// returns Action List
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsModel ActionList(MenuDetailsModel menuDetailsModel)
        {
            return menuDetails.ActionList(menuDetailsModel);
        }

        /// <summary>
        /// Maps MenuToAction
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsModel MapMenuToAction(MenuDetailsModel menuDetailsModel)
        {
            return menuDetails.MapMenuToAction(menuDetailsModel);
        }

        /// <summary>
        /// MapUnmapMenuActionButton
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsModel MapUnmapMenuActionButton(MenuDetailsModel menuDetailsModel)
        {
            return menuDetails.MapUnmapMenuActionButton(menuDetailsModel);
        }

        /// <summary>
        /// UnmapMenuToAction
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsModel UnmapMenuToAction(MenuDetailsModel menuDetailsModel)
        {
            return menuDetails.UnmapMenuToAction(menuDetailsModel);
        }
    }
}
