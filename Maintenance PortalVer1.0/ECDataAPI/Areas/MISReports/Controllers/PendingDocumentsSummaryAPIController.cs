#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   PendingDocumentsSummaryAPIController.cs
    * Author Name       :   Raman Kalegaonkar 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion


using CustomModels.Models.MISReports.PendingDocumentsSummary;
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
    public class PendingDocumentsSummaryAPIController : ApiController
    {
        IPendingDocsSummary balObject = null;

        /// <summary>
        /// Returns PendingDocSummaryViewModel Required to show Daily Receipt Details View
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/PendingDocumentsSummaryAPIController/PendingDocumentsSummaryView")]
        [EventApiAuditLogFilter(Description = "Pending Documents Summary View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult PendingDocumentsSummaryView(int OfficeID)
        {
            try
            {
                balObject = new PendingDocumentsSummaryBAL();
                PendingDocSummaryViewModel ViewModel = new PendingDocSummaryViewModel();

                ViewModel = balObject.PendingDocumentsSummaryView(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns PendingDocsSummaryResModel Required to show Pending Document Summary Data Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PendingDocumentsSummaryAPIController/LoadPendingDocumentSummaryDataTable")]
        [EventApiAuditLogFilter(Description = "Get Pending Documents Summary", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadPendingDocumentSummaryDataTable(PendingDocSummaryViewModel model)
        {
            try
            {
                balObject = new PendingDocumentsSummaryBAL();
                PendingDocsSummaryResModel responseModel = new PendingDocsSummaryResModel();

                responseModel = balObject.LoadPendingDocumentSummaryDataTable(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// Returns PendingDocsSummaryDetailsResModel Required to show Load Pending Document Details Data Table
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/PendingDocumentsSummaryAPIController/LoadPendingDocumentDetailsDataTable")]
        [EventApiAuditLogFilter(Description = "Get Pending Documents Summary", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadPendingDocumentDetailsDataTable(PendingDocSummaryViewModel model)
        {
            try
            {
                balObject = new PendingDocumentsSummaryBAL();
                PendingDocsSummaryDetailsResModel responseModel = new PendingDocsSummaryDetailsResModel();

                responseModel = balObject.LoadPendingDocumentDetailsDataTable(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
