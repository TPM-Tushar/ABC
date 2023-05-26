using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.ECCCSearchStatistics
{
    public class ECCCSearchStatisticsResultModel
    {
        public List<ECCCSearchStatisticsSummaryModel> SummaryList { get; set; }

        public List<ECCCSearchStatisticsDetailModel> DetailsList { get; set; }

        public int TotalSummaryRecords { get; set; }

        public int TotalDetailRecords { get; set; }

        public string MonthName { get; set; }

        public string FinancialYearName { get; set; }

        public string SroName { get; set; }

        public string DroName { get; set; }

        public SearchBy SearchBy { get; set; }
    }

    public class ECCCSearchStatisticsSummaryModel
    {
        public int SrNo { get; set; }
        public string MonthYear { get; set; }

        public int TotalUserLogged { get; set; }

        public int TotalECSearched { get; set; }

        public int TotalECSubmitted { get; set; }

        public int TotalECSigned { get; set; }

        public int TotalCCSearched { get; set; }

        public int TotalCCSubmitted { get; set; }

        public int TotalCCSigned { get; set; }
        //ADDED BY PANKAJ ON 11-06-2021
        public int AnywhereTotalECSigned { get; set; }
        public int LocalTotalECSigned { get; set; }
        public int AnywhereTotalCCSigned { get; set; }
        public int LocalTotalCCSigned { get; set; }
    }

    public class ECCCSearchStatisticsDetailModel
    {
        public int SrNo { get; set; }
        public string MonthYear { get; set; }

        public int TotalUserLogged { get; set; }

        public int TotalECSearched { get; set; }

        public int TotalECSubmitted { get; set; }

        public int TotalECSigned { get; set; }

        public int TotalCCSearched { get; set; }

        public int TotalCCSubmitted { get; set; }

        public int TotalCCSigned { get; set; }
        //ADDED BY PANKAJ ON 11-06-2021
        public int AnywhereTotalECSigned { get; set; }
        public int LocalTotalECSigned { get; set; }
        public int AnywhereTotalCCSigned { get; set; }
        public int LocalTotalCCSigned { get; set; }
        public int SroCode { get; set; }

    }
}
