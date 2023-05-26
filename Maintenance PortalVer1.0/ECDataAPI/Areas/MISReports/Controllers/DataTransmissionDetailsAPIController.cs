using CustomModels.Models.MISReports.DataTransmissionDetails;
using ECDataAPI.Areas.MISReports.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class DataTransmissionDetailsAPIController : ApiController
    {
        [HttpGet]
        [Route("api/DataTransmissionDetailsAPIController/DataTransmissionDetailsView")]
        public IHttpActionResult DataTransmissionDetailsView(int OfficeID)
        {
            try
            {
                return Ok(new DataTransmissionDetailsBAL().DataTransmissionDetailsView(OfficeID));
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/DataTransmissionDetailsAPIController/LoadDataTransmissionDetails")]
        public IHttpActionResult LoadDataTransmissionDetails(DataTransReqModel model)
        {
            try
            {
                return Ok(new DataTransmissionDetailsBAL().LoadDataTransmissionDetails(model));

            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/DataTransmissionDetailsAPIController/DataTransmissionDetailsCount")]
        public IHttpActionResult DataTransmissionDetailsCount(DataTransReqModel model)
        {
            try
            {
                return Ok(new DataTransmissionDetailsBAL().DataTransmissionDetailsCount(model));

            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
