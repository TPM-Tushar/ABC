using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomModels.Models.UserManagement;
namespace ECDataAPI.Areas.UserManagement.Interface
{
    interface IWorkflowAction
    {
        WorkflowActionModel GetWorkflowActionDetails();
        bool CreateNewWorkflowAction(WorkflowActionModel model);
        WorkflowActionGridWrapperModel LoadWorkflowActionGridData();
        WorkflowActionModel GetWorkflowActionModel(string EncryptedId);
        bool UpdateWorkflowAction(WorkflowActionModel model);
        bool DeleteWorkflowAction(string EncryptedId);
    }
}
