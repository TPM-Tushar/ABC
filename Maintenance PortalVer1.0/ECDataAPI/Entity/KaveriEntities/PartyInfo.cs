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
    
    public partial class PartyInfo
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PartyInfo()
        {
            this.PhotoThumbUploadDetails = new HashSet<PhotoThumbUploadDetails>();
            this.PhotoThumbUploadStatus = new HashSet<PhotoThumbUploadStatus>();
            this.PhotoThumbCD_Failed = new HashSet<PhotoThumbCD_Failed>();
            this.PhotoThumbCD_Success = new HashSet<PhotoThumbCD_Success>();
        }
    
        public long PartyID { get; set; }
        public int SROCode { get; set; }
        public long DocumentID { get; set; }
        public int PartyTypeID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Age { get; set; }
        public Nullable<short> Sex { get; set; }
        public bool IsExecutor { get; set; }
        public bool IsPresenter { get; set; }
        public Nullable<System.DateTime> AdmissionDate { get; set; }
        public string AliasName { get; set; }
        public string CorrectedName { get; set; }
        public string RelationShip { get; set; }
        public string RelativeName { get; set; }
        public string EPIC { get; set; }
        public string PAN { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<int> AvailableExtAcre { get; set; }
        public Nullable<int> AvailableExtGunta { get; set; }
        public Nullable<int> AvailableExtFGunta { get; set; }
        public string Bincom { get; set; }
        public string Category { get; set; }
        public Nullable<System.DateTime> DateOfDeath { get; set; }
        public Nullable<int> FingerID { get; set; }
        public Nullable<short> FingerVerificationStatusID { get; set; }
        public Nullable<bool> IsPartOfRTC { get; set; }
        public Nullable<long> LandCode { get; set; }
        public Nullable<long> MainOwnerNo { get; set; }
        public Nullable<long> OwnerNo { get; set; }
        public string PartyPOA { get; set; }
        public string PhotoPath { get; set; }
        public Nullable<long> PoAAdmission { get; set; }
        public Nullable<long> PoAPresentation { get; set; }
        public Nullable<bool> PrimarySeller { get; set; }
        public string Profession { get; set; }
        public string Restriction { get; set; }
        public string RestrictionDescription { get; set; }
        public string RestrictionType { get; set; }
        public Nullable<bool> Section88Exemption { get; set; }
        public Nullable<int> ThumbMatchFailedReasonID { get; set; }
        public byte[] ThumbMinutiae { get; set; }
        public string ThumbPath { get; set; }
        public Nullable<int> TotalExtAcre { get; set; }
        public Nullable<int> TotalExtGunta { get; set; }
        public Nullable<int> TotalExtFGunta { get; set; }
        public Nullable<int> TransactExtAcre { get; set; }
        public Nullable<int> TransactExtGunta { get; set; }
        public Nullable<int> TransactExtFGunta { get; set; }
        public string VolumeName { get; set; }
        public Nullable<bool> HasGPA { get; set; }
        public Nullable<bool> IsAUA { get; set; }
        public Nullable<long> ImportedPartyParentID { get; set; }
        public Nullable<short> SalutationID { get; set; }
    
        public virtual DocumentMaster DocumentMaster { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhotoThumbUploadDetails> PhotoThumbUploadDetails { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhotoThumbUploadStatus> PhotoThumbUploadStatus { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhotoThumbCD_Failed> PhotoThumbCD_Failed { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PhotoThumbCD_Success> PhotoThumbCD_Success { get; set; }
    }
}