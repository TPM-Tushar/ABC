#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   WorkFlowActionDetailsController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Controller for User Management module.
*/
#endregion

using CustomModels.Models.UserManagement;
using ECDataUI.Common;
using ECDataUI.Filters;
using ECDataUI.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ECDataUI.Areas.UserManagement.Controllers
{
    [KaveriAuthorizationAttribute]
    public class WorkFlowActionDetailsController : Controller
    {
        ServiceCaller caller = new ServiceCaller("WorkFlowActionDetailsApiController");
        string errorMessage = String.Empty;

        /// <summary>
        /// ShowWorkFlowActionView
        /// </summary>
        /// <returns></returns>
        public ActionResult ShowWorkFlowActionView()
        {
         
            return View("ShowWorkFlowActionView");

        }

        /// <summary>
        /// CreateUpdateNewWorkFlowAction
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateUpdateNewWorkFlowAction()
        {
            WorkflowActionModel objWorkFlowActionModelView = new WorkflowActionModel();
            objWorkFlowActionModelView = caller.GetCall<WorkflowActionModel>("GetWorkflowActionDetails", objWorkFlowActionModelView);
            return PartialView("CreateUpdateNewWorkFlowAction", objWorkFlowActionModelView);
        }

        /// <summary>
        /// UpdateWorkflowAction
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        public ActionResult UpdateWorkflowAction(String EncryptedId)
        {
            WorkflowActionModel objWorkFlowActionModelView = new WorkflowActionModel();
            objWorkFlowActionModelView.EncryptedId = EncryptedId;
            WorkflowActionModel objActionModel = caller.GetCall<WorkflowActionModel>("GetWorkflowActionModel", objWorkFlowActionModelView);
            objActionModel.IsForUpdate = true;
            return View("CreateUpdateNewWorkFlowAction", objActionModel);
        }

        /// <summary>
        /// UpdateWorkflowAction
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Update Work Flow Action data")]
        public ActionResult UpdateWorkflowAction(WorkflowActionModel viewModel)
        {
            Boolean Status = caller.PostCall<WorkflowActionModel, bool>("UpdateWorkflowAction", viewModel);
            if (Status == true)
            {
                return Json(new { success = true, message = "Updated successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Updation failed" });
            }

        }

        /// <summary>
        /// DeleteWorkflowAction
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Delete Work Flow Action ")]
        public ActionResult DeleteWorkflowAction(String EncryptedId)
        {
            WorkflowActionModel viewModel = new WorkflowActionModel();
            viewModel.EncryptedId = EncryptedId;
            Boolean Status = caller.PostCall<WorkflowActionModel, bool>("DeleteWorkflowAction", viewModel);
            if (Status == true)
            {
                return Json(new { success = true, message = "Deleted successfully" });
            }
            else
            {
                return Json(new { success = false, message = "Deletion failed failed" });
            }

        }

        /// <summary>
        /// CreateUpdateNewWorkFlowAction
        /// </summary>
        /// <param name="ViewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [EventAuditLogFilter(Description = "Create New Work Flow Action")]
        public ActionResult CreateUpdateNewWorkFlowAction(WorkflowActionModel ViewModel)
        {
            try
            {
                Boolean Status = false;
                String message = String.Empty;
             
                if (ModelState.IsValid)
                {

                    Status = caller.PostCall<WorkflowActionModel, Boolean>("CreateNewWorkflowAction", ViewModel, out errorMessage);
                    if (Status)
                        message = "Data added SuccessFully";
                    else
                        message = "Data Insertion Failed";


                }

                return Json(new { success = true, message = message });

            }
            catch (Exception)
            {

                throw ;
            }


        }

        /// <summary>
        /// LoadWorkflowActionGridData
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult LoadWorkflowActionGridData()
        {
            try
            {
                WorkflowActionGridWrapperModel result = caller.GetCall<WorkflowActionGridWrapperModel>("LoadWorkflowActionGridData", null, out errorMessage);
                var JsonData = Json(new { status = true, data = result.dataArray, columns = result.ColumnArray });
                JsonData.MaxJsonLength = Int32.MaxValue;
                return JsonData;
            }
            catch (Exception)
            {

                throw ;
            }
        }
     
    }
}