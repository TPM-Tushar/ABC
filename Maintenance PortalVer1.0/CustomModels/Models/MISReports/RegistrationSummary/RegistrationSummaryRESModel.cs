using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.RegistrationSummary
{
    public class RegistrationSummaryRESModel
    {
        //public string OfficeName { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }


        public int SROfficeID { get; set; }

        [Display(Name = "Nature of Document")]
        public List<SelectListItem> NatureOfDocumentList { get; set; }

        public int NatureOfDocumentID { get; set; }

        //[Display(Name = "Database")]
        //public List<SelectListItem> DatabaseList { get; set; }

        //public int databaseID;



        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }


        public int startLen { get; set; }
        public int totalNum { get; set; }
        public DateTime DateTime_FromDate { get; set; }
        public DateTime DateTime_ToDate { get; set; }
        public bool IsSearchValuePresent { get; set; }
        public int UserID { get; set; }

        ////[Required(ErrorMessage ="Amount is required")]
        ////[RegularExpression(@"^\d+$", ErrorMessage = "Amount should be in proper format")]
        //[Display(Name = "Amount (For IncomeTax Purpose)")]
        //public long Amount { get; set; }


        ////Added By Raman Kalegaonkar on 27-06-2019
        //[Required(ErrorMessage = "DR OfficeID is required")]

        //public int DROfficeID { get; set; }
        //[Display(Name = "District")]
        //public List<SelectListItem> DROfficeList { get; set; }


        public bool IsPdf { get; set; }
        public bool IsExcel { get; set; }

    }
}
