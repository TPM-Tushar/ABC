using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class GraphTableResponseModel
    {
        public SalesStatisticsLineChartModel _SalesStatisticsLineChartModel { get; set; }
        public TableDataWrapper _TableDataWrapper { get; set; }
        public SurchargeAndCessBarChartModel _SurchargeAndCessBarChartModel { get; set; }
        public HighValPropLineChartModel _HighValPropLineChartModel { get; set; }
        public RevenueTargetVsAchievedModel _RevenueTargetVsAchievedModel { get; set; }


        public ProgressChartModel _ProgressChartModel { get; set; }
    }
}
