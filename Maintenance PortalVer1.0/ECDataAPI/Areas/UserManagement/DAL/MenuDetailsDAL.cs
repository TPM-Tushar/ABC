using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;  using ECDataAPI.Entity.KaigrSearchDB; 
using Security;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.DAL
{
    public class MenuDetailsDAL : IMenuDetails, IDisposable
    {

        #region Properties
        private KaveriEntities dbContext = null;
        private Dictionary<String, String> decryptedParameters = null;
        private String[] encryptedParameters = null;

        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Add Menu Details
        /// <summary>
        /// Adds Menu
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsResponseModel AddMenu(MenuDetailsModel menuDetailsModel)
        {
            try
            {
                dbContext = new KaveriEntities();
                UMG_MenuDetails menuDetailsObj = new UMG_MenuDetails();

                #region RoleMenuMapping
                //UMG_RoleMenuMapping roleMenuMappingObj;
                #endregion

                // To trim white spaces
                menuDetailsModel.MenuName = menuDetailsModel.MenuName.Trim();
                menuDetailsModel.MenuNameR = menuDetailsModel.MenuNameR.Trim();



                // For Checking if Menu Name Already exists
                #region Commented by M rafe on 9-12-12
                //List<UMG_MenuDetails> umg_MenuDetailsList = dbContext.UMG_MenuDetails.ToList();
                //if (umg_MenuDetailsList != null)
                //{
                //    if (umg_MenuDetailsList.Count() != 0)
                //    {
                //        foreach (UMG_MenuDetails umg_MenuDetails in umg_MenuDetailsList)
                //        {
                //            if (umg_MenuDetails.MenuName.ToLower().Equals(menuDetailsModel.MenuName.ToLower()))
                //            {
                //                MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                //                menuDetailsResponseModel.Message = "Menu Name Already Exists.Please try another name.";
                //                menuDetailsResponseModel.Result = false;
                //                return menuDetailsResponseModel;
                //            }
                //        }
                //    }
                //} 
                #endregion



                if (dbContext.UMG_MenuDetails.Any(x => x.MenuName.ToLower().Equals(menuDetailsModel.MenuName.ToLower())))
                {
                    MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                    menuDetailsResponseModel.Message = "Menu Name Already Exists.Please try another name.";
                    menuDetailsResponseModel.Result = false;
                    return menuDetailsResponseModel;
                }


                using (TransactionScope ts = new TransactionScope())
                {
                    menuDetailsObj.MenuID = (dbContext.UMG_MenuDetails.Any() ? dbContext.UMG_MenuDetails.Max(a => a.MenuID) : 0) + 1;
                    menuDetailsObj.MenuName = menuDetailsModel.MenuName;
                    menuDetailsObj.MenuNameR = menuDetailsModel.MenuNameR;
                    menuDetailsObj.MenuIcon = menuDetailsModel.MenuIcon;
                    menuDetailsObj.MenuDesc = menuDetailsModel.MenuDescription;

                    //For adding the ParentId of Menu Details
                    if (menuDetailsModel.ParentID == 0)
                    {
                        menuDetailsObj.IsHorizontalMenu = false;
                        menuDetailsObj.ParentID = 0;
                    }
                    else if (menuDetailsModel.FirstChildMenuDetailsId == 0)
                    {
                        menuDetailsObj.IsHorizontalMenu = false;
                        menuDetailsObj.ParentID = menuDetailsModel.ParentID;
                    }
                    else if (menuDetailsModel.SecondChildMenuDetailsId == 0)
                    {
                        menuDetailsObj.IsHorizontalMenu = true;
                        menuDetailsObj.ParentID = menuDetailsModel.FirstChildMenuDetailsId;
                    }

                    menuDetailsObj.Sequence = Convert.ToInt16(menuDetailsModel.Sequence);
                    menuDetailsObj.VerticalLevel = Convert.ToInt16(menuDetailsModel.VerticalLevel);
                    menuDetailsObj.HorizontalSequence = Convert.ToInt16(menuDetailsModel.HorizontalSequence);
                    menuDetailsObj.IsActive = menuDetailsModel.IsActive;
                    menuDetailsObj.LevelGroupCode = Convert.ToInt32(menuDetailsModel.LevelGroupCode);
                    menuDetailsObj.IsMenuIDParameter = menuDetailsModel.IsMenuIDParameter;

                    dbContext.UMG_MenuDetails.Add(menuDetailsObj);

                    #region RoleMenuMapping
                    //if (menuDetailsModel.RoleListIds != null)
                    //{
                    //    foreach (int roleId in menuDetailsModel.RoleListIds)
                    //    {
                    //        roleMenuMappingObj = new UMG_RoleMenuMapping();

                    //        roleMenuMappingObj.MenuID = menuDetailsObj.MenuID;
                    //        roleMenuMappingObj.RoleID = (short)roleId;
                    //        roleMenuMappingObj.IsAdd = null;
                    //        roleMenuMappingObj.IsEdit = null;
                    //        roleMenuMappingObj.IsDelete = null;

                    //        dbContext.UMG_RoleMenuMapping.Add(roleMenuMappingObj);
                    //    }
                    //}
                    #endregion

                    // For Activity Log
                    String messageForActivityLog = "Menu Detail Added # " + menuDetailsModel.MenuName + "- Menu Detail Added.";
                    if (messageForActivityLog.Length < 1000)
                        ApiCommonFunctions.SystemUserActivityLog(menuDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                    else
                    {
                        messageForActivityLog = messageForActivityLog.Substring(0, 999);
                        ApiCommonFunctions.SystemUserActivityLog(menuDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                    }
                    dbContext.SaveChanges();
                    ts.Complete();
                    MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                    menuDetailsResponseModel.Message = "Menu Details Added Successfully";
                    menuDetailsResponseModel.Result = true;
                    return menuDetailsResponseModel;
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion
        #endregion

        #region Retrive Menu Details List
        /// <summary>
        /// Retrives Menu
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MenuDetailsModel> RetriveMenu()
        {
            List<MenuDetailsModel> menuDetailsList = null;
            MenuDetailsModel menuDetailsObj = null;
          //  List<UMG_MenuDetails> umg_MenuDetailsList = null;
            using (dbContext = new KaveriEntities())
            {
                try
                {
                    menuDetailsList = new List<MenuDetailsModel>();
                   
                  var   umg_MenuDetailsList = dbContext.USP_PopulateMenuActionDetails().ToList();

                    if (umg_MenuDetailsList != null)
                    {

                        foreach (var menuDetailsModel in umg_MenuDetailsList)
                        {
                            menuDetailsObj = new MenuDetailsModel();
                            // For Encrypting id

                            menuDetailsObj.ActionAssigned = menuDetailsModel.ActionAssigned;
                            //menuDetailsObj.MenuID = menuDetailsModel.MenuID;
                            // For Encrypting id
                            menuDetailsObj.EncryptedID = URLEncrypt.EncryptParameters(new String[] {
                                                                    "MenuID="+menuDetailsModel.MenuID
                                                        });
                            menuDetailsObj.MenuName = menuDetailsModel.MenuName;
                            menuDetailsObj.MenuNameR = menuDetailsModel.MenuNameR;
                            menuDetailsObj.ParentID = menuDetailsModel.ParentID;
                            menuDetailsObj.Sequence = Convert.ToString(menuDetailsModel.Sequence);
                            menuDetailsObj.VerticalLevel = Convert.ToString(menuDetailsModel.VerticalLevel);
                            menuDetailsObj.HorizontalSequence = Convert.ToString(menuDetailsModel.HorizontalSequence);
                            menuDetailsObj.IsActive = menuDetailsModel.IsActive;
                            //menuDetailsObj.IsActiveString = menuDetailsModel.IsActive ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";
                            menuDetailsObj.LevelGroupCode = Convert.ToString(menuDetailsModel.LevelGroupCode);
                            menuDetailsObj.IsMenuIDParameter = menuDetailsModel.IsMenuIDParameter;
                            menuDetailsObj.IsHorizontalMenu = menuDetailsModel.IsHorizontalMenu;

                          //  menuDetailsObj.MenuActionMappingButton = "<a href='#' onclick=MenuActionMapping('" + menuDetailsObj.EncryptedID + "');><i class='fa fa-plus-square fa-2x' aria-hidden='true' style='color:black;' ></i></a>";
                            
// changed by m rafe on 23-12-19                            
                            menuDetailsObj.MenuActionMappingButton = menuDetailsObj.ParentID==0? "<i class='fa fa-close fa-2x' style='color:black'></i>" :  "<a href='#' onclick=MenuActionMapping('" + menuDetailsObj.EncryptedID + "');><i class='fa fa-plus-square fa-2x' aria-hidden='true' style='color:black;' ></i></a>";

                            // For changing parent menu  
                            //menuDetailsObj.TempParentId = menuDetailsModel.ParentID;

                            //For Displaying Parent Menus
                             
                            //if (menuDetailsModel.ParentID == 0)
                            //{
                            //    menuDetailsObj.ParentMenu = "Self";
                            //}
                            //else
                            //{
                            //    UMG_MenuDetails parentMenuObj = dbContext.UMG_MenuDetails.Where(x => x.MenuID == menuDetailsModel.ParentID).FirstOrDefault();
                            //    if (parentMenuObj == null)
                            //    {
                            //    }
                            //    else
                            //    {
                            //        menuDetailsObj.ParentMenu = parentMenuObj.MenuName;
                            //    }
                            //}
                            menuDetailsObj.ParentMenu = menuDetailsModel.ParentMenuName;



                            menuDetailsList.Add(menuDetailsObj);
                        }
                        return menuDetailsList;
                    }
                    else
                    {
                        return menuDetailsList;
                    }
                }
                catch (Exception)
                {
                    throw;
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
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Edit Menu Details
        /// <summary>
        /// EditsMenu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public MenuDetailsModel EditMenu(String EncryptedID)
        {
            MenuDetailsModel menuDetailsModel = null;
            UMG_MenuDetails umg_MenuDetails = null;
            try
            {
                encryptedParameters = EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());

                //dbContext.UMG_MenuDetails.Where(m => m.MenuID == id).ToList();
                dbContext = new KaveriEntities();
                menuDetailsModel = new MenuDetailsModel();
                //menuDetailsModel.EncryptedID = EncryptedID;
                umg_MenuDetails = dbContext.UMG_MenuDetails.Where(x => x.MenuID == id).FirstOrDefault();
                if (umg_MenuDetails != null)
                {
                    menuDetailsModel.MenuID = umg_MenuDetails.MenuID;
                    menuDetailsModel.MenuName = umg_MenuDetails.MenuName;
                    menuDetailsModel.MenuNameR = umg_MenuDetails.MenuNameR;
                    menuDetailsModel.Sequence = Convert.ToString(umg_MenuDetails.Sequence);
                    menuDetailsModel.VerticalLevel = Convert.ToString(umg_MenuDetails.VerticalLevel);
                    menuDetailsModel.HorizontalSequence = Convert.ToString(umg_MenuDetails.HorizontalSequence);
                    menuDetailsModel.IsActive = umg_MenuDetails.IsActive;
                    menuDetailsModel.LevelGroupCode = Convert.ToString(umg_MenuDetails.LevelGroupCode);
                    menuDetailsModel.IsMenuIDParameter = umg_MenuDetails.IsMenuIDParameter;
                    menuDetailsModel.IsHorizontalMenu = umg_MenuDetails.IsHorizontalMenu;

                    menuDetailsModel.MenuIcon = umg_MenuDetails.MenuIcon;
                    menuDetailsModel.MenuDescription = umg_MenuDetails.MenuDesc;

                    #region RoleMenuMapping
                    ////Code for fetching All Roles 
                    //menuDetailsModel.RoleList = GetRoleList();

                    //// Code for fetching RoleListIds according to MenuId
                    //List<UMG_RoleMenuMapping> roleMenuMappingList = dbContext.UMG_RoleMenuMapping.Where(x => x.MenuID == menuDetailsModel.MenuID).ToList();
                    //List<int> roleListIds = new List<int>();
                    //if (roleMenuMappingList != null)
                    //{
                    //    foreach (var item in roleMenuMappingList)
                    //    {
                    //        roleListIds.Add(item.RoleID);
                    //    }
                    //}
                    //menuDetailsModel.RoleListIds = roleListIds.ToArray();
                    #endregion

                    menuDetailsModel.MenuList = GetMenuList(id);

                    menuDetailsModel.MenuDetailsResponseModel = new MenuDetailsResponseModel();
                    menuDetailsModel.MenuDetailsResponseModel.Result = true;
                    menuDetailsModel.MenuDetailsResponseModel.Message = "";


                    menuDetailsModel.ParentID = umg_MenuDetails.ParentID;
                    // Menu is at Parent Menu Position
                    if (menuDetailsModel.ParentID == 0)
                    {
                        //For Selecting Select option in Dropdownlist
                        menuDetailsModel.FirstChildMenuDetailsId = -1;
                        //For Selecting Select option in Dropdownlist
                        menuDetailsModel.SecondChildMenuDetailsId = -1;
                        //For displaying empty list 
                        menuDetailsModel.FirstChildMenuDetailsList = new List<SelectListItem>();
                        menuDetailsModel.SecondChildMenuDetailsList = new List<SelectListItem>();

                        // For checking the updatation of Parent, First child and Second Child menu
                        //    menuDetailsModel.IsParentMenuListUpdatable = true;
                        //    menuDetailsModel.IsFirstChildMenuListUpdatable = true;
                        //    menuDetailsModel.IsSecondChildMenuListUpdatable = true;
                    }
                    else
                    {
                        // Menu is at First Child Menu Position
                        UMG_MenuDetails umg_MenuDetailsOne = dbContext.UMG_MenuDetails.Where(x => x.MenuID == menuDetailsModel.ParentID).FirstOrDefault();
                        if (umg_MenuDetailsOne != null)
                        {
                            if (umg_MenuDetailsOne.ParentID == 0)
                            {
                                menuDetailsModel.ParentID = umg_MenuDetailsOne.MenuID;
                                menuDetailsModel.FirstChildMenuDetailsId = 0;
                                menuDetailsModel.SecondChildMenuDetailsId = -1;
                                menuDetailsModel.FirstChildMenuDetailsList = GetFirstChildMenuDetailsList(menuDetailsModel.ParentID, id);
                                menuDetailsModel.SecondChildMenuDetailsList = new List<SelectListItem>();

                                // For checking the updatation of Parent, First child and Second Child menu
                                //menuDetailsModel.IsParentMenuListUpdatable = false;
                                //menuDetailsModel.IsFirstChildMenuListUpdatable = true;
                                //menuDetailsModel.IsSecondChildMenuListUpdatable = true;

                            }
                        }
                        // Menu is at Second Child Menu Position
                        UMG_MenuDetails umg_MenuDetailsTwo = dbContext.UMG_MenuDetails.Where(x => x.MenuID == umg_MenuDetailsOne.ParentID).FirstOrDefault();
                        if (umg_MenuDetailsTwo != null)
                        {
                            if (umg_MenuDetailsTwo.ParentID == 0)
                            {
                                menuDetailsModel.ParentID = umg_MenuDetailsTwo.MenuID;
                                menuDetailsModel.FirstChildMenuDetailsId = umg_MenuDetailsOne.MenuID;
                                menuDetailsModel.SecondChildMenuDetailsId = 0;
                                menuDetailsModel.FirstChildMenuDetailsList = GetFirstChildMenuDetailsList(umg_MenuDetailsOne.ParentID, id);
                                menuDetailsModel.SecondChildMenuDetailsList = GetSecondChildMenuDetailsList(menuDetailsModel.ParentID, id);

                                // For checking the updatation of Parent, First child and Second Child menu
                                //menuDetailsModel.IsParentMenuListUpdatable = false;
                                //menuDetailsModel.IsFirstChildMenuListUpdatable = false;
                                //menuDetailsModel.IsSecondChildMenuListUpdatable = true;

                            }
                        }
                    }
                    // For checking if the menu is the parent of another menu.
                    List<UMG_MenuDetails> umg_MenuDetailsChildList = dbContext.UMG_MenuDetails.Where(x => x.ParentID == menuDetailsModel.MenuID).OrderBy(c => c.MenuName).ToList();

                    if (umg_MenuDetailsChildList != null)
                    {
                        if (umg_MenuDetailsChildList.Count == 0)
                        {
                            menuDetailsModel.DropDownValuesCanChange = true;
                        }
                        else
                        {
                            menuDetailsModel.DropDownValuesCanChange = false;
                            menuDetailsModel.DropDownValuesStatus = "Note: Child Already Exist for MenuName \"" + menuDetailsModel.MenuName + "\" so you cannot change the parent menu.";
                        }
                    }
                }
                else
                {
                    menuDetailsModel.MenuDetailsResponseModel = new MenuDetailsResponseModel();
                    menuDetailsModel.MenuDetailsResponseModel.Result = false;
                    menuDetailsModel.MenuDetailsResponseModel.Message = "Menu Details not found.";
                    return menuDetailsModel;
                }
                return menuDetailsModel;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Update Menu Details
        /// <summary>
        /// Updates Menu
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsResponseModel UpdateMenu(MenuDetailsModel menuDetailsModel)
        {
            #region RoleMenuMapping
            //UMG_RoleMenuMapping roleMenuMappingObj = null;
            #endregion

            try
            {
                encryptedParameters = menuDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());

                dbContext = new KaveriEntities();


                // To trim white spaces
                menuDetailsModel.MenuName = menuDetailsModel.MenuName.Trim();
                menuDetailsModel.MenuNameR = menuDetailsModel.MenuNameR.Trim();

                //For Checking if Menu Name Already exists
                List<UMG_MenuDetails> umg_MenuDetailsList = dbContext.UMG_MenuDetails.Where(x => x.MenuID != id).ToList();
                if (umg_MenuDetailsList != null)
                {
                    if (umg_MenuDetailsList.Count() != 0)
                    {
                        foreach (UMG_MenuDetails umg_MenuDetails in umg_MenuDetailsList)
                        {
                            if (umg_MenuDetails.MenuName.ToLower().Equals(menuDetailsModel.MenuName.ToLower()))
                            {
                                MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                                menuDetailsResponseModel.Message = "Menu Name Already Exists.Please try another name.";
                                menuDetailsResponseModel.Result = false;
                                return menuDetailsResponseModel;
                            }
                        }
                    }
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    UMG_MenuDetails umg_MenuDetails = dbContext.UMG_MenuDetails.Where(x => x.MenuID == id).FirstOrDefault();
                    UMG_MenuDetails_Log umg_MenuDetails_LogObj = new UMG_MenuDetails_Log();

                    if (umg_MenuDetails != null)
                    {

                        umg_MenuDetails_LogObj.LogID = (dbContext.UMG_MenuDetails_Log.Any() ? dbContext.UMG_MenuDetails_Log.Max(a => a.LogID) : 0) + 1;



                        umg_MenuDetails_LogObj.MenuID = umg_MenuDetails.MenuID;
                        umg_MenuDetails_LogObj.MenuName = umg_MenuDetails.MenuName;
                        umg_MenuDetails_LogObj.MenuNameR = umg_MenuDetails.MenuNameR;
                        umg_MenuDetails_LogObj.ParentID = umg_MenuDetails.ParentID;
                        umg_MenuDetails_LogObj.Sequence = umg_MenuDetails.Sequence;
                        umg_MenuDetails_LogObj.VerticalLevel = umg_MenuDetails.VerticalLevel;
                        umg_MenuDetails_LogObj.HorizontalSequence = umg_MenuDetails.HorizontalSequence;
                        umg_MenuDetails_LogObj.IsActive = umg_MenuDetails.IsActive;
                        umg_MenuDetails_LogObj.LevelGroupCode = umg_MenuDetails.LevelGroupCode;
                        umg_MenuDetails_LogObj.IsMenuIDParameter = umg_MenuDetails.IsMenuIDParameter;
                        umg_MenuDetails_LogObj.IsHorizontalMenu = umg_MenuDetails.IsHorizontalMenu;
                        umg_MenuDetails_LogObj.UpdateDateTime = DateTime.Now;
                        umg_MenuDetails_LogObj.UserID = menuDetailsModel.UserIdForActivityLogFromSession;
                        umg_MenuDetails_LogObj.UserIPAddress = menuDetailsModel.UserIPAddress;
                        umg_MenuDetails_LogObj.ActionPerformed = "Update";


                        dbContext.UMG_MenuDetails_Log.Add(umg_MenuDetails_LogObj);



                        umg_MenuDetails.MenuName = menuDetailsModel.MenuName;
                        umg_MenuDetails.MenuNameR = menuDetailsModel.MenuNameR;

                        umg_MenuDetails.MenuIcon = menuDetailsModel.MenuIcon;
                        umg_MenuDetails.MenuDesc = menuDetailsModel.MenuDescription;

                        //For adding the ParentId of Menu Details
                        if (menuDetailsModel.DropDownValuesCanChange == true)
                        {
                            if (menuDetailsModel.ParentID == 0)
                            {
                                umg_MenuDetails.IsHorizontalMenu = false;
                                umg_MenuDetails.ParentID = 0;
                            }
                            else if (menuDetailsModel.FirstChildMenuDetailsId == 0)
                            {
                                umg_MenuDetails.IsHorizontalMenu = false;
                                umg_MenuDetails.ParentID = menuDetailsModel.ParentID;
                            }
                            else if (menuDetailsModel.SecondChildMenuDetailsId == 0)
                            {
                                umg_MenuDetails.IsHorizontalMenu = true;
                                umg_MenuDetails.ParentID = menuDetailsModel.FirstChildMenuDetailsId;
                            }
                        }

                        umg_MenuDetails.Sequence = Convert.ToInt16(menuDetailsModel.Sequence);
                        umg_MenuDetails.VerticalLevel = Convert.ToInt16(menuDetailsModel.VerticalLevel);
                        umg_MenuDetails.HorizontalSequence = Convert.ToInt16(menuDetailsModel.HorizontalSequence);
                        umg_MenuDetails.IsActive = menuDetailsModel.IsActive;
                        umg_MenuDetails.LevelGroupCode = Convert.ToInt32(menuDetailsModel.LevelGroupCode);
                        umg_MenuDetails.IsMenuIDParameter = menuDetailsModel.IsMenuIDParameter;
                        // Changed because on editing it is taking false value of IsHorizontalMenu
                        umg_MenuDetails.IsHorizontalMenu = umg_MenuDetails.IsHorizontalMenu;
                        //umg_MenuDetails.IsHorizontalMenu = menuDetailsModel.IsHorizontalMenu;

                        #region RoleMenuMapping
                        ////For Checking if the roleListId exist in the coming Request
                        //if (menuDetailsModel.RoleListIds != null)
                        //{
                        //    //List<int> roleListIdsAsList = menuDetailsModel.RoleListIds.ToList();

                        //    List<UMG_RoleMenuMapping> roleMenuMappingList = dbContext.UMG_RoleMenuMapping.Where(x => x.MenuID == id).ToList();

                        //    if (roleMenuMappingList.Count != 0)
                        //    {
                        //        List<UMG_RoleMenuMapping> roleMenuMappingListForDeletion = dbContext.UMG_RoleMenuMapping.Where(x => x.MenuID == id).ToList();
                        //        if (roleMenuMappingListForDeletion.Count != 0)
                        //        {
                        //            foreach (var roleMenuMapping in roleMenuMappingListForDeletion)
                        //            { 
                        //                dbContext.UMG_RoleMenuMapping.Remove(roleMenuMapping);
                        //            }
                        //        }
                        //        foreach (var roleListId in menuDetailsModel.RoleListIds)
                        //        {
                        //            roleMenuMappingObj = new UMG_RoleMenuMapping();
                        //            roleMenuMappingObj.MenuID = id;
                        //            roleMenuMappingObj.RoleID = (short)roleListId;
                        //            roleMenuMappingObj.IsAdd = null;
                        //            roleMenuMappingObj.IsEdit = null;
                        //            roleMenuMappingObj.IsDelete = null;

                        //            dbContext.UMG_RoleMenuMapping.Add(roleMenuMappingObj);
                        //        }
                        //    }
                        //    else
                        //    {
                        //        foreach (var roleListId in menuDetailsModel.RoleListIds)
                        //        {
                        //            roleMenuMappingObj = new UMG_RoleMenuMapping();
                        //            roleMenuMappingObj.MenuID = id;
                        //            roleMenuMappingObj.RoleID = (short)roleListId;
                        //            roleMenuMappingObj.IsAdd = null;
                        //            roleMenuMappingObj.IsEdit = null;
                        //            roleMenuMappingObj.IsDelete = null;

                        //            dbContext.UMG_RoleMenuMapping.Add(roleMenuMappingObj);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    List<UMG_RoleMenuMapping> roleMenuMappingList = dbContext.UMG_RoleMenuMapping.Where(x => x.MenuID == id).ToList();
                        //    if (roleMenuMappingList.Count != 0)
                        //    {
                        //        foreach (var roleMenuMapping in roleMenuMappingList)
                        //        {
                        //            dbContext.UMG_RoleMenuMapping.Remove(roleMenuMapping);
                        //        }
                        //    }
                        //}
                        #endregion
                        #region Old Logic for Update
                        ////For Checking if the roleListId exist in the coming Request
                        //if (menuDetailsModel.RoleListIds.Count() != 0)
                        //{
                        //    List<int> roleListIdsAsList = menuDetailsModel.RoleListIds.ToList();

                        //    List<UMG_RoleMenuMapping> roleMenuMappingList = dbContext.UMG_RoleMenuMapping.Where(x => x.MenuID == id).ToList();

                        //    if (roleMenuMappingList.Count() != 0)
                        //    {
                        //        foreach (var roleListId in menuDetailsModel.RoleListIds)
                        //        {
                        //            foreach (var roleMenuMapping in roleMenuMappingList)
                        //            {
                        //                foreach (var item in roleListIdsAsList)
                        //                {
                        //                    if (item == roleMenuMapping.RoleID)
                        //                    {
                        //                        continue;
                        //                    }
                        //                    else
                        //                    {
                        //                        roleMenuMappingObj = dbContext.UMG_RoleMenuMapping.Where(x => x.MenuID == roleMenuMapping.MenuID && x.RoleID == roleMenuMapping.RoleID).FirstOrDefault();
                        //                        //roleMenuMappingList.Remove(roleMenuMappingObj);
                        //                        dbContext.UMG_RoleMenuMapping.Remove(roleMenuMappingObj);
                        //                    }
                        //                }
                        //            }
                        //            roleMenuMappingObj = new UMG_RoleMenuMapping();
                        //            roleMenuMappingObj.MenuID = id;
                        //            roleMenuMappingObj.RoleID = (short)roleListId;
                        //            roleMenuMappingObj.IsAdd = null;
                        //            roleMenuMappingObj.IsEdit = null;
                        //            roleMenuMappingObj.IsDelete = null;

                        //            foreach (var item in roleMenuMappingList)
                        //            {
                        //                if (item.MenuID == roleMenuMappingObj.MenuID)
                        //                {
                        //                }
                        //                else
                        //                {
                        //                    dbContext.UMG_RoleMenuMapping.Add(roleMenuMappingObj);
                        //                }
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        foreach (var roleListId in menuDetailsModel.RoleListIds)
                        //        {
                        //            roleMenuMappingObj = new UMG_RoleMenuMapping();
                        //            roleMenuMappingObj.MenuID = id;
                        //            roleMenuMappingObj.RoleID = (short)roleListId;
                        //            roleMenuMappingObj.IsAdd = null;
                        //            roleMenuMappingObj.IsEdit = null;
                        //            roleMenuMappingObj.IsDelete = null;

                        //            dbContext.UMG_RoleMenuMapping.Add(roleMenuMappingObj);
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    List<UMG_RoleMenuMapping> roleMenuMappingList = dbContext.UMG_RoleMenuMapping.Where(x => x.MenuID == id).ToList();
                        //    if (roleMenuMappingList.Count != 0)
                        //    {
                        //        foreach (var roleMenuMapping in roleMenuMappingList)
                        //        {
                        //            dbContext.UMG_RoleMenuMapping.Remove(roleMenuMapping);
                        //        }
                        //    }
                        //} 
                        #endregion

                        // For Activity Log
                        String messageForActivityLog = "Menu Detail Updated # " + menuDetailsModel.MenuName + "- Menu Detail Updated.";
                        if (messageForActivityLog.Length < 1000)
                            ApiCommonFunctions.SystemUserActivityLog(menuDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                        else
                        {
                            messageForActivityLog = messageForActivityLog.Substring(0, 999);
                            ApiCommonFunctions.SystemUserActivityLog(menuDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                        }
                        dbContext.SaveChanges();
                        ts.Complete();
                        MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                        menuDetailsResponseModel.Message = "Menu Details Updated Successfully";
                        menuDetailsResponseModel.Result = true;
                        return menuDetailsResponseModel;
                    }
                    else
                    {
                        MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
                        menuDetailsResponseModel.Message = "Menu Details not Updated ";
                        menuDetailsResponseModel.Result = true;
                        return menuDetailsResponseModel;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (dbContext != null)
                {
                    dbContext.Dispose();
                }
            }
        }
        #endregion
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Delete Menu Details
        /// <summary>
        /// Deletes Menu
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <param name="UserIdForActivityLogFromSession"></param>
        /// <returns></returns>
        public MenuDetailsResponseModel DeleteMenu(String EncryptedID, long UserIdForActivityLogFromSession, string IPAddress)
        {
                        MenuDetailsResponseModel menuDetailsResponseModel = new MenuDetailsResponseModel();
            UMG_MenuDetails umg_MenuDetails = null;
            #region RoleMenuMapping
            //List<UMG_RoleMenuMapping> roleMenuMappingList = null;
            #endregion
            try
            {
                encryptedParameters = EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());



                if (dbContext.UMG_RoleMenuMapping.Any(x => x.MenuID == id))
                {

                    menuDetailsResponseModel.Message = "Menu is mapped to Role. Kindly remove Role menu mapping for menu and proceed.";
                    menuDetailsResponseModel.Result = false;
                    return menuDetailsResponseModel;

                     
                }


                //if (dbContext.UMG_MenuActionAuthorizationMapping.Any(x => x.MenuId == id))
                //{
                //    menuDetailsResponseModel.Message = "Menu is mapped to Actions. Kindly remove menu action mapping for the menu and proceed.";
                //    menuDetailsResponseModel.Result = false;
                //    return menuDetailsResponseModel;

                //}
                if (dbContext.UMG_MenuActionMapping.Any(x => x.MenuID == id))
                {
                    menuDetailsResponseModel.Message = "Menu is mapped to Actions. Kindly remove menu action mapping for the menu and proceed.";
                    menuDetailsResponseModel.Result = false;
                    return menuDetailsResponseModel;

                }






                using (TransactionScope ts = new TransactionScope())
                {
                    dbContext = new KaveriEntities();

                    umg_MenuDetails = dbContext.UMG_MenuDetails.Where(x => x.MenuID == id).FirstOrDefault();
                    UMG_MenuDetails_Log umg_MenuDetails_LogObj = new UMG_MenuDetails_Log();




                    //roleMenuMappingList = dbContext.UMG_RoleMenuMapping.Where(x => x.MenuID == umg_MenuDetails.MenuID).ToList();
                    if (umg_MenuDetails != null)
                    {



                        umg_MenuDetails_LogObj.LogID = (dbContext.UMG_MenuDetails_Log.Any() ? dbContext.UMG_MenuDetails_Log.Max(a => a.LogID) : 0) + 1;



                        umg_MenuDetails_LogObj.MenuID = umg_MenuDetails.MenuID;
                        umg_MenuDetails_LogObj.MenuName = umg_MenuDetails.MenuName;
                        umg_MenuDetails_LogObj.MenuNameR = umg_MenuDetails.MenuNameR;
                        umg_MenuDetails_LogObj.ParentID = umg_MenuDetails.ParentID;
                        umg_MenuDetails_LogObj.Sequence = umg_MenuDetails.Sequence;
                        umg_MenuDetails_LogObj.VerticalLevel = umg_MenuDetails.VerticalLevel;
                        umg_MenuDetails_LogObj.HorizontalSequence = umg_MenuDetails.HorizontalSequence;
                        umg_MenuDetails_LogObj.IsActive = umg_MenuDetails.IsActive;
                        umg_MenuDetails_LogObj.LevelGroupCode = umg_MenuDetails.LevelGroupCode;
                        umg_MenuDetails_LogObj.IsMenuIDParameter = umg_MenuDetails.IsMenuIDParameter;
                        umg_MenuDetails_LogObj.IsHorizontalMenu = umg_MenuDetails.IsHorizontalMenu;
                        umg_MenuDetails_LogObj.UpdateDateTime = DateTime.Now;
                        umg_MenuDetails_LogObj.UserID = UserIdForActivityLogFromSession;
                        umg_MenuDetails_LogObj.UserIPAddress = IPAddress;
                        umg_MenuDetails_LogObj.ActionPerformed = "Delete";


                        dbContext.UMG_MenuDetails_Log.Add(umg_MenuDetails_LogObj);

                         


                        String menuName = umg_MenuDetails.MenuName;
                        dbContext.UMG_MenuDetails.Remove(umg_MenuDetails);
                        //if (roleMenuMappingList != null)
                        //{
                        //    if (roleMenuMappingList.Count() != 0)
                        //    {
                        //        foreach (var roleMenuMapping in roleMenuMappingList)
                        //        {
                        //            dbContext.UMG_RoleMenuMapping.Remove(roleMenuMapping);
                        //        }
                        //    }
                        //}

                        // For Activity Log
                        String messageForActivityLog = menuName + " Menu Detail Deleted.";
                        if (messageForActivityLog.Length < 1000)
                            ApiCommonFunctions.SystemUserActivityLog(UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                        else
                        {
                            messageForActivityLog = messageForActivityLog.Substring(0, 999);
                            ApiCommonFunctions.SystemUserActivityLog(UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                        }

                        dbContext.SaveChanges();
                        ts.Complete();
                        menuDetailsResponseModel.Message = "Menu Details Deleted Successfully";
                        menuDetailsResponseModel.Result = true;
                        return menuDetailsResponseModel;
                    }
                    else
                    { 
                        menuDetailsResponseModel.Message = "Menu Details Not Deleted";
                        menuDetailsResponseModel.Result = false;
                        return menuDetailsResponseModel;
                    }
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
        #endregion
        #endregion

        #region RoleMenuMapping
        //#region Get RoleList
        //// Fetching RoleName And Role Id From UMG_RoleDetails Table
        //public List<SelectListItem> GetRoleList()
        //{
        //    KaveriEntities localdbContext = null;
        //    try
        //    {
        //        localdbContext = new KaveriEntities();
        //        List<SelectListItem> roleList = new List<SelectListItem>();
        //        List<UMG_RoleDetails> umg_roleDetailsList = localdbContext.UMG_RoleDetails.ToList();
        //        if (umg_roleDetailsList != null)
        //        {
        //            if (umg_roleDetailsList.Count() != 0)
        //            {
        //                foreach (var roleDetails in umg_roleDetailsList)
        //                {
        //                    SelectListItem selectListItem = new SelectListItem();
        //                    selectListItem.Text = roleDetails.RoleName;
        //                    selectListItem.Value = Convert.ToString(roleDetails.RoleID);
        //                    roleList.Add(selectListItem);
        //                }
        //            }
        //        }
        //        return roleList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (localdbContext != null)
        //        {
        //            localdbContext.Dispose();
        //        }
        //    }
        //}
        //#endregion
        #endregion

        //#region Get Menu Details List
        //// Get List of Menu Names and their Ids to set their Parent Id
        //public List<SelectListItem> GetMenuList()
        //{
        //    KaveriEntities localdbContext = null;
        //    try
        //    {
        //        localdbContext = new KaveriEntities();
        //        List<SelectListItem> menuList = new List<SelectListItem>();
        //        menuList.Add(new SelectListItem { Text = "------------Select------------", Value = "-1" });
        //        menuList.Add(new SelectListItem { Text = "-------------Self-------------", Value = "0" });
        //        List<UMG_MenuDetails> umg_menuDetailsList = localdbContext.UMG_MenuDetails.ToList();
        //        foreach (var menuDetails in umg_menuDetailsList)
        //        {
        //            SelectListItem selectListItem = new SelectListItem();
        //            selectListItem.Text = menuDetails.MenuName;
        //            selectListItem.Value = Convert.ToString(menuDetails.MenuID);
        //            menuList.Add(selectListItem);
        //        }
        //        return menuList;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    finally
        //    {
        //        if (localdbContext != null)
        //        {
        //            localdbContext.Dispose();
        //        }
        //    }
        //}
        //#endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Get Menu Details List
        /// <summary>
        /// Gets MenuList
        /// </summary>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetMenuList(int menuId = 0)
        {
            KaveriEntities localdbContext = null;
            try
            {
                localdbContext = new KaveriEntities();
                List<SelectListItem> menuList = new List<SelectListItem>();
                menuList.Add(new SelectListItem { Text = "Select", Value = "-1" });
                menuList.Add(new SelectListItem { Text = "Self", Value = "0" });
              //  List<UMG_MenuDetails> umg_menuDetailsList = localdbContext.UMG_MenuDetails.Where(x => x.MenuID != menuId && x.ParentID == 0).ToList();
                List<UMG_MenuDetails> umg_menuDetailsList = localdbContext.UMG_MenuDetails.Where(x => x.MenuID != menuId && x.ParentID == 0).OrderBy(c=>c.MenuName) .ToList();
                if (umg_menuDetailsList != null)
                {
                    if (umg_menuDetailsList.Count() != 0)
                    {
                        foreach (var menuDetails in umg_menuDetailsList)
                        {
                            String activeOrInactive = (menuDetails.IsActive) ? " (Active)" : " (InActive)";
                            SelectListItem selectListItem = new SelectListItem();
                            selectListItem.Text = menuDetails.MenuName + activeOrInactive;
                            selectListItem.Value = Convert.ToString(menuDetails.MenuID);
                            menuList.Add(selectListItem);
                        }
                    }
                }
                return menuList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }
        #endregion
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Get model containing Role list and Menu list
        /// <summary>
        /// Adds Menu
        /// </summary>
        /// <returns></returns>
        public MenuDetailsModel AddMenu()
        {
            MenuDetailsModel menuDetailsModel = new MenuDetailsModel();
            menuDetailsModel.IsActive = true;
            #region RoleMenuMapping
            //menuDetailsModel.RoleList = GetRoleList();
            #endregion
            menuDetailsModel.MenuList = GetMenuList();

            menuDetailsModel.IsUpdatable = false;
            menuDetailsModel.IsHorizontalMenu = true;
            menuDetailsModel.DropDownValuesCanChange = true;

            //For displaying empty list 
            menuDetailsModel.FirstChildMenuDetailsList = new List<SelectListItem>();
            //For Selecting Select option in Dropdownlist
            menuDetailsModel.FirstChildMenuDetailsId = -1;
            menuDetailsModel.SecondChildMenuDetailsList = new List<SelectListItem>();
            //For Selecting Select option in Dropdownlist
            menuDetailsModel.SecondChildMenuDetailsId = -1;
            //For Selecting Select option in Dropdownlist
            menuDetailsModel.ParentID = -1;

            return menuDetailsModel;
        }
        #endregion
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Get First Child Menu Details List
        /// <summary>
        /// Gets FirstChildMenuDetailsList
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetFirstChildMenuDetailsList(int parentId, int menuId = 0)
        {
            KaveriEntities localdbContext = null;
            try
            {
                localdbContext = new KaveriEntities();
                List<SelectListItem> firstChildMenuDetailsList = new List<SelectListItem>();
                firstChildMenuDetailsList.Add(new SelectListItem { Text = "Select", Value = "-1" });
                firstChildMenuDetailsList.Add(new SelectListItem { Text = "Self", Value = "0" });
                List<UMG_MenuDetails> umg_menuDetailsList = localdbContext.UMG_MenuDetails.Where(x => x.MenuID != menuId && x.ParentID == parentId).OrderBy(c=>c.MenuName). ToList();
                if (umg_menuDetailsList != null)
                {
                    if (umg_menuDetailsList.Count() != 0)
                    {
                        foreach (var menuDetails in umg_menuDetailsList)
                        {
                            String activeOrInactive = (menuDetails.IsActive) ? " (Active)" : " (InActive)";
                            SelectListItem selectListItem = new SelectListItem();
                            selectListItem.Text = menuDetails.MenuName + activeOrInactive;
                            selectListItem.Value = Convert.ToString(menuDetails.MenuID);
                            firstChildMenuDetailsList.Add(selectListItem);
                        }
                    }
                }
                return firstChildMenuDetailsList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }
        #endregion
        #endregion

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Get Second Child Menu Details List
        /// <summary>
        /// Gets SecondChildMenuDetailsList
        /// </summary>
        /// <param name="firstChildMenuDetailsId"></param>
        /// <param name="menuId"></param>
        /// <returns></returns>
        public List<SelectListItem> GetSecondChildMenuDetailsList(int firstChildMenuDetailsId, int menuId = 0)
        {
            KaveriEntities localdbContext = null;
            try
            {
                localdbContext = new KaveriEntities();
                List<SelectListItem> secondChildMenuDetailsList = new List<SelectListItem>();
                secondChildMenuDetailsList.Add(new SelectListItem { Text = "Select", Value = "-1" });
                secondChildMenuDetailsList.Add(new SelectListItem { Text = "Self", Value = "0" });
                List<UMG_MenuDetails> umg_menuDetailsList = localdbContext.UMG_MenuDetails.Where(x => x.MenuID != menuId && x.ParentID == firstChildMenuDetailsId).OrderBy(c => c.MenuName).ToList();
                if (umg_menuDetailsList != null)
                {
                    if (umg_menuDetailsList.Count() != 0)
                    {
                        foreach (var menuDetails in umg_menuDetailsList)
                        {
                            String activeOrInactive = (menuDetails.IsActive) ? " (Active)" : " (InActive)";
                            SelectListItem selectListItem = new SelectListItem();
                            selectListItem.Text = menuDetails.MenuName + activeOrInactive;
                            selectListItem.Value = Convert.ToString(menuDetails.MenuID);
                            secondChildMenuDetailsList.Add(selectListItem);
                        }
                    }
                }
                return secondChildMenuDetailsList;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }
        #endregion
        #endregion

        #region Get Area List from Controller Action Details
        /// <summary>
        /// Maps a menu to an action
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>

        public MenuDetailsModel MenuActionMapping(String EncryptedID)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                MenuDetailsModel menuDetailsObj = new MenuDetailsModel();
                menuDetailsObj.EncryptedID = EncryptedID;

                UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == id).FirstOrDefault();
                menuDetailsObj.MenuName = umg_menuDetails.MenuName;

                // Modules List for Menu Action Mapping
                menuDetailsObj.MAS_Modules_ModuleList = new List<SelectListItem>();
                menuDetailsObj.MAS_Modules_ModuleList.Add(new SelectListItem { Text = "Select", Value = "-1" });
                List<MAS_Modules> mas_modulesList = localdbContext.MAS_Modules.Where(x => x.IsActive == true).OrderBy(c => c.ModuleName).ToList();
                if (mas_modulesList != null)
                {
                    if (mas_modulesList.Count() != 0)
                    {
                        foreach (var mas_modules in mas_modulesList)
                        {
                            SelectListItem selectListItem = new SelectListItem();
                            selectListItem.Text = mas_modules.ModuleName;
                            selectListItem.Value = Convert.ToString(mas_modules.ModuleID);
                            menuDetailsObj.MAS_Modules_ModuleList.Add(selectListItem);
                        }
                    }
                }

                // Area List for Menu Action Mapping
                menuDetailsObj.ControllerActionDetails_AreaList = new List<SelectListItem>();
                menuDetailsObj.ControllerActionDetails_AreaList.Add(new SelectListItem { Text = "Select", Value = "Select" });
                //menuDetailsObj.ControllerActionDetails_AreaList.Add(new SelectListItem { Text = "None", Value = "None" });
                List<UMG_ControllerActionDetails> umg_controllerActionDetailsList = localdbContext.UMG_ControllerActionDetails.Where(x => x.IsActive == true).ToList();//

                var areaNameList = umg_controllerActionDetailsList.Select(x => x.AreaName).Distinct().OrderBy(c => c);
                //List<String> areaNameStringList=areaNameList.ToList();
                //umg_controllerActionDetailsList = fdsa.ToList<UMG_ControllerActionDetails>();

                if (areaNameList != null)
                {
                    if (areaNameList.Count() != 0)
                    {
                        foreach (var areaName in areaNameList)
                        {
                            //string sCaid = Convert.ToString(umg_controllerActionDetails.CAID);
                            //if (!menuDetailsObj.ControllerActionDetails_AreaList.Any(x => x.Value == sCaid)) { continue; }
                            //if (menuDetailsObj.ControllerActionDetails_AreaList.Where(x=>x.Text== umg_controllerActionDetails.AreaName).)
                            if (areaName != "")
                            {
                                SelectListItem selectListItem = new SelectListItem();
                                selectListItem.Text = areaName;
                                selectListItem.Value = areaName;
                                menuDetailsObj.ControllerActionDetails_AreaList.Add(selectListItem);
                            }
                            else
                            {
                                menuDetailsObj.ControllerActionDetails_AreaList.Add(new SelectListItem { Text = "None", Value = "None" });
                            }
                        }
                    }
                }
                menuDetailsObj.ControllerActionDetails_ControllerList = new List<SelectListItem>();
                //menuDetailsObj.ControllerActionDetails_ControllerList.Add(new SelectListItem { Text = "Select", Value = "Select" });

                menuDetailsObj.ControllerActionDetails_ActionList = new List<SelectListItem>();
                //menuDetailsObj.ControllerActionDetails_ActionList.Add(new SelectListItem { Text = "Select", Value = "Select" });

                return menuDetailsObj;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }
        #endregion

        #region Get Controller List from Controller Action Details
        /// <summary>
        /// returns Controller List
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>

        public MenuDetailsModel ControllerList(MenuDetailsModel menuDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = menuDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                MenuDetailsModel menuDetailsObj = new MenuDetailsModel();
                menuDetailsObj.EncryptedID = menuDetailsModel.EncryptedID;

                menuDetailsObj.ControllerActionDetails_ControllerList = new List<SelectListItem>();
                menuDetailsObj.ControllerActionDetails_ControllerList.Add(new SelectListItem { Text = "Select", Value = "Select" });

                List<UMG_ControllerActionDetails> umg_ControllerActionDetailsList = null;
                if (menuDetailsModel.ControllerActionDetails_AreaListId == "None")
                {
                    umg_ControllerActionDetailsList = localdbContext.UMG_ControllerActionDetails.Where(x => x.AreaName == String.Empty && x.IsActive == true).ToList();
                }
                else
                {
                    umg_ControllerActionDetailsList = localdbContext.UMG_ControllerActionDetails.Where(x => x.AreaName == menuDetailsModel.ControllerActionDetails_AreaListId && x.IsActive == true).ToList();
                }
                var controllerNameList = umg_ControllerActionDetailsList.Select(x => x.ControllerName).OrderBy(c => c).Distinct();

                if (controllerNameList != null)
                {
                    if (controllerNameList.Count() != 0)
                    {
                        foreach (var controllerName in controllerNameList)
                        {
                            SelectListItem selectListItem = new SelectListItem();
                            selectListItem.Text = controllerName;
                            selectListItem.Value = controllerName;
                            menuDetailsObj.ControllerActionDetails_ControllerList.Add(selectListItem);
                        }
                    }
                }
                return menuDetailsObj;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }
        #endregion

        #region Get Action List from Controller Action Details
        /// <summary>
        /// returns Action List
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsModel ActionList(MenuDetailsModel menuDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = menuDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                MenuDetailsModel menuDetailsObj = new MenuDetailsModel();
                menuDetailsObj.EncryptedID = menuDetailsModel.EncryptedID;

                menuDetailsObj.ControllerActionDetails_ActionList = new List<SelectListItem>();
                menuDetailsObj.ControllerActionDetails_ActionList.Add(new SelectListItem { Text = "Select", Value = "Select" });


                // List<UMG_ControllerActionDetails> umg_ControllerActionDetailsList = localdbContext.UMG_ControllerActionDetails.Where(x => x.ControllerName == menuDetailsModel.ControllerActionDetails_ControllerListId && x.IsActive == true).ToList();

                // changed by m rafe on 23-12-19
                List<UMG_ControllerActionDetails> umg_ControllerActionDetailsList = localdbContext.UMG_ControllerActionDetails.Where(x => x.ControllerName == menuDetailsModel.ControllerActionDetails_ControllerListId && x.IsActive == true && x.IsForMenuActionMapping==true ).ToList();

                var actionNameList = umg_ControllerActionDetailsList.Select(x => x.ActionName). Distinct().OrderBy(c => c);

                if (actionNameList != null)
                {
                    if (actionNameList.Count() != 0)
                    {
                        foreach (var actionName in actionNameList)
                        {
                            SelectListItem selectListItem = new SelectListItem();
                            selectListItem.Text = actionName;
                            selectListItem.Value = actionName;
                            menuDetailsObj.ControllerActionDetails_ActionList.Add(selectListItem);
                        }
                    }
                }
                return menuDetailsObj;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }
        #endregion

        #region Map menu to action
        /// <summary>
        /// Maps MenuToAction
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsModel MapMenuToAction(MenuDetailsModel menuDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = menuDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                MenuDetailsModel menuDetailsObj = new MenuDetailsModel();
                menuDetailsObj.MenuDetailsResponseModel = new MenuDetailsResponseModel();

                UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == id).FirstOrDefault();

                MAS_Modules mas_modules = localdbContext.MAS_Modules.Where(x => x.ModuleID == menuDetailsModel.MAS_Modules_ModuleListId).FirstOrDefault();

                UMG_ControllerActionDetails umg_ControllerActionDetails = null;
                if (menuDetailsModel.ControllerActionDetails_AreaListId == "None")
                {
                    umg_ControllerActionDetails = localdbContext.UMG_ControllerActionDetails.Where(x => x.AreaName == "" && x.ControllerName == menuDetailsModel.ControllerActionDetails_ControllerListId && x.ActionName == menuDetailsModel.ControllerActionDetails_ActionListId).FirstOrDefault();
                }
                else
                {
                    umg_ControllerActionDetails = localdbContext.UMG_ControllerActionDetails.Where(x => x.AreaName == menuDetailsModel.ControllerActionDetails_AreaListId && x.ControllerName == menuDetailsModel.ControllerActionDetails_ControllerListId && x.ActionName == menuDetailsModel.ControllerActionDetails_ActionListId).FirstOrDefault();
                }

                UMG_MenuActionMapping umg_menuActionMapping = null;
                if (umg_menuDetails != null)
                {
                    if (mas_modules != null)
                    {
                        if (umg_ControllerActionDetails != null)
                        {
                            umg_menuActionMapping = new UMG_MenuActionMapping();
                            umg_menuActionMapping.ID = (localdbContext.UMG_MenuActionMapping.Any() ? localdbContext.UMG_MenuActionMapping.Max(a => a.ID) : 0) + 1; ;
                            umg_menuActionMapping.MenuID = umg_menuDetails.MenuID;
                            umg_menuActionMapping.CAID = umg_ControllerActionDetails.CAID;
                            umg_menuActionMapping.ModuleID = mas_modules.ModuleID;
                            localdbContext.UMG_MenuActionMapping.Add(umg_menuActionMapping);

                            #region 5-4-2019 For Table LOG by SB
                            UMG_MenuActionMapping_Log umg_MenuActionMapping_Log = new UMG_MenuActionMapping_Log();
                            umg_MenuActionMapping_Log.LogID = (localdbContext.UMG_MenuActionMapping_Log.Any() ? localdbContext.UMG_MenuActionMapping_Log.Max(a => a.LogID) : 0) + 1;
                            umg_MenuActionMapping_Log.ID = umg_menuActionMapping.ID;
                            umg_MenuActionMapping_Log.MenuID = umg_menuActionMapping.MenuID;
                            umg_MenuActionMapping_Log.CAID = umg_menuActionMapping.CAID;
                            umg_MenuActionMapping_Log.ModuleID = umg_menuActionMapping.ModuleID;
                            umg_MenuActionMapping_Log.UpdateDateTime = DateTime.Now;
                            umg_MenuActionMapping_Log.UserID = menuDetailsModel.UserIdForActivityLogFromSession;
                            umg_MenuActionMapping_Log.UserIPAddress = menuDetailsModel.UserIPAddress;
                            umg_MenuActionMapping_Log.ActionPerformed = "Insert";
                            localdbContext.UMG_MenuActionMapping_Log.Add(umg_MenuActionMapping_Log);
                            #endregion

                            localdbContext.SaveChanges();

                            // For Activity Log
                            String messageForActivityLog = "Menu Mapped to Action # \"" + umg_menuDetails.MenuName + "\" Menu Mapped to \"" + umg_ControllerActionDetails.ActionName + "\" Action.";
                            if (messageForActivityLog.Length < 1000)
                                ApiCommonFunctions.SystemUserActivityLog(menuDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                            else
                            {
                                messageForActivityLog = messageForActivityLog.Substring(0, 999);
                                ApiCommonFunctions.SystemUserActivityLog(menuDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                            }

                            menuDetailsObj.MenuDetailsResponseModel.Result = true;
                            menuDetailsObj.MenuDetailsResponseModel.Message = "Menu Action Mapped Successfully";
                        }
                        else
                        {
                            menuDetailsObj.MenuDetailsResponseModel.Result = false;
                            menuDetailsObj.MenuDetailsResponseModel.Message = "Controller Action not found ";
                        }
                    }
                    else
                    {
                        menuDetailsObj.MenuDetailsResponseModel.Result = false;
                        menuDetailsObj.MenuDetailsResponseModel.Message = "Module not found";
                    }
                }
                else
                {
                    menuDetailsObj.MenuDetailsResponseModel.Result = false;
                    menuDetailsObj.MenuDetailsResponseModel.Message = "Menu details not found";
                }
                return menuDetailsObj;
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }
        #endregion

        #region Unmap menu to action
        /// <summary>
        /// UnmapMenuToAction
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>
        public MenuDetailsModel UnmapMenuToAction(MenuDetailsModel menuDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = menuDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                MenuDetailsModel menuDetailsObj = new MenuDetailsModel();
                menuDetailsObj.MenuDetailsResponseModel = new MenuDetailsResponseModel();

                UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == id).FirstOrDefault();

                MAS_Modules mas_modules = localdbContext.MAS_Modules.Where(x => x.ModuleID == menuDetailsModel.MAS_Modules_ModuleListId).FirstOrDefault();
                UMG_ControllerActionDetails umg_ControllerActionDetails = null;
                if (menuDetailsModel.ControllerActionDetails_AreaListId == "None")
                {
                    umg_ControllerActionDetails = localdbContext.UMG_ControllerActionDetails.Where(x => x.AreaName == "" && x.ControllerName == menuDetailsModel.ControllerActionDetails_ControllerListId && x.ActionName == menuDetailsModel.ControllerActionDetails_ActionListId).FirstOrDefault();
                }
                else
                {
                    umg_ControllerActionDetails = localdbContext.UMG_ControllerActionDetails.Where(x => x.AreaName == menuDetailsModel.ControllerActionDetails_AreaListId && x.ControllerName == menuDetailsModel.ControllerActionDetails_ControllerListId && x.ActionName == menuDetailsModel.ControllerActionDetails_ActionListId).FirstOrDefault();
                }
                UMG_MenuActionMapping umg_menuActionMapping = null;
                if (umg_menuDetails != null)
                {
                    if (mas_modules != null)
                    {
                        if (umg_ControllerActionDetails != null)
                        {
                            umg_menuActionMapping = localdbContext.UMG_MenuActionMapping.Where(x => x.MenuID == umg_menuDetails.MenuID && x.CAID == umg_ControllerActionDetails.CAID && x.ModuleID == mas_modules.ModuleID).FirstOrDefault();
                            #region 5-4-2019 For Table LOG by SB
                            if (umg_menuActionMapping != null)
                            {
                                UMG_MenuActionMapping_Log umg_MenuActionMapping_Log = new UMG_MenuActionMapping_Log();
                                umg_MenuActionMapping_Log.LogID = (localdbContext.UMG_MenuActionMapping_Log.Any() ? localdbContext.UMG_MenuActionMapping_Log.Max(a => a.LogID) : 0) + 1;
                                umg_MenuActionMapping_Log.ID = umg_menuActionMapping.ID;
                                umg_MenuActionMapping_Log.MenuID = umg_menuActionMapping.MenuID;
                                umg_MenuActionMapping_Log.CAID = umg_menuActionMapping.CAID;
                                umg_MenuActionMapping_Log.ModuleID = umg_menuActionMapping.ModuleID;
                                umg_MenuActionMapping_Log.UpdateDateTime = DateTime.Now;
                                umg_MenuActionMapping_Log.UserID = menuDetailsModel.UserIdForActivityLogFromSession;
                                umg_MenuActionMapping_Log.UserIPAddress = menuDetailsModel.UserIPAddress;
                                umg_MenuActionMapping_Log.ActionPerformed = "Delete";
                                localdbContext.UMG_MenuActionMapping_Log.Add(umg_MenuActionMapping_Log);

                                localdbContext.UMG_MenuActionMapping.Remove(umg_menuActionMapping);
                            }
                            #endregion

                            localdbContext.SaveChanges();

                            // For Activity Log
                            String messageForActivityLog = "Menu Unmapped to Action # \"" + umg_menuDetails.MenuName + "\" Menu Unmapped to \"" + umg_ControllerActionDetails.ActionName + "\" Action.";
                            if (messageForActivityLog.Length < 1000)
                                ApiCommonFunctions.SystemUserActivityLog(menuDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                            else
                            {
                                messageForActivityLog = messageForActivityLog.Substring(0, 999);
                                ApiCommonFunctions.SystemUserActivityLog(menuDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.MenuActionDetails), messageForActivityLog);
                            }
                            menuDetailsObj.MenuDetailsResponseModel.Result = true;
                            menuDetailsObj.MenuDetailsResponseModel.Message = "Menu Action Unmapped Successfully";
                        }
                        else
                        {
                            menuDetailsObj.MenuDetailsResponseModel.Result = false;
                            menuDetailsObj.MenuDetailsResponseModel.Message = "Controller Action not found";
                        }
                    }
                    else
                    {
                        menuDetailsObj.MenuDetailsResponseModel.Result = false;
                        menuDetailsObj.MenuDetailsResponseModel.Message = "Module not found";
                    }
                }
                else
                {
                    menuDetailsObj.MenuDetailsResponseModel.Result = false;
                    menuDetailsObj.MenuDetailsResponseModel.Message = "Menu not found";
                }
                return menuDetailsObj;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }
        #endregion

        #region MapUnmapMenuActionButton
        /// <summary>
        /// MapUnmapMenuActionButton
        /// </summary>
        /// <param name="menuDetailsModel"></param>
        /// <returns></returns>

        public MenuDetailsModel MapUnmapMenuActionButton(MenuDetailsModel menuDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = menuDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["MenuID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                MenuDetailsModel menuDetailsObj = new MenuDetailsModel();
                menuDetailsObj.MenuDetailsResponseModel = new MenuDetailsResponseModel();

                UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == id).FirstOrDefault();

                MAS_Modules mas_modules = localdbContext.MAS_Modules.Where(x => x.ModuleID == menuDetailsModel.MAS_Modules_ModuleListId).FirstOrDefault();

                // Added By Shubham Bhagat on 29-10-2018
                UMG_ControllerActionDetails umg_ControllerActionDetails = null;
                if (menuDetailsModel.ControllerActionDetails_AreaListId == "None")
                {
                    umg_ControllerActionDetails = localdbContext.UMG_ControllerActionDetails.Where(x => x.AreaName == "" && x.ControllerName == menuDetailsModel.ControllerActionDetails_ControllerListId && x.ActionName == menuDetailsModel.ControllerActionDetails_ActionListId).FirstOrDefault();
                }
                else
                {
                    umg_ControllerActionDetails = localdbContext.UMG_ControllerActionDetails.Where(x => x.AreaName == menuDetailsModel.ControllerActionDetails_AreaListId && x.ControllerName == menuDetailsModel.ControllerActionDetails_ControllerListId && x.ActionName == menuDetailsModel.ControllerActionDetails_ActionListId).FirstOrDefault();
                }


                UMG_MenuActionMapping umg_menuActionMapping = null;
                if (umg_menuDetails != null)
                {
                    if (mas_modules != null)
                    {
                        if (umg_ControllerActionDetails != null)
                        {
                            umg_menuActionMapping = localdbContext.UMG_MenuActionMapping.Where(x => x.MenuID == umg_menuDetails.MenuID && x.CAID == umg_ControllerActionDetails.CAID && x.ModuleID == mas_modules.ModuleID).FirstOrDefault();

                            menuDetailsObj.MapUnmapMenuActionButton = (umg_menuActionMapping == null) ? "<a href = '#' class='btn btn-success' onclick=MapMenuToAction();>Map Menu To Action</a>" : "<a href = '#' class='btn btn-warning' onclick=UnmapMenuToAction();>UnMap Menu To Action</a>";
                        }
                    }
                }
                return menuDetailsObj;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (localdbContext != null)
                {
                    localdbContext.Dispose();
                }
            }
        }


        #endregion

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

    }
}
