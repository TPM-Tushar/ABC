using CustomModels.Models.MISReports.OtherDepartmentImport;
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
    public class OtherDepartmentImportAPIController : ApiController
    {
        [HttpGet]
        //DocumentScanAndDeliveryAPIController/DocumentScanAndDeliveryView
        [Route("api/OtherDepartmentImportAPIController/OtherDepartmentImportView")]
        [EventApiAuditLogFilter(Description = "Other Department Import View")]
        public IHttpActionResult OtherDepartmentImportView(int OfficeID)
        {
            try
            {
                return Ok(new OtherDepartmentImportBAL().OtherDepartmentImportView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/OtherDepartmentImportAPIController/OtherDepartmentImportDetails")]
        [EventApiAuditLogFilter(Description = "Other Department Import Details")]
        public IHttpActionResult OtherDepartmentImportDetails(OtherDepartmentImportREQModel model)
        {
            try
            {
                return Ok(new OtherDepartmentImportBAL().OtherDepartmentImportDetails(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/OtherDepartmentImportAPIController/OtherDepartmentImportCount")]
        public IHttpActionResult OtherDepartmentImportCount(OtherDepartmentImportREQModel model)
        {
            try
            {
                return Ok(new OtherDepartmentImportBAL().OtherDepartmentImportCount(model));
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/OtherDepartmentImportAPIController/GetXMLContent")]
        public IHttpActionResult GetXMLContent(OtherDepartmentImportREQModel model)
        {
            try
            {
                return Ok(new OtherDepartmentImportBAL().GetXMLContent(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration

        [HttpPost]
        [Route("api/OtherDepartmentImportAPIController/FormIIIDownloadFun")]
        public IHttpActionResult FormIIIDownloadFun(OtherDepartmentImportREQModel model)
        {
            try
            {
                return Ok(new OtherDepartmentImportBAL().FormIIIDownloadFun(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/OtherDepartmentImportAPIController/ViewTransXMLFun")]
        public IHttpActionResult ViewTransXMLFun(OtherDepartmentImportREQModel model)
        {
            try
            {
                return Ok(new OtherDepartmentImportBAL().ViewTransXMLFun(model));
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
