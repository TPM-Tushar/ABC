using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.SRODocCashCollection
{
    public class SRODocCashCollectionResponseModel
    {

        public string OfficeName { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }


        

               [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int SROfficeID { get; set; }
        [Display(Name = "Nature of Document")]
        public List<SelectListItem> NatureOfDocumentList { get; set; }

        public int NatureOfDocumentID { get; set; }

        [Display(Name = "Database")]
        public List<SelectListItem> DatabaseList { get; set; }

        public int databaseID;



        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }
        public DateTime DateTime_FromDate { get; set; }
        public DateTime DateTime_ToDate { get; set; }
        public bool IsSearchValuePresent { get; set; }
        public int UserID { get; set; }

        public int DROfficeID { get; set; }

        public bool IsPdf { get; set; }
        public bool IsExcel { get; set; }
    }
}
