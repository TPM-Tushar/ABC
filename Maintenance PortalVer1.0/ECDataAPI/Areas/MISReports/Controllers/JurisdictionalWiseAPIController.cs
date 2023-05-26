#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   JurisdictionalWiseAPIController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.JurisdictionalWise;
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
    public class JurisdictionalWiseAPIController : ApiController
    {
        /// <summary>
        /// returns Jurisdictional Wise View model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns>returns Jurisdictional Wise View model</returns>
        [HttpGet]
        [Route("api/JurisdictionalWiseAPIController/JurisdictionalWiseView")]
        [EventApiAuditLogFilter(Description = "Jurisdictional Wise View")]
        public IHttpActionResult JurisdictionalWiseView(int OfficeID)
        {
            try
            {
                return Ok(new JurisdictionalWiseBAL().JurisdictionalWiseView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        ///// <summary>
        ///// Jurisdictional Wise Summary
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns>Jurisdictional Wise Summary Model</returns>        
        //[HttpPost]
        //[Route("api/JurisdictionalWiseAPIController/JurisdictionalWiseSummary")]
        //[EventApiAuditLogFilter(Description = "Jurisdictional Wise Summary")]
        //public IHttpActionResult JurisdictionalWiseSummary(JurisdictionalWiseModel model)
        //{
        //    try
        //    {
        //        return Ok(new JurisdictionalWiseBAL().JurisdictionalWiseSummary(model));
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

        /// <summary>
        /// Jurisdictional Wise Detail
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Jurisdictional Wise Detail Model List</returns>
        [HttpPost]
        [Route("api/JurisdictionalWiseAPIController/JurisdictionalWiseDetail")]
        [EventApiAuditLogFilter(Description = "Jurisdictional Wise Detail")]
        public IHttpActionResult JurisdictionalWiseDetail(JurisdictionalWiseModel model)
        {
            try
            {
                return Ok(new JurisdictionalWiseBAL().JurisdictionalWiseDetail(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Jurisdictional Wise Total Count
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Jurisdictional Wise Total Count</returns>
        [HttpPost]
        [Route("api/JurisdictionalWiseAPIController/JurisdictionalWiseTotalCount")]
        public IHttpActionResult JurisdictionalWiseTotalCount(JurisdictionalWiseModel model)
        {
            try
            {
                return Ok(new JurisdictionalWiseBAL().JurisdictionalWiseTotalCount(model));
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
        [Route("api/JurisdictionalWiseAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                return Ok(new JurisdictionalWiseBAL().GetSroName(SROfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}