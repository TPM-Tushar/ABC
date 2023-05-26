using CustomModels.Models.MISReports.RegistrationSummary;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class RegistrationSummaryAPIController : ApiController
    {
        IRegistrationSummary balObject = null;

        /// <summary>
        /// returns IndexIIReports Response Model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RegistrationSummaryAPIController/RegistrationSummaryView")]
        //[EventApiAuditLogFilter(Description = "Index II Reports View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult RegistrationSummaryView(int OfficeID)
        {
            try
            {
                balObject = new RegistrationSummaryBAL();
                RegistrationSummaryRESModel responseModel = new RegistrationSummaryRESModel();

                responseModel = balObject.RegistrationSummaryView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns List of IndexIIReportsDetailsModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RegistrationSummaryAPIController/GetRegistrationSummaryDetails")]
       // [EventApiAuditLogFilter(Description = "Get Index II Reports Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetRegistrationSummaryDetails(RegistrationSummaryRESModel model)
        {
            try
            {
                balObject = new RegistrationSummaryBAL();
                RegistrationSummaryRESModel responseModel = new RegistrationSummaryRESModel();
                List<RegistrationSummaryDetailModel> IndexIIReportsDetailsList = new List<RegistrationSummaryDetailModel>();
                IndexIIReportsDetailsList = balObject.GetRegistrationSummaryDetails(model);

                return Ok(IndexIIReportsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns TolatCount of GetIndexIIReportsDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RegistrationSummaryAPIController/GetRegistrationSummaryDetailsTotalCount")]
        public IHttpActionResult GetRegistrationSummaryDetailsTotalCount(RegistrationSummaryRESModel model)
        {
            try
            {
                balObject = new RegistrationSummaryBAL();
                int totalCount = balObject.GetRegistrationSummaryDetailsTotalCount(model);
                return Ok(totalCount);
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
        /// <returns></returns>
        [HttpGet]
        [Route("api/RegistrationSummaryAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                balObject = new RegistrationSummaryBAL();
                string SroName = balObject.GetSroName(SROfficeID);
                return Ok(SroName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/RegistrationSummaryAPIController/DisplayScannedFile")]
        public IHttpActionResult DisplayScannedFile(RegistrationSummaryREQModel model)
        {
            try
            {
                balObject = new RegistrationSummaryBAL();
                return Ok(balObject.DisplayScannedFile(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
