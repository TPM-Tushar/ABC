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
    
    public partial class SAKALA_UploadFileDetails
    {
        public long SID { get; set; }
        public string GSCNo { get; set; }
        public int SROCode { get; set; }
        public string WebMethod { get; set; }
        public bool IsUploaded { get; set; }
        public Nullable<System.DateTime> UploadDateTime { get; set; }
        public int ProcessingCode { get; set; }
        public string ProcessingStatus { get; set; }
        public int RecordsInserted { get; set; }
        public int ErrorDataCount { get; set; }
        public string InputDataset { get; set; }
        public System.DateTime TransferDateTime { get; set; }
        public Nullable<bool> IsTransmitted { get; set; }
        public Nullable<bool> IsDeliverytransmitted { get; set; }
    }
}
