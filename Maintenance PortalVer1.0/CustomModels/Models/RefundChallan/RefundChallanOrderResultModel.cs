using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.RefundChallan
{
    public class RefundChallanOrderResultModel
    {
        public string ErrorMessage { get; set; }
        public string ResponseMessage { get; set; }
        public int OrderID { get; set; }
    }
}
