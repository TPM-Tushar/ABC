using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.PendingDocumentsSummary
{
    public class PendingDocsSummaryDetailsResModel
    {
       public IEnumerable<PendingDocsSummaryDetailsRecordModel> IPendingDocsSummaryRecordList{ get;set;}
        public int TotalCount { get; set; }
        public int FilteredRecordsCount { get; set; }
       public String DecryptedSROName { get; set; }

        public String DecryptedDistrictName { get; set; }

        public String DecryptedColumnName { get; set; }

        public int DecryptedSROCode { get; set; }
    }

    public class PendingDocsSummaryDetailsRecordModel
    {
        public int SerialNo { get; set; }
        public int SroCode { get; set; }
        public string SroName { get; set; }
        public String PendingNumber { get; set; }
        public String PendingDocumentNumber { get; set; }


        public long DocumentID { get; set; }
        public int DocumentNumber { get; set; }
        public String FRN { get; set; }
        public String PendingDate { get; set; }
        public String ReasonOfPending { get; set; }
        public bool WhetherRegistered { get; set; }
        public String RegistrationDate { get; set; }

        // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021        
        //public bool IsCleared { get; set; }

        // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021	

        public string RegArticle { get; set; }

        public string DocumentNo { get; set; }

        public string PresentDate { get; set; }
        public String ConsiderationAmount { get; set; }

        public Decimal ConsiderationAmount_Decimal { get; set; }

        public String WithdrawalDate { get; set; }




    }
}
