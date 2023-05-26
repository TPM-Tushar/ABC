using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
    public class FileDisplayModel
    {
        public bool isFileExist { get; set; }
        public byte[] fileBytes { get; set; }
        public string FileContentBase64 { get; set; }
        public string Message { get; set; }
        
        public string EncryptedID { get; set; }

        public string TittleForPDF { get; set; }

        public int NumberOfPages { get; set; }
        public long FirmID { get; set; }
        public string Checksum { get; set; }

        public bool IsFileNotFound { get; set; }

    }
    
}
