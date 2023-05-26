using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Dashboard
{
    public class DashboardDetailsViewModel
    {
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }
        public int DROfficeID { get; set; }

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }
        [Display(Name = "Nature of Document")]

        public List<SelectListItem> NatuereOfDocsList { get; set; }

        public int[] NatureOfDocID { get; set; }
        public String SNatureOfDocID { get; set; }
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public String SearchValue { get; set; }
        public int toggleBtnId { get; set; }
        public List<DashboardTileModel> Tiles { get; set; }
        public string TargetAchieved { get; set; }
        public String selectedType { get; set; }
        public int DistrictCode { get; set; }

        //Added By Raman Kalegaonkar on 17-06-2020
        [Display(Name = "Fin. Year")]
        public List<SelectListItem> FinYearList { get; set; }
        public int FinYearId { get; set; }

        //ADDED BY PANKAJ SAKHARE ON 18-09-2020
        public bool IsForExcel { get; set; }
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020        
        public String FinYear { get; set; }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 14-10-2020
    }
    #region ADDED BY SHUBHAM BHAGAT 09-04-2020
    public class NatureOfArticle_REQ_RES_Model
    {
        public List<SelectListItem> NatuereOfDocsList { get; set; }

        public String RadioType { get; set; }

        public int[] NatureOfDocID { get; set; }
    }
    #endregion
}
