using CustomModels.Models.MISReports.ARegister;
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
    public class ARegisterAPIController : ApiController
    {
        IARegister balObject = new ARegisterBAL();
        [HttpPost]
        [Route("api/ARegisterAPIController/ARegisterView")]
        public IHttpActionResult ARegisterView(ARegisterViewModel aRegisterViewModel)
        {
            try
            {
                return Ok(balObject.ARegisterView(aRegisterViewModel));
            }
            catch (Exception)
            {

                throw;
            }
          
        }

        [HttpPost]
        [Route("api/ARegisterAPIController/GenerateReport")]
        public IHttpActionResult GenerateReport(ARegisterViewModel aRegisterViewModel)
        {
            try
            {
                return Ok(balObject.GenerateReport(aRegisterViewModel));
            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpGet]
        [Route("api/ARegisterAPIController/ViewARegisterReport")]
        public IHttpActionResult ViewARegisterReport(string FileID)
        {
            try
            {
                return Ok(balObject.ViewARegisterReport(FileID));
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
