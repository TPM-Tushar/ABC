using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Utilities.ScannedfileDownload
{
    public class ScannedFileDownloadResModel
    {
        public Byte[] ScannedFileByteArray { get; set; }

        public String ErrorMessage { get; set; }
        public bool IsError { get; set; }
        public string FileNameWithExt { get; set; }

        public string FileNameWithoutExt { get; set; }

        public string UserName { get; set; }
        public string ReferenceString { get; set; }
        public bool IsReferenceStringExist { get; set; }
    }
}
