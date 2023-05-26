using CustomModels.Models.UserManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.UserManagement.Interface
{
    interface IWorkFlowConfiguration
    {
         WorkFlowConfigurationModel GetWorkFlowConfigurationDetails();
        bool CreateNewWorkFlowConfiguration(WorkFlowConfigurationModel model);
        WorkFlowConfigurationGridWrapperModel LoadWorkFlowConfigurationGridData();
        WorkFlowConfigurationModel GetWorkFlowConfigurationModel(string EncryptedId);
        WorkFlowConfigurationResponseModel UpdateWorkFlowConfiguration(WorkFlowConfigurationModel model);
        bool DeleteWorkFlowConfiguration(string EncryptedId);
    }
}
