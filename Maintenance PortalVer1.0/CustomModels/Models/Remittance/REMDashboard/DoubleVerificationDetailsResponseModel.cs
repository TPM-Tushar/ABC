using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.REMDashboard
{
    public class DoubleVerificationDetailsResponseModel
    {                                                        
        public string ID { get; set; }                       
        public string ChallanRefNumber { get; set; }        
        public string BankTransactionNumber { get; set; }    
        public string BankName { get; set; }                 
        public string PaymentMode { get; set; }              
        public string PaymentStatusCode { get; set; }            
        public string PaidAmount { get; set; }               
        public string TransactionTimeStamp { get; set; }     
        public string UserID { get; set; }                   
        public string IPAdd { get; set; }                    
        public string TransactionID { get; set; }            
        public string ServiceStatusCode { get; set; }        
        public string ServiceStatusDesc { get; set; }        
        public string SchedulerID { get; set; }
        public string InsertDateTime { get; set; }



    }
}
