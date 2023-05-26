using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.JSlipUploadReport
{
    public class JSlipUploadRptResModel
    {
        public List<JSlipUploadRptRecModel> JSlipUploadRecList { get; set; }
        public IEnumerable<JSlipUploadRptRecModel> IJSlipUploadRecList { get; set; }
        public int TotalCount { get; set; }
        public int FilteredRecCount { get; set; }
    }

    public class JSlipUploadRptRecModel
    {
        public int SerialNo { get; set; }
        public string District { get; set; }
        public string SRO { get; set; }
        public string OfficeName { get; set; }
        public String FileName { get; set; }
        public int TotalRecords { get; set; }
        public String DocNumberList { get; set; }
        public String AdditionalDocs { get; set; }
        public String UploadDateTime { get; set; }

    }
}
