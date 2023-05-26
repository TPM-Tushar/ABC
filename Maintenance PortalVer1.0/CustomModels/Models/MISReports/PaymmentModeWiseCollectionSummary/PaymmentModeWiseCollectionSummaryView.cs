using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.PaymmentModeWiseCollectionSummary
{
    public class PaymmentModeWiseCollectionSummaryView
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
        public bool IsExcel { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DistrictList { get; set; }

        public int PaymentModeID { get; set; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinYearList { get; set; }

        public int FinYearID { get; set; }

        [Display(Name = "Payment Mode")]
        public List<SelectListItem> PaymentModeList { get; set; }

        public int DistrictID { get; set; }
        [Display(Name = "Receipt Type")]
        public List<SelectListItem> ReceiptTypeList { get; set; }

        public int ReceiptTypeID { get; set; }

        public String SearchValue { get; set; }
    }
}
