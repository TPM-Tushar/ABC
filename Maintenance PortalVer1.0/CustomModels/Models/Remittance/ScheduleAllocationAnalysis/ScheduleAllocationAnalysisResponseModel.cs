using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.ScheduleAllocationAnalysis
{
    public class ScheduleAllocationAnalysisResponseModel
    {

        public int DROfficeListID { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int SROfficeListID { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        
        [Display(Name = "Year")]
        public List<SelectListItem> YearList { get; set; }

        public string SROName { get; set; }
        public string DROName { get; set; }

        public string Year { get; set; }

        //[Required(ErrorMessage = "Please select the Challan Purpose")]
        [Display(Name = "Registration Article ")]
        public int[] RegArticleId { get; set; }

        public List<SelectListItem> RegArticleList { get; set; }

        public string RegistrationArticle { get; set; }


        public bool IsSelectAll { get; set; }



        public int startLen { get; set; }

        public int totalNum { get; set; }


        public string YearName { get; set; }




        public int YearListID { get; set; }

        public string FinalRegistrationNumber { get; set; }
        public string Stamp5DateTime { get; set; }

        public bool IsPdf { get; set; }

        public bool IsExcel { get; set; }

        [Display(Name ="Party Id")]
        public bool IsPartyIdCheckBoxSelected { get; set; }

        //Added By ShivamB on 30-09-2022 for adding All options in District DropDown and SRO Dropdown parameter.
        public int [] SROList { get; set; }
        public bool IsThroughVerifyCheckBoxSelected { get; set; }
        public bool IsSelectAllYearSelected { get; set; }
        //Added By ShivamB on 30-09-2022 for adding All options in District DropDown and SRO Dropdown parameter.


    }
}
