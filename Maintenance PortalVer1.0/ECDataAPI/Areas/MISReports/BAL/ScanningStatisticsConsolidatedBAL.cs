using CustomModels.Models.MISReports.ScanningStatisticsConsolidated;
using ECDataAPI.Areas.MISReports.DAL;
using ECDataAPI.Areas.MISReports.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ECDataAPI.Areas.MISReports.BAL
{
    public class ScanningStatisticsConsolidatedBAL : IScanningStatisticsConsolidated
    {
        ScanningStatisticsConsolidatedDAL scanningStatisticsConsolidatedDAL = new ScanningStatisticsConsolidatedDAL();
        public ScanningStatisticsConsolidatedReqModel ScanningStatisticsConsolidatedView(int OfficeID)
        {
            return scanningStatisticsConsolidatedDAL.ScanningStatisticsConsolidatedView(OfficeID);
        }
        public ScanningStatisticsConsolidatedResModel GetScanningStatisticsConsolidatedDetails(ScanningStatisticsConsolidatedReqModel scanningStatisticsConsolidatedReqModel)
        {
            return scanningStatisticsConsolidatedDAL.GetScanningStatisticsConsolidatedDetails(scanningStatisticsConsolidatedReqModel);
        }
    }
}