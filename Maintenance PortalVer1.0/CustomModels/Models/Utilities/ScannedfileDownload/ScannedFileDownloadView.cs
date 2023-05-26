using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Utilities.ScannedfileDownload
{
    public class ScannedFileDownloadView
    {
        public List<ScannedFileLogTableModel> ScannedFileDownloadList { get; set; }

        [Required(ErrorMessage = "DR OfficeID is required")]
        public int DROfficeID { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }
        public int FeeTypeID { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }
        [Display(Name = "Book Type")]
        public List<SelectListItem> BookType { get; set; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinancialYear { get; set; }

        [Display(Name = "Document Number")]
        [Required(ErrorMessage = "Please enter Document Number.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Please enter valid Document Number.")]
        public long DocumentNumber { get; set; }

        public int BookTypeID { get; set; }

        public string FinancialYearStr { get; set; }

        public int FinancialYearID { get; set; }

        [Display(Name = "Download Reason")]

        public string DownloadReason { get; set; }
        public long UserID { get; set; }

        public Byte[] ByteArray { get; set; }

        public string DType { get; set; }
        //Added By Tushar on 23 march 2022
        public List<SelectListItem> ReasonDetaills { get; set; }

        public int ReasonID { get; set; }

        public List<SelectListItem> DocumentType { get; set; }

        [Display(Name = "Document Type")]
        public int DocumentTypeID { get; set; }

        //End By Tushar on 23 March 2022

        #region Added by mayank on 24Mar2022
        [Display(Name = "Marriage Type")]
        public List<SelectListItem> MarriageType { get; set; }

        public int MarriageTypeID { get; set; } 
        #endregion
        #region Added By Tushar on 1April 2022 for add Document Type Notice
        [Display(Name = "Notice Type")]
        public List<SelectListItem> NoticeType { get; set; }

        public int NoticeTypeListID { get; set; }
        #endregion
    }

    public class ScannedFileLogTableModel
    {
        public int SrNo { get; set; }
        public string FRN { get; set; }
        public string SroName { get; set; }
        public string FileName { get; set; }
        public string Filepath { get; set; }
        public string DownloadedBY { get; set; }
        public string DownloadReason { get; set; }
        public string DownloadDateTime { get; set; }

    }
}
