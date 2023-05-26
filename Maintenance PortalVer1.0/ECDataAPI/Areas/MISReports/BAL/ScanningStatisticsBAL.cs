using CustomModels.Models.MISReports.ScanningStatistics;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ScanningStatisticsBAL : IScanningStatistics
    {
        ScanningStatisticsDAL scanningStatisticsDAL = new ScanningStatisticsDAL();
        public ScanningStatisticsReqModel ScanningStatisticsView(int OfficeID)
        {
            return scanningStatisticsDAL.ScanningStatisticsView(OfficeID);
        }
        public ScanningStatisticsResModel GetScanningStatisticsDetails(ScanningStatisticsReqModel scanningStatisticsResModel)
        {
            return scanningStatisticsDAL.GetScanningStatisticsDetails(scanningStatisticsResModel);
        }
    }
}