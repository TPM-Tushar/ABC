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
    
    public partial class USP_RPT_REGISTRATION_STATUS_Result
    {
        public int SROCODE { get; set; }
        public string SRONameE { get; set; }
        public string Date { get; set; }
        public Nullable<int> SAMEDAY_REGISTERED_SAMEDAY_CENTRALIZED { get; set; }
        public Nullable<int> PreviousDay_REGISTERED_SAMEDAY_CENTRALIZED { get; set; }
        public System.DateTime LAST_DOCUMENT_CENTRALIZED_DATETIME { get; set; }
    }
}