using CustomModels.Models.MISReports.ScanningStatisticsConsolidated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
 interface IScanningStatisticsConsolidated
    {
        ScanningStatisticsConsolidatedReqModel ScanningStatisticsConsolidatedView(int OfficeID);

        ScanningStatisticsConsolidatedResModel GetScanningStatisticsConsolidatedDetails(ScanningStatisticsConsolidatedReqModel scanningStatisticsConsolidatedReqModel);
    }
}
