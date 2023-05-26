using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.PendingDocumentsSummary
{
    public class PendingDocsSummaryDetailsReqModel
    {

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }
        public int SROfficeID { get; set; }
        public bool IsExcel { get; set; }
        public int DistrictID { get; set; }
        public string ColumnName { get; set; }


    }
}
