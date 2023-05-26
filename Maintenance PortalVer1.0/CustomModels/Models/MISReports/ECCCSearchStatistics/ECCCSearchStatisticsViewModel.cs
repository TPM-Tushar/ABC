using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ECCCSearchStatistics
{
    public class ECCCSearchStatisticsViewModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROCode { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int DROCode { get; set; }

        public string SroName { get; set; }

        public string DroName { get; set; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinancialYearList { get; set; }

        public int FinancialyearCode { get; set; }

        public string FinancialYearName { get; set; }

        [Display(Name = "Month")]
        public List<SelectListItem> MonthList { get; set; }

        public int MonthCode { get; set; }

        public string MonthName { get; set; }

        public bool IsSearchOfficeWise { get; set; }
        public bool IsSearchDurationWise { get; set; }

        public SearchBy SearchBy { get; set; }

        public string TotalUserLoggedIn { get; set; }
        public string TotalECSearched { get; set; }
        public string TotalECSubmitted { get; set; }
        public string TotalECSigned { get; set; }
        public string TotalCCSearched { get; set; }
        public string TotalCCSubmitted { get; set; }
        public string TotalCCSigned { get; set; }

        public string TotalAnywhereEC { get; set; }
        public string TotalAnywhereCC { get; set; }
        public string TotalLocalEC { get; set; }
        public string TotalLocalCC { get; set; }
    }
    public enum SearchBy
    {
        SearchOfficeWise=1,
        SearchDurationWise=2

    }
}
