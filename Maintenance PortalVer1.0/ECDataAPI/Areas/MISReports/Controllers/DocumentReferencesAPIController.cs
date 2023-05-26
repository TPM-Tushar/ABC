using CustomModels.Models.MISReports.DocumentReferences;
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
    public class DocumentReferencesAPIController : ApiController
    {
        [HttpGet]
        //DocumentScanAndDeliveryAPIController/DocumentScanAndDeliveryView
        [Route("api/DocumentReferencesAPIController/DocumentReferencesView")]
        [EventApiAuditLogFilter(Description = "Document References View")]
        public IHttpActionResult DocumentReferencesView(int OfficeID)
        {
            try
            {
                return Ok(new DocumentReferencesBAL().DocumentReferencesView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DocumentReferencesAPIController/DocumentReferencesDetails")]
        [EventApiAuditLogFilter(Description = "Document References Details")]
        public IHttpActionResult DocumentReferencesDetails(DocumentReferencesREQModel model)
        {
            try
            {
                return Ok(new DocumentReferencesBAL().DocumentReferencesDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DocumentReferencesAPIController/DocumentReferencesCount")]
        public IHttpActionResult DocumentReferencesCount(DocumentReferencesREQModel model)
        {
            try
            {
                return Ok(new DocumentReferencesBAL().DocumentReferencesCount(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
