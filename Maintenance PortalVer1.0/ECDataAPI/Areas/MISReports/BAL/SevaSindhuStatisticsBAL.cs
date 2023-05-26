/*
 * Author: Rushikesh Chaudhari
 * Class Name: SevaSindhuStatisticsBAL.cs
 */

using CustomModels.Models.MISReports.SevaSidhuStatistics;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class SevaSindhuStatisticsBAL : ISevaSindhuStatistics
    {
        ISevaSindhuStatistics misReportsDal = new SevaSindhuStatisticsDAL();


        /// <summary>
        /// returns Seva Sindhu Statistics Report
        /// </summary>
        /// <param name="OfficeID"></param>
        /// <returns></returns>
        public SevaSindhuStatisticsReportModel SevaSindhuStatisticsReportView(int OfficeID)
        {
            return misReportsDal.SevaSindhuStatisticsReportView(OfficeID);
        }

        /// <summary>
        /// returns SevaSindhuReportDetails
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<SevaSindhuStatisticsReportDetailModel> SevaSindhuReportDetails(SevaSindhuStatisticsReportModel model)
        {
            return misReportsDal.SevaSindhuReportDetails(model);
        }

        /// <summary>
        /// returns DayWise SevaSindhuStatisticsReport
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public List<SevaSindhuStatisticsReportDetailModel> SevaSindhuStatisticsReportDetailsYearWise(SevaSindhuStatisticsReportModel model)
        {
            return misReportsDal.SevaSindhuStatisticsReportDetailsYearWise(model);

        }
        /// <summary>
        /// returns SevaSindhuStatisticsReportDetails
        /// </summary>
        /// <param name="SevaSindhuStatisticsReportModel"></param>
        /// <returns>List<SevaSindhuStatisticsReportDetailModel></returns>
        public List<SevaSindhuStatisticsReportDetailModel> LoadSevaSindhuStatisticsReportTblMonthWise(SevaSindhuStatisticsReportModel model)
        {
            return misReportsDal.LoadSevaSindhuStatisticsReportTblMonthWise(model);
        }
    }
}