using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.HighValueProperties
{
    public class HighValuePropertiesReqModel
    {
        [Display(Name = "Range")]
        public List<SelectListItem> RangeList{ get; set; }

        public int RangeID { get; set; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinYearList { get; set; }

        public int FinYearID { get; set; }
        public DateTime MaxDate { get; set; }

        public string ReportInfo { get; set; }
    }
}
