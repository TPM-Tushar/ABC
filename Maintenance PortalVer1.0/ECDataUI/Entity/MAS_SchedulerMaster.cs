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
    
    public partial class MAS_SchedulerMaster
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MAS_SchedulerMaster()
        {
            this.XELSchedulersExceptionLog = new HashSet<XELSchedulersExceptionLog>();
        }
    
        public int SchedulerID { get; set; }
        public string Scheduler_Name { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<XELSchedulersExceptionLog> XELSchedulersExceptionLog { get; set; }
    }
}
