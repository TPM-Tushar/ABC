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
    
    public partial class LBPASXML
    {
        public long LogId { get; set; }
        public long DocumentID { get; set; }
        public long PropertyID { get; set; }
        public string InputOwnerDetails { get; set; }
        public string OwnerDetails { get; set; }
        public Nullable<bool> Verified { get; set; }
        public Nullable<bool> IsUploaded { get; set; }
        public Nullable<long> AckNumber { get; set; }
        public string AckXMLDetails { get; set; }
        public Nullable<bool> IsBoundaryImported { get; set; }
        public Nullable<System.DateTime> UploadDateTime { get; set; }
        public string SentXMLDetails { get; set; }
        public int SROCode { get; set; }
        public string SentXMLUCDetails { get; set; }
    }
}
