using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.KOSPaymentStatusReport
{
      public  class KOSPaymentStatusRptResModel
    {
        public List<KOSPaymentStatusDetailsModel> KOSPaymentStatusDetailsList { get; set; }
            public int TotalCount { get; set; }
        public DateTime GenerationDateTime { get; set; }

        public string DistrictName { get; set; }
        public string SROName { get; set; }

    }

    public class KOSPaymentStatusDetailsModel
    {
        public int SerialNo { get; set; }

        public string ApplicationTypeId { get; set; }

        public string TotalPaymentReqSubmitted { get; set; }

        public string TotalPaymentsSuccessful { get; set; }

        public string TotalPaymentsFailed { get; set; }

        public string TotalPaymentsExpired { get; set; }

        public string TotalPaymentsPending { get; set; }

        public string  PaymentPendingSince { get; set; }

        public string LongestPaymentPendingSince { get; set; }

        public string AvgPaymentRealizationSpan { get; set; }

        #region ADDED BY SHUBHAM BHAGAT ON 11-12-2020
        public string PaymentWithNoStatus { get; set; }

        public string LongestPaymentPendingSinceDays { get; set; }

        public string LongestPaymentPendingSinceDate { get; set; }    
            
        #endregion

    }
}
