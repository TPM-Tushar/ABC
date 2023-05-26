using CustomModels.Models.MISReports.DocumentScanAndDeliveryReport;
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
    public class DocumentScanAndDeliveryAPIController : ApiController
    {
        [HttpGet]
        //DocumentScanAndDeliveryAPIController/DocumentScanAndDeliveryView
        [Route("api/DocumentScanAndDeliveryAPIController/DocumentScanAndDeliveryView")]
        [EventApiAuditLogFilter(Description = "Document Scan And Delivery View")]
        public IHttpActionResult DocumentScanAndDeliveryView(int OfficeID)
        {
            try
            {
                return Ok(new DocumentScanAndDeliveryBAL().DocumentScanAndDeliveryView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DocumentScanAndDeliveryAPIController/DocumentScanAndDeliveryDetails")]
        [EventApiAuditLogFilter(Description = "Document Scan And Delivery Details")]
        public IHttpActionResult DocumentScanAndDeliveryDetails(DocumentScanAndDeliveryREQModel model)
        {
            try
            {
                return Ok(new DocumentScanAndDeliveryBAL().DocumentScanAndDeliveryDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DocumentScanAndDeliveryAPIController/DocumentScanAndDeliveryCount")]
        public IHttpActionResult DocumentScanAndDeliveryCount(DocumentScanAndDeliveryREQModel model)
        {
            try
            {
                return Ok(new DocumentScanAndDeliveryBAL().DocumentScanAndDeliveryCount(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        //Added by RamanK on 25-02-2020
        [HttpPost]
        [Route("api/DocumentScanAndDeliveryAPIController/DocumentScanAndDeliveryDetailsForSRO")]
        [EventApiAuditLogFilter(Description = "Document Scan And Delivery Details")]
        public IHttpActionResult DocumentScanAndDeliveryDetailsForSRO(DocumentScanAndDeliveryREQModel model)
        {
            try
            {
                return Ok(new DocumentScanAndDeliveryBAL().DocumentScanAndDeliveryDetailsForSRO(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
