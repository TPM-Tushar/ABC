using CaptchaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Alerts
{
    public class OTPValidationModel
    {
        public short OTPTypeId { get; set; }
        public short IsOTPSent { get; set; }
        public string EncryptedUId { get; set; }
        [Required(ErrorMessage = "OTP is mandatory.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Invalid OTP. (OTP format: 123456)")]
        public string EncryptedOTP { get; set; }
        public string SessionSalt { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

       //[Required(ErrorMessage = "Password is required.")]
       // [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{6,20}",
       // ErrorMessage = "Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
       // public string Password { get; set; }

       // [Display(Name = "New Password")]
       // [Required(ErrorMessage = "New Password is required.")]
       // [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{6,20}",
       //ErrorMessage = "Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
       // public string NewPassword { get; set; }

       // [Display(Name = "Confirm Password")]
       // [Required(ErrorMessage = "Confirm Password is required.")]
       // [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{6,20}",
       //ErrorMessage = "Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
       // [Compare("NewPassword", ErrorMessage = "Please Enter the same Password")]   
       // public string ConfirmPassword { get; set; }

        public bool isToShowResponseMessage { get; set; }

       // [Display(Name = "Current Password")]
       // [Required(ErrorMessage = "Current Password is required.")]
       // [RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{6,20}",
       //ErrorMessage = "Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
       // public string CurrentPassword { get; set; }

        public string OTP;

        public long userID;
        public int NumberOfPreviousPasswordNotAllowed;

        public bool IsCaptchaVerified { get; set; }

      

    }
    public class OTPValidationModelForCaptcha
    {
        public short OTPTypeId { get; set; }
        public short IsOTPSent { get; set; }
        public string EncryptedUId { get; set; }
        [Required(ErrorMessage = "OTP is mandatory.")]
        [RegularExpression(@"^[0-9]{6}$", ErrorMessage = "Invalid OTP. (OTP format: 123456)")]
        public string EncryptedOTP { get; set; }
        public string SessionSalt { get; set; }

        public string Id { get; set; }

        public string Message { get; set; }

     
        public bool isToShowResponseMessage { get; set; }

   

        public string OTP;

        public long userID;
        public int NumberOfPreviousPasswordNotAllowed;

        public bool IsCaptchaVerified { get; set; }

        [Display(Name = "Enter Captcha")]
        [Required(ErrorMessage = "Captcha is required.")]
        [ValidateCaptcha(ErrorMessage = "Entered Captcha is Invalid")]
        public string Captcha { get; set; }

        // Added by shubham bhagat on 20-04-2019 to show mobile number
        public String MobileNumber { get; set; }


    }
}
