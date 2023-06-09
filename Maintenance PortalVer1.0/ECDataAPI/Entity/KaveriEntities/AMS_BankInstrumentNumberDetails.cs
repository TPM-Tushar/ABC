//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataAPI.Entity.KaveriEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AMS_BankInstrumentNumberDetails
    {
        public long RowId { get; set; }
        public string InstrumentNumber { get; set; }
        public Nullable<System.DateTime> InstrumentDate { get; set; }
        public Nullable<byte> StampTypeID { get; set; }
        public Nullable<byte> ReceiptPaymentMode { get; set; }
        public Nullable<long> ReceiptID { get; set; }
        public Nullable<long> StampDetailsID { get; set; }
        public Nullable<int> ReceiptNumber { get; set; }
        public Nullable<System.DateTime> Receipt_StampDate { get; set; }
        public Nullable<int> SROCode { get; set; }
        public Nullable<int> DROCode { get; set; }
        public bool IsDRO { get; set; }
        public Nullable<decimal> UniqueReqID { get; set; }
        public Nullable<System.DateTime> InsertDateTime { get; set; }
        public string SourceOfReceipt { get; set; }
        public bool IsthroughSyncOperation { get; set; }
        public decimal Amount { get; set; }
        public Nullable<short> ServiceID { get; set; }
        public Nullable<long> DocumentID { get; set; }
        public string DocumentPendingNumber { get; set; }
        public string PartyName { get; set; }
    
        public virtual MAS_ServiceMaster MAS_ServiceMaster { get; set; }
        public virtual SROMaster SROMaster { get; set; }
    }
}
