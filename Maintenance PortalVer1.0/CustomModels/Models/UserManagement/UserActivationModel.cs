using CaptchaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.UserManagement
{
    public class UserActivationModel
    {




        [Required(ErrorMessage = "EmailID is required.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        //ErrorMessage = "Please enter valid e-mail address.")]
        public string Email { get; set; }


        // [Required(ErrorMessage = "Mobile No. is required.")]
        [RegularExpression("[0-9]{10,10}", ErrorMessage = "Mobile Number should be numbers of 10 digits.")]
        public string MobileNumber { get; set; }

      //  public string ActivationCode { get; set; }

        public long UserID { get; set; }

        public string EncryptedId { get; set; }

        public bool IsSuccessfullyInserted { get; set; }

        public bool IsUpdatedSuccessfully { get; set; }//Added by Raman on 04-01-19


        public string ResponseMessage { get; set; }

        [Display(Name = "Enter the text you see in the image below")]
        //[Required(ErrorMessage = "Please Enter Text From the Image Above")]
        //[ValidateCaptcha(ErrorMessage = "Captcha Entered is Invalid")]
        public string Captcha { get; set; }

        public short IsOTPSent { get; set; }

        public bool IsRegisteredEmail { get; set; }


    }
}
