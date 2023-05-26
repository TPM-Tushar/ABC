using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.ChallanNoDataEntryCorrection
{
    public class ChallanNoDataEntryCorrectionViewModel
    {
        [Required(ErrorMessage = "Please enter old Challan number.")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{16}$", ErrorMessage = "Please verify the old challan number and challan date as per the format. <br/><br/>" +
                                 "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                                 "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                                 "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                                 "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> ")]
        [Display(Name = "Old Challan Number")]
        public string OldInstrumentNumber { get; set; }


        [Required(ErrorMessage = "Please enter Re-Enter old challan number.")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{16}$", ErrorMessage = "Please verify the Re-Enter old challan number and challan date as per the format. <br/><br/>" +
                         "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                         "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                         "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                         "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> ")]
        [Display(Name = "Re-Enter Old Challan Number")]
        public string ReEnterOldInstrumentNumber { get; set; }



        [Required(ErrorMessage = "Please enter new challan number.")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{16}$", ErrorMessage = "Please verify the new challan number and challan date as per the format. <br/><br/>" +
                         "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                         "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                         "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                         "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> ")]
        [Display(Name = "New Challan Number")]
        public string NewInstrumentNumber { get; set; }


        
        [Required(ErrorMessage = "Please enter Re-Enter new challan number.")]
        [RegularExpression(@"^[A-Z]{2}[0-9]{16}$", ErrorMessage = "Please verify the Re-Enter new challan number and challan date as per the format. <br/><br/>" +
                         "&nbsp &nbsp &nbsp e.g.  18 digit Challan number. <br/> " +
                         "&nbsp &nbsp &nbsp IG<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123" +
                         "[IG<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\">YY</span>123456789123] <br/> " +
                         "&nbsp &nbsp &nbsp CR<span style=\"color:red ;\">03</span><span style=\"color:#26C726;\">22</span>123456789123[CR<span style=\"color:red;\">MM</span><span style=\"color:#26C726;\"=>YY</span>123456789123] <br/> ")]
        [Display(Name = "Re-Enter new Challan Number")]
        public string ReEnterNewInstrumentNumber { get; set; }



        public int DROfficeID { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int SROfficeID { get; set; }


        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }


        [Required(ErrorMessage = "Please enter old Challan Date.")]
        [Display(Name = "Old Challan Date")]
        public string OldInstrumentDate { get; set; }


        [Required(ErrorMessage = "Please enter new Challan Date.")]
        [Display(Name = "new Challan Date")]
        public string NewInstrumentDate { get; set; }

        public int OfficeID { get; set; }
        public long UserID { get; set; }
        public int LevelID { get; set; }

    }


    //public class ChallanNoDataEntryCorrectionResponse
    //{
    //    public string ErrorMessage { get; set; }
    //    public string ResponseMessage { get; set; }
        
    //}



}
