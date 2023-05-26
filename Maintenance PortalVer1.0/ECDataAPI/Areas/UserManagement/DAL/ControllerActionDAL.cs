using CustomModels.Models.ControllerAction;
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
using System.Web.Routing;

namespace ECDataAPI.Areas.UserManagement.DAL
{
    public class ControllerActionDAL : IControllerActionDetails, IDisposable
    {

        KaveriEntities dbContext = new KaveriEntities();

        private String[] encryptedParameters = null;
        private Dictionary<String, String> decryptedParameters = null;







        public ControllerActionViewModel ShowControllerActionData(ControllerActionViewModel viewModel)
        {
            ControllerActionViewModel responseModel = new ControllerActionViewModel();
            responseModel.filterMenuDetailsList = PopulateMenusWithoutParentMenu(viewModel.CurrentRoleID, false);
            return responseModel;
        }




        public List<SelectListItem> PopulateMenusWithoutParentMenu(short CurrentRoleId, bool isAllOption = true)
        {
            List<SelectListItem> listItems = new List<SelectListItem>();

            if (CurrentRoleId == (short)ApiCommonEnum.RoleDetails.TechnicalAdmin)
            {
                if (!isAllOption)
                    listItems.Insert(0, new SelectListItem() { Text = "All", Value = "0", Selected = true });
            }
            else
            {
                if (!isAllOption)
                    listItems.Insert(0, new SelectListItem() { Text = "Select", Value = "0", Selected = true });
            }

            listItems.AddRange((from md in dbContext.UMG_MenuDetails
                                join pd in dbContext.UMG_MenuDetails
                                on
                                md.ParentID equals pd.MenuID
                                where
                                     md.ParentID > 0 && md.IsActive == true
                                orderby md.MenuName
                                select new SelectListItem()
                                {
                                    Text = md.MenuName + " - [" + pd.MenuName + "]",
                                    Value = md.MenuID.ToString()
                                }).ToList());


            return listItems;
        }

