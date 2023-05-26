using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.ScheduleAllocationAnalysis
{
    public class ScheduleAllocationAnalysisResultModel
    {

        public List<ScheduleAllocationAnalysisDetailsModel> scheduleAllocationDetailsList { get; set; }

        public string SROName { get; set; }
        public string DROName { get; set; }
        public string Year { get; set; }

    }
}
