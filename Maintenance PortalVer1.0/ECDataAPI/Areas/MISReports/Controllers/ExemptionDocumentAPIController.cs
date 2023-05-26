#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ExemptionDocumentAPIController.cs
    * Author Name       :   Shubham Bhagat 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.ExemptionDocument;
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
    public class ExemptionDocumentAPIController : ApiController
    {
        [HttpGet]
        [Route("api/ExemptionDocumentAPIController/ExemptionDocumentView")]
        [EventApiAuditLogFilter(Description = "Exempted Document View")]
        public IHttpActionResult ExemptionDocumentView(int OfficeID)
        {
            try
            {
                return Ok(new ExemptionDocumentBAL().ExemptionDocumentView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }


        //[HttpPost]
        //[Route("api/ExemptionDocumentAPIController/ExemptionDocumentSummary")]
        //[EventApiAuditLogFilter(Description = "Exempted Document Summary")]
        //public IHttpActionResult ExemptionDocumentSummary(ExemptionDocumentModel model)
        //{
        //    try
        //    {
        //        return Ok(new ExemptionDocumentBAL().ExemptionDocumentSummary(model));
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}


        [HttpPost]
        [Route("api/ExemptionDocumentAPIController/ExemptionDocumentDetail")]
        [EventApiAuditLogFilter(Description = "Exempted Document Detail")]
        public IHttpActionResult ExemptionDocumentDetail(ExemptionDocumentModel model)
        {
            try
            {
                return Ok(new ExemptionDocumentBAL().ExemptionDocumentDetail(model));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/ExemptionDocumentAPIController/ExemptionDocumentTotalCount")]
        public IHttpActionResult ExemptionDocumentTotalCount(ExemptionDocumentModel model)
        {
            try
            {
                return Ok(new ExemptionDocumentBAL().ExemptionDocumentTotalCount(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpGet]
        [Route("api/ExemptionDocumentAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                return Ok(new ExemptionDocumentBAL().GetSroName(SROfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
