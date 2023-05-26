using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.OtherDepartmentStatus
{
    public class OtherDepartmentStatusModel
    {
        [Display(Name = "Integration type")]
        public List<SelectListItem> IntegrationtypeList { get; set; }
        public int IntegrationtypeID { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }

        [Display(Name = "New Method")]
        public bool IsActive { get; set; }

        [Display(Name = "From Date")]
        public string FromDateString { get; set; }

        public DateTime FromDate { set; get; }

        [Display(Name = "To Date")]
        public string ToDateString { get; set; }

        public DateTime ToDate { get; set; }

        public int startLen { get; set; }

        public int totalNum { get; set; }
    }

    public class OtherDepartmentStatusDetailsModel
    {
        public int SerialNo { get; set; }
        public String Column1 { get; set; }
        public String Column2 { get; set; }
        public String Column3 { get; set; }
        public String Column4 { get; set; }
        public String Column5 { get; set; }
        public String Column6 { get; set; }
        public String Column7 { get; set; }
    }
}
