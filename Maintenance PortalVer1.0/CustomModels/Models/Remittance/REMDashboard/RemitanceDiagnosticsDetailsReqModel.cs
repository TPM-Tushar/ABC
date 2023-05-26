using CaptchaLib;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.REMDashboard
{
    public class RemitanceDiagnosticsDetailsReqModel
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
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }


        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }


        public DateTime Datetime_FromDate { get; set; }
        public DateTime Datetime_ToDate { get; set; }


        [Display(Name = "Enter the text you see in an image")]
        [Required(ErrorMessage = "Please Enter Text From the Image Above")]
        [ValidateCaptcha(ErrorMessage = "Captcha Entered is Invalid")]
        public string Captcha { get; set; }


        public string SearchFlag { get; set; }

        public int MasterTypeId { get; set; }

        public int DetailTypeId { get; set; }

        //Added By Raman Kalegaonkar on 15-03-2019

        [Display(Name = "SR Office")]
        public List<SelectListItem> SROOfficeList { get; set; }

        [Display(Name = "DR Office")]
        public List<SelectListItem> DROOfficeList { get; set; }

        public int SROOfficeID { get; set; }

        public int DROOfficeID { get; set; }

        [Display(Name = "Select All")]
        public bool IsActive { get; set; }

        public bool IsDRO { get; set; }


        [Display(Name = "Transaction Status")]
        public List<SelectListItem> TransactionStatusList { get; set; }

        public int TransactionID { get; set; }

        public int TransactionStatusID { get; set; }

        public bool IsForwardedFromSummaryLink { get; set; } 


    }
}