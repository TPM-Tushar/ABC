using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.DocumentScanAndDeliveryReport
{
    public class DocumentScanAndDeliveryREQModel
    {
        public bool IsExcel { get; set; }
        public String SearchValue { get; set; }

        [Display(Name = "From Date")]
        [Required(ErrorMessage = "From Date Required")]
        public String FromDate { get; set; }

        [Required(ErrorMessage = "To Date Required")]
        [Display(Name = "To Date")]
        public String ToDate { get; set; }

        public DateTime DateTime_FromDate { get; set; }

        public DateTime DateTime_ToDate { get; set; }

        public int startLen { get; set; }

        public int totalNum { get; set; }

        [Display(Name = "SRO")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }


        [Display(Name = "District")]
        public List<SelectListItem> DistrictList { get; set; }

        public int DistrictID { get; set; }

        public String OfficeType { get; set; }
        public bool IsSr { get; set; }
        public bool IsDr { get; set; }
        public bool IsSrLogin { get; set; }

        //Added by Raman Kalegaonkar on 08-04-2020
        public List<SelectListItem> DocumentType { get; set; }
        [Display(Name = "Document Type")]
        public int DocumentTypeID { get; set; }

    }


    public class DocumentScanAndDeliveryWrapper
    {
        public List<DocumentScanAndDeliveryDetail> DocumentScanAndDeliveryDetailList { get; set; }

        public String SelectedSRO { get; set; }

        public String SelectedDRO { get; set; }
        public int TotalRecords { get; set; }

    }
    public class DocumentScanAndDeliveryDetail
    {
        public int SerialNumber { get; set; }

        public String OfficeName { get; set; }

        public String DocumentType { get; set; }

        public String FinalRegistrationNumber { get; set; }

        public String LocalServerStoragePath { get; set; }

        public String FileUploadedToCentralServer { get; set; }

        public String StateDataCentreStoragePath { get; set; }

        public String SizeoftheFile { get; set; }

        public String DateofScan { get; set; }

        public String DateofUpload { get; set; }

        public String ArchivedinCD { get; set; }

        public String DocumentDeliveryDate { get; set; }

        public String DateofRegistration { get; set; }


    }

}
