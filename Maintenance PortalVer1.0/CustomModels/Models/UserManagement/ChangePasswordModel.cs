using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.UserManagement
{
   public  class ChangePasswordModel
    {

        public string Id { get; set; }

        public string Message { get; set; }
        [Required(ErrorMessage = "Password is required.")]

        [RegularExpression(@"(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[._@])[A-Za-z\d._@]{8,20}",
        ErrorMessage = "Password must contain at least 1 capital letter,digit & must contain one or more of these characters (._@) and length must be 8 char min.")]

        //[RegularExpression(@"(?=.*[A-Za-z])(?=.*\d)(?=.*[@._])[A-Za-z\d@._]{6,20}",
        //ErrorMessage = "Password should be Alphanumeric and must contain  one or more of these characters (._@)")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string ConfirmPassword { get; set; }
        public bool isToShowResponseMessage { get; set; }

    }
}
