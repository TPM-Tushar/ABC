#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationScanningDetailsAPIController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Registration Scanning Details Report .

*/
#endregion

using CustomModels.Models.Remittance.RegistrationScanningDetails;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class RegistrationScanningDetailsAPIController : ApiController
    {
        IRegistrationScanningDetails bal = null;
        [HttpGet]
        [Route("api/RegistrationScanningDetailsAPIController/RegistrationScanningDetailsView")]
        // GET: Remittance/RegistrationScanningDetailsAPI
        public IHttpActionResult RegistrationScanningDetailsView(int DocumentTypeId)
        {
            try
            {
                RegistrationScanningDetailsModel responseModel = new RegistrationScanningDetailsModel();
                bal = new RegistrationScanningDetailsBAL();
                responseModel = bal.RegistrationScanningDetailsView(DocumentTypeId);

                return Ok(responseModel);
            }
            catch (Exception)
            {

                throw;
            }
        }
        [HttpPost,Route("api/RegistrationScanningDetailsAPIController/GetRegistrationScanningDetails")]
        public IHttpActionResult GetRegistrationScanningDetails(RegistrationScanningDetailsModel registrationScanningDetailsModel)
        {
            try
            {
                RegistrationScanningDetailsResultModel registrationScanningDetailsResultModel = new RegistrationScanningDetailsResultModel();
                bal = new RegistrationScanningDetailsBAL();
                registrationScanningDetailsResultModel = bal.GetRegistrationScanningDetails(registrationScanningDetailsModel);
                return Ok(registrationScanningDetailsResultModel);

            }
            catch
            {
                throw;
            }
        }
    }
}