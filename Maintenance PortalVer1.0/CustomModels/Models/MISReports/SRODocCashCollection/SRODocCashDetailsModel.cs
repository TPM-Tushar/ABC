using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.SRODocCashCollection
{
    public class SRODocCashDetailsModel
    {

        public long SrNo { get; set; }

        public string  SROName { get; set; }

        public string DocumentNumber { get; set; }
        public string FinalRegistrationNumber { get; set; }
        public string PresentDatetime { get; set; }
        public decimal StampDuty { get; set; }
        public decimal ReceiptFee { get; set; }
        public string ReceiptNumber { get; set; }
        public decimal Total { get; set; }
        public decimal TotalSum { get; set; }




    }
}
