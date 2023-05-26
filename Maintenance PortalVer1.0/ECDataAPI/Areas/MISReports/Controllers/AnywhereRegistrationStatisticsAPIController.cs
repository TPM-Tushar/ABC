#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   AnywhereRegistrationStatisticsAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.AnywhereRegistrationStatistics;
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
    public class AnywhereRegistrationStatisticsAPIController : ApiController
    {
        IAnywhereRegistrationStatistics balObject = null;

        [HttpGet]
        [Route("api/AnywhereRegistrationStatisticsAPIController/AnywhereRegistrationStatisticsView")]
        [EventApiAuditLogFilter(Description = "Anywhere Registration Statistics View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult AnywhereRegistrationStatisticsView(int OfficeID)
        {
            try
            {
                balObject = new AnywhereRegistrationStatisticsBAL();
                AnywhereRegStatViewModel ViewModel = new AnywhereRegStatViewModel();
                ViewModel = balObject.AnywhereRegistrationStatisticsView(OfficeID);
                return Ok(ViewModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/AnywhereRegistrationStatisticsAPIController/GetAnywhereRegStatDetails")]
        [EventApiAuditLogFilter(Description = "Load Anywhere Registration Statistics Datatable", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetAnywhereRegStatDetails(AnywhereRegStatViewModel model)
        {
            try
            {
                balObject = new AnywhereRegistrationStatisticsBAL();
                AnywhereRegStatResModel responseModel = new AnywhereRegStatResModel();

                responseModel = balObject.GetAnywhereRegStatDetails(model);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        //[HttpPost]
        //[Route("api/ECDailyReceiptReportAPIController/GetECDailyReceiptsTotalCount")]
        //public IHttpActionResult GetECDailyReceiptsTotalCount(ECDailyReceiptRptView model)
        //{
        //    try
        //    {
        //        balObject = new ECDailyReceiptReportBAL();
        //        ECDailyReceiptRptResModel responseModel = new ECDailyReceiptRptResModel();

        //        int TotalRecords = balObject.GetECDailyReceiptsTotalCount(model);

        //        return Ok(TotalRecords);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
    }
}
