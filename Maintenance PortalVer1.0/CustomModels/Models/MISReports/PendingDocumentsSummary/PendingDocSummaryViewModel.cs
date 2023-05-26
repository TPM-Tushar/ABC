using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.PendingDocumentsSummary
{
    public class PendingDocSummaryViewModel
    {
        [Display(Name = "From Date (Presentation Date)")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date (Presentation Date)")]
        public String ToDate { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }
        public string SROIDEncrypted { get; set; }
        public bool IsExcel { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DistrictList { get; set; }

        public int DistrictID { get; set; }
        public String SearchValue { get; set; }
        public String ColumnName { get; set; }
        public String EncryptedSROCode { get; set; }

        public String ENCSROName { get; set; }

        public String ENCDistrictName { get; set; }


     

    }
    public class PendingDocsDatatableRecord
    {
        public int SerialNo { get; set; }
        public string District{ get; set; }

        public string SRO { get; set; }
        public int NoOfDocsPresented{ get; set; }
        public int NoOfDocsRegistered { get; set; }

        public int NoOfDocsKeptPending { get; set; }


        // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
        //public int DocsNotRegdNotPending { get; set; }
        public int NoOfPendingLaterFinalizedDocs{ get; set; }
        public string Str_Number_of_Pending_later_finalized_Docs { get; set; }

        
        // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021

        public string Str_NoOfDocsKeptPending { get; set; }

        // BELOW CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021
        //public string Str_DocsNotRegdNotPending { get; set; }
        // ABOVE CODE IS COMMENTED AND CHANGED BY SHUBHAM BHAGAT ON 23-03-2021




    }

}
