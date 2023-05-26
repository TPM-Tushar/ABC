using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.KOSPaymentStatusReport
{
    public  class KOSPaymentStatusRptResTableModel
    {
        public List<KOSPaymentStatusDetailsTableModel> KOSPaymentStatusDetailsTableList { get; set; }

        public int TotalCount { get; set; }

        public string DistrictName { get; set; }
        public string SROName { get; set; }

    }

    public class KOSPaymentStatusDetailsTableModel
    {
        public int SerialNo { get; set; }

        public string OfficeName { get; set; }

        public string ApplicationType { get; set; }

        public string ApplicationNumber { get; set; }

        public string TransactionDate { get; set; }

        public string ChallanReferencenNumber { get; set; }

        public string PaymentStatus { get; set; }

        public string PaymentRealizedInDays { get; set; }


    }
}
