using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
    public class SupportDocuments
    {
        public Int16 SupportDocumentID { get; set; }
        public String Description { get; set; }
        public String DescriptionR { get; set; }
        public Byte ApplicationTypeID { get; set; }
        public Boolean IsChecked { get; set; }
        public Boolean IsOptional { get; set; }
        public string EncryptedID { get; set; }
        public string FileServerPath { get; set; }
        public string VirtualServerPath { get; set; }
        public string FileName { get; set; }
    }
}
