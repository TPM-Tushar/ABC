using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.Interface;
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
    public class WorkflowActionDAL: IWorkflowAction
    {
        private String[] encryptedParameters = null;
        private Dictionary<String, String> decryptedParameters = null;

        /// <summary>
        /// Gets WorkflowActionDetails
        /// </summary>
        /// <returns></returns>
        public WorkflowActionModel GetWorkflowActionDetails()
        {
            WorkflowActionModel model = new WorkflowActionModel();
            model.ServiceList = GetServiceList();
            return model;
        }

        /// <summary>
        /// Creates NewWorkflowAction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CreateNewWorkflowAction(WorkflowActionModel model)
        {

            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            //KaveriEntities dbKaveriOnlineContext = new KaveriEntities();

            
            MAS_UMG_WorkFlowActions objNewWorkflowAction = new MAS_UMG_WorkFlowActions();
            try
            {
                if (model != null)
                {
                    objNewWorkflowAction.ActionID = dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.Max(x => x.ActionID);
                    objNewWorkflowAction.ActionID++;
                    objNewWorkflowAction.Description = model.Discription;
                    objNewWorkflowAction.DescriptionR = model.DiscriptionR;
                    objNewWorkflowAction.StatusMessage = model.StatusMessage;
                    objNewWorkflowAction.StatusMessageR = model.StatusMessageR;
                    objNewWorkflowAction.IsActive = (bool)model.isActive;
                    objNewWorkflowAction.ServiceID = (short)model.ServiceId;


                    dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.Add(objNewWorkflowAction);
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
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }

        }

        /// <summary>
        /// Loads WorkflowActionGridData
        /// </summary>
        /// <returns></returns>

        public WorkflowActionGridWrapperModel LoadWorkflowActionGridData()
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            
            try
            {




                WorkflowActionGridWrapperModel returnModel = new WorkflowActionGridWrapperModel();
                List<WorkflowActionModel> lstDatamodel = new List<WorkflowActionModel>();
                WorkflowActionModel GridModel = null;
                List<WorkflowActionDataColumn> lstcolumn = new List<WorkflowActionDataColumn>();
                List<MAS_UMG_WorkFlowActions> resultList = null;
                //.Where(x => x.IsActive == true)
                resultList = dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.ToList();

                foreach (var item in resultList)
                {

                    GridModel = new WorkflowActionModel();
                    GridModel.EncryptedId = URLEncrypt.EncryptParameters(new String[] { "ActionId=" + item.ActionID });
                    GridModel.Discription = item.Description;
                 //   GridModel.DiscriptionR = item.DescriptionR;
                    GridModel.StatusMessage = item.StatusMessage;
                  //  GridModel.StatusMessageR = item.StatusMessageR;
                    GridModel.isActive = item.IsActive;
                    if (GridModel.isActive)
                        GridModel.IsActiveIcon = "<i class='fa fa-check  ' style='color:black'></i>";
                    else
                        GridModel.IsActiveIcon = "<i class='fa fa-close  ' style='color:black'></i>";
                    GridModel.ServiceName = dbKaveriOnlineContext.MAS_ServiceMaster.Where(x => x.ServiceID == item.ServiceID).FirstOrDefault().ServiceName;
                    GridModel.EditBtn = "<a href='#'  onclick=UpdateWorkflowActionData('" + GridModel.EncryptedId + "'); ><i class='fa fa-pencil fa-2x ' style='color:black'></i></a>";
                   // GridModel.DeleteBtn = "<a href='#'  onclick=DeleteWorkflowActionData('" + GridModel.EncryptedId + "'); ><i class='fa fa-trash fa-2x  ' style='color:black'></i></a>";
                    lstDatamodel.Add(GridModel);
                }

                lstcolumn.Add(new WorkflowActionDataColumn { title = "SR NO", data = "Discription" });
                lstcolumn.Add(new WorkflowActionDataColumn { title = "Discription ", data = "Discription" });
               // lstcolumn.Add(new WorkflowActionDataColumn { title = "Discription(R)", data = "DiscriptionR" });

                lstcolumn.Add(new WorkflowActionDataColumn { title = "Status Message", data = "StatusMessage" });
               // lstcolumn.Add(new WorkflowActionDataColumn { title = "Status Message(R) ", data = "StatusMessageR" });
                lstcolumn.Add(new WorkflowActionDataColumn { title = "Service Name", data = "ServiceName" });
                lstcolumn.Add(new WorkflowActionDataColumn { title = "Active Status", data = "IsActiveIcon" });
             
                lstcolumn.Add(new WorkflowActionDataColumn { title = "Edit", data = "EditBtn" });
              //  lstcolumn.Add(new WorkflowActionDataColumn { title = "Delete", data = "DeleteBtn" });

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
        /// Gets WorkflowActionModel
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public WorkflowActionModel GetWorkflowActionModel(string EncryptedId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {
                WorkflowActionModel responseModel = new WorkflowActionModel();
                MAS_UMG_WorkFlowActions dbWorkFlowAction = new MAS_UMG_WorkFlowActions();

                encryptedParameters = EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int ActionId = Convert.ToInt32(decryptedParameters["ActionId"].ToString().Trim());
                dbWorkFlowAction = dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.Where(x => x.ActionID == ActionId).FirstOrDefault();

                responseModel.Discription = dbWorkFlowAction.Description;
                responseModel.DiscriptionR = dbWorkFlowAction.DescriptionR;
                responseModel.StatusMessage = dbWorkFlowAction.StatusMessage;
                responseModel.StatusMessageR = dbWorkFlowAction.StatusMessageR;
                responseModel.isActive = dbWorkFlowAction.IsActive;
                responseModel.ServiceId = (short)dbWorkFlowAction.ServiceID;

                responseModel.ServiceList = GetServiceList();
                return responseModel;
            }
            catch (Exception )
            {

                throw ;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }

        }

        /// <summary>
        /// Updates WorkflowAction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        public bool UpdateWorkflowAction(WorkflowActionModel model)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            try
            {

                MAS_UMG_WorkFlowActions objNewWorkflowAction = new MAS_UMG_WorkFlowActions();

                encryptedParameters = model.EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int ActionId = Convert.ToInt32(decryptedParameters["ActionId"].ToString().Trim());


                objNewWorkflowAction = dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.Where(x => x.ActionID == ActionId).FirstOrDefault();
               // objNewWorkflowAction.ActionID = (byte)model.ActionId;
                objNewWorkflowAction.Description = model.Discription;
                objNewWorkflowAction.DescriptionR = model.DiscriptionR;
                objNewWorkflowAction.StatusMessage = model.StatusMessage;
                objNewWorkflowAction.StatusMessageR = model.StatusMessageR;
                objNewWorkflowAction.IsActive = (bool)model.isActive;
                objNewWorkflowAction.ServiceID = (short)model.ServiceId;
                dbKaveriOnlineContext.SaveChanges();
                return true;
            }
            catch (Exception )
            {

                throw ;
            }
            finally
            {
                if (dbKaveriOnlineContext != null)
                    dbKaveriOnlineContext.Dispose();
            }

        }

        /// <summary>
        /// Deletes WorkflowAction
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>

        public bool DeleteWorkflowAction(string EncryptedId)
        {
            KaveriEntities dbKaveriOnlineContext = new KaveriEntities();
            MAS_UMG_WorkFlowActions objdbWorkflowActionModel = new MAS_UMG_WorkFlowActions();
            try
            {
                encryptedParameters = EncryptedId.Split('/');

                if (encryptedParameters.Length != 3)
                {
                    throw new SecurityException("URL is Tampered.Please Try Again Or Contact to Help Desk");
                }

                decryptedParameters = URLEncrypt.DecryptParameters(new String[] { encryptedParameters[0], encryptedParameters[1], encryptedParameters[2] });

                int ActionId = Convert.ToInt32(decryptedParameters["ActionId"].ToString().Trim());
                objdbWorkflowActionModel = dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.Where(x => x.ActionID == ActionId).FirstOrDefault();
                if (objdbWorkflowActionModel != null)
                {
                    MAS_UMG_WorkFlowConfigurations objWorkFlowConfiguration = new MAS_UMG_WorkFlowConfigurations();
                    objWorkFlowConfiguration = dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Where(x => x.ActionID == ActionId).FirstOrDefault();
                    if(objWorkFlowConfiguration!=null)  dbKaveriOnlineContext.MAS_UMG_WorkFlowConfigurations.Remove(objWorkFlowConfiguration);
                    dbKaveriOnlineContext.MAS_UMG_WorkFlowActions.Remove(objdbWorkflowActionModel);

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

    }
}