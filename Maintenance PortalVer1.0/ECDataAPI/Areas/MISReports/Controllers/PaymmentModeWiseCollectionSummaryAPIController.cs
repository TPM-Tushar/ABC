using CustomModels.Models.MISReports.PaymmentModeWiseCollectionSummary;
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
    public class PaymmentModeWiseCollectionSummaryAPIController : ApiController
    {
        IPaymentModeWiseCollectionSummary balObject = null;

        [HttpGet]
        [EventApiAuditLogFilter(Description = "Payment Mode Wise Collection", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        [Route("api/PaymmentModeWiseCollectionSummaryAPIController/PaymentModeWiseCollectionSummaryView")]

        public IHttpActionResult PaymentModeWiseCollectionSummaryView(int OfficeID)
        {
            try
            {
                balObject = new PaymmentModeWiseCollectionSummaryBAL();
                PaymmentModeWiseCollectionSummaryView ViewModel = new PaymmentModeWiseCollectionSummaryView();

                ViewModel = balObject.PaymentModeWiseCollectionSummaryView(OfficeID);

                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }



        [HttpPost]
        [Route("api/PaymmentModeWiseCollectionSummaryAPIController/GetPaymentModeWiseRPTTableData")]
        [EventApiAuditLogFilter(Description = "Get Payment Mode Wise RPT Table Data", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetPaymentModeWiseRPTTableData(PaymmentModeWiseCollectionSummaryView model)
        {
            try
            {
                balObject = new PaymmentModeWiseCollectionSummaryBAL();
                PaymentModeWiseCollectionSummaryResModel responseModel = new PaymentModeWiseCollectionSummaryResModel();
                responseModel = balObject.GetPaymentModeWiseRPTTableData(model);
                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
