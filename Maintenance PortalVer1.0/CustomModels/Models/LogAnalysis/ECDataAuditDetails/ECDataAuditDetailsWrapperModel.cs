using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.LogAnalysis.ECDataAuditDetails
{
    public class ECDataAuditDetailsWrapperModel
    {
       public  ECDataAuditDetailsRequestModel model { get; set; }
        public  int startLen { get; set; }
        public  int totalNum { get; set; }
    }
}
