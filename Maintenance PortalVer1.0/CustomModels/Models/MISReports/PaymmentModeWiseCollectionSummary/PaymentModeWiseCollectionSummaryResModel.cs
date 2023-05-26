using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.PaymmentModeWiseCollectionSummary
{
    public class PaymentModeWiseCollectionSummaryResModel
    {
        public List<PaymentModeWiseDetaisModel> PaymentModewiseList { get; set; }
        public Decimal TotalAmount { get; set; }
        public int TotalRecords { get; set; }
        public int TotalNoOfStampDuty { get; set; }
        public String TotalStampDutyCollected { get; set; }
        public int AllTotalNoofReceiptsandStampDuty { get; set; }
        public String TotalRegistrationFeeCollected { get; set; }
        public int TotalNoOfReceipts { get; set; }
        public String AllTotalCollection { get; set; }
    }
    public class PaymentModeWiseDetaisModel
    {
        public int SrNo { get; set; }
        public string DistrictName { get; set; }
        public string SROName { get; set; }
        public int NoOfReceipts { get; set; }
        public String RegistrationFeeCollected { get; set; }
        public int NoOfStampDuty { get; set; }
        public String StampDutyCollected { get; set; }
        public int TotalNoofReceiptsandStampDuty { get; set; }
        public String TotalCollection { get; set; }
        
    }
}
