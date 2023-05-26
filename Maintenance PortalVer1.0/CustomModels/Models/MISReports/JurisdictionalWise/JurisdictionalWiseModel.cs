using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.JurisdictionalWise
{
    public class JurisdictionalWiseModel
    {
        //public int DROfficeID { get; set; }
        public bool IsExcel { get; set; }
        public String SearchValue { get; set; }

        //[Display(Name = "District")]
        //public List<SelectListItem> DROfficeList { get; set; }

        public int SROfficeID { get; set; }

        [Display(Name = "Jurisdictional Office")]
        public List<SelectListItem> SROfficeList { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public int StartLen { get; set; }

        public int TotalNum { get; set; }

        public DateTime DateTime_FromDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }
        public DateTime MaxDate { get; set; }

        public string ReportInfo { get; set; }


    }

    public class JurisdictionalWiseSummary
    {
        public int SerialNo { get; set; }
        public String JurisdictionalOffice { get; set; }
        public String SROName { get; set; }
        public int Documents { get; set; }
        public decimal StumpDuty { get; set; }
        public decimal RegistrationFees { get; set; }
        public decimal Total { get; set; }

    }

    public class JurisdictionalWiseDetail
    {
        public int SerialNo { get; set; }
        public String JurisdictionalOffice { get; set; }
        public String SROName { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public decimal StumpDuty { get; set; }
        public decimal RegistrationFees { get; set; }
        public decimal Total { get; set; }

    }

    public class JurisdictionalWiseDetailWrapper
    {
        public List<JurisdictionalWiseDetail> JurisdictionalWiseDetailList { get; set; }
        public int TotalRecords { get; set; }
    }
}