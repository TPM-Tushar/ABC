//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataAPI.Entity.ECDATA_PENDOCS
{
    using System;
    using System.Collections.Generic;
    
    public partial class DocumentMaster_PENDOCS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DocumentMaster_PENDOCS()
        {
            this.DocPendingHistory = new HashSet<DocPendingHistory_PENDOCS>();
        }
    
        public long DocumentID { get; set; }
        public int SROCode { get; set; }
        public int BookID { get; set; }
        public int StampArticleCode { get; set; }
        public int RegArticleCode { get; set; }
        public Nullable<int> DocumentNumber { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public Nullable<System.DateTime> PresentDateTime { get; set; }
        public Nullable<System.DateTime> ExecutionDateTime { get; set; }
        public Nullable<System.DateTime> DateOfStamp { get; set; }
        public Nullable<decimal> ConsiderationAmount { get; set; }
        public Nullable<decimal> RequiredStampDuty { get; set; }
        public Nullable<decimal> PaidStampDuty { get; set; }
        public Nullable<System.DateTime> Stamp1DateTime { get; set; }
        public Nullable<System.DateTime> Stamp2DateTime { get; set; }
        public Nullable<System.DateTime> Stamp3DateTime { get; set; }
        public Nullable<System.DateTime> Stamp4DateTime { get; set; }
        public Nullable<System.DateTime> Stamp5DateTime { get; set; }
        public Nullable<System.DateTime> WithdrawalDate { get; set; }
        public Nullable<int> PageCount { get; set; }
        public string Index2Shera { get; set; }
        public bool IsVisited { get; set; }
        public bool IsFiling { get; set; }
        public bool IsPending { get; set; }
        public bool IsScanned { get; set; }
        public bool IsRefused { get; set; }
        public bool IsPaymentOfMoney { get; set; }
        public bool IsAdjudicated { get; set; }
        public bool IsWithdrawn { get; set; }
        public Nullable<System.DateTime> RefusalDate { get; set; }
        public string RefusalReason { get; set; }
        public string RemarksByUser { get; set; }
        public string RemarksBySystem { get; set; }
        public Nullable<long> CorrectionReference { get; set; }
        public string OldDocReference { get; set; }
        public string AdjudicationDetails { get; set; }
        public string CDNumber { get; set; }
        public Nullable<bool> IsXMLTransferredTOBHOOMI { get; set; }
        public Nullable<int> uid { get; set; }
        public string PendingDocumentNumber { get; set; }
        public Nullable<bool> istransmitted { get; set; }
        public Nullable<bool> IsPhotoThumbTransmitted { get; set; }
        public Nullable<System.DateTime> InsertedDateTime { get; set; }
        public Nullable<bool> InitialTransmitted { get; set; }
        public Nullable<System.DateTime> ExpiryDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DocPendingHistory_PENDOCS> DocPendingHistory { get; set; }
    }
}
