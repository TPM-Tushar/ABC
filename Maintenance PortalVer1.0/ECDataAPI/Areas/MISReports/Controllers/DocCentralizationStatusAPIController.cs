#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DocCentralizationStatusAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.DocCentralizationStatus;
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
    public class DocCentralizationStatusAPIController : ApiController
    {
        IDocCentralizationStatus balObject = null;

        /// <summary>
        /// returns HighValueProperties Request Model
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/DocCentralizationStatusAPIController/DocCentralizationStatusView")]
        [EventApiAuditLogFilter(Description = "Document Centralization Status View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DocCentralizationStatusView(int OfficeID)
        {
            try
            {
                balObject = new DocCentralizationStatusBAL();
                DocCentrStatusReqModel ReqModel = new DocCentrStatusReqModel();
                ReqModel = balObject.DocCentralizationStatusView(OfficeID);
                return Ok(ReqModel);
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
        [Route("api/DocCentralizationStatusAPIController/LoadDocCentralizationStatusDataTable")]
        [EventApiAuditLogFilter(Description = "Get High Value Property Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadDocCentralizationStatusDataTable(DocCentrStatusReqModel ReqModel)
        {
            try
            {
                balObject = new DocCentralizationStatusBAL();
                DocCentrStatusResModel ResModel = new DocCentrStatusResModel();
                ResModel = balObject.LoadDocCentralizationStatusDataTable(ReqModel);

                return Ok(ResModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
