using CustomModels.Models.Remittance.MasterData;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CustomModels.Models.MISReports.SevaSidhuStatistics
{
    public class SevaSindhuStatisticsReportModel
    {
        public string OfficeName { get; set; }

        [Display(Name = "SRO")]
        public List<SelectListItem> SROfficeList { get; set; }
        [Display(Name = "Article")]
        public List<SelectListItem> ArticleNameList { get; set; }
        public int SROfficeID { get; set; }
        public int ArticleID { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }
        public int DROfficeID { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }

        [Display(Name = "From Date")]
        public string fromDate { get; set; }

        public DateTime fromDateTime { set; get; }

        public DateTime ToDate { get; set; }
        [Display(Name = "To Date")]

        public string ToDate_Str { get; set; }

        public string SroName { get; set; }
        public string DroName { get; set; }


        [Display(Name = "Year")]
        public List<SelectListItem> YearDropdown { get; set; }
        public int selectedYear { get; set; }
        [Display(Name = "Month")]
        public List<SelectListItem> MonthList { get; set; }
        public int selectedMonth { get; set; }

        public bool IsPDF { get; set; }
        public bool IsExcel { get; set; }

        
        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinYearList { get; set; }
        public int finSelectedYear { get; set; }
        public DateTime maxDate { get; set; }
        public string ReportInfo { get; set; }
        public bool isExcelDownload { get; set; }

    }


    public class SevaSindhuStatisticsReportResModel
    {
        public List<SevaSindhuStatisticsReportDetailModel> TSevaSindhuStatisticsReportDetailModelList { get; set; }
    }
}
