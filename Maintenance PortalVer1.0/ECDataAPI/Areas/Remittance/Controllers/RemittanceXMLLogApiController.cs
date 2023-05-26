#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   RemittanceXMLLogApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.RemittanceXMLLog;
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
    public class RemittanceXMLLogApiController : ApiController
    {
        IRemittanceXMLLog balObject = null;

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/RemittanceXMLLogApiController/GetOfficeList")]
        [EventApiAuditLogFilter(Description = "Get Office List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetOfficeList(String OfficeType)
        {
            try
            {
                balObject = new RemittanceXMLLogBAL();
                RemittXMLLogModel model = new RemittXMLLogModel();

                model = balObject.GetOfficeList(OfficeType);

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// RemittanceXMLLogDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RemittanceXMLLogApiController/RemittanceXMLLogDetails")]
        [EventApiAuditLogFilter(Description = "Remittance XML Log Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult RemittanceXMLLogDetails(REMRequestXMLLogModel model)
        {
            try
            {
                balObject = new RemittanceXMLLogBAL();
                RemittXMLLogModel remittanceXMLLogModel = balObject.RemittanceXMLLogDetails(model);
                return Ok(remittanceXMLLogModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DownloadREMLogXml
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/RemittanceXMLLogApiController/DownloadREMLogXml")]
        [EventApiAuditLogFilter(Description = "Download REM Log Xml", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DownloadREMLogXml(REMRequestXMLLogModel reqModel)
        {
            try
            {
                balObject = new RemittanceXMLLogBAL();
                FileDownloadModel model = new FileDownloadModel();

                model = balObject.DownloadREMLogXml(reqModel);

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
