using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ScannedFileUploadStatusReport
{
    public class ScannedFileUploadStatusRptReqModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid SRO")]
        [Required]
        public int SROfficeID { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        [RegularExpression("^[0-9]*$", ErrorMessage = "Invalid District")]
        [Required]
        public int DROfficeID { get; set; }
        public bool IsDrLogin { get; set; }
        public bool IsSrLogin { get; set; }
        public string OfficeType { get; set; }

        //Added by Madhusoodan on 28-04-2020
        public List<SelectListItem> DocumentType { get; set; }
        [Display(Name = "Registration Type")]
        public int DocumentTypeID { get; set; }
    }
}
