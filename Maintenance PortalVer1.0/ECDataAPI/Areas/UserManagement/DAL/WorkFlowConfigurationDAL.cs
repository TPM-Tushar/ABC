using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.Interface;
using ECDataAPI.Common;
using ECDataAPI.Entity;
using Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Web;
using System.Web.Mvc;
using static ECDataAPI.Common.ApiCommonFunctions;

namespace ECDataAPI.Areas.UserManagement.DAL
{
    public class WorkFlowConfigurationDAL : IWorkFlowConfiguration
    {
        private String[] encryptedParameters = null;
        private Dictionary<String, String> decryptedParameters = null;

        /// <summary>
        /// Gets WorkFlowConfigurationDetails
        /// </summary>
        /// <returns></returns>
        public WorkFlowConfigurationModel GetWorkFlowConfigurationDetails()
        {
            WorkFlowConfigurationModel model = new WorkFlowConfigurationModel();
            model.ActionList = GetActionList();
            model.FromRoleList = GetRoleList();
            model.ToRoleList = GetRoleList();
            model.ServiceList = GetServiceList();
            model.OfficeList = GetOfficeNameList();
            // Shubham Bhagat
            model.ActionList_ForReverseofficeConfiguration = GetActionList();
            model.FromRoleList_ForReverseofficeConfiguration = GetRoleList();
            model.ToRoleList_ForReverseofficeConfiguration = GetRoleList();
            model.ServiceList_ForReverseofficeConfiguration = GetServiceList();
            model.OfficeList_ForReverseofficeConfiguration = GetOfficeNameList();
            return model;
        }

        /// <summary>
        /// Creates NewWorkFlowConfiguration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public bool CreateNewWorkFlowConfiguration(WorkFlowConfigurationModel model)
        {

            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            MAS_UMG_WorkFlowConfigurations objNewWorkflowConfiguration = new MAS_UMG_WorkFlowConfigurations();
            try
            {
                if (model != null)
                {                   
                    objNewWorkflowConfiguration.WorkFlowID = (dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Any() ? dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Max(a => a.WorkFlowID) : 0) + 1; //dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Max(x => x.WorkFlowID);
                    //objNewWorkflowConfiguration.WorkFlowID++;
                    objNewWorkflowConfiguration.ActionID = (byte)model.ActionId;
                    objNewWorkflowConfiguration.FromRoleID = (short)model.FromRoleId;
                    objNewWorkflowConfiguration.ToRoleID = (short)model.ToRoleId;
                    objNewWorkflowConfiguration.OfficeID = (short)model.OfficeId;
                    objNewWorkflowConfiguration.IsActive = model.IsActive;
                    objNewWorkflowConfiguration.ServiceID = (short)model.ServiceId;


                    dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Add(objNewWorkflowConfiguration);

                    if (model.ToAddReverseOfficeConfiguration == true) {
                        MAS_UMG_WorkFlowConfigurations mas_umg_WorkFlowConfigurations = new MAS_UMG_WorkFlowConfigurations();
                        mas_umg_WorkFlowConfigurations.WorkFlowID = objNewWorkflowConfiguration.WorkFlowID++;
                        mas_umg_WorkFlowConfigurations.ActionID = (byte)model.ActionId_ReverseofficeConfiguration_Hidden;
                        mas_umg_WorkFlowConfigurations.FromRoleID = (short)model.FromRoleId_ReverseofficeConfiguration_Hidden;
                        mas_umg_WorkFlowConfigurations.ToRoleID = (short)model.ToRoleId_ReverseofficeConfiguration_Hidden;
                        mas_umg_WorkFlowConfigurations.OfficeID = (short)model.OfficeId_ReverseofficeConfiguration_Hidden;
                        mas_umg_WorkFlowConfigurations.IsActive = model.IsActive_ReverseofficeConfiguration_Hidden;
                        mas_umg_WorkFlowConfigurations.ServiceID = (short)model.ServiceId_ReverseofficeConfiguration_Hidden;
                        dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Add(mas_umg_WorkFlowConfigurations);
                    }
                    dbKaveriOnlineContext.SaveChanges();
                    return true;
                }
                return false;

            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {

                dbKaveriOnlineContext.Dispose();
            }

        }

        /// <summary>
        /// Loads WorkFlowConfigurationGridData
        /// </summary>
        /// <returns></returns>

        public WorkFlowConfigurationGridWrapperModel LoadWorkFlowConfigurationGridData()
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {




                WorkFlowConfigurationGridWrapperModel returnModel = new WorkFlowConfigurationGridWrapperModel();
                List<WorkFlowConfigurationModel> lstDatamodel = new List<WorkFlowConfigurationModel>();
                WorkFlowConfigurationModel GridModel = null;
                List<WorkFlowConfigurationDataColumn> lstcolumn = new List<WorkFlowConfigurationDataColumn>();
                List<MAS_UMG_WorkFlowConfigurations> resultList = null;

                resultList = dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.ToList();

                foreach (var item in resultList)
                {

                    GridModel = new WorkFlowConfigurationModel();
                    GridModel.EncryptedId = URLEncrypt.EncryptParameters(new String[] { "WorkFlowId=" + item.WorkFlowID });
                    GridModel.ActionDesc = dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.Where(x => x.ActionID == item.ActionID).FirstOrDefault().Description;
                    GridModel.FromRoleDesc = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID == item.FromRoleID).FirstOrDefault().RoleName;
                    GridModel.ToRoleDesc = dbKaveriOnlineContext.UMG_RoleDetails.Where(x => x.RoleID == item.ToRoleID).FirstOrDefault().RoleName;
                    GridModel.IsActive = item.IsActive;
                    if (GridModel.IsActive)
                        GridModel.IsActiveIcon = "<i class='fa fa-check  ' style='color:black'></i>";
                    else
                        GridModel.IsActiveIcon = "<i class='fa fa-close  ' style='color:black'></i>";
                    GridModel.OfficeName = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x => x.OfficeID == item.OfficeID).FirstOrDefault().OfficeName;
                    GridModel.ServiceName = dbKaveriOnlineContext.MAS_ServiceMaster.Where(x => x.ServiceID == item.ServiceID).FirstOrDefault().ServiceName;
                    GridModel.EditBtn = "<a href='#'  onclick=UpdateWorkflowConfigurationData('" + GridModel.EncryptedId + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                 //   GridModel.DeleteBtn = "<a href='#'  onclick=DeleteWorkflowConfigurationData('" + GridModel.EncryptedId + "'); ><i class='fa fa-trash fa-2x  ' style='color:black'></i></a>";
                    lstDatamodel.Add(GridModel);
                }

                lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "SR NO", data = "ActionDesc" });
                lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "Action ", data = "ActionDesc" });
                lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "From Role", data = "FromRoleDesc" });
                lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "To Role", data = "ToRoleDesc" });
                lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "Active Status", data = "IsActiveIcon" });
                lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "Office Name", data = "OfficeName" });
                lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "Service Name", data = "ServiceName" });

                lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "Edit", data = "EditBtn" });
               // lstcolumn.Add(new WorkFlowConfigurationDataColumn { title = "Delete", data = "DeleteBtn" });

                returnModel.dataArray = lstDatamodel.ToArray();
                returnModel.ColumnArray = lstcolumn.ToArray();
                return returnModel;

            }
            catch (Exception)
            {

                throw;
            }
            finally
            {

            }
        }

        /// <summary>
        /// Gets WorkFlowConfigurationModel
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public WorkFlowConfigurationModel GetWorkFlowConfigurationModel(string EncryptedId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                WorkFlowConfigurationModel responseModel = new WorkFlowConfigurationModel();
                MAS_UMG_WorkFlowConfigurations dbWorkFlowConfiguration = new MAS_UMG_WorkFlowConfigurations();

                encryptedParameters = EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int WorkFlowId = Convert.ToInt32(decryptedParameters["WorkFlowId"].ToString().Trim());
                dbWorkFlowConfiguration = dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Where(x => x.WorkFlowID == WorkFlowId).FirstOrDefault();

                responseModel.ActionId = dbWorkFlowConfiguration.ActionID;
                responseModel.FromRoleId = dbWorkFlowConfiguration.FromRoleID;
                responseModel.ToRoleId = dbWorkFlowConfiguration.ToRoleID;
                responseModel.OfficeId = dbWorkFlowConfiguration.OfficeID;
                responseModel.IsActive = dbWorkFlowConfiguration.IsActive;
                responseModel.ServiceId = (short)dbWorkFlowConfiguration.ServiceID;

                responseModel.ActionList = GetActionList();
                responseModel.FromRoleList = GetRoleList();
                responseModel.ToRoleList = GetRoleList();
                responseModel.ServiceList = GetServiceList();
                responseModel.OfficeList = GetOfficeNameList();

                //Added By Shubham Bhagat on 18-12-2018
                responseModel.ActionList_ForReverseofficeConfiguration = GetActionList();
                responseModel.FromRoleList_ForReverseofficeConfiguration = GetRoleList();
                responseModel.ToRoleList_ForReverseofficeConfiguration = GetRoleList();
                responseModel.ServiceList_ForReverseofficeConfiguration = GetServiceList();
                responseModel.OfficeList_ForReverseofficeConfiguration = GetOfficeNameList();
                return responseModel;
            }
            catch (Exception )
            {

                throw ;
            }
            finally
            {

                dbKaveriOnlineContext.Dispose();
            }

        }

        /// <summary>
        /// Updates WorkFlowConfiguration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WorkFlowConfigurationResponseModel UpdateWorkFlowConfiguration(WorkFlowConfigurationModel model)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {

                WorkFlowConfigurationResponseModel ResponseModel = new WorkFlowConfigurationResponseModel();

                MAS_UMG_WorkFlowConfigurations objNewWorkflowConfiguration = new MAS_UMG_WorkFlowConfigurations();
                MAS_UMG_WorkFlowConfigurations_Log mas_UMG_WorkFlowConfigurations_Log = new MAS_UMG_WorkFlowConfigurations_Log();
                encryptedParameters = model.EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int WorkFlowId = Convert.ToInt32(decryptedParameters["WorkFlowId"].ToString().Trim());

                // For Logging before update on 5-1-2019 By Shubham Bhagat   



                objNewWorkflowConfiguration = dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Where(x => x.WorkFlowID == WorkFlowId).FirstOrDefault();



                //Compare UMG_UserProfile object
                WorkFlowConfigurationModel OfficeObj1 = new WorkFlowConfigurationModel();
                OfficeObj1.ActionId = model.ActionId;
                OfficeObj1.FromRoleId = model.FromRoleId;
                OfficeObj1.ToRoleId = model.ToRoleId;
                OfficeObj1.OfficeId = model.OfficeId;
                OfficeObj1.ServiceId = model.ServiceId;
                OfficeObj1.IsActive = model.IsActive;

                WorkFlowConfigurationModel OfficeObj2 = new WorkFlowConfigurationModel();
                OfficeObj2.ActionId = objNewWorkflowConfiguration.ActionID;
                OfficeObj2.FromRoleId = objNewWorkflowConfiguration.FromRoleID;
                OfficeObj2.ToRoleId = objNewWorkflowConfiguration.ToRoleID;
                OfficeObj2.OfficeId = objNewWorkflowConfiguration.OfficeID;
                OfficeObj2.ServiceId = objNewWorkflowConfiguration.ServiceID??0;
                OfficeObj2.IsActive = objNewWorkflowConfiguration.IsActive;




                bool IsObjectsSame = ApiCommonFunctions.CompareObjectsBeforeUpdate<WorkFlowConfigurationModel>(OfficeObj1, OfficeObj2);


                if (IsObjectsSame)
                {
                    //Same record..
                    ResponseModel.IsRecordUpdated = true;
                    ResponseModel.ResponseMessage = "No change found in work flow configuration details";

                    return ResponseModel;
                }
                else {
                    //Changes in record..
                    mas_UMG_WorkFlowConfigurations_Log.UpdateID = (dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations_Log.Any() ? dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations_Log.Max(x => x.UpdateID) : 0) + 1;

                    mas_UMG_WorkFlowConfigurations_Log.ActionID = objNewWorkflowConfiguration.ActionID;
                    mas_UMG_WorkFlowConfigurations_Log.WorkFlowID = objNewWorkflowConfiguration.WorkFlowID;
                    mas_UMG_WorkFlowConfigurations_Log.FromRoleID = objNewWorkflowConfiguration.FromRoleID;
                    mas_UMG_WorkFlowConfigurations_Log.ToRoleID = objNewWorkflowConfiguration.ToRoleID;
                    mas_UMG_WorkFlowConfigurations_Log.IsActive = objNewWorkflowConfiguration.IsActive;
                    mas_UMG_WorkFlowConfigurations_Log.OfficeID = objNewWorkflowConfiguration.OfficeID;
                    mas_UMG_WorkFlowConfigurations_Log.ServiceID = objNewWorkflowConfiguration.ServiceID;
                    mas_UMG_WorkFlowConfigurations_Log.UpdateDateTime = System.DateTime.Now;
                    dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations_Log.Add(mas_UMG_WorkFlowConfigurations_Log);

                    // For Logging before update on 5-1-2019 By Shubham Bhagat  


                    // objNewWorkflowAction.ActionID = (byte)model.ActionId;
                    objNewWorkflowConfiguration.ActionID = (byte)model.ActionId;
                    objNewWorkflowConfiguration.FromRoleID = (short)model.FromRoleId;
                    objNewWorkflowConfiguration.ToRoleID = (short)model.ToRoleId;
                    objNewWorkflowConfiguration.OfficeID = (short)model.OfficeId;
                    objNewWorkflowConfiguration.IsActive = model.IsActive;
                    objNewWorkflowConfiguration.ServiceID = (short)model.ServiceId;
                    dbKaveriOnlineContext.SaveChanges();

                    ResponseModel.IsRecordUpdated = true;
                    ResponseModel.ResponseMessage= "Work flow configuration details updated successfully";

                    return ResponseModel;
                }


         
            }
            catch (Exception)
            {

                throw;
            }
            finally
            {
                dbKaveriOnlineContext.Dispose();
            }

        }

        //public bool UpdateWorkFlowConfiguration(WorkFlowConfigurationModel model)
        //{
        //    KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
        //    try
        //    {

        //        MAS_UMG_WorkFlowConfigurations objNewWorkflowConfiguration = new MAS_UMG_WorkFlowConfigurations();

        //        encryptedParameters = model.EncryptedId.Split('/');

        //        if (encryptedParameters.Length != 3)
        //        {
        //            throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
        //        }

        //        decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

        //        int WorkFlowId = Convert.ToInt32(decryptedParameters["WorkFlowId"].ToString().Trim());


        //        objNewWorkflowConfiguration = dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Where(x => x.WorkFlowID == WorkFlowId).FirstOrDefault();
        //        // objNewWorkflowAction.ActionID = (byte)model.ActionId;
        //        objNewWorkflowConfiguration.ActionID = (byte)model.ActionId;
        //        objNewWorkflowConfiguration.FromRoleID = (short)model.FromRoleId;
        //        objNewWorkflowConfiguration.ToRoleID = (short)model.ToRoleId;
        //        objNewWorkflowConfiguration.OfficeID = (short)model.OfficeId;
        //        objNewWorkflowConfiguration.IsActive = model.IsActive;
        //        objNewWorkflowConfiguration.ServiceID = (short)model.ServiceId;
        //        dbKaveriOnlineContext.SaveChanges();
        //        return true;
        //    }
        //    catch (Exception )
        //    {

        //        throw ;
        //    }
        //    finally
        //    {
        //        dbKaveriOnlineContext.Dispose();
        //    }

        //}

        /// <summary>
        /// Deletes WorkFlowConfiguration
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public bool DeleteWorkFlowConfiguration(string EncryptedId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            MAS_UMG_WorkFlowConfigurations objdbWorkFlowConfigurationModel = new MAS_UMG_WorkFlowConfigurations();
            try
            {
                encryptedParameters = EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int WorkFlowId = Convert.ToInt32(decryptedParameters["WorkFlowId"].ToString().Trim());
                objdbWorkFlowConfigurationModel = dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Where(x => x.WorkFlowID == WorkFlowId).FirstOrDefault();
                if (objdbWorkFlowConfigurationModel != null)
                {
                    dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Remove(objdbWorkFlowConfigurationModel);
                    dbKaveriOnlineContext.SaveChanges();

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

        }
        public List<SelectListItem> GetActionList()
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> ActionList = new List<SelectListItem>();
                SelectListItem objAction = new SelectListItem();
                objAction.Text = "--Select Action--";
                objAction.Value = "0";
                ActionList.Add(objAction);
                foreach (var Action in dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.ToList())
                {
                    objAction = new SelectListItem();
                    objAction.Text = Action.Description;
                    objAction.Value = Action.ActionID.ToString();
                    ActionList.Add(objAction);
                }
                return ActionList;
            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {

                dbKaveriOnlineContext.Dispose();
            }
        }
        public List<SelectListItem> GetRoleList()
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                List<SelectListItem> roleList = new List<SelectListItem>();
                SelectListItem objRole = new SelectListItem();
                objRole.Text = "--Select Role--";
                objRole.Value = "0";
                roleList.Add(objRole);
                foreach (var role in dbKaveriOnlineContext.UMG_RoleDetails.ToList())
                {
                    objRole = new SelectListItem();
                    objRole.Text = role.RoleName;
                    objRole.Value = role.RoleID.ToString();
                    roleList.Add(objRole);
                }
                return roleList;
            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {

                dbKaveriOnlineContext.Dispose();
            }
        }
      
        public List<SelectListItem> GetOfficeNameList()
        {
            // GOAIGR_REG_CENTRALIZEDEntities dbKaveriCentralizedContext = new GOAIGR_REG_CENTRALIZEDEntities();
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {

                List<SelectListItem> objOfficeNametList = new List<SelectListItem>();
                SelectListItem objOfficeName = new SelectListItem();
                List<MAS_OfficeMaster> objOfficeNameData = new List<MAS_OfficeMaster>();
                objOfficeName.Text = "--Select Office--";
                objOfficeName.Value = "0";
                objOfficeNametList.Add(objOfficeName);

                objOfficeNameData = dbKaveriOnlineContext.MAS_OfficeMaster.Where(x=>x.OfficeTypeID==3).ToList();


                foreach (var ON in objOfficeNameData)
                {
                    objOfficeName = new SelectListItem();
                    objOfficeName.Text = ON.OfficeName;
                    objOfficeName.Value = ON.OfficeID.ToString();
                    objOfficeNametList.Add(objOfficeName);
                }
                return objOfficeNametList;

            }
            catch (Exception )
            {
                throw ;
            }
            finally
            {

                dbKaveriOnlineContext.Dispose();
            }
        }
    }
}