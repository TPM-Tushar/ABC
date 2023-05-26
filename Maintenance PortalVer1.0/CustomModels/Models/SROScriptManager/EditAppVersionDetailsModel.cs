using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.SROScriptManager
{
    public class EditAppVersionDetailsModel
    {

        [Display(Name = "Application Name")]
        [Required(ErrorMessage = "Application Name is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Application Name should be min 2 and max 50 characters long.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,50}", ErrorMessage = "Only alphabets are allowed.")]
        public string AppName { get; set; }

        public int DROfficeID { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }

        [Display(Name = "App Major")]
        [Required(ErrorMessage = "App Major is required.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only Number are allowed.")]
        public int AppMajor { get; set; }


        [Display(Name = "App Minor")]
        [Required(ErrorMessage = "App Minor is required.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Only Number are allowed.")]
        public int AppMinor { get; set; }


        [Display(Name = "Release Date")]
        [Required(ErrorMessage = "Release Date is required.")]
        public DateTime ReleaseDate { get; set; }


        [Display(Name = "Patch Update Last Date")]
        [Required(ErrorMessage = "Patch Update Last Date is required.")]
        public DateTime LastDateForPatchUpdate { get; set; }

        public bool IsUpdatedSuccessfully { get; set; }

        public string ErrorMessage { get; set; }

        public string ResponseMessage { get; set; }
    }
}
