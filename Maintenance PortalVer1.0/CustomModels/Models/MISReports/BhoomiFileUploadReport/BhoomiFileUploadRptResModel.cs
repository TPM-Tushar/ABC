using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.BhoomiFileUploadReport
{
    public class BhoomiFileUploadRptResModel
    {
        public List<BhoomiFileUploadRptRecModel> BhoomiFileUploadRecordList { get; set; }
        public IEnumerable<BhoomiFileUploadRptRecModel> IBhoomiFileUploadRecordList { get; set; }

        public int TotalCount { get; set; }
        public int FilteredRecCount { get; set; }
    }
    public class BhoomiFileUploadRptRecModel
    {
        public int SerialNo { get; set; }
        public string District { get; set; }
        public string SRO { get; set; }
        public string OfficeName { get; set; }

        public String SketchNumber { get; set; }
        public String RegistrationNumber { get; set; }

        public String ImportedXML { get; set; }
        public String SurveyNumber { get; set; }

        public String ExportedXML { get; set; }
        public String ReferenceNumber { get; set; }

        public String UploadDate { get; set; }
        public String DateofRegistration { get; set; }
        public bool WhetherDocRegistered { get; set; }


    }
}
