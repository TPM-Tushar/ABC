using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.AnywhereECLog
{
    public class AnywhereECLogView
    {
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DistrictList { get; set; }

        public int DistrictID { get; set; }

        [Display(Name = "Log Type")]
        public List<SelectListItem> LogTypeList { get; set; }

        public int LogTypeID { get; set; }
        public bool IsExcel { get; set; }
        public String SearchValue { get; set; }


    }
}
