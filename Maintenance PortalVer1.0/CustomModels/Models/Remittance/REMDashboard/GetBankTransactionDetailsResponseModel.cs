using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomModels.Models.Remittance.REMDashboard
{
    //Created By Raman Kaleaonkar on 15/03/2019
    public class BankTransactionDetailsResponseModel
    {

        public string TransactionID { get; set; }
        public string DateOfUpdate { get; set; }
        public string DocumentID { get; set; }
        public string DROCode { get; set; }
        public string InstrumentBankIFSCCode { get; set; }
        public string InstrumentBankMICRCode { get; set; }
        public string InstrumentBankName { get; set; }
        public string InstrumentDate { get; set; }
        public string InstrumentNumber { get; set; }
        public string IsDRO { get; set; }
        public string IsReceipt { get; set; }
        public string ReceiptID { get; set; }
        public string ReceiptNumber { get; set; }
        public string ReceiptPaymentMode { get; set; }
        public string Receipt_StampDate { get; set; }
        public string SourceOfReceipt { get; set; }
        public string SROCode { get; set; }
        public string StampDetailsID { get; set; }
        public string StampTypeID { get; set; }
        public string TotalAmount { get; set; }
        public string ServiceName { get; set; }

        public string EncryptedID { get; set; }

        public string InsertDateTime { get; set; }
        
        public string TransactionIDRedirectBtn { get; set; }

    }
}