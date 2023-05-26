using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using CustomModels.Models.MISReports.ReScanningDetails;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class ReScanningDetailsAPIController : ApiController
    {
        IReScanningDetails balObject = null;


        [HttpGet]
        [Route("api/ReScanningDetailsAPIController/ReScanningDetails")]
        public IHttpActionResult ReScanningDetails(int OfficeID)
        {
            try
            {
                balObject = new ReScanningDetailsBAL();
                ReScanningDetailsViewModel ViewModel = new ReScanningDetailsViewModel();

                ViewModel = balObject.ReScanningDetails(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ReScanningDetailsAPIController/GetReScanningTotalCount")]
        public IHttpActionResult GetReScanningTotalCount(ReScanningDetailsViewModel model)
        {
            try
            {
                balObject = new ReScanningDetailsBAL();
                int totalcount = balObject.GetReScanningTotalCount(model);
                return Ok(totalcount);
            }
            catch (Exception)
            {
                throw;
            }
        }


        [HttpPost]
        [Route("api/ReScanningDetailsAPIController/GetReScanningTableData")]
        public IHttpActionResult GetReScanningTableData(ReScanningDetailsViewModel model)
        {
            try
            {
                balObject = new ReScanningDetailsBAL();
                ReScanningDetailsResModel responseModel = new ReScanningDetailsResModel();
                responseModel = balObject.GetReScanningTableData(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
