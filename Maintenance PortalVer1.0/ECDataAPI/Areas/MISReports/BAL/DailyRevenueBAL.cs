#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DailyRevenueBAL.cs
    * Author Name       :   Raman Kalegaonkar
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   BAL layer for MIS Reports module.
*/
#endregion


using CustomModels.Models.MISReports.DailyRevenue;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class DailyRevenueBAL : IDailyRevenue
    {
        IDailyRevenue misReportsDal = new DailyRevenueDAL();


        /// <summary>
        /// returns Daily Revenue Report
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public DailyRevenueReportReqModel DailyRevenueReport(int OfficeID)
        {
            return misReportsDal.DailyRevenueReport(OfficeID);

        }

        /// <summary>
        /// returns DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DailyRevenueReportDetailModel> DailyRevenueReportDetails(DailyRevenueReportReqModel model)
        {
            return misReportsDal.DailyRevenueReportDetails(model);

        }


        /// <summary>
        /// returns TotalCount of DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DailyRevenueReportDetailsTotalCount(DailyRevenueReportReqModel model)
        {
            return misReportsDal.DailyRevenueReportDetailsTotalCount(model);

        }


        /// <summary>
        /// returns DayWise DailyRevenueReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DailyRevenueReportDetailModel> DailyRevenueReportDetailsDayWise(DailyRevenueReportReqModel model)
        {
            return misReportsDal.DailyRevenueReportDetailsDayWise(model);

        }

        /// <summary>
        /// returns DayWise DailyRevenueReportDetailsTotalCount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int DailyRevenueReportDetailsTotalCountDayWise(DailyRevenueReportReqModel model)
        {
            return misReportsDal.DailyRevenueReportDetailsTotalCountDayWise(model);

        }
        /// <summary>
        /// returns DailyRevenueReportDetails
        /// </summary>
        /// <param name="DailyRevenueReportReqModel"></param>
        /// <returns>List<DailyRevenueReportDetailModel></returns>
        public List<DailyRevenueReportDetailModel> LoadDailyRevenueReportTblMonthWise(DailyRevenueReportReqModel model)
        {
            return misReportsDal.LoadDailyRevenueReportTblMonthWise(model);

        }


        /// <summary>
        /// returns DayWise DailyRevenueReportDetailsTotalCount
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<DailyRevenueReportDetailModel> LoadDailyRevenueReportTblDocWise(DailyRevenueReportReqModel model)
        {
            return misReportsDal.LoadDailyRevenueReportTblDocWise(model);

        }
        /// <summary>
        /// returns DailyRevenueReportDetails
        /// </summary>
        /// <param name="DailyRevenueReportReqModel"></param>
        /// <returns>List<DailyRevenueReportDetailModel></returns>
        public int DailyRevenueReportDetailsTotalCountDocWise(DailyRevenueReportReqModel model)
        {
            return misReportsDal.DailyRevenueReportDetailsTotalCountDocWise(model);

        }
    }
}
