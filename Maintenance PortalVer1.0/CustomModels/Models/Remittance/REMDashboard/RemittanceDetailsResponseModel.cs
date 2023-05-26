using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomModels.Models.Remittance.REMDashboard
{
    public class RemittanceDetailsResponseModel
    {

        public string UserID { get; set; }                                                               
        public string DDOCode { get; set; }                                                 
        public string DeptReferenceCode { get; set; }                             
        public string ID { get; set; }                                            
        public string IPAdd { get; set; }                                         
        public string PaymentStatusCode { get; set; }                             
        public string RemitterName { get; set; }                                  
        public string StatusCode { get; set; }                                    
        public string StatusDesc { get; set; }                                    
        public string TransactionDateTime { get; set; }                           
        public string TransactionID { get; set; }                                 
        public string TransactionStatus { get; set; }                             
        public string UIRNumber { get; set; }

        public string InsertDateTime { get; set; }



    }
}
