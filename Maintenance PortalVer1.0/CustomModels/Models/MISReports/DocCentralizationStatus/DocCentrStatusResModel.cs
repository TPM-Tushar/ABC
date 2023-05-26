using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.DocCentralizationStatus
{
    public class DocCentrStatusResModel
    {
      public List<DocCentrStatusDetailsModel> DetailsList { get; set; }
      public int OfficeID { get; set; }
      public string ExcelDownloadBtn { get; set; }
      public int TotalDocsCentralized { get; set; }
      public int TotalDocsRegdPreviously { get; set; }
    }
    public class DocCentrStatusDetailsModel
    {
        public int SerialNo { get; set; }
        public int SROCode { get; set; }
        public string SROName { get; set; }
        public int DocsCentlzdToday { get; set; }
        public int DocsRegdPreviouslyCrtlzdToday { get; set; }
        public String LatestCentralizationDate { get; set; }



    }

}
