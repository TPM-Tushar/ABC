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
    
    public partial class USP_REM_BankTransactionDetailList_DRO_Result
    {
        public long TransactionID { get; set; }
        public string InstrumentBankIFSCCode { get; set; }
        public Nullable<decimal> InstrumentBankMICRCode { get; set; }
        public string InstrumentNumber { get; set; }
        public string DROCodeStr { get; set; }
        public Nullable<bool> IsReceipt { get; set; }
        public Nullable<long> DocumentID { get; set; }
        public Nullable<System.DateTime> DateOfUpdate { get; set; }
        public Nullable<System.DateTime> Receipt_StampDate { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public Nullable<System.DateTime> InstrumentDate { get; set; }
        public Nullable<long> ReceiptID { get; set; }
        public Nullable<long> StampDetailsID { get; set; }
        public string StampTypeID { get; set; }
        public Nullable<byte> ReceiptPaymentMode { get; set; }
        public Nullable<int> ReceiptNumber { get; set; }
        public string InstrumentBankName { get; set; }
        public string ServiceID { get; set; }
        public string SourceOfReceipt { get; set; }
        public Nullable<int> DROCode { get; set; }
        public bool IsDRO { get; set; }
        public Nullable<int> ReceiptTypeID { get; set; }
        public string ProcessID { get; set; }
        public Nullable<System.DateTime> InsertDateTime { get; set; }
    }
}
