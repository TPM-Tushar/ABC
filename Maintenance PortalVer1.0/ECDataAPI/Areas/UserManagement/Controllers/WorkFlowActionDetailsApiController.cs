
#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   WorkFlowActionDetailsApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Api Controller for WorkFlowActionDetails .
*/
#endregion


using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.BAL;
using ECDataAPI.Areas.UserManagement.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.UserManagement.Controllers
{
    public class WorkFlowActionDetailsApiController : ApiController
    {
        IWorkflowAction objWorkFlowActionBAL = null;
          
        /// <summary>
        /// returns WorkFlowActionDetails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WorkFlowActionDetailsApiController/GetWorkflowActionDetails")]
        public IHttpActionResult GetWorkflowActionDetails()
        {
            objWorkFlowActionBAL = new WorkflowActionBAL();
            return Ok(objWorkFlowActionBAL.GetWorkflowActionDetails());
        }

        /// <summary>
        /// Creates NewWork flow Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WorkFlowActionDetailsApiController/CreateNewWorkflowAction")]
        [EventApiAuditLogFilter(Description = "New Work Flow Action Created", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult CreateNewWorkflowAction(WorkflowActionModel model) {
            objWorkFlowActionBAL = new WorkflowActionBAL();
            return Ok(objWorkFlowActionBAL.CreateNewWorkflowAction(model));
        }

        /// <summary>
        /// Loads WorkflowActionGridData
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WorkFlowActionDetailsApiController/LoadWorkflowActionGridData")]
        public IHttpActionResult LoadWorkflowActionGridData() {
            objWorkFlowActionBAL = new WorkflowActionBAL();
            return Ok(objWorkFlowActionBAL.LoadWorkflowActionGridData());
        }

        /// <summary>
        /// Gets WorkflowActionModel
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WorkFlowActionDetailsApiController/GetWorkflowActionModel")]
        public IHttpActionResult GetWorkflowActionModel(string EncryptedId) {
            objWorkFlowActionBAL = new WorkflowActionBAL();
            return Ok(objWorkFlowActionBAL.GetWorkflowActionModel(EncryptedId));
        }

        /// <summary>
        /// Updates WorkflowAction
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WorkFlowActionDetailsApiController/UpdateWorkflowAction")]
        [EventApiAuditLogFilter(Description = "Work Flow Action Updated", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UpdateWorkflowAction(WorkflowActionModel model) {
            objWorkFlowActionBAL = new WorkflowActionBAL();
            return Ok(objWorkFlowActionBAL.UpdateWorkflowAction(model));
        }

        /// <summary>
        /// Delete Work flow Action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WorkFlowActionDetailsApiController/DeleteWorkflowAction")]
        [EventApiAuditLogFilter(Description = "Work Flow Action Deleted", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DeleteWorkflowAction(WorkflowActionModel model) {
            objWorkFlowActionBAL = new WorkflowActionBAL();
            return Ok(objWorkFlowActionBAL.DeleteWorkflowAction(model.EncryptedId));
        }
    }
}
