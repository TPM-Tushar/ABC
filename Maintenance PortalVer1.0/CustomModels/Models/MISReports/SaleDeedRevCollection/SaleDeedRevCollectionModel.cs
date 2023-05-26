using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.SaleDeedRevCollection
{
    public class SaleDeedRevCollectionModel
    {
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }
        public int DROfficeID { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }

        [Display(Name = "Financial year")]
        public List<SelectListItem> FinancialYearList { get; set; }
        public String FinacialYearID { get; set; }

        //[Display(Name = "Month")]
        //public List<SelectListItem> MonthList { get; set; }
        //public String MonthID { get; set; }

        public String ErrorMsg { get; set; }
        public int startLen { get; set; }
        public int totalNum { get; set; }
        //public List<SaleDeedRevCollectionDetail> SaleDeedRevCollectionList { get; set; }

        public bool IsPdf { get; set; }
        public bool IsExcel { get; set; }

        [Display(Name = "Month")]
        public List<SelectListItem> MonthList { get; set; }

        public int MonthID { get; set; }

        //Added By Ramank on 25-07-2019
        [Display(Name = "Property Type")]
        public List<SelectListItem> PropertyTypeList { get; set; }
        public int PropertyTypeID { get; set; }

        [Display(Name = "Build Type")]
        public List<SelectListItem> BuildTypeList { get; set; }
        public int BuildTypeID { get; set; }

        public DateTime MaxDate { get; set; }


        //Added By Ramank on 25-07-2019
        [Display(Name = "Property Value")]
        public List<SelectListItem> PropertyValueList { get; set; }
        public int PropertyValueID { get; set; }

        public string ReportInfo { get; set; }




    }

    public class SaleDeedRevCollectionDetail
    {
        public int SerialNo { get; set; }

        public String DistrictName { get; set; }

        public String SROName { get; set; }

        public int DocumentsRegistered { get; set; }

        public decimal StampDuty { get; set; }

        public decimal RegistrationFee { get; set; }

        public decimal Total { get; set; }
        //public String PDFDownloadBtn { get; set; }
        public string MonthName { get; set; }

    }
}
