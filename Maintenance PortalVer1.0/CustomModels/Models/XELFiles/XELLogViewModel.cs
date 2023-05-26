using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.XELFiles
{
    public class XELLogViewModel
    {
        public List<XELLogDetailsModel> XELLogDetailsModelList { get; set; }

        [Display(Name = "Table")]
        public List<SelectListItem> TableNameList { get; set; }

        public int TableID { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }
        public DateTime? dtToDate { get; set; }
        public DateTime? dtFromDate { get; set; }
        public string OfficeType { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }
    }


}
