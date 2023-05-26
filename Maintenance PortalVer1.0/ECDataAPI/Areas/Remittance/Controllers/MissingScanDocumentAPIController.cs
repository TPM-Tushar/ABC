using CustomModels.Models.Remittance.MissingSacnDocument;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class MissingScanDocumentAPIController : ApiController
    {
        IMissingScanDocument balObj = null;
        [HttpGet]
        [Route("api/MissingScanDocumentAPIController/MissingScanDocumentView")]
        // GET: Remittance/MissingSacnDocumentAPI
        public IHttpActionResult MissingScanDocumentView()
        {
            try
            {
                MissingScanDocumentModel responseModel = new MissingScanDocumentModel();
                balObj = new MissingScanDocumentBAL();
                responseModel = balObj.MissingSacnDocumentView();

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/MissingScanDocumentAPIController/GetMissingScanDocumentDetails")]
        public IHttpActionResult GetMissingScanDocumentDetails(MissingScanDocumentModel missingSacnDocumentModel)
        {
            try
            {
                balObj = new MissingScanDocumentBAL();
                MissingScanDocumentResModel missingSacnDocumentResModel = new MissingScanDocumentResModel();
                missingSacnDocumentResModel = balObj.GetMissingScanDocumentDetails(missingSacnDocumentModel);
                return Ok(missingSacnDocumentResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}