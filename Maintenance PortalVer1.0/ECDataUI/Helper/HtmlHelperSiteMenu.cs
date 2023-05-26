using ECDataUI.Common;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Helper
{
    public static class HtmlHelperSiteMenu
    {
        #region Properties
        private static bool IsFirstMenu = false;

        #endregion

        #region Methods

        /// <summary>
        /// SiteMenuAsUnorderedList
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="sequenceID"></param>
        /// <returns></returns>
        public static MvcHtmlString SiteMenuAsUnorderedList(this HtmlHelper helper, Int16 sequenceID, bool IsHorizontalMenu)
        {
            ////Added by m rafe on 12-June18
            //if (IsHorizontalMenu && KaveriSession.Current.KioskTokenID == 0)  // without any application do not load Horizontal menus
            //    return MvcHtmlString.Empty;
            if (IsHorizontalMenu && KaveriSession.Current.KioskTokenID == 0 && KaveriSession.Current.RoleID != (short)CommonEnum.RoleDetails.TechnicalAdmin && KaveriSession.Current.RoleID != (short)CommonEnum.RoleDetails.DepartmentAdmin)  // without any application do not load Horizontal menus
                return MvcHtmlString.Empty;

            List<ISiteLink> siteLinks = SiteMenuManager.GetSiteMenuItems(KaveriSession.Current.RoleID, IsHorizontalMenu);

            if (siteLinks == null || siteLinks.Count == 0)
                return MvcHtmlString.Empty;

            MvcHtmlString htmlMenus = null;
            if (IsHorizontalMenu)
                htmlMenus = MvcHtmlString.Create(BuildMenuItemsForListHoriZontal(siteLinks, IsHorizontalMenu));
            else
            {
                var topLevelParentId = SiteMenuManager.GetTopLevelParentId(siteLinks);
                htmlMenus = MvcHtmlString.Create(BuildMenuItemsForList(siteLinks, topLevelParentId, IsHorizontalMenu));
            }
            return htmlMenus;
        }

        /// <summary>
        /// buildMenuItemsForList
        /// </summary>
        /// <param name="siteLinks"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static string BuildMenuItemsForList(List<ISiteLink> siteLinks, int parentId, bool IsHorizontalMenu)
        {
            IsFirstMenu = true;
            var parentTag = new TagBuilder("ul");

            string ulClass = IsHorizontalMenu ? "list-inline" : "sidebar-menu tree";

            parentTag.MergeAttribute("class", ulClass);
            //  parentTag.MergeAttribute("style", "margin-bottom:3px");



            var childSiteLinks = SiteMenuManager.GetChildSiteLinks(siteLinks, parentId);

            List<ISiteLink> MenuList1 = null;
            List<ISiteLink> MenuList2 = null;

            bool IsLoadServiceSubMenus = true;
            if (!IsHorizontalMenu)
            {
                //1. Fills Home Menu here
                foreach (var siteLink in childSiteLinks.Where(c => c.Sequence == 0))
                {

                    //******************** For HOME PAGE *************************
                    var innerTag = new TagBuilder("li");
                    innerTag.MergeAttribute("class", "treeview");
                    innerTag.MergeAttribute("style", "border:solid 1px #E5E4E2;font-size:15px");


                    var listActionLink = new TagBuilder("i");
                    //    var listActionLinkNew = new TagBuilder("i");
                    listActionLink.MergeAttribute("class", "glyphicon glyphicon-home");
                    innerTag.MergeAttribute("id", "liHome");
                    innerTag.MergeAttribute("style", "padding-right:15px");

                    //  innerTag.MergeAttribute("style", "background-color:aliceblue");


                    var treeAction = new TagBuilder("i");
                    var anchorTag = new TagBuilder("a");
                    anchorTag.MergeAttribute("href", "/Home/HomePage");
                    var spanTag = new TagBuilder("span");

                    //Commented below line by shubham bhagat for kaveri menus on 31 - 5 - 2019
                    //spanTag.MergeAttribute("style", "font-weight: bold;");

                    spanTag.SetInnerText(siteLink.Text);
                    anchorTag.InnerHtml = listActionLink.ToString() + "&nbsp;&nbsp;" + spanTag.ToString() + treeAction.ToString();
                    innerTag.InnerHtml = anchorTag.ToString();
                    if (SiteMenuManager.SiteLinkHasChildren(siteLinks, siteLink.MenuId))
                        innerTag.InnerHtml += BuildMenuItems(siteLinks, siteLink.MenuId);
                    parentTag.InnerHtml += innerTag;
                }
            }




            //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.SR)
            //{
            //    MenuList1 = childSiteLinks.OrderBy(o => o.HorizontalSequence).Where(c => c.IsHorizontalMenu == false && c.Sequence != 0 && (c.MenuId == KaveriSession.Current.TopMostParentMenuId || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.Reports) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.AdditionalFeatures) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UserDetails) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.Dashboard)) && (c.ModuleID == (KaveriSession.Current.ModuleID == null ? 0 : KaveriSession.Current.ModuleID)) && IsLoadServiceSubMenus).ToList();
            //}
            //else
            //{
            // Commented below line by shubham bhagat for kaveri menus on 20-3-2019
            //MenuList1 = childSiteLinks.OrderBy(o => o.HorizontalSequence).Where(c => c.IsHorizontalMenu == false && c.Sequence != 0 && (c.MenuId == KaveriSession.Current.TopMostParentMenuId || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.Reports) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.AdditionalFeatures) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UserDetails) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.Dashboard) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UserDetails_Dashboard) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UserDetails_UserManager)) && (c.ModuleID == (KaveriSession.Current.ModuleID == 0 ? 0 : KaveriSession.Current.ModuleID)) && IsLoadServiceSubMenus).ToList();

            ////commented on 5 - 4 - 2019
            //MenuList1 = childSiteLinks.OrderBy(o => o.HorizontalSequence).Where(c => c.IsHorizontalMenu == false && c.Sequence != 0
            //&& (c.MenuId == KaveriSession.Current.TopMostParentMenuId
            //|| (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UserDetails)
            // || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UserDetails_UserManager))
            //&& (c.ModuleID == (KaveriSession.Current.ModuleID == 0 ? 0 : KaveriSession.Current.ModuleID)) && IsLoadServiceSubMenus).ToList();


            MenuList1 = childSiteLinks.OrderBy(o => o.HorizontalSequence).Where(c => c.IsHorizontalMenu == false && c.Sequence != 0
            && (c.MenuId == KaveriSession.Current.TopMostParentMenuId
           
            // COMMENTED BY RAFE ON 04-12-19
            // || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UserDetails) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UpdateyourProfile)
            // RAFE ON 26-11-19
            //||(c.MenuId == (int)CommonEnum.ParentMenuIdEnum.UserManagementForDeptAdmin) || (c.MenuId == (int)CommonEnum.ParentMenuIdEnum.MenuManagement)
            
            )
            && (c.ModuleID == (KaveriSession.Current.ModuleID == 0 ? 0 : KaveriSession.Current.ModuleID)) && IsLoadServiceSubMenus).ToList();

            //    }



            foreach (var siteLink in MenuList1) //Other than Home Menu
            {
                //*********** For Second Level Menus (eg-Firm) *********************
                var innerTag = new TagBuilder("li");
                innerTag.MergeAttribute("class", "nav nav-stacked");
                innerTag.MergeAttribute("style", "border:solid 1px #E5E4E2;font-size:15px;");

                var listActionLinkNew = new TagBuilder("i");

                var listActionLink = new TagBuilder("i");

                switch (siteLink.Text)
                {
                    //******* To assign specific ICON for PARENT MENU.*********
                    // Commented by shubham bhagat on 30-09-2019
                    //case "Firm":
                    //    innerTag.MergeAttribute("id", "liFirm");
                    //    listActionLink.MergeAttribute("class", "fa fa-university");
                    //    break;

                    //case "Certified Copy":
                    //    innerTag.MergeAttribute("id", "liCertifiedCopy");
                    //    listActionLink.MergeAttribute("class", "fa fa-book");
                    //    break;

                    //case "Additional Features":
                    //    innerTag.MergeAttribute("id", "liAdditionalFeatures");
                    //    listActionLink.MergeAttribute("class", "fa fa-tasks");
                    //    break;

                    case "Dashboard":
                        innerTag.MergeAttribute("id", "liDashboardView");
                        listActionLink.MergeAttribute("class", "fa fa-desktop");
                        break;

                    case "User Management":
                        innerTag.MergeAttribute("id", "liUserManagement");
                        listActionLink.MergeAttribute("class", "fa fa-users");
                        break;

                    case "User Details":
                        innerTag.MergeAttribute("id", "liUserDetails");
                        listActionLink.MergeAttribute("class", "fa fa-user");
                        break;

                    case "Reports":
                        innerTag.MergeAttribute("id", "liReports");
                        listActionLink.MergeAttribute("class", "fa fa-file");
                        break;

                    case "Log Analysis":
                        innerTag.MergeAttribute("id", "liLogAnalysis");
                        listActionLink.MergeAttribute("class", "fa fa-file");
                        break;

                    case "Remittance Diagnostics":
                        innerTag.MergeAttribute("id", "liRemittanceDiagnostics");
                        listActionLink.MergeAttribute("class", "fa fa-file");
                        break;

                    case "Menu Management":
                        innerTag.MergeAttribute("id", "liMenuManagement");
                        listActionLink.MergeAttribute("class", "fa fa-list-alt");
                        break;

                    case "Office Management":
                        innerTag.MergeAttribute("id", "liOfficeManagement");
                        listActionLink.MergeAttribute("class", "fa fa-building");
                        break;

                    case "Update your Profile":
                        innerTag.MergeAttribute("id", "liUpdateyourProfile");
                        listActionLink.MergeAttribute("class", "fa fa-user");
                        break;

                    case "MIS Reports":
                        innerTag.MergeAttribute("id", "liMISReports");
                        listActionLink.MergeAttribute("class", "fa fa-file");
                        break;

                    case "Kaveri Support":
                        innerTag.MergeAttribute("id", "liKaveriSupport");
                        listActionLink.MergeAttribute("class", "fa fa-ticket");
                        break;

                    // Added by shubham bhagat on 30-09-2019
                    case "Service Packs":
                        innerTag.MergeAttribute("id", "liServicePacks");
                        listActionLink.MergeAttribute("class", "fa fa-briefcase");
                        break;

                    // Added by shubham bhagat on 04-10-2019
                    case "Download Enclosures":
                        innerTag.MergeAttribute("id", "liDownloadEnclosures");
                        listActionLink.MergeAttribute("class", "fa fa-download");
                        break;

                    // Added by shubham bhagat on 04-10-2019
                    case "General Diagnostic":
                        innerTag.MergeAttribute("id", "liGeneralDiagnostic");
                        listActionLink.MergeAttribute("class", "fa fa-archive");
                        break;

                    // Added by shubham bhagat on 05-11-2019
                    case "Integration Status":
                        innerTag.MergeAttribute("id", "liIntegrationStatus");
                        listActionLink.MergeAttribute("class", "fa fa-archive");
                        break;
                }
                var treeAction = new TagBuilder("i");
                var anchorTag = new TagBuilder("a");
                anchorTag.MergeAttribute("href", siteLink.Url);
                var spanTag = new TagBuilder("span");
                //Commented below line by shubham bhagat for kaveri menus on 31 - 5 - 2019
                //spanTag.MergeAttribute("style", "font-weight: bold;");


                spanTag.SetInnerText(siteLink.Text);
                anchorTag.InnerHtml = listActionLink.ToString() + listActionLinkNew.ToString() + "&nbsp;&nbsp;" + spanTag.ToString() + treeAction.ToString();
                innerTag.InnerHtml = anchorTag.ToString();
                if (SiteMenuManager.SiteLinkHasChildren(siteLinks, siteLink.MenuId))
                    innerTag.InnerHtml += BuildChildMenuItemsRecursively(siteLinks, siteLink.MenuId);

                //if (SiteMenuManager.SiteLinkHasChildren(siteLinks.Where(x => x.IsHorizontalMenu == IsHorizontalMenu).ToList(), siteLink.MenuId))
                //    innerTag.InnerHtml += buildChildMenuItemsRecursively(siteLinks.Where(x => x.IsHorizontalMenu == IsHorizontalMenu).ToList(), siteLink.MenuId);
                parentTag.InnerHtml += innerTag;
            }

            // Commented below code by shubham bhagat for kaveri menus on 20-3-2019
            //List<int> ParentMenuIdList = new List<int>() {
            //(int)CommonEnum.ParentMenuIdEnum.DocumentRegistration,
            //(int)CommonEnum.ParentMenuIdEnum.MarriageRegistration,
            //(int)CommonEnum.ParentMenuIdEnum.EncumberanceSearch,
            //(int)CommonEnum.ParentMenuIdEnum.FirmRegistration,
            //(int)CommonEnum.ParentMenuIdEnum.CertifiedCopy,
            //(int)CommonEnum.ParentMenuIdEnum.Dissolution
            //};

            List<int> ParentMenuIdList = new List<int>() {
            (int)CommonEnum.ParentMenuIdEnum.EncumberanceSearch
            };

            if (ParentMenuIdList.Contains(KaveriSession.Current.ParentMenuId) || ParentMenuIdList.Contains(KaveriSession.Current.TopMostParentMenuId))
            {

                MenuList2 = childSiteLinks.OrderBy(o => o.HorizontalSequence).Where(c => c.IsHorizontalMenu == IsHorizontalMenu && c.Sequence == 2 && (c.ModuleID == KaveriSession.Current.ModuleID || c.ModuleID == 0)).ToList();
                foreach (var siteLink in MenuList2)
                {
                    //**************** For First Level Menus (other than HOME)*******************
                    var innerTag = new TagBuilder("li");
                    innerTag.MergeAttribute("class", "nav nav-stacked");
                    innerTag.MergeAttribute("style", "border:solid 2px #E5E4E2;font-size:15px");


                    var listActionLink = new TagBuilder("i");
                    var listActionLinkNew = new TagBuilder("i");
                    listActionLink.MergeAttribute("class", "fa fa-file ");
                    innerTag.MergeAttribute("id", "li" + siteLink.Text.Replace(" ", string.Empty));
                    var treeAction = new TagBuilder("i");
                    var anchorTag = new TagBuilder("a");
                    anchorTag.MergeAttribute("href", siteLink.Url);
                    var spanTag = new TagBuilder("span");
                    spanTag.SetInnerText(siteLink.Text);
                    anchorTag.InnerHtml = listActionLink.ToString() + listActionLinkNew.ToString() + spanTag.ToString() + treeAction.ToString();
                    innerTag.InnerHtml = anchorTag.ToString();
                    if (SiteMenuManager.SiteLinkHasChildren(siteLinks, siteLink.MenuId))
                        innerTag.InnerHtml += BuildMenuItems(siteLinks, siteLink.MenuId);
                    parentTag.InnerHtml += innerTag;
                }
            }
            return parentTag.ToString();
        }


        /// <summary>
        /// build MenuItems For Horizontal Menu's
        /// </summary>
        /// <param name="siteLinks"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static string BuildMenuItemsForListHoriZontal(List<ISiteLink> siteLinks, bool IsHorizontalMenu)
        {


            //***************** For Horizontal Menus ************************
            IsFirstMenu = true;
            var parentTag = new TagBuilder("ul");

            parentTag.MergeAttribute("class", "nav nav-pills");
            parentTag.MergeAttribute("id", "crumbs");
            //parentTag.MergeAttribute("style", "overflow:scroll;height:510px"); 

            siteLinks = siteLinks.Where(x => x.ParentId == KaveriSession.Current.ParentMenuId).ToList();

            int counter = 1;
            foreach (var siteLink in siteLinks) //Other than Home Menu
            {
                var innerTag = new TagBuilder("li");
                innerTag.MergeAttribute("class", "treeview");

                //******* To add ID for horizontal menu ***************
                string IdOfLiItem = siteLink.Text.Replace(" ", string.Empty);
                IdOfLiItem = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(IdOfLiItem);
                innerTag.MergeAttribute("id", IdOfLiItem);

                var listActionLink = new TagBuilder("i");
                var listActionLinkNew = new TagBuilder("i");

                if (KaveriSession.Current.FirmApplicationTypeID == (int)CommonEnum.FirmApplicationType.FirmRegistrationFilling)
                {
                    ////Added by Akash(03-09-2018) To skip specific Sub-Menu based on Document StatusID.
                    //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.OnlineUser)
                    //{
                    //    switch (siteLink.Text)
                    //    {
                    //        case "Fees and Receipt":
                    //            if (KaveriSession.Current.DocumentStatusID != (byte)CommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Firm Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Branch Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Support Documents":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Partner Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //            //To identify specific MENU based on its Label to take specific action for it.
                    //    }

                    //}

                    ////Added by Akash(03-09-2018) To skip specific Sub-Menu(Horizontal Menus) based on Document StatusID.
                    //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.SR)
                    //{
                    //    switch (siteLink.Text)
                    //    {
                    //        case "Fees and Receipt":
                    //            //if (KaveriSession.Current.DocumentStatusID != (byte)CommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment)
                    //            //{
                    //            //    continue;
                    //            //}
                    //            break;
                    //        case "Firm Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Branch Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Support Documents":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Partner Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Check Slip":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)
                    //            {
                    //                continue;
                    //            }
                    //            break;

                    //        case "Summary":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.ApprovedFirms)
                    //            {
                    //                continue;
                    //            }
                    //            break;


                    //            //To identify specific MENU based on its Label to take specific action for it.
                    //    }

                    //}
                }
               else if (KaveriSession.Current.FirmApplicationTypeID == (int)CommonEnum.FirmApplicationType.FirmDissolutionFilling)
                {
                    ////Added by Akash(03-09-2018) To skip specific Sub-Menu based on Document StatusID.
                    //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.OnlineUser)
                    //{
                    //    switch (siteLink.Text)
                    //    {
                    //        case "Fees and Receipt":
                    //            if (KaveriSession.Current.DocumentStatusID != (byte)CommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                          
                          
                    //        case "Dissolution Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.DissolutionAcceptedForPayment || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.RegisteredDissolutionApplication || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmDissolutionAccepted || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmDissolutioForwardedtoSR || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.PaymentDoneForDissolutionApplication)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Support Documents":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.DissolutionAcceptedForPayment || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.RegisteredDissolutionApplication || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmDissolutionAccepted || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmDissolutioForwardedtoSR || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.PaymentDoneForDissolutionApplication )
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //            //To identify specific MENU based on its Label to take specific action for it.
                    //    }

                    //}

                    ////Added by Akash(03-09-2018) To skip specific Sub-Menu(Horizontal Menus) based on Document StatusID.
                    //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.SR)
                    //{
                    //    switch (siteLink.Text)
                    //    {
                    //        case "Fees and Receipt":
                    //            //if (KaveriSession.Current.DocumentStatusID != (byte)CommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment)
                    //            //{
                    //            //    continue;
                    //            //}
                    //            break;
                    //        case "Dissolution Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.RegisteredDissolutionApplication || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmDissolutionAccepted || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.PaymentDoneForDissolutionApplication)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Support Documents":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.RegisteredDissolutionApplication || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmDissolutionAccepted || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.PaymentDoneForDissolutionApplication)
                    //            {
                    //                continue;
                    //            }
                    //            break;

                    //        case "Check Slip":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.RegisteredDissolutionApplication || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmDissolutionAccepted || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.PaymentDoneForDissolutionApplication)
                    //            {
                    //                continue;
                    //            }
                    //            break;

                    //        case "Summary":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.RegisteredDissolutionApplication || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmDissolutionAccepted)
                    //            {
                    //                continue;
                    //            }
                    //            break;


                    //            //To identify specific MENU based on its Label to take specific action for it.
                    //    }

                    //}
                }
               else if (KaveriSession.Current.FirmApplicationTypeID == (int)CommonEnum.FirmApplicationType.FirmAmendmentFilling)
                {
                    ////Added by Akash(03-09-2018) To skip specific Sub-Menu based on Document StatusID.
                    //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.OnlineUser)
                    //{
                    //    switch (siteLink.Text)
                    //    {
                    //        case "Fees and Receipt":
                    //            if (KaveriSession.Current.DocumentStatusID != (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentApplicationApprovedForPayment)
                    //            {
                    //                continue;
                    //            }
                    //            break;


                    //        case "Amendment Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentApplicationApprovedForPayment || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmended || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentCertificateHasbeenIssued || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentForwardedtoSR || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentDoneforAmendment)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Support Documents":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentApplicationApprovedForPayment || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmended || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentCertificateHasbeenIssued || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentForwardedtoSR || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentDoneforAmendment)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //            //To identify specific MENU based on its Label to take specific action for it.
                    //    }

                    //}

                    //Added by Akash(03-09-2018) To skip specific Sub-Menu(Horizontal Menus) based on Document StatusID.
                    //if (KaveriSession.Current.RoleID == (short)CommonEnum.RoleDetails.SR)
                    //{
                    //    switch (siteLink.Text)
                    //    {
                  
                    //        case "Amendment Details":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmended || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentCertificateHasbeenIssued || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentDoneforAmendment)
                    //            {
                    //                continue;
                    //            }
                    //            break;
                    //        case "Support Documents":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmended || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentCertificateHasbeenIssued || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentDoneforAmendment)
                    //            {
                    //                continue;
                    //            }
                    //            break;

                    //        case "Check Slip":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmended || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentCertificateHasbeenIssued || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentDoneforAmendment)
                    //            {
                    //                continue;
                    //            }
                    //            break;

                    //        case "Summary":
                    //            if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmended || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.FirmAmendmentCertificateHasbeenIssued)
                    //            {
                    //                continue;
                    //            }
                    //            break;


                    //            //To identify specific MENU based on its Label to take specific action for it.
                    //    }

                    //}
                }




                var treeAction = new TagBuilder("i");

                var anchorTag = new TagBuilder("a");
                anchorTag.MergeAttribute("href", siteLink.Url);
                var spanTag = new TagBuilder("span");
                spanTag.SetInnerText(siteLink.Text);

                TagBuilder SpanTagForNumbering = new TagBuilder("span");

                ////Added by Akash(04-09-2018) ,To not display Count in Menu(Horizontal) in case of only 1 menu.
                //if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.OnlinePaymentIsDone || KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.Registered)
                //    SpanTagForNumbering.SetInnerText("");
                //else
                    SpanTagForNumbering.SetInnerText(counter++.ToString() + ".");


                //Added by Akash(18-09-2018) For Certified Copy.
                if (KaveriSession.Current.ServiceID == (short)CommonEnum.enumServiceTypes.CertifiedCopies)
                {
                    //if (KaveriSession.Current.DocumentStatusID == (byte)CommonEnum.DocumentStatusTypes.CC_OnlineDataEntryIsInProgress)
                    //{
                        SpanTagForNumbering.SetInnerText("");

                    //}
                }


                SpanTagForNumbering.MergeAttribute("class", "numberingCls");

                anchorTag.InnerHtml = SpanTagForNumbering .ToString()+ listActionLink.ToString() + listActionLinkNew.ToString() + spanTag.ToString() + treeAction.ToString();
                innerTag.InnerHtml = anchorTag.ToString();
                if (SiteMenuManager.SiteLinkHasChildren(siteLinks, siteLink.MenuId))
                    innerTag.InnerHtml += BuildChildMenuItemsRecursively(siteLinks, siteLink.MenuId);

                //if (SiteMenuManager.SiteLinkHasChildren(siteLinks.Where(x => x.IsHorizontalMenu == IsHorizontalMenu).ToList(), siteLink.MenuId))
                //    innerTag.InnerHtml += buildChildMenuItemsRecursively(siteLinks.Where(x => x.IsHorizontalMenu == IsHorizontalMenu).ToList(), siteLink.MenuId);
                parentTag.InnerHtml += innerTag;
            }

            return parentTag.ToString();
        }




        /// <summary>
        /// buildMenuItems
        /// </summary>
        /// <param name="siteLinks"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private static string BuildMenuItems(List<ISiteLink> siteLinks, int parentId)
        {

            string menu = string.Empty;
            StringBuilder strBuilder = new StringBuilder();
            var childSiteLinks = SiteMenuManager.GetChildSiteLinks(siteLinks, parentId);
            var parentTag = new TagBuilder("ul");
            parentTag.MergeAttribute("class", "treeview-menu");

            if (IsFirstMenu)
            {
                parentTag.MergeAttribute("style", "display:block");
                IsFirstMenu = false;
            }
            StringBuilder innerTagAppend = new StringBuilder();
            foreach (var siteLink in childSiteLinks)
            {
                var innerTag = new TagBuilder("li");
                var childAction = new TagBuilder("i");
                var anchorTag = new TagBuilder("a");

                anchorTag.MergeAttribute("href", "javascript:void(0)");
                anchorTag.MergeAttribute("onclick", "return LoadPage('" + siteLink.Url + "')");
                switch (siteLink.Text)
                {
                    case "General Information":
                        anchorTag.MergeAttribute("id", "aGeneralInformation");
                        break;
                    case "Property Details":
                        anchorTag.MergeAttribute("id", "aPropertyDetails");
                        break;
                    case "Stamp Duty Calculation":
                        anchorTag.MergeAttribute("id", "aStampDutyDetails");
                        break;
                    case "Party Details":
                        anchorTag.MergeAttribute("id", "aPartyDetails");
                        break;
                    case "Witness Details":
                        anchorTag.MergeAttribute("id", "aWitnessDetails");
                        break;
                    case "Consideration Payment":
                        anchorTag.MergeAttribute("id", "aConsiderationPaymentDetails");
                        break;
                    case "Stamp Duty Payment":
                        anchorTag.MergeAttribute("id", "aStampDutyPaymentDetails");
                        break;
                    case "Enclosures":
                        anchorTag.MergeAttribute("id", "aEnclosureDetails");
                        break;
                }
                anchorTag.InnerHtml = childAction.ToString();
                anchorTag.InnerHtml += siteLink.Text;
                innerTag.InnerHtml = anchorTag.ToString();
                innerTagAppend.Append(innerTag.ToString());
            }
            parentTag.InnerHtml = innerTagAppend.ToString();
            menu = menu + parentTag.ToString();
            return menu;
        }

        /// <summary>
        /// buildChildMenuItemsRecursively
        /// </summary>
        /// <param name="siteLinks"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        private static string BuildChildMenuItemsRecursively(List<ISiteLink> siteLinks, int menuId)
        {
            var parentTag = new TagBuilder("ul");
            parentTag.MergeAttribute("class", "sidebar-menu tree SubMenuClass");
            //parentTag.MergeAttribute("style", "display:block;");

            parentTag.MergeAttribute("style", "display:block;padding-left:28px;");

            // changed by m rafe on 26-11-19
            //parentTag.MergeAttribute("style", "display:block;padding-left:28px;padding-bottom: 15%;");

            var childSiteLinks = SiteMenuManager.GetChildSiteLinks(siteLinks, menuId);
            foreach (var siteLink in childSiteLinks)
            {
                // **************** For Child Menus in vertical MenuBar(eg- Registration,Dissolution etc.) ******************* 
                var itemTag = new TagBuilder("li");
                itemTag.MergeAttribute("class", "sub-nav");

                // ADDED BY SHUBHAM BHAGAT ON 31-05-2019
                // ADDED BY SHUBHAM BHAGAT ON 10-06-2019
                if (menuId == (int)CommonEnum.SubParentMenuToBeActive.MISReports)
                    itemTag.MergeAttribute("style", "padding-top:0px;font-size:13px;list-style-type: none;width:175%;");
                else
                    itemTag.MergeAttribute("style", "padding-top:0px;font-size:13px;list-style-type: none;");
                // to split menu name in 2-3 lines.
                // itemTag.MergeAttribute("style", "padding-top:0px;font-size:13px;list-style-type: none;white-space:normal !important;");

                var anchorTag = new TagBuilder("a");
                var iTag = new TagBuilder("i");

                // ADDED BY SHUBHAM BHAGAT ON 31-05-2019
                //iTag.MergeAttribute("class", "fa fa-dot-circle-o");
                iTag.MergeAttribute("class", "fa fa-newspaper-o");

                //iTag.MergeAttribute("class", "fa fa-chevron-right");


                //commented by akash(13-06-2018)
                //"****************  To set ID for "Firm Registartion" so that it can be triggerd on PAGE LOAD **************************
                //switch (siteLink.ModuleID)
                //{    
                //    case (int)CommonEnum.ParentMenuIdEnum.FirmRegistration:
                //        anchorTag.MergeAttribute("id", "firmRegistationMenu");
                //        break;
                //}


                itemTag.MergeAttribute("id", "li" + siteLink.Text.Replace(" ", string.Empty)); //To set "ID" attribute for menus.(ID will be it's Label without space e.g- "FirmDetails") 
                var treeAction = new TagBuilder("i");
                if (SiteMenuManager.SiteLinkHasChildren(siteLinks, siteLink.MenuId))
                {
                    treeAction.MergeAttribute("class", "fa fa-caret-down pull-right");
                    anchorTag.MergeAttribute("href", "javascript:void(0)");
                }
                else
                {
                    anchorTag.MergeAttribute("href", "javascript:void(0)");
                    anchorTag.MergeAttribute("onclick", "return LoadPage('" + siteLink.Url + "')");
                }
                // ADDED BY SHUBHAM BHAGAT ON 31-05-2019
                anchorTag.MergeAttribute("data-toggle", "tooltip");
                anchorTag.MergeAttribute("title", siteLink.Text);

                var spanTag = new TagBuilder("span");
                spanTag.SetInnerText(siteLink.Text);
                anchorTag.InnerHtml = iTag.ToString() + "&nbsp;&nbsp;" + spanTag.ToString() + treeAction.ToString();
                itemTag.InnerHtml = anchorTag.ToString();
                if (SiteMenuManager.SiteLinkHasChildren(siteLinks, siteLink.MenuId))
                    itemTag.InnerHtml += BuildChildMenuItemsRecursively(siteLinks, siteLink.MenuId);
                parentTag.InnerHtml += itemTag;
            }

            // ADDED BY SHUBHAM BHAGAT ON 31-05-2019
            // TO ADD SPACE TO SCROLLBAR AND LIST ELEMENTS
            //parentTag.InnerHtml = parentTag.InnerHtml + "<li><a><span></span></a></li><li><a><span></span></a></li><li><a><span></span></a></li><li><a><span></span></a></li><li><a><span></span></a></li><li><a><span></span></a></li><li><a><span></span></a></li><li><a><span></span></a></li>";
            // ADDED If Condition BY SHUBHAM BHAGAT ON 12-06-2019

            // rafe commented 26-11-19
//            if (menuId != (int)CommonEnum.ParentMenuIdEnum.UserManagementForDeptAdmin)
              //  parentTag.InnerHtml = parentTag.InnerHtml + "<li><br/></li><li><br/></li><li><br/></li><li><br/></li><li><br/></li><li><br/></li><li><br/></li><li><br/></li><li><br/></li><li><br/></li><li><br/></li><li><br/></li>";
            
            return parentTag.ToString();
        }
        #endregion
    }
}