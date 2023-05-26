using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.KOSPaymentStatusReport
{
    public class KOSPaymentStatusRptViewModel
    {

        [Display(Name = "Application Type")]

        public List<SelectListItem> ApplicationTypeList { get; set; }

        public int ApplicationTypeId { get; set; }


        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsExcel { get; set; }


        [Display(Name = "From Date ")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date Required")]
        public String ToDate { get; set; }

        public int status { get; set; }

        public int paymentPendingsince { get; set; }

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-12-2020 
        // "Payment Pending Since" and  "Longest Payment Pending Since" days in variable
        public String Days { get; set; }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-12-2020 

        //Added by mayank on 15-07-2021 for SR and DR specific
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int DROfficeID { get; set; }

        public int SROfficeID { get; set; }
        //end

    }


}