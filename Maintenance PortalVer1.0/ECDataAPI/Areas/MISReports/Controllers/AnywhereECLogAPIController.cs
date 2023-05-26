#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   AnywhereECLogAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.AnywhereECLog;
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
    public class AnywhereECLogAPIController : ApiController
    {

        IAnywhereECLog balObject = null;

        [HttpGet]
        [Route("api/AnywhereECLogAPIController/AnywhereECLogView")]
        [EventApiAuditLogFilter(Description = "Anywhere EC Log View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult AnywhereECLogView(int OfficeID)
        {
            try
            {
                balObject = new AnywhereECLogBAL();
                AnywhereECLogView ViewModel = new AnywhereECLogView();

                ViewModel = balObject.AnywhereECLogView(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/AnywhereECLogAPIController/GetAnywhereECLogDetails")]
        [EventApiAuditLogFilter(Description = "Loads Anywhere EC Log Datatable", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult GetAnywhereECLogDetails(AnywhereECLogView model)
        {
            try
            {
                balObject = new AnywhereECLogBAL();
                AnywhereECLogResModel responseModel = new AnywhereECLogResModel();

                responseModel = balObject.GetAnywhereECLogDetails(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/AnywhereECLogAPIController/GetAnywhereECLogTotalCount")]
        public IHttpActionResult GetAnywhereECLogTotalCount(AnywhereECLogView model)
        {
            try
            {
                balObject = new AnywhereECLogBAL();
                AnywhereECLogResModel responseModel = new AnywhereECLogResModel();

                int TotalRecords = balObject.GetAnywhereECLogTotalCount(model);

                return Ok(TotalRecords);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
