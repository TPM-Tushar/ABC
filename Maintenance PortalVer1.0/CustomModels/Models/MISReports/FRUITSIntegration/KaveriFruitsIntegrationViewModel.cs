using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.FRUITSIntegration
{
    public class KaveriFruitsIntegrationViewModel
    {
        [Display(Name = "District")]
        public List<SelectListItem> DistrictList { get; set; }
        public int DistrictID { get; set; }

        // For Child table popup
        public String ColumnName { get; set; }
        public int StartLen { get; set; }
        public int TotalNum { get; set; }

        public int SROCode { get; set; }

        public bool IsForExcelDownload { get; set; }
        public bool IsForSearch { get; set; }
        
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }

        public string ReferenceNo { get; set; }

        public byte[] Form3 { get; set; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinancialYearList { get; set; }

        public int FinancialyearCode { get; set; }

        [Display(Name = "Month")]
        public List<SelectListItem> MonthList { get; set; }

        public int MonthCode { get; set; }

    }
}
