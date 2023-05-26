#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   ARegisterAnalysisReportModel.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :  Models for ARegister Analysis Report.

*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.ARegisterAnalysisReport
{
   public class ARegisterAnalysisReportModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

      
        public int SROfficeID { get; set; }

      

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Date Required")]
        public String FromDate { get; set; }

        public DateTime DateTime_Date { get; set; }

        public bool ARegister { get; set; }

        public bool AnyWhereECARegister { get; set; }

        public bool KOSARegister { get; set; }

    }

    public class ARegisterResultModel
    {
        public List<RPTARegisterResult> ARegister_Result { get; set; }
        //public ARegisterViewModel viewModel { get; set; }
        public List<AregisterKOSDetailModel> KOS_ARegisterDetailList { get; set; }
       public List<RPTARegisterAnywhereECResult> AnyWhereEC_ARegisterDetailList { get; set; }
    }

    public class RPTARegisterResult
    {
        public long srNo { get; set; }
        public long DocumentID { get; set; }
        public string SROName { get; set; }
        public int SROCode { get; set; }
        public string PresentDateTime { get; set; }
        public string PresenterName { get; set; }
        public string StampArticleName { get; set; }
        public decimal Consideration { get; set; }
        public decimal StampDuty { get; set; }
        public decimal GovtDuty { get; set; }
        public decimal StampDuty_Cash { get; set; }
        public  decimal StampDuty_Others { get; set; }
        public  decimal Infrastructure { get; set; }
        public  decimal Corporation { get; set; }
        public  decimal Muncipal { get; set; }
        public  decimal PrevTotalStampDuty { get; set; }
        public  decimal TalukBoard { get; set; }
        public string DocumentNumber { get; set; }
        public string BookID { get; set; }
        public string VolumeName { get; set; }
        public  string CompletionDate { get; set; }
        public  string ReturnDate { get; set; }
        public  decimal PrevTotalRegistrationFees { get; set; }
        public  decimal RegistrationFees { get; set; }
        public decimal Deficient_RegistrationFees { get; set; }
        public  DateTime PreviousDate { get; set; }
        public  decimal  Deficient_StampDuty { get; set; }
        public  decimal  CopyFees { get; set; }
        public  decimal  PrevTotalCopyFees { get; set; }
        public  decimal  HinduMarriageFee { get; set; }
        public  decimal  OtherFees { get; set; }
        public string OtherFeesDesc { get; set; }
        public  decimal  SplOtherMarriageFee { get; set; }
        public  decimal  PrevTotalMarriageFee { get; set; }
        public  decimal  SplMarriageFee { get; set; }
        public  decimal  PrevTotalMutationFee { get; set; }
        public  decimal  MutationFee { get; set; }
        public string ExemptionDescription { get; set; }
        public string Remarks { get; set; }
        public long ReceiptID { get; set; }
        public long ReceiptNumber { get; set; }
        public long StampDetailsID { get; set; }
    }

    public class RPTARegisterAnywhereECResult
    {
        public long srNo { get; set; }
        public long DocumentID { get; set; }
        public int SROCode { get; set; }
        public string SRONAME { get; set; }
        public DateTime ReceiptDateTime { get; set; }
        public string PresenterName { get; set; }
        public string DocumentNumber { get; set; }
        public decimal OtherFees { get; set; }
        public long ReceiptID { get; set; }
        public long ReceiptNumber { get; set; }
        public string ExemptionDescription { get; set; }
        public string SROApplicationNumber { get; set; }
    }

    public class AregisterKOSDetailModel
    {
        public long srNo { get; set; }
        public string OfficeName { get; set; }

        public string ApplicationType { get; set; }

        public string ApplicationNumber { get; set; }

        public string TransactionDateTime { get; set; }

        public string ChallanRefNumber { get; set; }

        public decimal TotalAmt { get; set; }

        public string PaymentStatus { get; set; }
        public string PartyName { get; set; }

        public string CCStampDuty { get; set; }
    }

    public class ARegisterSynchcheckResultModel
    {
       

        public string ResponseMessage { get; set; }

        public bool ResponseStatus { get; set; }

        public bool ReceiptsSynchronized { get; set; }

        public bool ARegisterGenerated { get; set; }


    }
}
