/*
 * Author: Rushikesh Chaudhari
 * Class Name: ISevaSindhuStatistics.cs
 */

using CustomModels.Models.MISReports.SevaSidhuStatistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface ISevaSindhuStatistics
    {
        SevaSindhuStatisticsReportModel SevaSindhuStatisticsReportView(int OfficeID);

        List<SevaSindhuStatisticsReportDetailModel> SevaSindhuReportDetails(SevaSindhuStatisticsReportModel model);
       
        List<SevaSindhuStatisticsReportDetailModel> SevaSindhuStatisticsReportDetailsYearWise(SevaSindhuStatisticsReportModel model);
     
        List<SevaSindhuStatisticsReportDetailModel> LoadSevaSindhuStatisticsReportTblMonthWise(SevaSindhuStatisticsReportModel model);
      }
}
