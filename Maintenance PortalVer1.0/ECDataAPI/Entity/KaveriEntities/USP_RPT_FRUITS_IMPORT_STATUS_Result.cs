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
    
    public partial class USP_RPT_FRUITS_IMPORT_STATUS_Result
    {
        public string SRONameE { get; set; }
        public int SROCode { get; set; }
        public string ReferenceNo { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public string ArticleNameE { get; set; }
        public Nullable<System.DateTime> Stamp5DateTime { get; set; }
        public long RequestID { get; set; }
        public Nullable<long> LogId { get; set; }
        public Nullable<byte> DocumentStatusCode { get; set; }
        public System.DateTime DataReceivedDate { get; set; }
        public Nullable<System.DateTime> UploadDateTime { get; set; }
        public Nullable<System.DateTime> ActionDateTime { get; set; }
        public string TransXML { get; set; }
        public System.DateTime KAIGREG_InsertDateTime { get; set; }
        public Nullable<bool> FormIIIExists { get; set; }
        public Nullable<bool> IsResponseUploaded { get; set; }
    }
}