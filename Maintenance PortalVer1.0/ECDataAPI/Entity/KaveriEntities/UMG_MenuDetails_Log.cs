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
    
    public partial class UMG_MenuDetails_Log
    {
        public long LogID { get; set; }
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
        public Nullable<System.DateTime> UpdateDateTime { get; set; }
        public Nullable<long> UserID { get; set; }
        public string UserIPAddress { get; set; }
        public string ActionPerformed { get; set; }
    
        public virtual UMG_UserDetails UMG_UserDetails { get; set; }
    }
}
