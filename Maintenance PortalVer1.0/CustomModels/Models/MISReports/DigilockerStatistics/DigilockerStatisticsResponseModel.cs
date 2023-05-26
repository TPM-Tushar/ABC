using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.DigilockerStatistics
{
    public class DigilockerStatisticsResponseModel
    {
        public string errorMessage { get; set; }
        public bool isDigilockerStatisticsFound { get; set; }
        public List<DigilockerStatisticsDetailsModel> DigilockerStatisticsDetailsList { get; set; }
        public int TotalCount { get; set; }
    }
}
