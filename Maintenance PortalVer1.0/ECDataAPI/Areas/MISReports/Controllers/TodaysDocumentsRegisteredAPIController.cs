#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   TodaysDocumentsRegisteredAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion

using CustomModels.Models.MISReports.TodaysDocumentsRegistered;
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
    public class TodaysDocumentsRegisteredAPIController : ApiController
    {
        ITodaysDocumentsRegistered balObject = null;

        /// <summary>
        /// Returns TodaysDocumentsRegisteredReqModel required to show TodaysDocumentsRegisteredView
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/TodaysDocumentsRegisteredAPIController/TodaysDocumentsRegisteredView")]
        [EventApiAuditLogFilter(Description = "TodaysDocumentsRegisteredView", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult TodaysDocumentsRegisteredView(int OfficeID)
        {
            try
            {
                balObject = new TodaysDocumentsRegisteredBAL();
                TodaysDocumentsRegisteredReqModel responseModel = new TodaysDocumentsRegisteredReqModel();

                responseModel = balObject.TodaysDocumentsRegisteredView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// Returns TodaysTotalDocsRegDetailsTable to show GridView
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/TodaysDocumentsRegisteredAPIController/GetTodaysTotalDocumentsRegisteredDetails")]
        [EventApiAuditLogFilter(Description = "Get Todays Total Documents Registered Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult GetTodaysTotalDocumentsRegisteredDetails(TodaysDocumentsRegisteredReqModel model)
        {
            try
            {
                balObject = new TodaysDocumentsRegisteredBAL();
                TodaysTotalDocsRegDetailsTable todaysTotalDocsRegDetails = new TodaysTotalDocsRegDetailsTable();
                todaysTotalDocsRegDetails = balObject.GetTodaysTotalDocumentsRegisteredDetails(model);

                return Ok(todaysTotalDocsRegDetails);
            }
            catch (Exception e)
            {
                throw;
            }
        }

    }
}
