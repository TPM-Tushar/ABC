
#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   HomePageDAL.cs
    * Author Name       :   Akash Patil
    * Creation Date     :   08-06-2018
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   DAL layer to retreive all Home Page related details.
*/
#endregion




#region references
using CustomModels.Models.HomePage;
using CustomModels.Models.MenuHelper;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using ECDataAPI.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using CustomModels.Models.Alerts;
using CustomModels.Security;
using System.Security;

#endregion
namespace ECDataAPI.DAL
{
    public class HomePageDAL : IHomePage
    {



        ///// <summary>
        ///// This method returns "HomePageModel" obj. which contains all info. about Menus for HomePage.
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //public HomePageModel GetHomePageDetails(LoadMenuModel model)
        //{
        //    KaveriEntities dbContext = null;

        //    HomePageModel responseModel = null;
        //    try
        //    {
        //        dbContext = new KaveriEntities();
        //        responseModel = new HomePageModel();
        //        responseModel.MenuListReturn = new List<MenuItems>();
        //        var menuList = dbContext.USP_PopulateMenus(model.RoleID).ToList<USP_PopulateMenus_Result>();
        //        menuList = menuList.Where(c => c.IsHorizontalMenu == false && c.IntMenuParentId == model.ParentMenuId && c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.AdditionalFeatures && c.IntMenuSeqNo != 0 && c.IntMenuSeqNo != (short)ApiCommonEnum.MenuDetails.Sequence2).ToList();
        //        // refinedMenuList = menuList.Where(c => c.IntMenuParentId == model.ParentMenuId && (!model.SequenceExcludeList.Contains(c.IntMenuSeqNo))).ToList();

        //        MenuItems MenuItemObj = null;
        //        for (int i = 0; i < menuList.Count; i++)
        //        {
        //            MenuItemObj = new MenuItems();
        //            MenuItemObj.BoolStatus = menuList[i].BoolStatus;
        //            MenuItemObj.HorizontalSequence = menuList[i].HorizontalSequence;
        //            MenuItemObj.IntMenuId = menuList[i].IntMenuId;
        //            MenuItemObj.IntMenuLevel = menuList[i].IntMenuLevel;
        //            MenuItemObj.IntMenuParentId = menuList[i].IntMenuParentId;
        //            MenuItemObj.IntMenuSeqNo = menuList[i].IntMenuSeqNo;
        //            MenuItemObj.StrAction = menuList[i].StrAction;
        //            MenuItemObj.strAreaName = menuList[i].strAreaName;
        //            MenuItemObj.StrController = menuList[i].StrController;
        //            MenuItemObj.StrMenuName = menuList[i].StrMenuName;
        //            MenuItemObj.isMenuIDParameter = menuList[i].IsMenuIDParameter == null ? false : (bool)menuList[i].IsMenuIDParameter;

        //            MenuItemObj.IntModuleId = menuList[i].ModuleID == null ? 0 : menuList[i].ModuleID;

        //            if (MenuItemObj.IntModuleId.HasValue)
        //            {
        //                MenuItems tempMenuItem = GetSubModulesStatistics(MenuItemObj.IntModuleId.Value, model);
        //                MenuItemObj.SubModuleList = tempMenuItem.SubModuleList;
        //                MenuItemObj.ModuleIcon = tempMenuItem.ModuleIcon;
        //            }
        //            if (MenuItemObj.SubModuleList == null) // **** to avoid exception at HomePage.cshtml ******
        //                MenuItemObj.SubModuleList = new List<SubModuleDetails>();


        //            responseModel.MenuListReturn.Add(MenuItemObj);
        //        }
        //        Int16 iOfficeTypeIDDR = Convert.ToInt16(ApiCommonEnum.OfficeTypes.SRO);
        //        int iMenuIDDROrderEntry = Convert.ToInt32(ApiCommonEnum.MenuDetails.DROrderEntry);


        //        UMG_UserProfile userObj = dbContext.UMG_UserProfile.Where(z => z.UserID == model.UserID).FirstOrDefault();

        //        responseModel.UserName = userObj.FirstName + " " + userObj.LastName;
        //        //remove menu if not applicable 
        //        short userOfficeId = dbContext.UMG_UserDetails.Where(z => z.UserID == model.UserID).Select(z => z.OfficeID).FirstOrDefault();
        //        bool IsSRUser = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == userOfficeId && x.OfficeTypeID == iOfficeTypeIDDR).Count() == 0 ? false : true;
        //        if (IsSRUser)
        //            return responseModel;
        //        else
        //        {
        //            //Removed DR Order Entry menu if the user is not dr user.
        //            responseModel.MenuListReturn.Remove(responseModel.MenuListReturn.Where(x => x.IntMenuId == iMenuIDDROrderEntry).FirstOrDefault());
        //            return responseModel;
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw e;
        //    }
        //    finally
        //    {
        //        if (dbContext != null)
        //            dbContext.Dispose();
        //    }

        //}


