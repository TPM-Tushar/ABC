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
    
    public partial class DB_RES_INITIATE_MASTER
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DB_RES_INITIATE_MASTER()
        {
            this.DB_RES_ACTIVATION_KEY_OTP = new HashSet<DB_RES_ACTIVATION_KEY_OTP>();
            this.DB_RES_INSERT_SCRIPT_DETAILS = new HashSet<DB_RES_INSERT_SCRIPT_DETAILS>();
            this.DB_RES_SERVICE_COMM_DETAILS = new HashSet<DB_RES_SERVICE_COMM_DETAILS>();
            this.DB_RES_TABLEWISE_COUNT = new HashSet<DB_RES_TABLEWISE_COUNT>();
            this.DB_RES_ACTIONS = new HashSet<DB_RES_ACTIONS>();
        }
    
        public int INIT_ID { get; set; }
        public Nullable<int> SROCODE { get; set; }
        public System.DateTime INIT_DATE { get; set; }
        public Nullable<int> STATUS_ID { get; set; }
        public bool IS_COMPLETED { get; set; }
        public Nullable<int> DROCODE { get; set; }
        public bool IS_DRO { get; set; }
        public Nullable<System.DateTime> COMPLETE_DATETIME { get; set; }
        public Nullable<System.DateTime> CONFIRM_DATETIME { get; set; }
        public Nullable<System.DateTime> DATE_OF_ABORT { get; set; }
        public string ABORT_DESCRIPTION { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_RES_ACTIVATION_KEY_OTP> DB_RES_ACTIVATION_KEY_OTP { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_RES_INSERT_SCRIPT_DETAILS> DB_RES_INSERT_SCRIPT_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_RES_SERVICE_COMM_DETAILS> DB_RES_SERVICE_COMM_DETAILS { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_RES_TABLEWISE_COUNT> DB_RES_TABLEWISE_COUNT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DB_RES_ACTIONS> DB_RES_ACTIONS { get; set; }
    }
}