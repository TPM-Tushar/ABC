using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CaptchaLib;
using CustomModels.CustomValidations;

namespace CustomModels.Models.UserManagement
{
    public class UserModel
    {
        public long UserID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression("[a-zA-Z. ]{1,50}", ErrorMessage = "Only alphabets are allowed")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression("[a-zA-Z. ]{1,50}", ErrorMessage = "Only alphabets are allowed")]
        public string LastName { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, ErrorMessage = "Address max length should be 100 char.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,100}", ErrorMessage = "Only alphabets and numbers are allowed")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression("^[1-9]{1}[0-9]{5}$", ErrorMessage = "Invalid Pinode")]
        [StringLength(6, ErrorMessage = "Max length of PIN should be 6")]
        public string Pincode { get; set; }


        [Display(Name = "Mobile Number")]
        [Required(ErrorMessage = "Mobile No. is required.")]
        [RegularExpression("[0-9]{10,10}", ErrorMessage = "Mobile Number should be numbers of 10 digits.")]
        public string MobileNumber { get; set; }

        public List<SelectListItem> CountryDropDown { get; set; }

        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Select proper country")]
        public int CountryID { get; set; }


        // public string PAN { get; set; } //optional...


        [Required(ErrorMessage = "EmailID is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please enter valid e-mail address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        //  [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{8,20}",
        // ErrorMessage = "Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm Password is required.")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }


        //added by akash (22-0-2018)
        [Display(Name = "ID Proof")]
        [Required(ErrorMessage = "IDProof is required.")]
        [Range(1, Int16.MaxValue, ErrorMessage = "IDProof is required.")]
        public Int16 IDProofID { get; set; }


        //added by akash (22-0-2018)
        [Display(Name = "ID Proof Number")]
        [Required(ErrorMessage = "IDProof Number is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Invalid IDProof Number.")]
        [IDProofValidation("IDProofID")]
        public String IDProofNumber { get; set; }

        [Display(Name = "Enter the text you see in the image below")]
        //   [Required(ErrorMessage = "Please Enter Text From the Image Above")]
        // [ValidateCaptcha(ErrorMessage = "Captcha Entered is Invalid")]
        public string Captcha { get; set; }

        public List<SelectListItem> IdProofsTypeDropDown { get; set; }

        public string CountryName { get; set; }
        public string IDProofName { get; set; }

        public bool IsForIDProofForPartner { get; set; }

    }

    public enum Country
    {
        India,
        USA,
        UK
    }

}