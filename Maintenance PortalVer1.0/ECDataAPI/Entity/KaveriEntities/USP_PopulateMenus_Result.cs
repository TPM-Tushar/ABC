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
    
    public partial class USP_PopulateMenus_Result
    {
        public int IntMenuId { get; set; }
        public string StrMenuName { get; set; }
        public int IntMenuParentId { get; set; }
        public short IntMenuLevel { get; set; }
        public bool BoolStatus { get; set; }
        public short IntMenuSeqNo { get; set; }
        public string strAreaName { get; set; }
        public string StrController { get; set; }
        public string StrAction { get; set; }
        public short HorizontalSequence { get; set; }
        public Nullable<short> ModuleID { get; set; }
        public Nullable<bool> IsMenuIDParameter { get; set; }
        public bool IsHorizontalMenu { get; set; }
        public bool SkipHomePage { get; set; }
        public string MenuIcon { get; set; }
        public string MenuDesc { get; set; }
    }
}
