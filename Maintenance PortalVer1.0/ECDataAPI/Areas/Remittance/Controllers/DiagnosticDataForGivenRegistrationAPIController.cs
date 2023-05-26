#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DiagnosticDataForGivenRegistrationAPIController.cs
    * Author Name       :   Pankaj Sakhare
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.DiagnosticDataForGivenRegistration;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class DiagnosticDataForGivenRegistrationAPIController : ApiController
    {
        IDiagnosticDataForGivenRegistration balObject = null;


        /// <summary>
        /// DownloadDiagnosticDataInsertScript
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DiagnosticDataForGivenRegistrationAPIController/DownloadDiagnosticDataInsertScript")]
        [EventApiAuditLogFilter(Description = "Downlaod Insert Script", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DownloadDiagnosticDataInsertScript(DiagnosticDataForRegistrationModel viewModel)
        {
            try
            {
                balObject = new DiagnosticDataForGivenRegistrationBAL();
                return Ok(balObject.DownloadDiagnosticDataInsertScript(viewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}