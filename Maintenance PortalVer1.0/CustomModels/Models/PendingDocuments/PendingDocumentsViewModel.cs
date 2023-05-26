using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CustomModels.Models.PendingDocuments
{
    public class PendingDocumentsViewModel
    {
        //public int OfficeID { get; set; }

        //public int startLen { get; set; }
        //public int totalNum { get; set; }
        //public bool IsSearchValuePresent { get; set; }

        //public long UserID { get; set; }
        //public int FeeTypeID { get; set; }

        ////DOCUMENT NUMBER FOR VIEW
        //[Display(Name = "Document Number")]
        //[Required(ErrorMessage = "Please enter Document Number.")]
        //[RegularExpression(@"^[0-9]*$", ErrorMessage = "Please enter valid Document Number.")]
        //public long DocumentNumber { get; set; }
        //SRO List FOR VIEW
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        ////DRO List FOR VIEW
        //[Display(Name = "DRO Name")]
        //public List<SelectListItem> DROfficeList { get; set; }
        ////BookType FOR VIEW
        //[Display(Name = "BookType")]
        //public List<SelectListItem> BookType { get; set; }
        ////Financial Year for FOR VIEW
        //[Display(Name = "Financial Year")]
        //public List<SelectListItem> FinancialYear { get; set; }


        public int SROfficeID { get; set; }
        //public int DROfficeID { get; set; }
        //public int SROCode { get; set; }
        //public string FinancialYearStr { get; set; }
        //public int FinancialYearID { get; set; }
        //public int BookTypeID { get; set; }



    }

    public class PendingDocumentsUpdateModel
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


    public class PendingDocumentsTableModel
    {
        public string PendingNumber { get; set; }
        //public long DocumentID { get; set; }
        //public long PartyID { get; set; }

        public string SROCode { get; set; }
        public string PresentationDate { get; set; }
        public string DateOfPending { get; set; }
        public string SROName { get; set; }
        public string PendingReason { get; set; }
        public int SrNo { get; set; }
        public String ErrorMessage { get; set; }
        public bool IsError { get; set; }
        //public string FileNameWithoutExt { get; set; }
        //public List<PartyDetailModel> PDM { get; set; }

    }

    public class UploadDet
    {
        public string UploadPath { get; set; }
        public DateTime Date { get; set; }
    }

    public class PendingDocumentsReqModel
    {
        public int SROCode { get; set; }
        public string FinancialYearStr { get; set; }
        public int BookTypeID { get; set; }
        public long DocumentNumber { get; set; }
    }

    public class PartyDetailModel
    {
        public int SROCode { get; set; } 
        public long DocumentID { get; set; } 
        public long PartyID { get; set; } 
        public string Fname{ get; set; }
        public string Lname { get; set; }
        public string PhotoPath { get; set; }
        public string ThumbPath { get; set; }
        public string RootDirectory { get; set; }
        public bool IsPhotoUploaded { get; set; }
        public bool IsThumbUploaded { get; set; }
        public string ConvertedPhoto { get; set; }
        public string ConvertedThumb { get; set; }
        public string UploadDatePhoto { get; set; }
        public string UploadDateThumb { get; set; }
        public int SrNo { get; set; }
    }

    //public class IsVillageMatchingViewTableModel
    //{
    //    public int SrNo { get; set; }
    //    public int SROCode { get; set; }
    //    public string SROName { get; set; }
    //    public string IsVillageMatching { get; set; }

    //}



}
