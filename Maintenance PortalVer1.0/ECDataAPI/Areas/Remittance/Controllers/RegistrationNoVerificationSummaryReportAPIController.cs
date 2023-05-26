#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationSummaryReportAPIController.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :  API Controller for Registration No Verification Summary Report .

*/
#endregion

using CustomModels.Models.Remittance.RegistrationNoVerificationSummaryReport;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class RegistrationNoVerificationSummaryReportAPIController : ApiController
    {
        IRegistrationNoVerificationSummaryReport balObj = null;
        [HttpGet]
        [Route("api/RegistrationNoVerificationSummaryReportAPIController/RegistrationNoVerificationSummaryReportView")]
        // GET: Remittance/RegistrationNoVerificationSummaryReportAPI
        public IHttpActionResult RegistrationNoVerificationSummaryReportView()
        {
            try
            {
                RegistrationNoVerificationSummaryReportModel responseModel = new RegistrationNoVerificationSummaryReportModel();
                balObj = new RegistrationNoVerificationSummaryReportBAL();
                responseModel = balObj.RegistrationNoVerificationSummaryReportView();

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
        [HttpPost]
        [Route("api/RegistrationNoVerificationSummaryReportAPIController/GetSummaryReportDetails")]
        public IHttpActionResult GetSummaryReportDetails(RegistrationNoVerificationSummaryReportModel registrationNoVerificationSummaryReportModel)
        {
            try
            {
                balObj = new RegistrationNoVerificationSummaryReportBAL();
                RegistrationNoVerificationSummaryResultModel registrationNoVerificationSummaryResultModel = new RegistrationNoVerificationSummaryResultModel();
                registrationNoVerificationSummaryResultModel = balObj.GetSummaryReportDetails(registrationNoVerificationSummaryReportModel);
                return Ok(registrationNoVerificationSummaryResultModel);

            }
            catch(Exception ex)
            {
                throw;
            }

        }
    }
}