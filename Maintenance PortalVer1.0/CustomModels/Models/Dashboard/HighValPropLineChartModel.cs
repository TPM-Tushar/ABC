using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class HighValPropLineChartModel
    {
        public decimal[] OneLakhToTenLakhs { get; set; }
        public decimal[] TenLakhsToOneCrore { get; set; }
        public decimal[] OneCroreToFiveCrore { get; set; }
        public decimal[] FiveCroreToTenCrore { get; set; }
        public decimal[] AboveTenCrore { get; set; }

        public String[] OneLakhToTenLakhs_DT_CLM { get; set; }
        public String[] TenLakhsToOneCrore_DT_CLM { get; set; }
        public String[] OneCroreToFiveCrore_DT_CLM { get; set; }
        public String[] FiveCroreToTenCrore_DT_CLM { get; set; }
        public String[] AboveTenCrore_DT_CLM { get; set; }
        public string Lbl_OneLakhToTenLakhs { get; set; }
        public string Lbl_TenLakhsToOneCrore { get; set; }
        public string Lbl_OneCroreToFiveCrore { get; set; }
        public string Lbl_FiveCroreToTenCrore { get; set; }
        public string Lbl_AboveTenCrore { get; set; }
        public String[] FinYear { get; set; }
        public String[] FinYear_DT_CLM { get; set; }

        

    }
}
