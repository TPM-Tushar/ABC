//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ECDataAPI.Entity.ECDATADOCS
{
    using System;
    using System.Collections.Generic;
    
    public partial class XELReadJobDetails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public XELReadJobDetails()
        {
            this.XELAuditSpecificationDetail = new HashSet<XELAuditSpecificationDetail>();
        }
    
        public long JobID { get; set; }
        public Nullable<int> SROCode { get; set; }
        public int FromYear { get; set; }
        public int FromMonth { get; set; }
        public int ToYear { get; set; }
        public int ToMonth { get; set; }
        public System.DateTime JobRegisterDateTime { get; set; }
        public bool IsJobCompleted { get; set; }
        public Nullable<System.DateTime> JobCompletionDateTime { get; set; }
        public Nullable<long> NumberFilesRead { get; set; }
        public bool IsErrorOccured { get; set; }
        public string ErrorMessage { get; set; }
        public Nullable<int> DROCode { get; set; }
        public bool isdro { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<XELAuditSpecificationDetail> XELAuditSpecificationDetail { get; set; }
    }
}
