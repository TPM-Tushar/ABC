using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class SurchargeAndCessBarChartModel
    {
        public decimal[] SurchargeCollected { get; set; }
        public decimal[] CessCollested { get; set; }
        public decimal[] Total { get; set; }

        public String[] FinYear { get; set; }

        public string Lbl_SurchargeCollected { get; set; }
        public string Lbl_CessCollected { get; set; }
        public string Lbl_Total { get; set; }


    }
}
