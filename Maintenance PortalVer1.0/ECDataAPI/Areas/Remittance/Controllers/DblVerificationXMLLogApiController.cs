#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DblVerificationXMLLogApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.DblVerificationXMLLog;
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
    public class DblVerificationXMLLogApiController : ApiController
    {
        IDblVerificationXMLLog balObject = null;

        /// <summary>
        /// DblVeriXMLLogDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DblVerificationXMLLogApiController/DblVeriXMLLogDetails")]
        [EventApiAuditLogFilter(Description = "Dbl Veri XML Log Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DblVeriXMLLogDetails(DblVeriReqXMLLogModel model)
        {
            try
            {
                balObject = new DblVerificationXMLLogBAL();
                DblVeriXMLLogWrapperModel challanMatrixWrapperModel = balObject.DblVeriXMLLogDetails(model);
                return Ok(challanMatrixWrapperModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DownloadDblVeriXML
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DblVerificationXMLLogApiController/DownloadDblVeriXML")]
        [EventApiAuditLogFilter(Description = "Download Dbl Veri XML", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DownloadDblVeriXML(DblVeriReqXMLLogModel reqModel)
        {
            try
            {
                balObject = new DblVerificationXMLLogBAL();
                FileDownloadModel model = new FileDownloadModel();

                model = balObject.DownloadDblVeriXML(reqModel);

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
