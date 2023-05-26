using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.XELFiles
{
    public class XELLogDetailsModel 
    {
        public int sroCode { get; set; }
        public string sroName { get; set; }
        public int SrNo { get; set; }

        public string AbsolutePath { get; set; }
        public string fileName { get; set; }
        public string IsSuccessfullUpload { get; set; }
        public string IsFileReadSuccessful { get; set; }

        
        public string TransmissionInitateDateTime { get; set; }
        public string TransmissionCompleteDateTime { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string FileSize { get; set; }
        public string FileReadDateTime { get; set; }

        public string EventStartDate { get; set; }
        public string EventEndDate { get; set; }

        public string sExceptionType { get; set; }
        public string InnerExceptionMsg { get; set; }
        public string ExceptionMsg { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionMethodName { get; set; }
        public string LogDate { get; set; }
        public string SchedulerName { get; set; }
        public string OfficeType { get; set; }
    }
}
