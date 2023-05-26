using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.TodaysDocumentsRegistered
{
    public class TodaysTotalDocsRegDetailsTable
    {
        public List<TodaysDocumentsRegisteredDetailsModel> TodaysTotalDocsRegTableList { get; set; }

        public int TotalDocuments { get; set; }
        public decimal TotalRegFee { get; set; }
        public decimal TotalStampDuty { get; set; }
        public decimal Total { get; set; }
        public int TotalNoOfRecords { get; set; }
        public string GenerationDateTime { get; set; }
        public string RegistrationDate { get; set; }

        public string PDFDownloadBtn { get; set; }
        public string EXCELDownloadBtn { get; set; }

        public string str_TotalRegFee { get; set; }
        public string str_TotalStampDuty { get; set; }
        public string str_Total { get; set; }

        public DateTime MaxDate { get; set; }

        public string ReportInfo { get; set; }
        public string DistrictName { get; set; }
        public Dictionary<string, int> DistrictWiseSRODictForSingleDistrict { get; set; }
        public Dictionary<int, int> DistrictWiseSRODict { get; set; }






    }
}
