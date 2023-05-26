using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.AnywhereEC
{
    public class ForgotPasswordRequestModel
    {
        public string EncryptedUId { get; set; }
        public string SessionSalt { get; set; }
        public string Id { get; set; }
        public string Message { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "New Password is required.")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@])[A-Za-z\d._@#$%^&+=]{6,32}",
        ErrorMessage = "Password must contain at least 1 capital letter,digit & must contain one or more of these characters (._@#$%^&+=) and length must be 6 char min.")]
        public string NewPassword { get; set; }

        [Display(Name = "Confirm Password")]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@])[A-Za-z\d._@#$%^&+=]{6,32}",
        ErrorMessage = "Password must contain at least 1 capital letter,digit & must contain one or more of these characters (._@#$%^&+=) and length must be 6 char min.")]
        [Compare("NewPassword", ErrorMessage = "Please Enter the same Password")]
        public string ConfirmPassword { get; set; }
        public bool isToShowResponseMessage { get; set; }

        public long userID;
    }
}
