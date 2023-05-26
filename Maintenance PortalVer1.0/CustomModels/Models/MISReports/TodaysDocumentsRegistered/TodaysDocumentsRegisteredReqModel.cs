using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.TodaysDocumentsRegistered
{
    public class TodaysDocumentsRegisteredReqModel
    {

        public string OfficeName { get; set; }

        [Display(Name = "SRO")]
        public List<SelectListItem> SROfficeList { get; set; }

        [Required(ErrorMessage = "SR OfficeID is required")]

        public int SROfficeID { get; set; }

        [Display(Name = "District")]

        public List<SelectListItem> DROfficeList { get; set; }
        [Required(ErrorMessage = "DR OfficeID is required")]

        public int DROfficeID { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }

        public string Stamp5Date { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date is required")]
        public DateTime Stamp5DateTime { set; get; }


        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date is required")]
        public DateTime ToDate { get; set; }

        public string ToDate_Str { get; set; }

        public string SroName { get; set; }
        public string DroName { get; set; }

        public DateTime MaxDate { get; set; }

        public string ReportInfo { get; set; }

        //Added by Madhusoodan on 29-04-2020
        public List<SelectListItem> DocumentType { get; set; }
        [Display(Name = "Registration Type")]
        public int DocumentTypeID { get; set; }
        public int isDRO { get; set; }
        public string SpecialSP { get; set; }
    }
}
