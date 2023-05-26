using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.HighValueProperties
{
    public class HighValuePropDetailsResModel
    {
       public List<HighValuePropDetailsModel> RangeList { get; set; } 
        public DateTime GenerationDateTime { get; set; }

        public string PDFDownloadBtn { get; set; }
        public string ExcelDownloadBtn { get; set; }

        public string FinancialYear { get; set; }
        public DateTime MaxDate { get; set; }

        public string ReportInfo { get; set; }



    }
}
