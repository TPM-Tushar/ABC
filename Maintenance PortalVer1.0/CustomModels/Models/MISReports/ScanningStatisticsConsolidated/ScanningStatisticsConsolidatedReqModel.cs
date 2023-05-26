using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ScanningStatisticsConsolidated
{
   public class ScanningStatisticsConsolidatedReqModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }


        public int SROfficeID { get; set; }


        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int DROfficeID { get; set; }

  

        [Required(ErrorMessage = "Date Required")]
        //[Display(Name = "Date")]
        [Display(Name = "Month")]
        public String ToDate { get; set; }

        public DateTime DateTime_Date { get; set; }
    }

    public class ScanningStatisticsConsolidatedTableModel
    {
        public long srNo { get; set; }
        public string DistrictName { get; set; }

        public string SROName { get; set; }

        public string Month { get; set; }

        public int? Total_S_Page { get; set; }

        public string DocType { get; set; }

    }
    public class ScanningStatisticsConsolidatedResModel
    {
        public List<ScanningStatisticsConsolidatedTableModel> scanningStatisticsConsolidatedTablesList { get; set; }
    }
}
