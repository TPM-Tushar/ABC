using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.FirmCentralization
{
   public class FirmCentralizationModel
    {
        [Display(Name = "District")]
        public List<SelectListItem> DROfficeList { get; set; }

        public int DROfficeID { get; set; }
        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_FromDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }

        public string SearchBy { get; set; }
    }

    public class FirmCentralizationResultModel
    {
        public List<FirmCentralizationTableModel> DetailsList { get; set; }

        public List<LocalFirmCentralizationTableModel> Local_DetailsList { get; set; }

        public List<CentralFirmCentralizationTableModel> Central_DetailsList { get; set; }
    }
    public class FirmCentralizationTableModel
    {
        public long Sr_No { get; set; }
        public long RegistrationID { get; set; }
        public string L_FirmNumber { get; set; }
        public string L_CDNumber { get; set; }

        public string C_FirmNumber { get; set; }
        public string C_CDNumber { get; set; }
        //public string IsLocalFirmDataCentralized { get; set; }
        //public string IsLocalScanDocumentUpload { get; set; }
        //public string IsCDWriting { get; set; }
        //public string IsFirmDataCentralized { get; set; }
        //public string IsScanDocumentUploaded { get; set; }
        //public string IsUploadedScanDocumentPresent { get; set; }
        //public bool bool_IsLocalFirmDataCentralized { get; set; }
        //public bool bool_IsLocalScanDocumentUpload { get; set; }
        //public bool bool_IsCDWriting { get; set; }
        //public bool bool_IsFirmDataCentralized { get; set; }
        //public bool bool_IsScanDocumentUploaded { get; set; }
        //public bool bool_IsUploadedScanDocumentPresent { get; set; }
        public string L_DateOfRegistration { get; set; }
        public string C_DateOfRegistration { get; set; }

        public DateTime? L_DateOfRegistrationDate { get; set; }
        public DateTime? C_DateOfRegistrationDate { get; set; }
        //
        public string L_ScanFileName { get; set; }
        public string C_ScanFileName { get; set; }

        public string UploadDateTime { get; set; }

        public DateTime? UploadDateTimeDate { get; set; }
        //
    }

    public class LocalFirmCentralizationTableModel
    {
        public long Sr_No { get; set; }
        public long RegistrationID { get; set; }
        public int DROCode { get; set; }
        public string FirmNumber { get; set; }
        public bool IsFirmDataCentralizaed { get; set; }
        public string CDNumber { get; set; }
        public string DateOfRegistration { get; set; }
        public bool? IsScanDocumentUploaded { get; set; }
        public string ScanFileName { get; set; }
        public string CDID { get; set; }
     
        public string ScanDateTime { get; set; }

    }

    public class CentralFirmCentralizationTableModel
    {
        public long Sr_No { get; set; }
        public decimal RegistrationID { get; set; }
        public string FirmNumber { get; set; }
    
        public string DateOfRegistration { get; set; }

        public Nullable<int> Pages { get; set; }
        public string Remarks { get; set; }

        public Nullable<long> ReceiptID { get; set; }

        public int DRCode { get; set; }
        public string FirmType { get; set; }
        public Nullable<int> IsScanned { get; set; }
        public string CDNumber { get; set; }


    }
}
