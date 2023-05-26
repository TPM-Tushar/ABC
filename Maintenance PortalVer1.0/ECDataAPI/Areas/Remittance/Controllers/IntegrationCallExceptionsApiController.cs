#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   IntegrationCallExceptionsApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.IntegrationCallExceptions;
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
    public class IntegrationCallExceptionsApiController : ApiController
    {
        IIntegrationCallExceptions balObject = null;

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/IntegrationCallExceptionsApiController/GetOfficeList")]
        public IHttpActionResult GetOfficeList(String OfficeType)
        {
            try
            {
                balObject = new IntegrationCallExceptionsBAL();
                IntegrationCallExceptionsModel model = new IntegrationCallExceptionsModel();

                model = balObject.GetOfficeList(OfficeType);

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// GetExceptionsDetails
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <param name="OfficeTypeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/IntegrationCallExceptionsApiController/GetExceptionsDetails")]
        [EventApiAuditLogFilter(Description = "Get Exceptions Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetExceptionsDetails(String OfficeType, String OfficeTypeID)
        {
            try
            {
                balObject = new IntegrationCallExceptionsBAL();
                IEnumerable<IntegrationCallExceptionsModel> exceptionsDetailsList = balObject.GetExceptionsDetails(OfficeType, OfficeTypeID);
                return Ok(exceptionsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
