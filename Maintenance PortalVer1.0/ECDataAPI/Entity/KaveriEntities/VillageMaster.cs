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
    
    public partial class VillageMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VillageMaster()
        {
            this.ECPropertySearchKeyValues = new HashSet<ECPropertySearchKeyValues>();
            this.VillageMasterVillagesMergingMapping = new HashSet<VillageMasterVillagesMergingMapping>();
        }
    
        public long VillageCode { get; set; }
        public int SROCode { get; set; }
        public Nullable<int> HobliCode { get; set; }
        public string CensusCode { get; set; }
        public int TalukCode { get; set; }
        public string VillageNameK { get; set; }
        public string VillageNameE { get; set; }
        public bool IsUrban { get; set; }
        public Nullable<int> BhoomiTalukCode { get; set; }
        public Nullable<int> BhoomiVillageCode { get; set; }
        public string BhoomiVillageName { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ECPropertySearchKeyValues> ECPropertySearchKeyValues { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VillageMasterVillagesMergingMapping> VillageMasterVillagesMergingMapping { get; set; }
    }
}