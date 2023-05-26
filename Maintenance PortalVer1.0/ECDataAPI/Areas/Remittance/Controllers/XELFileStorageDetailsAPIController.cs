using CustomModels.Models.Remittance.XELFileStorageDetails;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class XELFileStorageDetailsAPIController : ApiController
    {
        [HttpGet]
        [Route("api/XELFileStorageDetailsAPIController/XELFileStorageView")]
        [EventApiAuditLogFilter(Description = "XEL File Storage View")]

        public IHttpActionResult XELFileStorageView(int OfficeID)
        {
            try
            {
                return Ok(new XELFileStorageDetailsBAL().XELFileStorageView(OfficeID));
            }
            catch (Exception) { throw; }
        }


        [HttpPost]
        [Route("api/XELFileStorageDetailsAPIController/XELFileOfficeList")]
        [EventApiAuditLogFilter(Description = "XEL File Office List")]

        public IHttpActionResult XELFileOfficeList(XELFileStorageViewModel reqModel)
        {
            try
            {
                return Ok(new XELFileStorageDetailsBAL().XELFileOfficeList(reqModel));
            }
            catch (Exception) { throw; }
        }


        [HttpPost]
        [Route("api/XELFileStorageDetailsAPIController/XELFileListOfficeWise")]
        [EventApiAuditLogFilter(Description = "XEL File List Office Wise")]

        public IHttpActionResult XELFileListOfficeWise(XELFileStorageViewModel reqModel)
        {
            try
            {
                return Ok(new XELFileStorageDetailsBAL().XELFileListOfficeWise(reqModel));
            }
            catch (Exception) { throw; }
        }

        [HttpPost]
        [Route("api/XELFileStorageDetailsAPIController/RootDirectoryTable")]
        [EventApiAuditLogFilter(Description = "Root Directory Table")]

        public IHttpActionResult RootDirectoryTable(XELFileStorageViewModel reqModel)
        {
            try
            {
                return Ok(new XELFileStorageDetailsBAL().RootDirectoryTable(reqModel));
            }
            catch (Exception) { throw; }
        }

        [HttpPost]
        [Route("api/XELFileStorageDetailsAPIController/XELFileDownloadPathVerify")]
        [EventApiAuditLogFilter(Description = "XEL File Download Path Verify")]

        public IHttpActionResult XELFileDownloadPathVerify(XELFileStorageViewModel reqModel)
        {
            try

            {
                return Ok(new XELFileStorageDetailsBAL().XELFileDownloadPathVerify(reqModel));
            }
            catch (Exception) { throw; }
        }




        [HttpPost]
        [Route("api/XELFileStorageDetailsAPIController/XELFileDownload")]
        [EventApiAuditLogFilter(Description = "XEL File Download")]

        public IHttpActionResult XELFileDownload(XELFileStorageViewModel reqModel)
        {
            try
            {
                return Ok(new XELFileStorageDetailsBAL().XELFileDownload(reqModel));
            }
            catch (Exception) { throw; }
        }
    }
}
