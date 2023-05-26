using CustomModels.Models.MISReports.DigilockerStatistics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECDataAPI.Areas.MISReports.Interface
{
    public interface IDigilockerStatistics
    {
        DigiLockerStatisticsViewModel DigilockerStatisticsView(int OfficeID);

        DigilockerStatisticsResponseModel DigilockerStatisticsReportDetails(DigiLockerStatisticsViewModel model);
        
    }
}
