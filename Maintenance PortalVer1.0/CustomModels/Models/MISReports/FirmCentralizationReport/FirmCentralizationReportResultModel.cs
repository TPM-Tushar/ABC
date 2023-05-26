using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.FirmCentralizationReport
{
    public class FirmCentralizationReportResultModel
    {
        public List<FirmCentralizationReportResultDetailModel> DetailsList { get; set; }
        public int TotalCount { get; set; }

        public string DistrictName { get; set; }
    }

    public class FirmCentralizationReportResultDetailModel
    {
        public int Sno { get; set; }
        public long RegistrationID { get; set; }
        public string FirmNumber { get; set; }
        public string CDNumber { get; set; }
        public string IsLocalFirmDataCentralized { get; set; }
        public string IsLocalScanDocumentUpload { get; set; }
        public string IsCDWriting { get; set; }
        public string IsFirmDataCentralized { get; set; }
        public string IsScanDocumentUploaded { get; set; }
        public string IsUploadedScanDocumentPresent{ get; set; }
        public bool bool_IsLocalFirmDataCentralized { get; set; }
        public bool bool_IsLocalScanDocumentUpload { get; set; }
        public bool bool_IsCDWriting { get; set; }
        public bool bool_IsFirmDataCentralized { get; set; }
        public bool bool_IsScanDocumentUploaded { get; set; }
        public bool bool_IsUploadedScanDocumentPresent{ get; set; }
        public string DateOfRegistration { get; set; }

        //Added by mayank on 11/02/2022
        public string FilePath { get; set; }
        public bool bool_IsFilePresent { get; set; }
        public string IsFilePresent { get; set; }
        public string IsFileReadable { get; set; }
        public bool bool_IsFileReadable { get; set; }
        public int DroCode { get; set; }
    }
}
