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
    
    public partial class RPT_GetInstrumentDetails_Result
    {
        public string InstrumentNumber { get; set; }
        public Nullable<System.DateTime> InstrumentDate { get; set; }
        public byte StampTypeID { get; set; }
        public long ReceiptID { get; set; }
        public Nullable<int> ReceiptNumber { get; set; }
        public Nullable<System.DateTime> Receipt_StampDate { get; set; }
        public Nullable<int> SROCode { get; set; }
        public Nullable<int> DROCode { get; set; }
        public string SRONameE { get; set; }
        public string DistrictNameE { get; set; }
        public Nullable<System.DateTime> InsertDateTime { get; set; }
        public decimal Amount { get; set; }
        public string ServiceName { get; set; }
        public Nullable<long> DocumentID { get; set; }
        public string DocumentPendingNumber { get; set; }
        public bool IsDRO { get; set; }
        public string FRN { get; set; }
    }
}
