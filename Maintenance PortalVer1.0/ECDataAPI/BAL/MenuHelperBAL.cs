using CustomModels.Models.MenuHelper;
using ECDataAPI.DAL;
using ECDataAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.BAL
{
    public class MenuHelperBAL : IMenuHelper
    {
        IMenuHelper objDAL = new MenuHelperDAL();
        public List<MenuItems> PopulateMenu(Int16 roleID, long userId)
        {
            return objDAL.PopulateMenu(roleID, userId);
        }


        /// <summary>
        /// REturn Submenu detail
        /// </summary>
        /// <param name="ParentMenuID"></param>
        /// <returns></returns>
        public MenuItems GetSubMenuDetails(int ParentMenuID, int RoleID)
        {
            return objDAL.GetSubMenuDetails(ParentMenuID, RoleID);
             
        }
    }



}