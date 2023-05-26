using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.MISReports.SroDD_POCollection
{
   public  class SroDD_POCollectionDetailsModel
    {
        public long SrNo { get; set; }

        public string DocumentNumber { get; set; }
        public string ReceiptNumber { get; set; }

        public string PresentDatetime { get; set; }
        public string DDChalNumber { get; set; }

        public decimal StampDuty { get; set; }
        public decimal RegistrationFee { get; set; }


        public decimal DDAmount { get; set; }

        public string SroName { get; set; }





    }
}
