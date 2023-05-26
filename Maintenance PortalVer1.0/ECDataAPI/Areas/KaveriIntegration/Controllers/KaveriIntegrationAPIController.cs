/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: KaveriIntegrationAPIController.cs
 * Author : Shubham Bhagat
 * Creation Date :14 Oct 2019
 * Desc : Service 
 * ECR No : 
*/

using CustomModels.Models.KaveriIntegration;
using ECDataAPI.Areas.KaveriIntegration.BAL;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.KaveriIntegration.Controllers
{
    public class KaveriIntegrationAPIController : ApiController
    {
        /// <summary>
        /// Kaveri Integration View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns Kaveri Integration Model</returns>
        [HttpGet]
        [Route("api/KaveriIntegrationAPIController/KaveriIntegrationView")]
        public IHttpActionResult KaveriIntegrationView(int OfficeID)
        {
            try
            {
                return Ok(new KaveriIntegrationBAL().KaveriIntegrationView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Load Kaveri Integration Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns Kaveri Integration Wrapper Model</returns>
        [HttpPost]
        [Route("api/KaveriIntegrationAPIController/LoadKaveriIntegrationTable")]
        [EventApiAuditLogFilter(Description = "Load Kaveri Integration Table")]
        public IHttpActionResult LoadKaveriIntegrationTable(KaveriIntegrationModel model)
        {            
                try
                {
                    return Ok(new KaveriIntegrationBAL().LoadKaveriIntegrationTable(model));
                }
                catch (Exception)
                {
                    throw;
                }           
        }

        /// <summary>
        /// Other Table Details
        /// </summary>
        /// <param name="model"></param>
        /// <returns>returns KI Details Wrapper Model</returns>
        [HttpPost]
        [Route("api/KaveriIntegrationAPIController/OtherTableDetails")]
        [EventApiAuditLogFilter(Description = "Other Table Details")]
        public IHttpActionResult OtherTableDetails(KaveriIntegrationModel model)
        {
            try
            {
                return Ok(new KaveriIntegrationBAL().OtherTableDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
