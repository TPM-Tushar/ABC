
using CaptchaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CustomModels.Models.UserManagement
{
    public class LoginViewModel
    {

        [Required(ErrorMessage = "Email is required.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        //ErrorMessage = "Please enter valid e-mail address.")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        //[RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{8,20}",
        //  ErrorMessage = "• Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
        public string Password { get; set; }

        //public bool IsValid { set; get; }


    }


    public class LoginViewModelTemp
    {

        [Required(ErrorMessage = "Username is required.")]
        //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
        //ErrorMessage = "Please enter valid e-mail address.")]
        [RegularExpression(@"(?=[a-zA-Z].*)[a-zA-Z\d_@.-]{4,50}",
        ErrorMessage = "Username can contain alphabets, digits, characters (._@-) and length must be minimum 4 character.")]
        public string EmailId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        //[RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{8,20}",
        // ErrorMessage = "• Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
        public string Password { get; set; }


        [Display(Name = "Enter the text you see in an image")]
        [Required(ErrorMessage = "Captcha is required.")]
        [ValidateCaptcha(ErrorMessage = "Entered Captcha is Invalid")]
        public string Captcha { get; set; }
    }
}