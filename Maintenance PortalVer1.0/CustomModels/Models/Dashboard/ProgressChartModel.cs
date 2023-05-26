using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class ProgressChartModel
    {
        public int[] Documents { get; set; }
        public int[] Revenue { get; set; }
        public string Lbl_Documents { get; set; }
        public String[] FinYear { get; set; }
        public string Lbl_Revenue { get; set; }
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
        public String[] Months { get; set; }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
    }
}
