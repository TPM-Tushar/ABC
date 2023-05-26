using CustomModels.Models.MISReports.TransactionDetails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
    public class XMLResModel
    {
        public string XMLString { get; set; }
        //Added by mayank on 01/09/2021 for Kaveri-FRUITS Integration
        public string PDFString { get; set; }
        public byte[] PDFbyte { get; set; }

        public string HTMLString { get; set; }
        public TransactionDetails TransactionDetails { get; set; }
    }
}
