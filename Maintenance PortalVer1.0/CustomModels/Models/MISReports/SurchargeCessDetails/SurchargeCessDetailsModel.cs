using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.SurchargeCessDetails
{
    public class SurchargeCessDetailsModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public bool IsExcel { get; set; }         public String SearchValue { get; set; }

        public int SROfficeID { get; set; }
        [Display(Name = "Nature of Document")]
        public List<SelectListItem> NatureOfDocumentList { get; set; }

        public int NatureOfDocumentID { get; set; }
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }
        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public DateTime DateTime_FromDate { get; set; }
        public DateTime DateTime_ToDate { get; set; }

        //[Required(ErrorMessage = "DR OfficeID is required")]
        public int DROfficeID { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }
    }
    public class SurchargeCessDetail
    {
        public int SerialNo { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public string PropertyDetails { get; set; }
        public string VillageNameE { get; set; }
        public string Executant { get; set; }
        public string Claimant { get; set; }
        public decimal PropertyValue { get; set; }
        public decimal GovtDuty { get; set; }
        public decimal TWOPERCENT_GOVTDUTY { get; set; }
        public decimal THREEPERCENT_GOVTDUTY { get; set; }
        public decimal CessDuty { get; set; }
        public decimal TotalStumpDuty { get; set; }
        public decimal PaidStumpDuty { get; set; }
        public string RegisteredDatetime { get; set; }
        public string ArticleNameE { get; set; }
        public string SroName { get; set; }
        public decimal Total { get; set; }
    }

    public class SurchargeCessDetailWrapper
    {
        public int TotalRecords { get; set; }
        public List<SurchargeCessDetail> SurchargeCessDetailList { get; set; }
        public decimal TotalOfTotal { get; set; }
        public decimal Total_GovtDuty { get; set; }
        public decimal Total_THREEPERCENT_GOVTDUTY { get; set; }
        public decimal Total_TWOPERCENT_GOVTDUTY { get; set; }

        public decimal Total_CessDuty { get; set; }
        public decimal Total_TotalStumpDuty { get; set; }
        public decimal Total_PaidStumpDuty { get; set; }
        public decimal Total_PropertyValue { get; set; }

    }
}

