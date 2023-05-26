using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.IncomeTaxReport
{
    public class IncomeTaxReportResponseModel
    {
        //public long ReportSrNo { get; set; }
        //public long OriginalReportSrNo { get; set; }
        //public long CustomerId { get; set; }
        //public string PersonName { get; set; }
        //public string DateOfBirth { get; set; }
        //public string FathersName { get; set; }
        //public string PanAckNo { get; set; }
        //public string AadharNo { get; set; }
        //public string IdentificationType { get; set; }
        //public string IdentificationNumber { get; set; }
        //public string FlatDoorBuilding { get; set; }
        //public string NameOfPremises { get; set; }
        //public string RoadStreet { get; set; }
        //public string AreaLocality { get; set; }
        //public string CityTown { get; set; }
        //public string PostalCode { get; set; }
        //public string StateCode { get; set; }
        //public int CountryCode { get; set; }
        //public string MobileNo { get; set; }
        //public string StdCode { get; set; }
        //public string TelephoneNo { get; set; }
        //public decimal EstimatedAgriIncome { get; set; }
        //public decimal EstimatedNonAgriIncome { get; set; }
        //public string Remarks { get; set; }
        //public string Form60AckNo { get; set; }
        //public string TransactionDate { get; set; }
        //public string TransactionID { get; set; }
        //public string TransactionType { get; set; }
        //public decimal TransactionAmount { get; set; }
        //public string TransactionMode { get; set; }
        
        public int SROfficeListID { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int DROfficeListID { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinYearList { get; set; }

        public int FinYearListID { get; set; }

        public int startLen { get; set; }

        public int totalNum { get; set; }

        public bool IsPdf { get; set; }
        public bool IsExcel { get; set; }

        public bool isClickedOnSearchBtn { get; set; }

        public string SROName { get; set; }
        public string DROName { get; set; }

        public string FinYearName { get; set; }

        public List<IncomeTaxReportDetailsModel> incomeTaxReportDetailsList { get; set; }
    }
    
}
