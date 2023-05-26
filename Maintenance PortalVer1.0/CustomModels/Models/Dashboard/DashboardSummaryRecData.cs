using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class DashboardSummaryRecData
    {
        public DashboardSummaryRecData()
        {
            this.Description = string.Empty;
        }
        public string Today { get; set; }
        public string Yesterday { get; set; }
        public string WRTYesterday { get; set; }
        public string CurrentMonth { get; set; }
        public string PreviousMonth { get; set; }
        public string WRTPreviousMonth { get; set; }
        public string CurrentFinYear { get; set; }
        public string PrevFinYear { get; set; }
        public string WRTPrevFinYear { get; set; }
        public string UptoPrevFinYear { get; set; }
        public string WRTUptoPrevFinYear { get; set; }
        public string UptoCurrentFinYear { get; set; }
        public string Description { get; set; }

        public decimal Percentage { get; set; }
    }
}
