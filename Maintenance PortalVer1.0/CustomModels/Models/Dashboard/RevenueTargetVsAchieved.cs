using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Dashboard
{
    public class RevenueTargetVsAchievedModel
    {
        public int[] Target { get; set; }    
        public int[] Achieved { get; set; }
        public string Lbl_Target { get; set; }
        public int[] FinYear { get; set; }
        public string Lbl_Achieved { get; set; }
        public string[] FinYears { get; set; }

    }
}
