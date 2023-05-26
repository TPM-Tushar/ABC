#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   CDWrittenReportAPIController.cs
    * Author Name       :   Raman Kalegaonkar 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.CDWrittenReport;
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
    public class CDWrittenReportAPIController : ApiController
    {
        ICDWrittenReport balObject = null;

        [HttpGet]
        [Route("api/CDWrittenReportAPIController/CDWrittenReportView")]
        [EventApiAuditLogFilter(Description = "CD Written Report View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult CDWrittenReportView(int OfficeID)
        {
            try
            {
                balObject = new CDWrittenReportBAL();
                CDWrittenReportViewModel ViewModel = new CDWrittenReportViewModel();

                ViewModel = balObject.CDWrittenReportView(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("api/CDWrittenReportAPIController/LoadCDWrittenReportDataTable")]
        [EventApiAuditLogFilter(Description = "CD Written Report Data Table", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadCDWrittenReportDataTable(CDWrittenReportViewModel model)
        {
            try
            {
                balObject = new CDWrittenReportBAL();
                CDWrittenReportResModel responseModel = new CDWrittenReportResModel();
                responseModel = balObject.LoadCDWrittenReportDataTable(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
