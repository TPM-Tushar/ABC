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
    
    public partial class ARegisterFileDetails
    {
        public long FileID { get; set; }
        public Nullable<int> DistrictCode { get; set; }
        public Nullable<int> SROCode { get; set; }
        public Nullable<int> SearchSROCode { get; set; }
        public Nullable<int> SearchDROCode { get; set; }
        public System.DateTime SearchDate { get; set; }
        public System.DateTime FileGenerationDate { get; set; }
        public Nullable<bool> ISDROffice { get; set; }
        public string PFilePath { get; set; }
        public string VFilePath { get; set; }
        public Nullable<long> UserID { get; set; }
        public long ID { get; set; }
    
        public virtual ARegisterGenerationDetails ARegisterGenerationDetails { get; set; }
        public virtual DistrictMaster DistrictMaster { get; set; }
        public virtual DistrictMaster DistrictMaster1 { get; set; }
        public virtual SROMaster SROMaster { get; set; }
        public virtual SROMaster SROMaster1 { get; set; }
    }
}
