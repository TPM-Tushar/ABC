using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.KaveriSupport
{
    public class AppDeveloperViewModel
    {
        // Generate KeyPair
        public string PublicKeyStr { get; set; }
        public string PrivateKeyStr { get; set; }
        public bool IsUploadedSuccessfully { get; set; }
        public long UserID { get; set; }
        public string ResponseMessage { get; set; }

        // Decrypt Enclosure
        public string Filepath { get; set; }

        //Added by Akash
        public string EcryptedPatchFilePath { get; set; }
        public string ErrorMessage { get; set; }

        //Encrypt SQL Patch
        [Display(Name = "Ticket Number")]
        [Required(ErrorMessage ="Ticket Number is required.")]
        [StringLength(8, ErrorMessage = "Ticket Number should be less than 8 digits.")]
        [RegularExpression(@"^([0-9])+$", ErrorMessage ="Please Enter valid Ticket Number.")]
        public string TicketNumber { get; set; }



        //added by akash (25-04-2018)

        [Display(Name = "SRO Name")]
        [Required(ErrorMessage = "SRO Name is required.")]
        [Range(1, Int16.MaxValue, ErrorMessage = "SRO Name is required.")]
        public Int16 SRONameID { get; set; }

        public List<SelectListItem> SRONameDropDown { get; set; }



        [Display(Name = "Module Name")]
        [Required(ErrorMessage = "Module Name is required.")]
        [Range(1, Int16.MaxValue, ErrorMessage = "Module Name is required.")]
        public Int16 ModuleID { get; set; }

        public List<SelectListItem> ModuleNameDropDown { get; set; }



        [Display(Name = "Ticket Description")]
        [Required(ErrorMessage = "Ticket Description is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Ticket Description should be min 5 and max 200 characters long.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,100}", ErrorMessage = "Only alphabets and numbers are allowed.")]
        public string TicketDescription { get; set; }


        public bool ResponseStatus { get; set; }

        public long TicketID { get; set; }
        //[Display(Name = "Ticket Number")]
        //[Required(ErrorMessage = "Ticket Number is required.")]
        //[RegularExpression("[0-9]", ErrorMessage = "Ticket Number should be numbers only.")]
        //public string TicketNumber { get; set; }



    }

    public class DecryptEnclosureModel
    {
        public bool ResponseStatus { get; set; }
        public string ErrorMessage { get; set; }

        public string Filepath { get; set; }
    }

}
