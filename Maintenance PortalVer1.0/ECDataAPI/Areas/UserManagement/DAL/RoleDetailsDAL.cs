using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity.KaveriEntities;
using ECDataAPI.Entity.KaigrSearchDB;
using Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace ECDataAPI.Areas.UserManagement.DAL
{
    public class RoleDetailsDAL : IRoleDetails, IDisposable
    {
        #region Properties
        private KaveriEntities dbContext = null;
        private Dictionary<String, String> decryptedParameters = null;
        private String[] encryptedParameters = null;

        #endregion

        #region Retrive Role Details List
        public IEnumerable<RoleDetailsModel> RoleDetailsList(short userLoggedInRole)
        {
            List<RoleDetailsModel> roleDetailsList = null;
            RoleDetailsModel roleDetailsModel = null;

            using (dbContext = new KaveriEntities())
            {
                try
                {
                    roleDetailsList = new List<RoleDetailsModel>();
                    // umg_RoleDetailsList = dbContext.UMG_RoleDetails.ToList();
                    var umg_RoleDetailsList = dbContext.USP_Populate_RoleDetails(false
                        ).ToList();

                    if (umg_RoleDetailsList != null)
                    {

                        foreach (var umg_RoleDetails in umg_RoleDetailsList)
                        {
                            roleDetailsModel = new RoleDetailsModel();

                            // For Encrypting id
                            roleDetailsModel.EncryptedID = URLEncrypt.EncryptParameters(new String[] {
                                                                    "RoleID="+umg_RoleDetails.RoleID
                                                        });
                            roleDetailsModel.RoleName = umg_RoleDetails.RoleName;
                            roleDetailsModel.RoleNameR = umg_RoleDetails.RoleNameR;
                            roleDetailsModel.IsActive = umg_RoleDetails.IsActive;
                            // commented by shubham bhagat on 12-04-2019 due to requirement change
                            //roleDetailsModel.MapMenuButton = (userLoggedInRole == (short)ApiCommonEnum.RoleDetails.DepartmentAdmin) ? "<a href='#' onclick=RoleMenuMapping('" + roleDetailsModel.EncryptedID + "');><i class='fa fa-plus-square fa-2x' aria-hidden='true' style='color:black;' ></i></a>" : "<i class='fa fa-times fa-2x' aria-hidden='true' style='color:black;'></i>";


                            if (userLoggedInRole == (short)ApiCommonEnum.RoleDetails.TechnicalAdmin)

                            {
                                roleDetailsModel.EditRoleButton = "<a href='#' onclick=showEditRoleDetailForm('" + roleDetailsModel.EncryptedID + "');><i class='fa fa-pencil  fa-2x' aria-hidden='true' style='color:black;' ></i></a>";

                                roleDetailsModel.DeleteRoleButton = "<a href='#' onclick=deleteRoleDetails('" + roleDetailsModel.EncryptedID + "'); ><i class='fa fa-trash fa-2x' aria-hidden='true' style='color:black;'></i></a>";
                                roleDetailsModel.MapMenuButton = "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";
                            }
                            //if (userLoggedInRole == (short)ApiCommonEnum.RoleDetails.DepartmentAdmin)

                            //{
                            //    roleDetailsModel.EditRoleButton = "";
                            //    roleDetailsModel.DeleteRoleButton = "";
                            //    roleDetailsModel.MapMenuButton = "<a href='#' onclick=RoleMenuMapping('" + roleDetailsModel.EncryptedID + "');><i class='fa fa-plus-square fa-2x' aria-hidden='true' style='color:black;' ></i></a>";
                            //}
                            #region Deptadmin functionality to aigr comp
                            //Added by mayank on 15-07-2021 start
                            if (userLoggedInRole == (short)ApiCommonEnum.RoleDetails.DepartmentAdmin || userLoggedInRole == (short)ApiCommonEnum.RoleDetails.AIGRComp)

                            {
                                roleDetailsModel.EditRoleButton = "";
                                roleDetailsModel.DeleteRoleButton = "";
                                roleDetailsModel.MapMenuButton = "<a href='#' onclick=RoleMenuMapping('" + roleDetailsModel.EncryptedID + "');><i class='fa fa-plus-square fa-2x' aria-hidden='true' style='color:black;' ></i></a>";
                            }
                            //end 
                            #endregion

                            if (string.IsNullOrEmpty(umg_RoleDetails.AssignedMenus))
                            {
                                roleDetailsModel.AssignedMenus = "";
                            }
                            else
                            {
                                string sTempString = umg_RoleDetails.AssignedMenus.Trim();
                                roleDetailsModel.AssignedMenus = sTempString.Last() == ',' ? sTempString.Substring(0, sTempString.Length - 1) : sTempString;
                            }

                            //roleDetailsModel.EditRoleButton = (userLoggedInRole == (short)ApiCommonEnum.RoleDetails.TechnicalAdmin) ? "<a href='#' onclick=showEditRoleDetailForm('" + roleDetailsModel.EncryptedID + "');><i class='fa fa-pencil  fa-2x' aria-hidden='true' style='color:black;' ></i></a>" : "<i class='fa fa-times fa-2x' aria-hidden='true' style='color:black;'></i>";
                            // changed by m rafe on 27-11-19
                            //menuDetailsObj.IsActiveString = menuDetailsModel.IsActive ? "<i class='fa fa-check' aria-hidden='true' style='color:black;'></i>" : "<i class='fa fa-times' aria-hidden='true' style='color:black;'></i>";

                            roleDetailsList.Add(roleDetailsModel);
                        }
                        return roleDetailsList;
                    }
                    else
                    {
                        return roleDetailsList;
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

        #region Add Role Details
        public RoleDetailsResponseModel AddRoleDetails(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                dbContext = new KaveriEntities();
                UMG_RoleDetails umg_RoleDetailsObj = new UMG_RoleDetails();

                // To trim white spaces
                roleDetailsModel.RoleName = roleDetailsModel.RoleName.Trim();
                roleDetailsModel.RoleNameR = roleDetailsModel.RoleNameR.Trim();

                // For Checking if Role Name Already exists
                List<UMG_RoleDetails> umg_RoleDetailsList = dbContext.UMG_RoleDetails.ToList();
                if (umg_RoleDetailsList != null)
                {
                    if (umg_RoleDetailsList.Count() != 0)
                    {
                        foreach (UMG_RoleDetails umg_RoleDetails in umg_RoleDetailsList)
                        {
                            if (umg_RoleDetails.RoleName.ToLower().Equals(roleDetailsModel.RoleName.ToLower()))
                            {
                                RoleDetailsResponseModel roleDetailsResponseModel = new RoleDetailsResponseModel();
                                roleDetailsResponseModel.Message = "Role Name Already Exists. Please try another name.";
                                roleDetailsResponseModel.Status = false;
                                return roleDetailsResponseModel;
                            }
                        }
                    }
                }

                using (TransactionScope ts = new TransactionScope())
                {
                    #region 10-04-2019 For Level drop down validation by shubham bhagat  
                    short levelIDShort = Convert.ToInt16(roleDetailsModel.LevelID);
                    bool isExist = dbContext.UMG_LevelDetails.Where(x => x.LevelID == levelIDShort).Any();
                    if (!isExist)
                    {
                        RoleDetailsResponseModel responseOBJ = new RoleDetailsResponseModel();
                        responseOBJ.Message = "Level not Exists.";
                        responseOBJ.Status = false;
                        return responseOBJ;
                    }
                    #endregion

                    umg_RoleDetailsObj.RoleID = Convert.ToInt16((dbContext.UMG_RoleDetails.Any() ? dbContext.UMG_RoleDetails.Max(a => a.RoleID) : 0) + 1);
                    umg_RoleDetailsObj.RoleName = roleDetailsModel.RoleName;
                    umg_RoleDetailsObj.RoleNameR = roleDetailsModel.RoleNameR;
                    umg_RoleDetailsObj.IsActive = roleDetailsModel.IsActive;
                    dbContext.UMG_RoleDetails.Add(umg_RoleDetailsObj);

                    #region 10-04-2019 For Level drop down by shubham bhagat 
                    UMG_RoleLevelMapping umg_RoleLevelMapping = new UMG_RoleLevelMapping();
                    umg_RoleLevelMapping.ID = Convert.ToInt16((dbContext.UMG_RoleLevelMapping.Any() ? dbContext.UMG_RoleLevelMapping.Max(a => a.ID) : 0) + 1);
                    umg_RoleLevelMapping.RoleID = umg_RoleDetailsObj.RoleID;
                    umg_RoleLevelMapping.LevelID = Convert.ToInt16(roleDetailsModel.LevelID);
                    dbContext.UMG_RoleLevelMapping.Add(umg_RoleLevelMapping);
                    #endregion
                    // For Activity Log
                    String messageForActivityLog = "Role Detail Added # " + roleDetailsModel.RoleName + "- Role Detail Added.";
                    if (messageForActivityLog.Length < 1000)
                        ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                    else
                    {
                        messageForActivityLog = messageForActivityLog.Substring(0, 999);
                        ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                    }
                    dbContext.SaveChanges();

                    dbContext.USP_PERFORM_DEFAULT_OPERATIONS_FOR_ROLE(umg_RoleDetailsObj.RoleID);

                    ts.Complete();
                    // Response model
                    RoleDetailsResponseModel roleDetailsResponseModel = new RoleDetailsResponseModel();
                    roleDetailsResponseModel.Message = "Role Details Added Successfully";
                    roleDetailsResponseModel.Status = true;
                    return roleDetailsResponseModel;
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

        #region Edit Role Details
        public RoleDetailsModel EditRoleDetails(String EncryptedID)
        {
            RoleDetailsModel roleDetailsModel = null;
            UMG_RoleDetails umg_RoleDetails = null;
            try
            {
                encryptedParameters = EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                dbContext = new KaveriEntities();
                roleDetailsModel = new RoleDetailsModel();
                umg_RoleDetails = dbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                if (umg_RoleDetails != null)
                {
                    roleDetailsModel.RoleID = umg_RoleDetails.RoleID;
                    roleDetailsModel.RoleName = umg_RoleDetails.RoleName;
                    roleDetailsModel.RoleNameR = umg_RoleDetails.RoleNameR;
                    roleDetailsModel.IsActive = umg_RoleDetails.IsActive;

                    #region 09-04-2019 For Level drop down by shubham bhagat 
                    var roleLevelOBJ = dbContext.UMG_RoleLevelMapping.Where(x => x.RoleID == id).FirstOrDefault();
                    if (roleLevelOBJ != null)
                    {
                        roleDetailsModel.LevelID = roleLevelOBJ.LevelID;
                    }
                    roleDetailsModel.LevelList = GetLevelList();
                    #endregion

                    roleDetailsModel.RoleDetailsResponseModel = new RoleDetailsResponseModel();
                    roleDetailsModel.RoleDetailsResponseModel.Status = true;
                    roleDetailsModel.RoleDetailsResponseModel.Message = "Role Details found.";
                    return roleDetailsModel;
                }
                else
                {
                    roleDetailsModel.RoleDetailsResponseModel = new RoleDetailsResponseModel();
                    roleDetailsModel.RoleDetailsResponseModel.Status = false;
                    roleDetailsModel.RoleDetailsResponseModel.Message = "Role Details not found.";
                    return roleDetailsModel;
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

        #region Update Role Details
        public RoleDetailsResponseModel UpdateRoleDetails(RoleDetailsModel roleDetailsModel)
        {
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                dbContext = new KaveriEntities();

                // To trim white spaces
                roleDetailsModel.RoleName = roleDetailsModel.RoleName.Trim();
                roleDetailsModel.RoleNameR = roleDetailsModel.RoleNameR.Trim();

                //For Checking if Role Name Already exists
                List<UMG_RoleDetails> umg_RoleDetailsList = dbContext.UMG_RoleDetails.Where(x => x.RoleID != id).ToList();

                if (umg_RoleDetailsList != null)
                {
                    if (umg_RoleDetailsList.Count() != 0)
                    {
                        foreach (UMG_RoleDetails umg_RoleDetails in umg_RoleDetailsList)
                        {
                            if (umg_RoleDetails.RoleName.ToLower().Equals(roleDetailsModel.RoleName.ToLower()))
                            {
                                RoleDetailsResponseModel roleDetailsResponseModel = new RoleDetailsResponseModel();
                                roleDetailsResponseModel.Message = "Role Name Already Exists. Please try another name.";
                                roleDetailsResponseModel.Status = false;
                                return roleDetailsResponseModel;
                            }
                        }
                    }
                }

                #region 10-04-2019 For Level drop down validation by shubham bhagat  
                short levelIDShort = Convert.ToInt16(roleDetailsModel.LevelID);
                bool isExist = dbContext.UMG_LevelDetails.Where(x => x.LevelID == levelIDShort).Any();
                if (!isExist)
                {
                    RoleDetailsResponseModel responseOBJ = new RoleDetailsResponseModel();
                    responseOBJ.Message = "Level not Exists.";
                    responseOBJ.Status = false;
                    return responseOBJ;
                }
                #endregion

                using (TransactionScope ts = new TransactionScope())
                {
                    UMG_RoleDetails umg_RoleDetails = dbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();

                    if (umg_RoleDetails != null)
                    {
                        #region 3-4-2019 For Table LOG by SB
                        UMG_RoleDetails_Log umg_RoleDetails_Log = new UMG_RoleDetails_Log();
                        umg_RoleDetails_Log.LogID = Convert.ToInt16((dbContext.UMG_RoleDetails_Log.Any() ? dbContext.UMG_RoleDetails_Log.Max(a => a.LogID) : 0) + 1);
                        umg_RoleDetails_Log.RoleID = umg_RoleDetails.RoleID;
                        umg_RoleDetails_Log.RoleName = umg_RoleDetails.RoleName;
                        umg_RoleDetails_Log.RoleNameR = umg_RoleDetails.RoleNameR;
                        umg_RoleDetails_Log.IsActive = umg_RoleDetails.IsActive;
                        umg_RoleDetails_Log.UpdateDateTime = DateTime.Now;
                        umg_RoleDetails_Log.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                        umg_RoleDetails_Log.UserIpAddress = roleDetailsModel.UserIPAddress;
                        umg_RoleDetails_Log.ActionPerformed = "Update";
                        dbContext.UMG_RoleDetails_Log.Add(umg_RoleDetails_Log);
                        #endregion

                        umg_RoleDetails.RoleName = roleDetailsModel.RoleName;
                        umg_RoleDetails.RoleNameR = roleDetailsModel.RoleNameR;
                        // Changes on 15-12-2018 Final Changes in User Management
                        //umg_RoleDetails.IsActive = roleDetailsModel.IsActive
                        // Changed on 01-04-2019
                        //umg_RoleDetails.IsActive = umg_RoleDetails.IsActive;
                        umg_RoleDetails.IsActive = roleDetailsModel.IsActive;

                        #region 10-04-2019 For Level drop down by shubham bhagat 
                        UMG_RoleLevelMapping umg_RoleLevelMapping = dbContext.UMG_RoleLevelMapping.Where(x => x.LevelID == roleDetailsModel.OldLevelID && x.RoleID == umg_RoleDetails.RoleID).FirstOrDefault();
                        if (umg_RoleLevelMapping != null)
                        {
                            umg_RoleLevelMapping.LevelID = Convert.ToInt16(roleDetailsModel.LevelID);
                        }
                        #endregion

                        // For Activity Log
                        String messageForActivityLog = "Role Detail Updated # " + roleDetailsModel.RoleName + "- Role Detail Updated.";
                        if (messageForActivityLog.Length < 1000)
                            ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                        else
                        {
                            messageForActivityLog = messageForActivityLog.Substring(0, 999);
                            ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                        }
                        dbContext.SaveChanges();
                        ts.Complete();

                        RoleDetailsResponseModel roleDetailsResponseModel = new RoleDetailsResponseModel();
                        roleDetailsResponseModel.Message = "Role Details Updated Successfully";
                        roleDetailsResponseModel.Status = true;
                        return roleDetailsResponseModel;
                    }
                    else
                    {
                        RoleDetailsResponseModel roleDetailsResponseModel = new RoleDetailsResponseModel();
                        roleDetailsResponseModel.Message = "Role Details not Updated ";
                        roleDetailsResponseModel.Status = false;
                        return roleDetailsResponseModel;
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

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        #region Delete Role Details
        public RoleDetailsResponseModel DeleteRoleDetails(String EncryptedID, long UserIdForActivityLogFromSession)
        {
            UMG_RoleDetails umg_RoleDetails = null;

            try
            {
                encryptedParameters = EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                using (TransactionScope ts = new TransactionScope())
                {
                    RoleDetailsResponseModel roleDetailsResponseModel = new RoleDetailsResponseModel();
                    dbContext = new KaveriEntities();
                    umg_RoleDetails = dbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                    if (umg_RoleDetails != null)
                    {

                        if (dbContext.UMG_RoleActionAuth.Any(x => x.RoleID == id))
                        {

                            roleDetailsResponseModel.Message = "Role is mapped to Actions. Kindly remove action mapping for the role and proceed.";
                            roleDetailsResponseModel.Status = true;
                            return roleDetailsResponseModel;
                        }


                        if (dbContext.UMG_RoleMenuMapping.Any(x => x.RoleID == id))
                        {
                            roleDetailsResponseModel.Message = "Role is mapped to Menus. Kindly remove menu mapping for the role and proceed.";
                            roleDetailsResponseModel.Status = true;
                            return roleDetailsResponseModel;

                        }


                        String roleName = umg_RoleDetails.RoleName;
                        dbContext.UMG_RoleDetails.Remove(umg_RoleDetails);


                        // added by m rafe  on 03-12-19
                        var RoleLevelMapList = dbContext.UMG_RoleLevelMapping.Where(x => x.RoleID == id).ToList();

                        if (RoleLevelMapList != null && RoleLevelMapList.Count > 0)
                            dbContext.UMG_RoleLevelMapping.RemoveRange(RoleLevelMapList);

                        // For Activity Log
                        String messageForActivityLog = "Role Detail Deleted # " + roleName + "- Role Detail Deleted.";
                        if (messageForActivityLog.Length < 1000)
                            ApiCommonFunctions.SystemUserActivityLog(UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                        else
                        {
                            messageForActivityLog = messageForActivityLog.Substring(0, 999);
                            ApiCommonFunctions.SystemUserActivityLog(UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                        }
                        dbContext.SaveChanges();
                        ts.Complete();
                        roleDetailsResponseModel.Message = "Role Details Deleted Successfully";
                        roleDetailsResponseModel.Status = true;
                        return roleDetailsResponseModel;
                    }
                    else
                    {
                        roleDetailsResponseModel.Message = "Role Details Not Deleted ";
                        roleDetailsResponseModel.Status = true;
                        return roleDetailsResponseModel;
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

        #region Get Parent Menu List
        // Get Parent Menu Details List 
        public RoleDetailsModel RoleMenuMapping(String EncryptedID, short RoleIDFromSession)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = EncryptedID;

                UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                roleDetailsObj.RoleName = umg_roleDetails.RoleName;

                roleDetailsObj.ParentMenuDetailsList = new List<SelectListItem>();
                roleDetailsObj.ParentMenuDetailsList.Add(new SelectListItem { Text = "Select", Value = "0" });
                roleDetailsObj.FirstChildMenuDetailsList = new List<SelectListItem>();
                roleDetailsObj.SecondChildMenuDetailsList = new List<SelectListItem>();

                // Uncommented by Shubham Bhagat on 22-04-2019 due to requirement change
                #region cOMMENTED BY M RAFE ON 26-11-19 
                //if (RoleIDFromSession == (short)ApiCommonEnum.RoleDetails.TechnicalAdmin)
                //{   
                //    List<UMG_MenuDetails> umg_menuDetailsList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == 0 && x.IsActive == true).ToList();
                //    if (umg_menuDetailsList != null)
                //    {
                //        if (umg_menuDetailsList.Count() != 0)
                //        {
                //            foreach (var menuDetails in umg_menuDetailsList)
                //            {
                //                SelectListItem selectListItem = new SelectListItem();
                //                selectListItem.Text = menuDetails.MenuName;
                //                selectListItem.Value = Convert.ToString(menuDetails.MenuID);
                //                roleDetailsObj.ParentMenuDetailsList.Add(selectListItem);
                //            }
                //        }
                //    }
                //}
                //else
                //{
                //    // Changes on 15-12-2018 Final Changes in User Management
                //    List<UMG_RoleMenuMapping> umg_roleMenuMappingList = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id).ToList();

                //    if (umg_roleMenuMappingList != null)
                //    {
                //        if (umg_roleMenuMappingList.Count() != 0)
                //        {
                //            foreach (var roleMenuMapping in umg_roleMenuMappingList)
                //            {
                //                UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == 0 && x.IsActive == true && x.MenuID == roleMenuMapping.MenuID).FirstOrDefault();
                //                if (umg_menuDetails != null)
                //                {
                //                    SelectListItem selectListItem = new SelectListItem();
                //                    selectListItem.Text = umg_menuDetails.MenuName;
                //                    selectListItem.Value = Convert.ToString(roleMenuMapping.MenuID);
                //                    roleDetailsObj.ParentMenuDetailsList.Add(selectListItem);
                //                }
                //            }
                //        }
                //    }
                //} 
                #endregion



                var umg_menuDetailsList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == 0 && x.IsActive == true).OrderBy(c => c.MenuName).Select(m=> new { m.MenuID,m.MenuName}) . ToList();
                if (umg_menuDetailsList != null)
                {
                    if (umg_menuDetailsList.Count() != 0)
                    {
                        foreach (var menuDetails in umg_menuDetailsList)
                        {
                            SelectListItem selectListItem = new SelectListItem();
                            selectListItem.Text = menuDetails.MenuName;
                            //  selectListItem.Value = Convert.ToString(menuDetails.MenuID);

                            //Changed by m rafe on 24-12-19

                            selectListItem.Value = URLEncrypt.EncryptParameters(new String[] {
                                                                    "ParentMenuID="+menuDetails.MenuID
                                                        });


                            roleDetailsObj.ParentMenuDetailsList.Add(selectListItem);
                        }
                    }
                }



                return roleDetailsObj;
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

        #region Get First Child Menu Details List and button of menu mapped or unmapped of parent menu
        public RoleDetailsModel FirstChildMenuList(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;
                roleDetailsObj.ParentMenuDetailsList = new List<SelectListItem>();
                roleDetailsObj.FirstChildMenuDetailsList = new List<SelectListItem>();
                roleDetailsObj.FirstChildMenuDetailsList.Add(new SelectListItem { Text = "Select", Value = "0" });
                roleDetailsObj.SecondChildMenuDetailsList = new List<SelectListItem>();

                // Get First Child Menu Details List 
                List<UMG_MenuDetails> umg_FirstChildDetailsList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == roleDetailsModel.ParentMenuDetailsId && x.IsActive == true).OrderBy(c => c.MenuName).ToList();
                if (umg_FirstChildDetailsList != null)
                {
                    if (umg_FirstChildDetailsList.Count() != 0)
                    {
                        foreach (var firstChildMenuDetails in umg_FirstChildDetailsList)
                        {

                            // rafe 25-11-19
                            // if (firstChildMenuDetails.MenuID != (int)ApiCommonEnum.ParentMenuIdEnum.RoleMenu)
                            {

                                SelectListItem selectListItem = new SelectListItem();
                                selectListItem.Text = firstChildMenuDetails.MenuName;

                                //selectListItem.Value = Convert.ToString(firstChildMenuDetails.MenuID);
                               
                                selectListItem.Value = URLEncrypt.EncryptParameters(new String[] {
                                                                    "ParentMenuID="+firstChildMenuDetails.MenuID
                                                        });

                                roleDetailsObj.FirstChildMenuDetailsList.Add(selectListItem);
                            }

                        }
                    }
                }

                // Changes on 19-12-2018 Final Changes in User Management
                //List<UMG_RoleMenuMapping> umg_roleMenuMappingList = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id).ToList();

                //if (umg_roleMenuMappingList != null)
                //{
                //    if (umg_roleMenuMappingList.Count() != 0)
                //    {
                //        foreach (var roleMenuMapping in umg_roleMenuMappingList)
                //        {
                //            UMG_MenuDetails umg_FirstChildDetails = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == roleDetailsModel.ParentMenuDetailsId && x.IsActive == true && x.MenuID == roleMenuMapping.MenuID).FirstOrDefault();
                //            if (umg_FirstChildDetails != null)
                //            {
                //                SelectListItem selectListItem = new SelectListItem();
                //                selectListItem.Text = umg_FirstChildDetails.MenuName;
                //                selectListItem.Value = Convert.ToString(umg_FirstChildDetails.MenuID);
                //                roleDetailsObj.FirstChildMenuDetailsList.Add(selectListItem);
                //            }
                //        }
                //    }
                //}

                UMG_RoleMenuMapping umg_roleMenuMapping = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == roleDetailsModel.ParentMenuDetailsId).FirstOrDefault();

                // Uncommented and if condition added by Shubham Bhagat on 22-04-2019 due to requirement change
                #region cOMMENTED BY M RAFE ON 26-11-19 

                //if (roleDetailsModel.RoleIDFromSession == (short)ApiCommonEnum.RoleDetails.TechnicalAdmin)
                //{
                //    roleDetailsObj.MapUnmapButtonForParent = (umg_roleMenuMapping == null) ? "<a href = '#' class='btn btn-success' onclick=MapParentMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>" : "<a href = '#' class='btn btn-warning' onclick=UnmapParentMenu('" + roleDetailsModel.EncryptedID + "');>UnMap</a>";
                //}
                //else
                //{
                //    roleDetailsObj.MapUnmapButtonForParent = "";
                //} 
                #endregion


                roleDetailsObj.MapUnmapButtonForParent = (umg_roleMenuMapping == null) ? "<a href = '#' class='btn btn-success' onclick=MapParentMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>" : "<a href = '#' class='btn btn-warning' onclick=UnmapParentMenu('" + roleDetailsModel.EncryptedID + "');>UnMap</a>";

                return roleDetailsObj;
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

        #region Get Second Child Menu Details List and button of menu mapped or unmapped of first Child menu
        public RoleDetailsModel SecondChildMenuList(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;
                roleDetailsObj.ParentMenuDetailsList = new List<SelectListItem>();
                roleDetailsObj.FirstChildMenuDetailsList = new List<SelectListItem>();
                roleDetailsObj.SecondChildMenuDetailsList = new List<SelectListItem>();
                roleDetailsObj.SecondChildMenuDetailsList.Add(new SelectListItem { Text = "Select", Value = "0" });

                // For checking if parent menu is mapped or not 
                UMG_MenuDetails firstChildMenu = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();

                UMG_RoleMenuMapping umg_roleMenuMappingForParentMenu = null;

                if (firstChildMenu != null)
                {
                    umg_roleMenuMappingForParentMenu = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == firstChildMenu.ParentID).FirstOrDefault();
                }

                // For Sending button of map/unmap for a particular menu
                if (umg_roleMenuMappingForParentMenu == null)
                {
                    roleDetailsObj.IsParentMenuMapped = false;
                    //roleDetailsObj.MapUnmapButtonForFirstChild = "";
                }
                else
                {
                    roleDetailsObj.IsParentMenuMapped = true;

                    //// Get Second Child Menu Details List 
                    List<UMG_MenuDetails> umg_SecondChildDetailsList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == roleDetailsModel.FirstChildMenuDetailsId && x.IsActive == true).OrderBy(c => c.MenuName).ToList();
                    if (umg_SecondChildDetailsList != null)
                    {
                        if (umg_SecondChildDetailsList.Count() != 0)
                        {
                            foreach (var secondChildMenuDetails in umg_SecondChildDetailsList)
                            {
                                SelectListItem selectListItem = new SelectListItem();
                                selectListItem.Text = secondChildMenuDetails.MenuName;
                                selectListItem.Value = Convert.ToString(secondChildMenuDetails.MenuID);
                                roleDetailsObj.SecondChildMenuDetailsList.Add(selectListItem);
                            }
                        }
                    }



                    //// Changes on 19-12-2018 Final Changes in User Management
                    //List<UMG_RoleMenuMapping> umg_roleMenuMappingList = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id).ToList();

                    //if (umg_roleMenuMappingList != null)
                    //{
                    //    if (umg_roleMenuMappingList.Count() != 0)
                    //    {
                    //        foreach (var roleMenuMapping in umg_roleMenuMappingList)
                    //        {
                    //            UMG_MenuDetails umg_SecondChildDetails = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == roleDetailsModel.FirstChildMenuDetailsId && x.IsActive == true && x.MenuID == roleMenuMapping.MenuID).FirstOrDefault();
                    //            if (umg_SecondChildDetails != null)
                    //            {
                    //                SelectListItem selectListItem = new SelectListItem();
                    //                selectListItem.Text = umg_SecondChildDetails.MenuName;
                    //                selectListItem.Value = Convert.ToString(umg_SecondChildDetails.MenuID);
                    //                roleDetailsObj.SecondChildMenuDetailsList.Add(selectListItem);
                    //            }
                    //        }
                    //    }
                    //}

                    UMG_RoleMenuMapping umg_roleMenuMapping = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();

                    roleDetailsObj.MapUnmapButtonForFirstChild = (umg_roleMenuMapping == null) ? "<a href = '#' class='btn btn-success' onclick=MapFirstChildMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>" : "<a href = '#' class='btn btn-warning' onclick=UnmapFirstChildMenu('" + roleDetailsModel.EncryptedID + "');>UnMap</a>";
                }

                return roleDetailsObj;
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

        #region Get Map Unmap Button For Second Child Menu
        public RoleDetailsModel GetMapUnmapButtonForSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                // For checking if First Child menu is maped or not 
                UMG_MenuDetails secondChildMenu = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.SecondChildMenuDetailsId).FirstOrDefault();
                //UMG_MenuDetails firstChildMenu = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();

                UMG_RoleMenuMapping umg_roleMenuMappingForFirstChildMenu = null;
                if (secondChildMenu != null)
                {
                    umg_roleMenuMappingForFirstChildMenu = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == secondChildMenu.ParentID).FirstOrDefault();
                }

                // For Sending button of map/unmap for a particular menu
                if (umg_roleMenuMappingForFirstChildMenu == null)
                {
                    roleDetailsObj.IsFirstChildMenuMapped = false;
                    roleDetailsObj.MapUnmapButtonForFirstChild = "";
                }
                else
                {
                    roleDetailsObj.IsFirstChildMenuMapped = true;
                    UMG_RoleMenuMapping umg_roleMenuMapping = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == roleDetailsModel.SecondChildMenuDetailsId).FirstOrDefault();

                    roleDetailsObj.MapUnmapButtonForSecondChild = (umg_roleMenuMapping == null) ? "<a href = '#' class='btn btn-success' onclick=MapSecondChildMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>" : "<a href = '#' class='btn btn-warning' onclick=UnmapSecondChildMenu('" + roleDetailsModel.EncryptedID + "');>UnMap</a>";
                }
                return roleDetailsObj;
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

        #region  Map Parent Menu to role and return Unmap Button
        public RoleDetailsModel MapParentMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleMenuMapping umg_roleMenuMapping = new UMG_RoleMenuMapping();
                umg_roleMenuMapping.RoleID = Convert.ToInt16(id);
                umg_roleMenuMapping.MenuID = roleDetailsModel.ParentMenuDetailsId;
                umg_roleMenuMapping.IsAdd = true;
                umg_roleMenuMapping.IsEdit = true;
                umg_roleMenuMapping.IsDelete = true;

                localdbContext.UMG_RoleMenuMapping.Add(umg_roleMenuMapping);

                #region 3-4-2019 For Table LOG by SB
                UMG_RoleMenuMapping_Log umg_RoleMenuMapping_Log = new UMG_RoleMenuMapping_Log();
                umg_RoleMenuMapping_Log.LogID = (localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1;
                umg_RoleMenuMapping_Log.RoleID = Convert.ToInt16(id);
                umg_RoleMenuMapping_Log.MenuID = roleDetailsModel.ParentMenuDetailsId;
                umg_RoleMenuMapping_Log.IsAdd = true;
                umg_RoleMenuMapping_Log.IsEdit = true;
                umg_RoleMenuMapping_Log.IsDelete = true;
                umg_RoleMenuMapping_Log.UpdateDateTime = DateTime.Now;
                umg_RoleMenuMapping_Log.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                umg_RoleMenuMapping_Log.UserIPAddress = roleDetailsModel.UserIPAddress;
                umg_RoleMenuMapping_Log.ActionPerformed = "Insert";
                localdbContext.UMG_RoleMenuMapping_Log.Add(umg_RoleMenuMapping_Log);
                #endregion


                localdbContext.SaveChanges();

                // For Activity Log
                UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.ParentMenuDetailsId).FirstOrDefault();
                UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                // For Activity Log
                String messageForActivityLog = "Role Mapped # \"" + umg_roleDetails.RoleName + "\" Role Mapped to \"" + umg_menuDetails.MenuName + "\" menu.";
                if (messageForActivityLog.Length < 1000)
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                else
                {
                    messageForActivityLog = messageForActivityLog.Substring(0, 999);
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                }
                roleDetailsObj.MapUnmapButtonForParent = "<a href = '#' class='btn btn-warning' onclick=UnmapParentMenu('" + roleDetailsModel.EncryptedID + "');>UnMap</a>";
                return roleDetailsObj;
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

        #region FirstChildList_SecondChildList_BeforeParentUnmap
        public RoleDetailsModel FirstChildList_SecondChildList_BeforeParentUnmap(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleDetails umg_RoleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                if (umg_RoleDetails != null)
                    roleDetailsObj.RoleName = umg_RoleDetails.RoleName;

                UMG_MenuDetails umg_MenuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.ParentMenuDetailsId).FirstOrDefault();
                if (umg_MenuDetails != null)
                    roleDetailsObj.ParentMenuName = umg_MenuDetails.MenuName;

                List<UMG_MenuDetails> firstChildList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == roleDetailsModel.ParentMenuDetailsId).OrderBy(c => c.MenuName).ToList();
                List<UMG_MenuDetails> secondChildList = null;

                if (firstChildList != null)
                {
                    roleDetailsObj.FirstChildListString = new List<string>();
                    roleDetailsObj.SecondChildListString = new List<String>();
                    if (firstChildList.Count != 0)
                    {
                        foreach (UMG_MenuDetails umg_menuDetailsForFirstChild in firstChildList)
                        {
                            // For Adding First Child menu to the list
                            UMG_RoleMenuMapping umg_roleMenuMappingForFirstChild = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == umg_menuDetailsForFirstChild.MenuID).FirstOrDefault();
                            // Check if first Child is mapped to role or not. 
                            if (umg_roleMenuMappingForFirstChild != null)
                            {
                                roleDetailsObj.FirstChildListString.Add(umg_menuDetailsForFirstChild.MenuName);
                            }

                            // For Adding Second Child menu to the list
                            secondChildList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == umg_menuDetailsForFirstChild.MenuID).OrderBy(c => c.MenuName).ToList();
                            foreach (UMG_MenuDetails umg_menuDetailsForSecondChild in secondChildList)
                            {
                                UMG_RoleMenuMapping umg_roleMenuMappingForSecondChild = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == umg_menuDetailsForSecondChild.MenuID).FirstOrDefault();
                                // Check if second Child is mapped to role or not. 
                                if (umg_roleMenuMappingForSecondChild != null)
                                {
                                    roleDetailsObj.SecondChildListString.Add(umg_menuDetailsForSecondChild.MenuName);
                                }
                            }
                        }
                    }
                }
                return roleDetailsObj;
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

        #region  Unmap Parent Menu to role and return Map Button
        public RoleDetailsModel UnmapParentMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleMenuMapping umg_roleMenuMapping = new UMG_RoleMenuMapping();
                umg_roleMenuMapping = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == roleDetailsModel.ParentMenuDetailsId).FirstOrDefault();

                if (umg_roleMenuMapping != null)
                {
                    #region 3-4-2019 For Table LOG by SB
                    UMG_RoleMenuMapping_Log umg_RoleMenuMapping_Log = new UMG_RoleMenuMapping_Log();
                    umg_RoleMenuMapping_Log.LogID = (localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1;
                    umg_RoleMenuMapping_Log.RoleID = umg_roleMenuMapping.RoleID;
                    umg_RoleMenuMapping_Log.MenuID = umg_roleMenuMapping.MenuID;
                    umg_RoleMenuMapping_Log.IsAdd = umg_roleMenuMapping.IsAdd;
                    umg_RoleMenuMapping_Log.IsEdit = umg_roleMenuMapping.IsEdit;
                    umg_RoleMenuMapping_Log.IsDelete = umg_roleMenuMapping.IsDelete;
                    umg_RoleMenuMapping_Log.UpdateDateTime = DateTime.Now;
                    umg_RoleMenuMapping_Log.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                    umg_RoleMenuMapping_Log.UserIPAddress = roleDetailsModel.UserIPAddress;
                    umg_RoleMenuMapping_Log.ActionPerformed = "Delete";
                    localdbContext.UMG_RoleMenuMapping_Log.Add(umg_RoleMenuMapping_Log);
                    #endregion

                    localdbContext.UMG_RoleMenuMapping.Remove(umg_roleMenuMapping);
                    localdbContext.SaveChanges();

                    // For Activity Log
                    UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.ParentMenuDetailsId).FirstOrDefault();
                    UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                    // For Activity Log
                    String messageForActivityLog = "Role Unmapped # \"" + umg_roleDetails.RoleName + "\" Role Unmapped to \"" + umg_menuDetails.MenuName + "\" menu.";
                    if (messageForActivityLog.Length < 1000)
                        ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                    else
                    {
                        messageForActivityLog = messageForActivityLog.Substring(0, 999);
                        ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                    }
                }

                roleDetailsObj.MapUnmapButtonForParent = "<a href = '#' class='btn btn-success' onclick=MapParentMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>";
                return roleDetailsObj;
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

        #region  Unmap Parent Menu and sub menu to role and return Map Button
        public RoleDetailsModel UnmapParentMenuAndSubMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleMenuMapping umg_roleMenuMappingForParent = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == roleDetailsModel.ParentMenuDetailsId).FirstOrDefault();

                // For Activity Log
                UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                String messageForActivityLog = "Role Unmapped # \"" + umg_roleDetails.RoleName + "\" Role is Unmapped with following menus : ";

                // Parent menu Unmap Log
                #region 3-4-2019 For Table LOG by SB
                long LogID = 0;
                UMG_RoleMenuMapping_Log mappinglogParent = new UMG_RoleMenuMapping_Log();
                LogID = (localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1;
                mappinglogParent.LogID = LogID;
                mappinglogParent.RoleID = umg_roleMenuMappingForParent.RoleID;
                mappinglogParent.MenuID = umg_roleMenuMappingForParent.MenuID;
                mappinglogParent.IsAdd = umg_roleMenuMappingForParent.IsAdd;
                mappinglogParent.IsEdit = umg_roleMenuMappingForParent.IsEdit;
                mappinglogParent.IsDelete = umg_roleMenuMappingForParent.IsDelete;
                mappinglogParent.UpdateDateTime = DateTime.Now;
                mappinglogParent.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                mappinglogParent.UserIPAddress = roleDetailsModel.UserIPAddress;
                mappinglogParent.ActionPerformed = "Delete";
                localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogParent);
                #endregion

                if (umg_roleMenuMappingForParent != null)
                {
                    // Parent Menu Removed
                    UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.ParentMenuDetailsId).FirstOrDefault();
                    messageForActivityLog = messageForActivityLog + "\"" + umg_menuDetails.MenuName + "\"";

                    localdbContext.UMG_RoleMenuMapping.Remove(umg_roleMenuMappingForParent);
                }
                List<UMG_MenuDetails> firstChildMenuList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == roleDetailsModel.ParentMenuDetailsId).OrderBy(c => c.MenuName).ToList();
                if (firstChildMenuList != null)
                {
                    foreach (var umg_menuDetailsForFirstChild in firstChildMenuList)
                    {
                        if (umg_menuDetailsForFirstChild != null)
                        {
                            List<UMG_MenuDetails> secondChildMenuList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == umg_menuDetailsForFirstChild.MenuID).OrderBy(c => c.MenuName).ToList();
                            if (secondChildMenuList != null)
                            {
                                foreach (var umg_menuDetailsForSecondChild in secondChildMenuList)
                                {
                                    // second Child Menu Removed
                                    UMG_RoleMenuMapping umg_roleMenuMappingForSecondChild = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == umg_menuDetailsForSecondChild.MenuID).FirstOrDefault();

                                    if (umg_roleMenuMappingForSecondChild != null)
                                    {
                                        // Second Child menu Unmap Log
                                        #region 3-4-2019 For Table LOG by SB
                                        UMG_RoleMenuMapping_Log mappinglogSecondChild = new UMG_RoleMenuMapping_Log();
                                        LogID = LogID + 1;
                                        mappinglogSecondChild.LogID = LogID;
                                        mappinglogSecondChild.RoleID = umg_roleMenuMappingForSecondChild.RoleID;
                                        mappinglogSecondChild.MenuID = umg_roleMenuMappingForSecondChild.MenuID;
                                        mappinglogSecondChild.IsAdd = umg_roleMenuMappingForSecondChild.IsAdd;
                                        mappinglogSecondChild.IsEdit = umg_roleMenuMappingForSecondChild.IsEdit;
                                        mappinglogSecondChild.IsDelete = umg_roleMenuMappingForSecondChild.IsDelete;
                                        mappinglogSecondChild.UpdateDateTime = DateTime.Now;
                                        mappinglogSecondChild.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                                        mappinglogSecondChild.UserIPAddress = roleDetailsModel.UserIPAddress;
                                        mappinglogSecondChild.ActionPerformed = "Delete";
                                        localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogSecondChild);
                                        #endregion

                                        localdbContext.UMG_RoleMenuMapping.Remove(umg_roleMenuMappingForSecondChild);
                                        messageForActivityLog = messageForActivityLog + ", \"" + umg_menuDetailsForSecondChild.MenuName + "\"";
                                    }
                                }
                            }
                            // First Child Menu Removed
                            UMG_RoleMenuMapping umg_roleMenuMappingForFirstChild = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == umg_menuDetailsForFirstChild.MenuID).FirstOrDefault();
                            if (umg_roleMenuMappingForFirstChild != null)
                            {
                                // First Child menu Unmap Log
                                #region 3-4-2019 For Table LOG by SB
                                UMG_RoleMenuMapping_Log mappinglogFirstChild = new UMG_RoleMenuMapping_Log();
                                LogID = LogID + 2;
                                mappinglogFirstChild.LogID = LogID;
                                mappinglogFirstChild.RoleID = umg_roleMenuMappingForFirstChild.RoleID;
                                mappinglogFirstChild.MenuID = umg_roleMenuMappingForFirstChild.MenuID;
                                mappinglogFirstChild.IsAdd = umg_roleMenuMappingForFirstChild.IsAdd;
                                mappinglogFirstChild.IsEdit = umg_roleMenuMappingForFirstChild.IsEdit;
                                mappinglogFirstChild.IsDelete = umg_roleMenuMappingForFirstChild.IsDelete;
                                mappinglogFirstChild.UpdateDateTime = DateTime.Now;
                                mappinglogFirstChild.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                                mappinglogFirstChild.UserIPAddress = roleDetailsModel.UserIPAddress;
                                mappinglogFirstChild.ActionPerformed = "Delete";
                                localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogFirstChild);
                                #endregion


                                localdbContext.UMG_RoleMenuMapping.Remove(umg_roleMenuMappingForFirstChild);
                                messageForActivityLog = messageForActivityLog + ", \"" + umg_menuDetailsForFirstChild.MenuName + "\"";
                            }
                        }
                    }
                }
                localdbContext.SaveChanges();

                // For Activity Log
                if (messageForActivityLog.Length < 1000)
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                else
                {
                    messageForActivityLog = messageForActivityLog.Substring(0, 999);
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                }
                roleDetailsObj.MapUnmapButtonForParent = "<a href = '#' class='btn btn-success' onclick=MapParentMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>";
                return roleDetailsObj;
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

        #region  Map First Child Menu to role and return Unmap Button
        public RoleDetailsModel MapFirstChildMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleMenuMapping umg_roleMenuMapping = new UMG_RoleMenuMapping();
                umg_roleMenuMapping.RoleID = Convert.ToInt16(id);
                umg_roleMenuMapping.MenuID = roleDetailsModel.FirstChildMenuDetailsId;
                umg_roleMenuMapping.IsAdd = true;
                umg_roleMenuMapping.IsEdit = true;
                umg_roleMenuMapping.IsDelete = true;

                localdbContext.UMG_RoleMenuMapping.Add(umg_roleMenuMapping);

                // First Child menu map Log
                #region 3-4-2019 For Table LOG by SB
                UMG_RoleMenuMapping_Log mappinglogFirstChild = new UMG_RoleMenuMapping_Log();
                mappinglogFirstChild.LogID = (localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1;
                mappinglogFirstChild.RoleID = Convert.ToInt16(id);
                mappinglogFirstChild.MenuID = roleDetailsModel.FirstChildMenuDetailsId;
                mappinglogFirstChild.IsAdd = true;
                mappinglogFirstChild.IsEdit = true;
                mappinglogFirstChild.IsDelete = true;
                mappinglogFirstChild.UpdateDateTime = DateTime.Now;
                mappinglogFirstChild.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                mappinglogFirstChild.UserIPAddress = roleDetailsModel.UserIPAddress;
                mappinglogFirstChild.ActionPerformed = "Insert";
                localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogFirstChild);
                #endregion

                localdbContext.SaveChanges();

                // For Activity Log
                UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();
                UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                // For Activity Log
                String messageForActivityLog = "Role Mapped # \"" + umg_roleDetails.RoleName + "\" Role Mapped to \"" + umg_menuDetails.MenuName + "\" menu.";
                if (messageForActivityLog.Length < 1000)
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                else
                {
                    messageForActivityLog = messageForActivityLog.Substring(0, 999);
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                }
                roleDetailsObj.MapUnmapButtonForFirstChild = "<a href = '#' class='btn btn-warning' onclick=UnmapFirstChildMenu('" + roleDetailsModel.EncryptedID + "');>UnMap</a>";
                return roleDetailsObj;
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

        #region SecondChildList_BeforeFirstChildUnmap
        public RoleDetailsModel SecondChildList_BeforeFirstChildUnmap(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleDetails umg_RoleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                if (umg_RoleDetails != null)
                    roleDetailsObj.RoleName = umg_RoleDetails.RoleName;

                UMG_MenuDetails umg_MenuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();
                if (umg_MenuDetails != null)
                    roleDetailsObj.FirstChildMenuName = umg_MenuDetails.MenuName;

                List<UMG_MenuDetails> secondChildList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == roleDetailsModel.FirstChildMenuDetailsId).OrderBy(c => c.MenuName).ToList();

                if (secondChildList != null)
                {
                    roleDetailsObj.SecondChildListString = new List<String>();
                    if (secondChildList.Count != 0)
                    {
                        foreach (UMG_MenuDetails umg_menuDetailsForSecondChild in secondChildList)
                        {
                            // For Adding Second Child menu to the list
                            UMG_RoleMenuMapping umg_roleMenuMappingForSecondChild = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == umg_menuDetailsForSecondChild.MenuID).FirstOrDefault();
                            // Check if second Child is mapped to role or not. 
                            if (umg_roleMenuMappingForSecondChild != null)
                            {
                                roleDetailsObj.SecondChildListString.Add(umg_menuDetailsForSecondChild.MenuName);
                            }
                        }
                    }
                }
                return roleDetailsObj;
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

        #region  Unmap First Child Menu to role and return Map Button
        public RoleDetailsModel UnmapFirstChildMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleMenuMapping umg_roleMenuMapping = new UMG_RoleMenuMapping();
                umg_roleMenuMapping = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();

                if (umg_roleMenuMapping != null)
                {
                    // First Child menu unmap Log
                    #region 3-4-2019 For Table LOG by SB
                    UMG_RoleMenuMapping_Log mappinglogFirstChild = new UMG_RoleMenuMapping_Log();
                    mappinglogFirstChild.LogID = (localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1;
                    mappinglogFirstChild.RoleID = umg_roleMenuMapping.RoleID;
                    mappinglogFirstChild.MenuID = umg_roleMenuMapping.MenuID;
                    mappinglogFirstChild.IsAdd = umg_roleMenuMapping.IsAdd;
                    mappinglogFirstChild.IsEdit = umg_roleMenuMapping.IsEdit;
                    mappinglogFirstChild.IsDelete = umg_roleMenuMapping.IsDelete;
                    mappinglogFirstChild.UpdateDateTime = DateTime.Now;
                    mappinglogFirstChild.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                    mappinglogFirstChild.UserIPAddress = roleDetailsModel.UserIPAddress;
                    mappinglogFirstChild.ActionPerformed = "Delete";
                    localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogFirstChild);
                    #endregion


                    localdbContext.UMG_RoleMenuMapping.Remove(umg_roleMenuMapping);
                    localdbContext.SaveChanges();

                    // For Activity Log
                    UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();
                    UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                    // For Activity Log
                    String messageForActivityLog = "Role Unmapped # \"" + umg_roleDetails.RoleName + "\" Role Unmapped to \"" + umg_menuDetails.MenuName + "\" menu.";
                    if (messageForActivityLog.Length < 1000)
                        ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                    else
                    {
                        messageForActivityLog = messageForActivityLog.Substring(0, 999);
                        ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                    }
                }
                roleDetailsObj.MapUnmapButtonForFirstChild = "<a href = '#' class='btn btn-success' onclick=MapFirstChildMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>";
                return roleDetailsObj;
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

        #region Unmap First Child Menu and sub menu to role and return Map Button
        public RoleDetailsModel UnmapFirstChildMenuAndSubMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleMenuMapping umg_roleMenuMappingForFirstChild = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();

                // For Activity Log
                UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                String messageForActivityLog = "Role Unmapped # \"" + umg_roleDetails.RoleName + "\" Role is Unmapped with following menus : ";


                long LogID = 0;
                if (umg_roleMenuMappingForFirstChild != null)
                {
                    // First Child menu unmap Log
                    #region 3-4-2019 For Table LOG by SB
                    UMG_RoleMenuMapping_Log mappinglogFirstChild = new UMG_RoleMenuMapping_Log();
                    LogID = (localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1;
                    mappinglogFirstChild.LogID = LogID;
                    mappinglogFirstChild.RoleID = umg_roleMenuMappingForFirstChild.RoleID;
                    mappinglogFirstChild.MenuID = umg_roleMenuMappingForFirstChild.MenuID;
                    mappinglogFirstChild.IsAdd = umg_roleMenuMappingForFirstChild.IsAdd;
                    mappinglogFirstChild.IsEdit = umg_roleMenuMappingForFirstChild.IsEdit;
                    mappinglogFirstChild.IsDelete = umg_roleMenuMappingForFirstChild.IsDelete;
                    mappinglogFirstChild.UpdateDateTime = DateTime.Now;
                    mappinglogFirstChild.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                    mappinglogFirstChild.UserIPAddress = roleDetailsModel.UserIPAddress;
                    mappinglogFirstChild.ActionPerformed = "Delete";
                    localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogFirstChild);
                    #endregion

                    // First Child Menu Removed
                    localdbContext.UMG_RoleMenuMapping.Remove(umg_roleMenuMappingForFirstChild);
                    // For Activity Log
                    UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.FirstChildMenuDetailsId).FirstOrDefault();
                    messageForActivityLog = messageForActivityLog + "\"" + umg_menuDetails.MenuName + "\"";
                }

                List<UMG_MenuDetails> secondChildMenuList = localdbContext.UMG_MenuDetails.Where(x => x.ParentID == roleDetailsModel.FirstChildMenuDetailsId).OrderBy(c => c.MenuName).ToList();
                if (secondChildMenuList != null)
                {
                    foreach (var umg_menuDetailsForSecondChild in secondChildMenuList)
                    {
                        // second Child Menu Removed
                        UMG_RoleMenuMapping umg_roleMenuMappingForSecondChild = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == umg_menuDetailsForSecondChild.MenuID).FirstOrDefault();
                        if (umg_roleMenuMappingForSecondChild != null)
                        {
                            // Second Child menu unmap Log
                            #region 3-4-2019 For Table LOG by SB
                            UMG_RoleMenuMapping_Log mappinglogSecondChild = new UMG_RoleMenuMapping_Log();
                            // mappinglogSecondChild.LogID = Convert.ToInt16((localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1);
                            LogID = LogID + 1;
                            mappinglogSecondChild.LogID = LogID;
                            mappinglogSecondChild.RoleID = umg_roleMenuMappingForSecondChild.RoleID;
                            mappinglogSecondChild.MenuID = umg_roleMenuMappingForSecondChild.MenuID;
                            mappinglogSecondChild.IsAdd = umg_roleMenuMappingForSecondChild.IsAdd;
                            mappinglogSecondChild.IsEdit = umg_roleMenuMappingForSecondChild.IsEdit;
                            mappinglogSecondChild.IsDelete = umg_roleMenuMappingForSecondChild.IsDelete;
                            mappinglogSecondChild.UpdateDateTime = DateTime.Now;
                            mappinglogSecondChild.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                            mappinglogSecondChild.UserIPAddress = roleDetailsModel.UserIPAddress;
                            mappinglogSecondChild.ActionPerformed = "Delete";
                            localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogSecondChild);
                            #endregion

                            localdbContext.UMG_RoleMenuMapping.Remove(umg_roleMenuMappingForSecondChild);
                            // For Activity Log
                            messageForActivityLog = messageForActivityLog + ", \"" + umg_menuDetailsForSecondChild.MenuName + "\"";
                        }
                    }
                }
                localdbContext.SaveChanges();

                // For Activity Log
                if (messageForActivityLog.Length < 1000)
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                else
                {
                    messageForActivityLog = messageForActivityLog.Substring(0, 999);
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                }
                roleDetailsObj.MapUnmapButtonForFirstChild = "<a href = '#' class='btn btn-success' onclick=MapFirstChildMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>";
                return roleDetailsObj;
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

        #region  Map Second Child Menu to role and return Unmap Button
        public RoleDetailsModel MapSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());


                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleMenuMapping umg_roleMenuMapping = new UMG_RoleMenuMapping();
                umg_roleMenuMapping.RoleID = Convert.ToInt16(id);
                umg_roleMenuMapping.MenuID = roleDetailsModel.SecondChildMenuDetailsId;
                umg_roleMenuMapping.IsAdd = true;
                umg_roleMenuMapping.IsEdit = true;
                umg_roleMenuMapping.IsDelete = true;

                localdbContext.UMG_RoleMenuMapping.Add(umg_roleMenuMapping);

                // Second Child menu map Log
                #region 3-4-2019 For Table LOG by SB
                UMG_RoleMenuMapping_Log mappinglogSecondChild = new UMG_RoleMenuMapping_Log();
                mappinglogSecondChild.LogID = (localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1;
                mappinglogSecondChild.RoleID = Convert.ToInt16(id);
                mappinglogSecondChild.MenuID = roleDetailsModel.SecondChildMenuDetailsId;
                mappinglogSecondChild.IsAdd = true;
                mappinglogSecondChild.IsEdit = true;
                mappinglogSecondChild.IsDelete = true;
                mappinglogSecondChild.UpdateDateTime = DateTime.Now;
                mappinglogSecondChild.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                mappinglogSecondChild.UserIPAddress = roleDetailsModel.UserIPAddress;
                mappinglogSecondChild.ActionPerformed = "Insert";
                localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogSecondChild);
                #endregion

                localdbContext.SaveChanges();

                // For Activity Log
                UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.SecondChildMenuDetailsId).FirstOrDefault();
                UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                // For Activity Log
                String messageForActivityLog = "Role Mapped # \"" + umg_roleDetails.RoleName + "\" Role Mapped to \"" + umg_menuDetails.MenuName + "\" menu.";
                if (messageForActivityLog.Length < 1000)
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                else
                {
                    messageForActivityLog = messageForActivityLog.Substring(0, 999);
                    ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                }
                roleDetailsObj.MapUnmapButtonForSecondChild = "<a href = '#' class='btn btn-warning' onclick=UnmapSecondChildMenu('" + roleDetailsModel.EncryptedID + "');>UnMap</a>";
                return roleDetailsObj;
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

        #region  Unmap Second Child Menu to role and return Map Button
        public RoleDetailsModel UnmapSecondChildMenu(RoleDetailsModel roleDetailsModel)
        {
            KaveriEntities localdbContext = null;
            try
            {
                encryptedParameters = roleDetailsModel.EncryptedID.Split('/');
                if (!(encryptedParameters.Length == 3))
                    throw new SecurityException("URL Tempered");

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });
                int id = Convert.ToInt16(decryptedParameters["RoleID"].ToString().Trim());

                localdbContext = new KaveriEntities();
                RoleDetailsModel roleDetailsObj = new RoleDetailsModel();
                roleDetailsObj.EncryptedID = roleDetailsModel.EncryptedID;

                UMG_RoleMenuMapping umg_roleMenuMapping = new UMG_RoleMenuMapping();
                umg_roleMenuMapping = localdbContext.UMG_RoleMenuMapping.Where(x => x.RoleID == id && x.MenuID == roleDetailsModel.SecondChildMenuDetailsId).FirstOrDefault();

                if (umg_roleMenuMapping != null)
                {
                    // Second Child menu unmap Log
                    #region 3-4-2019 For Table LOG by SB
                    UMG_RoleMenuMapping_Log mappinglogSecondChild = new UMG_RoleMenuMapping_Log();
                    mappinglogSecondChild.LogID = (localdbContext.UMG_RoleMenuMapping_Log.Any() ? localdbContext.UMG_RoleMenuMapping_Log.Max(a => a.LogID) : 0) + 1;
                    mappinglogSecondChild.RoleID = umg_roleMenuMapping.RoleID;
                    mappinglogSecondChild.MenuID = umg_roleMenuMapping.MenuID;
                    mappinglogSecondChild.IsAdd = umg_roleMenuMapping.IsAdd;
                    mappinglogSecondChild.IsEdit = umg_roleMenuMapping.IsEdit;
                    mappinglogSecondChild.IsDelete = umg_roleMenuMapping.IsDelete;
                    mappinglogSecondChild.UpdateDateTime = DateTime.Now;
                    mappinglogSecondChild.UserID = roleDetailsModel.UserIdForActivityLogFromSession;
                    mappinglogSecondChild.UserIPAddress = roleDetailsModel.UserIPAddress;
                    mappinglogSecondChild.ActionPerformed = "Delete";
                    localdbContext.UMG_RoleMenuMapping_Log.Add(mappinglogSecondChild);
                    #endregion

                    localdbContext.UMG_RoleMenuMapping.Remove(umg_roleMenuMapping);
                    localdbContext.SaveChanges();

                    // For Activity Log
                    UMG_MenuDetails umg_menuDetails = localdbContext.UMG_MenuDetails.Where(x => x.MenuID == roleDetailsModel.SecondChildMenuDetailsId).FirstOrDefault();
                    UMG_RoleDetails umg_roleDetails = localdbContext.UMG_RoleDetails.Where(x => x.RoleID == id).FirstOrDefault();
                    // For Activity Log
                    String messageForActivityLog = "Role Unmapped # \"" + umg_roleDetails.RoleName + "\" Role Unmapped to \"" + umg_menuDetails.MenuName + "\" menu.";
                    if (messageForActivityLog.Length < 1000)
                        ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                    else
                    {
                        messageForActivityLog = messageForActivityLog.Substring(0, 999);
                        ApiCommonFunctions.SystemUserActivityLog(roleDetailsModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.RoleMenuDetails), messageForActivityLog);
                    }
                }

                roleDetailsObj.MapUnmapButtonForSecondChild = "<a href = '#' class='btn btn-success' onclick=MapSecondChildMenu('" + roleDetailsModel.EncryptedID + "');>Map</a>";
                return roleDetailsObj;
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

        #region 09-04-2019 For Level drop down by shubham bhagat
        public List<SelectListItem> GetLevelList()
        {
            try
            {
                dbContext = new KaveriEntities();
                List<SelectListItem> levelList = new List<SelectListItem>();
                levelList.Add(new SelectListItem { Text = "Select", Value = "0" });

                var dbLevelList = dbContext.UMG_LevelDetails.Where(x => x.IsActive == true).ToList();
                if (dbLevelList != null)
                {
                    foreach (var item in dbLevelList)
                    {
                        SelectListItem selectListItem = new SelectListItem();
                        selectListItem.Text = item.LevelName;
                        selectListItem.Value = Convert.ToString(item.LevelID);
                        levelList.Add(selectListItem);
                    }
                }
                return levelList;
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
    }
}
