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
    
    public partial class ECNameSearchKeyValues
    {
        public long KeyID { get; set; }
        public long DocumentID { get; set; }
        public int SROCode { get; set; }
        public int PartyTypeID { get; set; }
        public Nullable<long> PartyID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Nullable<int> OrderID { get; set; }
        public int IsActivated { get; set; }
        public string ActionType { get; set; }
    
        public virtual DEC_DROrderMaster DEC_DROrderMaster { get; set; }
        public virtual DocumentMaster DocumentMaster { get; set; }
        public virtual PartyTypeMaster PartyTypeMaster { get; set; }
    }
}
