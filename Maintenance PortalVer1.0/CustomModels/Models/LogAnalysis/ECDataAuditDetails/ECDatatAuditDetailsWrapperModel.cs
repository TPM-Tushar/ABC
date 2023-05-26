using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.LogAnalysis.ECDataAuditDetails
{
    public class ECDatatAuditDetailsWrapperModel
    {
        public DateTime Datetime_FromDate { get; set; }
        public DateTime Datetime_ToDate { get; set; }

        public String programs { get; set; }
        public int OfficeID { get; set; }

        public int StartLength { get; set; }

        public int TotalNum { get; set; }



    }

    public class WrapperModelForDescPDF
    {
        public long logID { get; set; }
        public int logTypeID { get; set; }
        public int sROCODE { get; set; }
        public long iTEMID { get; set; }
    }
}
