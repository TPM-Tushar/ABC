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
    
    public partial class NoticeScanDocVerifyLog
    {
        public long NoticeID { get; set; }
        public int SROCode { get; set; }
        public bool IsSuccess { get; set; }
        public System.DateTime LogDateTime { get; set; }
        public string Exception { get; set; }
        public Nullable<bool> IsPython { get; set; }
        public Nullable<bool> IsDotNet { get; set; }
        public Nullable<bool> IsMagick { get; set; }
        public Nullable<System.DateTime> StartTime { get; set; }
        public Nullable<System.DateTime> EndTime { get; set; }
        public string LogBy { get; set; }
    
        public virtual NoticeMaster NoticeMaster { get; set; }
    }
}
