using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.CDWrittenReport
{
    public class CDWrittenReportResModel
    {
        public List<CDWrittenReportRecordModel> CDWrittenReportRecordList { get; set; }
        public IEnumerable<CDWrittenReportRecordModel> ICDWrittenReportRecordList { get; set; }

        public int TotalCount { get; set; }
        public int FilteredRecCount { get; set; }


    }

    public class CDWrittenReportRecordModel
    {
        public int SerialNo { get; set; }
        public string District { get; set; }

        public string SRO { get; set; }
        public string OfficeName { get; set; }

        public String DocType { get; set; }
        public String RegistrationNumber { get; set; }

        public String LocalServerStoragePath { get; set; }
        public int CentralServerPath { get; set; }
        public String FileUploadedToCentralServer { get; set; }

        public string SDCStoragePath { get; set; }
        public Double SizeOfFile { get; set; }

        public string DateOfScan { get; set; }

        public string DateOfUpload { get; set; }
        public string DocDeliveryDate { get; set; }
        public string DateOfRegistration { get; set; }




    }
}
