#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   ECDailyReceiptReportAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using System;
using CustomModels.Models.MISReports.ECDailyReceiptReport;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ECDataAPI.Areas.MISReports.Interface;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Filters;
using ECDataAPI.Common;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class ECDailyReceiptReportAPIController : ApiController
    {
        IECDailyReceiptReport balObject = null;

        [HttpGet]
        [Route("api/ECDailyReceiptReportAPIController/ECDailyReceiptReportView")]
        [EventApiAuditLogFilter(Description = "EC Daily Receipt Details View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]

        public IHttpActionResult ECDailyReceiptReportView(int OfficeID)
        {
            try
            {
                balObject = new ECDailyReceiptReportBAL();
                ECDailyReceiptRptView ViewModel = new ECDailyReceiptRptView();

                ViewModel = balObject.ECDailyReceiptDetails(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ECDailyReceiptReportAPIController/GetECDailyReceiptDetails")]
        [EventApiAuditLogFilter(Description = "Get EC Daily Receipt Details DataTable", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetECDailyReceiptDetails(ECDailyReceiptRptView model)
        {
            try
            {
                balObject = new ECDailyReceiptReportBAL();
                ECDailyReceiptRptResModel responseModel = new ECDailyReceiptRptResModel();

                responseModel = balObject.GetECDailyReceiptDetails(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ECDailyReceiptReportAPIController/GetECDailyReceiptsTotalCount")]
        public IHttpActionResult GetECDailyReceiptsTotalCount(ECDailyReceiptRptView model)
        {
            try
            {
                balObject = new ECDailyReceiptReportBAL();
                ECDailyReceiptRptResModel responseModel = new ECDailyReceiptRptResModel();

                int TotalRecords = balObject.GetECDailyReceiptsTotalCount(model);

                return Ok(TotalRecords);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
