using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ARegisterGenerationDetails
{
    public class ARegisterGenerationDetailsModel
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

        public int OfficeID { get; set; }

        public bool NGFile { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }

        public DateTime DateTime_FromDate { get; set; }
        public DateTime DateTime_ToDate { get; set; }

     
    }

    public class ARegisterGenerationDetailsTableModel
    {
        public long srNo { get; set; }
        public string SroName { get; set; }

        public string DistrictName { get; set; }

        public string File_Path { get; set; }

        public string Receipt_Date { get; set; }

        public string File_Gen_Date { get; set; }

        public string IsReceiptsSynchronized { get; set; }
    }
}
