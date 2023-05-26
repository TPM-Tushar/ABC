using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.SupportEnclosure
{
    public class SupportEnclosureDetailsViewModel
    {

        public string OfficeName { get; set; }
        [Display(Name = "Database")]
        public List<SelectListItem> DatabaseList { get; set; }
        public int databaseID;
        public int startLen { get; set; }
        public int totalNum { get; set; }
        public bool IsSearchValuePresent { get; set; }
        public int UserID { get; set; }
        public bool IsPdf { get; set; }
        public bool IsExcel { get; set; }

        [Required(ErrorMessage = "DR OfficeID is required")]
        public int DROfficeID { get; set; }
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }
        public int FeeTypeID { get; set; }
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }
        [Display(Name = "BookType")]
        public List<SelectListItem> BookType { get; set; }

        [Display(Name = "Financial Year")]
        public List<SelectListItem> FinancialYear { get; set; }

        [Display(Name = "Document Number")]
        [Required(ErrorMessage = "Please enter Document Number.")]
        [RegularExpression(@"^[0-9]*$", ErrorMessage = "Please enter valid Document Number.")]
        public long? DocumentNumber { get; set; }

        public int BookTypeID { get; set; }

        public string FinancialYearStr { get; set; }

        public int FinancialYearID { get; set; }

    }
}
