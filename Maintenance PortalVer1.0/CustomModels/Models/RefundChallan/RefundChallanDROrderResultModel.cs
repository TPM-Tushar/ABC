using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.RefundChallan
{
   public class RefundChallanDROrderResultModel
    {
       
        public string RelativeFilePath { get; set; }

        public string FileName { get; set; }
        public string rootPath { get; set; }
        public byte[] refundChallanApproveFileBytes { get; set; }

    }
}
