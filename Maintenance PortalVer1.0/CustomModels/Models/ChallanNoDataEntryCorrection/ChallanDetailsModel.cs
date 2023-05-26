using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.ChallanNoDataEntryCorrection
{
    public class ChallanDetailsModel
    {
        [Display(Name = "Enter Challan No to be Corrected.")]
        public string InstrumentNumber { get; set; }

        [Display(Name = "Type")]
        public List<SelectListItem> StampType { get; set; }
        public int StampTypeId { get; set; }

        [Display(Name = "Date")]
        public string Date { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsExcel { get; set; }

        
        //[RegularExpression(@"^[CR]{2}[(A-Z0-9)]{16}$", ErrorMessage = "Invalid Challan Number, Challan should be 18 digit with first two constant character")]
        [Required(ErrorMessage = "New Challan Number is mandatory.")]
        [Display(Name = "New Challan Number")]
        public string NewInstrumentNumber { get; set; }

        //[RegularExpression(@"^[CR]{2}[(A-Z0-9)]{16}$", ErrorMessage = "Invalid Challan Number, Challan should be 18 digit with first two constant character")]
        [Required(ErrorMessage = "Re-Enter Challan Number is mandatory.")]
        [Display(Name = "Re-Enter Challan Number")]
        public string ReEnterInstrumentNumber { get; set; }

        //[Required(ErrorMessage ="Please enter Challan Date.")]
        [Display(Name = "New Challan Date")]
        public string NewInstrumentDate { get; set; }
        //[Required(ErrorMessage ="Please enter Challan Date.")]
        [Display(Name = "Re-Enter Challan Date")]
        public string ReEnterInstrumentDate { get; set; }

        //[RegularExpression(@"^[#$<>]$", ErrorMessage = "Please enter valid Reason.")]
        //[RegularExpression(@"^[A-Za-z]{}$", ErrorMessage = "Please enter valid Reason.")]
        [Required(ErrorMessage = "Reason is mandatory.")]
        [StringLength(250,MinimumLength = 25,ErrorMessage ="Reason should be minimum 25 characters.")]
        public string Reason { get; set; }
        public int OfficeID { get; set; }
        public int LevelID { get; set; }
        public long UserID { get; set; }

        //public int SROfficeID { get; set; }

        //[Display(Name = "SRO Name")]
        //public List<SelectListItem> SROfficeList { get; set; }

        public List<ChallanDetailsDataTableModel> challanDetailsDataTableList { get; set; }
        public int TotalRecords { get; set; }

        public ChallanDetailsModel ChallanDetails { get; set; }

        public string HiddenInstrumentNo { get; set; }

        public List<string> InstrumentNoList { get; set; }

        public bool IsChallanDateSelected { get; set; }

        public string Receipt_StampPayDate { get; set; }
        public string message { get; set; }

        public string RemarkMessage { get; set; }

    }

    public class ChallanDetailsResModel
    {
        public List<ChallanDetailsDataTableModel> challanDetailsDataTableList { get; set; }
        public int TotalRecords { get; set; }
    }

    public class ChallanDetailsDataTableModel
    {
        public long RowId { get; set; }
        public int SrNo { get; set; }
        public string SROName { get; set; }
        public string IsPayDoneAtDROffice { get; set; }
        public string DistrictName { get; set; }
        public string ChallanNumber { get; set; }
        public string ChallanDate { get; set; }
        public decimal Amount { get; set; }
        public string IsStampPayment { get; set; }
        public string IsReceiptPayment { get; set; }
        public string ReceiptNumber { get; set; }
        public string Receipt_StampPayDate { get; set; }
        public string InsertDateTime { get; set; }
        public string ServiceName { get; set; }
        public string DocumentPendingNumber { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public int? SROCode { get; set; }
        public int? DROCode { get; set; }
        public long? DocumentID { get; set; }
        public long ? ReceiptID { get; set; }
        public long? StampDetailsID { get; set; }
        public string UpdateButton { get; set; }
        
        public bool IsDRO { get; set; }
        public int ServiceId { get; set; }
        public string Reason { get; set; }

        public string NewInstrumentNumber { get; set; }
        public string ReEnterInstrumentNumber { get; set; }
        public string NewInstrumentDate { get; set; }
        public string ReEnterInstrumentDate { get; set; }
        public string RemarkMessage { get; set; }
    }

    public class ChallanNoDataEntryCorrectionResponse
    {
        public string ErrorMessage { get; set; }
        public string ResponseMessage { get; set; }

    }

    


}
