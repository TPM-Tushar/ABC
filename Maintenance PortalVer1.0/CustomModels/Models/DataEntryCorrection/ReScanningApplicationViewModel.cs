using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace CustomModels.Models.DataEntryCorrection
{
    public class ReScanningApplicationViewModel
    {
        public int OfficeID { get; set; }
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsSearchValuePresent { get; set; }
        public long UserID { get; set; }
        public int FeeTypeID { get; set; }
        public int SROfficeID { get; set; }
        public int DROfficeID { get; set; }
        public int SROCode { get; set; }
        public int FinancialYearID { get; set; }
        public int BookTypeID { get; set; }
        public string FinancialYearStr { get; set; }
        public string BookTypeStr { get; set; }


        //DOCUMENT NUMBER FOR VIEW
        [Display(Name = "Document Number")]
        [Required(ErrorMessage = "Please enter Document Number.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Please enter valid Document Number.")]
        public long DocumentNumber { get; set; }


        //Order NUMBER FOR VIEW
        [Display(Name = "Order Number")]
        [Required(ErrorMessage = "Please enter Order Number.")]
        public string OrderNo { get; set; }


        //SRO List FOR VIEW
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        //DRO List FOR VIEW
        [Display(Name = "DRO Name")]
        public List<SelectListItem> DROfficeList { get; set; }


        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }


        [Display(Name = "BookType")]
        public List<SelectListItem> BookType { get; set; }
        //Financial Year for FOR VIEW
        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinancialYear { get; set; }




    }

    public class ReScanningApplicationReqModel
    {
        public int SROCode { get; set; }
        public int DROCode { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string OrderNo { get; set; }
        public string Date { get; set; }
        public string TriLetter { get; set; }
        public string DocNo { get; set; }
        public string DType { get; set; }
        public string IPAddress { get; set; }
        public long DocID { get; set; }
        public long UserID { get; set; }
        public int isUploaded { get; set; }
        public int NPC { get; set; }
        public string FinancialYearStr { get; set; }
        public string BookTypeStr { get; set; }
        //Added by madhur on 24-08-2022
        public bool? isMissingDocument { get; set; }
        //end


    }

    public class ReScanningApplicationResModel
    {
        public int SROCode { get; set; }
        public int DROCode { get; set; }
        public HttpPostedFileBase file { get; set; }
        public DataTable ExcelTable { get; set; }
        public string OrderNo { get; set; }
        public string Date { get; set; }
        public string DocNo { get; set; }
        public long DocID { get; set; }
        public string TriLetter { get; set; }
        public string DType { get; set; }
        public int isSuccess { get; set; }
    }


    public class DetailModel
    {
        public string TriLetter { get; set; }
        //public string FRN { get; set; }
        public long DocID { get; set; }

    }


    public class PhotoThumbTableModel
    {
        //public int SROCode { get; set; }
        //public long DocumentID { get; set; }
        //public long PartyID { get; set; }

        //public string Fname { get; set; }
        //public string Lname { get; set; }
        //public string PhotoPath { get; set; }
        //public string ThumbPath { get; set; }
        public String ErrorMessage { get; set; }
        public bool IsError { get; set; }
        public string FileNameWithoutExt { get; set; }
        public string SROName { get; set; }
        public List<PartyDetailModel> PDM { get; set; }

    }

    public class UploadDet
    {
        public string UploadPath { get; set; }
        public DateTime Date { get; set; }
    }

    public class PhotoThumbReqModel
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
        public string Fname { get; set; }
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


    public class ReScanningApplicationOrderTableModel
    {
        public string Select { get; set; }
        public int DocTypeID { get; set; }
        public string SROName { get; set; }
        public string DROrderNumber { get; set; }
        public string OrderDate { get; set; }
        public string EntryDate { get; set; }
        public string RegistrationNumber { get; set; }
        public string AbsoluteFilePath { get; set; }
        public string FileName { get; set; }
        public string SNo { get; set; }
        public string ViewBtn { get; set; }
        public string Action { get; set; }
        public string EnteredBY { get; set; }
        public string Status { get; set; }
        public string DistrictName { get; set; }
        public bool? IsActive { get; set; }
        public bool? isReScanCompleted { get; set; }
    }

}
