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
    
    public partial class AMS_REG_PaymentTransDetails
    {
        public long ID { get; set; }
        public long TransactionID { get; set; }
        public string RemitterName { get; set; }
        public string DeptReferenceCode { get; set; }
        public string UIRNumber { get; set; }
        public string StatusCode { get; set; }
        public string StatusDescription { get; set; }
        public Nullable<bool> TransactionStatus { get; set; }
        public Nullable<System.DateTime> TransactionDateTime { get; set; }
        public Nullable<long> UserID { get; set; }
        public string IPAdd { get; set; }
        public string PaymentStatusCode { get; set; }
        public string DDOCode { get; set; }
        public Nullable<System.DateTime> InsertDateTime { get; set; }
    }
}