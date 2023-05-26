using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.RefundChallan
{
   public class RefundChallanResultModel
   {
        public List<RefundChallanTableModel> refundChallanTableList { get; set; }

        public bool IsSROrDRLogin { get; set; }
   }
}
