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
    
    public partial class AppVersion
    {
        public string AppName { get; set; }
        public int SROCode { get; set; }
        public int AppMajor { get; set; }
        public int AppMinor { get; set; }
        public System.DateTime ReleaseDate { get; set; }
        public Nullable<System.DateTime> LastDateForPatchUpdate { get; set; }
        public Nullable<System.DateTime> SPExecutionDateTime { get; set; }
        public Nullable<int> DRCode { get; set; }
        public Nullable<bool> IsDROffice { get; set; }
        public long ID { get; set; }
    
        public virtual SROMaster SROMaster { get; set; }
    }
}
