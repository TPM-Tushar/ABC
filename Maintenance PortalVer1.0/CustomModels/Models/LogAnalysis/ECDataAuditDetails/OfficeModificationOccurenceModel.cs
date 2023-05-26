using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomModels.Models.LogAnalysis.ECDataAuditDetails
{
    public class OfficeModificationOccurenceModel
    {
        public int SROCode { get; set; }
        public String SROName { get; set; }
        public String LastModifiedDateTime { get; set; }
        public int NoOfOccurances{ get; set; }

    }
}