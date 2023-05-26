using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ESignConsumptionReport
{
    public class ESignConsumptionReportViewModel
    {

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Display(Name = "To Date")]
        [Required(ErrorMessage = "To Date Required")]
        public String ToDate { get; set; }


        //-----

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinancialYearList { get; set; }

        [Required(ErrorMessage = "Please select financial year")]
        public int FinancialYearCode { get; set; }

        public string FinancialYearName { get; set; }

        [Display(Name = "Month")]
        public List<SelectListItem> MonthList { get; set; }

        public int MonthCode { get; set; }

        public string MonthName { get; set; }

        public int StartLength { get; set; }

        public int TotalNum { get; set; }


        //public short CurrentRoleID { get; set; }
        //-----



        [Display(Name = "Application Status Type")]
        public List<SelectListItem> ApplicationStatusTypeList { get; set; }

        public int ApplicationStatusTypeID { get; set; }

        public bool IsExcel { get; set; }

        public bool ViewESignDetailsDataTable { get; set; }     //This property is set to show eSignDetails Datatable to TechAdmin login only
    }
}
