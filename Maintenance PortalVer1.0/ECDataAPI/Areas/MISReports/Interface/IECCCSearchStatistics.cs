using CustomModels.Models.MISReports.ECCCSearchStatistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    interface IECCCSearchStatistics
    {
         ECCCSearchStatisticsViewModel ECCCSearchStatisticsView(int OfficeId);

        ECCCSearchStatisticsResultModel GetSummary(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel);
        ECCCSearchStatisticsResultModel GetDetails(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel);
        ECCCSearchStatisticsViewModel GetSroList(ECCCSearchStatisticsViewModel viewModel);
        ECCCSearchStatisticsResultModel GetSummaryDetailsforExcel(ECCCSearchStatisticsViewModel eCCCSearchStatisticsViewModel);
    }
}
