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
    
    public partial class MAS_Villages
    {
        public long VillageID { get; set; }
        public short OfficeID { get; set; }
        public short TalukaID { get; set; }
        public string VillageName { get; set; }
        public string VillageNameR { get; set; }
        public bool IsUrban { get; set; }
        public string CensusCode { get; set; }
    
        public virtual MAS_OfficeMaster MAS_OfficeMaster { get; set; }
        public virtual MAS_Talukas MAS_Talukas { get; set; }
    }
}
