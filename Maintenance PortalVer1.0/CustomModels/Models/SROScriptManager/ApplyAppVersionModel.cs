using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.SROScriptManager
{
    public class ApplyAppVersionModel
    {
        [Display(Name = "Application Name List")]
        [Required(ErrorMessage = "Application Name is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Application Name should be min 2 and max 50 characters long.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,50}", ErrorMessage = "Only alphabets are allowed.")]
        public List<SelectListItem> ApplicationNameList { get; set; }

        [Display(Name = "SRO Office Name List")]
        [Required(ErrorMessage = "Application Name is required.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,50}", ErrorMessage = "Only alphabets are allowed.")]
        public List<SelectListItem> SROfficeNameList { get; set; }

        [Display(Name = "DRO Office Name List")]
        [Required(ErrorMessage = "Application Name is required.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,50}", ErrorMessage = "Only alphabets are allowed.")]
        public List<SelectListItem> DROfficeNameList { get; set; }

        [Display(Name = "List of DR, SR & SROCode")]
        [Required(ErrorMessage = "Application Name is required.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,50}", ErrorMessage = "Only alphabets are allowed.")]
        public List<ApplyAppVersionDetaillstModel> ApplyAppVersionViewList { get; set; }


        [Display(Name = "Application Name ")]
        [Required(ErrorMessage = "Application Name is required.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Application Name should be min 2 and max 50 characters long.")]
        [RegularExpression("[a-zA-Z0-9-.,/() ]{1,50}", ErrorMessage = "Only alphabets are allowed.")]
        public string AppName { get; set; }


        [Display(Name = "District")]
        public List<SelectListItem> DROOfficeList { get; set; }

        [Display(Name = "DRO Name")]
        public string DROfficeList { get; set; }
        public int DROfficeID { get; set; }

        [Display(Name = "SRO Name")]
        public string SROfficeList { get; set; }
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
        public DateTime? LastDateForPatchUpdate { get; set; }


        public bool? IsDROOffice { get; set; }
        public string IsDROOfficestr { get; set; }

        public bool IsInsertedSuccessfully { get; set; }

        public string ErrorMessage { get; set; }

        public string ResponseMessage { get; set; }

        public bool IsForExcelDownload { get; set; }
        public bool IsForSearch { get; set; }
    }

    public class ApplyAppVersionWrapperModel
    {
        public List<ApplyAppVersionDetaillstModel> applyAppVersionDetailList { get; set; }

        public int TotalCount { get; set; }
    }

    public class ApplyAppVersionDetaillstModel
    {
        public string SerialNo { get; set; }

        public string AppName { get; set; }

        public string SROId { get; set; }

        public string SROOfficeName { get; set; }

        public string AppMinor { get; set; }

        public string AppMajor { get; set; }

        public string ReleaseDate { get; set; }

        public string LastDateForPatch { get; set; }

        public string DROId { get; set; }

        public string DROOfficeName { get; set; }

        public string Action { get; set; }

        public string SPExecutionDate { get; set; }


        public string IsDROOffice { get; set; }
    }



}
