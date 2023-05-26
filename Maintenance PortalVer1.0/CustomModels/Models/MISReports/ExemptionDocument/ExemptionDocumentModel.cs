using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ExemptionDocument
{
    public class ExemptionDocumentModel
    {
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

    }
    public class ExemptionDocumentSummary
    {
        public int SerialNo { get; set; }
        public String JurisdictionalOffice { get; set; }
        public String SROName { get; set; }
        public int Documents { get; set; }

        //  BELOW ADDED BY SHUBHAM on 05-09-2019
        public decimal STAMPDUTY_BEFORE_EXEMPTION { get; set; }

        public decimal EXEMPTION_GIVEN { get; set; }

        public decimal STAMPDUTY_AFTER_EXEMPTION { get; set; }

        // ABOVE ADDED BY SHUBHAM on 05-09-2019
        public decimal RegistrationFees { get; set; }
        public decimal Total { get; set; }
    }

    public class ExemptionDocumentDetail
    {
        public int SerialNo { get; set; }
        public String JurisdictionalOffice { get; set; }
        public String SROName { get; set; }
        public string FinalRegistrationNumber { get; set; }
        //  BELOW ADDED BY SHUBHAM on 05-09-2019
        public decimal STAMPDUTY_BEFORE_EXEMPTION { get; set; }

        public decimal EXEMPTION_GIVEN { get; set; }

        public decimal STAMPDUTY_AFTER_EXEMPTION { get; set; }

        // ABOVE ADDED BY SHUBHAM on 05-09-2019
        public decimal RegistrationFees { get; set; }
        public decimal Total { get; set; }
    }

    public class ExemptionDocumentDetailWrapper
    {
        public List<ExemptionDocumentDetail> ExemptionDocumentDetailList { get; set; }
    }
}
