#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   BhoomiFileUploadReportAPIController.cs
    * Author Name       :   Raman Kalegaonkar 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.BhoomiFileUploadReport;
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
    public class BhoomiFileUploadReportAPIController : ApiController
    {
        IBhoomiFileUploadReport balObject = null;

        [HttpGet]
        [Route("api/BhoomiFileUploadReportAPIController/BhoomiFileUploadReportView")]
        [EventApiAuditLogFilter(Description = "EC Daily Receipt Details View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult BhoomiFileUploadReportView(int OfficeID)
        {
            try
            {
                balObject = new BhoomiFileUploadReportBAL();
                BhoomiFileUploadRptViewModel ViewModel = new BhoomiFileUploadRptViewModel();

                ViewModel = balObject.BhoomiFileUploadReportView(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/BhoomiFileUploadReportAPIController/LoadBhoomiFileUploadReportDataTable")]
        [EventApiAuditLogFilter(Description = "Get Bhoomi File Upload Report DataTable", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadBhoomiFileUploadReportDataTable(BhoomiFileUploadRptViewModel model)
        {
            try
            {
                balObject = new BhoomiFileUploadReportBAL();
                BhoomiFileUploadRptResModel responseModel = new BhoomiFileUploadRptResModel();

                responseModel = balObject.LoadBhoomiFileUploadReportDataTable(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
