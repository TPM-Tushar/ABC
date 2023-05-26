using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.REMDashboard
{
    public class RemittanceOfficeListSummaryModel
    {
        public List<RemittanceOfficeDetailModel> SROfficeDetailList { get; set; }
        public List<RemittanceOfficeDetailModel> DROfficeDetailList { get; set; }

    }
    //Raman Kalegoankar on 05-04-2019
    public class RemittanceOfficeDetailModel
    {
        public String SROfficeName { get; set; }
        public int TotalReceiptsGenerated { get; set; }
        public int TotalReceiptsRemitted { get; set; }
        public int TotalChallanGenerated { get; set; }
        public int TotalPaymentsReconciled { get; set; }
        public string LinkForReceiptsNotRemitted { get; set; }
        public string LinkForChallanNotGenerated { get; set; }
        public string LinkForBankReconcilationPending { get; set; }
        public List<int> TransactionIDList { get; set; }
        public int SROCode { get; set; }
        public int DROCode { get; set; }
        public string DROfficeName { get; set; }
        public string BtnToRedirectToDetails { get; set; }
        public string EncryptedID { get; set; }
        public string EncryptedIDForReceiptsNotRemitted { get; set; }

        public string EncryptedIDChallanNotGenerated { get; set; }
        public string EncryptedIDForBankReconcilationPending { get; set; }

        public int SubmittedForRemittance { get; set; }

        public int ReceiptsNotSubmittedForRemittance{get;set;}

        public string LinkForReceiptsNotSubmittedForRemittance { get; set; }
        public string EncryptedIDForLinkForReceiptsNotSubmittedForRemittance { get; set; }

    }
}
