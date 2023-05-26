using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace CustomModels.Models.Remittance.OfficeWiseDiagnosticStatus
{
    public class OfficeWiseDiagnosticStatusModel
    {
        [Display(Name = "Office Name")]
        public int OfficeTypeID { get; set; }
        public List<SelectListItem> OfficeTypeList { get; set; }

        //[Display(Name = "From Date")]
        //public string FromDate { get; set; }

        //[Display(Name = "To Date")]
        //public String ToDate { get; set; }
        [Display(Name = "Date")]
        public String Date { get; set; }
        public int startLen { get; set; }
        public int totalNum { get; set; }

        //[Display(Name = "Action")]
        //public int ActionId { get; set; }
        //public List<SelectListItem> ActionList { get; set; }

        ////for action list as multi select
        //public int[] ActionIdList { get; set; }

        [Display(Name = "Overall Status")]
        public int Status { get; set; }
        public List<SelectListItem> StatusList { get; set; }
        public bool IsDRO { get; set; }
        public string OfficeType { get; set; }
        public TilesModel TilesData { get; set; }
        public int ActionIDForDetailTable { get; set; }
    }



    public class OfficeWiseDiagnosticStatusDetailModel
    {
        public int SrNo { get; set; }
        public string OfficeName { get; set; }
        public string DiagnosticDate { get; set; }
        //public string ActionDescription { get; set; }

        public bool DBCCStatus { get; set; }
        public string DBCCErrorDesc { get; set; }
        public string DBCCOutput { get; set; }
        public string DBCCCCellHtml { get; set; }

        public bool ConstraintIntegrityStatus { get; set; }
        public string ConstraintIntegrityErrorDesc { get; set; }
        public string ConstraintIntegrityOutput { get; set; }
        public string ConstraintIntegrityCellHtml { get; set; }

        public bool AuditEventStatus { get; set; }
        public string AuditEventErrorDesc { get; set; }
        public string AuditEventOutput { get; set; }
        public string AuditEventCellHtml { get; set; }

        public bool Optimizer1Status { get; set; }
        public string Optimizer1ErrorDesc { get; set; }
        public string Optimizer1Output { get; set; }
        public string Optimizer1CellHtml { get; set; }

        public bool Optimizer2Status { get; set; }
        public string Optimizer2ErrorDesc { get; set; }
        public string Optimizer2Output { get; set; }
        public string Optimizer2CellHtml { get; set; }

        public bool LastFullBackupStatus { get; set; }
        public string LastFullBackupErrorDesc { get; set; }
        public string LastFullBackupOutput { get; set; }
        public string LastFullBackupCellHtml { get; set; }

        public bool LastDiffBackupStatus { get; set; }
        public string LastDiffBackupErrorDesc { get; set; }
        public string LastDiffBackupOutput { get; set; }
        public string LastDiffBackupCCellHtml { get; set; }
        //ALL ACTION STATUS
        public bool AllActionStatus { get; set; }
        public string AllActionCellHtml { get; set; }
        public double DataFileSize { get; set; }
        //public double LogFileSize { get; set; }
        public string LogFileSize { get; set; }
        //public double DBDiskSpace { get; set; }
        public string DBDiskSpace { get; set; }
        //for timezone
        public bool TimeZoneStatus { get; set; }
        public string TimeZoneErrorDesc { get; set; }
        public string TimeZoneOutput { get; set; }
        public string TimeZoneCCellHtml { get; set; }

        //ADDED ON 29-12-2020 FOR SECURE CODE TEMPERED
        public string SecureCodeHtml { get; set; }

    }

    public class OfficeWiseDiagnosticStatusListModel
    {
        public List<OfficeWiseDiagnosticStatusDetailModel> OfficeWiseDiagnosticStatusList { get; set; }
        public Decimal TotalAmount { get; set; }
        public int TotalRecords { get; set; }
        public TilesModel TilesData { get; set; }

        //public List<string> TableColumns { get; set; }

    }

    public class DiagnosticActionDetail
    {
        public string ActionDetail { get; set; }

        public int ActionId { get; set; }
        public int DetailId { get; set; }
        public int MasterId { get; set; }
    }

    public class TilesModel
    {
        //public string Figure { get; set; }
        //public string Title { get; set; }
        //public string Description { get; set; }
        public int TotalIssueFound { get; set; }
        public int TotalNo { get; set; }
        public int StatusAvailabelNo { get; set; }
        public int AllOkNo { get; set; }
        public int StatusNotAvailable { get; set; }
        public int IssueFoundNo { get; set; }
        public String StatusAvailabelDesc { get; set; }
        public string AllOkDesc { get; set; }
        public string StatusNotAvailableDesc { get; set; }
        public string IssueFoundNoDesc { get; set; }

        //FOR NO OF ERRORS IN EACH ACTION
        public List<NumberOfErrorsInAction> ActionErrorsList { get; set; }
        //public int DBCCError { get; set; }
        //public int TimeZoneCheckError { get; set; }
        //public int ConstraintIntegrityError { get; set; }
        //public int AuditEventError { get; set; }
        //public int Optimizer1Error { get; set; }
        //public int Optimizer2Error { get; set; }
        //public int LastFullBakVerifyError { get; set; }
        //public int LastDiffBakVerifyError { get; set; }

    }

    public class NumberOfErrorsInAction
    {
        public string ActionDesc { get; set; }
        public int NumberOfErrors { get; set; }
    }

    #region commented
    //public class DiagnosticMasterTable
    //{
    //    public int DiagnosticId { get; set; }
    //    public int  Officecode { get; set; }
    //    public string DiagnosticDate { get; set; }
    //    public int IsSuccessful { get; set; }
    //    public bool IsCentralized { get; set; }
    //    public bool IsDRO { get; set; }

    //}

    //public class DiagnosticDetailsTable
    //{
    //    public int DiagnosticId { get; set; }
    //    public int ActionId { get; set; }
    //    public bool ActionStatus { get; set; }
    //    public string ErrorDescription { get; set; }
    //    public string DiagnosticOutput { get; set; }

    //}
    #endregion
}
