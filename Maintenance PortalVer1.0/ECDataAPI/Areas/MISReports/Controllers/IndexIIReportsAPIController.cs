#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IndexIIReportsAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion


using CustomModels.Models.MISReports.IndexIIReports;
using CustomModels.Models.MISReports.RegistrationSummary;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class IndexIIReportsAPIController : ApiController
    {
        IIndexIIReports balObject = null;

        /// <summary>
        /// returns IndexIIReports Response Model
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/IndexIIReportsAPIController/IndexIIReportsView")]
        [EventApiAuditLogFilter(Description = "Index II Reports View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult IndexIIReportsView(int OfficeID)
        {
            try
            {
                balObject = new IndexIIReportsBAL();
                IndexIIReportsResponseModel responseModel = new IndexIIReportsResponseModel();

                responseModel = balObject.IndexIIReportsView(OfficeID);

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
        [Route("api/IndexIIReportsAPIController/GetIndexIIReportsDetails")]
        [EventApiAuditLogFilter(Description = "Get Index II Reports Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetIndexIIReportsDetails(IndexIIReportsResponseModel model)
        {
            try
            {
                balObject = new IndexIIReportsBAL();
                IndexIIReportsResponseModel responseModel = new IndexIIReportsResponseModel();
                List<IndexIIReportsDetailsModel> IndexIIReportsDetailsList = new List<IndexIIReportsDetailsModel>();
                IndexIIReportsDetailsList = balObject.GetIndexIIReportsDetails(model);

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
        [Route("api/IndexIIReportsAPIController/GetIndexIIReportsDetailsTotalCount")]
        public IHttpActionResult GetIndexIIReportsDetailsTotalCount(IndexIIReportsResponseModel model)
        {
            try
            {
                balObject = new IndexIIReportsBAL();
                int totalCount = balObject.GetIndexIIReportsDetailsTotalCount(model);
                return Ok(totalCount);
            }
            catch (Exception )
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
        [Route("api/IndexIIReportsAPIController/GetSroName")]
        public IHttpActionResult GetSroName(int SROfficeID)
        {
            try
            {
                balObject = new IndexIIReportsBAL();
                string SroName= balObject.GetSroName(SROfficeID);
                return Ok(SroName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/IndexIIReportsAPIController/DisplayScannedFile")]
        public IHttpActionResult DisplayScannedFile(RegistrationSummaryREQModel model)
        {
            try
            {
                balObject = new IndexIIReportsBAL();
                return Ok(balObject.DisplayScannedFile(model));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
