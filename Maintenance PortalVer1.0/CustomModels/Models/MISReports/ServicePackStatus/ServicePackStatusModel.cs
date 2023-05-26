using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ServicePackStatus
{
    public class ServicePackStatusModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }

        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }
        public int DROfficeID { get; set; }

        //[Display(Name = "Service Pack")]
        //public List<SelectListItem> ServicePackList { get; set; }
        //public int ServicePackID { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }

        //[Display(Name = "Status")]
        //public List<SelectListItem> StatusList { get; set; }
        //public int StatusID { get; set; }

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 07-09-2020

        [Display(Name = "Release Type")]
        public List<SelectListItem> SoftwareReleaseTypeList { get; set; }
        public int SoftwareReleaseTypeID { get; set; }

        [Display(Name = "Change Type")]
        public List<SelectListItem> ServicePackChangeTypeList { get; set; }
        public int ServicePackChangeTypeID { get; set; }

        [Display(Name = "Released Status")]
        public List<SelectListItem> ReleasedStatusList { get; set; }
        public int ReleasedStatusID { get; set; }

        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 07-09-2020


        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 17-09-2020 
        public String IsSRDRFlag { get; set; }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 17-09-2020
    }
    public class ServicePackStatusDetails
    {
        public int SerialNo { get; set; }

        public String DistrictName { get; set; }

        public String SROName { get; set; }
        public String SoftwareReleaseType { get; set; }
        public String ReleaseMode { get; set; }
        public String Major { get; set; }
        public String Minor { get; set; }

        public String Description { get; set; }

        public String InstallationProcedure { get; set; }
        public String ChangeType { get; set; }
        public String Status { get; set; }
        public String ReleaseInstruction { get; set; }

        public String AddedDate { get; set; }
        public String ReleaseDate { get; set; }
    }
}
