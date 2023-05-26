using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.REMDashboard
{
    public class RemittanceDiagnosticsDetialsWrapperModel
    {

        public bool IsActive { get; set; }
        public int SROOfficeID { get; set; }

        public int DROOfficeID { get; set; }

        public DateTime Datetime_FromDate { get; set; }
        public DateTime Datetime_ToDate { get; set; }

        public int StartLength { get; set; }

        public int TotalNum { get; set; }

        public int TransactionID { get; set; }

        public int TransactionStatus { get; set; }

        public string DeptRefNo { get; set; }

        public string ChallanRefNumber { get; set; }
        public int IsDro { get; set; }
        

    }
}
