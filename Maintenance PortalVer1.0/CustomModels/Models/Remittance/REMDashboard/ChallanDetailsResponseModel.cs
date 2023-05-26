using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.REMDashboard
{
    public class ChallanDetailsResponseModel
    {
        public string BatchID { get; set; }
        public string CardType { get; set; }
        public string CCNumber { get; set; }
        public string ChallanAmount { get; set; }
        public string ChallanDate { get; set; }
        public string ChallanExpiryDate { get; set; }
        public string ChallanID { get; set; }
        public string ChallanRefNum { get; set; }
        public string ChallanRequestID { get; set; }
        public string ChallanTotalAmount { get; set; }
        public string DepartmentRefNumber { get; set; }
        public string InstrmntDate { get; set; }
        public string InstrmntNumber { get; set; }
        public string MICRCode { get; set; }
        public string PaymentMode { get; set; }
        public string RemitterName { get; set; }
        public string RmtncAgencyBank { get; set; }
        public string InsertDateTime { get; set; }


    }
}