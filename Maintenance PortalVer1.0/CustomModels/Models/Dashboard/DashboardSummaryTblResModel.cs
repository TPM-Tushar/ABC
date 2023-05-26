using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class DashboardSummaryTblResModel
    {
        //public List<string> DescriptionList { get; set; }
        //public List<DayWiseModel> DayWiseModelList { get; set; }
        //public List<MonthWiseModel> MonthWiseModelList { get; set; }
        //public List<FinYearWiseModel> FinYearWiseModelList { get; set; }
        //public List<UptoDateWiseModel> UptoDateWiseModelList { get; set; }
        public List<DashboardSummaryRecData> DashboardSummaryRecData { get; set; }
        public IEnumerable<DashboardSummaryRecData> IDashboardSummaryRecData { get; set; }
    }
    public class DayWiseModel
    {
        public string Today { get; set; }
        public string Yesterday { get; set; }
    }
    public class MonthWiseModel
    {
        public string CurrentMonth { get; set; }
        public string PreviousMonth { get; set; }
    }
    public class FinYearWiseModel
    {
        public string CurrentFinYear { get; set; }
        public string PrevFinYear { get; set; }
    }
    public class UptoDateWiseModel
    {
        public string UptoPrevFinYear { get; set; }
        public string UptoCurrentFinYear { get; set; }
    }
}
