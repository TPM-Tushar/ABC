//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataAPI.Entity.ECDATA_PENDOCS
{
    using System;
    using System.Collections.Generic;
    
    public partial class SROMaster_PENDOCS
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SROMaster_PENDOCS()
        {
            this.NoticeMaster = new HashSet<NoticeMaster_PENDOCS>();
        }
    
        public int SROCode { get; set; }
        public Nullable<int> DistrictCode { get; set; }
        public string SRONameK { get; set; }
        public string SRONameE { get; set; }
        public string ShortnameK { get; set; }
        public string ShortNameE { get; set; }
        public bool GetBhoomiData { get; set; }
        public Nullable<bool> IsVillageMatching { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NoticeMaster_PENDOCS> NoticeMaster { get; set; }
    }
}