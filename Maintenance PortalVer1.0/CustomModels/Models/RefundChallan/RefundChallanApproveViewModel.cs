using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.RefundChallan
{
    public class RefundChallanApproveViewModel
    {

        [Display(Name = "Party Name ")]
        public string PartyName { get; set; }

        [Display(Name = "Party Mobile Number ")]
        public string PartyMobileNumber { get; set; }
        
        [Display(Name = "Challan Number")]
        public string InstrumentNumber { get; set; }
      
        //[Display(Name = "Re-Enter Challan Number")]
        //public string ReEnterInstrumentNumber { get; set; }

        [Display(Name = "Challan Date")]
        public string InstrumentDate { get; set; }
  
        [Display(Name = "Challan Amount")]
        public decimal ChallanAmount { get; set; }
        
        [Display(Name = "Refund Amount")]
        public decimal RefundAmount { get; set; }

        [Display(Name = "Application Date")]
        public string ApplicationDateTime { get; set; }
        
        //[MaxLength(50, ErrorMessage = "DR Order Number should have maximum 50 Characters")]
        [MaxLength(100, ErrorMessage = "DR Order Number should have maximum 100 Characters")]
        [MinLength(2, ErrorMessage = "DR Order Number should have minimum 2 Characters")]
        //[RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Please Enter Characters and Numbers Only")]
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()\[\]%&*. :;]*$", ErrorMessage = "Please enter valid Order Number.")]
        [Display(Name = "DR Order No")]
        [Required(ErrorMessage ="Please enter DR Order Number.")]
        public string DROrderNumber { get; set; }
        public string DROrderNumberHidden { get; set; }

        [Required(ErrorMessage = "Please Enter DR Order Date")]
        [Display(Name = "DR Order Date")]
        public string DROrderDate { get; set; }

        [Display(Name = "Rejection Reason")]
        //[Required (ErrorMessage ="Please enter Rejection Reason")]
        public string RejectionReason { get; set; }
        public string RejectionReasonHidden { get; set; }
        
        public int DROCode { get; set; }
        public long RowId { get; set; }
        public bool IsFinalized { get; set; }
        public int OfficeID { get; set; }
        public int SROCode { get; set; }

        public string FilePath { get; set; }
        public long UserID { get; set; }
        public string RelativeFilePath { get; set; }
        public bool IsOrderInEditMode { get; set; }
        public string ExistingFileName { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }
        [Display(Name = "Upload Refund Challan DR Order File")]
        public string RefundChallanNoteFile { get; set; }
        public bool isEditMode { get; set; }
        [Display(Name = "Uploaded DR Order PDF :")]
        public string AlreadyFileIsNoted { get; set; }
        public string SROName { get; set; }
        public string DROName { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeOrderList { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeOrderList { get; set; }
        public int SROfficeOrderID { get; set; }
        public long DistrictOrderID { get; set; }
        public string ChallanEntryStatus { get; set; }
        public string DRApprovalStatus { get; set; }
        public List<SelectListItem> ChallanPurposeList { get; set; }
        [Display(Name = "Challan Purpose ")]
        public int[] ChallanPurposeId { get; set; }
        public bool IsSROrDRLogin { get; set; }

    }

    public class RefundChallanApproveTableModel
    {
        public long RowId { get; set; }
        public long SrNo { get; set; }
        public string InstrumentNumber { get; set; }
        public string InstrumentDate { get; set; }
        public int SROCode { get; set; }
        public int DROCode { get; set; }
        public bool IsDRApproved { get; set; }
        public string ApplicationDateTime { get; set; }
        public decimal ChallanAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public string PartyName { get; set; }
        public string RejectionReason { get; set; }
        public string SROUserId { get; set; }
        public string DROUserId { get; set; }
        public string ApproveDateTime { get; set; }
        public string PartyMobileNumber { get; set; }
        public string ViewBtn { get; set; }
        public string Action { get; set; }
        public bool IsFinalized { get; set; }
        public string SROName { get; set; }
        public string DROName { get; set; }
        public string DROrderNumber { get; set; }
        public string DROrderDate { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeOrderList { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeOrderList { get; set; }
        public bool isEditMode { get; set; }
        public int SROrderCode { get; set; }
        public long DistrictOrderCode { get; set; }
        public string ChallanEntryStatus { get; set; }
        public bool IsDRO { get; set; }
        public string DRApprovalStatus { get; set; }
    }

    public class RefundChallanRejectionViewModel
    {
        public string InstrumentNumber { get; set; }

        public long RowId { get; set; }

        [Display(Name = "Rejection Reason")]
        [Required (ErrorMessage ="Please enter Rejection Reason")]
        public string RejectionReason { get; set; }
    }
}
