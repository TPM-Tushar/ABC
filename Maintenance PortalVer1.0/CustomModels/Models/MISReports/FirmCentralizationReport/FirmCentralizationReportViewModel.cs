using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.FirmCentralizationReport
{
    public class FirmCentralizationReportViewModel
    {
        [Display(Name = "DRO Name")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int DROfficeID { get; set; }
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_FromDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }

        public string CCFileDetailsBy { get; set; }
    }
}
