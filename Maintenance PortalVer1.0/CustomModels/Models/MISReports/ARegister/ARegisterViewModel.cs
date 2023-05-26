using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.ARegister
{
    public class ARegisterViewModel
    {
        [Display(Name = "For Date")]
        [Required(ErrorMessage = "For Date Required")]
        public String ForDate { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }

        public long UserID { get; set; }

        public int OfficeID { get; set; }

        public int RoleID { get; set; }

        public int LevelID { get; set; }

        public DateTime ForDate_DateTime { get; set; }

        public string SroName { get; set; }
    }
}
