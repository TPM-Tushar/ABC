#region File Header
/*
    * Project Id        :   -
    * Project Name      :   ECDATA Portal
    * File Name         :   DataRestorationReportViewModel.cs
    * Author Name       :   Shubham Bhagat
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Modal for DataRestorationReport.
	* ECR No			:	431
*/
#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.MISReports.DataRestorationReport
{
    public class DataRestorationReportViewModel
    {
        [Display(Name = "SRO Name")]
        public List<SelectListItem> SROfficeList { get; set; }

        public int SROfficeID { get; set; }

        public int CurrentRoleID { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 23-07-2020
        //public String InitiateBTNForSR { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 24-07-2020 AT 6:05 PM
        public int INIT_ID_INT { get; set; }

       
    }

    public class DataRestorationPartialViewModel
    {

        public bool IsDataRestored { get; set; }

        public bool IsDataInserted { get; set; }

        public String DataRestorationDate { get; set; }
        public String UptoWhichDateDataRestorated { get; set; }
        public String LastDocumentRegistrationNumber { get; set; }
        public String LastMarriageRegistrationNumber { get; set; }
        public String LastNoticeRegistrationNumber { get; set; }
        public String LastDocumentRegistrationDate { get; set; }
        public String LastMarriageRegistrationDate { get; set; }
        public String LastNoticeRegistrationDate { get; set; }

        #region COMMENTED BY SHUBHAM BHAGAT ON 03-07-2020 AT 6:30 PM.

        //public String DocumentRegInsertionProjection { get; set; }
        //public String MarriageRegInsertionProjection { get; set; }
        //public String NoticeRegInsertionProjection { get; set; }
        //public String DocumentRegInsertionProjectionStatus { get; set; }
        //public String MarriageRegInsertionProjectionStatus { get; set; }
        //public String NoticeRegInsertionProjectionStatus { get; set; } 
        #endregion
        public String ApproveBtn { get; set; }

        // ADDED  BY SHUBHAM BHAGAT ON 03-07-2020 AT 6:30 PM.
        public String DB_RES_BAK_FILE_DATETIME { get; set; }

        public String SRName { get; set; }

        public String SRContactNumber { get; set; }
        public String OfficeName { get; set; }

        [Display(Name = "Initiation Date")]
        //[Required(ErrorMessage = "Initiation Date")]
        public String InitiationDate { get; set; }

        public String GenerateKeyBtn { get; set; }

        public String InitiationDateBtn { get; set; }

        public String GenerateKeyValue { get; set; }

        public String GenerateKeyBtnAndTextMsg { get; set; }

        public bool ShowInitDateAndGeneratedKeyMsg { get; set; }

        public List<DataInsertionDetails> DataInsertionDetailsList { get; set; }

        public bool IsDataInsertionTableFetched { get; set; }

        public String DownloadScriptForTechAdmin { get; set; }

        public String UploadScriptForTechAdmin { get; set; }

        #region ADDED BY PANKAJ ON 01-07-2020
        public int DocumentRegistrationsToBeRestored { get; set; }
        public int DocumentRegistratiosRestored { get; set; }
        public int MarriageRegistratiosToBeRestored { get; set; }
        public int MarriageRegistrationsRestored { get; set; }
        public int NoticeRegistrationsToBeRestored { get; set; }
        public int NoticeRegistrationsRestored { get; set; }
        public List<DB_RES_TABLE_WISE_COUNT> dB_RES_TABLE_WISE_COUNT_List { get; set; }
        public bool IsCompleted { get; set; }   // added by pankaj on 15-07-2020
        public String ConfirmBtn { get; set; }  // added by pankaj on 15-07-2020
        public String ConfirmBtnMsg { get; set; }

        public bool IsConclusionToDisplay { get; set; }   // added by pankaj on 16-07-2020
        #endregion

        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
        // TO DISPLAY NOTE TO SR WHEN DATABASE IS RESTORED AND TO BE APPROVED AND TO BE HIDDEN APTER APPROVAL
        public String NoteForApprovalForSR { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
        public String DB_RES_BAK_FILE_Size { get; set; }

        public String ScriptExecutionErrorMsg { get; set; }

        public bool IsDatabaseNotRestored { get; set; }

        public String DatabaseNotRestoredErrorMsg { get; set; }

        // ADDED ON 22-07-2020 BY SHUBHAM BHAGAT 
        public String ConclusionMsg { get; set; }
        
        public List<InitMasterModel> InitMasterModelList { get; set; }

        public int TotalRecords { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 24-07-2020
        public String InitiateBTNForSR { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 29-07-2020 AT 4:24 PM BELOW CODE

        public String DocumentRegistrationCDNumber { get; set; }
        public String MarriageRegistrationCDNumber { get; set; }
        public String DB_Restored_upto_BAK_FileName { get; set; }
        // ADDED BY SHUBHAM BHAGAT ON 29-07-2020 AT 4:24 PM ABOVE CODE


        // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 10:55 PM BELOW CODE

        // FOR DISPLAYING COUNT OF DOCUMENT MASTER IN SUMMARY TABLE
        public int Summary_Tbl_DM_Count { get; set; }

        // FOR DISPLAYING COUNT OF MARRIAGE REGISTRATION IN SUMMARY TABLE
        public int Summary_Tbl_MR_Count { get; set; }

        // FOR DISPLAYING COUNT OF NOTICE MASTER IN SUMMARY TABLE
        public int Summary_Tbl_NM_Count { get; set; }

        // ADDED BY SHUBHAM BHAGAT ON 30-07-2020 10:55 PM BELOW CODE

        // ADDED BY PANKAJ ON 31-07-2020      
        public List<DB_RES_ACTIONS_CLASS> dB_RES_ACTIONsForErrorHistory { get; set; }

        // ADDED  BY SHUBHAM BHAGAT ON 05-08-2020
        public String ScriptRectificationHistory { get; set; }


        // BELOW CODE ADDED BU SHUBHAM BHAGAT ON 10-08-2020              
        public bool IsScriptGenerationError { get; set; }

        public String IsScriptGenerationMsg { get; set; }
        // ABOVE CODE ADDED BU SHUBHAM BHAGAT ON 10-08-2020       
        
        public String PresentDateTime { get; set; }

        public String DocumentNumber { get; set; }


        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 8-12-2020


        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021
        public String OtherReceiptID { get; set; }
        public int Summary_Tbl_OtherReceipt_Count { get; set; }

        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 03-05-2021

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 06-05-2021
        public String GenerateScriptBTN_UI { get; set; }


        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 06-05-2021
        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 06-05-2021
        public bool Is_GenerateScriptBTN_UI { get; set; }


        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 06-05-2021



        //BELOW CODE ADDED BY SHUBHAM BHAGAT ON 11-05-2021
        public String ReceiptNumber { get; set; }
        public String ReceiptDateTime { get; set; }
                       
        //ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 11-05-2021
    }

    public class DataRestorationReportReqModel
    {
        public DateTime InitiationDate { get; set; }

        public int SROfficeID { get; set; }

        public String GenerateKeyValue { get; set; }

        public int INITID_INT { get; set; }

        public int KEYID_INT { get; set; }

        public int SCRIPT_ID_INT { get; set; }

        public String ScriptRectificationHistory { get; set; }

        public byte[] FileContentField { get; set; }

        public bool IsRectifiedScriptUploaded { get; set; }

        // ADDED BY SHUBHAM BHAGAT BY 24-07-2020
        public String SearchValue { get; set; }

        public int startLen { get; set; }

        public int totalNum { get; set; }

        public int CurrentRoleID { get; set; }        

    }

    public class DataRestorationReportResModel
    {
        public bool IsDataSavedSuccefully { get; set; }

        public bool IsKeyGeneratedSuccefully { get; set; }

        public String GeneratedKeyWithMsgAfterExpiration { get; set; }

        public bool IsScriptApprovedSuccefully { get; set; }

        public String ScriptApprovedMSG { get; set; }

        public bool IsFileExistAtDownloadPath { get; set; }

        public byte[] FileContentField { get; set; }

        public int SROCodeForFileNameDownload { get; set; }

        public bool IsFileFetchedSuccesfully { get; set; }


        public bool IsRectifiedScriptUploadedSuccefully { get; set; }

        public String RectifiedScriptUploadedMsg { get; set; }

        #region ADDED BY PANKAJ ON 15-07-2020
        public String DataInsertionConfrimationMsg { get; set; }

        public bool IsDataInsertionConfirmed { get; set; }
        #endregion
        // ADDED BY SHUBHAM BHAGAT ON 15-07-2020
        // ADDED TO TAKE MESSAGE FROM DAL TO MVC AND THEN TO JS TO SHOW MESSAGE TO SR AFTER CLICKING APPROVING BUTTON
        public String ApproveBtnORMessage { get; set; }

        public String InitiationProcessStartedOrNotMSG { get; set; }


    }

    public class DataInsertionDetails
    {
        public int IterationNo { get; set; }

        public String RegistrationType { get; set; }

        public String RegistrationNumber { get; set; }

        public String RegistrationDate { get; set; }

        public String Status { get; set; }

        public String ScriptRectificationHistory { get; set; }

        //  ADDED BY SHUBHAM BHAGAT ON 30-07-2020 AT 6:50 PM
        public int SrNo { get; set; }

    }


    public class UploadRectifiedScriptViewModel
    {

        public String ScriptRectificationHistory { get; set; }

        public String INITID_STR { get; set; }

        public String SCRIPT_ID_STR { get; set; }

    }

    #region ADDED BY PANKAJ ON 07-01-2020
    public class DB_RES_TABLE_WISE_COUNT
    {
        public int RowID { get; set; }
        public int InitId { get; set; }
        public int TableId { get; set; }
        public string TableName { get; set; }
        public int RowsToBeInserted { get; set; }
        public int RowsInserted { get; set; }
    }
    #endregion



    public class InitMasterModel
    {
        public int SrNo { get; set; }

        public int SroCode { get; set; }

        public String SROName { get; set; }

        public String InitiationDateTime { get; set; }

        public String CompleteDateTime { get; set; }

        public String ConfirmDateTime { get; set; }

        public String STATUS_DESCRIPTION { get; set; }

        public String Is_Completed_STR { get; set; }

        public int INIT_ID { get; set; }

        // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
        public String AbortBtn { get; set; }
        // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020	


    }

    #region ADDED BY PANKAJ ON 31-07-2020
    public class DB_RES_ACTIONS_CLASS
    {
        public int RowId { get; set; }
        public int InitId { get; set; }
        public int ActionId { get; set; }
        public String ActionDateTime { get; set; }
        public bool IsSuccessful { get; set; }
        public int ScriptId { get; set; }
        public String ErrorDescription { get; set; }
        public String CorrectiveAction { get; set; }
        public String ActionDescription { get; set; }
    }
    #endregion



    // BELOW CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020
    public class AbortViewModel
    {
        [Display(Name = "Reason for Abort")]
        //public StringBuilder AbortDescription { get; set; }
        public String AbortDescription { get; set; }

        public String INIT_ID_ForAbort { get; set; }

        public String Message { get; set; }

        public int INIT_ID_INT { get; set; }
    }
    // ABOVE CODE ADDED BY SHUBHAM BHAGAT ON 30-09-2020	

}