        /// <summary>
        /// This method returns "HomePageModel" obj. which contains all info. about Menus for HomePage.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public HomePageModel GetHomePageDetails(LoadMenuModel model)
        {
            KaveriEntities dbContext = null;

            HomePageModel responseModel = null;
            try
            {
                dbContext = new KaveriEntities();
                responseModel = new HomePageModel();
                responseModel.MenuListReturn = new List<MenuItems>();
                var menuList = dbContext.USP_PopulateMenus(model.RoleID).ToList<USP_PopulateMenus_Result>();

                ////For SR
                //if (model.RoleID == (short)ApiCommonEnum.RoleDetails.SR)
                //{
                //    menuList = menuList.Where(c => c.IsHorizontalMenu == false && c.IntMenuParentId == model.ParentMenuId && c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.AdditionalFeatures && c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails && c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.Dashboard && c.IntMenuSeqNo != 0 && c.IntMenuSeqNo != (short)ApiCommonEnum.MenuDetails.Sequence2).ToList();
                //}
                //else
                //{
                //For HOD & DR & Online User
                // Commented below line by shubham bhagat for kaveri menus on 20-3-2019
                //menuList = menuList.Where(c => c.IsHorizontalMenu == false && c.IntMenuParentId == model.ParentMenuId && c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.AdditionalFeatures && c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails && c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails_Dashboard && c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails_UserManager && c.IntMenuSeqNo != 0 && c.IntMenuSeqNo != (short)ApiCommonEnum.MenuDetails.Sequence2).ToList();

                //// Commented below line by shubham bhagat for kaveri menus on 04-04-2019
                //menuList = menuList.Where(c => c.IsHorizontalMenu == false && c.IntMenuParentId == model.ParentMenuId
                //&& c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails &&
                //c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails_UserManager &&
                //c.IntMenuSeqNo != 0 && c.IntMenuSeqNo != (short)ApiCommonEnum.MenuDetails.Sequence2).ToList();

                // Commented below line by shubham bhagat for kaveri menus on 05-04-2019
                //menuList = menuList.Where(c => c.IsHorizontalMenu == false && c.IntMenuParentId == model.ParentMenuId
                //&& c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails &&
                //c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails_UserManager &&
                //c.IntMenuSeqNo != 0 && c.IntMenuSeqNo != (short)ApiCommonEnum.MenuDetails.Sequence2 && c.SkipHomePage!=true).ToList();

                // Commented below line by shubham bhagat for kaveri menus on 04-04-2019
                menuList = menuList.Where(c => c.IsHorizontalMenu == false && c.IntMenuParentId == model.ParentMenuId

                // COMMENTED BY RAFE ON 04-12-19

                //&& c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserDetails && 
                //c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UpdateyourProfile 
                &&
                // rafe 26-11-19
                //c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.UserManagementForDeptAdmin &&
                //c.IntMenuId != (short)ApiCommonEnum.ParentMenuIdEnum.MenuManagement &&
                c.IntMenuSeqNo != 0 && c.IntMenuSeqNo != (short)ApiCommonEnum.MenuDetails.Sequence2).ToList();

                // }



                // refinedMenuList = menuList.Where(c => c.IntMenuParentId == model.ParentMenuId && (!model.SequenceExcludeList.Contains(c.IntMenuSeqNo))).ToList();

                MenuItems MenuItemObj = null;
                for (int i = 0; i < menuList.Count; i++)
                {
                    MenuItemObj = new MenuItems();
                    MenuItemObj.BoolStatus = menuList[i].BoolStatus;
                    MenuItemObj.HorizontalSequence = menuList[i].HorizontalSequence;
                    MenuItemObj.IntMenuId = menuList[i].IntMenuId;
                    MenuItemObj.IntMenuLevel = menuList[i].IntMenuLevel;
                    MenuItemObj.IntMenuParentId = menuList[i].IntMenuParentId;
                    MenuItemObj.IntMenuSeqNo = menuList[i].IntMenuSeqNo;
                    MenuItemObj.StrAction = menuList[i].StrAction;
                    MenuItemObj.strAreaName = menuList[i].strAreaName;
                    MenuItemObj.StrController = menuList[i].StrController;
                    MenuItemObj.StrMenuName = menuList[i].StrMenuName;
                    MenuItemObj.isMenuIDParameter = menuList[i].IsMenuIDParameter == null ? false : (bool)menuList[i].IsMenuIDParameter;

                    MenuItemObj.IntModuleId = menuList[i].ModuleID == null ? 0 : menuList[i].ModuleID;

                    // Added by shubham bhagat on 03-09-2019
                    MenuItemObj.ModuleIcon = menuList[i].MenuIcon;

                    // ADDED BY SHUBHAM BHAGAT ON 17-09-2019
                    MenuItemObj.ModuleDescription = menuList[i].MenuDesc;

                    // Commented by shubham bhagat on 03-09-2019
                    //if (MenuItemObj.IntModuleId.HasValue)
                    //{
                    //    MenuItems tempMenuItem = GetSubModulesStatistics(MenuItemObj.IntModuleId.Value, model);
                    //    MenuItemObj.SubModuleList = tempMenuItem.SubModuleList;
                    //    MenuItemObj.ModuleIcon = tempMenuItem.ModuleIcon;
                    //}

                    if (MenuItemObj.SubModuleList == null) // **** to avoid exception at HomePage.cshtml ******
                        MenuItemObj.SubModuleList = new List<SubModuleDetails>();


                    responseModel.MenuListReturn.Add(MenuItemObj);
                }
                Int16 iOfficeTypeIDDR = Convert.ToInt16(ApiCommonEnum.OfficeTypes.SRO);
                Int16 HODOfficeType = Convert.ToInt16(ApiCommonEnum.OfficeTypes.IGR);

                int iMenuIDDROrderEntry = Convert.ToInt32(ApiCommonEnum.MenuDetails.DROrderEntry);


                UMG_UserProfile userObj = dbContext.UMG_UserProfile.Where(z => z.UserID == model.UserID).FirstOrDefault();

                responseModel.UserName = userObj.FirstName + " " + userObj.LastName;
                //remove menu if not applicable 

                // Mobile verification check -- shrinivas

                if (userObj.IsMobileNumVerified)
                {
                    responseModel.IsMobileNumberVerfied = 1;
                }
                else
                {
                    responseModel.IsMobileNumberVerfied = 0;
                }

                // Mobile verification check -- shrinivas

                short userOfficeId = dbContext.UMG_UserDetails.Where(z => z.UserID == model.UserID).Select(z => z.OfficeID).FirstOrDefault();
                bool IsSRUser = dbContext.MAS_OfficeMaster.Where(x => x.OfficeID == userOfficeId && (x.OfficeTypeID == iOfficeTypeIDDR || x.OfficeTypeID == HODOfficeType)).Count() == 0 ? false : true;
                if (IsSRUser)
                    return responseModel;
                else
                {
                    //Removed DR Order Entry menu if the user is not dr user.
                    responseModel.MenuListReturn.Remove(responseModel.MenuListReturn.Where(x => x.IntMenuId == iMenuIDDROrderEntry).FirstOrDefault());
                    return responseModel;
                }
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



        #region For Module Buttons on HomePage 

        /// <summary>
        /// This method returns "MenuItems" obj. containing 1. Sub module list and 2. Module Icon class to display on Homepage
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public MenuItems GetSubModulesStatistics(int ModuleID, LoadMenuModel model)
        {

            try
            {
                MenuItems menuItem = new MenuItems();
                SubModuleDetails responseModel = null;

                responseModel = new SubModuleDetails();

                switch (ModuleID)
                {
                    case ((int)ApiCommonEnum.Modules.FirmRegistration):

                        //    menuItem.SubModuleList = getSubModulesStatisticsDetails(ModuleID, model);

                        menuItem.ModuleIcon = "fa fa-bank";

                        break;

                    case ((int)ApiCommonEnum.Modules.UserManager):

                        //    menuItem.SubModuleList = getSubModulesStatisticsDetails(ModuleID, model);

                        menuItem.ModuleIcon = "fa fa-users";

                        break;

                    case ((int)ApiCommonEnum.Modules.Dashboard):

                        //   menuItem.SubModuleList = getSubModulesStatisticsDetails(ModuleID, model);

                        menuItem.ModuleIcon = "fa fa-desktop";

                        break;


                    case ((int)ApiCommonEnum.Modules.Home):
                        //Code
                        break;

                    case ((int)ApiCommonEnum.Modules.CertifiedCopy):

                        menuItem.SubModuleList = getSubModulesStatisticsDetails(ModuleID, model);

                        menuItem.ModuleIcon = "fa fa-book";

                        break;

                    case ((int)ApiCommonEnum.Modules.SupportEnclosure):

                        menuItem.ModuleIcon = "fa fa-file";

                        break;

                    default:
                        //shubham bhagat on 30-08-2019
                        // Added by shubham bhagat on 30-08-2019 
                        menuItem.ModuleIcon = "fa fa-file";
                        break;
                }


                return menuItem;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method returns List<SubModuleDetails> which contains statistics about sub modules.
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<SubModuleDetails> getSubModulesStatisticsDetails(int ModuleID, LoadMenuModel model)
        {
            //********************** For Status List in Module Buttons *******************************
            try
            {
                List<SubModuleDetails> ModuleList = new List<SubModuleDetails>();
                KaveriEntities dbContext = new KaveriEntities();

                DateTime TodaysDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                //SubModuleDetails obj = null;
                switch (ModuleID)
                {
                    case ((int)ApiCommonEnum.Modules.FirmRegistration):

                        #region Firm-Registartion
                        if (model.RoleID == (short)ApiCommonEnum.RoleDetails.OnlineUser)
                        {
                            //********* Online User ***********************
                            // Commented By Shubham Bhagat on 13-04-2019 due to requirement change
                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Submitted For Approval",
                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval) ).Count()
                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Approved For Online Payment",

                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment || m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.OnlinePaymentIsDone) ).Count()

                            //};
                            //ModuleList.Add(obj);


                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Registerd & Digitally singed",
                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.Registered || m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApprovedFirms)).Count()

                            //};
                            //ModuleList.Add(obj);
                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Correction",
                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.Correction)).Count()

