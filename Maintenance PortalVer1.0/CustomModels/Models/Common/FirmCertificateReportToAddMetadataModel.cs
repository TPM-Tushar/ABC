using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
    public class FirmCertificateReportToAddMetadataModel
    {
        //public string UserName { get; set; }
        //public string Password { get; set; }
        //public string FolderNameAndReportName { get; set; }
        //public string ReportServerUrl { get; set; }
        //public Object ReportParameter { get; set; }
        //public string QueryString { get; set; }

        public string EncryptedID { get; set; }
        public byte[] FileBytes { get; set; }
        public short ServiceID { get; set; }
    }
}
