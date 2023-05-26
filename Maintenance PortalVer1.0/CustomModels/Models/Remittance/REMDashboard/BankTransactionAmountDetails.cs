using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.REMDashboard
{
    //Created by Raman Kalegaonkar on 29-03-2019
   public class BankTransactionAmountDetailsResponseModel
    {
        public string ID { get; set; }
        public string TransactionID { get; set; }
        public string Amount { get; set; }
        public string FeesRuleCode { get; set; }
        public string SROCode { get; set; }
        public string DeptSubPurpooseID { get; set; }
        public string InsertDateTime { get; set; }

    }
}
