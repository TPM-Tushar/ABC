using CustomModels.Models.Remittance.FirmCentralization;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class FirmCentralizationAPIController : ApiController
    {
        IFirmCentralization balObj = null;
        [HttpGet]
        [Route("api/FirmCentralizationAPIController/FirmCentralizationView")]
        // GET: Remittance/FirmCentralizationAPI
        public IHttpActionResult FirmCentralizationView()
        {
            try
            {
                FirmCentralizationModel responseModel = new FirmCentralizationModel();
                balObj = new FirmCentralizationBAL();
                responseModel = balObj.FirmCentralizationView();

                return Ok(responseModel);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/FirmCentralizationAPIController/GetFirmCentralizationDetails")]
        public IHttpActionResult GetFirmCentralizationDetails(FirmCentralizationModel firmCentralizationModel)
        {
            try
            {
                balObj = new FirmCentralizationBAL();
                FirmCentralizationResultModel firmCentralizationResultModel = new FirmCentralizationResultModel();
                firmCentralizationResultModel = balObj.GetFirmCentralizationDetails(firmCentralizationModel);
                return Ok(firmCentralizationResultModel);
            }
            catch (Exception)
            {

                throw;
            }

        }
        [HttpPost]
        [Route("api/FirmCentralizationAPIController/GetFirmCentralizationLocalDetails")]
        public IHttpActionResult GetFirmCentralizationLocalDetails(FirmCentralizationModel firmCentralizationModel)
        {
            try
            {
                balObj = new FirmCentralizationBAL();
                FirmCentralizationResultModel firmCentralizationResultModel = new FirmCentralizationResultModel();
                firmCentralizationResultModel = balObj.GetFirmCentralizationLocalDetails(firmCentralizationModel);
                return Ok(firmCentralizationResultModel);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/FirmCentralizationAPIController/GetFirmCentralizationCentralDetails")]
        public IHttpActionResult GetFirmCentralizationCentralDetails(FirmCentralizationModel firmCentralizationModel)
        {
            try
            {
                balObj = new FirmCentralizationBAL();
                FirmCentralizationResultModel firmCentralizationResultModel = new FirmCentralizationResultModel();
                firmCentralizationResultModel = balObj.GetFirmCentralizationCentralDetails(firmCentralizationModel);
                return Ok(firmCentralizationResultModel);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}