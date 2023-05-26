using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.OtherDepartmentImport
{
    public class OtherDepartmentImportREQModel
    {
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

        [Display(Name = "Report Name")]
        public List<SelectListItem> ReportList { get; set; }

        public String ReportName { get; set; }

        public String XMLType { get; set; }

        public int XMLLogID { get; set; }

        public int XMLSROCODE { get; set; }


    }
    public class OtherDepartmentImportWrapper
    {
        public List<OtherDepartmentImportDetail> OtherDepartmentImportDetailList { get; set; }

        public String SelectedSRO { get; set; }

        public String SelectedDRO { get; set; }

        public String ReportName { get; set; }

    }

    public class OtherDepartmentImportDetail
    {
        public int SerialNumber { get; set; }


        public String OfficeName { get; set; }


        public String FinalRegistrationNumber { get; set; }


        public String PropertyID { get; set; }



        public String ImportedXML { get; set; }

        public String ExportedXML { get; set; }


        public String ReferenceNumber { get; set; }


        public String UploadDate { get; set; }


        public String DateofRegistration { get; set; }


        public String SketchNumber { get; set; }

        //public String WhetherDocumentRegistered { get; set; }
       

        //Added by mayank on 02/09/2021 for Kaveri-FRUITS Integration
        public string ArticleNameE{ get; set; }
        public string ActionDate{ get; set; }
        public string BtnViewSummary{ get; set; }
        //Added by mayank on 05/01/2022
        public string KaigrRegInsertDate { get; set; }

    }
}
