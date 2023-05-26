#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDataPortal
    * File Name         :   DailyRevenueAPIController.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   API Controller for MIS Reports module.
*/
#endregion


using CustomModels.Models.MISReports.DailyRevenue;
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
    public class DailyRevenueAPIController : ApiController
    {
        IDailyRevenue balObject = null;



        /// <summary>
        /// returns Daily Revenue Report
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/DailyRevenueAPIController/DailyRevenueReport")]
        [EventApiAuditLogFilter(Description = "Daily Revenue Report", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DailyRevenueReport(int OfficeID)
        {
            try
            {
                balObject = new DailyRevenueBAL();
                DailyRevenueReportReqModel responseModel = new DailyRevenueReportReqModel();

                responseModel = balObject.DailyRevenueReport(OfficeID);

                return Ok(responseModel);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyRevenueAPIController/DailyRevenueReportDetails")]
        [EventApiAuditLogFilter(Description = "Daily Revenue Report Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DailyRevenueReportDetails(DailyRevenueReportReqModel model)
        {
            try
            {
                balObject = new DailyRevenueBAL();
                List<DailyRevenueReportDetailModel> IndexIIReportsDetailsList = new List<DailyRevenueReportDetailModel>();
                IndexIIReportsDetailsList = balObject.DailyRevenueReportDetails(model);

                return Ok(IndexIIReportsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns DayWise DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyRevenueAPIController/DailyRevenueReportDetailsDayWise")]
        [EventApiAuditLogFilter(Description = "Daily Revenue Report Details Day Wise", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult DailyRevenueReportDetailsDayWise(DailyRevenueReportReqModel model)
        {
            try
            {
                balObject = new DailyRevenueBAL();
                List<DailyRevenueReportDetailModel> IndexIIReportsDetailsList = new List<DailyRevenueReportDetailModel>();
                IndexIIReportsDetailsList = balObject.DailyRevenueReportDetailsDayWise(model);

                return Ok(IndexIIReportsDetailsList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns TotalCount of DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyRevenueAPIController/DailyRevenueReportDetailsTotalCount")]
        public IHttpActionResult DailyRevenueReportDetailsTotalCount(DailyRevenueReportReqModel model)
        {
            try
            {
                balObject = new DailyRevenueBAL();
                int totalCount = balObject.DailyRevenueReportDetailsTotalCount(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns DayWise DailyRevenueReportDetailsTotalCount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyRevenueAPIController/DailyRevenueReportDetailsTotalCountDayWise")]
        public IHttpActionResult DailyRevenueReportDetailsTotalCountDayWise(DailyRevenueReportReqModel model)
        {
            try
            {
                balObject = new DailyRevenueBAL();
                int totalCount = balObject.DailyRevenueReportDetailsTotalCountDayWise(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// returns DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyRevenueAPIController/LoadDailyRevenueReportTblMonthWise")]
        [EventApiAuditLogFilter(Description = "Daily Revenue Month Wise Report Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadDailyRevenueReportTblMonthWise(DailyRevenueReportReqModel model)
        {
            try
            {
                balObject = new DailyRevenueBAL();
                List<DailyRevenueReportDetailModel> DailyRevMonthWiseList = new List<DailyRevenueReportDetailModel>();
                DailyRevMonthWiseList = balObject.LoadDailyRevenueReportTblMonthWise(model);

                return Ok(DailyRevMonthWiseList);
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// returns DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyRevenueAPIController/LoadDailyRevenueReportTblDocWise")]
        [EventApiAuditLogFilter(Description = "Daily Revenue Month Wise Report Details", ServiceID = (short)ApiCommonEnum.enumServiceTypes.UserManager)]
        public IHttpActionResult LoadDailyRevenueReportTblDocWise(DailyRevenueReportReqModel model)
        {
            try
            {
                balObject = new DailyRevenueBAL();
                List<DailyRevenueReportDetailModel> DailyRevMonthWiseList = new List<DailyRevenueReportDetailModel>();
                DailyRevMonthWiseList = balObject.LoadDailyRevenueReportTblDocWise(model);

                return Ok(DailyRevMonthWiseList);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// returns DayWise DailyRevenueReportDetailsTotalCount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/DailyRevenueAPIController/DailyRevenueReportDetailsTotalCountDocWise")]
        public IHttpActionResult DailyRevenueReportDetailsTotalCountDocWise(DailyRevenueReportReqModel model)
        {
            try
            {
                balObject = new DailyRevenueBAL();
                int totalCount = balObject.DailyRevenueReportDetailsTotalCountDocWise(model);
                return Ok(totalCount);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
