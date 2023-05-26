using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CustomModels.Models.BhoomiMapping
{
    public class BhoomiMappingViewModel
    {
        public int OfficeID { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsSearchValuePresent { get; set; }

        public long UserID { get; set; }

        public int FeeTypeID { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }

        public int SROCode { get; set; }


        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeOrderList { get; set; }

        public int SROOrderCode { get; set; }

        [Display(Name = "Upload Excel")]
        public HttpPostedFileBase ExcelInput { get; set; }
    }

    public class BhoomiMappingUpdateModel
    {

        public int SROCode { get; set; }
        public HttpPostedFileBase file { get; set; }
        public DataTable ExcelTable { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public string Extension { get; set; }
        public int KaveriSROCode { get; set; }
        public int KaveriVillageCode { get; set; }
        public int BhoomiDistrictCode { get; set; }
        public int BhoomiTalukCode { get; set; }
        public int BhoomiHobiCode { get; set; }
        public int BhoomiVillageCode { get; set; }
    }


    public class BhoomiMappingTableModel
    {
        public int SrNo { get; set; }
        public string FileName { get; set; }
        public int SROCode { get; set; }
        public int KaveriVillageCode { get; set; }
        public int KaveriSROCode { get; set; }
        public int BhoomiDistrictCode { get; set; }
        public int BhoomiTalukCode { get; set; }
        public int BhoomiHobiCode { get; set; }
        public int KaveriHobiCode { get; set; }
        public int BhoomiVillageCode { get; set; }
        public string DistrictName { get; set; }
        public string KaveriSROName { get; set; }
        public string KaveriVillageName { get; set; }
        public string KaveriHobiName { get; set; }
        public string BhoomiTalukName { get; set; }
        public string BhoomiHobiName { get; set; }
        public string BhoomiVillageName { get; set; }
    }

    public class IsVillageMatchingViewTableModel
    {
        public int SrNo { get; set; }
        public int SROCode { get; set; }
        public string SROName { get; set; }
        public string IsVillageMatching { get; set; }
 
    }



}
