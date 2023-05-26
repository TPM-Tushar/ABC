using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ScanningStatistics
{
   public class ScanningStatisticsReqModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }


        public int SROfficeID { get; set; }

       
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int DROfficeID { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        public DateTime DateTime_FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }

        public int startLen { get; set; }

        public int totalNum { get; set; }
    }

    public class ScanningStatisticsTableModel
    {
        public long srNo { get; set; }
        public string DistrictName { get; set; }

        public string SROName { get; set; }

        public string RegistrationNumber { get; set; }

        public string DateOfRegistration { get; set; }
        public string ScannedPagecount { get; set; }
        public string ScanDate { get; set; }
        public string DocType { get; set; }
    }
    public class  ScanningStatisticsResModel
    {
       public List<ScanningStatisticsTableModel> scanningStatisticsTableModelsList { get; set; }
    }
}
