using CustomModels.Models.MISReports.ScanningStatistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IScanningStatistics
    {
        ScanningStatisticsReqModel ScanningStatisticsView(int OfficeID);

        ScanningStatisticsResModel GetScanningStatisticsDetails(ScanningStatisticsReqModel scanningStatisticsResModel);
    }
}
