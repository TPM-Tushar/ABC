using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CustomModels.Models.PhotoThumb
{
    public class PhotoThumbFailedViewModel
    {
        public int OfficeID { get; set; }

        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsSearchValuePresent { get; set; }

        public long UserID { get; set; }
        public int FeeTypeID { get; set; }


        //SRO List FOR VIEW
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        //DRO List FOR VIEW
        [Display(Name = "DRO Name")]
        public List<SelectListItem> DROfficeList { get; set; }



        public int SROfficeID { get; set; }
        public int DROfficeID { get; set; }
        public int SROCode { get; set; }
        public string FinancialYearStr { get; set; }
        public int FinancialYearID { get; set; }
        public int BookTypeID { get; set; }



    }

    public class PhotoThumbFailedUpdateModel
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

    public class PhotoThumbFailedListModel
    {
        public long ID { get; set; }
        public long LogID { get; set; }
        public long PartyID { get; set; }
        public bool IsPhoto { get; set; }
        public bool IsThumb { get; set; }
        public int SROCode { get; set; }
        public int SNo { get; set; }
        public string ErrorMessage { get; set; }
        public string CDNumber { get; set; }
        public string PartyName { get; set; }
        public string Date { get; set; }
        public string FRN { get; set; }

    }

    public class PhotoThumbFailedTableModel
    {
        public string ExError { get; set; }
        public bool IsError { get; set; }
        public string FileNameWithoutExt { get; set; }
        public string SROName { get; set; }
        public List<PhotoThumbFailedListModel> FailedList { get; set; }
        public List<PhotoThumbFailedListModel> NonDistinctFailedList { get; set; }

    }

    public class PhotoThumbFailedReqModel
    {
        public int SROCode { get; set; }
        public bool IsPhoto{ get; set; }
        public bool IsThumb { get; set; }
        public long PartyID { get; set; }
    }


}
