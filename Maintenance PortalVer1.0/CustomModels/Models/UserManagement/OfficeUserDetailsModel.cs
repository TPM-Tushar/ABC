using CustomModels.CustomValidations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.UserManagement
{
    //added by amit :this two new classes for grid
    public class UserGridWrapperModel
    {
        public UserModelDataColumn[] ColumnArray { get; set; }
        public OfficeUserDetailsModel[] dataArray { get; set; }
        // add another class array  ( document status wise )if required and at mvc controller assign it conditionally before generating response
    }

    public class UserModelDataColumn
    {
        public string title { get; set; } // represents Label at table header
        public string data { get; set; } // represents property name which you want to bind a corresponding label
    }

    public class OfficeUserDetailsModel
    {
        public long UserID { get; set; }
        //added by amit 3-10-18
        public String EncryptedId { get; set; }
        public String CountryName { get; set; }
        public String OfficeName { get; set; }
        public String RoleDesc { get; set; }
        public String LevelDesc { get; set; }
        [Display(Name = "Is Active")]
        public bool IsActive { get; set; }
        public String IsActiveIcon { get; set; }
        public String EditBtn { get; set; }
        public String DeleteBtn { get; set; }
        public bool IsForUpdate { get; set; }

        public List<SelectListItem> OfficeNamesDropDown { get; set; }
        [Display(Name = "Office")]
        // [Required(ErrorMessage = "• Office is required.")]
        // [RegularExpression("^[0-9]*$", ErrorMessage = "Office is required.")]

        // Added by Shubham Bhagat on 5-1-2019
        //[Required(ErrorMessage = "Office is required.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Office is required.")]
        //public int OfficeID { get; set; }

        // Added by Shubham Bhagat on 5-1-2019
        [StringLength(100, ErrorMessage = "Invalid Office")]
        [Required(ErrorMessage = "Office is required.")]
        public String OfficeID { get; set; }

        public List<SelectListItem> LevelDetailsDropDown { get; set; }
        [Display(Name = "Level")]
        //[Required(ErrorMessage = "Level is required.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Level is required.")]
        //public int? LevelID { get; set; }

        // Added by Shubham Bhagat on 5-1-2019
        [StringLength(100, ErrorMessage = "Invalid Level")]
        [Required(ErrorMessage = "Office is required.")]
        public String LevelID { get; set; }

        public List<SelectListItem> RoleDropDown { get; set; }
        [Display(Name = "Role")]

        // Added by Shubham Bhagat on 5-1-2019
        //[Required(ErrorMessage = "Role is required.")]
        //[Range(1, int.MaxValue, ErrorMessage = "Role is required.")]
        //public int RoleID { get; set; }

        // Added by Shubham Bhagat on 5-1-2019
        [StringLength(100, ErrorMessage = "Invalid Role")]
        [Required(ErrorMessage = "Role is required.")]
        public String RoleID { get; set; }


        public List<SelectListItem> CountryDropDown { get; set; }
        [Display(Name = "Country")]
        [Required(ErrorMessage = "Country is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Country is required.")]
        public int CountryID { get; set; }

        //end

        public bool IsForIDProofForPartner { get; set; }


        // For Activity Log
        public long UserIdForActivityLogFromSession { get; set; }



        [Required(ErrorMessage = "First Name is required.")]
        [RegularExpression("[a-zA-Z. ]{1,50}", ErrorMessage = "Only alphabets are allowed")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }


        [Required(ErrorMessage = "Last Name is required.")]
        [RegularExpression("[a-zA-Z. ]{1,50}", ErrorMessage = "Only alphabets are allowed")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }


        [Required(ErrorMessage = "Address is required.")]
        [StringLength(100, ErrorMessage = "Address max length should be 100 char.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,100}", ErrorMessage = "Alphabets and numbers are allowed.")]
        [Display(Name = "Address")]
        public string Address1 { get; set; }

        [Required(ErrorMessage = "Pincode is required.")]
        [RegularExpression("^[1-9]{1}[0-9]{5}$", ErrorMessage = "Invalid Pinode")]
        [StringLength(6, ErrorMessage = "Max length of PIN should be 6")]
        [Display(Name = "Pincode")]
        public string Pincode { get; set; }


        [Required(ErrorMessage = "Mobile No. is required.")]
        [RegularExpression("[0-9]{10,10}", ErrorMessage = "Mobile Number should be numbers of 10 digits.")]
        [Display(Name = "Mobile Number")]
        public string MobileNumber { get; set; }

        // public string PAN { get; set; } //optional...


        [Required(ErrorMessage = "EmailID is required.")]
        [RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        ErrorMessage = "Please enter valid e-mail address.")]
        [Display(Name = "Email ID")]
        public string Email { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required.")]
        //  [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{8,20}",
        // ErrorMessage = "Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
        // Added By Shubham Bhagat on 13-4-2019 
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@])[A-Za-z\d._@]{8,20}",
        ErrorMessage = "Password must contain at least 1 capital letter,digit & must contain one or more of these characters (._@) and length must be 8 char min.")]
        public string Password { get; set; }


        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required.")]
        // Added By Shubham Bhagat on 13-4-2019 
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@])[A-Za-z\d._@]{8,20}",
        ErrorMessage = "Password must contain at least 1 capital letter,digit & must contain one or more of these characters (._@) and length must be 8 char min.")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }


        //added by akash (22-0-2018)
        [Display(Name = "ID Proof")]
        [Required(ErrorMessage = "ID Proof is required.")]
        [Range(1, Int16.MaxValue, ErrorMessage = "ID Proof is required.")]
        public Int16 IDProofID { get; set; }


        //added by akash (22-0-2018)
        [Display(Name = "ID Proof Number")]
        [Required(ErrorMessage = "ID Proof Number is required.")]
        [StringLength(20, MinimumLength = 2, ErrorMessage = "Invalid IDProof Number.")]
        [IDProofValidation("IDProofID")]
        public String IDProofNumber { get; set; }

        [Display(Name = "Enter the text you see in the image below")]
        //   [Required(ErrorMessage = "Please Enter Text From the Image Above")]
        // [ValidateCaptcha(ErrorMessage = "Captcha Entered is Invalid")]
        public string Captcha { get; set; }

        public List<SelectListItem> IdProofsTypeDropDown { get; set; }


        // Added By Shubham Bhagat on 15-12-2018
        [Display(Name = "Office Short Name")]
        public String Office_ShortName { get; set; }
        [Display(Name = "Office Type")]
        public String Office_OfficeType { get; set; }
        [Display(Name = "Office District")]
        public String Office_District { get; set; }

        #region 5-4-2019 For Table LOG by SB
        public String UserIPAddress { get; set; }

        #endregion

        // Added By shubham bhagat 0n 18-04-2019
        [Display(Name = "Username")]
        [Required(ErrorMessage = "Username is required.")]
        [RegularExpression(@"(?=[a-zA-Z].*)[a-zA-Z\d_@.-]{4,50}",
        ErrorMessage = "Username can contain alphabets, digits, characters (._@-) and length must be minimum 4 character.")]

        public String Username { get; set; }


        //Added by mayank for User Manager on 26/08/2021
        public bool isDrLogin { get; set; }

    }

}
