#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ServicePackStatusAPIController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.ServicePackStatus;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class ServicePackStatusAPIController : ApiController
    {
        IServicePackStatus balObject = null;

        /// <summary>
        /// Service Pack Status View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ServicePackStatusAPIController/ServicePackStatusView")]
        [EventApiAuditLogFilter(Description = "Service Pack Status View")]
        public IHttpActionResult ServicePackStatusView(int OfficeID)
        {
            try
            {
                balObject = new ServicePackStatusBAL();
                ServicePackStatusModel responseModel = new ServicePackStatusModel();
                responseModel = balObject.ServicePackStatusView(OfficeID);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Service Pack Status Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns Service Pack Status Detail List</returns>
        [HttpPost]
        [Route("api/ServicePackStatusAPIController/ServicePackStatusDetails")]
        [EventApiAuditLogFilter(Description = "Service Pack Status Details")]
        public IHttpActionResult ServicePackStatusDetails(ServicePackStatusModel model)
        {
            try
            {
                balObject = new ServicePackStatusBAL();
                ServicePackStatusModel responsemodel = new ServicePackStatusModel();
                List<ServicePackStatusDetails> ServicePackStatusDetailList = new List<ServicePackStatusDetails>();
                ServicePackStatusDetailList = balObject.ServicePackStatusDetails(model);
                return Ok(ServicePackStatusDetailList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Service Pack Status Total Count
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ServicePackStatusAPIController/ServicePackStatusTotalCount")]
        public IHttpActionResult ServicePackStatusTotalCount(ServicePackStatusModel model)
        {
            try
            {
                balObject = new ServicePackStatusBAL();
                int totalcount = balObject.ServicePackStatusTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
