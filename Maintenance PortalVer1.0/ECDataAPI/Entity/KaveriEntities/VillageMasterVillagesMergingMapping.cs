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
    
    public partial class VillageMasterVillagesMergingMapping
    {
        public long ID { get; set; }
        public int SROCode { get; set; }
        public long VillageCode { get; set; }
        public long MergedVillageCode { get; set; }
    
        public virtual SROMaster SROMaster { get; set; }
        public virtual VillageMaster VillageMaster { get; set; }
    }
}