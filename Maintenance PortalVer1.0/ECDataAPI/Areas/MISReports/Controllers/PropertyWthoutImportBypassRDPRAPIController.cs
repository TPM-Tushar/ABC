/*File Header
 * Project Id: 
 * Project Name: Kaveri Maintainance Portal
 * File Name: KaveriIntegrationAPIController.cs
 * Author : Shubham Bhagat
 * Creation Date :14 Oct 2019
 * Desc : Service 
 * ECR No : 
*/

using CustomModels.Models.MISReports.PropertyWthoutImportBypassRDPR;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class PropertyWthoutImportBypassRDPRAPIController : ApiController
    {
        /// <summary>
        /// Kaveri Integration View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns Kaveri Integration Model</returns>
        [HttpGet]
        [Route("api/PropertyWthoutImportBypassRDPRAPIController/ReportView")]
        public IHttpActionResult ReportView(int OfficeID)
        {
            try
            {
                return Ok(new PropertyWthoutImportBypassRDPRBAL().ReportView(OfficeID));
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
        [Route("api/PropertyWthoutImportBypassRDPRAPIController/LoadReportTable")]
        [EventApiAuditLogFilter(Description = "Load Kaveri Integration Table")]
        public IHttpActionResult LoadReportTable(ReportModel model)
        {            
                try
                {
                    return Ok(new PropertyWthoutImportBypassRDPRBAL().LoadReportTable(model));
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
        [Route("api/PropertyWthoutImportBypassRDPRAPIController/OtherTableDetailsBypassRDPR")]
        [EventApiAuditLogFilter(Description = "Other Table Details")]
        public IHttpActionResult OtherTableDetailsBypassRDPR(ReportModel model)
        {
            try
            {
                return Ok(new PropertyWthoutImportBypassRDPRBAL().OtherTableDetailsBypassRDPR(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
