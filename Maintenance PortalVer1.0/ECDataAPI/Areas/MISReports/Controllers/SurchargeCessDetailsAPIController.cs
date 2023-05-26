#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   SurchargeCessDetailsAPIController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.SurchargeCessDetails;
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
    public class SurchargeCessDetailsAPIController : ApiController
    {
        /// <summary>
        /// returns Surcharge Cess Details View model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns Surcharge Cess Details View model</returns>
        [HttpGet]
        [Route("api/SurchargeCessDetailsAPIController/SurchargeCessDetailsView")]
        [EventApiAuditLogFilter(Description = "Surcharge Cess Details View")]
        public IHttpActionResult SurchargeCessDetailsView(int OfficeID)
        {
            try
            {
                return Ok(new SurchargeCessDetailsBAL().SurchargeCessDetailsView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Get Surcharge Cess Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Get Surcharge Cess Details list</returns>
        [HttpPost]
        [Route("api/SurchargeCessDetailsAPIController/SurchargeCessDetails")]
        [EventApiAuditLogFilter(Description = "Get Surcharge Cess Details")]
        public IHttpActionResult SurchargeCessDetails(SurchargeCessDetailsModel model)
        {
            try
            {
                return Ok(new SurchargeCessDetailsBAL().SurchargeCessDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns Tolat Count of Surcharge Cess Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns Tolat Count of Surcharge Cess Details</returns>
        [HttpPost]
        [Route("api/SurchargeCessDetailsAPIController/SurchargeCessDetailsTotalCount")]
        public IHttpActionResult SurchargeCessDetailsTotalCount(SurchargeCessDetailsModel model)
        {
            try
            {
                return Ok(new SurchargeCessDetailsBAL().SurchargeCessDetailsTotalCount(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns SroName
        /// </summary>
        /// <param name="SROfficeID"></param>
        /// <returns>Returns SroName</returns>
        [HttpGet]
        [Route("api/SurchargeCessDetailsAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                return Ok(new SurchargeCessDetailsBAL().GetSroName(SROfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}