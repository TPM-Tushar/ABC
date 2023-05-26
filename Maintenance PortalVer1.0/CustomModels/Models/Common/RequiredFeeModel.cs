using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Common
{
    public class RequiredFeeModel
    {
        public String EncryptedFeeID { get; set; }
        public Int64 FirmID { get; set; }
        public Int16 OfficeID { get; set; }
        public Int16 FeeRuleID { get; set; }
        public string FeeRuleIdDescription { get; set; }

        public Decimal ApplicableAmount { get; set; }
        public Decimal ExemptedAmount { get; set; }
        public Decimal PayableAmount { get; set; }
        public Decimal PaidAmount { get; set; }
        public DateTime DateOfPayment { get; set; }
        public Int64 UserID { get; set; }
        public Int32 ResourceID { get; set; }
        public short ProcessID { get; set; }
        public string AmountInWords { get; set; }
    }
}
