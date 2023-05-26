using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Utilities.ScannedfileDownload
{
    public class CorrectedPdfModel
    {
        public List<String> BaseImageList { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public int SROCode { get; set; }
        public int documentType { get; set; }
    }
}
