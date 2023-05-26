#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   HighValuePropertiesAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.HighValueProperties;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class HighValuePropertiesAPIController : ApiController
    {
        IHighValueProperties balObject = null;


        /// <summary>
        /// returns HighValueProperties Request Model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/HighValuePropertiesAPIController/HighValuePropertiesView")]
        [EventApiAuditLogFilter(Description = "High Value Properties View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult HighValuePropertiesView()
        {
            try
            {
                balObject = new HighValuePropertiesBAL();
                HighValuePropertiesReqModel responseModel = new HighValuePropertiesReqModel();

                responseModel = balObject.HighValuePropertiesView();

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns HighValuePropDetailsResponseModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/HighValuePropertiesAPIController/GetHighValuePropertyDetails")]
        [EventApiAuditLogFilter(Description = "Get High Value Property Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetHighValuePropertyDetails(HighValuePropDetailsReqModel model)
        {
            try
            {
                balObject = new HighValuePropertiesBAL();
                HighValuePropDetailsResModel highValuePropDetails = new HighValuePropDetailsResModel();
                highValuePropDetails = balObject.GetHighValuePropertyDetails(model);

                return Ok(highValuePropDetails);
            }
            catch (Exception )
            {
                throw;
            }
        }
                       
    }
}
