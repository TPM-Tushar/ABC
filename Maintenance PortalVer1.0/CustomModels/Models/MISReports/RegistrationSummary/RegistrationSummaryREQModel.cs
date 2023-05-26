using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.RegistrationSummary
{
    public class RegistrationSummaryREQModel
    {
        public String FinalRegistrationNumberFilePath { get; set; }

        public Byte[] ScannedFileByteArray { get; set; }

        public String ErrorMessage { get; set; }
        public bool IsError { get; set; }
    }
}
