using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.ReScanningDetails
{
    public class ReScanningDetailsResModel
    {
        public List<ReScanningDetailsModel> ReScanningDetailsList { get; set; }
        public decimal TotalAmountSum { get; set; }
    }

    public class ReScanningDetailsModel
    {
        public int SerialNo { get; set; }
        public byte ModuleType { get; set; }
        public string DocumentNo { get; set; }
        public int SROCode { get; set; }
        public string SROOffice { get; set; }
        public string RescanEnableDateTime { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public bool IsFileUploaded { get; set; }
        public string MarriageCaseNo { get; set; }
    }
}
