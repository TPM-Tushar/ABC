using CustomModels.Models.MenuHelper;
using ECDataUI.Common;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ECDataUI.Helper
{
    public class SiteMenuManager
    {
        #region Methods

        /// <summary>
        /// GetSiteMenuItems
        /// </summary>
        /// <param name="roleID"></param>
        /// <returns></returns>
        public static List<ISiteLink> GetSiteMenuItems(short roleID, bool IsHorizontalMenu)
        {
            var menus = new List<ISiteLink>();
            List<MenuItems> menuList = new List<MenuItems>();
            long userId = KaveriSession.Current.UserID;

            string errormsg = string.Empty;
            ServiceCaller caller = new ServiceCaller("MenuHelperApiController");

            menuList = caller.GetCall<List<MenuItems>>("PopulateMenu", new { roleID = roleID, userId = userId }, out errormsg);
            StringBuilder strBuilder = null;
            if (menuList.Count > 0)
            {
                menuList = menuList.Where(x => x.IsHorizontalMenu == IsHorizontalMenu).ToList();
                foreach (var menu in menuList)
                {
                    strBuilder = new StringBuilder();
                    if ((menu.IntMenuParentId == 0 && menu.IntMenuSeqNo != 2) || (string.IsNullOrEmpty(menu.strAreaName) && string.IsNullOrEmpty(menu.StrController) && string.IsNullOrEmpty(menu.StrAction)))
                        strBuilder.Append("#");
                    else
                    {
                        strBuilder.Append("/");
                        strBuilder.Append(string.IsNullOrEmpty(menu.strAreaName) ? string.Empty : menu.strAreaName.Trim());
                        strBuilder.Append(string.IsNullOrEmpty(menu.strAreaName) ? string.Empty : "/");
                        strBuilder.Append(menu.StrController.Trim());
                        strBuilder.Append("/");
                        strBuilder.Append(menu.StrAction.Trim());
                        if (menu.isMenuIDParameter)
                        {
                            strBuilder.Append("?MenuID=");
                            strBuilder.Append(menu.IntMenuId);
                        }
                    }

                    string errorMessage = string.Empty;

                    menus.Add(new SiteMenuItem
                    {
                        MenuId = menu.IntMenuId,
                        ParentId = menu.IntMenuParentId,
                        Text = menu.StrMenuName,
                        Url = strBuilder.ToString(),
                        Sequence = menu.IntMenuSeqNo,
                        VerticalLevel = menu.HorizontalSequence,
                        HorizontalSequence = menu.HorizontalSequence,
                        ModuleID = menu.IntModuleId,
                        IsHorizontalMenu=menu.IsHorizontalMenu
                    });

                }
            }
            return menus;
        }

        /// <summary>
        /// GetTopLevelParentId
        /// </summary>
        /// <param name="siteLinks"></param>
        /// <returns></returns>
        public static int GetTopLevelParentId(IEnumerable<ISiteLink> siteLinks)
        {
            return siteLinks.Min(l => l.ParentId);
        }

        /// <summary>
        /// SiteLinkHasChildren
        /// </summary>
        /// <param name="siteLinks"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool SiteLinkHasChildren(IEnumerable<ISiteLink> siteLinks, int id)
        {
            return siteLinks.Any(i => i.ParentId == id);
        }

        /// <summary>
        /// GetChildSiteLinks
        /// </summary>
        /// <param name="siteLinks"></param>
        /// <param name="parentIdForChildren"></param>
        /// <returns></returns>
        public static IEnumerable<ISiteLink> GetChildSiteLinks(IEnumerable<ISiteLink> siteLinks, int parentIdForChildren)
        {
            return siteLinks.Where(i => i.ParentId == parentIdForChildren).OrderBy(i => i.VerticalLevel).ThenBy(i => i.Sequence).ThenBy(i => i.HorizontalSequence);
        }

        #endregion
    }



}