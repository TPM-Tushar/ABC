#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ChallanMatrixXMLLogApiController.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for Remittance module.
*/
#endregion

using CustomModels.Models.Remittance.ChallanMatrixXMLLog;
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
    public class ChallanMatrixXMLLogApiController : ApiController
    {
        IChallanMatrixXMLLog balObject = null;

        /// <summary>
        /// GetOfficeList
        /// </summary>
        /// <param name="OfficeType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/ChallanMatrixXMLLogApiController/GetOfficeList")]
        [EventApiAuditLogFilter(Description = "Get Office List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetOfficeList(String OfficeType)
        {
            try
            {
                balObject = new ChallanMatrixXMLLogBAL();
                ChallanMatrixWrapperModel model = new ChallanMatrixWrapperModel();

                model = balObject.GetOfficeList(OfficeType);

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// ChallanMatrixDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ChallanMatrixXMLLogApiController/ChallanMatrixDetails")]
        [EventApiAuditLogFilter(Description = "Challan Matrix Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult ChallanMatrixDetails(ChallanMatrixLogRequestModel model)
        {
            try
            {
                balObject = new ChallanMatrixXMLLogBAL();
                ChallanMatrixWrapperModel challanMatrixWrapperModel = balObject.ChallanMatrixDetails(model);
                return Ok(challanMatrixWrapperModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// DownloadChallanMatrixXML
        /// </summary>
        /// <param name="reqModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/ChallanMatrixXMLLogApiController/DownloadChallanMatrixXML")]
        [EventApiAuditLogFilter(Description = "Download Challan Matrix XML", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DownloadChallanMatrixXML(ChallanMatrixLogRequestModel reqModel)
        {
            try
            {
                balObject = new ChallanMatrixXMLLogBAL();
                FileDownloadModel model = new FileDownloadModel();

                model = balObject.DownloadChallanMatrixXML(reqModel);

                return Ok(model);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
