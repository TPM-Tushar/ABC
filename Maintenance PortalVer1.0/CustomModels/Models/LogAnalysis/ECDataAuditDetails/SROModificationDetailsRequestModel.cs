using CaptchaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace CustomModels.Models.LogAnalysis.ECDataAuditDetails
{
    public class SROModificationDetailsRequestModel
    {
        [Display(Name = "Office")]
        public List<SelectListItem> OfficeList { get; set; }
        public int OfficeID { get; set; }


        [Display(Name = "Application Name")]
        public List<SelectListItem> LogsProgramNameList { get; set; }
        public int[] ProgramID { get; set; }
        public String Programs { get; set; }

        public bool DisplayModificationList { get; set; }
        [Display(Name = "Enter the text you see in an image")]
        [Required(ErrorMessage = "Please Enter Text From the Image Above")]
        [ValidateCaptcha(ErrorMessage = "Captcha Entered is Invalid")]
        public string Captcha { get; set; }


        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }


        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }
    }
}