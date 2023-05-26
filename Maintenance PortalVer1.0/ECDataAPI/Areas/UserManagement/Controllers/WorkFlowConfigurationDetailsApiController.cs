
#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   UserRegistrationApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Api Controller for WorkFlowActionDetails .
*/
#endregion


using ECDataAPI.Areas.UserManagement.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CustomModels.Models.UserManagement;
using ECDataAPI.Areas.UserManagement.BAL;
using ECDataAPI.Common;
using ECDataAPI.Filters;

namespace ECDataAPI.Areas.UserManagement.Controllers
{
    public class WorkFlowConfigurationDetailsApiController : ApiController
    {
        IWorkFlowConfiguration objWorkFlowConfigurationBAL = new WorkFlowConfigurationBAL();
        /// <summary>
        /// Creates NewWorkFlowConfiguration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WorkFlowConfigurationDetailsApiController/CreateNewWorkFlowConfiguration")]
        [EventApiAuditLogFilter(Description = "Work Flow Configuration Created", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult CreateNewWorkFlowConfiguration(WorkFlowConfigurationModel model)
        {
            return Ok(objWorkFlowConfigurationBAL.CreateNewWorkFlowConfiguration(model));
        }

        /// <summary>
        /// Deletes WorkFlowConfiguration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WorkFlowConfigurationDetailsApiController/DeleteWorkFlowConfiguration")]
        [EventApiAuditLogFilter(Description = "Work Flow Configuration Deleted", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DeleteWorkFlowConfiguration(WorkFlowConfigurationModel model)
        {
            return Ok(objWorkFlowConfigurationBAL.DeleteWorkFlowConfiguration(model.EncryptedId));
        }

        /// <summary>
        /// Gets WorkFlowConfigurationDetails
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WorkFlowConfigurationDetailsApiController/GetWorkFlowConfigurationDetails")]
        public IHttpActionResult GetWorkFlowConfigurationDetails()
        {
            return Ok(objWorkFlowConfigurationBAL.GetWorkFlowConfigurationDetails());
        }

        /// <summary>
        /// Gets WorkFlowConfigurationModel
        /// </summary>
        /// <param name="EncryptedId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WorkFlowConfigurationDetailsApiController/GetWorkFlowConfigurationModel")]
        public IHttpActionResult GetWorkFlowConfigurationModel(string EncryptedId)
        {
            return Ok(objWorkFlowConfigurationBAL.GetWorkFlowConfigurationModel(EncryptedId));
        }

        /// <summary>
        /// Load WorkFlowConfigurationGridData
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/WorkFlowConfigurationDetailsApiController/LoadWorkFlowConfigurationGridData")]
        public IHttpActionResult LoadWorkFlowConfigurationGridData()
        {
            return Ok(objWorkFlowConfigurationBAL.LoadWorkFlowConfigurationGridData());
        }

        /// <summary>
        /// Updates WorkFlowConfiguration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/WorkFlowConfigurationDetailsApiController/UpdateWorkFlowConfiguration")]
        [EventApiAuditLogFilter(Description = "Work Flow Configuration Updated", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult UpdateWorkFlowConfiguration(WorkFlowConfigurationModel model)
        {
            return Ok(objWorkFlowConfigurationBAL.UpdateWorkFlowConfiguration(model));
        }
    }
}
