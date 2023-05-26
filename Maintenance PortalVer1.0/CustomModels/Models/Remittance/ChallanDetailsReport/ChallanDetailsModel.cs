using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.ChallanDetailsReport
{
    public class ChallanDetailsModel
    {
        //[RegularExpression(@"^[CR]{2}[(A-Z0-9)]{16}$", ErrorMessage = "Invalid Challan Number, Challan should be 18 digit with first two constant character")]
        [Required(ErrorMessage = "Instrument Number is mandatory.")]
        [Display(Name = "Challan Number")]
        public string InstrumentNumber { get; set; }

        [Display(Name = "Type")]
        public List<SelectListItem> StampType { get; set; }
        public int StampTypeId { get; set; }

        [Display(Name = "Date")]
        public string Date { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsExcel { get; set; }
    }

    public class ChallanDetailsResModel
    {
        public List<ChallanDetailsDataTableModel> challanDetailsDataTableList { get; set; }
        public int TotalRecords { get; set; }
    }

    public class ChallanDetailsDataTableModel
    {
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

    }
}
