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
    
    public partial class UMG_UserProfile
    {
        public long UserProfileID { get; set; }
        public long UserID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public Nullable<byte> GenderID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public short CountryID { get; set; }
        public string Pincode { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string PAN { get; set; }
        public string EPIC { get; set; }
        public string UID { get; set; }
        public byte[] ThumbTemplate { get; set; }
        public byte[] ThumbMinutae { get; set; }
        public Nullable<int> FingerID { get; set; }
        public Nullable<System.DateTime> ProfileLatestUpdateDate { get; set; }
        public string PhotoVirtualPath { get; set; }
        public string ThumbVirtualPath { get; set; }
        public string PhotoFilePath { get; set; }
        public string ThumbFilePath { get; set; }
        public Nullable<int> ResourceID { get; set; }
        public Nullable<short> IDProofTypeID { get; set; }
        public string IDProofNumber { get; set; }
        public bool IsMobileNumVerified { get; set; }
    
        public virtual MAS_Countries MAS_Countries { get; set; }
        public virtual UMG_UserProfile UMG_UserProfile1 { get; set; }
        public virtual UMG_UserProfile UMG_UserProfile2 { get; set; }
        public virtual UMG_UserProfile UMG_UserProfile11 { get; set; }
        public virtual UMG_UserProfile UMG_UserProfile3 { get; set; }
        public virtual MAS_Gender MAS_Gender { get; set; }
        public virtual MAS_IDProofTypes MAS_IDProofTypes { get; set; }
        public virtual UMG_UserDetails UMG_UserDetails { get; set; }
    }
}