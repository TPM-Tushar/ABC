#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Kaveri
    * File Name         :   ECCCSearchStatisticsAPIController.cs
    * Author Name       :   Mayank Wankhede
    * Creation Date     :   14-07-2020
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   EC/CC search statistics API controller
*/
#endregion

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
using CustomModels.Models.MISReports.ECCCSearchStatistics;

namespace ECDataAPI.Areas.MISReports.Controllers
{
    public class ECCCSearchStatisticsAPIController : ApiController
    {
        IECCCSearchStatistics balobj = null;

        [HttpGet]
        [Route("api/ECCCSearchStatisticsAPIController/ECCCSearchStatisticsView")]
        [EventApiAuditLogFilter(Description = "Get ECCCSearchStatisticsView model", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        //Added by mayank date 14-7-20
        //to get view page model populate and return to view page
        /// <summary>
        /// ECCCSearchStatisticsView
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        public IHttpActionResult ECCCSearchStatisticsView(int officeid)
        {
            try
            {
                balobj = new ECCCSearchStatisticsBAL();
                return Ok(balobj.ECCCSearchStatisticsView(officeid));
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        //[Route("api/ActionDetailsApiController/ShowControllerActionData")]
        [Route("api/ECCCSearchStatisticsAPIController/GetSroList")]
        [EventApiAuditLogFilter(Description = "Get sro List", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        //Added by mayank date 14-7-20
        //to get sro list
        /// <summary>
        /// GetSroList
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public IHttpActionResult GetSroList(ECCCSearchStatisticsViewModel viewModel)
        {
            try
            {
                balobj = new ECCCSearchStatisticsBAL();

                return Ok(balobj.GetSroList(viewModel));
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpPost]
        [Route("api/ECCCSearchStatisticsAPIController/GetSummaryApi")]
        [EventApiAuditLogFilter(Description = "Get Summary model", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        //Added by mayank date 14-7-20
        //to get Summary table data
        /// <summary>
        /// GetSummary
        /// </summary>
        /// <param name="eCCCSearchStatisticsViewModel"></param>
        /// <returns></returns>
        public IHttpActionResult GetSummaryApi(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel)
        {
            try
            {
                balobj = new ECCCSearchStatisticsBAL();
                return Ok(balobj.GetSummary(eCCCSearchStatisticsViewModel));
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        [Route("api/ECCCSearchStatisticsAPIController/GetDetailsApi")]
        [EventApiAuditLogFilter(Description = "Get Details model", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        //Added by mayank date 14-7-20
        //to get Detail table data
        /// <summary>
        /// GetDetails
        /// </summary>
        /// <param name="eCCCSearchStatisticsViewModel"></param>
        /// <returns></returns>
        public IHttpActionResult GetDetailsApi(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel)
        {
            try
            {
                balobj = new ECCCSearchStatisticsBAL();
                return Ok(balobj.GetDetails(eCCCSearchStatisticsViewModel));
            }
            catch (Exception)
            {
                throw;

            }
        }


        [HttpPost]
        [Route("api/ECCCSearchStatisticsAPIController/GetSummaryDetailsforExcelApi")]
        [EventApiAuditLogFilter(Description = "Get Summary and Details model", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        //Added by mayank date 15-7-20
        //to get Summary and Detail table data for excel
        /// <summary>
        /// GetDetails
        /// </summary>
        /// <param name="eCCCSearchStatisticsViewModel"></param>
        /// <returns></returns>
        public IHttpActionResult GetSummaryDetailsforExcelApi(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel)
        {
            try
            {
                balobj = new ECCCSearchStatisticsBAL();
                return Ok(balobj.GetSummaryDetailsforExcel(eCCCSearchStatisticsViewModel));
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
