#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   WorkflowActionBAL.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for User Management module.
*/
#endregion

using ECDataAPI.Areas.UserManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.DAL;

namespace ECDataAPI.Areas.UserManagement.BAL
{

    public class WorkflowActionBAL : IWorkflowAction
    {
        IWorkflowAction dalWorkflowAction = new WorkflowActionDAL();
        /// <summary>
        /// Creates NewWorkflowAction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CreateNewWorkflowAction(WorkflowActionModel model)
        {
            return dalWorkflowAction.CreateNewWorkflowAction(model);
        }

        /// <summary>
        /// Deletes WorkflowAction
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public bool DeleteWorkflowAction(string EncryptedId)
        {
            return dalWorkflowAction.DeleteWorkflowAction(EncryptedId);
        }

        /// <summary>
        /// Gets WorkflowActionDetails
        /// </summary>
        /// <returns></returns>
        public WorkflowActionModel GetWorkflowActionDetails()
        {
            return dalWorkflowAction.GetWorkflowActionDetails();
        }

        /// <summary>
        /// Gets WorkflowActionModel
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public WorkflowActionModel GetWorkflowActionModel(string EncryptedId)
        {
            return dalWorkflowAction.GetWorkflowActionModel(EncryptedId);
        }

        /// <summary>
        /// Loads WorkflowActionGridData
        /// </summary>
        /// <returns></returns>
        public WorkflowActionGridWrapperModel LoadWorkflowActionGridData()
        {
            return dalWorkflowAction.LoadWorkflowActionGridData();
        }

        /// <summary>
        /// Updates WorkflowAction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateWorkflowAction(WorkflowActionModel model)
        {
            return dalWorkflowAction.UpdateWorkflowAction(model);
        }
    }
}