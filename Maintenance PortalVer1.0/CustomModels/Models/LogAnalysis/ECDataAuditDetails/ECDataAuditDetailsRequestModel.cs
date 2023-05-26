using CaptchaLib;

//using ECDataAuditDetails.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomModels.Models.LogAnalysis.ECDataAuditDetails
{
    public class ECDataAuditDetailsRequestModel
    {
        [Display(Name = "Office")]
        public List<SelectListItem> OfficeList { get; set; }
        public int OfficeID { get; set; }
        public string OfficeName { get; set; }


        [Display(Name = "Application/Tools")]
        public List<SelectListItem> ProgramNameList { get; set; }

        public int[] ProgramID { get; set; }
        public String programs { get; set; }

        


        [Display(Name = "From Date")]
        [Required(ErrorMessage="From Date Required")]
        public String FromDate { get; set; }


        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }


        public DateTime Datetime_FromDate { get; set; }
        public DateTime Datetime_ToDate { get; set; }


        //[Display(Name = "Enter the text you see in an image")]
        //[Required(ErrorMessage = "Please Enter Text From the Image Above")]
        //[ValidateCaptcha(ErrorMessage = "Captcha Entered is Invalid")]
        //public string Captcha { get; set; }


        public string SearchFlag { get; set; }

        public int MasterTypeId { get; set; }

        public int DetailTypeId { get; set; }

        public string PDFDownloadBtn { get; set; }


    }
}