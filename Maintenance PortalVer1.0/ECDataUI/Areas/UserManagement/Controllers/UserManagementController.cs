#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   UserManagementController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for User Management module.
*/
#endregion

using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Areas.UserManagement.Controllers
{
    [KaveriAuthorizationAttribute]
    public class UserManagementController : Controller
    {
        /// <summary>
        /// WorkFlowDetails
        /// </summary>
        /// <returns></returns>
        public ActionResult WorkFlowDetails() {
            try
            {
                //added by akash(14-06-2018) To reset horizontal menu's
                KaveriSession.Current.KioskTokenID = 0;
                KaveriSession.Current.ParentMenuId = 0;//To Display MenuBar(Horizontal) Properly.

                KaveriSession.Current.ParentMenuId = (int)CommonEnum.ParentMenuIdEnum.WorkFlow;
                return View();
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Work Flow Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// RoleMenuActionDetails
        /// </summary>
        /// <returns></returns>
        public ActionResult RoleMenuActionDetails()
        {
            try
            {
                //added by akash(14-06-2018) To reset horizontal menu's
                KaveriSession.Current.KioskTokenID = 0;
                KaveriSession.Current.ParentMenuId = 0;//To Display MenuBar(Horizontal) Properly.

                KaveriSession.Current.ParentMenuId = (int)CommonEnum.ParentMenuIdEnum.RoleMenu;

                // Added BY SB on 2-04-2019 to active link clicked
                // commented below line BY SB on 6-04-2019 
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.RoleMenuActionDetails;

                return View();
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Role Menu Action Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

        /// <summary>
        /// OfficeUserDetails
        /// </summary>
        /// <returns></returns>
        public ActionResult OfficeUserDetails()
        {
            try
            {
                //added by akash(14-06-2018) To reset horizontal menu's
                KaveriSession.Current.KioskTokenID = 0;
                KaveriSession.Current.ParentMenuId = 0;//To Display MenuBar(Horizontal) Properly.

                KaveriSession.Current.ParentMenuId = (int)CommonEnum.ParentMenuIdEnum.OfficeUserDetails;

                // Added BY SB on 2-04-2019 to active link clicked
                // commented below line BY SB on 6-04-2019 
                //KaveriSession.Current.SubParentMenuToBeActive = (int)CommonEnum.SubParentMenuToBeActive.OfficeUserDetails;
                return View();
            }
            catch (Exception e)
            {
                ExceptionLogs.LogException(e);
                return RedirectToAction("ShowErrorMessage", "Error", new { area = "", message = "Error occured while retreiving Office User Details View", URLToRedirect = "/Home/HomePage" });
            }
        }

    }
}