                            //};
                            //ModuleList.Add(obj);
                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Demo",
                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.Registered || m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.DigitallySigned)).Count()

                            //};
                            //ModuleList.Add(obj);
                        }
                        else
                        {
                            //************** SR ***********************
                            // Commented By Shubham Bhagat on 13-04-2019 due to requirement change
                            //obj = new SubModuleDetails()
                            //{

                            //    SubModuleName = "Submitted For Approval",
                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.OfficeID == model.OfficeID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval)).Count()// calculate if frm db after function calls

                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Approved For Online Payment",

                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.OfficeID == model.OfficeID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment || m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)).Count()// calculate if frm db after function calls

                            //};
                            //ModuleList.Add(obj);


                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Registerd",
                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.OfficeID == model.OfficeID && m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.Registered).Count()

                            //};
                            //ModuleList.Add(obj);

                            //     obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Digitally Singed",
                            //    Count = dbContext.FRM_FirmDetails.Where(m => m.OfficeID == model.OfficeID &&  m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApprovedFirms).Count()

                            //};
                            //ModuleList.Add(obj);
                        }
                        #endregion

                        break;

                    case ((int)ApiCommonEnum.Modules.CertifiedCopy):

                        #region Certified Copy

                        //                       if (model.RoleID == (short)ApiCommonEnum.RoleDetails.OnlineUser)
                        //                       {
                        //                           ////********* Online User ***********************
                        //                           obj = new SubModuleDetails()
                        //                         {
                        //                             SubModuleName = "CC Data Entry in Progress",
                        //                             Count = dbContext.CC_ApplicationDetails.Where(m => m.UserId == model.UserID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_OnlineDataEntryIsInProgress).Count()

                        //                         };
                        //                           ModuleList.Add(obj);
                        //                           obj = new SubModuleDetails()
                        //                           {
                        //                               SubModuleName = "CC Application Submitted",
                        //                               Count = dbContext.CC_ApplicationDetails.Where(m => m.UserId == model.UserID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_ApplicationSubmitted).Count()

                        //                           };
                        //                           ModuleList.Add(obj);

                        //                           obj = new SubModuleDetails()
                        //                           {
                        //                               SubModuleName = "Online Payment Done",
                        //                               Count = dbContext.CC_ApplicationDetails.Where(m => m.UserId == model.UserID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_OnlinePaymentDone).Count()

                        //                           };
                        //                           ModuleList.Add(obj);


                        //                           obj = new SubModuleDetails()
                        //                           {
                        //                               SubModuleName = "CC Application Accepted",
                        //                               Count = dbContext.CC_ApplicationDetails.Where(m => m.UserId == model.UserID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_ApplicationAccepted).Count()

                        //                           };
                        //                           ModuleList.Add(obj);
                        //                       }
                        //                       else
                        //                       {
                        //                           //********* SR ***********************

                        //                           obj = new SubModuleDetails()
                        //                           {
                        //                               SubModuleName = "CC Application Submitted",
                        //                               Count = dbContext.CC_ApplicationDetails.Where(m => m.CCIssueOfficeID == model.OfficeID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_ApplicationSubmitted).Count()

                        //                           };
                        //                           ModuleList.Add(obj);

                        //                           obj = new SubModuleDetails()
                        //                           {
                        //                               SubModuleName = "Online Payment Done",
                        //                               Count = dbContext.CC_ApplicationDetails.Where(m => m.CCIssueOfficeID == model.OfficeID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_OnlinePaymentDone).Count()

                        //                           };
                        //                           ModuleList.Add(obj);


                        //                           obj = new SubModuleDetails()
                        //                           {
                        //                               SubModuleName = "CC Application Accepted",
                        //                               Count = dbContext.CC_ApplicationDetails.Where(m => m.CCIssueOfficeID == model.OfficeID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_ApplicationAccepted).Count()

                        //                           };
                        //                           ModuleList.Add(obj);
                        //                       } 
                        #endregion

                        break;

                    default:
                        break;
                }
                return ModuleList;

            }
            catch (Exception)
            {
                throw;
            }



        }

        #endregion



        #region For SideBar Statistics on HomePage.


        /// <summary>
        /// This method returns "MenuItems" obj. containing 1. Sub module list and 2. Module Icon class to display on Homepage SideBar Statistics.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public MenuItems GetHomePageSideBarStatistics(LoadMenuModel model)
        {
            try
            {
                MenuItems menuItem = new MenuItems();
                SubModuleDetails responseModel = null;
                responseModel = new SubModuleDetails();
                switch (model.ModuleID)
                {
                    case ((int)ApiCommonEnum.Modules.FirmRegistration):
                        menuItem.SubModuleList = getSideBarStatistics(model.ModuleID, model);
                        menuItem.ModuleIcon = "fa fa-bank";
                        break;

                    case ((int)ApiCommonEnum.Modules.Home):
                        //Code
                        break;

                    case ((int)ApiCommonEnum.Modules.CertifiedCopy):
                        menuItem.SubModuleList = getSideBarStatistics(model.ModuleID, model);
                        menuItem.ModuleIcon = "fa fa-book";
                        break;

                    default:
                        break;
                }
                return menuItem;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// This method returns List<SubModuleDetails> which contains statistics about sub modules for Side Bar Statistics.
        /// </summary>
        /// <param name="ModuleID"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private List<SubModuleDetails> getSideBarStatistics(int ModuleID, LoadMenuModel model)
        {
            //********************** For Status List in Side-Bar (HomePage)*******************************
            model.MonthID = model.MonthID - 1;//To adjust index for Months List(Due to addition of "All-Months" in List )

            try
            {
                List<SubModuleDetails> ModuleList = new List<SubModuleDetails>();
                KaveriEntities dbContext = new KaveriEntities();

                //SubModuleDetails obj = null;
                switch (ModuleID)
                {

                    case ((int)ApiCommonEnum.Modules.FirmRegistration):

                        #region Firm Registartion

                        if (model.RoleID == (short)ApiCommonEnum.RoleDetails.OnlineUser)
                        {
                            // Commented By Shubham Bhagat on 13-04-2019 due to requirement change
                            //List<FRM_FirmDetails> firmApplicationList = null;
                            //if (model.MonthID != 0){
                            //    //******** For Specific Month in Year ****************** 
                            //    firmApplicationList = dbContext.FRM_FirmDetails.Where(c => c.UserID == model.UserID && c.ApplicationDate.Month == model.MonthID && c.ApplicationDate.Year == model.YearID).ToList();
                            //}
                            //else{                            
                            //    //******** For All Months in Year ****************** 
                            //    firmApplicationList = dbContext.FRM_FirmDetails.Where(c => c.UserID == model.UserID && c.ApplicationDate.Year == model.YearID).ToList();
                            //}


                            //************************ Online USer ***********************
                            // Commented By Shubham Bhagat on 13-04-2019 due to requirement change
                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Submitted For Approval",
                            //    Count = firmApplicationList.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval)).Count()// calculate if frm db after function calls
                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Approved For Online Payment",
                            //    Count = firmApplicationList.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment || m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)).Count()// calculate if frm db after function calls
                            //};
                            //ModuleList.Add(obj);


                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Correction",
                            //    Count = firmApplicationList.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.Correction)).Count()
                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Registerd",
                            //    Count = firmApplicationList.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.Registered)).Count()
                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Digitally Signed",
                            //    Count = firmApplicationList.Where(m => m.UserID == model.UserID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApprovedFirms)).Count()
                            //};
                            //ModuleList.Add(obj);

                        }
                        else
                        {
                            //******************************** SR ************************************
                            // Commented By Shubham Bhagat on 13-04-2019 due to requirement change
                            //List<FRM_FirmDetails> firmApplicationForSRList = null; //dbContext.FRM_FirmDetails.Where(c => c.OfficeID == model.OfficeID && c.ApplicationDate.Month == model.MonthID && c.ApplicationDate.Year == model.YearID).ToList();
                            //if (model.MonthID != 0)
                            //{
                            //    //******** For Specific Month in Year ****************** 
                            //    firmApplicationForSRList = dbContext.FRM_FirmDetails.Where(c => c.OfficeID == model.OfficeID && c.ApplicationDate.Month == model.MonthID && c.ApplicationDate.Year == model.YearID).ToList();
                            //}
                            //else
                            //{
                            //    //******** For All Months in Year ****************** 
                            //    firmApplicationForSRList = dbContext.FRM_FirmDetails.Where(c => c.OfficeID == model.OfficeID  && c.ApplicationDate.Year == model.YearID).ToList();
                            //}

                            //---------------------------------------------------------------------------------------------------------------------------------
                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Submitted For Approval",
                            //    Count = firmApplicationForSRList.Where(m => m.OfficeID == model.OfficeID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.OnlineApplicationSubmittedforApproval)).Count()// calculate if frm db after function calls
                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Approved For Online Payment",
                            //    Count = firmApplicationForSRList.Where(m => m.OfficeID == model.OfficeID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment || m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.OnlinePaymentIsDone)).Count()// calculate if frm db after function calls
                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Accepted",
                            //    Count = firmApplicationForSRList.Where(m => m.OfficeID == model.OfficeID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApplicationApprovedforOnlinePayment)).Count()
                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Registerd",
                            //    Count = firmApplicationForSRList.Where(m => m.OfficeID == model.OfficeID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.Registered)).Count()
                            //};
                            //ModuleList.Add(obj);

                            //obj = new SubModuleDetails()
                            //{
                            //    SubModuleName = "Digitally signed",
                            //    Count = firmApplicationForSRList.Where(m => m.OfficeID == model.OfficeID && (m.DocumentStatusID == (byte)ApiCommonEnum.DocumentStatusTypes.ApprovedFirms)).Count()
                            //};
                            //ModuleList.Add(obj);
                        }

                        #endregion

                        break;


                    case ((int)ApiCommonEnum.Modules.Home):

                        //Code for cc
                        break;

                    case ((int)ApiCommonEnum.Modules.CertifiedCopy):

                        #region Certified Copy

                        // List<CC_ApplicationDetails> CCApplicationList = null;

                        //    if (model.MonthID != 0)
                        //    {
                        //        //******** For Specific Month in Year ****************** 
                        //        CCApplicationList = dbContext.CC_ApplicationDetails.Where(c => c.UserId == model.UserID && c.ApplicationDate.Month == model.MonthID && c.ApplicationDate.Year == model.YearID).ToList();
                        //    }
                        //    else
                        //    {
                        //        //******** For All Months in Year ****************** 
                        //        CCApplicationList = dbContext.CC_ApplicationDetails.Where(c => c.UserId == model.UserID && c.ApplicationDate.Year == model.YearID).ToList();
                        //    }

                        //if (model.RoleID == (short)ApiCommonEnum.RoleDetails.OnlineUser)
                        //{
                        //    //********* Online User ***********************
                        //    obj = new SubModuleDetails()
                        //    {
                        //        SubModuleName = "CC Data Entry in Progress",
                        //        Count = CCApplicationList.Where(m => m.UserId == model.UserID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_OnlineDataEntryIsInProgress).Count()
                        //    };
                        //    ModuleList.Add(obj);
                        //    obj = new SubModuleDetails()
                        //    {
                        //        SubModuleName = "CC Application Submitted",
                        //        Count = CCApplicationList.Where(m => m.UserId == model.UserID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_ApplicationSubmitted).Count()
                        //    };
                        //    ModuleList.Add(obj);

                        //    obj = new SubModuleDetails()
                        //    {
                        //        SubModuleName = "Online Payment Done",
                        //        Count = CCApplicationList.Where(m => m.UserId == model.UserID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_OnlinePaymentDone).Count()
                        //    };
                        //    ModuleList.Add(obj);


                        //    obj = new SubModuleDetails()
                        //    {
                        //        SubModuleName = "CC Application Accepted",
                        //        Count = CCApplicationList.Where(m => m.UserId == model.UserID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_ApplicationAccepted).Count()
                        //    };
                        //    ModuleList.Add(obj);
                        //}
                        //else
                        //{
                        //    //******************************** SR ************************************

                        //    List<CC_ApplicationDetails> CCApplicationListForSR = null; //dbContext.FRM_FirmDetails.Where(c => c.OfficeID == model.OfficeID && c.ApplicationDate.Month == model.MonthID && c.ApplicationDate.Year == model.YearID).ToList();
                        //    if (model.MonthID != 0)
                        //    {
                        //        //******** For Specific Month in Year ****************** 
                        //        CCApplicationListForSR = dbContext.CC_ApplicationDetails.Where(c => c.CCIssueOfficeID == model.OfficeID && c.ApplicationDate.Month == model.MonthID && c.ApplicationDate.Year == model.YearID).ToList();
                        //    }
                        //    else
                        //    {
                        //        //******** For All Months in Year ****************** 
                        //        CCApplicationListForSR = dbContext.CC_ApplicationDetails.Where(c => c.CCIssueOfficeID == model.OfficeID && c.ApplicationDate.Year == model.YearID).ToList();
                        //    }

                        //    //-----------------------------------------------------------------------------------------------------------------------------
                        //    obj = new SubModuleDetails()
                        //    {
                        //        SubModuleName = "CC Application Submitted",
                        //        Count = CCApplicationListForSR.Where(m => m.CCIssueOfficeID == model.OfficeID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_ApplicationSubmitted).Count()
                        //    };
                        //    ModuleList.Add(obj);

                        //    obj = new SubModuleDetails()
                        //    {
                        //        SubModuleName = "Online Payment Done",
                        //        Count = CCApplicationListForSR.Where(m => m.CCIssueOfficeID == model.OfficeID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_OnlinePaymentDone).Count()
                        //    };
                        //    ModuleList.Add(obj);


                        //    obj = new SubModuleDetails()
                        //    {
                        //        SubModuleName = "CC Application Accepted",
                        //        Count = CCApplicationListForSR.Where(m => m.CCIssueOfficeID == model.OfficeID && m.DocumentStatusId == (byte)ApiCommonEnum.DocumentStatusTypes.CC_ApplicationAccepted).Count()
                        //    };
                        //    ModuleList.Add(obj);
                        //} 
                        #endregion

                        break;

                    default:
                        break;
                }
                return ModuleList;

            }
            catch (Exception)
            {
                throw;
            }



        }
        #endregion



        public PasswordDetailsModel GetUserPasswordDetails(long UserID, short RoleID)
        {
            KaveriEntities dbContext = null;
            PasswordDetailsModel model = new PasswordDetailsModel();
            model.IsPasswordExpired = false;
            try
            {
                dbContext = new KaveriEntities();
                int NumberOfDaysToForceChangePassword = Convert.ToInt32(ConfigurationManager.AppSettings["NumberOfDaysToForceChangePassword"]);
                //Int32 NumberOfDaysToForceChangePassword = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["NumberOfDaysToForceChangePassword"]);


                UMG_UserDetails userDetails = dbContext.UMG_UserDetails.Where(x => x.UserID == UserID).FirstOrDefault();
                TimeSpan? difference = DateTime.Now - userDetails.PasswordChangeDate;

                if (difference.HasValue)
                {
                    int daysLeft = difference.Value.Days;
                    if (daysLeft >= NumberOfDaysToForceChangePassword)
                    {
                        model.ResponseMessage = "Please change your Password , You haven't changed it for a while now.";
                        model.IsPasswordExpired = true;
                    }
                }

                // On 9-4-2019 by Shubham Bhagat for password change on first login
                model.IsFirstLogin = userDetails.IsFirstLoginDone;

                return model;
            }
            catch (Exception)
            {
                throw;
            }
        }



        #region added by madhur for OTP on 08-02-2022

        public bool IsMobileNumberVerified(long userID)
        {
            try
            {
                dbContext = new KaveriEntities();

                UMG_UserProfile userProfileDbObj = dbContext.UMG_UserProfile.FirstOrDefault(x => x.UserID == userID);

                if (userProfileDbObj != null)
                {
                    return userProfileDbObj.IsMobileNumVerified;
                }

                return false; // needs to be fixed
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

        public SendSMSResponseModel SendOTP(SendOTPRequestModel OTPRequest)
        {
            long otpVerificationId = 0;
            SendSMSResponseModel OTPResponse = null;
            UMG_OTP_VerificationDetails otpVerificationDbObject = null;

            try
            {
                dbContext = new KaveriEntities();
                OTPResponse = new SendSMSResponseModel();
                //DateTime otpSentDateTime = new DateTime();

                // code to check if OTP sent within last 10 mins -- commented

                //UMG_OTP_VerificationDetails otpVerificationDbObjectForValidity = dbContext.UMG_OTP_VerificationDetails.OrderByDescending(x => x.OtpVerificationID).FirstOrDefault(x => x.UserId == OTPRequest.OTPRequestUserId);

                //TimeSpan validMinutes = new TimeSpan(0, 10, 0);

                //if (otpVerificationDbObjectForValidity != null)
                //    otpSentDateTime = otpVerificationDbObjectForValidity.OTPSentDateTime;

                //if (otpVerificationDbObjectForValidity == null || otpSentDateTime < DateTime.Now.Subtract(validMinutes))
                //{

                encryptedParameters = OTPRequest.OTPRequestEncryptedUId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                OTPRequest.OTPRequestUserId = Convert.ToInt64(decryptedParameters["UserId"].ToString().Trim());


                CommonDAL commonDalObject = new CommonDAL();

                string OTP = commonDalObject.GenerateOTP();
                OTPRequest.messageToBeSent = OTPRequest.messageToBeSent + OTP;

                UMG_UserProfile userProfileDbObj = dbContext.UMG_UserProfile.FirstOrDefault(x => x.UserID == OTPRequest.OTPRequestUserId);

                if (userProfileDbObj != null)
                {
                    OTPRequest.toContact = userProfileDbObj.MobileNumber;
                }
                else
                {
                    OTPResponse.errorCode = "1";
                    return OTPResponse;
                }

                SendSMSRequestModel SMSRequest = new SendSMSRequestModel();

                SMSRequest.messageToBeSent = OTPRequest.messageToBeSent;
                SMSRequest.toContact = OTPRequest.toContact;

                string result = SendSMS(SMSRequest);

                if (string.IsNullOrEmpty(result))
                {
                    // Encrypt OTP and insert entry into Database 

                    otpVerificationDbObject = new UMG_OTP_VerificationDetails();
                    otpVerificationId = (dbContext.UMG_OTP_VerificationDetails.Any() ? dbContext.UMG_OTP_VerificationDetails.Max(x => x.OtpVerificationID) : 0) + 1;
                    otpVerificationDbObject.OtpVerificationID = otpVerificationId;
                    otpVerificationDbObject.UserId = OTPRequest.OTPRequestUserId;
                    UMG_UserDetails userDetailsObj = dbContext.UMG_UserDetails.FirstOrDefault(x => x.UserID == OTPRequest.OTPRequestUserId);
                    if (userDetailsObj != null)
                        otpVerificationDbObject.UMG_UserDetails = userDetailsObj;

                    string encryptedOTP = SHA512Checksum.CalculateSHA512Hash(OTP);
                    otpVerificationDbObject.OTPToSend = encryptedOTP;
                    otpVerificationDbObject.OTPSentDateTime = DateTime.Now; // needs to change to response datetime

                    otpVerificationDbObject.OtpTypeId = OTPRequest.OTPTypeId;

                    UMG_OTP_VerificationDetails otpVerificationReturnObject = dbContext.UMG_OTP_VerificationDetails.Add(otpVerificationDbObject);

                    if (otpVerificationDbObject != null)
                        dbContext.SaveChanges();

                    // Encrypt OTP and insert entry into Database 

                    OTPResponse.errorCode = "0";

                }
                else
                {
                    OTPResponse.errorCode = "1";
                }
                //}
                //else
                //{
                //    OTPResponse.errorCode = "0";
                //}

                // code to check if OTP sent within last 30 mins

                // Added by Shubham Bhagat on 20-04-2019 to show mobile number 
                OTPResponse.MobileNumber = "XXXXXXX" + userProfileDbObj.MobileNumber.Substring(7, 3);

                return OTPResponse;
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




        public string SendSMS(SendSMSRequestModel SMSRequest)
        {
            // SendSMSResponseModel SMSResponse = new SendSMSResponseModel();

            try
            {

                ECDataAPI.PreRegApplicationDetailsService.ApplicationDetailsService service = new PreRegApplicationDetailsService.ApplicationDetailsService();
                bool result;
                try
                {
                    result = service.SendSMS(SMSRequest.messageToBeSent, SMSRequest.toContact);

                }
                catch (Exception e)
                {
                    ApiExceptionLogs.LogError(e);
                    return "Something went wrong while connecting to OTP service";

                }

                if (result)
                    return string.Empty; //success
                else
                    return "Unable to send OTP"; //Error



            }
            catch (Exception e)
            {
                ApiExceptionLogs.LogError(e);
                return "Unable to send OTP";

            }
        }



        //public SendSMSResponseModel SendSMS(SendSMSRequestModel SMSRequest)
        //{
        //    SendSMSResponseModel SMSResponse = new SendSMSResponseModel();

        //    try
        //    {

        //        PreRegApplicationDetailsService.ApplicationDetailsService service = new PreRegApplicationDetailsService.ApplicationDetailsService();
        //       bool result= service.SendSMS(SMSRequest.messageToBeSent, SMSRequest.toContact);

        //        return result;


        //    }
        //    catch (Exception e)
        //    {
        //        ApiExceptionLogs.LogError(e);
        //        SMSResponse.errorCode = "1";
        //        return SMSResponse;
        //        //throw;
        //    }
        //}

        KaveriEntities dbContext;
        private String[] encryptedParameters = null;
        private Dictionary<String, String> decryptedParameters = null;


        public ValidateOTPResponseModel ValidateOTP(OTPValidationModel otpValidationModel)
        {
            ValidateOTPResponseModel responseModel = null;
            //UMG_OTP_UserMobile_Verification userMobileVerifyDbObj = new UMG_OTP_UserMobile_Verification();
            try
            {
                dbContext = new KaveriEntities();
                responseModel = new ValidateOTPResponseModel();

                encryptedParameters = otpValidationModel.EncryptedUId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                long userID = Convert.ToInt64(decryptedParameters["UserId"].ToString().Trim());

                UMG_OTP_VerificationDetails userMobileVerifyDbObj = dbContext.UMG_OTP_VerificationDetails.Where(x => x.UserId == userID && x.OtpTypeId == otpValidationModel.OTPTypeId).OrderByDescending(x => x.OtpVerificationID).FirstOrDefault();

                if (userMobileVerifyDbObj != null)
                {
                    string OTPHash = SHA512ChecksumWrapper.ComputeHash(userMobileVerifyDbObj.OTPToSend, otpValidationModel.SessionSalt);
                    if (OTPHash.Equals(otpValidationModel.EncryptedOTP.ToUpper()))
                    {
                        TimeSpan validMinutes = new TimeSpan(0, 10, 0);

                        DateTime otpSentDateTime = userMobileVerifyDbObj.OTPSentDateTime;

                        if (otpSentDateTime > DateTime.Now.Subtract(validMinutes))
                        {
                            userMobileVerifyDbObj.IsOTPVerified = true;
                            userMobileVerifyDbObj.OTPVerifiedDateTime = DateTime.Now;
                            dbContext.Entry(userMobileVerifyDbObj).State = System.Data.Entity.EntityState.Modified;
                            dbContext.SaveChanges();

                            responseModel.responseStatus = true;
                            responseModel.responseMessage = "Your mobile number has been successfully verified.";

                            UMG_UserProfile userProfileDbObj = dbContext.UMG_UserProfile.FirstOrDefault(x => x.UserID == userID);
                            if (userProfileDbObj != null)
                            {
                                if (userProfileDbObj.IsMobileNumVerified == false)
                                {
                                    userProfileDbObj.IsMobileNumVerified = true;
                                    dbContext.Entry(userProfileDbObj).State = System.Data.Entity.EntityState.Modified;
                                    dbContext.SaveChanges();
                                }
                                else
                                {
                                    responseModel.responseMessage = "OTP Verified.";
                                }

                            }
                            else
                            {
                                responseModel.responseStatus = false;
                                responseModel.responseMessage = "The entered OTP is invalid.";
                            }
                        }
                        else
                        {
                            responseModel.responseStatus = false;
                            responseModel.responseMessage = "The entered OTP is invalid.";
                        }
                    }
                    else
                    {
                        responseModel.responseStatus = false;
                        responseModel.responseMessage = "The entered OTP is invalid.";
                    }
                }
                else
                {
                    responseModel.responseStatus = false;
                    responseModel.responseMessage = "The entered OTP is invalid.";
                }
                return responseModel; // needs to be fixed
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                    dbContext.Dispose();
            }
        }



        public SendSMSResponseModel ResendOTP(SendOTPRequestModel OTPRequest)
        {


            long otpVerificationId = 0;
            SendSMSResponseModel OTPResponse = null;
            UMG_OTP_VerificationDetails otpVerificationDbObject = null;

            try
            {
                dbContext = new KaveriEntities();
                OTPResponse = new SendSMSResponseModel();

                CommonDAL commonDalObject = new CommonDAL();

                string OTP = commonDalObject.GenerateOTP();
                OTPRequest.messageToBeSent = OTPRequest.messageToBeSent + OTP;

                UMG_UserProfile userProfileDbObj = dbContext.UMG_UserProfile.FirstOrDefault(x => x.UserID == OTPRequest.OTPRequestUserId);

                if (userProfileDbObj != null)
                {
                    OTPRequest.toContact = userProfileDbObj.MobileNumber;
                }
                else
                {
                    OTPResponse.errorCode = "1";
                    return OTPResponse;
                }

                SendSMSRequestModel SMSRequest = new SendSMSRequestModel();

                SMSRequest.messageToBeSent = OTPRequest.messageToBeSent;
                SMSRequest.toContact = OTPRequest.toContact;

                string result = SendSMS(SMSRequest);

                if (string.IsNullOrEmpty(result))
                {

                    // CHETAN - CODE TO UPDATE PREVIOUS ISNULLIFIED ENTRIES TO TRUE
                    var Obj_OTPVerificationDetail = dbContext.UMG_OTP_VerificationDetails.Where(x => x.UserId == OTPRequest.OTPRequestUserId).ToList();

                    foreach (var item in Obj_OTPVerificationDetail)
                    {
                        item.IsNullfied = true;
                        dbContext.Entry(userProfileDbObj).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();

                    }


                    // Encrypt OTP and insert entry into Database 

                    otpVerificationDbObject = new UMG_OTP_VerificationDetails();

                    otpVerificationId = (dbContext.UMG_OTP_VerificationDetails.Any() ? dbContext.UMG_OTP_VerificationDetails.Max(x => x.OtpVerificationID) : 0) + 1;
                    otpVerificationDbObject.OtpVerificationID = otpVerificationId;
                    otpVerificationDbObject.UserId = OTPRequest.OTPRequestUserId;



                    UMG_UserDetails userDetailsObj = dbContext.UMG_UserDetails.FirstOrDefault(x => x.UserID == OTPRequest.OTPRequestUserId);
                    if (userDetailsObj != null)
                        otpVerificationDbObject.UMG_UserDetails = userDetailsObj;




                    string encryptedOTP = SHA512Checksum.CalculateSHA512Hash(OTP);

                    otpVerificationDbObject.OTPToSend = encryptedOTP;
                    otpVerificationDbObject.OTPSentDateTime = DateTime.Now; // needs to change to response datetime

                    otpVerificationDbObject.OtpTypeId = OTPRequest.OTPTypeId;

                    UMG_OTP_VerificationDetails otpVerificationReturnObject = dbContext.UMG_OTP_VerificationDetails.Add(otpVerificationDbObject);

                    if (otpVerificationDbObject != null)
                        dbContext.SaveChanges();

                    // Encrypt OTP and insert entry into Database 
                }
                else
                {
                    OTPResponse.errorCode = "1";
                }


                // code to check if OTP sent within last 30 mins

                return OTPResponse;
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


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
            // free native resources
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
