//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataAPI.Entity.KAIGR_ONLINE
{
    using System;
    using System.Collections.Generic;
    
    public partial class MAS_OfficeMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAS_OfficeMaster()
        {
            this.MAS_OfficeMaster1 = new HashSet<MAS_OfficeMaster>();
            this.MAS_Villages = new HashSet<MAS_Villages>();
        }
    
        public short OfficeID { get; set; }
        public short OfficeTypeID { get; set; }
        public string OfficeName { get; set; }
        public string OfficeNameR { get; set; }
        public string ShortName { get; set; }
        public string ShortNameR { get; set; }
        public Nullable<short> DistrictID { get; set; }
        public Nullable<short> ParentOfficeID { get; set; }
        public Nullable<short> KaveriCode { get; set; }
        public Nullable<short> BhoomiCensusCode { get; set; }
        public Nullable<bool> AnyWhereRegEnabled { get; set; }
        public string OfficeAddress { get; set; }
        public string Landline { get; set; }
        public string Mobile { get; set; }
        public Nullable<bool> OnlineBookingEnabled { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAS_OfficeMaster> MAS_OfficeMaster1 { get; set; }
        public virtual MAS_OfficeMaster MAS_OfficeMaster2 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MAS_Villages> MAS_Villages { get; set; }
    }
}