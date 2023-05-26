using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ESignConsumptionReport
{
    public class ESignTotalConsumptionResModel
    {
        
        public List<ESignConsumptionMonthWiseTableModel> ESignConsumptionMonthWiseTableList { get; set; }

        public int TotalESignConsumedForEC { get; set; }
        public int TotalESignConsumedForCC { get; set; }

        public int TotalESignConsumedForFinYear { get; set; } //total count for selected financial year
    }

    public class ESignConsumptionMonthWiseTableModel
    {
        public string MonthAndYear { get; set; }
        public string TotalESignConsumedForEC { get; set; }
        public string TotalESignConsumedForCC { get; set; }

        public int TotalESignConsumedMonthlyForECCC { get; set; }  //total count for each month
    }
}
