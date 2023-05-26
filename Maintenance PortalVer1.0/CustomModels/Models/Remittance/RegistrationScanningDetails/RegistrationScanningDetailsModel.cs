#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationScanningDetailsModel.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   CustomModels for Registration Scanning Details Report .

*/
#endregion


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.RegistrationScanningDetails
{
   public class RegistrationScanningDetailsModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        public DateTime DateTime_FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }

        [Display(Name = " Document Type")]
        public int DocumentTypeId { get; set; }
        public List<SelectListItem> DocumentType { get; set; }

        //Added By Tushar on 13 jan 2023
        public string ScanFilterValue { get; set; }
        //End By Tushar on 13 jan 2023
    }

   public class RegistrationScanningDetailsResultModel
    {
        public List<RegistrationScanningDetailsTableModel> registrationScanningDetailsTableModelsList { get; set; }
    }

    public class RegistrationScanningDetailsTableModel
    {
        public long srNo { get; set; }

        public string MarriageCaseNo { get; set; }

        public int SROCode { get; set; }

        public string CDNumber { get; set; }

        public DateTime? DateOfRegistration { get; set; }

        public string DateOfRegistration_Date { get; set; }

        public long MarriageRegistrationID { get; set; }

        public string ScanMasterID { get; set; }

        public string ScannedFileUploadDetailsID { get; set; }

        public string IsCDWritten { get; set; }

        //Added By Tushar on 13 jan 2023
        public string ScanFilePath { get; set; }
        //End By Tushar on 13 jan 2023
	    //Added By Tushar on 16 jan 2023
        public string FirmNumber { get; set; }

        public int DRCode { get; set; }
        //End By Tushar on 16 jan 2023
    }
}
