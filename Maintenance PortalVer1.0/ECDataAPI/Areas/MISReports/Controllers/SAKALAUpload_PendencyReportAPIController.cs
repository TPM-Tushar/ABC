#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   SAKALAUpload_PendencyReportAPIController.cs
    * Author Name       :   Raman Kalegaonkar 
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.Common;
using CustomModels.Models.MISReports.SAKALAUpload_PendencyReport;
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
    public class SAKALAUpload_PendencyReportAPIController : ApiController
    {
        ISAKALAUpload_PendencyReport balObject = null;

        [HttpGet]
        [Route("api/SAKALAUpload_PendencyReportAPIController/SAKALUploadReportView")]
        [EventApiAuditLogFilter(Description = "SAKALA Upload Report Details View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult SAKALUploadReportView(int OfficeID)
        {
            try
            {
                balObject = new SAKALAUpload_PendencyReportBAL();
                SAKALAUploadRptViewModel ViewModel = new SAKALAUploadRptViewModel();
                ViewModel = balObject.SAKALUploadReportView(OfficeID);
                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/SAKALAUpload_PendencyReportAPIController/LoadSakalaUploadReportDataTable")]
        [EventApiAuditLogFilter(Description = "Get Sakala Upload Report DataTable", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadSakalaUploadReportDataTable(SAKALAUploadRptViewModel model)
        {
            try
            {
                balObject = new SAKALAUpload_PendencyReportBAL();
                SAKALAUploadRptResModel responseModel = new SAKALAUploadRptResModel();

                responseModel = balObject.LoadSakalaUploadReportDataTable(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/SAKALAUpload_PendencyReportAPIController/GetXMLContent")]
        [EventApiAuditLogFilter(Description = "Get XML in string format", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetXMLContent(XMLInputForSAKALAUploadModel InputModel)
        {
            try
            {
                balObject = new SAKALAUpload_PendencyReportBAL();
                XMLResModel responseModel = new XMLResModel();

                responseModel = balObject.GetXMLContent(InputModel);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
