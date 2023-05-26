using CustomModels.Models.MenuHelper;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using ECDataAPI.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.DAL
{
    public class MenuHelperDAL : IMenuHelper
    {
        /// <summary>
        /// PupulateMenu
        /// </summary>Pupulate Menu
        /// <param name="roleID"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<MenuItems> PopulateMenu(Int16 roleID, long userId)
        {
            KaveriEntities dbContext = null;
            List<MenuItems> menuListReturn = new List<MenuItems>();

            try
            {
                dbContext = new KaveriEntities();
                List<USP_PopulateMenus_Result> menuList = dbContext.USP_PopulateMenus(roleID).ToList<USP_PopulateMenus_Result>();

                
                MenuItems[] arrMenuItems = new MenuItems[menuList.Count];
                for (int i = 0; i < menuList.Count; i++)
                {
                    arrMenuItems[i] = new MenuItems();
                    arrMenuItems[i].BoolStatus = menuList[i].BoolStatus;
                    arrMenuItems[i].HorizontalSequence = menuList[i].HorizontalSequence;
                    arrMenuItems[i].IntMenuId = menuList[i].IntMenuId;
                    arrMenuItems[i].IntMenuLevel = menuList[i].IntMenuLevel;
                    arrMenuItems[i].IntMenuParentId = menuList[i].IntMenuParentId;
                    arrMenuItems[i].IntMenuSeqNo = menuList[i].IntMenuSeqNo;
                    arrMenuItems[i].StrAction = menuList[i].StrAction;
                    arrMenuItems[i].strAreaName = menuList[i].strAreaName;
                    arrMenuItems[i].StrController = menuList[i].StrController;
                    arrMenuItems[i].StrMenuName = menuList[i].StrMenuName;
                    arrMenuItems[i].isMenuIDParameter = menuList[i].IsMenuIDParameter == null ? false : (bool)menuList[i].IsMenuIDParameter;
                    arrMenuItems[i].IsHorizontalMenu = menuList[i].IsHorizontalMenu;

                    arrMenuItems[i].IntModuleId = menuList[i].ModuleID == null ? 0 : menuList[i].ModuleID;
                    menuListReturn.Add(arrMenuItems[i]);
                }

                return menuListReturn;
                // On 12-07-2019 At 12:30 PM 
                // below code is commented by Akash and Shubham because menu with 69 MenuID is skipping above return statement is added.
                //Int16 iOfficeTypeIDDR = Convert.ToInt16(ApiCommonEnum.OfficeTypes.SRO);
                //int iMenuIDDROrderEntry = Convert.ToInt32(ApiCommonEnum.MenuDetails.DROrderEntry);
                
                ////remove menu if not applicable 
                //short userOfficeId = dbContext.UMG_UserDetails.Where(z => z.UserID == userId).Select(z => z.OfficeID).FirstOrDefault();
                //bool IsDRUser = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == userOfficeId && x.OfficeTypeID == iOfficeTypeIDDR).Count() == 0 ? false : true;
                //if (IsDRUser)
                //    return menuListReturn;
                //else
                //{
                //    //Removed DR Order Entry menu if the user is not dr user.
                //    menuListReturn.Remove(menuListReturn.Where(x => x.IntMenuId == iMenuIDDROrderEntry).FirstOrDefault());
                //}
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }




        /// <summary>
        /// REturn Submenu detail
        /// </summary>
        /// <param name="ParentMenuID"></param>
        /// <returns></returns>
        public MenuItems GetSubMenuDetails(int ParentMenuID, int RoleID)
        {
            KaveriEntities dbContext = null;
            try
            {
                dbContext = new KaveriEntities();
                var subMenuDetails = (from menuDetails in dbContext.UMG_MenuDetails
                                      join menuActionMapping in dbContext.UMG_MenuActionMapping
                                      on menuDetails.MenuID equals menuActionMapping.MenuID
                                      join controllerActionDetails in dbContext.UMG_ControllerActionDetails
                                      on menuActionMapping.CAID equals controllerActionDetails.CAID
                                      join roleMenuMapping in dbContext.UMG_RoleMenuMapping
                                      on menuDetails.MenuID equals roleMenuMapping.MenuID
                                      where menuDetails.ParentID == ParentMenuID && menuDetails.IsActive == true && roleMenuMapping.RoleID == RoleID
                                      orderby menuDetails.Sequence
                                      select new MenuItems()
                                      {
                                          IntMenuId = menuDetails.MenuID,
                                          IntMenuParentId = menuDetails.ParentID,
                                          StrMenuName = menuDetails.MenuName,
                                          strAreaName = controllerActionDetails.AreaName,
                                          StrController = controllerActionDetails.ControllerName,
                                          StrAction = controllerActionDetails.ActionName,
                                          isMenuIDParameter = menuDetails.IsMenuIDParameter == null ? false : (bool)menuDetails.IsMenuIDParameter
                                      }
                                ).FirstOrDefault();

                return subMenuDetails;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
    }
}