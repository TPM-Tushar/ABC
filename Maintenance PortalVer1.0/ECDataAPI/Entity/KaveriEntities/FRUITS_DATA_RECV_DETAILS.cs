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
    
    public partial class FRUITS_DATA_RECV_DETAILS
    {
        public long RequestID { get; set; }
        public int PSROCode { get; set; }
        public string ReferenceNo { get; set; }
        public int SROCode { get; set; }
        public string FormIIIData { get; set; }
        public string TransXML { get; set; }
        public string FormIIIPath { get; set; }
        public string TransXMLPath { get; set; }
        public string AknowledgementNo { get; set; }
        public System.DateTime DataReceivedDate { get; set; }
        public Nullable<long> DocumentID { get; set; }
        public string FormIIIDataSigned { get; set; }
        public Nullable<bool> IsSignedFormIIIUploaded { get; set; }
        public Nullable<byte> DocumentStatusCode { get; set; }
        public System.DateTime KAIGREG_InsertDateTime { get; set; }
        public Nullable<System.DateTime> ActionDateTime { get; set; }
        public Nullable<byte> ActionStatusCode { get; set; }
        public Nullable<bool> ISTransmitted { get; set; }
        public Nullable<bool> IsRejectedTransmitted { get; set; }
    
        public virtual DocumentMaster DocumentMaster { get; set; }
    }
}
