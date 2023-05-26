using CaptchaLib;
using CustomModels.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ECDataUI.Models.UserRegistration
{
    public class UserViewModel
    {



        public int UserID;

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "• First Name is required.")]
        //  [RegularExpression("^[a-zA-Z]{2,20}",ErrorMessage = "• First Name should contain alphabets only.")]
        [RegularExpression("[a-zA-Z. ]{1,50}", ErrorMessage = "Only characters are allowed.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "• Last Name is required.")]
        //   [RegularExpression("^[a-zA-Z]{2,20}",ErrorMessage = "• Last Name should contain alphabets only.")]
        [RegularExpression("[a-zA-Z. ]{1,50}", ErrorMessage = "Only characters are allowed.")]
        public string LastName { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "• Address is required.")]
        [StringLength(100,ErrorMessage = "• Address max length should be 100 char.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,100}", ErrorMessage = "Only characters and numbers are allowed.")]
        public string Address1 { get; set; }

        [Display(Name = "Pin Code")]
        [Required(ErrorMessage = "• Pincode is required.")]
        [RegularExpression("^[1-9]{1}[0-9]{5}$", ErrorMessage="Invalid Pinode")]
        [StringLength(6,ErrorMessage= "Max length of PIN should be 6")]
      //  [RegularExpression("^[0-9]{6}",ErrorMessage = "• Please enter valid Pincode.")]
        public string Pincode { get; set; }

        [Display(Name = "Mobile No")]
        [Required(ErrorMessage = "• Mobile No is required.")]
        [RegularExpression("[0-9]{10,10}", ErrorMessage = "Mobile Number should be numbers of 10 digits.")]
        public string MobileNumber { get; set; }


        public List<SelectListItem> CountryDropDown { get; set; }


        [Display(Name = "Country")]
        [Required(ErrorMessage = "• Country is required.")]
        [Range(1,int.MaxValue, ErrorMessage="Select proper country")]
        public int CountryID { get; set; }

        //[Display(Name = "PAN No")]
        //[Required(ErrorMessage = "• PAN is required.")]
        //[RegularExpression(@"^[(A-Z)]{3}[(P|C|H|F|A|T|B|L|J|G)]{1}[(A-Z)]{1}[\d]{4}[(A-Z)]{1}$", ErrorMessage = "• Invalid PAN format.")]
        //public string PAN { get; set; } //optional...

        [Display(Name = "User Name")]
        [Required(ErrorMessage = " Email is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",ErrorMessage = " Please enter valid e-mail address.")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "• Password is required.")]
        //[RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{6,20}",
        //ErrorMessage = " Password should be Alphanumeric and must contain one or more of these characters (._@)")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@])[A-Za-z\d._@]{8,20}",
        ErrorMessage = "Password must contain at least 1 capital letter,digit & must contain one or more of these characters (._@) and length must be 8 char min.")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = " Confirm Password is required.")]
       // [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{6,20}",
       //ErrorMessage = " Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@])[A-Za-z\d._@]{8,20}",
        ErrorMessage = "Password must contain at least 1 capital letter,digit & must contain one or more of these characters (._@) and length must be 8 char min.")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = " Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Enter the text you see in the image")]
        [Required(ErrorMessage = " Please Enter Text From the Image Above")]
        [ValidateCaptcha(ErrorMessage = " Captcha Entered is Invalid")]
        public string Captcha { get; set; }

        //added by akash (22-0-2018)
        [Display(Name = "ID Proof")]
        [Required(ErrorMessage = "• ID Proof is required.")]
        [Range(1, Int16.MaxValue, ErrorMessage = "• ID Proof is required.")]
        public Int16 IDProofID { get; set; }


        //added by akash (22-0-2018)
        [Display(Name = "ID Proof Number")]
        [Required(ErrorMessage = " ID Proof Number is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = " Invalid ID Proof Number.")]
        [IDProofValidation("ID ProofID")]
        public String IDProofNumber { get; set; }

        public List<SelectListItem> IdProofsTypeDropDown { get; set; }

        public bool IsForIDProofForPartner { get; set; }

    }

    public enum Country
    {
        India,
        USA,
        UK
    }







}
