using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.REMDashboard
{
    public class ChallanMatrixTransactionDetailsResponseModel
    {
        public string ChallanReqID { get; set; }
        public string TransactionDateTime { get; set; }
        public string SroCode { get; set; }
        public string DDOCode { get; set; }
        public string RemittanceBankName { get; set; }
        public string StatusDesc { get; set; }

        public string ReceiptDate { get; set; }
        public string UIRNumber { get; set; }
        public string TransactionStatus { get; set; }
        public string StatusCode { get; set; }
        public string UserID { get; set; }
        public string IPAddress { get; set; }
        public string BatchID { get; set; }
        public string FirstPrintDate { get; set; }
        public string ReqPaymentMode { get; set; }
        public string IsDro { get; set; }
        public string DroCode { get; set; }
        public string SchedulerID { get; set; }

        public string InsertDateTime { get; set; }


    }
}
