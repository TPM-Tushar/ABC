using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.SevaSidhuStatistics
{
    public class SevaSindhuStatisticsReportDetailModel
    {
        public long SRNo { get; set; }
        public string  SROoffice { get; set; }

        public string Application_received_date { get; set; }

        public string Application_Received_Month { get; set; }

        public string Application_Received_Year { get; set; }

        public long No_of_Application_Received { get; set; }

        public long No_of_Application_Processed { get; set; }

        public long No_of_Application_Registered { get; set; }

        public long No_of_Application_Rejected { get; set; }
        public string DistrictName { get; set; }

        public string SROName { get; set; }
    }
}
