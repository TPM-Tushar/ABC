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
    
    public partial class USP_BLOCKING_PROCESSES_Result
    {
        public Nullable<int> session_id { get; set; }
        public string command { get; set; }
        public Nullable<short> blocking_session_id { get; set; }
        public string wait_type { get; set; }
        public int wait_time { get; set; }
        public string wait_resource { get; set; }
        public string TEXT { get; set; }
        public System.DateTime DateTime { get; set; }
    }
}
