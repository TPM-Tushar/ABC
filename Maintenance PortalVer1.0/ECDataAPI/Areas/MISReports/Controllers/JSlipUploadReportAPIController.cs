#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   JSlipUploadReportAPIController.cs
    * Author Name       :   Raman Kalegaonkar 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.JSlipUploadReport;
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
    public class JSlipUploadReportAPIController : ApiController
    {
        IJSlipUploadReport balObject = null;

        [HttpGet]
        [Route("api/JSlipUploadReportAPIController/JSlipUploadReportView")]
        [EventApiAuditLogFilter(Description = "EC Daily Receipt Details View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult JSlipUploadReportView(int OfficeID)
        {
            try
            {
                balObject = new JSlipUploadReportBAL();
                JSlipUploadRptViewModel ViewModel = new JSlipUploadRptViewModel();

                ViewModel = balObject.JSlipUploadReportView(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/JSlipUploadReportAPIController/LoadJSlipUploadReportDataTable")]
        [EventApiAuditLogFilter(Description = "Get Bhoomi File Upload Report DataTable", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadJSlipUploadReportDataTable(JSlipUploadRptViewModel model)
        {
            try
            {
                balObject = new JSlipUploadReportBAL();
                JSlipUploadRptResModel responseModel = new JSlipUploadRptResModel();

                responseModel = balObject.LoadJSlipUploadReportDataTable(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
