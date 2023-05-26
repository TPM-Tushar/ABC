/*
 * Author: Rushikesh Chaudhari
 * Class Name: SevaSindhuStatisticsAPIController.cs
 */

using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

using CustomModels.Models.MISReports;
using ECDataAPI.Areas.MISReports.BAL;
using ECDataAPI.Common;
using ECDataAPI.Filters;
using System.Net;
using System.Net.Http;
using CustomModels.Models.MISReports.SevaSidhuStatistics;

namespace ECDataAPI.Areas.MISReports.Controllers
{
   //SevaSindhuStatisticsAPIController
    public class SevaSindhuStatisticsAPIController : ApiController
    {
        ISevaSindhuStatistics balObject = null;

        /// <summary>
        /// returns Seva Sindhu Statistics Report
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/SevaSindhuStatisticsAPIController/SevaSindhuStatisticsReportView")]
        [EventApiAuditLogFilter(Description = "Seva Sindhu Statistics Report View", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SevaSindhuStatisticsReportView(int OfficeID)
        {
            try
            {
                balObject = new SevaSindhuStatisticsBAL();
                SevaSindhuStatisticsReportModel responseModel = new SevaSindhuStatisticsReportModel();

                responseModel = balObject.SevaSindhuStatisticsReportView(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns SevaSindhuStatisticsReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SevaSindhuStatisticsAPIController/SevaSindhuReportDetails")]
        [EventApiAuditLogFilter(Description = "Seva Sindhu Report Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SevaSindhuReportDetails(SevaSindhuStatisticsReportModel model)
        {
            try
            {
                balObject = new SevaSindhuStatisticsBAL();
                List<SevaSindhuStatisticsReportDetailModel> IndexIIReportsDetailsList = new List<SevaSindhuStatisticsReportDetailModel>();
                IndexIIReportsDetailsList = balObject.SevaSindhuReportDetails(model);

                return Ok(IndexIIReportsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns DayWise SevaSindhuReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SevaSindhuStatisticsAPIController/SevaSindhuStatisticsReportDetailsYearWise")]
        [EventApiAuditLogFilter(Description = "Seva Sindhu Statistics Report Details Day Wise", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult SevaSindhuStatisticsReportDetailsYearWise(SevaSindhuStatisticsReportModel model)
        {
            try
            {
                balObject = new SevaSindhuStatisticsBAL();
                List<SevaSindhuStatisticsReportDetailModel> IndexIIReportsDetailsList = new List<SevaSindhuStatisticsReportDetailModel>();
                IndexIIReportsDetailsList = balObject.SevaSindhuStatisticsReportDetailsYearWise(model);

                return Ok(IndexIIReportsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns SevaSindhuReportDetails Month Wise
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/SevaSindhuStatisticsAPIController/LoadSevaSindhuStatisticsReportTblMonthWise")]
        [EventApiAuditLogFilter(Description = "Load Seva Sindhu Statistics Report Tbl Month Wise", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadSevaSindhuStatisticsReportTblMonthWise(SevaSindhuStatisticsReportModel model)
        {
            try
            {
                balObject = new SevaSindhuStatisticsBAL();
                List<SevaSindhuStatisticsReportDetailModel> DailyRevMonthWiseList = new List<SevaSindhuStatisticsReportDetailModel>();
                DailyRevMonthWiseList = balObject.LoadSevaSindhuStatisticsReportTblMonthWise(model);

                return Ok(DailyRevMonthWiseList);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}