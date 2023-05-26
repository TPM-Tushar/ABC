using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.DocumentReferences
{
    public class DocumentReferencesREQModel
    {
        public bool IsExcel { get; set; }         public String SearchValue { get; set; }
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_FromDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }

        public int startLen { get; set; }

        public int totalNum { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }


        [Display(Name = "District")]
        public List<SelectListItem> DistrictList { get; set; }

        public int DistrictID { get; set; }

        public String OfficeType { get; set; }

    }

    public class DocumentReferencesWrapper
    {
        public List<DocumentReferencesDetail> DocumentReferencesDetailList { get; set; }

        public String SelectedSRO { get; set; }

        public String SelectedDRO { get; set; }
        public int TotalRecords { get; set; }

    }
    public class DocumentReferencesDetail
    {
        public int SerialNumber { get; set; }

        public String OfficeName { get; set; }

        public String DocumentType { get; set; }

        public String FinalRegistrationNumber { get; set; }

        public String ReferenceText { get; set; }

        public String RevenueOfficerNo_CourtNo { get; set; }

        public String RevenueOfficerDate_CourtOrderDate { get; set; }

        public String Date_of_Registration { get; set; }

        public String ThroghType { get; set; }



    }

}
