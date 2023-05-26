#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationDetailsAPIController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Registration No Verification Details.

*/
#endregion

using CustomModels.Models.Remittance.RegistrationNoVerificationDetails;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class RegistrationNoVerificationDetailsAPIController : ApiController
    {
        IRegistrationNoVerificationDetails balObj = null;
        [HttpGet]
        [Route("api/RegistrationNoVerificationDetailsAPIController/RegistrationNoVerificationDetailsView")]
        public IHttpActionResult RegistrationNoVerificationDetailsView(int OfficeID)
        {
            try
            {
                RegistrationNoVerificationDetailsModel responseModel = new RegistrationNoVerificationDetailsModel();
                balObj = new RegistrationNoVerificationDetailsBAL();
                responseModel = balObj.RegistrationNoVerificationDetailsView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/RegistrationNoVerificationDetailsAPIController/GetRegistrationNoVerificationDetails")]
        public IHttpActionResult GetRegistrationNoVerificationDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            try
            {
                balObj = new RegistrationNoVerificationDetailsBAL();
                List <RegistrationNoVerificationDetailsTableModel> registrationNoVerificationDetailsTableModels = new List<RegistrationNoVerificationDetailsTableModel>();
                registrationNoVerificationDetailsTableModels = balObj.GetRegistrationNoVerificationDetails(registrationNoVerificationDetailsModel);
                return Ok(registrationNoVerificationDetailsTableModels);
            }
            catch (Exception)
            {
                throw;
            }
        }



        //Added by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 
        [HttpGet]
        [Route("api/RegistrationNoVerificationDetailsAPIController/RefreshPropertyAreaDetailsDetails")]
        public IHttpActionResult RefreshPropertyAreaDetailsDetails(int SROfficeID)
        {
            try
            {
                balObj = new RegistrationNoVerificationDetailsBAL();
                string result = balObj.RefreshPropertyAreaDetailsDetails(SROfficeID);
                return Ok(result);
            }
            catch (Exception)
            {
                throw;
            }
        }
        //Ended by ShivamB on 13-02-2023 for adding IsRefreshPropertyAreaDetailsEnable button to delete PropertyAreaDetails table rows SROwise 

        [HttpPost]
        [Route("api/RegistrationNoVerificationDetailsAPIController/GetDocRegNoCLBatchDetails")]
        public IHttpActionResult GetDocRegNoCLBatchDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            try
            {
                RegistrationNoVerificationDetailsTableModel responseModel = new RegistrationNoVerificationDetailsTableModel();
                balObj = new RegistrationNoVerificationDetailsBAL();
                responseModel = balObj.GetDocRegNoCLBatchDetails(registrationNoVerificationDetailsModel);

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        //Added By Tushar on 7 Nov 2022
        [HttpPost]
        [Route("api/RegistrationNoVerificationDetailsAPIController/GetScannedFileDetails")]
        public IHttpActionResult GetScannedFileDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            try
            {
                RegistrationNoVerificationDetailsTableModel responseModel = new RegistrationNoVerificationDetailsTableModel();
                balObj = new RegistrationNoVerificationDetailsBAL();
                responseModel = balObj.GetScannedFileDetails(registrationNoVerificationDetailsModel);

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/RegistrationNoVerificationDetailsAPIController/GetFinalRegistrationNumberDetails")]
        public IHttpActionResult GetFinalRegistrationNumberDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            try
            {
                RegistrationNoVerificationDetailsTableModel responseModel = new RegistrationNoVerificationDetailsTableModel();
                balObj = new RegistrationNoVerificationDetailsBAL();
                responseModel = balObj.GetFinalRegistrationNumberDetails(registrationNoVerificationDetailsModel);

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //End By Tushar on 7 Nov 2022
        //Added By Tushar 8 Feb
        [HttpPost]
        [Route("api/RegistrationNoVerificationDetailsAPIController/GetDateDetails")]
        public IHttpActionResult GetDateDetails(RegistrationNoVerificationDetailsModel registrationNoVerificationDetailsModel)
        {
            try
            {
                RegistrationNoVerificationDetailsTableModel responseModel = new RegistrationNoVerificationDetailsTableModel();
                balObj = new RegistrationNoVerificationDetailsBAL();
                responseModel = balObj.GetDateDetails(registrationNoVerificationDetailsModel);

                return Ok(responseModel);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        //End 8 feb 
    }
}
