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
    
    public partial class UMG_LevelDetails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UMG_LevelDetails()
        {
            this.MAS_OfficeMaster = new HashSet<MAS_OfficeMaster>();
            this.UMG_RoleLevelMapping = new HashSet<UMG_RoleLevelMapping>();
            this.UMG_UserDetails = new HashSet<UMG_UserDetails>();
            this.UMG_UserDetails_Log = new HashSet<UMG_UserDetails_Log>();
        }
    
        public short LevelID { get; set; }
        public string LevelName { get; set; }
        public short ParentID { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAS_OfficeMaster> MAS_OfficeMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UMG_RoleLevelMapping> UMG_RoleLevelMapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UMG_UserDetails> UMG_UserDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UMG_UserDetails_Log> UMG_UserDetails_Log { get; set; }
    }
}
