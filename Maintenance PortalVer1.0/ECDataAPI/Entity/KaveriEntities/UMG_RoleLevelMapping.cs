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
    
    public partial class UMG_RoleLevelMapping
    {
        public int ID { get; set; }
        public short RoleID { get; set; }
        public short LevelID { get; set; }
    
        public virtual UMG_LevelDetails UMG_LevelDetails { get; set; }
        public virtual UMG_RoleDetails UMG_RoleDetails { get; set; }
    }
}
