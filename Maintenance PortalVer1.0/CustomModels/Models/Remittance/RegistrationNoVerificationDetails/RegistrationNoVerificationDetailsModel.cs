#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   RegistrationNoVerificationDetailsModel.cs
    * Author Name       :   Tushar Mhaske
    * Creation Date     :   - 
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Model Class for Registration No Verification Details.

*/
#endregion
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.RegistrationNoVerificationDetails
{
   public class RegistrationNoVerificationDetailsModel
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

        public bool IsDateNull { get; set; }

        //Added By Tushar on 14 Oct 2022
        public bool IsFRNCheck { get; set; }

        public bool IsSFNCheck { get; set; }

        public bool IsRefresh { get; set; }
        //End By Tushar on 14 Oct 2022

        //Added By Tushar on 1 Nov 2022

        public bool IsFileNA { get; set; }


        public bool IsCNull { get; set; }

        public bool IsLNull { get; set; }

        //End By Tushar on 1 Nov 2022
        //Added By Tushar on 2 nov 2022
        public bool IsErrorTypecheck { get; set; }

        public int ErrorCode { get; set; }
        //End By Tushar on 2 Nov 2022
        //Added BY Tushar on 29 Nov 2022
        public bool IsDuplicate { get; set; }
        //End By Tushar on 29 Nov 2022
	    //Added By Tushar on 3 Jan 2023
        public bool IsPropertyAreaDetailsErrorType { get; set; }
		//End By Tushar on 3 Jan 2023

        //Added by Rushikesh on 6 Feb 2023
        public bool IsDateDetailsErrorType { get; set; }
        //End By Rushikesh on 6 Feb 2023

        public bool IsDateErrorType_DateDetails { get; set; }
    }
    public class RegistrationNoVerificationDetailsTableModel
    {
        public long srNo { get; set; }

        public long? DocumentID { get; set; }

        public int SROCode { get; set; }

        public string C_Stamp5DateTime { get; set; }

        public string C_FRN { get; set; }

        public string C_ScannedFileName { get; set; }

        public string L_Stamp5DateTime { get; set; }

        public string L_FRN { get; set; }

        public string L_ScannedFileName { get; set; }

        public long? BatchID { get; set; }

        public string C_CDNumber { get; set; }

        public string L_CDNumber { get; set; }

        public string ErrorType { get; set; }

        public int DocumentTypeID { get; set; }

        public string BatchDateTime { get; set; }


        public string C_ScanFileUploadDateTime { get; set; }

        public string L_ScanDate { get; set; }

        //Added By Rushikesh 9 Feb 2023
        public string TableName { get; set; }
        public Nullable<long> ReceiptID { get; set; }
        public string L_DateOfReceipt { get; set; }
        public string C_DateOfReceipt { get; set; }
        public Nullable<long> StampDetailsID { get; set; }
        // public string L_DateOfStamp { get; set; }
        // public string C_DateOfStamp { get; set; }
        public string L_DDChalDate { get; set; }
        public string C_DDChalDate { get; set; }
        public string L_StampPaymentDate { get; set; }
        public string C_StampPaymentDate { get; set; }
        public string L_DateOfReturn { get; set; }
        public string C_DateOfReturn { get; set; }
        public Nullable<long> PartyID { get; set; }
        public string L_AdmissionDate { get; set; }
        public string C_AdmissionDate { get; set; }
        //End By Rushikesh 9 Feb 2023

        //Added By Tushar on 17 Oct 2022
        public List<RPT_DocReg_NoCLBatchDetailsTable> RPT_DocReg_NoCLBatchDetailsExcelSheet { get; set; }
        //End By Tushar on 17 Oct 2022
        //Added By Tushar on 7 Nov 2022
        public List<ScannedFileDetails> scannedFileDetailsList { get; set; }

        public List<DocumentMasterFRN> DocumentMasterFRNList { get; set; }
        //End By Tushar on 7 Nov 2022
        //Added By Tushar on 29 Nov 2022
        public bool IsDuplicate { get; set; }

        public string L_StartTime { get; set; }

        public string L_EndTime { get; set; }

        public string L_Filesize { get; set; }

        public long L_Pages { get; set; }

        public long L_Checksum { get; set; }
        //End By Tushar on 29 Nov 2022

        //Added By Tushar on 3 Jan 2023
        public long PropertyID { get; set; }
  
        public int? VillageCode { get; set; }
        public decimal? TotalArea { get; set; }
        public int? MeasurementUnit { get; set; }
  
        public long? C_PropertyID { get; set; }
        public int? C_SROCode { get; set; }
        public int? C_VillageCode { get; set; }
        public decimal? C_TotalArea { get; set; }
        public long? C_DocumentID { get; set; }
        public int? C_MeasurementUnit { get; set; }

        //End By Tushar on 3 Jan 2023
        //Added By Tushar on 5 Jan 2023
        public string RefreshMessage { get; set; }
        //End By Tushar on 5 Jan 2023

        //Added By Rushikesh 6 Feb 2023
        public string L_Stamp5DateTime_1 { get; set; }
        public string L_Stamp1DateTime { get; set; }
        public string L_Stamp2DateTime { get; set; }
        public string L_Stamp3DateTime { get; set; }
        public string L_Stamp4DateTime { get; set; }
        public string L_PresentDateTime { get; set; }
        public string L_ExecutionDateTime { get; set; }
        public string L_DateOfStamp { get; set; }
        public string L_WithdrawalDate { get; set; }
        public string L_RefusalDate { get; set; }
        public string C_Stamp1DateTime { get; set; }
        public string C_Stamp2DateTime { get; set; }
        public string C_Stamp3DateTime { get; set; }
        public string C_Stamp4DateTime { get; set; }
        public string C_PresentDateTime { get; set; }
        public string C_ExecutionDateTime { get; set; }
        public string C_DateOfStamp { get; set; }
        public string C_WithdrawalDate { get; set; }
        public string C_RefusalDate { get; set; }
        //End By Rushikesh 6 Feb 2023
        //Added By Tushar on 8 Feb 2023
       public List<RPT_DocReg_DateDetailsList> RPT_DocReg_DateDetailsList { get; set; }
        //End By Tushar on 8 Feb 2023
    }
    public class RPT_DocReg_NoCLBatchDetailsTable
    {
        public long srNo { get; set; }
        public long BatchID { get; set; }

        public int? SROCode { get; set; }

        public long? FromDocumentID { get; set; }
        public long? ToDocumentID { get; set; }
        public short? DocumentTypeID { get; set; }
        public string BatchDateTime { get; set; }
        public bool? IsVerified { get; set; }
        public bool? IsMismatchFound { get; set; }



    }
    //Added By Tushar on 7 Nov 2022
    public class ScannedFileDetails
    {
        public long srNo { get; set; }
        public int? SROCode { get; set; }

        public string ScannedFileName { get; set; }

        public string SroName { get; set; }

        public int CountD { get; set; }
    }

    public class DocumentMasterFRN
    {
        public long srNo { get; set; }
        public int? SROCode { get; set; }

        public string FinalRegistrationNumber { get; set; }

        public string SroName { get; set; }

        public int Count { get; set; }
    }
    //End By Tushar on 7 Nov 2022

    //Added By Tushar on 8 Feb 2023
    public class RPT_DocReg_DateDetailsList
    {
        public long srNo { get; set; }
        public long ID { get; set; }
        public long DocumentID { get; set; }
        public int SROCode { get; set; }
        public string TableName { get; set; }
        public Nullable<long> ReceiptID { get; set; }
        public string L_DateOfReceipt { get; set; }
        public string C_DateOfReceipt { get; set; }
        public Nullable<long> StampDetailsID { get; set; }
        public string L_DateOfStamp { get; set; }
        public string C_DateOfStamp { get; set; }
        public string L_DDChalDate { get; set; }
        public string C_DDChalDate { get; set; }
        public string L_StampPaymentDate { get; set; }
        public string C_StampPaymentDate { get; set; }
        public string L_DateOfReturn { get; set; }
        public string C_DateOfReturn { get; set; }
        public Nullable<long> PartyID { get; set; }
        public string L_AdmissionDate { get; set; }
        public string C_AdmissionDate { get; set; }
        public Nullable<long> BatchID { get; set; }
        public string ErrorType { get; set; }
    }
    //End By Tushar on 8 Feb 2023
}
