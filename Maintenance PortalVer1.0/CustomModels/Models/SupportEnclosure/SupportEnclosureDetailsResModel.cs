using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.SupportEnclosure
{
    public class SupportEnclosureDetailsResModel
    {
        public List<SupportEnclosureDetailsModel> SupportEnclosureDetailsList { get; set; }
        public byte[] EnclosureFileContent { get; set; }
        public bool IsError { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class SupportEnclosureDetailsModel
    {
        public int SerialNo { get; set; }
        public int SROCode { get; set; }
        public string SROOffice { get; set; }
        public string DocumentNo { get; set; }
        public long DocumentID { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public int SupportDocumentTypeID { get; set; }    // Uploaded Docs ID :: SupportDocID :: SupportDocumentUploads ------For Reference
        public string SupportDocumentType { get; set; }   // Uploaded Docs Description :: NameEnglish :: SupportDocumentMaster ------For Reference
        public string UploadDateTime { get; set; }
        public string FileName { get; set; }
        public long PartyID { get; set; }
        public string PartyName { get; set; }
        public string FilePath { get; set; }

    }
}
