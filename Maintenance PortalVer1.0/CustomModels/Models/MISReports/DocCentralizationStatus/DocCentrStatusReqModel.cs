using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.DocCentralizationStatus
{
    public class DocCentrStatusReqModel
    {
        public int OfficeID { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROID { get; set; }
        public string Date { get; set; }
    }
}
