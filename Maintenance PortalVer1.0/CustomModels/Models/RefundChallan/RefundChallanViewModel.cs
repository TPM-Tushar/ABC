using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.RefundChallan
{
    public class RefundChallanViewModel
    {

        [Required(ErrorMessage = "Please enter Party Name.")]
        [Display(Name = "Party Name ")]
        [MinLength(2, ErrorMessage = "Party Name should have minimum 2 Characters")]
        [RegularExpression("[a-zA-Z. ]{1,50}", ErrorMessage = "Only characters are allowed in party name.")]
        public string PartyName { get; set; }


        [Required(ErrorMessage = "Please enter Mobile Number.")]
        [Display(Name = "Party Mobile Number ")]
        [RegularExpression("[6-9]{1}[0-9]{9}", ErrorMessage = "Mobile number should be of 10 digits and should start with 6-10 range .")]
        public string PartyMobileNumber { get; set; }

        
        [Required(ErrorMessage = "Please enter Challan Number.")]
        //[RegularExpression(@"^[A-Z]{2}[0-9]{16}$", ErrorMessage = "Please enter valid Challan number. <br/> " +
        //                         "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
        //                         "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
        //                         "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
        //                         "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> ")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{16}$", ErrorMessage = "Please verify the challan number and challan date as per the format. <br/><br/>" +
                                 "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                                 "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                                 "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                                 "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> ")]
        [Display(Name = "Challan Number")]
        public string InstrumentNumber { get; set; }

        public string InstrumentNumberHidden { get; set; }

        

        //[RegularExpression(@"^[A-Z]{2}[0-9]{16}$", ErrorMessage = "Please enter valid Challan number<br/>" +
        //                         "&nbsp &nbsp &nbsp or valid Challan date with proper month[<span style=\"color:red;\">MM</span>] and year[<span style=\"color:#26C726;\">YY</span>]<br/> " +
        //                         "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/><br/> " +
        //                         "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
        //                         "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
        //                         "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> ")]

        [Required(ErrorMessage = "Please enter Re-Enter Challan Number.")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{16}$", ErrorMessage = "Please verify the challan number and challan date as per the format. <br/><br/>" +
                                 "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                                 "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                                 "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                                 "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> ")]

        [Display(Name = "Re-Enter Challan Number")]
        
        public string ReEnterInstrumentNumber { get; set; }


        [Required(ErrorMessage = "Please enter Challan Date.")]
        [Display(Name = "Challan Date")]
        public string InstrumentDate { get; set; }


        [Required(ErrorMessage = "Please enter Challan Amount.")]
        [Display(Name = "Challan Amount")]
        public decimal ChallanAmount { get; set; }


        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:#.#}")]
        [Required(ErrorMessage = "Please enter Refund Amount.")]
        [Display(Name = "Refund Amount")]
        public decimal RefundAmount { get; set; }


        [Required(ErrorMessage = "Please Enter Application Date.")]
        [Display(Name = "Application Date")]
        public string ApplicationDateTime { get; set; }



        public int DROCode { get; set; }
        public int SROCode { get; set; }
        public long RowId { get; set; }
        public bool IsFinalized { get; set; }
        public int OfficeID { get; set; }
        public long UserID { get; set; }
        public bool IsOrderInEditMode { get; set; }

        public bool isEditMode { get; set; }
        
        public string SROName { get; set; }
        public string DROName { get; set; }

        public int DROfficeID { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int SROfficeID { get; set; }


        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        
        public string ChallanEntryStatus { get; set; }
        public string DRApprovalStatus { get; set; }
        public string RefundPurpose { get; set; }

        [Required(ErrorMessage = "Please select the Challan Purpose")]
        [Display(Name = "Challan Purpose ")]
        public int[] ChallanPurposeId { get; set; }

        public List<SelectListItem> ChallanPurposeList { get; set; }

        public bool IsSRLogin { get; set; }

        public bool IsDRLogin { get; set; }

    }


    public class RefundChallanTableModel
    {
        public long RowId { get; set; }
        public long SrNo { get; set; }
        public string InstrumentNumber { get; set; }
        public string InstrumentDate { get; set; }
        public bool IsDRApproved { get; set; }
        public string ApplicationDateTime { get; set; }
        public decimal ChallanAmount { get; set; }
        public decimal RefundAmount { get; set; }
        public string PartyName { get; set; }
        public int SROCode { get; set; }
        public int DROCode { get; set; }
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
        public string ChallanEntryStatus { get; set; }
        public string DRApprovalStatus { get; set; } 
    }

}
