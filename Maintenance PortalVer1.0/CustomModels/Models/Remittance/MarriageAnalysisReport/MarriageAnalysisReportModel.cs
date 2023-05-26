using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.MarriageAnalysisReport
{
    public class MarriageAnalysisReportModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int SROfficeID { get; set; }

        public int DROfficeID { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }


        public bool BRIDEPersonID { get; set; }
        public bool BRIDEGroomPersonID { get; set; }

        public bool WitnessCount { get; set; }

        public bool ReceiptCount { get; set; }



        public int startLen { get; set; }
        public int totalNum { get; set; }

        public DateTime DateTime_FromDate { get; set; }
        public DateTime DateTime_ToDate { get; set; }


    }

    public class MarriageAnalysisReportTableModel
    {
        public long srNo { get; set; }
        public string SroName { get; set; }

        public string MarriageCaseNo { get; set; }

        public Int64 RegistrationID { get; set; }

        public Int64 Bride { get; set; }

        public Int64 BrideGroom { get; set; }

        public int SROCODE { get; set; }

        public Int64 DocumentID { get; set; }

        public string BRIDEPersonID { get; set; }

        public string BRIDEGroomPersonID { get; set; }

        //public Int64 BRIDEPersonID { get; set; }

        //public Int64 BRIDEGroomPersonID { get; set; }
    }

}
