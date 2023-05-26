using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.DigilockerStatistics
{
    public class DigiLockerStatisticsViewModel
    {
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsExcel { get; set; }
        
        [Display(Name = "From Date ")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date Required")]
        public String ToDate { get; set; }
    }
}
