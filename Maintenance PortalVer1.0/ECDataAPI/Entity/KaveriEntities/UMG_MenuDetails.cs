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
    
    public partial class UMG_MenuDetails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UMG_MenuDetails()
        {
            this.UMG_MenuActionAuthorizationMapping = new HashSet<UMG_MenuActionAuthorizationMapping>();
            this.UMG_MenuActionMapping = new HashSet<UMG_MenuActionMapping>();
            this.UMG_RoleMenuMapping = new HashSet<UMG_RoleMenuMapping>();
        }
    
        public int MenuID { get; set; }
        public string MenuName { get; set; }
        public string MenuNameR { get; set; }
        public int ParentID { get; set; }
        public short Sequence { get; set; }
        public short VerticalLevel { get; set; }
        public short HorizontalSequence { get; set; }
        public bool IsActive { get; set; }
        public int LevelGroupCode { get; set; }
        public Nullable<bool> IsMenuIDParameter { get; set; }
        public bool IsHorizontalMenu { get; set; }
        public bool SkipHomePage { get; set; }
        public string MenuIcon { get; set; }
        public string MenuDesc { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UMG_MenuActionAuthorizationMapping> UMG_MenuActionAuthorizationMapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UMG_MenuActionMapping> UMG_MenuActionMapping { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UMG_RoleMenuMapping> UMG_RoleMenuMapping { get; set; }
    }
}
