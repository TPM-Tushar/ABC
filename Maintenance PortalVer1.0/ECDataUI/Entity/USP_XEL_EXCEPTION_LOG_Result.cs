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
    
    public partial class USP_XEL_EXCEPTION_LOG_Result
    {
        public long LogID { get; set; }
        public string SroName { get; set; }
        public Nullable<int> SROCode { get; set; }
        public string ExceptionType { get; set; }
        public string InnerExceptionMsg { get; set; }
        public string ExceptionMsg { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionMethodName { get; set; }
        public Nullable<System.DateTime> LogDate { get; set; }
        public Nullable<int> SchedulerID { get; set; }
        public string Scheduler_Name { get; set; }
    }
}