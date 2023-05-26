using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.DataEntryCorrection
{
    public class DataEntryCorrectionOrderViewModel
    {
        [Display(Name = "Order No.")]
        [Required(ErrorMessage = "Please enter Order Number.")]
        [RegularExpression(@"^[\u0C80-\u0CFFa-zA-Z0-9_\/\\\-()\[\]%&*. :;#]*$", ErrorMessage = "Please enter valid Order Number.")]
        [StringLength(100)]
        public string OrderNo { get; set; }

        [Display(Name = "Order Date")]
        public string OrderDate { get; set; }

        //[Display(Name = "Correction Note")]
        //public string OrderNote { get; set; }

        [Display(Name = "Upload DR Order PDF File")]
        public string OrderNoteFile { get; set; }

        public int OrderID { get; set; }
        public int SROCode { get; set; }
        public long DocumentID { get; set; }
        public long PropertyID { get; set; }
        public string IPAddress { get; set; }

        //public string EncryptedPropertyID { get; set; }   //Not in use
        //public string EncryptedDocumentID { get; set; }  //Not in use
        public string FilePath { get; set; }
        public bool IsInsertedSuccessfully { get; set; }
        public long UserID { get; set; }

        public int OfficeID { get; set; }
        
        //Added by Madhusoodan on 11/08/2021 (For Saving relative filepath)
        public string RelativeFilePath { get; set; }

        //Added by Madhusoodan on 11/08/2021 (For order in edit mode)
        public bool IsOrderInEditMode { get; set; }

        public string ExistingFileName { get; set; }

        //Added by mayank on 30/11/2021
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }
        public int SROfficeID { get; set; }
    }

}
