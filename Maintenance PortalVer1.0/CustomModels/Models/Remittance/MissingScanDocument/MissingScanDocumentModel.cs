using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.MissingSacnDocument
{
    public class MissingScanDocumentModel
    {

        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }


        public int SROfficeID { get; set; }



        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        public DateTime DateTime_Date { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }

        [Display(Name = " Document Type")]
        public int DocumentTypeId { get; set; }
        public List<SelectListItem> DocumentType { get; set; }



        public bool IsErrorTypecheck { get; set; }

        public int ErrorCode { get; set; }

    }

    public class MissingScanDocumentResModel
    {
        public List<MissingScanDocumentTableModel> MissingScanDocumentTableModelList { get; set; }
    }

    public class MissingScanDocumentTableModel
    {
        public long srNo { get; set; }

        public long DocumentID { get; set; }

        public int SROCode { get; set; }

        public string SRO_Name { get; set; }

        public string C_Stamp5DateTime { get; set; }

        public string C_FRN { get; set; }

        public string C_ScannedFileName { get; set; }

        public string L_Stamp5DateTime { get; set; }

        public string L_FRN { get; set; }

        public string L_ScannedFileName { get; set; }



        public string C_CDNumber { get; set; }

        public string L_CDNumber { get; set; }
        //
        public DateTime? L_Stamp5DateTime_DateTime { get; set; }
        //

    }


}
