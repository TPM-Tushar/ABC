using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.LogAnalysis.ValuationDifferenceReport
{
    public class ValuationDiffReportViewModel
    {
        public int StartLen { get; set; }
        public int TotalNum { get; set; }
        public String SearchValue { get; set; }
        public bool IsExcel { get; set; }

        public int SROCode { get; set; }
        public DateTime MaxDate { get; set; }

        public string ReportInfo { get; set; }

        public int PropertyID { get; set; }

        [Display(Name = "Property Type")]
        public List<SelectListItem> PropertyTypeList { get; set; }


        [Display(Name = "Registration Article")]
        public List<SelectListItem> RegistrationArticleList { get; set; }
        public int[] RegIDArr { get; set; }
        public string strRegArtId { get; set; }


    }
}
