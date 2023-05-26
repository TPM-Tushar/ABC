using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.XELFiles
{
    public class XELFilesViewModel
    {

        public string OfficeName { get; set; }
        [Display(Name = "Database")]
        public List<SelectListItem> DatabaseList { get; set; }
        public int databaseID;
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsSearchValuePresent { get; set; }
        public int UserID { get; set; }
        public bool IsPdf { get; set; }
        public bool IsExcel { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        [Required(ErrorMessage = "Please Select valid SRO")]
        public int SROfficeID { get; set; }
        [Display(Name = "From Year")]
        public List<SelectListItem> Year { get; set; }
        [Display(Name = "From Month")]
        public List<SelectListItem> Month { get; set; }
        public int FromYearID { get; set; }
        public int FromMonthID { get; set; }

        [Display(Name = "To Year")]
        public int ToYearID { get; set; }
        [Display(Name = "To Month")]
        public int ToMonthID { get; set; }

        [Display(Name = "From Event Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }
        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Event Date")]
        public String ToDate { get; set; }
        public DateTime DateTime_FromDate { get; set; }
        public DateTime DateTime_ToDate { get; set; }

        public bool IsInserted { get; set; }
        public string ErrorMessage { get; set; }
        public string ResponseMessage { get; set; }
        public string OfficeType { get; set; }

    }
}
