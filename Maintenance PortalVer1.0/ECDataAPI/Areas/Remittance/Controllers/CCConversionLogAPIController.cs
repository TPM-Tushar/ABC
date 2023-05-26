#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   CCConversionLogAPIController.cs
    * Author Name       :   Madhusoodan Bisen
    * Creation Date     :   15-09-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for CC Conversion Log Report in General Diagnostics Module
*/
#endregion

using CustomModels.Models.Remittance.CCConversionLog;
using ECDataAPI.Areas.Remittance.BAL;
using ECDataAPI.Areas.Remittance.Interface;
using System;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using ECDataAPI.Filters;
using ECDataAPI.Common;

namespace ECDataAPI.Areas.Remittance.Controllers
{
    public class CCConversionLogAPIController : ApiController
    {
        ICCConversionLog balObj = null;

        /// <summary>
        /// API Controller to load CC Conversion Log View
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/CCConversionLogAPIController/CCConversionLogView")]
        [EventApiAuditLogFilter(Description = "CCConversionLogView", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult CCConversionLogView()
        {
            try
            {
                balObj = new CCConversionLogBAL();
                CCConversionLogWrapperModel cCConversionLogWrapperModel = balObj.CCConversionLogView();

                return Ok(cCConversionLogWrapperModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// API Controller to get CC Conversion Logs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/CCConversionLogAPIController/CCConversionLogDetails")]
        [EventApiAuditLogFilter(Description = "CCConversionLogDetails", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult CCConversionLogDetails(CCConversionLogReqModel model)
        {
            try
            {
                balObj = new CCConversionLogBAL();
                CCConversionLogWrapperModel cCConversionLogWrapperModel = balObj.CCConversionLogDetails(model);
                
                return Ok(cCConversionLogWrapperModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// API Controller to get total count of CC Conversion Logs
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/CCConversionLogAPIController/GetCCConversionLogDetailsTotalCount")]
        [EventApiAuditLogFilter(Description = "GetCCConversionLogDetailsTotalCount", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetCCConversionLogDetailsTotalCount(CCConversionLogReqModel model)
        {
            try
            {
                balObj = new CCConversionLogBAL();
                int totalCount = balObj.GetCCConversionLogDetailsTotalCount(model);

                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}