        /// <summary>
        /// Populates CAIDList
        /// </summary>
        /// <returns></returns>
        public List<ControllerActionModel> PopulateCAIDList(ControllerActionViewModel viewModel)
        {
            List<ControllerActionModel> CAIDList = new List<ControllerActionModel>();
            #region  cOMMENTED BY M RAFE ON 28-11-19
            //foreach (var t in dbContext.UMG_ControllerActionDetails.ToList())
            //{
            //    ControllerActionModel tempModel = new ControllerActionModel();
            //    // tempModel.CAID = t.CAID;
            //    tempModel.EncryptedID = URLEncrypt.EncryptParameters(new String[] { "CAID=" + t.CAID });
            //    tempModel.AreaNameId = t.AreaName;
            //    tempModel.ControllerNameId = t.ControllerName;
            //    tempModel.ActionNameId = t.ActionName;
            //    tempModel.IsActive = t.IsActive;
            //    CAIDList.Add(tempModel);
            //} 
            #endregion




            var result = dbContext.USP_PopulateControllerActionDetails(viewModel.filterMenuDetailsId).ToList();
            int i = 0;
            foreach (var t in result)
            {
                ControllerActionModel tempModel = new ControllerActionModel();
                // tempModel.CAID = t.CAID;
                tempModel.SrNo = (++i);
                tempModel.EncryptedID = URLEncrypt.EncryptParameters(new String[] { "CAID=" + t.CAID });

                tempModel.AreaNameId = t.AreaName;
                tempModel.ControllerNameId = t.ControllerName;
                tempModel.ActionNameId = t.ActionName;
                tempModel.Description = t.Description;
                tempModel.IsActive = t.IsActive;
                //if (viewModel.CurrentRoleID == (short)ApiCommonEnum.RoleDetails.DepartmentAdmin)
                //{
                //    tempModel.ForMenu = "<a href='#'  onclick=GetRoleAuthView('" + tempModel.EncryptedID + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                //}
                //Added by mayank on 15-07-2021 for deptadmin funct to aigrcomp start
                if(viewModel.CurrentRoleID == (short)ApiCommonEnum.RoleDetails.DepartmentAdmin || viewModel.CurrentRoleID == (short)ApiCommonEnum.RoleDetails.AIGRComp)
                {
                    tempModel.ForMenu = "<a href='#'  onclick=GetRoleAuthView('" + tempModel.EncryptedID + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                }
                //end
                else
                {
                    if (string.IsNullOrEmpty(t.ForMenu))
                        tempModel.ForMenu = "";
                    else
                    {
                        string sTempString = t.ForMenu.Trim();
                        tempModel.ForMenu = sTempString.Last() == ',' ? sTempString.Substring(0, sTempString.Length - 1) : sTempString;
                    }

                }


                if (string.IsNullOrEmpty(t.AssignedToRoles))
                    tempModel.AssignedToRoles = "";
                else
                {
                    string sTempString = t.AssignedToRoles.Trim();
                    tempModel.AssignedToRoles = sTempString.Last() == ',' ? sTempString.Substring(0, sTempString.Length - 1) : sTempString;
                }


                tempModel.IsActiveStr = tempModel.IsActive ? "<i class='fa fa-check  ' style='color:black'></i>" : "<i class='fa fa-close  ' style='color:black'></i>";
                tempModel.Edit = "<a href='#'  onclick=UpdateControllerActionData('" + tempModel.EncryptedID + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                tempModel.Delete = "<a href='#'  onclick=DeleteControllerActionData('" + tempModel.EncryptedID + "'); ><i class='fa fa-trash fa-2x ' style='color:black'></i></a>";
                CAIDList.Add(tempModel);
            }

            return CAIDList;
        }

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Insert NewControllerAction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ControllerActionModel InsertNewControllerAction(ControllerActionModel model)
        {

            try
            {

                using (TransactionScope ts = new TransactionScope())
                {

                    int cid = 0;

                    int rid = 0;
                    UMG_ControllerActionDetails dbObj = new UMG_ControllerActionDetails();
                    UMG_RoleActionAuth dbRoleAuth = null;
                    if (dbContext.UMG_ControllerActionDetails.Any(x => x.CAID == 1))
                    {
                        cid = dbContext.UMG_ControllerActionDetails.Max(x => x.CAID) + 1;
                    }
                    else
                    {
                        cid = 1;
                    }
                    dbObj.CAID = cid;
                    if (model.AreaNameId == "NoArea")
                        dbObj.AreaName = "";
                    else
                        dbObj.AreaName = model.AreaNameId;
                    dbObj.ControllerName = model.ControllerNameId;
                    dbObj.ActionName = model.ActionNameId;
                    dbObj.IsActive = model.IsActive;
                    dbObj.IsForMenuActionMapping = model.IsForMenuActionMapping;

                    dbObj.Description = model.Description;
                    dbContext.UMG_ControllerActionDetails.Add(dbObj);
                    //  BELOW LINES ARE COMMENTED BY M RAFE ON 04-12-19
                    // ROLE ACTION AUTHORIZATION IS NOW GIVEN TO DEPTADMIN AND PERFORMED BY UpdateRoleActionAuth  METHOD
                    //

                    /*
                    if (dbContext.UMG_RoleActionAuth.Any(x => x.ID == 1))
                    {
                        rid = dbContext.UMG_RoleActionAuth.Max(x => x.ID) + 1;
                    }
                    else
                    {
                        rid = 1;
                    }

                   
                    String roleDetailListForActivityLog = "And assigned to the Following role : ";
                    foreach (var item in model.RoleId)
                    {
                        dbRoleAuth = new UMG_RoleActionAuth();
                        dbRoleAuth.CAID = cid;
                        dbRoleAuth.RoleID = (short)item;
                        dbRoleAuth.ID = rid;
                        dbContext.UMG_RoleActionAuth.Add(dbRoleAuth);
                        UMG_RoleDetails umg_roleDetails = dbContext.UMG_RoleDetails.Where(x => x.RoleID == item).FirstOrDefault();
                        roleDetailListForActivityLog = roleDetailListForActivityLog + umg_roleDetails.RoleName + ", ";
                        rid++;
                    }

    */



                    // For Activity Log
                    //String messageForAuditLog = "Action Detail Added # " + model.AreaNameId + "/" + model.ControllerNameId + "/" + model.ActionNameId + "- Action Detail Added." + roleDetailListForActivityLog;
                    // ABOVE LINE  COMMENTED BY M RAFE ON 04-12-19


                    String messageForAuditLog = "Action Detail Added # " + model.AreaNameId + "/" + model.ControllerNameId + "/" + model.ActionNameId + "- Action Detail Added.";
                    if (messageForAuditLog.Length < 1000)
                        ApiCommonFunctions.SystemUserActivityLog(model.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.ControllerActionDetails), messageForAuditLog);

                    // added by m rafe on 28-11-19


                    foreach (var item in model.MenuDetailsId)
                    {
                        UMG_MenuActionAuthorizationMapping authorizationMapping = new UMG_MenuActionAuthorizationMapping();
                        authorizationMapping.CAID = dbObj.CAID;
                        authorizationMapping.MenuId = item;
                        dbContext.UMG_MenuActionAuthorizationMapping.Add(authorizationMapping);
                    }

                    dbContext.SaveChanges();
                    ts.Complete();
                    return model;
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

        /// <summary>
        /// Gets ControllerActionModel
        /// </summary>
        /// <param name="DataModel"></param>
        /// <returns></returns>
        public ControllerActionModel GetControllerActionModel(ControllerActionDataModel DataModel)
        {

            try
            {
                dbContext = new KaveriEntities();
                UMG_ControllerActionDetails dbCAObj = new UMG_ControllerActionDetails();
                ControllerActionModel responseModel = new ControllerActionModel();
                if (DataModel.EncryptedId != string.Empty)
                {

                    encryptedParameters = DataModel.EncryptedId.Split('/');

                    if (encryptedParameters.Length != 3)
                    {
                        throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                    }

                    decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                    int caid = Convert.ToInt32(decryptedParameters["CAID"].ToString().Trim());

                    dbCAObj = dbContext.UMG_ControllerActionDetails.Where(x => x.CAID == caid).FirstOrDefault();

                    if (dbCAObj != null)
                    {
                        responseModel.CAID = dbCAObj.CAID;
                        if (dbCAObj.AreaName == "")
                            responseModel.AreaNameId = "NoArea";
                        else
                            responseModel.AreaNameId = dbCAObj.AreaName;
                        responseModel.ControllerNameId = dbCAObj.ControllerName;
                        responseModel.ActionNameId = dbCAObj.ActionName;
                        responseModel.IsActive = dbCAObj.IsActive;
                        responseModel.IsForMenuActionMapping = dbCAObj.IsForMenuActionMapping ?? false;


                        List<UMG_RoleActionAuth> dbRoleActionAuth = new List<UMG_RoleActionAuth>();
                        dbRoleActionAuth = dbContext.UMG_RoleActionAuth.Where(x => x.CAID == caid).ToList();
                        int[] arrayRoleId = new int[dbRoleActionAuth.Count];
                        int i = 0;
                        foreach (var role in dbRoleActionAuth)
                        {
                            arrayRoleId[i] = role.RoleID;
                            i++;
                        }
                        responseModel.RoleList = GetRoleData(arrayRoleId);
                        responseModel.ControllerList = GetControllerList(DataModel.AreaList, dbCAObj.AreaName);
                        responseModel.ActionList = GetActionList(DataModel.AreaList, dbCAObj.AreaName, dbCAObj.ControllerName);
                        responseModel.AreaList = GetAreaList(DataModel.AreaList);

                        responseModel.Description = dbCAObj.Description;
                        responseModel.MenuDetailsId = dbContext.UMG_MenuActionAuthorizationMapping.Where(x => x.CAID == dbCAObj.CAID).Select(c => c.MenuId).ToArray();
                    }

                }
                else
                {
                    responseModel.RoleList = GetRoleData(null);
                    responseModel.ControllerList = GetControllerList(DataModel.AreaList, "NoArea");
                    responseModel.ActionList = GetActionList(DataModel.AreaList, "NoArea", "Home");
                    responseModel.AreaList = GetAreaList(DataModel.AreaList);

                }
                //added by rafe on  28-11-19
                responseModel.MenuDetailsList = PopulateMenusWithoutParentMenu(0, true);

                // responseModel.role = dbContext.UMG_RoleDetails.Where(x => x.RoleID == (dbContext.UMG_RoleActionAuth.Where(xc => xc.CAID == caid).FirstOrDefault().RoleID)).FirstOrDefault().RoleName;
                return responseModel;
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
        /// updates ControllerActionModel
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns></returns>
        public ControllerActionModel updateControllerActionModel(ControllerActionModel updateModel)
        {

            try
            {
                ControllerActionModel responseModel = new ControllerActionModel();
                UMG_RoleActionAuth dbRoleAuth = new UMG_RoleActionAuth();
                UMG_ControllerActionDetails dbCAObj = new UMG_ControllerActionDetails();

                encryptedParameters = updateModel.EncryptedID.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                updateModel.CAID = Convert.ToInt32(decryptedParameters["CAID"].ToString().Trim());
                dbCAObj = dbContext.UMG_ControllerActionDetails.Where(x => x.CAID == updateModel.CAID).FirstOrDefault();

                #region 5-4-2019 For Table LOG by SB
                List<UMG_RoleActionAuth_Log> UMG_RoleActionAuth_LogList = new List<UMG_RoleActionAuth_Log>();

                UMG_ControllerActionDetails_Log umg_ControllerActionDetails_Log = new UMG_ControllerActionDetails_Log();
                umg_ControllerActionDetails_Log.LogID = (dbContext.UMG_ControllerActionDetails_Log.Any() ? dbContext.UMG_ControllerActionDetails_Log.Max(a => a.LogID) : 0) + 1;
                umg_ControllerActionDetails_Log.CAID = dbCAObj.CAID;

                if (updateModel.AreaNameId == "NoArea" || updateModel.AreaNameId == "CommonArea")
                {
                    dbCAObj.AreaName = "";
                    umg_ControllerActionDetails_Log.AreaName = "";
                }
                else
                {
                    dbCAObj.AreaName = updateModel.AreaNameId;
                    umg_ControllerActionDetails_Log.AreaName = updateModel.AreaNameId;
                }
                //dbCAObj.AreaName = updateModel.AreaNameId;
                dbCAObj.ControllerName = updateModel.ControllerNameId;
                dbCAObj.ActionName = updateModel.ActionNameId;
                dbCAObj.IsActive = updateModel.IsActive;
                dbCAObj.IsForMenuActionMapping = updateModel.IsForMenuActionMapping;

                dbCAObj.Description = updateModel.Description;

                umg_ControllerActionDetails_Log.ControllerName = updateModel.ControllerNameId;
                umg_ControllerActionDetails_Log.ActionName = updateModel.ActionNameId;
                umg_ControllerActionDetails_Log.IsActive = updateModel.IsActive;
                umg_ControllerActionDetails_Log.UpdateDateTime = DateTime.Now;
                umg_ControllerActionDetails_Log.UserID = updateModel.UserIdForActivityLogFromSession;
                umg_ControllerActionDetails_Log.UserIPAddress = updateModel.UserIPAddress;
                umg_ControllerActionDetails_Log.ActionPerformed = "Update";
                dbContext.UMG_ControllerActionDetails_Log.Add(umg_ControllerActionDetails_Log);
                #endregion


                //  BELOW AREA IS COMMENTED BY M RAFE ON 04-12-19
                // ROLE ACTION AUTHORIZATION IS NOW GIVEN TO DEPTADMIN AND PERFORMED BY UpdateRoleActionAuth  METHOD
                //

                #region cOMMENTED CODE 
                /*List<UMG_RoleActionAuth> dbRoleActionAuth = new List<UMG_RoleActionAuth>();
                dbRoleActionAuth = dbContext.UMG_RoleActionAuth.Where(x => x.CAID == updateModel.CAID).ToList();
                int[] arrayRoleId = new int[dbRoleActionAuth.Count];
                int i = 0;
                foreach (var role in dbRoleActionAuth)
                {
                    arrayRoleId[i] = role.RoleID;
                    i++;
                }
                //dbContext.UMG_RoleActionAuth.RemoveRange(dbContext.UMG_RoleActionAuth.Where(r => !arrayRoleId.All(updateModel.RoleId) && r.CAID==updateModel.CAID));
                //dbContext..RemoveRange(.UMG_RoleActionAuth.Where(x=>x.CAID==updateModel.CAID ));
                if (arrayRoleId.Length == updateModel.RoleId.Length)
                {
                    //check contents are same or not
                    var status = arrayRoleId.All(updateModel.RoleId.Contains);
                    if (status)
                    {
                        dbContext.SaveChanges();
                    }
                    else
                    {
                        bool matchStatus = false;

                        //insertion
                        foreach (var outitem in updateModel.RoleId)
                        {
                            foreach (var inneritem in arrayRoleId)
                            {
                                if (inneritem == outitem)
                                {
                                    matchStatus = true;
                                }

                            }
                            if (!matchStatus)
                            {
                                dbRoleAuth = new UMG_RoleActionAuth();
                                dbRoleAuth.CAID = updateModel.CAID;
                                dbRoleAuth.RoleID = (short)outitem;
                                dbRoleAuth.ID = dbContext.UMG_RoleActionAuth.Max(x => x.ID) + 1;
                                dbContext.UMG_RoleActionAuth.Add(dbRoleAuth);

                                // For Log By Shubham Bhagat on 5-4-2019
                                UMG_RoleActionAuth_Log umgRoleActionAuthLog = new UMG_RoleActionAuth_Log();
                                umgRoleActionAuthLog.CAID = updateModel.CAID;
                                umgRoleActionAuthLog.RoleID = (short)outitem;
                                umgRoleActionAuthLog.ID = dbRoleAuth.ID;
                                umgRoleActionAuthLog.ActionPerformed = "Insert";
                                UMG_RoleActionAuth_LogList.Add(umgRoleActionAuthLog);
                            }
                            matchStatus = false;
                        }
                        //for deletion
                        foreach (var outitem in arrayRoleId)
                        {
                            foreach (var inneritem in updateModel.RoleId)
                            {
                                if (inneritem == outitem)
                                {
                                    matchStatus = true;
                                }

                            }
                            if (!matchStatus)
                            {
                                UMG_RoleActionAuth objroleauth = new UMG_RoleActionAuth();
                                objroleauth = dbContext.UMG_RoleActionAuth.Where(x => x.CAID == updateModel.CAID && x.RoleID == outitem).FirstOrDefault();
                                if (objroleauth != null)
                                {
                                    // For Log By Shubham Bhagat on 5-4-2019
                                    UMG_RoleActionAuth_Log umgRoleActionAuthLog = new UMG_RoleActionAuth_Log();
                                    umgRoleActionAuthLog.CAID = objroleauth.CAID;
                                    umgRoleActionAuthLog.RoleID = objroleauth.RoleID;
                                    umgRoleActionAuthLog.ID = objroleauth.ID;
                                    umgRoleActionAuthLog.ActionPerformed = "Delete";
                                    UMG_RoleActionAuth_LogList.Add(umgRoleActionAuthLog);
                                }
                                dbContext.UMG_RoleActionAuth.Remove(objroleauth);

                            }
                            matchStatus = false;
                        }
                    }
                }
                else
                {
                    bool matchStatus = false;
                    // Added by shubham bhagat on 5-4-2019
                    int maxIDFromRoleActionAuth = dbContext.UMG_RoleActionAuth.Max(x => x.ID) + 1;

                    //insertion
                    foreach (var outitem in updateModel.RoleId)
                    {
                        foreach (var inneritem in arrayRoleId)
                        {
                            if (inneritem == outitem)
                            {
                                matchStatus = true;
                            }

                        }
                        if (!matchStatus)
                        {
                            dbRoleAuth = new UMG_RoleActionAuth();
                            dbRoleAuth.CAID = updateModel.CAID;
                            dbRoleAuth.RoleID = (short)outitem;
                            dbRoleAuth.ID = maxIDFromRoleActionAuth;
                            maxIDFromRoleActionAuth = maxIDFromRoleActionAuth + 1;
                            dbContext.UMG_RoleActionAuth.Add(dbRoleAuth);

                            // For Log By Shubham Bhagat on 5-4-2019
                            UMG_RoleActionAuth_Log umgRoleActionAuthLog = new UMG_RoleActionAuth_Log();
                            umgRoleActionAuthLog.CAID = updateModel.CAID;
                            umgRoleActionAuthLog.RoleID = (short)outitem;
                            umgRoleActionAuthLog.ID = dbRoleAuth.ID;
                            umgRoleActionAuthLog.ActionPerformed = "Insert";
                            UMG_RoleActionAuth_LogList.Add(umgRoleActionAuthLog);

                        }
                        matchStatus = false;
                    }
                    //for deletion
                    foreach (var outitem in arrayRoleId)
                    {
                        foreach (var inneritem in updateModel.RoleId)
                        {
                            if (inneritem == outitem)
                            {
                                matchStatus = true;
                            }

                        }
                        if (!matchStatus)
                        {
                            UMG_RoleActionAuth objroleauth = new UMG_RoleActionAuth();
                            objroleauth = dbContext.UMG_RoleActionAuth.Where(x => x.CAID == updateModel.CAID && x.RoleID == outitem).FirstOrDefault();
                            if (objroleauth != null)
                            {
                                // For Log By Shubham Bhagat on 5-4-2019
                                UMG_RoleActionAuth_Log umgRoleActionAuthLog = new UMG_RoleActionAuth_Log();
                                umgRoleActionAuthLog.CAID = objroleauth.CAID;
                                umgRoleActionAuthLog.RoleID = objroleauth.RoleID;
                                umgRoleActionAuthLog.ID = objroleauth.ID;
                                umgRoleActionAuthLog.ActionPerformed = "Delete";
                                UMG_RoleActionAuth_LogList.Add(umgRoleActionAuthLog);
                            }
                            dbContext.UMG_RoleActionAuth.Remove(objroleauth);

                        }
                        matchStatus = false;
                    }
                }
               

                long LogIDRoleActionAuth = (dbContext.UMG_RoleActionAuth_Log.Any() ? dbContext.UMG_RoleActionAuth_Log.Max(a => a.LogID) : 0) + 1;
                foreach (var item in UMG_RoleActionAuth_LogList)
                {
                    item.LogID = LogIDRoleActionAuth;
                    item.UpdateDateTime = DateTime.Now;
                    item.UserID = updateModel.UserIdForActivityLogFromSession;
                    item.UserIPAddress = updateModel.UserIPAddress;
                    dbContext.UMG_RoleActionAuth_Log.Add(item);

                    LogIDRoleActionAuth = LogIDRoleActionAuth + 1;// increase primary key value
                }
                 */

                #endregion




                //// added by m rafe on 28-11-19
                //UMG_MenuActionAuthorizationMapping _MenuActionAuthorizationMapping = dbContext.UMG_MenuActionAuthorizationMapping.Where(x => x.CAID == updateModel.CAID).FirstOrDefault();
                //_MenuActionAuthorizationMapping.MenuId = updateModel.MenuDetailsId;



                // added by m rafe on 28-11-19


                var existingList = dbContext.UMG_MenuActionAuthorizationMapping.Where(x => x.CAID == updateModel.CAID).ToList();
                if (existingList != null && existingList.Count > 0)
                {
                    dbContext.UMG_MenuActionAuthorizationMapping.RemoveRange(existingList);
                }
                foreach (var item in updateModel.MenuDetailsId)
                {
                    UMG_MenuActionAuthorizationMapping authorizationMapping = new UMG_MenuActionAuthorizationMapping();
                    authorizationMapping.CAID = updateModel.CAID;
                    authorizationMapping.MenuId = item;
                    dbContext.UMG_MenuActionAuthorizationMapping.Add(authorizationMapping);
                }

                dbContext.SaveChanges();


                // For Activity Log

                //  BELOW LINES ARE COMMENTED BY M RAFE ON 04-12-19
                // ROLE ACTION AUTHORIZATION IS NOW GIVEN TO DEPTADMIN AND PERFORMED BY UpdateRoleActionAuth  METHOD
                //
                /*
                String oldRoleName = "Previous Roles who have authorization : ";
                String newRoleName = "New Roles who have authorization : ";
                List<UMG_RoleDetails> roleDetailsList = dbContext.UMG_RoleDetails.ToList();

                foreach (var item in dbRoleActionAuth)
                {
                    foreach (UMG_RoleDetails umg_roleDetails in roleDetailsList)
                    {
                        if (item.RoleID == umg_roleDetails.RoleID)
                        {
                            oldRoleName = oldRoleName + "\"" + umg_roleDetails.RoleName + "\", ";
                        }
                    }
                }
                foreach (var item in updateModel.RoleId)
                {
                    foreach (UMG_RoleDetails umg_roleDetails in roleDetailsList)
                    {
                        if (item == umg_roleDetails.RoleID)
                        {
                            newRoleName = newRoleName + "\"" + umg_roleDetails.RoleName + "\", ";
                        }
                    }
                }

                */


                // For Activity Log
                //String messageForActivityLog = "Action Detail Updated # " + updateModel.AreaNameId + "/" + updateModel.ControllerNameId + "/" + updateModel.ActionNameId + "- Action Detail Updated." + oldRoleName + " has Updated to " + newRoleName;

                //  ABOVE LINE IS COMMENTED BY M RAFE ON 04-12-19

                String messageForActivityLog = "Action Detail Updated # " + updateModel.AreaNameId + "/" + updateModel.ControllerNameId + "/" + updateModel.ActionNameId + "- Action Detail Updated.";
                if (messageForActivityLog.Length < 1000)
                    ApiCommonFunctions.SystemUserActivityLog(updateModel.UserIdForActivityLogFromSession, Convert.ToInt32(Common.ApiCommonEnum.AdminActivityType.ControllerActionDetails), messageForActivityLog);

                return responseModel;
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

        #region Commented By Shubham Bhagat on 18-06-2019 all working
        /// <summary>
        /// Deletes ControllerData
        /// </summary>
        /// <param name="EncryptedID"></param>
        /// <returns></returns>
        public bool DeleteControllerData(String EncryptedID)
        {
            try
            {
                ControllerActionModel responseModel = new ControllerActionModel();
                UMG_RoleActionAuth dbRoleAuth = new UMG_RoleActionAuth();
                UMG_ControllerActionDetails dbCAObj = new UMG_ControllerActionDetails();
                encryptedParameters = EncryptedID.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int CAID = Convert.ToInt32(decryptedParameters["CAID"].ToString().Trim());
                dbCAObj = dbContext.UMG_ControllerActionDetails.Where(x => x.CAID == CAID).FirstOrDefault();
                if (dbCAObj != null)
                {
                    dbContext.UMG_ControllerActionDetails.Remove(dbCAObj);
                    dbContext.UMG_RoleActionAuth.RemoveRange(dbContext.UMG_RoleActionAuth.Where(r => r.CAID == CAID));
                    //  dbContext.UMG_RoleActionAuth.Remove(dbRoleAuth);


                    // added by m rafe on 28-11-19


                    var existingList = dbContext.UMG_MenuActionAuthorizationMapping.Where(x => x.CAID == CAID).ToList();
                    if (existingList != null && existingList.Count > 0)
                    {
                        dbContext.UMG_MenuActionAuthorizationMapping.RemoveRange(existingList);
                    }

                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
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

        /// <summary>
        /// Gets RoleData
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public List<SelectListItem> GetRoleData(int[] array)
        {
            List<SelectListItem> roleList = new List<SelectListItem>();
            try
            {
                if (array == null)
                {

                    SelectListItem objRole = new SelectListItem();
                    foreach (var role in dbContext.UMG_RoleDetails.OrderBy(c => c.RoleName).ToList())
                    {
                        objRole = new SelectListItem();
                        objRole.Text = role.RoleName;
                        objRole.Value = role.RoleID.ToString();
                        roleList.Add(objRole);
                    }
                }
                else
                {
                    SelectListItem objRole = new SelectListItem();
                    foreach (var role in dbContext.UMG_RoleDetails.OrderBy(c => c.RoleName).ToList())
                    {
                        objRole = new SelectListItem();
                        if (array.Contains(role.RoleID))
                            objRole.Selected = true;
                        objRole.Text = role.RoleName;
                        objRole.Value = role.RoleID.ToString();
                        roleList.Add(objRole);
                    }
                }
                return roleList;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// Gets ControllerList
        /// </summary>
        /// <param name="areaList"></param>
        /// <param name="areaName"></param>
        /// <returns></returns>
        public List<SelectListItem> GetControllerList(List<KaveriArea> areaList, String areaName = "")
        {
            List<SelectListItem> ControllerList = new List<SelectListItem>();
            SelectListItem objController = new SelectListItem();
            List<KaveriController> controllerList = new List<KaveriController>();
            try
            {
                if (areaName == "")
                    areaName = "NoArea";
                objController.Text = "--Select Controller--";
                objController.Value = "0";
                ControllerList.Add(objController);
                controllerList = areaList.Where(x => x.Name == areaName).FirstOrDefault().KaveriControllers;


                foreach (var itemKaveriController in controllerList)
                {
                    objController = new SelectListItem();
                    objController.Text = itemKaveriController.Name;
                    objController.Value = itemKaveriController.Name.ToString();
                    ControllerList.Add(objController);
                }



                return ControllerList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<SelectListItem> GetAreaList(List<KaveriArea> areaList)
        {
            List<SelectListItem> AreaList = new List<SelectListItem>();
            SelectListItem objArea = new SelectListItem();
            try
            {
                objArea.Text = "--Select Area--";
                objArea.Value = "0";
                AreaList.Add(objArea);
                foreach (var Area in areaList)
                {
                    objArea = new SelectListItem();
                    if (Area.Name == "NoArea")
                    {
                        objArea.Text = "CommonArea";
                        objArea.Value = Area.Name.ToString();
                    }
                    else
                    {
                        objArea.Text = Area.Name;
                        objArea.Value = Area.Name.ToString();
                    }
                    AreaList.Add(objArea);
                }



                return AreaList;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Gets ActionList
        /// </summary>
        /// <param name="areaList"></param>
        /// <param name="areaName"></param>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public List<SelectListItem> GetActionList(List<KaveriArea> areaList, String areaName, String controllerName = "")
        {
            List<SelectListItem> ActionList = new List<SelectListItem>();
            SelectListItem objAction = new SelectListItem();
            List<KaveriController> controllerList = new List<KaveriController>();
            List<KaveriAction> GActionList = new List<KaveriAction>();
            try
            {
                if (areaName == "")
                    areaName = "NoArea";

                objAction.Text = "--Select Action--";
                objAction.Value = "0";
                ActionList.Add(objAction);
                controllerList = areaList.Where(x => x.Name == areaName).FirstOrDefault().KaveriControllers;
                GActionList = controllerList.Where(x => x.Name == controllerName).FirstOrDefault().MyActions;
                foreach (var itemKaveriAction in GActionList)
                {
                    objAction = new SelectListItem();
                    if (itemKaveriAction.IsHttpGet == true)
                        objAction.Text = itemKaveriAction.Name + "(Get)";
                    else if (itemKaveriAction.IsHttpPost == true)
                        objAction.Text = itemKaveriAction.Name + "(Post)";
                    else if (itemKaveriAction.IsHttpOther == true)
                        objAction.Text = itemKaveriAction.Name + "(Other)";
                    else
                        objAction.Text = itemKaveriAction.Name + "(Get)";
                    objAction.Value = itemKaveriAction.Name.ToString();
                    ActionList.Add(objAction);
                }



                return ActionList;
            }
            catch (Exception)
            {
                throw;
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


        public ControllerActionModel GetRoleAuthView(string EncryptedID)
        {
            try
            {
                dbContext = new KaveriEntities();
                ControllerActionModel responseModel = new ControllerActionModel();
                responseModel.RoleList = new List<SelectListItem>();
                encryptedParameters = EncryptedID.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int caid = Convert.ToInt32(decryptedParameters["CAID"].ToString().Trim());



                List<short> dbRoleActionAuth = dbContext.UMG_RoleActionAuth.Where(x => x.CAID == caid).Select(C => C.RoleID).ToList();



                var roleListDB = dbContext.UMG_RoleDetails.OrderBy(c => c.RoleName).Select(x => new { x.RoleID, x.RoleName }).ToList();
                foreach (var role in roleListDB)
                {
                    SelectListItem objRole = new SelectListItem();
                    if (dbRoleActionAuth.Contains(role.RoleID))
                        objRole.Selected = true;
                    objRole.Text = role.RoleName;
                    //objRole.Value = role.RoleID.ToString();
                    objRole.Value = URLEncrypt.EncryptParameters(new String[] { "RoleID=" + role.RoleID });
                    responseModel.RoleList.Add(objRole);
                }




                // responseModel.role = dbContext.UMG_RoleDetails.Where(x => x.RoleID == (dbContext.UMG_RoleActionAuth.Where(xc => xc.CAID == caid).FirstOrDefault().RoleID)).FirstOrDefault().RoleName;
                return responseModel;
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
        public ControllerActionModel UpdateRoleActionAuth(ControllerActionModel model)
        {

            try
            {

                ControllerActionModel responseModel = new ControllerActionModel();

                encryptedParameters = model.EncryptedID.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int caid = Convert.ToInt32(decryptedParameters["CAID"].ToString().Trim());

                model.CAID = caid;

                List<int> RoleIdListInt = new List<int>();
                if (model.RoleId != null)
                {
                    foreach (var item in model.RoleId)
                    {
                        var encryptedParameters2 = item.Split('/');
                        var decryptedParameters2 = URLEncrypt.DecryptParameters(new String[] { encryptedParameters2[0], encryptedParameters2[1], encryptedParameters2[2] });
                        RoleIdListInt.Add(Convert.ToInt32(decryptedParameters2["RoleID"].ToString().Trim()));

                    }
                }



                using (TransactionScope ts = new TransactionScope())
                {

                    dbContext = new KaveriEntities();

                    var dbRoleActionAuthList = dbContext.UMG_RoleActionAuth.Where(x => x.CAID == model.CAID).ToList();
                    var TempList = new List<short>();

                    var UMG_RoleActionAuth_LogList = new List<UMG_RoleActionAuth_Log>();
                    long LogIDRoleActionAuth = (dbContext.UMG_RoleActionAuth_Log.Any() ? dbContext.UMG_RoleActionAuth_Log.Max(a => a.LogID) : 0) + 1;

                    if (dbRoleActionAuthList != null && dbRoleActionAuthList.Count > 0)
                    {
                        TempList.AddRange(dbRoleActionAuthList.Select(x => x.RoleID).ToList());
                        foreach (var x in dbRoleActionAuthList)
                        {
                            //if (model.RoleId != null && model.RoleId.Contains(x.RoleID))
                            if (RoleIdListInt.Contains(x.RoleID))
                            {
                            }
                            else
                            {
                                UMG_RoleActionAuth_LogList.Add(new UMG_RoleActionAuth_Log()
                                {
                                    CAID = x.CAID,
                                    RoleID = x.RoleID,
                                    ID = x.ID,
                                    ActionPerformed = "Delete",
                                    LogID = LogIDRoleActionAuth++,
                                    UpdateDateTime = DateTime.Now,
                                    UserID = model.UserIdForActivityLogFromSession,
                                });
                            }

                        }
                        dbContext.UMG_RoleActionAuth.RemoveRange(dbRoleActionAuthList);
                    }


                    if (RoleIdListInt.Count > 0)
                    {
                        int maxIDFromRoleActionAuth = (dbContext.UMG_RoleActionAuth.Any() ? dbContext.UMG_RoleActionAuth.Max(x => x.ID) : 0);
                        // foreach (var item in model.RoleId)
                        foreach (var item in RoleIdListInt)
                        {
                            UMG_RoleActionAuth dbRoleAuth = new UMG_RoleActionAuth();
                            dbRoleAuth.CAID = model.CAID;
                            dbRoleAuth.RoleID = (short)item;
                            dbRoleAuth.ID = ++maxIDFromRoleActionAuth;
                            dbContext.UMG_RoleActionAuth.Add(dbRoleAuth);

                            if (TempList.Contains((short)item))
                            {
                            }
                            else
                            {
                                UMG_RoleActionAuth_LogList.Add(new UMG_RoleActionAuth_Log()
                                {
                                    CAID = model.CAID,
                                    RoleID = (short)item,
                                    ID = maxIDFromRoleActionAuth,
                                    ActionPerformed = "Insert",
                                    LogID = LogIDRoleActionAuth++,
                                    UpdateDateTime = DateTime.Now,
                                    UserID = model.UserIdForActivityLogFromSession,
                                });
                            }

                        }
                    }
                    if (UMG_RoleActionAuth_LogList != null && UMG_RoleActionAuth_LogList.Count > 0)
                    {
                        dbContext.UMG_RoleActionAuth_Log.AddRange(UMG_RoleActionAuth_LogList);
                    }

                    dbContext.SaveChanges();
                    ts.Complete();
                }
                return responseModel;
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }



        }

    }



}
