#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   REMDaignosticsSummaryApiController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.REMDashboard;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class REMDaignosticsSummaryApiController : ApiController
    {
        IREMDaignosticsSummary balObject = null;

        /// <summary>
        /// GetOfficeListSummary
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/REMDaignosticsSummaryApiController/GetOfficeListSummary")]
        [EventApiAuditLogFilter(Description = "Get Office List Summary", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetOfficeListSummary()
        {
            try
            {
                balObject = new REMDaignosticsSummaryBAL();
                RemittanceOfficeListSummaryModel responseModel = new RemittanceOfficeListSummaryModel();

                responseModel = balObject.GetOfficeListSummary();

                return Ok(responseModel);
            }
            catch (Exception )
            {
                throw;
            }
        }
    }
}
