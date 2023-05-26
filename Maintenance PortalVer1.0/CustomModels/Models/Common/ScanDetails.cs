#region File Header
/*
    * Project Id        :   -
    * Project Name      :   Gauri-I
    * File Name         :   ScanDetails.cs
    * Author Name       :   -Avinash Gawali
    * Creation Date     :   -
    * Last Modified By  :   -
    * Last Modified On  :   -
    * Description       :   Contract to define properties for Scan Details.
*/
#endregion


#region references
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#endregion

namespace CustomModels.Models.Common
{

    public class ScanDetails
    {
        public long ScanID { get; set; }
        public short OfficeID { get; set; }
        public short ProcessID { get; set; }
        public short Pages { get; set; }
        public DateTime? ScanForwardDate { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string LocalPath { get; set; }
        public string FileServerPath { get; set; }
        public DateTime ScanDate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool IsUploaded { get; set; }
        public string Checksum { get; set; }
        public long ScannedByUserID { get; set; }
        public int ScannedByResourceID { get; set; }
        public long UploadedByUserID { get; set; }
        public int UploadedByResourceID { get; set; }
        public DateTime UploadDateTime { get; set; }
        public bool IsApproved { get; set; }
        public DateTime ApprovedDate { get; set; }

        //Need for other operation
        public byte? DocumentStatusID { get; set; }
        public string RoleName { get; set; }
        public string RoleNameR { get; set; }

        //Need for validation i.e. ServiceID
        public int ServiceID { get; set; }

        //for Document Registration Service
        public long DocumentID { get; set; }

        //for Marriage Registration Service
        public long MarriageApplicationID { get; set; }
        public long? MarriageID { get; set; }
        public long? NoticeID { get; set; }
        public long? ObjectionID { get; set; }
        public long? EnquiryID { get; set; }

        //for Firm Registration
        public long? FirmID { get; set; }
        public long? AmendmentID { get; set; }
        public long? DissolutionID { get; set; }

        //DR Order
        public long? CourtOrderID { get; set; }
        public long? DROrderID { get; set; }
        public long? LiabilityID { get; set; }

        public short? ScanProcessID { get; set; }
        public bool IsEnclosure { get; set; }

        public short ProcessMappingID { get; set; }
        public string VirtualServerPath { get; set; }
        public long? PrevDocumentID { get; set; }

        public string Operation { get; set; }
        public long? CrossRefID { get; set; }

        public long KioskTokenID { get; set; }

        public int FirmApplicationTypeID { get; set; }
        public bool IsSupportingDocumentAdded { get; set; }
        public bool IsDigitalSign { get; set; }
    }
}
