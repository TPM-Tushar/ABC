#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   WorkFlowConfigurationBAL.cs
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
    public class WorkFlowConfigurationBAL : IWorkFlowConfiguration
    {
        IWorkFlowConfiguration objWorkFlowConfigurationDAL = new WorkFlowConfigurationDAL();
        /// <summary>
        /// Creates NewWorkFlowConfiguration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CreateNewWorkFlowConfiguration(WorkFlowConfigurationModel model)
        {
            return objWorkFlowConfigurationDAL.CreateNewWorkFlowConfiguration(model);
        }

        /// <summary>
        /// Deletes WorkFlowConfiguration
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public bool DeleteWorkFlowConfiguration(string EncryptedId)
        {
            return objWorkFlowConfigurationDAL.DeleteWorkFlowConfiguration(EncryptedId);
        }

        /// <summary>
        /// Gets WorkFlowConfigurationDetails
        /// </summary>
        /// <returns></returns>
        public WorkFlowConfigurationModel GetWorkFlowConfigurationDetails()
        {
            return objWorkFlowConfigurationDAL.GetWorkFlowConfigurationDetails();
        }

        /// <summary>
        /// Gets WorkFlowConfigurationModel
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public WorkFlowConfigurationModel GetWorkFlowConfigurationModel(string EncryptedId)
        {
            return objWorkFlowConfigurationDAL.GetWorkFlowConfigurationModel(EncryptedId);
        }

        /// <summary>
        /// Loads WorkFlowConfigurationGridData
        /// </summary>
        /// <returns></returns>
        public WorkFlowConfigurationGridWrapperModel LoadWorkFlowConfigurationGridData()
        {
            return objWorkFlowConfigurationDAL.LoadWorkFlowConfigurationGridData();
        }

        /// <summary>
        /// Updates WorkFlowConfiguration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public WorkFlowConfigurationResponseModel UpdateWorkFlowConfiguration(WorkFlowConfigurationModel model)
        {
            return objWorkFlowConfigurationDAL.UpdateWorkFlowConfiguration(model);
        }
    }
}