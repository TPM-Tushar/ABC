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
    using System.Collections.Generic;
    
    public partial class ECLOG_FUNCTIONALITY_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ECLOG_FUNCTIONALITY_MASTER()
        {
            this.ECLOG_FUNCTIONALITY_DEBUG_LOG = new HashSet<ECLOG_FUNCTIONALITY_DEBUG_LOG>();
            this.ECLOG_FUNCTIONALITY_MASTER1 = new HashSet<ECLOG_FUNCTIONALITY_MASTER>();
        }
    
        public int FUNCTIONALITY_ID { get; set; }
        public string FUNCTIONALITY_NAME { get; set; }
        public string FUNCTIONALITY_DESC { get; set; }
        public int LEVEL_ID { get; set; }
        public Nullable<int> PARENT_FUNCTIONALITY_ID { get; set; }
        public bool STATUS { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ECLOG_FUNCTIONALITY_DEBUG_LOG> ECLOG_FUNCTIONALITY_DEBUG_LOG { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ECLOG_FUNCTIONALITY_MASTER> ECLOG_FUNCTIONALITY_MASTER1 { get; set; }
        public virtual ECLOG_FUNCTIONALITY_MASTER ECLOG_FUNCTIONALITY_MASTER2 { get; set; }
    }
}
