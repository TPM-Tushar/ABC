using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.IncomeTaxReport
{
    public class IncomeTaxReportResultModel
    {
        public List<IncomeTaxReportDetailsModel> incomeTaxReportDetailsList { get; set; }

        public string SROName { get; set; }
        public string DROName { get; set; }
        public string FinYearName { get; set; }
        public bool isClickedOnSearchBtn { get; set; }

    }
}
