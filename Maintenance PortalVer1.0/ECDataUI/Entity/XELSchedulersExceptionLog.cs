//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataUI.Entity
{
    using System;
    using System.Collections.Generic;
    
    public partial class XELSchedulersExceptionLog
    {
        public long LogID { get; set; }
        public Nullable<int> SROCode { get; set; }
        public string ExceptionType { get; set; }
        public string InnerExceptionMsg { get; set; }
        public string ExceptionMsg { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionMethodName { get; set; }
        public Nullable<System.DateTime> LogDate { get; set; }
        public Nullable<int> SchedulerID { get; set; }
    
        public virtual MAS_SchedulerMaster MAS_SchedulerMaster { get; set; }
    }
}
